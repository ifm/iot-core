namespace ifm.IoTCore.EventSender.Contracts;

using System.Collections.Generic;
using ifm.Common.Variant;
using Common;

/// <summary>
/// Represents the data for an event message.
/// </summary>
public class EventMessageDataBase
{
    /// <summary>
    /// The number of the message.
    /// </summary>
    [VariantProperty("eventno", Required = true)]
    public int Number { get; set; }

    /// <summary>
    /// The address of the element that raised the event.
    /// </summary>
    [VariantProperty("srcurl", Required = true)]
    public string Sender { get; set; }

    /// <summary>
    /// The id which identifies the subscription.
    /// </summary>
    [VariantProperty("subscribeid", Required = true)]
    public int SubscribeId { get; set; }
}

/// <summary>
/// Represents the data for an event message.
/// </summary>
public class EventMessageData : EventMessageDataBase
{
    /// <summary>
    /// The requested data points.
    /// key: The address of each requested data point.
    /// value: The values; each tuple contains the read result code, the value and timestamps if successful.
    /// </summary>
    [VariantProperty("payload", IgnoredIfNull = true)]
    public Dictionary<string, CodeDataPair> Payload { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public EventMessageData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="sender">The address of the element that raised the event.</param>
    /// <param name="subscribeId">The id which identifies the subscription.</param>
    /// <param name="payload">List of data element addresses and values that are requested in the subscription.</param>
    public EventMessageData(string sender, 
        int subscribeId, 
        Dictionary<string, CodeDataPair> payload)
    {
        Number = 0;
        Sender = sender;
        SubscribeId = subscribeId;
        Payload = payload;
    }
}
