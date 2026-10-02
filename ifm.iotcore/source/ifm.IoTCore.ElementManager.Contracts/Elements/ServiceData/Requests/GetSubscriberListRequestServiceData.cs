namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IDeviceElement.GetSubscriberList service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="address">The address of the event element; if null, all event elements.</param>
public class GetSubscriberListRequestServiceData(string address)
{
    /// <summary>
    /// The address of the event element; if null, all event elements.
    /// </summary>
    [VariantProperty("adr", IgnoredIfNull = true)]
    public string Address { get; set; } = address;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public GetSubscriberListRequestServiceData() : this(null)
    {
    }
}