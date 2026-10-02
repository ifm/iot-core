namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;
using Formats;

/// <summary>
/// Represents the valuation for a map type data element.
/// </summary>
public class MapValuation
{
    /// <summary>
    /// Gets the format of a value.
    /// </summary>
    [VariantProperty("format", IgnoredIfNull = true)]
    public Format Format { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public MapValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="format">The format for the field.</param>
    public MapValuation(Format format)
    {
        Format = format;
    }
}