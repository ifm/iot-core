namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents the format of a reference element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="reference">The reference.</param>
public class ReferenceFormat(string reference) : Format(Types.Reference, null, null)
{
    /// <summary>
    /// Gets the reference.
    /// </summary>
    [VariantProperty("ref", Required = true)]
    public string Reference { get; set; } = reference;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ReferenceFormat() : this(null)
    {
    }
}