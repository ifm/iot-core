namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Variant;

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.QueryTree service call
    /// </summary>
    public class QueryTreeResponseServiceData
    {
        /// <summary>
        /// The list of addresses of the elements retrieved by the query
        /// </summary>
        [VariantProperty("adrlist", Required = true)]
        public List<string> Addresses { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public QueryTreeResponseServiceData()
        {
        }

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
