using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

#pragma warning disable CS8600, CS8601, CS8602, CS8603, CS8604, CS8618

namespace LLMJson
{
    // Really simple JSON parser in ~300 lines
    // - Attempts to parse JSON files with minimal GC allocation
    // - Nice and simple "[1,2,3]".FromJson<List<int>>() API
    // - Classes and structs can be parsed too!
    //      class Foo { public int Value; }
    //      "{\"Value\":10}".FromJson<Foo>()
    // - Can parse JSON without type information into Dictionary<string,object> and List<object> e.g.
    //      "[1,2,3]".FromJson<object>().GetType() == typeof(List<object>)
    //      "{\"Value\":10}".FromJson<object>().GetType() == typeof(Dictionary<string,object>)
    // - No JIT Emit support to support AOT compilation on iOS
    // - Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns null instead.
    // - Only public fields and property setters on classes/structs will be written to
    //
    // Limitations:
    // - No JIT Emit support to parse structures quickly
    // - Limited to parsing <2GB JSON files (due to int.MaxValue)
    // - Parsing of abstract classes or interfaces is NOT supported and will throw an exception.
    public static class JsonParser
    {

        public static bool UseRepair     { get; set; } = true;
        public static bool UseRecognizer { get; set; } = true;


        [ThreadStatic] static Stack<List<string>> splitArrayPool;
        [ThreadStatic] static StringBuilder stringBuilder;
        [ThreadStatic] static Dictionary<Type, Dictionary<string, FieldInfo>> fieldInfoCache;
        [ThreadStatic] static Dictionary<Type, Dictionary<string, PropertyInfo>> propertyInfoCache;
        private static object? _baseObject;
        private static bool _isbase;

        //public static T FromJson<T>(this string json, T? baseObject) where T : new()
        //{
        //    _baseObject = baseObject;
        //    _isbase = true;
        //    return json.FromJson<T>();
        //}

        public static T FromJson<T>(this string json, T baseObject) //where T : new()
        {
            _baseObject = baseObject;
            _isbase = true;
            JsonRepair.Context = JsonRepair.InputType.LLM;
            if (UseRepair) { try { json = JsonRepair.RepairJson(json); } catch (Exception) { /* cleaning failed */ } }
            // Initialize, if needed, the ThreadStatic variables
            if (propertyInfoCache == null) propertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            if (fieldInfoCache == null) fieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            if (stringBuilder == null) stringBuilder = new StringBuilder();
            if (splitArrayPool == null) splitArrayPool = new Stack<List<string>>();

            //Remove all whitespace not within strings to make parsing simpler
            stringBuilder.Length = 0;
            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];
                if (c == '"')
                {
                    i = AppendUntilStringEnd(true, i, json);
                    continue;
                }
                if (char.IsWhiteSpace(c))
                    continue;

                stringBuilder.Append(c);
            }

