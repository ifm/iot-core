namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of a boolean type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class BooleanFormat(BooleanValuation valuation = null, string ns = null) : ValueFormat(Types.Boolean, null, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public BooleanValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public BooleanFormat() : this(null)
    {
    }
}