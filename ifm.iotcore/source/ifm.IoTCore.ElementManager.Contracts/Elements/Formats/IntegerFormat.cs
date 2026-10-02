namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of a signed 8-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class Int8Format(Int8Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Int8, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public Int8Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int8Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of an unsigned 8-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class UInt8Format(UInt8Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Int8, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public UInt8Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt8Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of a signed 16-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class Int16Format(Int16Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Int16, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public Int16Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int16Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of an unsigned 16-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class UInt16Format(UInt16Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.UInt16, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public UInt16Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt16Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of a signed 32-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class Int32Format(Int32Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Int32, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public Int32Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int32Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of an unsigned 32-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class UInt32Format(UInt32Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.UInt32, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public UInt32Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt32Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of a signed 64-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class Int64Format(Int64Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Int64, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public Int64Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public Int64Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of an unsigned 64-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class UInt64Format(UInt64Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.UInt64, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public UInt64Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UInt64Format() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of a signed 32-bit integer type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class IntegerFormat(Int64Valuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Integer, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public Int64Valuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public IntegerFormat() : this(null)
    {
    }
}
