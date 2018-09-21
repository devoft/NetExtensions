using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace devoft.Core.System
{
    public static class Convertions
    {

        public static object ConvertFromXamlScalar(this string str, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);
            return str.TryConvertFromXamlString(type.Name, out object result);
        }

        public static object ConvertFromXamlScalar(this string str, string typeName)
            => (TryConvertFromXamlString(str, typeName, out object result))
                 ? result
                 : throw new InvalidCastException($"Cannot convert string to {typeName}");

        public static string ConvertToXamlScalar(this object x, string nullValue = null)
            => x == null
                ? nullValue ?? "{x:Null}"
                : ((x as DateTime?)?.ToString("s", CultureInfo.InvariantCulture)
                   ?? (x as DateTimeOffset?)?.ToString("s", CultureInfo.InvariantCulture)
                   ?? (x as TimeSpan?)?.ToString("s", CultureInfo.InvariantCulture)
                   ?? (x as int?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as long?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as BigInteger?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as byte?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as double?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as decimal?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as bool?)?.ToString(CultureInfo.InvariantCulture)
                   ?? (x as Guid?)?.ToString("d")
                   ?? (x is byte[]
                        ? Convert.ToBase64String((byte[])x)
                        : Convert.ToString(x, CultureInfo.InvariantCulture)));

        public static bool TryConvertFromXamlString(this string str, string typeName, out object result)
        {
            result = null;
            var res = true;
            if (str == "{x:Null}")
                result = null;
            else
                switch (typeName)
                {
                    case nameof(Guid):
                        result = Guid.Parse(str);
                        break;
                    case "Int32":
                        result = int.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "Int64":
                        result = long.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case nameof(BigInteger):
                        result = BigInteger.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "Byte":
                        result = byte.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "Decimal":
                        result = decimal.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "Double":
                        result = double.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "Single":
                        result = float.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "Boolean":
                        result = bool.Parse(str);
                        break;
                    case "char":
                        result = char.Parse(str);
                        break;
                    case "DateTime":
                        result = DateTime.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case nameof(TimeSpan):
                        result = TimeSpan.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case nameof(DateTimeOffset):
                        result = DateTimeOffset.Parse(str, CultureInfo.InvariantCulture);
                        break;
                    case "String":
                        result = str;
                        break;
                    case "Byte[]":
                        result = Convert.FromBase64String(str);
                        break;
                    default:
                        result = null;
                        res = false;
                        break;
                }
            return res;
        }
    }
}
