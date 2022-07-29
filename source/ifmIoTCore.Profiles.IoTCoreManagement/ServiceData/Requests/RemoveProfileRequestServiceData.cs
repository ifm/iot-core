namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System.Collections.Generic;
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.RemoveProfile service call
    /// </summary>
    public class RemoveProfileRequestServiceData
    {
        /// <summary>
        /// The list of element addresses, that specify the elements from which the specified profiles are removed
        /// </summary>
        [VariantProperty("adrlist", Required = true)]
        public List<string> Addresses { get; set; }

        /// <summary>
        /// The list of profiles to remove
        /// </summary>
        [VariantProperty("profiles", Required = true)]
        public List<string> Profiles { get; set; }

        /// <summary>
        /// true, if the request is persisted; otherwise false. false is default
        /// </summary>
        [VariantProperty("persist", Required = false)]
        public bool Persist { get; set; }

        [VariantConstructor]
        public RemoveProfileRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="addresses">The list of element addresses, that specify the elements from which the specified profiles are removed</param>
        /// <param name="profiles">The list of profiles to remove</param>
        /// <param name="persist">true, if the request is persisted; otherwise false. false is default</param>
        public RemoveProfileRequestServiceData(List<string> addresses, List<string> profiles, bool persist = false)
        {
            Addresses = addresses;
            Profiles = profiles;
            Persist = persist;
        }
    }
}
