namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of a map type element.
/// Initializes a new instance of the class.
/// </summary>
public class MapFormat(MapValuation valuation = null, string ns = null) : Format(Types.Map, null, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public MapValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public MapFormat() : this(null)
    {
    }
}