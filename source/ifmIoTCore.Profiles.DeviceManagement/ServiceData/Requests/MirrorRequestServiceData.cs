namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Common.Variant;
    using Messages;

    /// <summary>
    /// Represents the incoming data for a Mirror service call
    /// </summary>
    public class MirrorRequestServiceData
    {
        /// <summary>
        /// The url of the mirrored IoTCore
        /// </summary>
        [VariantProperty("uri", Required = true)]
        public string RemoteUri { get; set; }

        /// <summary>
        /// The callback where the mirrored IoTCore can send its events to
        /// </summary>
        [VariantProperty("callback", Required = true)]
        public string Callback { get; set; }

        /// <summary>
        /// The alias name for the mirrored IoTCore; if null, the identifier of the mirrored IoTCore is used
        /// </summary>
        [VariantProperty("alias", IgnoredIfNull = true)]
        public string Alias { get; set; }

        /// <summary>
        /// The time interval when the the data element cache expires in milliseconds; if null, caching is disabled
        /// </summary>
        [VariantProperty("cache_timeout", IgnoredIfNull = true)]
        public int? CacheTimeout { get; set; }

        /// <summary>
        /// The authentication information to access the remote IoTCore
        /// </summary>
        [VariantProperty("auth", IgnoredIfNull = true)]
        public AuthenticationInfo AuthenticationInfo { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public MirrorRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="remoteUri">The url of the mirrored IoTCore</param>
        /// <param name="callback">The callback where the mirrored IoTCore can send its events to</param>
        /// <param name="alias">The alias name for the mirrored IoTCore; if null, the identifier of the mirrored IoTCore is used</param>
        /// <param name="cacheTimeout">The time interval when the the data element cache expires in milliseconds; if null, caching is disabled</param>
        /// <param name="authenticationInfo">The authentication information to access the remote IoTCore</param>
        public MirrorRequestServiceData(string remoteUri, string callback, string alias = null, int? cacheTimeout = 1000, AuthenticationInfo authenticationInfo = null)
        {
            RemoteUri = remoteUri;
            Callback = callback;
            Alias = alias;
            CacheTimeout = cacheTimeout;
            AuthenticationInfo = authenticationInfo;
        }
    }
}