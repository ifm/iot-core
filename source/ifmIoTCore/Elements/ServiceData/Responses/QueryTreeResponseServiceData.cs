namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.QueryTree service call
    /// </summary>
    public class QueryTreeResponseServiceData
    {
        /// <summary>
        /// The list of addresses of the elements retrieved by the query
        /// </summary>
        [JsonProperty("adrlist", Required = Required.Always)]
        public readonly List<string> Addresses;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="addresses">The list of addresses of the elements retrieved by the query</param>
        public QueryTreeResponseServiceData(IEnumerable<string> addresses)
        {
            Addresses = addresses.ToList();
        }
    }
}
