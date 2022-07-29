namespace ifmIoTCore.Elements.ServiceData.Events
{
    using System.Collections.Generic;
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IEventElement.TriggerEvent service call
    /// </summary>
    public class EventServiceData
    {
        /// <summary>
        /// The number of the event
        /// </summary>
        [VariantProperty("eventno", IgnoredIfNull = true)]
        public int EventNumber { get; set; }

        /// <summary>
        /// The address of the element that raised the event
        /// </summary>
        [VariantProperty("srcurl", IgnoredIfNull = true)]
        public string EventAddress { get; set; }

        /// <summary>
        /// List of data element addresses and values that are requested in the event subscription 
        /// </summary>
        [VariantProperty("payload", IgnoredIfNull = true)]
        public Dictionary<string, CodeDataPair> Payload { get; set; }

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [VariantProperty("subscribeid", IgnoredIfNull = true)]
        public int? SubscriptionId { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public EventServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="eventAddress">The address of the element that raised the event</param>
        /// <param name="eventNumber">The number of the event</param>
        /// <param name="payload">List of data element addresses and values that are requested in the event subscription</param>
        /// <param name="subscriptionId">The id which identifies the subscription</param>
        public EventServiceData(int eventNumber, string eventAddress, Dictionary<string, CodeDataPair> payload, int? subscriptionId = null)
        {
            EventNumber = eventNumber;
            EventAddress = eventAddress;
            Payload = payload;
            SubscriptionId = subscriptionId;
        }
    }
}
