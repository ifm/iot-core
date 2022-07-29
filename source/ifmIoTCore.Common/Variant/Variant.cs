namespace ifmIoTCore.Common.Variant
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class Variant
    {
        #region FromObject

        /// <summary>
        /// Convert object to variant
        /// </summary>
        public static Variant FromObject(object data)
        {
            return VariantFromObject(data, data?.GetType());
        }

        private static Variant VariantFromObject(object data, Type type)
        {
            if (data == null)
            {
                return new VariantValue();
            }

            if (IsVariant(type))
            {
                return (Variant)data;
            }
            if (IsSimple(type) || IsSimple(data))
            {
                return VariantFromSimple(data, type);
            }
            if (IsArray(type) || IsList(type) || IsGenericList(type))
            {
                return VariantFromList(data, new VariantArray());
            }
            if (IsDictionary(type) || IsGenericDictionary(type))
            {
                return VariantFromDictionary(data, new VariantObject());
            }
            if (IsComplex(type))
            {
                return VariantFromComplex(data, type, new VariantObject());
            }

            throw new Exception($"Unsupported type {type.FullName}");
        }


        private static VariantValue VariantFromSimple(object data, Type type)
        {
            if (type == typeof(bool))
            {
                return new VariantValue((bool)data);
            }
            if (type == typeof(char))
            {
                return new VariantValue((char)data);
            }
            if (type == typeof(sbyte))
            {
                return new VariantValue((sbyte)data);
            }
            if (type == typeof(byte))
            {
                return new VariantValue((byte)data);
            }
            if (type == typeof(short))
            {
                return new VariantValue((short)data);
            }
            if (type == typeof(ushort))
            {
                return new VariantValue((ushort)data);
            }
            if (type == typeof(int))
            {
                return new VariantValue((int)data);
            }
            if (type == typeof(uint))
            {
                return new VariantValue((uint)data);
            }
            if (type == typeof(long))
            {
                return new VariantValue((long)data);
            }
            if (type == typeof(ulong))
            {
                return new VariantValue((ulong)data);
            }
            if (type == typeof(float))
            {
                return new VariantValue((float)data);
            }
            if (type == typeof(double))
            {
                return new VariantValue((double)data);
            }
            if (type == typeof(decimal))
            {
                return new VariantValue((decimal)data);
            }
            if (type == typeof(string))
            {
                return new VariantValue((string)data);
            }
            if (type == typeof(byte[]))
            {
                return new VariantValue((byte[])data);
            }
            if (type.IsEnum)
            {
                return new VariantValue(Convert.ToInt32(data));
            }

            throw new Exception($"Unsupported value type {type.FullName}");
        }

        private static VariantArray VariantFromList(object data, VariantArray vArray)
        {
            foreach (var item in (IEnumerable)data)
            {
                vArray.Add(VariantFromObject(item, item?.GetType()));
            }

            return vArray;
        }

        private static VariantObject VariantFromDictionary(object data, VariantObject vObject)
        {
            foreach (var item in (IEnumerable)data)
            {
                Variant key;
                Variant value;
                if (item is DictionaryEntry dictionaryEntry)
                {
                    key = VariantFromObject(dictionaryEntry.Key, dictionaryEntry.Key?.GetType());
                    value = VariantFromObject(dictionaryEntry.Value, dictionaryEntry.Value?.GetType());
                }
                else
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var itemKey = item?.GetType().GetProperty("Key").GetValue(item);
                    key = VariantFromObject(itemKey, itemKey?.GetType());

                    // ReSharper disable once PossibleNullReferenceException
                    var itemValue = item?.GetType().GetProperty("Value").GetValue(item);
                    value = VariantFromObject(itemValue, itemValue?.GetType());
                }

                vObject.Add(key, value);
            }

            return vObject;
        }

        private static VariantObject VariantFromComplex(object data, Type type, VariantObject vObject)
        {
            foreach (var property in type.GetProperties())
            {
                var propertyAttribute = GetVariantPropertyAttribute(property);

                if (GetPropertyIgnored(propertyAttribute))
                {
                    continue;
                }

                var propertyValue = property.GetValue(data);

                if (propertyValue == null && GetPropertyIgnoredIfNull(propertyAttribute))
                {
                    continue;
                }

                var propertyType = GetPropertyType(propertyValue, property);

                var value = VariantFromObject(propertyValue, propertyType);
 
                var propertyName = GetPropertyName(property, propertyAttribute);

                vObject.Add(propertyName, value);
            }

            return vObject;
        }

        #endregion

        #region ToObject

        /// <summary>
        /// Convert variant to object of type T
        /// </summary>
        public static T ToObject<T>(Variant data)
        {
            return (T)VariantToObject(data, typeof(T));
        }

        /// <summary>
        /// Convert this variant to object of type T
        /// </summary>
        public T ToObject<T>()
        {
            return (T) VariantToObject(this, typeof(T));
        }

        private static object VariantToObject(Variant data, Type type)
        {
            if (data == null)
            {
                return default;
            }

            if (IsVariant(type))
            {
                return data;
            }
            if (IsSimple(type))
            {
                if (data is VariantValue vValue)
                {
                    return VariantToSimple(vValue, type);
                }
                return null;
            }
            if (IsArray(type))
            {
                if (data is VariantArray vArray)
                {
                    return VariantToArray(vArray, type);
                }
                return null;
            }
            if (IsList(type))
            {
                if (data is VariantArray vArray)
                {
                    return VariantToList(vArray, type);
                }
                return null;
            }
            if (IsGenericList(type))
            {
                if (data is VariantArray vArray)
                {
                    return VariantToGenericList(vArray, type);
                }
                return null;
            }
            if (IsDictionary(type))
            {
                if (data is VariantObject vObject)
                {
                    return VariantToDictionary(vObject, type);
                }
                return null;
            }
            if (IsGenericDictionary(type))
            {
                if (data is VariantObject vObject)
                {
                    return VariantToGenericDictionary(vObject, type);
                }
                return null;
            }
            if (IsComplex(type))
            {
                if (data is VariantObject vObject)
                {
                    return VariantToComplex(vObject, type);
                }
                return null;
            }
            throw new Exception($"Unsupported type {type.FullName}");
        }

        private static object VariantToSimple(VariantValue data, Type type)
        {
            if (type == typeof(bool))
            {
                return (bool)data;
            }
            if (type == typeof(bool?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (bool) data;
            }

            if (type == typeof(char))
            {
                return (char)data;
            }
            if (type == typeof(char?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (char)data;
            }

            if (type == typeof(sbyte))
            {
                return (sbyte)data;
            }
            if (type == typeof(sbyte?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (sbyte)data;
            }

            if (type == typeof(byte))
            {
                return (byte)data;
            }
            if (type == typeof(byte?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (byte) data;
            }

            if (type == typeof(short))
            {
                return (short)data;
            }
            if (type == typeof(short?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (short)data;
            }

            if (type == typeof(ushort))
            {
                return (ushort)data;
            }
            if (type == typeof(ushort?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (ushort)data;
            }


            if (type == typeof(int))
            {
                return (int)data;
            }
            if (type == typeof(int?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (int)data;
            }

            if (type == typeof(uint))
            {
                return (uint)data;
            }
            if (type == typeof(uint?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (uint)data;
            }

            if (type == typeof(long))
            {
                return (long)data;
            }
            if (type == typeof(long?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (long)data;
            }

            if (type == typeof(ulong))
            {
                return (ulong)data;
            }
            if (type == typeof(ulong?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (ulong)data;
            }

            if (type == typeof(float))
            {
                return (float)data;
            }
            if (type == typeof(float?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (float)data;
            }

            if (type == typeof(double))
            {
                return (double)data;
            }
            if (type == typeof(double?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (double)data;
            }

            if (type == typeof(decimal))
            {
                return (decimal)data;
            }
            if (type == typeof(decimal?))
            {
                if (data.Type == VariantValue.ValueType.Null) return null;
                return (decimal)data;
            }

            if (type == typeof(string))
            {
                return (string)data;
            }
            if (type == typeof(byte[]))
            {
                return (byte[])data;
            }
            if (type.IsEnum)
            {
                return (int)data;
            }
            throw new Exception($"Unsupported value type {type.FullName}");
        }

        private static object VariantToArray(VariantArray data, Type type)
        {
            var arrayToFill = (IList)Activator.CreateInstance(type, data.Count);

            var elementType = type.GetElementType();

            for (var index = 0; index < arrayToFill.Count; index++)
            {
                arrayToFill[index] = VariantToObject(data[index], elementType);
            }

            return arrayToFill;
        }

        private static object VariantToList(VariantArray data, Type type)
        {
            var listToFill = (IList)Activator.CreateInstance(type);

            var elementType = GetCollectionElementType(type);

            foreach (var item in data.GetItems())
            {
                listToFill.Add(VariantToObject(item, elementType));
            }

            return listToFill;
        }

        private static object VariantToGenericList(VariantArray data, Type type)
        {
            var listToFill = Activator.CreateInstance(type);

            var elementType = GetCollectionElementType(type);

            foreach (var item in data.GetItems())
            {
                var addMethodInfo = type.GetMethod("Add");
                // ReSharper disable once PossibleNullReferenceException
                addMethodInfo.Invoke(listToFill, new[] { VariantToObject(item, elementType) });
            }

            return listToFill;
        }

        private static object VariantToDictionary(VariantObject data, Type type)
        {
            var dictionaryToFill = (IDictionary)Activator.CreateInstance(type);

            var elementType = GetCollectionElementType(type);
            var keyType = elementType.GetProperty("Key")?.PropertyType;
            var valueType = elementType.GetProperty("Value")?.PropertyType;

            foreach (var item in data.GetItems())
            {
                var key = VariantToObject(item.Key, keyType);
                var value = VariantToObject(item.Value, valueType);
                dictionaryToFill.Add(key, value);
            }

            return dictionaryToFill;
        }

        private static object VariantToGenericDictionary(VariantObject data, Type type)
        {
            var dictionaryToFill = (IEnumerable)Activator.CreateInstance(type);

            var elementType = GetCollectionElementType(type);
            var keyType = elementType.GetProperty("Key")?.PropertyType;
            var valueType = elementType.GetProperty("Value")?.PropertyType;

            foreach (var item in data.GetItems())
            {
                var key = VariantToObject(item.Key, keyType);
                var value = VariantToObject(item.Value, valueType);

                var addMethodInfo = type.GetMethod("Add", new [] { keyType, valueType});
                // ReSharper disable once PossibleNullReferenceException
                addMethodInfo.Invoke(dictionaryToFill, new[] { key, value });
            }

            return dictionaryToFill;
        }

        private static object VariantToComplex(VariantObject data, Type type)
        {
            // Support complex types (structs and classes) with no parameterless constructor and no public settable properties:
            // The type must have a constructor with VariantConstructor attribute and the parameters must have the
            // VariantProperty attribute. Then search for the constructor with the VariantConstructor attribute.
            // From the parameter-list find the corresponding parameter for each value by VariantProperty attribute and build
            // the parameter array for CreateInstance accordingly. Then call CreateInstance with the parameter list and return the object.

            var objectToFill = Activator.CreateInstance(type);

            foreach (var property in type.GetProperties())
            {
                var propertyAttribute = GetVariantPropertyAttribute(property);

                // Get the variant item for the property
                Variant item;
                if (propertyAttribute != null)
                {
                    // First try the attribute name
                    if (!data.TryGetValue(propertyAttribute.Name, out item))
                    {
                        if (propertyAttribute.AlternativeNames != null)
                        {
                            // Try the alternative names
                            var itemName = data.FindFirstKey(propertyAttribute.AlternativeNames);
                            if (itemName != null)
                            {
                                item = data[itemName];
                            }
                        }
                    }
                }
                else
                {
                    data.TryGetValue(property.Name, out item);
                }

                if (item != null)
                {
                    if (property.SetMethod != null)
                    {
                        var value = VariantToObject(item, property.PropertyType);
                        property.SetValue(objectToFill, value);
                    }
                }
                else
                {
                    if (GetPropertyRequired(propertyAttribute))
                    {
                        throw new Exception($"Required property '{property.Name}' not found in variant");
                    }
                }
            }

            return objectToFill;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsVariant(Type type)
        {
            return type == typeof(Variant) || type.IsSubclassOf(typeof(Variant));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSimple(object value)
        {
            return value is char ||
                   value is sbyte || 
                   value is byte ||
                   value is short ||
                   value is ushort ||
                   value is int ||
                   value is uint ||
                   value is long ||
                   value is ulong ||
                   value is float ||
                   value is double ||
                   value is decimal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsSimple(Type type)
        {
            // If it's a nullable type check the underlying type
            if (IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }

            return type.IsPrimitive || 
                   type.IsEnum || 
                   type == typeof(decimal) || 
                   type == typeof(string) || 
                   type == typeof(byte[]) || 
                   type == typeof(Nullable<>);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsList(Type type)
        {
            return typeof(IList).IsAssignableFrom(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsGenericList(Type type)
        {
            return type.GetInterfaces().Any(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IList<>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDictionary(Type type)
        {
            return typeof(IDictionary).IsAssignableFrom(type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsGenericDictionary(Type type)
        {
            return type.GetInterfaces().Any(item => item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsComplex(Type type)
        {
            // If it is a value type, but not a primitive type or enum it is a struct
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum && !IsNullable(type) ||
                   type.IsClass && !IsList(type) && !IsDictionary(type);
        }

        private static Type GetCollectionElementType(Type type)
        {
            // First try the generic way

            // Query the IEnumerable<T> interface for its generic parameter
            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType && item.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return item.GetGenericArguments()[0];
                }
            }

            // Now try the non-generic way

            // If it's a dictionary return DictionaryEntry
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return typeof(DictionaryEntry);
            }

            // If it's a list look for an item property with an int index parameter
            // where the property type is anything but object
            if (typeof(IList).IsAssignableFrom(type))
            {
                foreach (var item in type.GetProperties())
                {
                    if (item.Name == "Item" && item.PropertyType != typeof(object))
                    {
                        var ipa = item.GetIndexParameters();
                        if (ipa.Length == 1 && ipa[0].ParameterType == typeof(int))
                        {
                            return item.PropertyType;
                        }
                    }
                }
            }

            // If it's a collection, look for an Add() method whose parameter is 
            // anything but object
            if (typeof(ICollection).IsAssignableFrom(type))
            {
                foreach (var item in type.GetMethods())
                {
                    if (item.Name == "Add")
                    {
                        var pa = item.GetParameters();
                        if (pa.Length == 1 && pa[0].ParameterType != typeof(object))
                        {
                            return pa[0].ParameterType;
                        }
                    }
                }
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return typeof(object);
            }
            return null;
        }

        //private static void GetKeyValuePairTypes(Type type, out Type keyType, out Type valueType)
        //{
        //    keyType = null;
        //    valueType = null;
        //    if (type is { IsGenericType: true })
        //    {
        //        var baseType = type.GetGenericTypeDefinition();
        //        if (baseType == typeof(KeyValuePair<,>))
        //        {
        //            var argTypes = baseType.GetGenericArguments();
        //            keyType = argTypes[0];
        //            valueType = argTypes[1];
        //        }
        //    }
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static VariantPropertyAttribute GetVariantPropertyAttribute(MemberInfo property)
        {
            var attributes = property.GetCustomAttributes(false);
            return attributes.Where(attribute => attribute.GetType() == typeof(VariantPropertyAttribute)).Cast<VariantPropertyAttribute>().FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type GetPropertyType(object data, PropertyInfo property)
        {
            return data?.GetType() ?? property.PropertyType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPropertyName(PropertyInfo property, VariantPropertyAttribute attribute)
        {
            return attribute?.Name ?? property.Name;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GetPropertyIgnored(VariantPropertyAttribute attribute)
        {
            return attribute?.Ignored ?? false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GetPropertyRequired(VariantPropertyAttribute attribute)
        {
            return attribute?.Required ?? false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GetPropertyIgnoredIfNull(VariantPropertyAttribute attribute)
        {
            return attribute?.IgnoredIfNull ?? false;
        }

        #endregion
    }
}
