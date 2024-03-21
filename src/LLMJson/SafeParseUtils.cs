using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;

namespace LLMJson
{
    public class SafeParseUtils
    {
        private static readonly Regex RegexFloatNumberLike    = new Regex(@"\d+(?:[,. ]\d+)*");
        private static readonly Regex RegexIntegerNumberLike  = new Regex(@"(?<![.\d])\d+(?![.\d])"); //@"\d+(?:\d+)*");
        private static readonly CultureInfo CommaDecSeparator = new CultureInfo("en") { NumberFormat = { NumberDecimalSeparator = "," } };

        public static bool IsPrimitiveInteger(Type type)
        {
            return type.IsPrimitive && (
                type == typeof(sbyte) || type == typeof(byte)   ||
                type == typeof(short) || type == typeof(ushort) ||
                type == typeof(int)   || type == typeof(uint)   ||
                type == typeof(long)  || type == typeof(ulong)
            );
        }

        public static bool IsPrimitiveFloat(Type type)
        {
            return type.IsPrimitive && (
                type == typeof(double) || type == typeof(Single)
            );
        }

        public static Int64? GetInt64(string str)
        {
            if (string.IsNullOrEmpty(str)) { return null; }

            var potentialNumbers = RegexIntegerNumberLike.Matches(str);
            if (potentialNumbers.Count != 0)
            {
                for (var i = 0; i < potentialNumbers.Count; i++)
                {
                    var potentialNumber = potentialNumbers[i].Value;
                    var succes = Int64.TryParse(potentialNumber, CultureInfo.InvariantCulture, out var value); if (succes) return value;
                }
            }
        
            // try if this is a double
            var floatingPointNum = GetDouble(str); if (floatingPointNum==null) return null;                                     // check if parseable as floating point value
            floatingPointNum     = Math.Max(Math.Min((double)floatingPointNum, (double)long.MaxValue), (double)long.MinValue);  // if so, clamp first
            return (Int64)floatingPointNum;                                                                                     // and convert
        }

        public static object? GetSafeInteger(Type type, string json, bool useRecognizer=true)
        {
            json = PrepString(json); if (string.IsNullOrEmpty(json)) return null;
            object? result = null;
            var longValue  = GetInt64(json);
            if (longValue != null)
            {
                var clampedValue = ClampToIntegerTypeRange((long)longValue, type); // Clamp
                try { return Convert.ChangeType(clampedValue, type, System.Globalization.CultureInfo.InvariantCulture); } catch { /* */ }
            }
            return result?? (useRecognizer?RecognizeOrdinal(json): null);
        }

        public static object? GetSafeFloatingPoint(Type type, string json, bool useRecognizer = true)
        {
            json = PrepString(json); if (string.IsNullOrEmpty(json)) return null;
            object? result = null;
            var doubleValue = GetDouble(json); if (doubleValue != null) {
                // first cast to a double version, to make sure it fits
                var clampedValue = ClampToFloatTypeRange((double)doubleValue, type);                                                              // Clamp
                try { return Convert.ChangeType(clampedValue, type, System.Globalization.CultureInfo.InvariantCulture); } catch { /* */ }  // Cast to final size
            }
            return result ?? (useRecognizer ? RecognizeFloatingPoint(json) : null);
        }

        public static object? GetSafeDateTime(Type type, string json, bool useRecognizer = true)
        {
            json = PrepString(json); if (string.IsNullOrEmpty(json)) return null;
            bool success = DateTime.TryParse(PrepString(json), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var result);
            return success ? result : (useRecognizer ? RecognizeDateTime(json) : null);
        }

        public static string PrepString(string json)
        {
            return json.Trim().Replace("\"", "").Trim();
        }

        private static Double ClampToFloatTypeRange(Double value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single : return Math.Max(Math.Min(value, Single.MaxValue ), Single.MinValue );
                case TypeCode.Double : return Math.Max(Math.Min(value, Double.MaxValue ), Double.MinValue );
                default: return 0;
            }
        }

        public static Int64 ClampToIntegerTypeRange(Int64 value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte : return Math.Max(Math.Min(value, (Int64)sbyte.MaxValue ), (Int64)sbyte.MinValue );
                case TypeCode.Byte  : return Math.Max(Math.Min(value, (Int64)byte.MaxValue  ), (Int64)byte.MinValue  );
                case TypeCode.Int16 : return Math.Max(Math.Min(value, (Int64)short.MaxValue ), (Int64)short.MinValue );
                case TypeCode.UInt16: return Math.Max(Math.Min(value, (Int64)ushort.MaxValue), (Int64)ushort.MinValue);
                case TypeCode.Int32 : return Math.Max(Math.Min(value, (Int64)int.MaxValue   ), (Int64)int.MinValue   );
                case TypeCode.UInt32: return Math.Max(Math.Min(value, (Int64)uint.MaxValue  ), (Int64)uint.MinValue  );
                case TypeCode.Int64 : return Math.Max(Math.Min(value, (Int64)long.MaxValue  ), (Int64)long.MinValue  );
                case TypeCode.UInt64: return Math.Max(Math.Min(value, (Int64)long.MaxValue  ), (Int64)ulong.MinValue );
                default: return 0;
            }
        }

        internal static int? RecognizeOrdinal(string text)
        {
            return int.TryParse(RecognizerResultToValue(NumberRecognizer.RecognizeOrdinal(text, Culture.English)), out var result) ? result : null;
        }

        internal static float? RecognizeFloatingPoint(string text)
        {
            return float.TryParse(RecognizerResultToValue(NumberRecognizer.RecognizeNumber(text, Culture.English)), out var result) ? result : null;
        }

        internal static DateTime? RecognizeDateTime(string text)
        {
            return DateTime.TryParse(RecognizerResultToValue(DateTimeRecognizer.RecognizeDateTime(text, Culture.English)), out var result) ? result : null;
        }

        internal static string? RecognizerResultToValue(List<ModelResult>? results)
        {
            if (results == null || results.Count==0) { return null;}
            var result = results[0].Resolution["value"]?.ToString()??null;
            return result;
        }

        public static double? GetDouble(string str)
        {
            if (string.IsNullOrEmpty(str)) { return null; }
            
            var potentialNumbers = RegexFloatNumberLike.Matches(str);
            if (potentialNumbers.Count == 0) { return null; }

            double value;
            for (var i = 0; i < potentialNumbers.Count; i++)
            {
                var potentialNumber = potentialNumbers[i].Value;
                
                var succes = Double.TryParse(potentialNumber, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out value); if (succes) return value;
                    succes = Double.TryParse(potentialNumber, System.Globalization.NumberStyles.Float, CommaDecSeparator           , out value); if (succes) return value;
                    
            }
            return null;
        }
    }
}
