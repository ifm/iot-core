namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Responses;

using ifm.Common.Variant;

/// <summary>
/// Represents the outgoing data for a IDataElement.GetData service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="value">The value to get.</param>
/// <param name="timeStamp">The time stamp of the value.</param>
public class GetDataResponseServiceData(Variant value, long? timeStamp = null)
{
    /// <summary>
    /// The value to get.
    /// </summary>
    [VariantProperty("value", Required = true)]
    public Variant Value { get; set; } = value;

    /// <summary>
    ///  The time stamp of the value.
    /// </summary>
    [VariantProperty("timestamp", IgnoredIfNull = true)]
    public long? TimeStamp { get; set; } = timeStamp;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public GetDataResponseServiceData() : this(null)
    {
    }
}