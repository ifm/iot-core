namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Creates an element format from a given type.
/// </summary>
public static class FormatFactory
{
    /// <summary>
    /// Creates an element format from a given type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The format.</returns>
    public static Format Create(Type type)
    {
        if (type == null) return null;

        if (IsSimple(ref type)) return FromSimple(type);
        if (IsEnum(ref type)) return FromEnum(type);
        if (IsVariant(type)) return FromVariant(type);
        if (IsArray(type)) return FromArray(type);
        if (IsGenericDictionary(type)) return FromGenericDictionary(type);
        if (IsGenericEnumerable(type)) return FromGenericEnumerable(type);
        if (IsComplex(type)) return FromComplex(type);
        return null;
    }

    private static Format FromSimple(Type type)
    {
        if (type == typeof(bool)) return new BooleanFormat();
        if (type == typeof(sbyte)) return new Int8Format();
        if (type == typeof(byte)) return new UInt8Format();
        if (type == typeof(short)) return new Int16Format();
        if (type == typeof(char) || type == typeof(ushort)) return new UInt16Format();
        if (type == typeof(int)) return new Int32Format();
        if (type == typeof(uint)) return new UInt32Format();
        if (type == typeof(long)) return new Int64Format();
        if (type == typeof(ulong)) return new UInt64Format();
        if (type == typeof(float)) return new FloatFormat();
        if (type == typeof(double)) return new DoubleFormat();
        if (type == typeof(string)) return new StringFormat();
        return null;
    }

    private static Format FromEnum(Type type)
    {
        return new IntegerEnumFormat(new IntegerEnumValuation(GetEnumAsDictionary(type)));
    }

    private static Format FromVariant(Type type)
    {
        if (type == typeof(VariantValue)) return new ValueFormat();
        if (type == typeof(VariantArray)) return new ArrayFormat();
        if (type == typeof(VariantObject)) return new ObjectFormat();
        return new AnyFormat();
    }

    private static Format FromArray(Type type)
    {
        var valueType = GetArrayUnderlyingType(type);
        return new ArrayFormat(new ArrayValuation(Create(valueType)));
    }

    private static Format FromGenericEnumerable(Type type)
    {
        var valueType = GetGenericEnumerableUnderlyingType(type);
        return new ArrayFormat(new ArrayValuation(Create(valueType)));
    }

    private static Format FromGenericDictionary(Type type)
    {
        var elementType = GetGenericEnumerableUnderlyingType(type);
        var valueType = elementType.GetProperty("Value")?.PropertyType;
        var format = new MapFormat(new MapValuation(Create(valueType)));
        return format;
    }

    private static Format FromComplex(Type type)
    {
        var format = new ObjectFormat(new ObjectValuation([]));
        foreach (var propertyInfo in type.GetProperties())
        {
            var propertyAttribute = VariantPropertyAttribute.Get(propertyInfo);
            var name = propertyAttribute?.Name ?? propertyInfo.Name;
            bool? optional = propertyAttribute is { Required: true } ? null : true;
            format.Valuation.Fields.Add(new ObjectValuation.Field(name, Create(propertyInfo.PropertyType), optional));
        }
        return format;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSimple(ref Type type)
    {
        if (IsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsPrimitive || type == typeof(string);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEnum(ref Type type)
    {
        if (IsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsEnum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsVariant(Type type)
    {
        return type == typeof(Variant) || type.IsSubclassOf(typeof(Variant));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsArray(Type type)
    {
        return type.IsArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsStruct(Type type)
    {
        // If it is a value type, but not a primitive type and not an enum it is a struct
        return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsComplex(Type type)
    {
        return type.IsClass || IsStruct(type);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsGenericEnumerable(Type type)
    {
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || // It's exactly IEnumerable<>
               type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)); // It implements IEnumerable<>
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsGenericDictionary(Type type)
    {
        return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)) || // It's exactly IDictionary<>
               type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>)); // It implements IDictionary<>
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Type GetArrayUnderlyingType(Type type)
    {
        return type.GetElementType();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Type GetGenericEnumerableUnderlyingType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)) // It's exactly IEnumerable<>
        {
            return type.GetGenericArguments()[0];
        }
        return type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GetGenericArguments()[0]; // It implements IEnumerable<>
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNullable(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Dictionary<string, string> GetEnumAsDictionary(Type type)
    {
        return Enum.GetValues(type).Cast<object>().ToDictionary(v => ((int)v).ToString(), v => v.ToString());
    }
}