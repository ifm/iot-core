namespace ifm.IoTCore.Common;

using ifm.Common.Variant;

/// <summary>
/// Represents a single value.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="value">The value.</param>
/// <param name="text">The value text.</param>
public class SingleValue<T>(T value, string text)
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    [VariantProperty("value", Required = true)]
    public T Value { get; set; } = value;

    /// <summary>
    /// Gets the value text.
    /// </summary>
    [VariantProperty("text", IgnoredIfNull = true)]
    public string Text { get; set; } = text;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public SingleValue() : this(default, null)
    {
    }
}

