namespace ifm.Common.Variant;

using System;
using Resources;

/// <summary>
/// Provides a Variant class for simple data types.
/// </summary>
public class VariantValue : Variant, IEquatable<VariantValue>
{
    /// <summary>
    /// The value types.
    /// </summary>
    public enum ValueTypes
    {
        /// <summary> The Null value type.</summary>
        Null,
        /// <summary> The Boolean value type.</summary>
        Boolean,
        /// <summary> The Character value type.</summary>
        Character,
        /// <summary> The Int8 value type.</summary>
        Int8,
        /// <summary> The UInt8 value type.</summary>
        UInt8,
        /// <summary> The Int16 value type.</summary>
        Int16,
        /// <summary> The UInt16 value type.</summary>
        UInt16,
        /// <summary> The Int32 value type.</summary>
        Int32,
        /// <summary> The UInt32 value type.</summary>
        UInt32,
        /// <summary> The Int64 value type.</summary>
        Int64,
        /// <summary> The UInt64 value type.</summary>
        UInt64,
        /// <summary> The Float value type.</summary>
        Float,
        /// <summary> The Double value type.</summary>
        Double,
        /// <summary> The Decimal value type.</summary>
        Decimal,
        /// <summary> The String value type.</summary>
        String,
        /// <summary> The DateTime value type.</summary>
        DateTime,
        /// <summary> The TimeSpan value type.</summary>
        TimeSpan,
        /// <summary> The Uri value type.</summary>
        Uri,
        /// <summary> The Guid value type.</summary>
        Guid
    }

    /// <summary>
    /// Gets the value type.
    /// </summary>
    public ValueTypes Type { get; }

    /// <summary>
    /// Gets the value as object.
    /// </summary>
    public object Value { get; }

    private readonly bool _strict;

