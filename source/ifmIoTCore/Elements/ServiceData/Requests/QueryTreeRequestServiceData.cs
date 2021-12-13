namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.QueryTree service call
    /// </summary>
    public class QueryTreeRequestServiceData
    {
        /// <summary>
        /// The profile filter for the query
        /// </summary>
        [JsonProperty("profile", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Profile;

        /// <summary>
        /// The type filter for the query
        /// </summary>
        [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Type;

        /// <summary>
        /// The name filter for the query
        /// </summary>
        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Name;

        /// <summary>
        /// The link filter for the query
        /// </summary>
        [JsonProperty("link", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly bool? Link;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="profile">The profile filter</param>
        /// <param name="type">The type filter</param>
        /// <param name="name">The name filter</param>
        public QueryTreeRequestServiceData(string profile = null, string type = null, string name = null, bool? link = null)
        {
            Profile = profile;
            Type = type;
            Name = name;
            Link = link;
        }
    }
}