namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.GetSubscriberList service call
    /// </summary>
    public class GetSubscriberListRequestServiceData
    {
        /// <summary>
        /// The address of the event element; if null, all event elements
        /// </summary>
        [VariantProperty("adr", IgnoredIfNull = true)]
        public string Address { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetSubscriberListRequestServiceData()
        {
        }

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