            //Parse the thing!
            return (T)ParseValue(typeof(T), stringBuilder.ToString());
        }

        static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json)
        {
            stringBuilder.Append(json[startIdx]);
            for (int i = startIdx + 1; i < json.Length; i++)
            {
                if (json[i] == '\\')
                {
                    if (appendEscapeCharacter)
                        stringBuilder.Append(json[i]);
                    stringBuilder.Append(json[i + 1]);
                    i++;//Skip next character as it is escaped
                }
                else if (json[i] == '"')
                {
                    stringBuilder.Append(json[i]);
                    return i;
                }
                else
                    stringBuilder.Append(json[i]);
            }
            return json.Length - 1;
        }

        //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
        static List<string> Split(string json)
        {
            List<string> splitArray = splitArrayPool.Count > 0 ? splitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if (json.Length == 2)
                return splitArray;
            int parseDepth = 0;
            stringBuilder.Length = 0;
            for (int i = 1; i < json.Length - 1; i++)
            {
                switch (json[i])
                {
                    case '[':
                    case '{':
                        parseDepth++;
                        break;
                    case ']':
                    case '}':
                        parseDepth--;
                        break;
                    case '"':
                        i = AppendUntilStringEnd(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0)
                        {
                            splitArray.Add(stringBuilder.ToString());
                            stringBuilder.Length = 0;
                            continue;
                        }
                        break;
                }

                stringBuilder.Append(json[i]);
            }

            splitArray.Add(stringBuilder.ToString());

            return splitArray;
        }

        private static object? ParseValueJsonProp(object item, Type type, string json)
        { 

            // If immutable, do not update the value
            type.GetProperty("UpdateState")?.SetValue(item, UpdateStates.Unchanged);
            bool Immutable = (bool)        (type.GetProperty("Immutable")  ?.GetValue(item) ?? false);if (Immutable) {return item; }

            // Now update the inner value

            // Check if the JsonProp has its own setter (converter from string to the internal type
            bool updateSucces = false;
            bool _hasSetter = (bool)(type.GetProperty("HasSetter")?.GetValue(item) ?? false);
            if (_hasSetter)
            {
                // Has custom setter. Now call JsonProp<>.FromString to use it
                MethodInfo? methodInfo = type.GetMethod("FromString");

                if (methodInfo != null) {
                    var returnValue = methodInfo.Invoke(item, new object[] { json });
                    if (returnValue != null) { updateSucces = (bool)returnValue; }
                }
            }
            else
            {
                Type internalType = type.GetGenericArguments()[0];
                var internalValue = ParseValue(internalType, json);
                if (internalValue != null)
                {
                    type.GetProperty("Value")?.SetValue(item, internalValue);
                    updateSucces = true;
                }
            }

            type.GetProperty("UpdateState")?.SetValue(item, updateSucces? UpdateStates.Updated: UpdateStates.InvalidUpdate); 
            

            return item;
        }



        internal static object? ParseValue(Type type, string json)
        {
            if (type == typeof(string))
            {
                if (json.Length <= 2)
                    return string.Empty;
                StringBuilder parseStringBuilder = new StringBuilder(json.Length);
                for (int i = 1; i < json.Length - 1; ++i)
                {
                    if (json[i] == '\\' && i + 1 < json.Length - 1)
                    {
                        int j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                        if (j >= 0)
                        {
                            parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                            ++i;
                            continue;
                        }
                        if (json[i + 1] == 'u' && i + 5 < json.Length - 1)
                        {
                            uint c = 0;
                            if (uint.TryParse(json.Substring(i + 2, 4), System.Globalization.NumberStyles.AllowHexSpecifier, null, out c))
                            {
                                parseStringBuilder.Append((char)c);
                                i += 5;
                                continue;
                            }
                        }
                    }
                    parseStringBuilder.Append(json[i]);
                }
                return parseStringBuilder.ToString();
            }
            if (SafeParseUtils.IsPrimitiveInteger(type))
            {
                return SafeParseUtils.GetSafeInteger(type, json, UseRecognizer);
            }
            if (SafeParseUtils.IsPrimitiveFloat(type))
            {
                return SafeParseUtils.GetSafeFloatingPoint(type, json,UseRecognizer);
            }
            if (type == typeof(decimal))
            {
                return SafeParseUtils.GetSafeFloatingPoint(type, json, UseRecognizer);
            }
            if (type == typeof(DateTime))
            {
                return SafeParseUtils.GetSafeDateTime(type, json, UseRecognizer);
            }
            else if (type.IsPrimitive)
            {
                //return SafeParseUtils.GetSafeFloatingPoint(type, json, UseRecognizer);
                var result = Convert.ChangeType(json, type, System.Globalization.CultureInfo.InvariantCulture);
                return result;
            }
            if (json.ToLower() == "null")
            {
                return null;
            }
            if (type.IsEnum)
            {
                if (json[0] == '"')
                    json = json.Substring(1, json.Length - 2);
                try
                {
                    return Enum.Parse(type, json, false);
                }
                catch
                {
                    try { return Enum.Parse(type, json, true); } catch { return 0; }
                }
            }
            if (type.IsArray)
            {
                Type arrayType = type.GetElementType();
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                Array? newArray = Array.CreateInstance(arrayType, elems.Count);
                for (int i = 0; i < elems.Count; i++)
                    newArray.SetValue(ParseValue(arrayType, elems[i]), i);
                splitArrayPool.Push(elems);
                return newArray;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type listType = type.GetGenericArguments()[0];
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                List<string> elems = Split(json);
                var list = (IList)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count });
                for (int i = 0; i < elems.Count; i++)
                    list.Add(ParseValue(listType, elems[i]));
                splitArrayPool.Push(elems);
                return list;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType, valueType;
                {
                    Type[] args = type.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }

                //Refuse to parse dictionary keys that aren't of type string
                if (keyType != typeof(string))
                    return null;
                //Must be a valid dictionary element
                if (json[0] != '{' || json[json.Length - 1] != '}')
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;

                var dictionary = (IDictionary)type.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { elems.Count / 2 });
                for (int i = 0; i < elems.Count; i += 2)
                {
                    if (elems[i].Length <= 2)
                        continue;
                    string keyValue = elems[i].Substring(1, elems[i].Length - 2);
                    object? val = ParseValue(valueType, elems[i + 1]);
                    dictionary[keyValue] = val;
                }
                return dictionary;
            }
            if (type == typeof(object))
            {
                return ParseAnonymousValue(json);
            }
            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                return ParseObject(type, json);
            }

            if (_isbase && _baseObject != null)
            {
                _isbase = false;
                return _baseObject;
            }

            return null;
        }

        static object? ParseAnonymousValue(string json)
        {
            if (json.Length == 0)
                return null;
            if (json[0] == '{' && json[json.Length - 1] == '}')
            {
                List<string> elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (int i = 0; i < elems.Count; i += 2)
                    dict[elems[i].Substring(1, elems[i].Length - 2)] = ParseAnonymousValue(elems[i + 1]);
                return dict;
            }
            if (json[0] == '[' && json[json.Length - 1] == ']')
            {
                List<string> items = Split(json);
                var finalList = new List<object>(items.Count);
                for (int i = 0; i < items.Count; i++)
                    finalList.Add(ParseAnonymousValue(items[i]));
                return finalList;
            }
            if (json[0] == '"' && json[json.Length - 1] == '"')
            {
                string str = json.Substring(1, json.Length - 2);
                return str.Replace("\\", string.Empty);
            }
            if (char.IsDigit(json[0]) || json[0] == '-')
            {
                if (json.Contains("."))
                {
                    return SafeParseUtils.GetSafeFloatingPoint(typeof(double), json);
                }
                else
                {
                    int result;
                    int.TryParse(json, out result);
                    return result;
                }
            }
            if (json.ToLower() == "true")
                return true;
            if (json.ToLower() == "false")
                return false;
            // handles json == "null" as well as invalid JSON
            return null;
        }

        static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo
        {
            Dictionary<string, T> nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < members.Length; i++)
            {
                T member = members[i];
                if (member.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                string name = member.Name;
                if (member.IsDefined(typeof(DataMemberAttribute), true))
                {
                    DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        name = dataMemberAttribute.Name;
                }

                nameToMember.Add(name, member);
            }

            return nameToMember;
        }

        static object? ParseObject(Type type, string json)
        {
            object? instance;
            if (_isbase && _baseObject != null)
            {
                instance = _baseObject;
                _isbase = false;
            }
            else
            {
#if NET5_0_OR_GREATER
                instance = System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(type);
#else
                instance = FormatterServices.GetUninitializedObject(type);
#endif
            }

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            List<string> elems = Split(json);
            if (elems.Count % 2 != 0)
                return instance;

            Dictionary<string, FieldInfo> nameToField;
            Dictionary<string, PropertyInfo> nameToProperty;
            if (!fieldInfoCache.TryGetValue(type, out nameToField))
            {
                nameToField = CreateMemberNameDictionary(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                fieldInfoCache.Add(type, nameToField);
            }
            if (!propertyInfoCache.TryGetValue(type, out nameToProperty))
            {
                nameToProperty = CreateMemberNameDictionary(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                propertyInfoCache.Add(type, nameToProperty);
            }

            for (int i = 0; i < elems.Count; i += 2)
            {
                if (elems[i].Length <= 2)
                    continue;
                string key = elems[i].Substring(1, elems[i].Length - 2);
                string value = elems[i + 1];

                FieldInfo fieldInfo;
                PropertyInfo propertyInfo;
                if (nameToField.TryGetValue(key, out fieldInfo))
                    fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
                else if (nameToProperty.TryGetValue(key, out propertyInfo))
                {
                    // check if this is a special JsonProp<T> type
                    var isJsonProp  = false;
                    Type propType = propertyInfo.PropertyType;
                    isJsonProp   = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(JsonProp<>);
                    if (!isJsonProp)
                    {
                        // If not, it may be a derived class from property type. Let's check
                        var  baseType = propertyInfo.PropertyType.BaseType;
                        if (baseType != null)
                        {
                            // If so use this a type for setting values
                            isJsonProp = baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(JsonProp<>);
                            if (isJsonProp) { propType = baseType; }
                        }
                    }

                    if (isJsonProp)
                    {
                        // Find the instance of the inner type of the JsonProp
                        var instanceType = instance.GetType();  
                        var propertyInfoJsonProp = instanceType.GetProperty(propertyInfo.Name);
                        if (propertyInfoJsonProp!=null)
                        {
                            object? instanceJsonProp = propertyInfoJsonProp.GetValue(instance);
                            if (instanceJsonProp != null)
                            {
                                propertyInfo.SetValue(instance, ParseValueJsonProp(instanceJsonProp, propType, value), null);
                            }
                        }
                    } else
                    {
                        propertyInfo.SetValue(instance, ParseValue(propType, value), null);
                    }
                }
            }

            return instance;
        }


    }
}