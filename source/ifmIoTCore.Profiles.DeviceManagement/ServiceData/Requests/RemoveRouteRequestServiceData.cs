namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a AddRoute service call
    /// </summary>
    public class RemoveRouteRequestServiceData
    {
        /// <summary>
        /// The identifier name for the routed IoTCore
        /// </summary>
        [JsonProperty("identifier", Required = Required.Always)]
        public readonly string Identifier;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="identifier">The identifier name for the routed IoTCore</param>
        public RemoveRouteRequestServiceData(string identifier)
        {
            Identifier = identifier;
        }
    }
}
