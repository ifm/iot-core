namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a ReadAcyclic service call
    /// </summary>
    public class ReadAcyclicRequestServiceData
    {
        /// <summary>
        /// The ISDU index to read from the device
        /// </summary>
        [JsonProperty("index", Required = Required.Always)]
        public readonly int Index;

        /// <summary>
        /// The ISDU subindex to read from the device
        /// </summary>
        [JsonProperty("subindex", Required = Required.Always)]
        public readonly int SubIndex;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="index">The ISDU index to read from the device</param>
        /// <param name="subIndex">The ISDU subindex to read from the device</param>
        public ReadAcyclicRequestServiceData(int index, int subIndex)
        {
            Index = index;
            SubIndex = subIndex;
        }
    }
}
