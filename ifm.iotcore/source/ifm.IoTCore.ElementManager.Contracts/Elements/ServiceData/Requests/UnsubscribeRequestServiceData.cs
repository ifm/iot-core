namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Requests;

using ifm.Common.Variant;

/// <summary>
/// Represents the incoming data for a IEventElement.Unsubscribe service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="callbackUrl">The callback address of the subscription.</param>
/// <param name="subscriptionId">The id which identifies the subscription.</param>
public class UnsubscribeRequestServiceData(string callbackUrl,
    int? subscriptionId = null)
{
    /// <summary>
    /// The callback address of the subscription.
    /// </summary>
    [VariantProperty("callbackurl", IgnoredIfNull = true, AlternativeNames = ["callback"])]
    public string CallbackUrl { get; set; } = callbackUrl;

    /// <summary>
    /// The callback address of the subscription, for backward compatibility.
    /// </summary>
    [VariantProperty("callback", IgnoredIfNull = true)]
    public string Callback { get => CallbackUrl; set => CallbackUrl = value; }

    /// <summary>
    /// The id which identifies the subscription.
    /// </summary>
    [VariantProperty("subscribeid", IgnoredIfNull = true)]
    public int? SubscriptionId { get; set; } = subscriptionId;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public UnsubscribeRequestServiceData() : this(null)
    {
    }
}