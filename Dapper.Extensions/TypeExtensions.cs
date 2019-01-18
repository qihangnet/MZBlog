using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dapper.Extensions
{
    internal static class TypeExtensions
    {
        public static string Name(this Type type) => type.GetTypeInfo().Name;

        public static bool IsValueType(this Type type) => type.GetTypeInfo().IsValueType;

        public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

        public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

        public static bool IsInterface(this Type type) => type.GetTypeInfo().IsInterface;

        public static IEnumerable<Attribute> GetCustomAttributes(this Type type, bool inherit)
        {
            return type.GetCustomAttributes<Attribute>(inherit);
        }

        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null) return TypeCode.Empty;
            if (typeCodeLookup.TryGetValue(type, out TypeCode result)) return result;

            if (type.IsEnum())
            {
                type = Enum.GetUnderlyingType(type);
                if (typeCodeLookup.TryGetValue(type, out result)) return result;
            }
            return TypeCode.Object;
        }

        private static readonly Dictionary<Type, TypeCode> typeCodeLookup = new Dictionary<Type, TypeCode>
        {
            [typeof(bool)] = TypeCode.Boolean,
            [typeof(byte)] = TypeCode.Byte,
            [typeof(char)] = TypeCode.Char,
            [typeof(DateTime)] = TypeCode.DateTime,
            [typeof(decimal)] = TypeCode.Decimal,
            [typeof(double)] = TypeCode.Double,
            [typeof(short)] = TypeCode.Int16,
            [typeof(int)] = TypeCode.Int32,
            [typeof(long)] = TypeCode.Int64,
            [typeof(object)] = TypeCode.Object,
            [typeof(sbyte)] = TypeCode.SByte,
            [typeof(float)] = TypeCode.Single,
            [typeof(string)] = TypeCode.String,
            [typeof(ushort)] = TypeCode.UInt16,
            [typeof(uint)] = TypeCode.UInt32,
            [typeof(ulong)] = TypeCode.UInt64,
        };

        public static MethodInfo GetPublicInstanceMethod(this Type type, string name, Type[] types)
        {
            var method = type.GetMethod(name, types);
            return (method?.IsPublic == true && !method.IsStatic) ? method : null;
        }
    }
}