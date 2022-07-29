namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a IDeviceElement.QueryTree service call
    /// </summary>
    public class QueryTreeRequestServiceData
    {
        /// <summary>
        /// The profile filter for the query
        /// </summary>
        [VariantProperty("profile", IgnoredIfNull = true)]
        public string Profile { get; set; }

        /// <summary>
        /// The type filter for the query
        /// </summary>
        [VariantProperty("type", IgnoredIfNull = true)]
        public string Type { get; set; }

        /// <summary>
        /// The identifier (name) filter for the query
        /// </summary>
        [VariantProperty("identifier", IgnoredIfNull = true, AlternativeNames = new []{"name"})]
        public string Identifier { get; set; }

        /// <summary>
        /// The link filter for the query
        /// </summary>
        [VariantProperty("link", IgnoredIfNull = true)]
        public bool? Link { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public QueryTreeRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="profile">The profile filter</param>
        /// <param name="type">The type filter</param>
        /// <param name="identifier">The name filter</param>
        /// <param name="link">The link filter</param>
        public QueryTreeRequestServiceData(string profile = null, string type = null, string identifier = null, bool? link = null)
        {
            Profile = profile;
            Type = type;
            Identifier = identifier;
            Link = link;
        }
    }
}