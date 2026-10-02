namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of a float type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class FloatFormat(FloatValuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Float, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public FloatValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public FloatFormat() : this(null)
    {
    }
}

/// <summary>
/// Represents the format of a double type data element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class DoubleFormat(DoubleValuation valuation = null, string ns = null) : NumberFormat(Types.Number, Encodings.Double, ns)
{
    /// <summary>
    /// Gets the valuation
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public DoubleValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public DoubleFormat() : this(null)
    {
    }
}