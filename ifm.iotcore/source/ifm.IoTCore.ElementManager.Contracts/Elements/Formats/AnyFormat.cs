namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents the format of an any type element.
/// </summary>
public class AnyFormat : Format
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public AnyFormat() : base(Types.Any, null, NameSpaces.Json)
    {
    }
}