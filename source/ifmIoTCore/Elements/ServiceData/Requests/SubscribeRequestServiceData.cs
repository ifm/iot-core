namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a IEventElement.Subscribe service call
    /// </summary>
    public class SubscribeRequestServiceData
    {
        /// <summary>
        /// The url to which the IoTCore is sending events
        /// </summary>
        [JsonProperty("callback", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Callback;

        /// <summary>
        /// List of data element addresses, whose values are sent with the event
        /// </summary>
        [JsonProperty("datatosend", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly List<string> DataToSend;

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [JsonProperty("subscribeid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? SubscriptionId;

        /// <summary>
        /// If true the subscription is persistent; otherwise not 
        /// </summary>
        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="callback">The url to which the IoTCore is sending events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">If true the subscription is persistent; otherwise not</param>
        /// <param name="subscriptionId">The id which identifies the subscription</param>
        public SubscribeRequestServiceData(string callback, List<string> dataToSend = null, int? subscriptionId = null, bool persist = false)
        {
            Callback = callback;
            DataToSend = dataToSend;
            SubscriptionId = subscriptionId;
            Persist = persist;
        }
    }
}
