namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Requests
{
    using System.Collections.Generic;
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.AddProfile service call
    /// </summary>
    public class AddProfileRequestServiceData
    {
        /// <summary>
        /// The list of element addresses, that specify the elements to which the specified profiles are added
        /// </summary>
        [VariantProperty("adrlist", Required = true)]
        public List<string> Addresses { get; set; }

        /// <summary>
        /// The list of profiles to add
        /// </summary>
        [VariantProperty("profiles", Required = true)]
        public List<string> Profiles { get; set; }

        /// <summary>
        /// true, if the request is persisted; otherwise false. false is default
        /// </summary>
        [VariantProperty("persist", Required = false)]
        public bool Persist { get; set; }

        [VariantConstructor]
        public AddProfileRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="addresses">The list of element addresses, that specify the elements to which the specified profiles are added</param>
        /// <param name="profiles">The list of profiles to add</param>
        /// <param name="persist">true, if the request is persisted; otherwise false. false is default</param>
        public AddProfileRequestServiceData(List<string> addresses, List<string> profiles, bool persist = false)
        {
            Addresses = addresses;
            Profiles = profiles;
            Persist = persist;
        }
    }
}
