namespace ifmIoTCore.Elements.ServiceData.Events
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a IEventElement.TriggerEvent service call
    /// </summary>
    public class EventServiceData
    {
        /// <summary>
        /// The number of the event
        /// </summary>
        [JsonProperty("eventno", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly int EventNumber;

        /// <summary>
        /// The address of the element that raised the event
        /// </summary>
        [JsonProperty("srcurl", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string EventSource;

        /// <summary>
        /// List of data element addresses and values that are requested in the event subscription 
        /// </summary>
        [JsonProperty("payload", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly Dictionary<string, CodeDataPair> Payload;

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [JsonProperty("subscribeid", Required = Required.Always)]
        public readonly int SubscriptionId;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="eventSource">The address of the element that raised the event</param>
        /// <param name="eventNumber">The number of the event</param>
        /// <param name="payload">List of data element addresses and values that are requested in the event subscription</param>
        /// <param name="subscriptionId">The id which identifies the subscription</param>
        public EventServiceData(int eventNumber, string eventSource, Dictionary<string, CodeDataPair> payload, int subscriptionId)
        {
            EventNumber = eventNumber;
            EventSource = eventSource;
            Payload = payload;
            SubscriptionId = subscriptionId;
        }
    }
}
