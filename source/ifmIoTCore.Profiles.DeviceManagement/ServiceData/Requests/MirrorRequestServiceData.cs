namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a Mirror service call
    /// </summary>
    public class MirrorRequestServiceData
    {
        /// <summary>
        /// The url of the mirrored IoTCore
        /// </summary>
        [JsonProperty("uri", Required = Required.Always)]
        public readonly string Uri;

        /// <summary>
        /// The url of the mirrored IoTCore
        /// </summary>
        [JsonProperty("callback", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string CallbackUri;

        /// <summary>
        /// The alias name for the mirrored IoTCore; if omitted the identifier of the IoTCore is used
        /// </summary>
        [JsonProperty("alias", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Alias;

        [JsonProperty("cache_timeout", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly int? CacheTimeout;

        [JsonProperty("auth", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly AuthenticationInfo Authentication;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="uri">The url of the mirrored IoTCore</param>
        /// <param name="callbackUri">The url where the event raising part can send its events to.</param>
        /// <param name="alias">The alias name for the device element</param>
        /// <param name="cacheTimeout">The time interval when the the data element cache expires in milliseconds; if null, caching is disabled</param>
        /// <param name="authentication">The authentication information</param>
        public MirrorRequestServiceData(string uri, string callbackUri = null, string alias = null, int? cacheTimeout = 1000, AuthenticationInfo authentication = null)
        {
            Uri = uri;
            CallbackUri = callbackUri;
            Alias = alias;
            CacheTimeout = cacheTimeout;
            Authentication = authentication;
        }
    }
}