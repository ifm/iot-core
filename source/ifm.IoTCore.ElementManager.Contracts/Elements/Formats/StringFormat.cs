namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;
using Valuations;

/// <summary>
/// Represents the format of a string type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="valuation">The valuation.</param>
/// <param name="encoding">The encoding.</param>
/// <param name="ns">The namespace.</param>
public class StringFormat(StringValuation valuation = null, string encoding = Format.Encodings.Utf8, string ns = null) : ValueFormat(Types.String, encoding, ns)
{
    /// <summary>
    /// Gets the valuation.
    /// </summary>
    [VariantProperty("valuation", IgnoredIfNull = true)]
    public StringValuation Valuation { get; set; } = valuation;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public StringFormat() : this(null)
    {
    }
}