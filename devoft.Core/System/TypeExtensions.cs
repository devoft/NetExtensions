using devoft.System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace devoft.System
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetBaseTypes(this Type type, bool inclusive = false, bool includeInterfaces = true)
        {
            if (type == null)
                yield break;
            if (type == typeof(object))
                yield break;
            yield return inclusive
                ? type
                : type.BaseType;
            foreach (var t in type.BaseType.GetBaseTypes(inclusive, includeInterfaces))
                yield return t;
            if (includeInterfaces)
                foreach (var interfaceType in type.GetInterfaces())
                {
                    yield return interfaceType;
                    foreach (var t in interfaceType.GetBaseTypes(inclusive, includeInterfaces))
                        yield return t;
                }
        }

        public static bool IsAnonymousType(this Type type)
            => type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any() &&
               type.FullName.Contains("AnonymousType");


        public static void InvokeGenericMethod(
            this object instance,
            string methodName,
            Type[] genericTypes,
            Type[] argTypes,
            params object[] args)
        {
            var instanceType = instance?.GetType();
            instanceType?.InvokeGenericMethod(instance, methodName, genericTypes, argTypes, args);
        }

        public static void InvokeGenericMethod(
            this Type type,
            object instance,
            string methodName,
            Type[] genericTypes,
            Type[] argTypes,
            params object[] args)
        {
            var methodInfo = type.GetMethod(methodName, argTypes);
            methodInfo.MakeGenericMethod(genericTypes).Invoke(instance, args);
        }

        public static TResult InvokeGenericMethod<TResult>(
            this object instance,
            string methodName,
            Type[] genericTypes,
            Type[] argTypes,
            params object[] args)
        {
            var instanceType = instance?.GetType();
            return instanceType != null
                ? instanceType.InvokeGenericMethod<TResult>(instance, methodName, genericTypes, argTypes, args)
                : default(TResult);
        }

        public static TResult InvokeGenericMethod<TResult>(
            this Type type,
            object instance,
            string methodName,
            Type[] genericTypes,
            Type[] argTypes,
            params object[] args)
        {
            var methodInfo = type.GetMethod(methodName, argTypes);
            return (TResult)methodInfo.MakeGenericMethod(genericTypes).Invoke(instance, args);
        }


        public static bool IsScalar(this Type type)
                => type.In(
                    typeof(int),
                    typeof(long),
                    typeof(double),
                    typeof(decimal),
                    typeof(bool),
                    typeof(char),
                    typeof(string),
                    typeof(DateTime),
                    typeof(DateTimeOffset));

        public static object Parse(this Type type, string str)
        {
            switch (type.Name)
            {
                case "Int32":
                    return int.Parse(str);
                case "Int64":
                    return long.Parse(str);
                case "Single":
                    return double.Parse(str);
                case "Decimal":
                    return decimal.Parse(str);
                case "Boolean":
                    return bool.Parse(str);
                case "Char":
                    return str[0];
                case "DateTime":
                    return DateTime.Parse(str);
                case "DateTimeOffset":
                    return DateTimeOffset.Parse(str);
                default:
                    return str;
            }
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo member, bool inherit = false)
            where T : Attribute
            => member.GetCustomAttributes(typeof(T), inherit).OfType<T>();



    }


}
