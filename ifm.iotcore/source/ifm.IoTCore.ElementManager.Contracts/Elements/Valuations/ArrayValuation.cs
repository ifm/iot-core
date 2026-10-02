namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;
using Formats;

/// <summary>
/// Represents the valuation for an array type data element.
/// </summary>
public class ArrayValuation
{
    /// <summary>
    /// Gets the format for an array item.
    /// </summary>
    [VariantProperty("format", Required = true)]
    public Format Format { get; set; }

    /// <summary>
    /// Gets the size of the array.
    /// </summary>
    [VariantProperty("length", IgnoredIfNull = true)]
    public int? Length { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ArrayValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="format">The format for an array item.</param>
    /// <param name="length">The size of the array.</param>
    public ArrayValuation(Format format, int? length = null)
    {
        Format = format;
        Length = length;
    }
}