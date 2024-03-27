using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Globalization;

public class JsonParameterParser
{
    public static Dictionary<string, object?> ParseJsonToDictionary(JsonElement jsonElement, Dictionary<string, Type> parameters)
    {
        var result = new Dictionary<string, object?>();

        foreach (var parameter in parameters)
        {
            var paramName = parameter.Key;
            var paramType = parameter.Value;

            if (jsonElement.TryGetProperty(paramName!, out JsonElement element))
            {
                object? value = ConvertJsonElementToType(element, paramType);
                if (value != null)
                {
                    result.Add(paramName!, value);
                }
            }
        }

        return result;
    }

    private static object? ConvertJsonElementToType(JsonElement element, Type targetType)
    {
        try
        {
            // Handling string to bool conversion
            if (targetType == typeof(bool) && element.ValueKind == JsonValueKind.String)
            {
                var stringValue = element.GetString();
                if (bool.TryParse(stringValue, out bool boolValue))
                {
                    return boolValue;
                }
                else
                {
                    throw new InvalidCastException($"Cannot convert string '{stringValue}' to a boolean value.");
                }
            }
            // Handling string to numeric conversion
            else if (targetType.IsNumericType() && element.ValueKind == JsonValueKind.String)
            {
                var stringValue = element.GetString();
                return Convert.ChangeType(stringValue, targetType, CultureInfo.InvariantCulture);
            }
            // Handling numeric to string conversion
            else if (targetType == typeof(string) && (element.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False))
            {
                return element.GetRawText();
            }
            // Basic and numeric types
            else if (element.TryGetCommonType(targetType, out object? commonValue))
            {
                return commonValue;
            }
            // Complex object or value type
            else
            {
                return JsonSerializer.Deserialize(element.GetRawText(), targetType);
            }
        }
        catch
        {
            return null;
        }
    }
}

public static class JsonElementExtensions
{
    public static bool TryGetCommonType(this JsonElement element, Type targetType, out object? value)
    {
        value = null;

        if (element.ValueKind == JsonValueKind.Null)
            return true;

        try
        {
            if (targetType == typeof(int) && element.ValueKind == JsonValueKind.Number)
                value = element.GetInt32();
            else if (targetType == typeof(long) && element.ValueKind == JsonValueKind.Number)
                value = element.GetInt64();
            else if (targetType == typeof(float) && element.ValueKind == JsonValueKind.Number)
                value = element.GetSingle();
            else if (targetType == typeof(double) && element.ValueKind == JsonValueKind.Number)
                value = element.GetDouble();
            else if (targetType == typeof(decimal) && element.ValueKind == JsonValueKind.Number)
                value = element.GetDecimal();
            else if (targetType == typeof(bool) && (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False))
                value = element.GetBoolean();
            else if (targetType == typeof(string) && element.ValueKind == JsonValueKind.String)
                value = element.GetString();
            else if (targetType == typeof(DateTime) && element.ValueKind == JsonValueKind.String)
                value = DateTime.Parse(element.GetString() ?? "", CultureInfo.InvariantCulture);
            else if (targetType == typeof(Guid) && element.ValueKind == JsonValueKind.String)
                value = Guid.Parse(element.GetString() ?? "");
            else
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsNumericType(this Type type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Byte or TypeCode.SByte
            or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64
            or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64
            or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
            _ => false
        };
    }
}