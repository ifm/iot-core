namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Common.Variant;

    /// <summary>
    /// Represents the outgoing data for a Subscribe service call
    /// </summary>
    public class SubscribeResponseServiceData
    {
        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [VariantProperty("subscribeid", Required = true)]
        public int SubscriptionId { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public SubscribeResponseServiceData()
        {
        }

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
