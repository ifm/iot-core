namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IDataElement.SetData service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="value">The value to set.</param>
public class SetDataRequestServiceData(Variant value)
{
    /// <summary>
    /// The value to set.
    /// </summary>
    [VariantProperty("value", Required = true, AlternativeNames = ["newvalue"])]
    public Variant Value { get; set; } = value;

    /// <summary>
    /// The value to set, for backward compatibility.
    /// </summary>
    [VariantProperty("newvalue", IgnoredIfNull = true)]
    public Variant NewValue { get => Value; set => Value = value; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public SetDataRequestServiceData() : this(null)
    {
    }
}