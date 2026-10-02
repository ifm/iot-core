namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of an array type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class ArrayFormat(ArrayValuation valuation = null, string ns = null) : Format(Types.Array, null, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public ArrayValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ArrayFormat() : this(null)
    {
    }
}