namespace ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;

using System.Collections.Generic;
using ifm.Common.Variant;

/// <summary>
/// Represents the valuation for an integer enumeration type data element.
/// </summary>
public class IntegerEnumValuation
{
    /// <summary>
    /// Gets the list of values.
    /// </summary>
    [VariantProperty("valuelist", IgnoredIfNull = true)]
    public Dictionary<string, string> Values { get; set; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    [VariantProperty("default", IgnoredIfNull = true)]
    public int? DefaultValue { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public IntegerEnumValuation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="values">The list of values.</param>
    /// <param name="defaultValue">The default value.</param>
    public IntegerEnumValuation(Dictionary<string, string> values, int? defaultValue = null)
    {
        Values = values;
        DefaultValue = defaultValue;
    }
}