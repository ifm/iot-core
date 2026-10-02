namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents the format of a simple value type element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="type">The format type.</param>
/// <param name="encoding">The format encoding.</param>
/// <param name="ns">The format namespace.</param>
public class ValueFormat(string type, string encoding, string ns) : Format(type, encoding, ns)
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ValueFormat() : this(Types.Value, null, NameSpaces.Json)
    {
    }
}
