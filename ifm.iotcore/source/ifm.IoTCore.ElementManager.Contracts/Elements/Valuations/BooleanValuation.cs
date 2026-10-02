namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;

/// <summary>
/// Represents the valuation for a boolean type data element.
/// </summary>
public class BooleanValuation
{
    /// <summary>
    /// Gets the default value.
    /// </summary>
    [VariantProperty("default", IgnoredIfNull = true)]
    public bool? DefaultValue { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public BooleanValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    public BooleanValuation(bool? defaultValue = null)
    {
        DefaultValue = defaultValue;
    }
}