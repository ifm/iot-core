namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.GetSubscriberList service call
    /// </summary>
    public class GetSubscriberListRequestServiceData
    {
        /// <summary>
        /// The address of the event element; if null, all event elements
        /// </summary>
        [JsonProperty("adr", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Address;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the event element; if null, all event elements</param>
        public GetSubscriberListRequestServiceData(string address)
        {
            Address = address;
        }
    }
}
