namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GetSubscriberListItem
    {
        /// <summary>
        /// The address of the event element
        /// </summary>
        [JsonProperty("adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Address;

        /// <summary>
        /// The url to which the IoTCore is sending events
        /// </summary>
        [JsonProperty("callbackurl", Required = Required.Always)]
        public readonly string Callback;

        /// <summary>
        /// List of data element addresses, whose values are sent with the event
        /// </summary>
        [JsonProperty("datatosend", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly List<string> DataToSend;

        /// <summary>
        /// Specifies the persistence duration type
        /// </summary>
        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [JsonProperty("subscribeid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int SubscriptionId;

        /// <summary>
        /// The old id which identifies the subscription
        /// </summary>
        [JsonProperty("cid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int Cid => SubscriptionId;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the event element</param>
        /// <param name="callback">The url to which the IoTCore is sending events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">Specifies the persistence duration type</param>
        /// <param name="subscriptionId">The uid which identifies the subscription</param>
        public GetSubscriberListItem(string address, string callback, List<string> dataToSend, bool persist, int subscriptionId)
        {
            Address = address;
            Callback = callback;
            DataToSend = dataToSend;
            Persist = persist;
            SubscriptionId = subscriptionId;
        }
    }

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.GetSubscriberList service call
    /// </summary>
    public class GetSubscriberListResponseServiceData : IEnumerable<GetSubscriberListItem>
    {
        private readonly List<GetSubscriberListItem> _subscriptions = new List<GetSubscriberListItem>();

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<GetSubscriberListItem> GetEnumerator()
        {
            return _subscriptions.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _subscriptions).GetEnumerator();
        }

        /// <summary>
        /// Add a new subscription info item 
        /// </summary>
        /// <param name="address">The address of the event element</param>
        /// <param name="callback">The url to which the IoTCore is sending events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">Specifies the persistence duration type</param>
        /// <param name="sid">The id which identifies the subscription</param>
        public void Add(string address, string callback, List<string> dataToSend, bool persist, int sid)
        {
            _subscriptions.Add(new GetSubscriberListItem(address,
                callback,
                dataToSend,
                persist,
                sid));
        }
    }
}
