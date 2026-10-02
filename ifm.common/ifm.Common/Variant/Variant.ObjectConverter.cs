namespace ifm.Common.Variant;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Resources;

public abstract partial class Variant
{
    /// <summary>
    /// Converts an object to a variant.
    /// </summary>
    /// <param name="data">The object to convert.</param>
    /// <returns>The converted variant.</returns>
    public static Variant FromObject(object data)
    {
        return VariantFromObject(data);
    }

    private static Variant VariantFromObject(object data)
    {
        if (data == null) return null;

        var type = data.GetType();
        if (IsVariant(type)) return (Variant)data;
        if (IsSimple(ref type)) return VariantFromSimple(type, data);
        if (IsArray(type)) return VariantFromEnumerable(data, []);
        if (IsGenericDictionary(type)) return VariantFromDictionary(data, []);
        if (IsGenericEnumerable(type)) return VariantFromEnumerable(data, []);
        if (IsComplex(type)) return VariantFromComplex(type, data, []);
        throw new Exception(string.Format(Resource1.UnsupportedType, type.FullName));
    }

    private static VariantValue VariantFromSimple(Type type, object data)
    {
        if (type == typeof(bool)) return new VariantValue((bool)data);
        if (type == typeof(char)) return new VariantValue((char)data);
        if (type == typeof(sbyte)) return new VariantValue((sbyte)data);
        if (type == typeof(byte)) return new VariantValue((byte)data);
        if (type == typeof(short)) return new VariantValue((short)data);
        if (type == typeof(ushort)) return new VariantValue((ushort)data);
        if (type == typeof(int)) return new VariantValue((int)data);
        if (type == typeof(uint)) return new VariantValue((uint)data);
        if (type == typeof(long)) return new VariantValue((long)data);
        if (type == typeof(ulong)) return new VariantValue((ulong)data);
        if (type == typeof(float)) return new VariantValue((float)data);
        if (type == typeof(double)) return new VariantValue((double)data);
        if (type == typeof(decimal)) return new VariantValue((decimal)data);
        if (type == typeof(string)) return new VariantValue((string)data);
        if (type == typeof(DateTime)) return new VariantValue((DateTime)data);
        if (type == typeof(TimeSpan)) return new VariantValue((TimeSpan)data);
        if (type == typeof(Uri)) return new VariantValue((Uri)data);
        if (type == typeof(Guid)) return new VariantValue((Guid)data);
        if (type.IsEnum) return new VariantValue((int)data);
        throw new Exception(string.Format(Resource1.UnsupportedType, type.FullName));
    }

    private static VariantArray VariantFromEnumerable(object data, VariantArray vArray)
    {
        foreach (var item in (IEnumerable)data)
        {
            vArray.Add(VariantFromObject(item));
        }
        return vArray;
    }

    private static VariantObject VariantFromDictionary(object data, VariantObject vObject)
    {
        foreach (var item in (IEnumerable)data)
        {
            var valueType = item.GetType();
            var key = valueType.GetProperty("Key")?.GetValue(item);
            var value = valueType.GetProperty("Value")?.GetValue(item);
            vObject.Add(VariantFromObject(key), VariantFromObject(value));
        }
        return vObject;
    }

    private static VariantObject VariantFromComplex(Type type, object data, VariantObject vObject)
    {
        foreach (var property in type.GetProperties())
        {
            var propertyIgnored = false;
            var propertyIgnoredIfNull = false;
            var propertyName = property.Name;
            var attribute = VariantPropertyAttribute.Get(property);
            if (attribute != null)
            {
                propertyIgnored = attribute.Ignored;
                propertyIgnoredIfNull = attribute.IgnoredIfNull;
                propertyName = attribute.Name;
            }
            if (propertyIgnored)
            {
                continue;
            }
            var value = property.GetValue(data);
            if (value == null && propertyIgnoredIfNull)
            {
                continue;
            }
            vObject.Add(propertyName, VariantFromObject(value));
        }
        return vObject;
    }

    /// <summary>
    /// Converts a variant to a class instance of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    /// <param name="data">The variant to convert.</param>
    /// <returns>The converted object.</returns>
    public static T ToObject<T>(Variant data)
    {
        return (T)VariantToObject(typeof(T), data);
    }

    /// <summary>
    /// Converts this variant to a class instance of type T.
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    /// <returns>The converted object.</returns>
    public T ToObject<T>()
    {
        return (T)VariantToObject(typeof(T), this);
    }

    private static object VariantToObject(Type type, Variant data)
    {
        if (data == null) return null;

        var contractResolver = type.GetCustomAttribute<VariantContractResolverAttribute>();
        if (contractResolver != null)
        {
            var contractResolverInstance = (IVariantContractResolver)Activator.CreateInstance(contractResolver.ResolverType);
            return contractResolverInstance.CreateInstance(data);
        }

        if (IsVariant(type)) return data;
        if (IsSimple(ref type)) return VariantToSimple(type, (VariantValue)data);
        if (IsArray(type)) return VariantToArray(type, (VariantArray)data);
        if (IsGenericDictionary(type)) return VariantToDictionary(type, (VariantObject)data);
        if (IsGenericEnumerable(type)) return VariantToEnumerable(type, (VariantArray)data);
        if (IsComplex(type)) return VariantToComplex(type, (VariantObject)data);
        throw new Exception(string.Format(Resource1.UnsupportedType, type.FullName));
    }

