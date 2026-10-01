namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;

/// <summary>
/// Represents the valuation for a number type data element.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class NumberValuation<T> where T : struct
{
    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    [VariantProperty("min", IgnoredIfNull = true)]
    public T? Min { get; set; }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    [VariantProperty("max", IgnoredIfNull = true)]
    public T? Max { get; set; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    [VariantProperty("default", IgnoredIfNull = true)]
    public T? DefaultValue { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    protected NumberValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="defaultValue">The default value.</param>
    protected NumberValuation(T? min,
        T? max,
        T? defaultValue = null)
    {
        Min = min;
        Max = max;
        DefaultValue = defaultValue;
    }
}