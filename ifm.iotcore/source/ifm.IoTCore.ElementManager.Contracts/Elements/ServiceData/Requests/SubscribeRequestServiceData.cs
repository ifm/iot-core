namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using System.Collections.Generic;
using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IEventElement.Subscribe service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="callbackUrl">The url to which the IoTCore is sending events.</param>
/// <param name="dataToSend">List of data element addresses, whose values are sent with the event.</param>
/// <param name="subscriptionId">The id which identifies the subscription.</param>
/// <param name="persist">If true the subscription is persistent; otherwise not.</param>
public class SubscribeRequestServiceData(string callbackUrl,
    List<string> dataToSend = null,
    int? subscriptionId = null,
    bool persist = true)
{
    /// <summary>
    /// The callback address of the subscription.
    /// </summary>
    [VariantProperty("callbackurl", Required = true, AlternativeNames = ["callback"])]
    public string CallbackUrl { get; set; } = callbackUrl;

    /// <summary>
    /// The callback address of the subscription, for backward compatibility.
    /// </summary>
    [VariantProperty("callback", IgnoredIfNull = true)]
    public string Callback { get => CallbackUrl; set => CallbackUrl = value; }

    /// <summary>
    /// List of data element addresses, whose values are sent with the event.
    /// </summary>
    [VariantProperty("datatosend", IgnoredIfNull = true)]
    public List<string> DataToSend { get; set; } = dataToSend;

    /// <summary>
    /// The id which identifies the subscription.
    /// </summary>
    [VariantProperty("subscribeid", IgnoredIfNull = true)]
    public int? SubscriptionId { get; set; } = subscriptionId;

    /// <summary>
    /// If true the subscription is persistent; otherwise not.
    /// </summary>
    [VariantProperty("persist", IgnoredIfNull = true)]
    public bool Persist { get; set; } = persist;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public SubscribeRequestServiceData() : this(null)
    {
        Persist = true;
    }
}