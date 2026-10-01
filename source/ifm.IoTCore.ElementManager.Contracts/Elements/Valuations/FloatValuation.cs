namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using ifm.Common.Variant;

/// <summary>
/// Represents the valuation for a float type data element.
/// </summary>
public class FloatValuation : NumberValuation<float>
{
    /// <summary>
    /// Gets the number of decimals.
    /// </summary>
    [VariantProperty("decimalplaces", IgnoredIfNull = true)]
    public int? Decimals { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public FloatValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="decimals">The number of decimals.</param>
    /// <param name="defaultValue">The default value.</param>
    public FloatValuation(float? min,
        float? max,
        int? decimals,
        float? defaultValue = null) : base(min, max, defaultValue)
    {
        Decimals = decimals;
    }
}

/// <summary>
/// Represents the valuation for a double type data element.
/// </summary>
public class DoubleValuation : NumberValuation<double>
{
    /// <summary>
    /// Gets the number of decimals.
    /// </summary>
    [VariantProperty("decimalplaces", IgnoredIfNull = true)]
    public int? Decimals { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public DoubleValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="decimals">The number of decimals.</param>
    /// <param name="defaultValue">The default value.</param>
    public DoubleValuation(double? min,
        double? max,
        int? decimals,
        double? defaultValue = null) : base(min, max, defaultValue)
    {
        Decimals = decimals;
    }
}