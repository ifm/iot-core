namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using System.Collections.Generic;
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IEventElement.Subscribe service call
    /// </summary>
    public class SubscribeRequestServiceData
    {
        /// <summary>
        /// The callback address of the subscription
        /// </summary>
        [VariantProperty("callback", Required = true, AlternativeNames = new[] { "callbackurl" })]
        public string Callback { get; set; }

        /// <summary>
        /// List of data element addresses, whose values are sent with the event
        /// </summary>
        [VariantProperty("datatosend", IgnoredIfNull = true)]
        public List<string> DataToSend { get; set; }

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [VariantProperty("subscribeid", IgnoredIfNull = true)]
        public int? SubscriptionId { get; set; }

        /// <summary>
        /// If true the subscription is persistent; otherwise not 
        /// </summary>
        [VariantProperty("persist", IgnoredIfNull = true)]
        public bool Persist { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public SubscribeRequestServiceData()
        {
        }

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