    /// <summary>
    /// Initializes a new class instance with null value.
    /// </summary>
    public VariantValue()
    {
        Type = ValueTypes.Null;
        Value = null;
        _strict = false;
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public VariantValue(VariantValue value)
    {
        Type = value.Type;
        Value = value.Value;
        _strict = value._strict;
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(object value, bool strict = true)
    {
        Type = value switch
        {
            bool => ValueTypes.Boolean,
            char => ValueTypes.Character,
            sbyte => ValueTypes.Int8,
            byte => ValueTypes.UInt8,
            short => ValueTypes.Int16,
            ushort => ValueTypes.UInt16,
            int => ValueTypes.Int32,
            uint => ValueTypes.UInt32,
            long => ValueTypes.Int64,
            ulong => ValueTypes.UInt64,
            float => ValueTypes.Float,
            double => ValueTypes.Double,
            decimal => ValueTypes.Decimal,
            string => ValueTypes.String,
            DateTime => ValueTypes.DateTime,
            TimeSpan => ValueTypes.TimeSpan,
            Uri => ValueTypes.Uri,
            Guid => ValueTypes.Guid,
            _ => throw new ArgumentOutOfRangeException()
        };
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(bool value, bool strict = true)
    {
        Type = ValueTypes.Boolean;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(bool value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static explicit operator bool(VariantValue value) => value?.GetBool() ?? false;// throw new ArgumentNullException(nameof(value));

    private bool GetBool()
    {
        if (Type == ValueTypes.Boolean) return (bool)Value;
        if (_strict) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Boolean));
        return Convert.ToBoolean(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(char value, bool strict = true)
    {
        Type = ValueTypes.Character;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(char value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator char(VariantValue value) => value?.GetChar() ?? '\0';// throw new ArgumentNullException(nameof(value));

    private char GetChar()
    {
        if (Type == ValueTypes.Character) return (char)Value;
        if (_strict) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Character));
        return Convert.ToChar(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(sbyte value, bool strict = true)
    {
        Type = ValueTypes.Int8;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(sbyte value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator sbyte(VariantValue value) => value?.GetSByte() ?? 0;// throw new ArgumentNullException(nameof(value));

    private sbyte GetSByte()
    {
        if (Type == ValueTypes.Int8) return (sbyte)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Int8));
        return Convert.ToSByte(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(byte value, bool strict = true)
    {
        Type = ValueTypes.UInt8;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(byte value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator byte(VariantValue value) => value?.GetByte() ?? 0;// throw new ArgumentNullException(nameof(value));

    private byte GetByte()
    {
        if (Type == ValueTypes.UInt8) return (byte)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.UInt8));
        return Convert.ToByte(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(short value, bool strict = true)
    {
        Type = ValueTypes.Int16;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(short value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator short(VariantValue value) => value?.GetShort() ?? 0;// throw new ArgumentNullException(nameof(value));

    private short GetShort()
    {
        if (Type == ValueTypes.Int16) return (short)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Int16));
        return Convert.ToInt16(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(ushort value, bool strict = true)
    {
        Type = ValueTypes.UInt16;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(ushort value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator ushort(VariantValue value) => value?.GetUShort() ?? 0;// throw new ArgumentNullException(nameof(value));

    private ushort GetUShort()
    {
        if (Type == ValueTypes.UInt16) return (ushort)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.UInt16));
        return Convert.ToUInt16(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(int value, bool strict = true)
    {
        Type = ValueTypes.Int32;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(int value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator int(VariantValue value) => value?.GetInt() ?? 0;// throw new ArgumentNullException(nameof(value));

    private int GetInt()
    {
        if (Type == ValueTypes.Int32) return (int)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Int32));
        return Convert.ToInt32(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(uint value, bool strict = true)
    {
        Type = ValueTypes.UInt32;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(uint value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator uint(VariantValue value) => value?.GetUInt() ?? 0;// throw new ArgumentNullException(nameof(value));

    private uint GetUInt()
    {
        if (Type == ValueTypes.UInt32) return (uint)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.UInt32));
        return Convert.ToUInt32(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(long value, bool strict = true)
    {
        Type = ValueTypes.Int64;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(long value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator long(VariantValue value) => value?.GetLong() ?? 0;// throw new ArgumentNullException(nameof(value));

    private long GetLong()
    {
        if (Type == ValueTypes.Int64) return (long)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Int64));
        return Convert.ToInt64(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(ulong value, bool strict = true)
    {
        Type = ValueTypes.UInt64;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(ulong value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator ulong(VariantValue value) => value?.GetULong() ?? 0;// throw new ArgumentNullException(nameof(value));

    private ulong GetULong()
    {
        if (Type == ValueTypes.UInt64) return (ulong)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.UInt64));
        return Convert.ToUInt64(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(float value, bool strict = true)
    {
        Type = ValueTypes.Float;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(float value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator float(VariantValue value) => value?.GetFloat() ?? 0;// throw new ArgumentNullException(nameof(value));

    private float GetFloat()
    {
        if (Type == ValueTypes.Float) return (float)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Float));
        return Convert.ToSingle(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(double value, bool strict = true)
    {
        Type = ValueTypes.Double;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(double value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator double(VariantValue value) => value?.GetDouble() ?? 0;// throw new ArgumentNullException(nameof(value));

    private double GetDouble()
    {
        if (Type == ValueTypes.Double) return (double)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Double));
        return Convert.ToDouble(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(decimal value, bool strict = true)
    {
        Type = ValueTypes.Decimal;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(decimal value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator decimal(VariantValue value) => value?.GetDecimal() ?? 0;// throw new ArgumentNullException(nameof(value));

    private decimal GetDecimal()
    {
        if (Type == ValueTypes.Decimal) return (decimal)Value;
        if (_strict && !IsNumberType()) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Decimal));
        return Convert.ToDecimal(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(string value, bool strict = true)
    {
        Type = ValueTypes.String;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(string value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator string(VariantValue value) => value?.GetString();

    private string GetString()
    {
        if (Type == ValueTypes.String) return (string)Value;
        if (_strict) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.String));
        return Convert.ToString(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    /// <param name="strict">If true only conversion between compatible types is supported; otherwise not.</param>
    public VariantValue(DateTime value, bool strict = true)
    {
        Type = ValueTypes.DateTime;
        Value = value;
        _strict = strict;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(DateTime value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator DateTime(VariantValue value) => value?.GetDateTime() ?? default;// throw new ArgumentNullException(nameof(value));

    private DateTime GetDateTime()
    {
        if (Type == ValueTypes.DateTime) return (DateTime)Value;
        if (_strict) throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.DateTime));
        return Convert.ToDateTime(Value);
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public VariantValue(TimeSpan value)
    {
        Type = ValueTypes.TimeSpan;
        Value = value;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(TimeSpan value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator TimeSpan(VariantValue value) => value?.GetTimeSpan() ?? TimeSpan.Zero;// throw new ArgumentNullException(nameof(value));

    private TimeSpan GetTimeSpan()
    {
        if (Type == ValueTypes.TimeSpan) return (TimeSpan)Value;
        throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.TimeSpan));
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public VariantValue(Uri value)
    {
        Type = ValueTypes.Uri;
        Value = value;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(Uri value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator Uri(VariantValue value) => value?.GetUri();

    private Uri GetUri()
    {
        if (Type == ValueTypes.Uri) return (Uri)Value;
        throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Uri));
    }

    /// <summary>
    /// Initializes a new class instance with the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public VariantValue(Guid value)
    {
        Type = ValueTypes.Guid;
        Value = value;
    }

    /// <summary>
    /// Performs an implicit conversion of the provided value.
    /// </summary>
    /// <param name="value">The provided value.</param>
    public static implicit operator VariantValue(Guid value) => new(value);

    /// <summary>
    /// Performs an explicit conversion of the provided value.
    /// </summary>
    /// <returns>The value.</returns>
    public static explicit operator Guid(VariantValue value) => value?.GetGuid() ?? Guid.Empty;// throw new ArgumentNullException(nameof(value));

    private Guid GetGuid()
    {
        if (Type == ValueTypes.Guid) return (Guid)Value;
        throw new Exception(string.Format(Resource1.TypeConversionFailed, Value.GetType(), ValueTypes.Guid));
    }

    /// <summary>
    /// Gets a string representation of the value.
    /// </summary>
    /// <returns>The string representation of the value.</returns>
    public override string ToString()
    {
        return Value?.ToString();
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public override bool Equals(object other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;

        return Equals((VariantValue)other);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">The object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(VariantValue other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Type != other.Type) return false;

        return Type switch
        {
            ValueTypes.Null => true,
            ValueTypes.Boolean => GetBool() == other.GetBool(),
            ValueTypes.Character => GetChar() == other.GetChar(),
            ValueTypes.Int8 => GetSByte() == other.GetSByte(),
            ValueTypes.UInt8 => GetByte() == other.GetByte(),
            ValueTypes.Int16 => GetShort() == other.GetShort(),
            ValueTypes.UInt16 => GetUShort() == other.GetUShort(),
            ValueTypes.Int32 => GetInt() == other.GetInt(),
            ValueTypes.UInt32 => GetUInt() == other.GetUInt(),
            ValueTypes.Int64 => GetLong() == other.GetLong(),
            ValueTypes.UInt64 => GetULong() == other.GetULong(),
            ValueTypes.Float => GetFloat().EqualsWithPrecision(other.GetFloat()),
            ValueTypes.Double => GetDouble().EqualsWithPrecision(other.GetDouble()),
            ValueTypes.Decimal => GetDecimal().EqualsWithPrecision(other.GetDecimal()),
            ValueTypes.String => GetString().Equals(other.GetString(), StringComparison.OrdinalIgnoreCase),
            ValueTypes.DateTime => GetDateTime().Equals(other.GetDateTime()),
            ValueTypes.TimeSpan => GetTimeSpan().Equals(other.GetTimeSpan()),
            ValueTypes.Uri => GetUri().Equals(other.GetUri()),
            ValueTypes.Guid => GetGuid().Equals(other.GetGuid()),
            _ => throw new ArgumentOutOfRangeException(nameof(Type))
        };
    }

    /// <summary>
    /// Gets a hash code for the current object.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((Value != null ? Value.GetHashCode() : 0) * 397) ^ (int)Type;
        }
    }

    private bool IsNumberType()
    {
        return Type is 
            ValueTypes.Int8 or ValueTypes.UInt8 or
            ValueTypes.Int16 or ValueTypes.UInt16 or
            ValueTypes.Int32 or ValueTypes.UInt32 or
            ValueTypes.Int64 or ValueTypes.UInt64 or
            ValueTypes.Float or ValueTypes.Double or 
            ValueTypes.Decimal;
    }
}