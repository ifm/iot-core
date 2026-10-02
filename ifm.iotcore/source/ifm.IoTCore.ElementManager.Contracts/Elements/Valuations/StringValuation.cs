namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;

/// <summary>
/// Represents the valuation for a string type data element.
/// </summary>
public class StringValuation
{
    /// <summary>
    /// Gets the minimum length.
    /// </summary>
    [VariantProperty("minlength", IgnoredIfNull = true)]
    public int? MinLength { get; set; }

    /// <summary>
    /// Gets the maximum length.
    /// </summary>
    [VariantProperty("maxlength", IgnoredIfNull = true)]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Gets the evaluation pattern.
    /// </summary>
    [VariantProperty("pattern", IgnoredIfNull = true)]
    public string Pattern { get; set; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    [VariantProperty("default", IgnoredIfNull = true)]
    public string DefaultValue { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public StringValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="minLength">The minimum length.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="pattern">The evaluation pattern.</param>
    /// <param name="defaultValue">The default value.</param>
    public StringValuation(int? minLength, int? maxLength, string pattern = null, string defaultValue = null)
    {
        MinLength = minLength;
        MaxLength = maxLength;
        Pattern = pattern;
        DefaultValue = defaultValue;
    }
}