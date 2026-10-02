namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Responses;

using ifm.Common.Variant;

/// <summary>
/// Represents the outgoing data for a Subscribe service call.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="subscriptionId">The id which identifies the subscription.</param>
public class SubscribeResponseServiceData(int subscriptionId)
{
    /// <summary>
    /// The id which identifies the subscription.
    /// </summary>
    [VariantProperty("subscribeid", Required = true)]
    public int SubscriptionId { get; set; } = subscriptionId;

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public SubscribeResponseServiceData() : this(0)
    {
    }
}