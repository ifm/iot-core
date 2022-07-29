namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a Unmirror service call
    /// </summary>
    public class UnmirrorRequestServiceData
    {
        /// <summary>
        /// The url of the mirrored IoTCore
        /// </summary>
        [VariantProperty("uri", IgnoredIfNull = true)]
        public string RemoteUri { get; set; }

        /// <summary>
        /// The alias name for the mirrored IoTCore
        /// </summary>
        [VariantProperty("alias", IgnoredIfNull = true)]
        public string Alias { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public UnmirrorRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="remoteUri">The url of the mirrored IoTCore</param>
        /// <param name="alias">The alias name of the mirrored IoTCore</param>
        public UnmirrorRequestServiceData(string remoteUri, string alias = null)
        {
            RemoteUri = remoteUri;
            Alias = alias;
        }
    }
}