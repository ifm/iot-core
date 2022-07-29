namespace ifmIoTCore.Common.Variant
{
    using System;
    using System.Linq;
    using Common;

    public class VariantValue : Variant, IEquatable<VariantValue>
    {
        public enum ValueType
        {
            Null,
            Boolean,
            Character,
            SignedByte,
            Byte,
            Short,
            UnsignedShort, 
            Integer,
            UnsignedInteger,
            Long,
            UnsignedLong,
            Float,
            Double,
            Decimal,
            String,
            Bytes
        }

        public ValueType Type { get; }

        private readonly object _value;

        public VariantValue()
        {
            Type = ValueType.Null;
            _value = null;
        }

        public static VariantValue CreateNull()
        {
            return new VariantValue();
        }

        public VariantValue(VariantValue other)
        {
            Type = other.Type;
            _value = other._value;
        }

        public VariantValue(bool value)
        {
            Type = ValueType.Boolean;
            _value = value;
        }

        public static implicit operator VariantValue(bool value) => new VariantValue(value);

        public static explicit operator bool(VariantValue value) => value.GetBoolean();

        private bool GetBoolean()
        {
            //return (bool)_value;
            return Convert.ToBoolean(_value);
        }

        public VariantValue(char value)
        {
            Type = ValueType.Character;
            _value = value;
        }

        public static implicit operator VariantValue(char value) => new VariantValue(value);

        public static explicit operator char(VariantValue value) => value.GetCharacter();

        private char GetCharacter()
        {
            //return (char)_value;
            return Convert.ToChar(_value);
        }

        public VariantValue(sbyte value)
        {
            Type = ValueType.SignedByte;
            _value = value;
        }

        public static implicit operator VariantValue(sbyte value) => new VariantValue(value);

        public static explicit operator sbyte(VariantValue value) => value.GetSignedByte();

        private sbyte GetSignedByte()
        {
            //return (sbyte)_value;
            return Convert.ToSByte(_value);
        }

        public VariantValue(byte value)
        {
            Type = ValueType.Byte;
            _value = value;
        }

        public static implicit operator VariantValue(byte value) => new VariantValue(value);

        public static explicit operator byte(VariantValue value) => value.GetByte();

        private byte GetByte()
        {
            //return (byte)_value;
            return Convert.ToByte(_value);
        }

        public VariantValue(short value)
        {
            Type = ValueType.Short;
            _value = value;
        }

        public static implicit operator VariantValue(short value) => new VariantValue(value);

        public static explicit operator short(VariantValue value) => value.GetShort();

        private short GetShort()
        {
            //return (short)_value;
            return Convert.ToInt16(_value);
        }

        public VariantValue(ushort value)
        {
            Type = ValueType.UnsignedShort;
            _value = value;
        }

        public static implicit operator VariantValue(ushort value) => new VariantValue(value);

        public static explicit operator ushort(VariantValue value) => value.GetUnsignedShort();

        private ushort GetUnsignedShort()
        {
            //return (ushort)_value;
            return Convert.ToUInt16(_value);
        }

        public VariantValue(int value)
        {
            Type = ValueType.Integer;
            _value = value;
        }

        public static implicit operator VariantValue(int value) => new VariantValue(value);

        public static explicit operator int(VariantValue value) => value.GetInteger();

        private int GetInteger()
        {
            //return (int)_value;
            return Convert.ToInt32(_value);
        }

        public VariantValue(uint value)
        {
            Type = ValueType.UnsignedInteger;
            _value = value;
        }

        public static implicit operator VariantValue(uint value) => new VariantValue(value);

        public static explicit operator uint(VariantValue value) => value.GetUnsignedInteger();

        private uint GetUnsignedInteger()
        {
            //return (uint)_value;
            return Convert.ToUInt32(_value);
        }

        public VariantValue(long value)
        {
            Type = ValueType.Long;
            _value = value;
        }

        public static implicit operator VariantValue(long value) => new VariantValue(value);

        public static explicit operator long(VariantValue value) => value.GetLong();

        private long GetLong()
        {
            //return (long)_value;
            return Convert.ToInt64(_value);
        }

        public VariantValue(ulong value)
        {
            Type = ValueType.UnsignedLong;
            _value = value;
        }

        public static implicit operator VariantValue(ulong value) => new VariantValue(value);

        public static explicit operator ulong(VariantValue value) => value.GetUnsignedLong();

        private ulong GetUnsignedLong()
        {
            //return (ulong)_value;
            return Convert.ToUInt64(_value);
        }

        public VariantValue(float value)
        {
            Type = ValueType.Float;
            _value = value;
        }

        public static implicit operator VariantValue(float value) => new VariantValue(value);

        public static explicit operator float(VariantValue value) => value.GetFloat();

        private float GetFloat()
        {
            //return (float)_value;
            return Convert.ToSingle(_value);
        }

        public VariantValue(double value)
        {
            Type = ValueType.Double;
            _value = value;
        }

        public static implicit operator VariantValue(double value) => new VariantValue(value);

        public static explicit operator double(VariantValue value) => value.GetDouble();

        private double GetDouble()
        {
            //return (double)_value;
            return Convert.ToDouble(_value);
        }

        public VariantValue(decimal value)
        {
            Type = ValueType.Decimal;
            _value = value;
        }

        public static implicit operator VariantValue(decimal value) => new VariantValue(value);

        public static explicit operator decimal(VariantValue value) => value.GetDecimal();

        private decimal GetDecimal()
        {
            //return (decimal)_value;
            return Convert.ToDecimal(_value);
        }

        public VariantValue(string value)
        {
            Type = ValueType.String;
            _value = value;
        }

        public static implicit operator VariantValue(string value) => new VariantValue(value);

        public static explicit operator string(VariantValue value) => value.GetString();

        private string GetString()
        {
            //return (string)_value;
            return _value != null ? Convert.ToString(_value) : null;
        }

        public VariantValue(byte[] value)
        {
            Type = ValueType.Bytes;
            _value = value;
        }

        public static implicit operator VariantValue(byte[] value) => new VariantValue(value);

        public static explicit operator byte[](VariantValue value) => value.GetBytes();

        private byte[] GetBytes()
        {
            return (byte[])_value;
        }

        public override string ToString()
        {
            return _value?.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((VariantValue)obj);
        }

        public bool Equals(VariantValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (Type == ValueType.Null)
            {
                return _value == other._value;
            }
            if (Type == ValueType.Boolean)
            {
                return GetBoolean() == other.GetBoolean();
            }
            if (Type == ValueType.SignedByte)
            {
                return GetSignedByte() == other.GetSignedByte();
            }
            if (Type == ValueType.Byte)
            {
                return GetByte() == other.GetByte();
            }
            if (Type == ValueType.Short)
            {
                return GetShort() == other.GetShort();
            }
            if (Type == ValueType.UnsignedShort)
            {
                return GetUnsignedShort() == other.GetUnsignedShort();
            }
            if (Type == ValueType.Integer)
            {
                return GetInteger() == other.GetInteger();
            }
            if (Type == ValueType.UnsignedInteger)
            {
                return GetUnsignedInteger() == other.GetUnsignedInteger();
            }
            if (Type == ValueType.Long)
            {
                return GetLong() == other.GetLong();
            }
            if (Type == ValueType.UnsignedLong)
            {
                return GetUnsignedLong() == other.GetUnsignedLong();
            }
            if (Type == ValueType.Float)
            {
                return GetFloat().EqualsWithPrecision(other.GetFloat());
            }
            if (Type == ValueType.Double)
            {
                return GetDouble().EqualsWithPrecision(other.GetDouble());
            }
            if (Type == ValueType.Decimal)
            {
                return GetDecimal().EqualsWithPrecision(other.GetDecimal());
            }
            if (Type == ValueType.String)
            {
                return GetString() == other.GetString();
            }
            if (Type == ValueType.Bytes)
            {
                return GetBytes().SequenceEqual(other.GetBytes());
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_value != null ? _value.GetHashCode() : 0) * 397) ^ (int)Type;
            }
        }
    }
}
