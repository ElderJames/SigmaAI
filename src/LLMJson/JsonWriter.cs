using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

#pragma warning disable CS8602

namespace LLMJson;


// Based on https://github.com/zanders3/json/blob/master/src/JsonWriter.cs

//Really simple JSON writer
//- Outputs JSON structures from an object
//- Really simple API (new List<int> { 1, 2, 3 }).ToJson() == "[1,2,3]"
//- Will only output public fields and property getters on objects


// idea. Make property objects that
// cast to their base function
// to string gives value
// has value field
// has getter & setter callbacks
// has immutable tag
// has visible tag
// has description tag. (automatically add immutable comment)
// Inotify changed implementation
// indication of: updated with new value, updated with old value, not updates


public static class JsonWriter
{
    private static OutputModes  _outputMode;
    private static CreateField? _createField;

    public delegate string CreateField(string value, string type, string description);


    public static string ToJson(this object item, OutputModes outputMode = OutputModes.Value, CreateField? createField = null)
    {
        _outputMode  = outputMode;
        _createField = createField;
        StringBuilder stringBuilder = new StringBuilder();
        AppendValue(stringBuilder, item, "");
        return stringBuilder.ToString();
    }

    static bool AppendValue(StringBuilder stringBuilder, object? item, string description = "", bool addTypeDescription=true)
    {
        StringBuilder valueStringBuilder = new StringBuilder();        
        //var valueTypeString = "";

        if (item == null)
        {
            valueStringBuilder.Append("null");
            //valueTypeString = "is non-existent. Ignore";
            //CreateEntry(stringBuilder,valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
            return false;
        }
        Type type = item.GetType();

        if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JsonProp<>))
        {
            // Use reflection to find the type argument of the derived class
            bool Visible             = (bool)        (type.GetProperty("Visible")    ?.GetValue(item) ?? default(bool)); if (!Visible) return false;
            //bool Immutable           = (bool)        (type.GetProperty("Immutable")  ?.GetValue(item) ?? default(bool)); 
            string newDescription    =                type.GetProperty("Description")?.GetValue(item) ?.ToString()??"";
            object? value            =                type.GetProperty("Value")      ?.GetValue(item);
            //UpdateStates UpdateState = (UpdateStates)(type.GetProperty("UpdateState")?.GetValue(item) ?? default(UpdateStates));
                        
            AppendValue(stringBuilder, value, string.IsNullOrEmpty(newDescription)?description:newDescription, true);
        }
        else 
        if (type == typeof(string) || type == typeof(char))
        {
            valueStringBuilder.Append('"');
            var str = item.ToString();
            for (int i = 0; i < str?.Length; ++i)
            {
                if (str[i] < ' ' || str[i] == '"' || str[i] == '\\')
                {
                    valueStringBuilder.Append('\\');
                    int j = "\"\\\n\r\t\b\f".IndexOf(str[i]);
                    if (j >= 0)
                        valueStringBuilder.Append("\"\\nrtbf"[j]);
                    else
                        valueStringBuilder.AppendFormat("u{0:X4}", (UInt32)str[i]);
                }
                else
                {
                    valueStringBuilder.Append(str[i]);
                }
            }

            valueStringBuilder.Append('"');
            //valueTypeString = "string";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(byte) )
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "8-bit unsigned integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if ( type == typeof(sbyte))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "8-bit integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(short))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "16-bit integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(ushort))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "16-bit unsigned integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(int))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "32-bit integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(uint))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "32-bit unsigned integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(long))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "64-bit integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if ( type == typeof(ulong))
        {
            valueStringBuilder.Append(item.ToString());
            //valueTypeString = "64-bit unsigned integer";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);

        }
        else if (type == typeof(float))
        {
            valueStringBuilder.Append(((float)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            //valueTypeString = "floating point";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(double))
        {
            valueStringBuilder.Append(((double)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            //valueTypeString = "floating point";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);

        }
        else if (type == typeof(decimal))
        {
            valueStringBuilder.Append(((decimal)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            //valueTypeString = "floating point";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(bool))
        {
            valueStringBuilder.Append(((bool)item) ? "true" : "false");
            //valueTypeString = "bool";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type == typeof(DateTime))
        {
            valueStringBuilder.Append('"');
            valueStringBuilder.Append(((DateTime)item).ToString(System.Globalization.CultureInfo.InvariantCulture));
            valueStringBuilder.Append('"');
            //valueTypeString = $"date and time in format {System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.FullDateTimePattern} {description}";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (type.IsEnum)
        {
            valueStringBuilder.Append('"');
            valueStringBuilder.Append(item.ToString());
            valueStringBuilder.Append('"');
            //var enumList = String.Join(",", Enum.GetNames(item.GetType()));
            //valueTypeString = $"enum. Possible values are {enumList}";
            CreateEntry(stringBuilder, valueStringBuilder, addTypeDescription?GetValueTypeString(item):"", description);
        }
        else if (item is IList)
        {
            Type valueType = type.GetGenericArguments()[0];
            // ** type is list
            stringBuilder.Append('[');
            //object? typeValue = null; // value created to be used for type
            if (_outputMode != OutputModes.Description)
            {
                bool isFirst = true;
                IList? list = item as IList;
                for (int i = 0; i < list?.Count; i++)
                {
                   // typeValue = list[i];
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    AppendValue(stringBuilder, list[i],"",false);
                }
            }
            //else
            //{
            //    if (item is IList list && list.Count > 0) typeValue = list[0];
            //}

            //Type typeValue = ((item as IList)!).GetType().GetGenericArguments().Single();

            CreateEndList(stringBuilder, GetTypeString(valueType),description);
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            // ** Type is dictionary
            Type keyType   = type.GetGenericArguments()[0];
            Type valueType = type.GetGenericArguments()[1];

            //Refuse to output dictionary keys that aren't of type string
            if (keyType != typeof(string))
            {
                stringBuilder.Append("{}");
                return true;
            }

            stringBuilder.Append('{');
            object? typeValue = null; // value created to be used for type
            if (_outputMode != OutputModes.Description)
            {
                bool isFirst = true;
                if (item is IDictionary dict) foreach (object key in dict.Keys)
                {
                    //typeValue = dict[key];
                    if (isFirst)
                        isFirst = false;
                    else
                        stringBuilder.Append(',');
                    stringBuilder.Append('\"');
                    stringBuilder.Append((string)key);
                    stringBuilder.Append("\":");
                    AppendValue(stringBuilder, dict[key],"", false);
                }
            }
            //else
            //{
            //    if (item is IDictionary dict) foreach (object key in dict.Keys)
            //    {
            //        typeValue = dict[key];
            //        break;
            //    }
            //}

            //typeValue = ((item as IDictionary)!).GetType().GetGenericArguments()[1];

            CreateEndDictionary(stringBuilder, GetTypeString(valueType), description);
            //stringBuilder.Append('}');
        }
        else
        {
            // ** type is object (may be root)
            stringBuilder.Append('{');

            bool isFirst = true;
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (fieldInfos[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                object? value = fieldInfos[i].GetValue(item);
                if (value != null)
                {
                    var fieldStringBuilder = new StringBuilder();
                    // Get description attribute
                    object[] attributes = fieldInfos[i].GetCustomAttributes(typeof(DescriptionAttribute), true);
                    var fieldDescription = "";
                    if (attributes.Length > 0)
                    {
                        DescriptionAttribute descriptionAttribute = (DescriptionAttribute)attributes[0];
                        fieldDescription = descriptionAttribute.Description;
                    }
                    if (isFirst)
                        isFirst = false;
                    else
                        fieldStringBuilder.Append(',');
                    fieldStringBuilder.Append('\"');
                    fieldStringBuilder.Append(GetMemberName(fieldInfos[i]));
                    fieldStringBuilder.Append("\":");
                    var succes = AppendValue(fieldStringBuilder, value, fieldDescription);
                    if (succes) stringBuilder.Append(fieldStringBuilder.ToString());
                }
            }
            PropertyInfo[] propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                if (!propertyInfo[i].CanRead || propertyInfo[i].IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                object? value = propertyInfo[i].GetValue(item, null);
                if (value != null)
                {
                    var propertyStringBuilder = new StringBuilder();
                    // Get description attribute
                    object[] attributes = propertyInfo[i].GetCustomAttributes(typeof(DescriptionAttribute), true);
                    var fieldDescription = "";
                    if (attributes.Length > 0)
                    {
                        DescriptionAttribute descriptionAttribute = (DescriptionAttribute)attributes[0];
                        fieldDescription = descriptionAttribute.Description;
                    }
                    if (isFirst)
                        isFirst = false;
                    else
                    {
                        propertyStringBuilder.Append(',');
                    }
                    propertyStringBuilder.Append('\"');
                    propertyStringBuilder.Append(GetMemberName(propertyInfo[i]));
                    propertyStringBuilder.Append("\":");
                    var succes = AppendValue(propertyStringBuilder, value, fieldDescription);
                    if (succes) stringBuilder.Append(propertyStringBuilder.ToString());
                }
            }

            stringBuilder.Append('}');
        }

        return true;
    }

    static string GetTypeString(Type type)
    {
        if (type == typeof(string)
         || type == typeof(char    )) return "string";
        if (type == typeof(byte    )) return "8-bit unsigned integer";
        if (type == typeof(sbyte   )) return "8-bit integer";
        if (type == typeof(short   )) return "16-bit integer";
        if (type == typeof(ushort  )) return "16-bit unsigned integer";
        if (type == typeof(int     )) return "32-bit integer";
        if (type == typeof(uint    )) return "32-bit unsigned integer";
        if (type == typeof(long    )) return "64-bit integer";
        if (type == typeof(ulong   )) return "64-bit unsigned integer";
        if (type == typeof(float)
         || type == typeof(double)
         || type == typeof(decimal )) return "floating point";
        if (type == typeof(bool    )) return "bool";
        if (type == typeof(DateTime)) return $"date and time in format {System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.FullDateTimePattern}";
        if (type.IsEnum) return $"enum. Possible values are {String.Join(",", Enum.GetNames(type))}";

        return "";
    }


    static string GetValueTypeString(object? item)
    {
        if (item == null           ) return "is non-existent. Ignore";
        Type type = item.GetType();
        if (type == typeof(string  ) 
         || type == typeof(char    )) return "string";
        if (type == typeof(byte    )) return "8-bit unsigned integer";
        if (type == typeof(sbyte   )) return "8-bit integer";
        if (type == typeof(short   )) return "16-bit integer";
        if (type == typeof(ushort  )) return "16-bit unsigned integer";
        if (type == typeof(int     )) return "32-bit integer";
        if (type == typeof(uint    )) return "32-bit unsigned integer";
        if (type == typeof(long    )) return "64-bit integer";
        if (type == typeof(ulong   )) return "64-bit unsigned integer";
        if (type == typeof(float   ) 
         || type == typeof(double  ) 
         || type == typeof(decimal )) return "floating point";
        if (type == typeof(bool    )) return "bool";
        if (type == typeof(DateTime)) return $"date and time in format {System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.FullDateTimePattern}";
        if (type.IsEnum            )  return $"enum. Possible values are {String.Join(",", Enum.GetNames(item.GetType()))}";
        
        return "";
    }

    private static void CreateEndDictionary(StringBuilder stringBuilder, string valueTypeString, string description)
    
    {
        switch (_outputMode)
        {
            default:
            case OutputModes.Value:
                stringBuilder.Append('}');
                break;
            case OutputModes.Custom:
            case OutputModes.Description:
            case OutputModes.ValueAndDescription:
                stringBuilder.Append($"}}{" \\\\ ".IfBothNotEmpty(description.IfBothNotEmpty(". ") + $"Is of type Dictionary. The key is of type string, the value of {valueTypeString}.")}\n");
                break;
        }
    }

    private static void CreateEndList(StringBuilder stringBuilder, string valueTypeString, string description)
    {
        switch (_outputMode)
        {
            default:
            case OutputModes.Value:
                stringBuilder.Append(']');
                break;
            case OutputModes.Custom:
            case OutputModes.Description:
            case OutputModes.ValueAndDescription:
                stringBuilder.Append($"]{" \\\\ ".IfBothNotEmpty(description.IfBothNotEmpty(". ") + $"Is of type List, items are of type {valueTypeString}")}.\n");
                break;
        }
    }


    public static string GetPrompt()
    {
        switch (_outputMode)
        {
            default:
            case OutputModes.Description:
                return "Fill in the described format below. Provide a RFC8259 compliant JSON response, following this format without deviation, but values may be changed, lists and dictionaries may change in length. Add additional explanation of the changes made after the json structure. ";
            case OutputModes.Value:
                return "Update the data in the described format below. Provide a RFC8259 compliant JSON response, following this format without deviation, but values may be changed, lists and dictionaries may change in length. Add additional explanation of the changes made after the json structure.";
            case OutputModes.Custom:
            case OutputModes.ValueAndDescription:
                return "Update the data in the described format below. Comment are added for your understanding, remove in updated response. provide a RFC8259 compliant JSON response, following this format without deviation, but values may be changed, lists and dictionaries may change in length. Add additional explanation of the changes made after the json structure.";
        }
    }

    private static void CreateEntry(StringBuilder stringBuilder, StringBuilder valueStringBuilder, string valueTypeString, string description)
    {
        var valueString = valueStringBuilder.ToString();
        switch (_outputMode)
        {
            default:
            case OutputModes.Value:
                stringBuilder.Append($"{valueString}\n");
                break;
            case OutputModes.Description:
                stringBuilder.Append($"\"{description.IfBothNotEmpty(". ")}{valueTypeString}\"\n");
                break;
            case OutputModes.ValueAndDescription:
                stringBuilder.Append($"{valueString}{" \\\\ ".IfBothNotEmpty(description.IfBothNotEmpty(". ")+"Is of type ".IfBothNotEmpty(valueTypeString))}.\n");
                break;
            case OutputModes.Custom:
                if (_createField!=null) stringBuilder.Append(_createField(valueString,valueTypeString,description));
                break;
        }
    }

    static string GetMemberName(MemberInfo member)
    {
        if (member.IsDefined(typeof(DataMemberAttribute), true))
        {
            DataMemberAttribute? dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true)!;
            if (!string.IsNullOrEmpty(dataMemberAttribute?.Name))
                return dataMemberAttribute.Name;
        }

        return member.Name;
    }

    //public static string GetAttribute(Type t)
    //{
    //    // Get instance of the attribute.
    //    //var descriptionAttribute = Attribute.GetCustomAttribute(t, typeof(DescriptionAttribute)) as DescriptionAttribute;


    //    var customAttributes = (DescriptionAttribute[])t.GetCustomAttributes(typeof(DescriptionAttribute), true);
    //    if (customAttributes.Length > 0)
    //    {
    //        var myAttribute = customAttributes[0];
    //        string value = myAttribute.Description;
    //        return value;
    //    }

    //    return "";
    //    //if (descriptionAttribute == null)
    //    //{
    //    //    return "";
    //    //}
    //    //else
    //    //{
    //    //    return descriptionAttribute.Description;
    //    //}

    //    PropertyInfo propertyInfo = typeof(Foo).GetProperty(propertyToCheck);
    //    object[] attribute = propertyInfo.GetCustomAttributes(typeof(MyCustomAttribute), true);
    //    if (attribute.Length > 0)
    //    {
    //        MyCustomAttribute myAttribute = (MyCustomAttribute)attribute[0];
    //        string propertyValue = myAttribute.SomeProperty;
    //    }
    //}

    public static string IfBothNotEmpty(this string lh, string rh)
    {
        if (string.IsNullOrEmpty(lh) || string.IsNullOrEmpty(rh)) return "";
        return lh + rh;
    }

}

public enum OutputModes
{
    Value,
    Description,
    ValueAndDescription,
    Custom
}

[AttributeUsage(AttributeTargets.All)]
public class DescriptionAttribute : Attribute
{
    // Private fields.
    private string _description;

    // This is a read/write attribute.

    public DescriptionAttribute(string description)
    {
        _description = description;
    }

    public virtual string Description
    {
        get { return _description; }
        set { _description = value; }
    }
}