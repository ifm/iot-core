namespace ifm.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides utility methods for analyzing and categorizing .NET types, including determining type characteristics such
/// as whether a type is simple, complex, nullable, or implements common collection interfaces.
/// </summary>
/// <remarks>
/// TypeHelpers offers a set of static methods to assist with reflection-based type inspection, making it
/// easier to identify type categories and extract underlying types for generics, arrays, and collections. These methods
/// are useful when building serialization, mapping, or type-driven logic. All methods validate input parameters and may
/// throw exceptions if invalid types are provided. Thread safety is guaranteed as all members are stateless and
/// static.
/// </remarks>
public static class TypeHelpers
{
    public static bool IsNullable(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return InnerIsNullable(type);
    }

    public static bool IsSimple(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        if (InnerIsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsPrimitive;
    }

    public static bool IsSimple(ref Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        if (InnerIsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsPrimitive;
    }

    public static bool IsEnum(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        if (InnerIsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsEnum;
    }

    public static bool IsEnum(ref Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        if (InnerIsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsEnum;
    }

    public static bool IsString(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return type == typeof(string);
    }

    public static bool IsArray(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return type.IsArray;
    }

    public static bool IsVariant(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return type == typeof(Variant.Variant) || type.IsSubclassOf(typeof(Variant.Variant));
    }

    public static bool IsStruct(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return InnerIsStruct(type);
    }

    public static bool IsComplex(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return type.IsClass || InnerIsStruct(type);
    }

    public static bool IsGeneric(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return type.IsGenericType;
    }

    public static bool IsNonGenericEnumerable(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return typeof(IEnumerable).IsAssignableFrom(type);
    }

    public static bool IsGenericEnumerable(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || // It's exactly IEnumerable<>
               type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)); // It implements IEnumerable<>
    }

    public static bool IsNonGenericCollection(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return typeof(ICollection).IsAssignableFrom(type);
    }

    public static bool IsGenericCollection(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>)) || // It's exactly ICollection<>
               type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)); // It implements ICollection<>
    }

    public static bool IsNonGenericList(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return typeof(IList).IsAssignableFrom(type);
    }

    public static bool IsGenericList(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) || // It's exactly IList<>
               type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>)); // It implements IList<>
    }

    public static bool IsNonGenericDictionary(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return typeof(IDictionary).IsAssignableFrom(type);
    }

    public static bool IsGenericDictionary(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) || // It's exactly IDictionary<>
                type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)); // It implements IDictionary<>
    }

    public static Type GetArrayUnderlyingType(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return type.GetElementType();
    }

    public static Type GetGenericUnderlyingType(Type type, int idx = 0)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));
        ExceptionHelpers.ThrowIfInvalid(() => idx < 0, nameof(type));

        var args = type.GetGenericArguments();
        return idx < args.Length ? args[idx] : null;
    }

    public static Type GetGenericEnumerableType(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) // It's exactly IEnumerable<>
        {
            return type;
        }
        return type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)); // It implements IEnumerable<>
    }

    public static Type GetGenericEnumerableUnderlyingType(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) // It's exactly IEnumerable<>
        {
            return type.GetGenericArguments()[0];
        }
        return type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GetGenericArguments()[0]; // It implements IEnumerable<>
    }

    public static Dictionary<string, string> GetEnumAsDictionary(Type type)
    {
        ExceptionHelpers.ThrowIfNull(type, nameof(type));

        return Enum.GetValues(type).Cast<object>().ToDictionary(v => ((int)v).ToString(), v => v.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool InnerIsNullable(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool InnerIsStruct(Type type)
    {
        // If it is a value type, but not a primitive type and not an enum it is a struct
        return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
    }
}