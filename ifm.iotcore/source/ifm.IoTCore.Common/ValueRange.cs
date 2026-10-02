namespace ifm.IoTCore.Common;

using ifm.Common.Variant;

/// <summary>
/// Represents a value range.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="lowerValue">The lower value.</param>
/// <param name="upperValue">The upper value.</param>
/// <param name="text">The value range text.</param>
public class ValueRange<T>(T lowerValue, T upperValue, string text)
{
    /// <summary>
    /// Gets the lower value.
    /// </summary>
    [VariantProperty("lower_value", Required = true)]
    public T LowerValue { get; set; } = lowerValue;

    /// <summary>
    /// Gets the upper value.
    /// </summary>
    [VariantProperty("upper_value", Required = true)]
    public T UpperValue { get; set; } = upperValue;

    /// <summary>
    /// Gets the value range text.
    /// </summary>
    [VariantProperty("text", IgnoredIfNull = true)]
    public string Text { get; set; } = text;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public ValueRange() : this(default, default, null)
    {
    }
}
