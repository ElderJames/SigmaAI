using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma.Core.OutputParsers
{
    internal sealed class TypeParser
    {
        private static readonly Dictionary<Type, string> typeMappings = new Dictionary<Type, string>
        {
            {typeof(int), "number"},
            {typeof(long), "number"},
            {typeof(short), "number"},
            {typeof(float), "number"},
            {typeof(double), "number"},
            {typeof(decimal), "number"},
            {typeof(string), "string"},
            {typeof(bool), "boolean"},
            {typeof(object), "object"},
            {typeof(DateTime), "Date"},
            {typeof(void), "void"}
        };

        public static string ConvertToTypeScriptType(Type type)
        {
            if (type.IsArray)
            {
                Type? elementType = type.GetElementType();
                string tsElementType = ConvertSingleTypeToTypeScript(elementType!);
                return $"{tsElementType}[]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type itemType = type.GetGenericArguments()[0];
                string tsItemType = ConvertSingleTypeToTypeScript(itemType);
                return $"{tsItemType}[]";
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = type.GetGenericArguments()[0];
                Type valueType = type.GetGenericArguments()[1];
                string tsKeyType = ConvertSingleTypeToTypeScript(keyType);
                string tsValueType = ConvertSingleTypeToTypeScript(valueType);
                // Assuming all keys are strings in TypeScript object
                if (tsKeyType != "string")
                {
                    tsKeyType = "string";
                }
                return $"{{ [key: {tsKeyType}]: {tsValueType}; }}";
            }
            else
            {
                return ConvertSingleTypeToTypeScript(type);
            }
        }

        public static bool IsArrayOrList(Type type)
        {
            return type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
        }

        private static string ConvertSingleTypeToTypeScript(Type type)
        {
            if (typeMappings.TryGetValue(type, out var tsType))
            {
                return tsType;
            }
            else
            {
                return "any";
            }
        }
    }
}
