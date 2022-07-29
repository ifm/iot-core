namespace ifmIoTCore.MessageConverter.Json.Newtonsoft
{
    using System;
    using Common.Variant;
    using global::Newtonsoft.Json.Linq;

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
                return new VariantValue();
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
                return new VariantValue();
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
            if (data.Type == JTokenType.Bytes)
            {
                return new VariantValue((byte[])data);
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
            if (data.Type == VariantValue.ValueType.Null)
            {
                return JValue.CreateNull();
            }
            if (data.Type == VariantValue.ValueType.Boolean)
            {
                return new JValue((bool)data);
            }
            if (data.Type == VariantValue.ValueType.Character)
            {
                return new JValue((char)data);
            }
            if (data.Type == VariantValue.ValueType.SignedByte)
            {
                return new JValue((sbyte)data);
            }
            if (data.Type == VariantValue.ValueType.Byte)
            {
                return new JValue((byte)data);
            }
            if (data.Type == VariantValue.ValueType.Short)
            {
                return new JValue((short)data);
            }
            if (data.Type == VariantValue.ValueType.UnsignedShort)
            {
                return new JValue((ushort)data);
            }
            if (data.Type == VariantValue.ValueType.Integer)
            {
                return new JValue((int)data);
            }
            if (data.Type == VariantValue.ValueType.UnsignedInteger)
            {
                return new JValue((uint)data);
            }
            if (data.Type == VariantValue.ValueType.Long)
            {
                return new JValue((long)data);
            }
            if (data.Type == VariantValue.ValueType.UnsignedLong)
            {
                return new JValue((ulong)data);
            }
            if (data.Type == VariantValue.ValueType.Float)
            {
                return new JValue((float)data);
            }
            if (data.Type == VariantValue.ValueType.Double)
            {
                return new JValue((double)data);
            }
            if (data.Type == VariantValue.ValueType.Decimal)
            {
                return new JValue((decimal)data);
            }
            if (data.Type == VariantValue.ValueType.String)
            {
                return new JValue((string)data);
            }
            if (data.Type == VariantValue.ValueType.Bytes)
            {
                return new JValue((byte[])data);
            }
            throw new Exception($"Unsupported variant value type {data.Type}");
        }

        private static JArray VariantToJArray(VariantArray data, JArray jArray)
        {
            foreach (var item in data.GetItems())
            {
                jArray.Add(ToJToken(item));
            }
            return jArray;
        }

        private static JObject VariantToJObject(VariantObject data, JObject jObject)
        {
            foreach (var item in data.GetItems())
            {
                jObject.Add(item.Key.ToString(), ToJToken(item.Value));
            }
            return jObject;
        }
    }
}
