
namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.RemoveElement service call
    /// </summary>
    public class RemoveElementRequestServiceData
    {
        /// <summary>
        /// The address of the element to remove
        /// </summary>
        [VariantProperty("adr", Required = true)]
        public string Address { get; set; }

        /// <summary>
        /// true, if the request is persisted; otherwise false. false is default
        /// </summary>
        [VariantProperty("persist", Required = false)]
        public bool Persist { get; set; }

        [VariantConstructor]
        public RemoveElementRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the element to remove</param>
        /// <param name="persist">true, if the request is persisted; otherwise false. false is default</param>
        public RemoveElementRequestServiceData(string address, bool persist = false)
        {
            Address = address;
            Persist = persist;
        }
    }
}
