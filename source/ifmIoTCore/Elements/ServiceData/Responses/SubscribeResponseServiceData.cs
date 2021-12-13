namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the outgoing data for a Subscribe service call
    /// </summary>
    public class SubscribeResponseServiceData
    {
        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [JsonProperty("subscribeid", Required = Required.Always)]
        public readonly int SubscriptionId;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="subscriptionId">The id which identifies the subscription</param>
        public SubscribeResponseServiceData(int subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
