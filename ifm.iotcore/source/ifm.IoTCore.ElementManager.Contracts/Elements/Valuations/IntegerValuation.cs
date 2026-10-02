namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;

/// <summary>
/// Represents the valuation for a signed 8-bit integer type data element.
/// </summary>
public class Int8Valuation : NumberValuation<sbyte>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int8Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public Int8Valuation(sbyte? min,
        sbyte? max,
        sbyte? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for an unsigned 8-bit integer type data element.
/// </summary>
public class UInt8Valuation : NumberValuation<byte>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt8Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public UInt8Valuation(byte? min,
        byte? max,
        byte? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for a signed 16-bit integer type data element.
/// </summary>
public class Int16Valuation : NumberValuation<short>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int16Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public Int16Valuation(short? min,
        short? max,
        short? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for an unsigned 16-bit integer type data element.
/// </summary>
public class UInt16Valuation : NumberValuation<ushort>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt16Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public UInt16Valuation(ushort? min,
        ushort? max,
        ushort? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for a signed 32-bit integer type data element.
/// </summary>
public class Int32Valuation : NumberValuation<int>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int32Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public Int32Valuation(int? min,
        int? max,
        int? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for an unsigned 32-bit integer type data element.
/// </summary>
public class UInt32Valuation : NumberValuation<uint>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt32Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public UInt32Valuation(uint? min,
        uint? max,
        uint? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for a signed 64-bit integer type data element.
/// </summary>
public class Int64Valuation : NumberValuation<long>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int64Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public Int64Valuation(long? min,
        long? max,
        long? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for an unsigned 64-bit integer type data element.
/// </summary>
public class UInt64Valuation : NumberValuation<ulong>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt64Valuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public UInt64Valuation(ulong? min,
        ulong? max,
        ulong? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}

/// <summary>
/// Represents the valuation for a signed 32-bit integer type data element.
/// </summary>
public class IntegerValuation : NumberValuation<int>
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public IntegerValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    public IntegerValuation(int? min,
        int? max,
        int? defaultValue = null) : base(min, max, defaultValue)
    {
    }
}
