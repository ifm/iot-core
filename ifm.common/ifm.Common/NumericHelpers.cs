namespace ifm.Common;

using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides numeric helpers.
/// </summary>
public static class NumericHelpers
{
    /// <summary>
    /// The bit length of signed byte.
    /// </summary>
    public const int BitLengthOfSByte = sizeof(sbyte) * 8;

    /// <summary>
    /// The bit length of byte.
    /// </summary>
    public const int BitLengthOfByte = sizeof(byte) * 8;

    /// <summary>
    /// The bit length of short.
    /// </summary>
    public const int BitLengthOfShort = sizeof(short) * 8;

    /// <summary>
    /// The bit length of unsigned short.
    /// </summary>
    public const int BitLengthOfUShort = sizeof(ushort) * 8;

    /// <summary>
    /// The bit length of int.
    /// </summary>
    public const int BitLengthOfInt = sizeof(int) * 8;

    /// <summary>
    /// The bit length of unsigned int.
    /// </summary>
    public const int BitLengthOfUInt = sizeof(uint) * 8;

    /// <summary>
    /// The bit length of long.
    /// </summary>
    public const int BitLengthOfLong = sizeof(long) * 8;

    /// <summary>
    /// The bit length of unsigned long.
    /// </summary>
    public const int BitLengthOfULong = sizeof(ulong) * 8;

    /// <summary>
    /// The bit length of float.
    /// </summary>
    public const int BitLengthOfFloat = sizeof(float) * 8;

    /// <summary>
    /// The bit length of double.
    /// </summary>
    public const int BitLengthOfDouble = sizeof(double) * 8;

