namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a IEventElement.Unsubscribe service call
    /// </summary>
    public class UnsubscribeRequestServiceData
    {
        /// <summary>
        /// The callback address of the subscription
        /// </summary>
        [JsonProperty("callback", Required = Required.Always)]
        public readonly string Callback;

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [JsonProperty("subscribeid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly int? SubscriptionId;

        /// <summary>
        /// true, if the corresponding subscribe request is deleted from storage; otherwise false. false is default
        /// </summary>
        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="callback">The callback address of the subscription</param>
        /// <param name="persist">true, if the corresponding subscribe request is deleted from storage; otherwise false. false is default</param>
        /// <param name="subscriptionId">The id which identifies the subscription</param>
        public UnsubscribeRequestServiceData(string callback, int? subscriptionId = null, bool persist = true)
        {
            Callback = callback;
            SubscriptionId = subscriptionId;
            Persist = persist;
        }
    }
}
