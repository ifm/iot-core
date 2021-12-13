namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a WriteAcyclic service call
    /// </summary>
    public class WriteAcyclicRequestServiceData
    {
        /// <summary>
        /// The ISDU index to write to the device
        /// </summary>
        [JsonProperty("index", Required = Required.Always)]
        public readonly int Index;

        /// <summary>
        /// The ISDU subindex to write to the device
        /// </summary>
        [JsonProperty("subindex", Required = Required.Always)]
        public readonly int SubIndex;

        /// <summary>
        /// The value  to write to the device
        /// </summary>
        [JsonProperty("value", Required = Required.Always)]
        public readonly string Value;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="index">The ISDU index to write to the device</param>
        /// <param name="subIndex">The ISDU subindex to write to the device</param>
        /// <param name="value">The value  to write to the device</param>
        public WriteAcyclicRequestServiceData(int index, int subIndex, string value)
        {
            Index = index;
            SubIndex = subIndex;
            Value = value;
        }
    }
}
