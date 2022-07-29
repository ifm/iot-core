namespace ifmIoTCore.Profiles.DeviceManagement.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a AddRoute service call
    /// </summary>
    public class RemoveRouteRequestServiceData
    {
        /// <summary>
        /// The identifier name for the routed IoTCore
        /// </summary>
        [VariantProperty("identifier", Required = true)]
        public string Identifier { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public RemoveRouteRequestServiceData()
        {
        }

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