    /// <summary>
    /// The bit length of decimal.
    /// </summary>
    public const int BitLengthOfDecimal = sizeof(decimal) * 8;

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Min(sbyte a, sbyte b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Max(sbyte a, sbyte b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Min(byte a, byte b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Max(byte a, byte b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Min(short a, short b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Max(short a, short b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Min(ushort a, ushort b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Max(ushort a, ushort b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int a, int b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Min(uint a, uint b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Max(uint a, uint b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(long a, long b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(long a, long b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Min(ulong a, ulong b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Max(ulong a, ulong b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float a, float b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(double a, double b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(double a, double b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Returns the lower value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Min(decimal a, decimal b)
    {
        return a < b ? a : b;
    }

    /// <summary>
    /// Returns the upper value of the given values.
    /// </summary>
    /// <param name="a">The one value.</param>
    /// <param name="b">The other value.</param>
    /// <returns>The lower value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Max(decimal a, decimal b)
    {
        return a > b ? a : b;
    }

    /// <summary>
    /// Combines the provided values.
    /// </summary>
    /// <param name="high">The high part.</param>
    /// <param name="low">The low part.</param>
    /// <returns>The combined value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Combine(byte high, byte low)
    {
        return (ushort)((high << 8) | low);
    }

    /// <summary>
    /// Cracks the provided value in high and low part.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="high">The high part.</param>
    /// <param name="low">The low part.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Crack(ushort value, out byte high, out byte low)
    {
        low = (byte)value;
        high = (byte)(value >> 8);
    }

    /// <summary>
    /// Combines the provided values.
    /// </summary>
    /// <param name="high">The high part.</param>
    /// <param name="low">The low part.</param>
    /// <returns>The combined value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Combine(ushort high, ushort low)
    {
        return ((uint)high << 16) | low;
    }

    /// <summary>
    /// Cracks the provided value in high and low part.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="high">The high part.</param>
    /// <param name="low">The low part.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Crack(uint value, out ushort high, out ushort low)
    {
        low = (ushort)value;
        high = (ushort)(value >> 16);
    }

    /// <summary>
    /// Combines the provided values.
    /// </summary>
    /// <param name="high">The high part.</param>
    /// <param name="low">The low part.</param>
    /// <returns>The combined value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Combine(uint high, uint low)
    {
        return ((ulong)high << 32) | low;
    }

    /// <summary>
    /// Cracks the provided value in high and low part.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="high">The high part.</param>
    /// <param name="low">The low part.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Crack(ulong value, out uint high, out uint low)
    {
        low = (uint)value;
        high = (uint)(value >> 32);
    }

    /// <summary>
    /// Gets the low part from the provided value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The low part.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetLow(ushort value)
    {
        return (byte)value;
    }

    /// <summary>
    /// Gets the high part from the provided value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The high part.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetHigh(ushort value)
    {
        return (byte)(value >> 16);
    }

    /// <summary>
    /// Gets the low part from the provided value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The low part.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetLow(uint value)
    {
        return (ushort)value;
    }

    /// <summary>
    /// Gets the high part from the provided value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The high part.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort GetHigh(uint value)
    {
        return (ushort)(value >> 16);
    }

    /// <summary>
    /// Gets the low part from the provided value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The low part.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetLow(ulong value)
    {
        return (uint)value;
    }

    /// <summary>
    /// Gets the high part from the provided value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The high part.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetHigh(ulong value)
    {
        return (uint)(value >> 32);
    }

    /// <summary>
    /// Combines two values into a hash code.
    /// </summary>
    /// <param name="value1">The first value to combine into the hash code.</param>
    /// <param name="value2">The second value to combine into the hash code.</param>
    /// <returns>The hash code that represents the two values.</returns>
    public static int CombineHashCodes(int value1, int value2)
    {
        return ((value1 << 5) + value1) ^ value2;
    }

    /// <summary>
    /// Compares two float values with precision.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="otherValue">The value to compare against.</param>
    /// <param name="precision">The precision of the comparison. If the difference between the values is less than the precision, the values are considered equal.</param>
    /// <returns>true if the difference between the values is within the precision; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsWithPrecision(this float value, float otherValue, float precision = 0.001f)
    {
        return Math.Abs(value - otherValue) < precision;
    }

    /// <summary>
    /// Compares two double values with precision.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="otherValue">The value to compare against.</param>
    /// <param name="precision">The precision of the comparison. If the difference between the values is less than the precision, the values are considered equal.</param>
    /// <returns>true if the difference between the values is within the precision; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsWithPrecision(this double value, double otherValue, double precision = 0.001)
    {
        return Math.Abs(value - otherValue) < precision;
    }

    /// <summary>
    /// Compares two decimal values with precision.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="otherValue">The value to compare against.</param>
    /// <param name="precision">The precision of the comparison. If the difference between the values is less than the precision, the values are considered equal.</param>
    /// <returns>true if the difference between the values is within the precision; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsWithPrecision(this decimal value, decimal otherValue, decimal precision = 0.001m)
    {
        return Math.Abs(value - otherValue) < precision;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this sbyte value, sbyte min, sbyte max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this byte value, byte min, byte max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this short value, short min, short max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this ushort value, ushort min, ushort max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this uint value, uint min, uint max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this long value, long min, long max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this ulong value, ulong min, ulong max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this float value, float min, float max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this double value, double min, double max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if a value is within the specified range.
    /// </summary>
    /// <param name="value">The value to compare.</param>
    /// <param name="min">The lower value.</param>
    /// <param name="max">The upper value.</param>
    /// <returns>True if the value is within the range; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWithinRange(this decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Checks if the bit value is set.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitValue">The bit value to compare.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(this byte value, byte bitValue)
    {
        return (value & bitValue) == bitValue;
    }

    /// <summary>
    /// Checks if the bit value is set.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitValue">The bit value to compare.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(this ushort value, ushort bitValue)
    {
        return (value & bitValue) == bitValue;
    }

    /// <summary>
    /// Checks if the bit value is set.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitValue">The bit value to compare.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(this uint value, uint bitValue)
    {
        return (value & bitValue) == bitValue;
    }

    /// <summary>
    /// Checks if the bit value is set.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitValue">The bit value to compare.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(this ulong value, ulong bitValue)
    {
        return (value & bitValue) == bitValue;
    }

    /// <summary>
    /// Gets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this byte value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfByte, nameof(bitOffset));
        return (value & (1U << bitOffset)) != 0;
    }

    /// <summary>
    /// Sets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(this ref byte value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfByte, nameof(bitOffset));
        value |= (byte)(1U << bitOffset);
    }

    /// <summary>
    /// Gets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this ushort value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfUShort, nameof(bitOffset));
        return (value & (1U << bitOffset)) != 0;
    }

    /// <summary>
    /// Sets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(this ref ushort value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfUShort, nameof(bitOffset));
        value |= (ushort)(1U << bitOffset);
    }

    /// <summary>
    /// Gets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this uint value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfUInt, nameof(bitOffset));
        return (value & (1U << bitOffset)) != 0;
    }

    /// <summary>
    /// Sets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(this ref uint value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfUInt, nameof(bitOffset));
        value |= 1U << bitOffset;
    }

    /// <summary>
    /// Gets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    /// <returns>True if the bit is set; otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this ulong value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfULong, nameof(bitOffset));
        return (value & (1UL << bitOffset)) != 0;
    }

    /// <summary>
    /// Sets the bit at the specified offset.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="bitOffset">The offset.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(this ref ulong value, int bitOffset)
    {
        ExceptionHelpers.ThrowIfOutOfRange(() => bitOffset is >= 0 and < BitLengthOfULong, nameof(bitOffset));
        value |= 1UL << bitOffset;
    }
}