using ifm.Common.Variant;

namespace ifm.IoTCore.ElementManager.Contracts.Elements.Formats;

/// <summary>
/// Represents no format.
/// </summary>
public class NoFormat : Format
{
    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public NoFormat() : base(null, null, null)
    {
    }
}