using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BigIron
{
    /// <summary>
    /// Reflection help
    /// </summary>
    static class Mirror
    {
        public static object GetFieldValue<T>(this T instance, string field)
        {
            object value = null;
            var reflectedField = typeof(T).GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
            if (reflectedField != null)
                value = reflectedField.GetValue(instance);

            return value;
        }

        public static void WithAttribute<T>(this MemberInfo member, Action<T> action)
            where T : Attribute
        {
            T attribute = member.GetCustomAttribute<T>();
            if (attribute != null)
                action(attribute);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsValueType
                   && type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static TypeCode GetUnderlyingTypeCode(this Type type)
        {
            if (IsNullable(type))
                type = type.GetGenericArguments().First();

            return Type.GetTypeCode(type);
        }

        public static bool IsNumeric(this Type type)
        {
            switch(GetUnderlyingTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;

            }
        }
    }
}
