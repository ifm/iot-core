namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of an integer enumeration type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class IntegerEnumFormat(IntegerEnumValuation valuation = null, string ns = null) : ValueFormat(Types.Enum, Encodings.Integer, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public IntegerEnumValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public IntegerEnumFormat() : this(null)
    {
    }
}