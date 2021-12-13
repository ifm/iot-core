namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Messages;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a AddRoute service call
    /// </summary>
    public class AddRouteRequestServiceData
    {
        /// <summary>
        /// The url of the routed IoTCore
        /// </summary>
        [JsonProperty("url", Required = Required.Always)]
        public readonly string Uri;

        /// <summary>
        /// The identifier name for the routed IoTCore
        /// </summary>
        [JsonProperty("identifier", Required = Required.Always)]
        public readonly string Identifier;

        /// <summary>
        /// If true, the request is persisted; otherwise not. false is default
        /// </summary>
        [JsonProperty("persist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool Persist;

        /// <summary>
        /// The authentication information
        /// </summary>
        [JsonProperty("auth", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly AuthenticationInfo Authentication;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="uri">The url of the routed IoTCore</param>
        /// <param name="identifier">The identifier name for the routed IoTCore</param>
        /// <param name="persist">If true, the request is persisted; otherwise not. false is default</param>
        /// <param name="authentication">The authentication information</param>
        public AddRouteRequestServiceData(string uri, string identifier, bool persist = false, AuthenticationInfo authentication = null)
        {
            Uri = uri;
            Identifier = identifier;
            Persist = persist;
            Authentication = authentication;
        }
    }
}
