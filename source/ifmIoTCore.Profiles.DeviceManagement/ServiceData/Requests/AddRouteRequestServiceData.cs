namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Common.Variant;
    using Messages;

    /// <summary>
    /// Represents the incoming data for a AddRoute service call
    /// </summary>
    public class AddRouteRequestServiceData
    {
        /// <summary>
        /// The url of the routed IoTCore
        /// </summary>
        [VariantProperty("uri", Required = true)]
        public string Uri { get; set; }

        /// <summary>
        /// The identifier name for the routed IoTCore
        /// </summary>
        [VariantProperty("identifier", Required = true)]
        public string Identifier { get; set; }

        /// <summary>
        /// If true, the request is persisted; otherwise not. false is default
        /// </summary>
        [VariantProperty("persist", IgnoredIfNull = true)]
        public bool Persist { get; set; }

        /// <summary>
        /// The authentication information
        /// </summary>
        [VariantProperty("auth", IgnoredIfNull = true)]
        public AuthenticationInfo Authentication { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public AddRouteRequestServiceData()
        {
        }

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
