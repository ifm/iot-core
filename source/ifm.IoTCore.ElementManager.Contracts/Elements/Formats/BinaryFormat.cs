namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents a data format for binary content encoded using Base64.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="ns">The optional namespace to associate with the format. May be null to use the default namespace.</param>
public class BinaryFormat(string ns = null) : Format(Types.Binary, Encodings.Base64, ns)
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public BinaryFormat() : this(null)
    {
    }
}

