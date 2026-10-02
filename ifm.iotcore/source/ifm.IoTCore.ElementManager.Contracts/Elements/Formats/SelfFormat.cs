namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

using ifm.Common.Variant;

/// <summary>
/// Represents the format of a self reference.
/// </summary>
public class SelfFormat : ReferenceFormat
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public SelfFormat() : base(Types.Self)
    {
    }
}