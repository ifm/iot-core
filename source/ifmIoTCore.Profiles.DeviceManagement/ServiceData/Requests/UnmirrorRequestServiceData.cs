namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a Unmirror service call
    /// </summary>
    public class UnmirrorRequestServiceData
    {
        /// <summary>
        /// The url of the mirrored IoTCore
        /// </summary>
        [JsonProperty("uri", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Uri;

        /// <summary>
        /// The alias name for the mirrored IoTCore
        /// </summary>
        [JsonProperty("alias", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Alias;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="uri">The url of the mirrored IoTCore</param>
        /// <param name="alias">The alias name of the mirrored IoTCore</param>
        public UnmirrorRequestServiceData(string uri, string alias = null)
        {
            Uri = uri;
            Alias = alias;
        }
    }
}