namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Responses;

using System.Collections.Generic;
using ifm.Common.Variant;

/// <summary>
/// Represents a subscription on an event element.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="address">The address of the event element.</param>
/// <param name="callbackUrl">The url to which the IoTCore is sending events.</param>
/// <param name="dataToSend">List of data element addresses, whose values are sent with the event.</param>
/// <param name="persist">Specifies the persistence duration type.</param>
/// <param name="subscriptionId">The uid which identifies the subscription.</param>
public class GetSubscriberListItem(string address,
    string callbackUrl,
    List<string> dataToSend,
    bool persist,
    int subscriptionId)
{
    /// <summary>
    /// The address of the event element.
    /// </summary>
    [VariantProperty("adr", IgnoredIfNull = true)]
    public string Address { get; set; } = address;

    /// <summary>
    /// The url to which the IoTCore is sending events.
    /// </summary>
    [VariantProperty("callbackurl", Required = true)]
    public string CallbackUrl { get; set; } = callbackUrl;

    /// <summary>
    /// List of data element addresses, whose values are sent with the event.
    /// </summary>
    [VariantProperty("datatosend", IgnoredIfNull = true)]
    public List<string> DataToSend { get; set; } = dataToSend;

    /// <summary>
    /// Specifies the persistence duration type.
    /// </summary>
    [VariantProperty("persist", IgnoredIfNull = true)]
    public bool Persist { get; set; } = persist;

    /// <summary>
    /// The id which identifies the subscription.
    /// </summary>
    [VariantProperty("subscribeid", IgnoredIfNull = true)]
    public int SubscriptionId { get; set; } = subscriptionId;

    /// <summary>
    /// The old id which identifies the subscription.
    /// </summary>
    [VariantProperty("cid", IgnoredIfNull = true)]
    public int Cid { get => SubscriptionId; set => SubscriptionId = value; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public GetSubscriberListItem() : this(null, null, null, false, 0)
    {
    }
}

/// <summary>
/// Represents the outgoing data for a IDeviceElement.GetSubscriberList service call.
/// </summary>
public class GetSubscriberListResponseServiceData : List<GetSubscriberListItem>
{
    /// <summary>
    /// Add a new subscription info item.
    /// </summary>
    /// <param name="address">The address of the event element.</param>
    /// <param name="callback">The url to which the IoTCore is sending events.</param>
    /// <param name="dataToSend">List of data element addresses, whose values are sent with the event.</param>
    /// <param name="persist">Specifies the persistence duration type.</param>
    /// <param name="sid">The id which identifies the subscription.</param>
    public void Add(string address, string callback, List<string> dataToSend, bool persist, int sid)
    {
        Add(new GetSubscriberListItem(address,
            callback,
            dataToSend,
            persist,
            sid));
    }
}