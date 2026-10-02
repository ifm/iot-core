namespace ifm.IoTCore.UnitTests;

using System;
using ifm.Common.Variant;
using Newtonsoft.Json.Linq;

internal class VariantConverter
{
    public static Variant FromJToken(JToken data)
    {
        return VariantFromJToken(data);
    }

    private static Variant VariantFromJToken(JToken data)
    {
        if (data == null)
        {
            return null;
        }
        if (data is JValue jValue)
        {
            return VariantFromJValue(jValue);
        }
        if (data is JArray jArray)
        {
            return VariantFromJArray(jArray, new VariantArray());
        }
        if (data is JObject jObject)
        {
            return VariantFromJObject(jObject, new VariantObject());
        }
        throw new Exception($"Unsupported json type {data.Type}");
    }

    private static VariantValue VariantFromJValue(JValue data)
    {
        if (data.Type == JTokenType.Null)
        {
            return null;
        }
        if (data.Type == JTokenType.Boolean)
        {
            return new VariantValue((bool)data);
        }
        if (data.Type == JTokenType.Integer)
        {
            return new VariantValue((int)data);
        }
        if (data.Type == JTokenType.Float)
        {
            return new VariantValue((float)data);
        }
        if (data.Type == JTokenType.String)
        {
            return new VariantValue((string)data);
        }
        throw new Exception($"Unsupported json value type {data.Type}");
    }

    private static Variant VariantFromJArray(JArray data, VariantArray vArray)
    {
        foreach (var item in data)
        {
            vArray.Add(VariantFromJToken(item));
        }
        return vArray;
    }

    private static Variant VariantFromJObject(JObject data, VariantObject vObject)
    {
        foreach (var item in data)
        {
            vObject.Add((VariantValue)item.Key, VariantFromJToken(item.Value));
        }
        return vObject;
    }

    public static JToken ToJToken(Variant data)
    {
        return VariantToJToken(data);
    }

    private static JToken VariantToJToken(Variant data)
    {
        if (data == null)
        {
            return JValue.CreateNull();
        }
        if (data is VariantValue vValue)
        {
            return VariantToJValue(vValue);
        }
        if (data is VariantArray vArray)
        {
            return VariantToJArray(vArray, new JArray());
        }
        if (data is VariantObject vObject)
        {
            return VariantToJObject(vObject, new JObject());
        }
        throw new Exception($"Unsupported variant type {data.GetType()}");
    }

    private static JValue VariantToJValue(VariantValue data)
    {
        if (data.Type == VariantValue.ValueTypes.Boolean)
        {
            return new JValue((bool)data);
        }
        if (data.Type == VariantValue.ValueTypes.Character)
        {
            return new JValue((char)data);
        }
        if (data.Type == VariantValue.ValueTypes.Int8)
        {
            return new JValue((sbyte)data);
        }
        if (data.Type == VariantValue.ValueTypes.UInt8)
        {
            return new JValue((byte)data);
        }
        if (data.Type == VariantValue.ValueTypes.Int16)
        {
            return new JValue((short)data);
        }
        if (data.Type == VariantValue.ValueTypes.UInt16)
        {
            return new JValue((ushort)data);
        }
        if (data.Type == VariantValue.ValueTypes.Int32)
        {
            return new JValue((int)data);
        }
        if (data.Type == VariantValue.ValueTypes.UInt32)
        {
            return new JValue((uint)data);
        }
        if (data.Type == VariantValue.ValueTypes.Int64)
        {
            return new JValue((long)data);
        }
        if (data.Type == VariantValue.ValueTypes.UInt64)
        {
            return new JValue((ulong)data);
        }
        if (data.Type == VariantValue.ValueTypes.Float)
        {
            return new JValue((float)data);
        }
        if (data.Type == VariantValue.ValueTypes.Double)
        {
            return new JValue((double)data);
        }
        if (data.Type == VariantValue.ValueTypes.Decimal)
        {
            return new JValue((decimal)data);
        }
        if (data.Type == VariantValue.ValueTypes.String)
        {
            return new JValue((string)data);
        }
        if (data.Type == VariantValue.ValueTypes.DateTime)
        {
            return new JValue((DateTime)data);
        }
        if (data.Type == VariantValue.ValueTypes.TimeSpan)
        {
            return new JValue((TimeSpan)data);
        }
        if (data.Type == VariantValue.ValueTypes.Uri)
        {
            return new JValue((Uri)data);
        }
        if (data.Type == VariantValue.ValueTypes.Guid)
        {
            return new JValue((Guid)data);
        }
        throw new Exception($"Unsupported variant value type {data.Type}");
    }

    private static JArray VariantToJArray(VariantArray data, JArray jArray)
    {
        foreach (var item in data)
        {
            jArray.Add(ToJToken(item));
        }
        return jArray;
    }

    private static JObject VariantToJObject(VariantObject data, JObject jObject)
    {
        foreach (var item in data)
        {
            jObject.Add(item.Key.ToString(), ToJToken(item.Value));
        }
        return jObject;
    }
}