    private static object VariantToSimple(Type type, VariantValue data)
    {
        if (type == typeof(bool)) return (bool)data;
        if (type == typeof(char)) return (char)data;
        if (type == typeof(sbyte)) return (sbyte)data;
        if (type == typeof(byte)) return (byte)data;
        if (type == typeof(short)) return (short)data;
        if (type == typeof(ushort)) return (ushort)data;
        if (type == typeof(int)) return (int)data;
        if (type == typeof(uint)) return (uint)data;
        if (type == typeof(long)) return (long)data;
        if (type == typeof(ulong)) return (ulong)data;
        if (type == typeof(float)) return (float)data;
        if (type == typeof(double)) return (double)data;
        if (type == typeof(decimal)) return (decimal)data;
        if (type == typeof(string)) return (string)data;
        if (type == typeof(DateTime)) return (DateTime)data;
        if (type == typeof(TimeSpan)) return (TimeSpan)data;
        if (type == typeof(Uri)) return (Uri)data;
        if (type == typeof(Guid)) return (Guid)data;
        if (type.IsEnum) return (int)data;
        throw new Exception(string.Format(Resource1.UnsupportedType, type.FullName));
    }

    private static object VariantToArray(Type type, VariantArray data)
    {
        var objectToFill = (IList)Activator.CreateInstance(type, data.Count);
        var elementType = type.GetElementType();
        for (var index = 0; index < data.Count; index++)
        {
            objectToFill[index] = VariantToObject(elementType, data[index]);
        }
        return objectToFill;
    }

    private static object VariantToEnumerable(Type type, VariantArray data)
    {
        var objectToFill = Activator.CreateInstance(type /*, data.Count*/);
        var elementType = GetGenericEnumerableUnderlyingType(type);
        var addMethod = type.GetMethod("Add", [elementType]) ?? throw new Exception($"Requested type '{type.FullName}' does not provide an Add-method");
        foreach (var item in data)
        {
            addMethod.Invoke(objectToFill, [VariantToObject(elementType, item)]);
        }
        return objectToFill;
    }

    private static object VariantToDictionary(Type type, VariantObject data)
    {
        var objectToFill = Activator.CreateInstance(type /*, data.Count*/);
        var elementType = GetGenericEnumerableUnderlyingType(type);
        var keyType = elementType.GetProperty("Key")?.PropertyType;
        var valueType = elementType.GetProperty("Value")?.PropertyType;
        var addMethod = type.GetMethod("Add", [keyType, valueType]) ?? throw new Exception($"Requested type '{type.FullName}' does not provide an Add-method");
        foreach (var item in data)
        {
            var key = VariantToObject(keyType, item.Key);
            var value = VariantToObject(valueType, item.Value);
            addMethod.Invoke(objectToFill, [key, value]);
        }
        return objectToFill;
    }

    private static object VariantToComplex(Type type, VariantObject data)
    {
        // How to support complex types (structs and classes) with no parameterless constructor and no public settable properties:
        // The type must have a constructor with VariantConstructor attribute and the constructor parameters must have the
        // VariantProperty attribute. Then search for the constructor with the VariantConstructor attribute.
        // From the parameter list find the corresponding parameter for each value by VariantProperty attribute and build
        // the parameter array for CreateInstance accordingly. Then call CreateInstance with the parameter list and return the object.

        var objectToFill = Activator.CreateInstance(type);
        foreach (var property in type.GetProperties())
        {
            var propertyName = property.Name;
            string[] propertyAlternativeNames = null;
            var propertyRequired = false;
            var propertyIgnored = false;

            var attribute = VariantPropertyAttribute.Get(property);
            if (attribute != null)
            {
                propertyName = attribute.Name;
                propertyAlternativeNames = attribute.AlternativeNames;
                propertyRequired = attribute.Required;
                propertyIgnored = attribute.Ignored;
            }

            if (propertyIgnored) continue;
            var propertyFound = false;
            if (data.TryGetValue(propertyName, out var item))
            {
                propertyFound = true;
            }
            else
            {
                if (propertyAlternativeNames != null)
                {
                    if (data.TryGetValue(propertyAlternativeNames, out item))
                    {
                        propertyFound = true;
                    }
                }
            }

            if (propertyFound)
            {
                if (property.SetMethod != null)
                {
                    var value = VariantToObject(property.PropertyType, item);
                    try
                    {
                        property.SetValue(objectToFill, value);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(string.Format(Resource1.PropertySetFailed, propertyName, e.Message));
                    }
                }
                else
                {
                    throw new Exception(string.Format(Resource1.PropertySetNotFound, propertyName));
                }
            }
            else
            {
                if (propertyRequired)
                {
                    throw new Exception(string.Format(Resource1.PropertyNotFound, propertyName));
                }
            }
        }
        return objectToFill;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsSimple(ref Type type)
    {
        if (IsNullable(type))
        {
            type = type.GetGenericArguments()[0];
        }
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(decimal) ||
               type == typeof(string) ||
               type == typeof(DateTime) ||
               type == typeof(TimeSpan) ||
               type == typeof(Uri) ||
               type == typeof(Guid);
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
}