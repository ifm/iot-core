namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of an object type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="ns">The namespace.</param>
public class ObjectFormat(ObjectValuation valuation = null, string ns = null) : Format(Types.Object, null, ns)
{
    /// <summary>
    /// Gets or sets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public ObjectValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ObjectFormat() : this(null)
    {
    }
}