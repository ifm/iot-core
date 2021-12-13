namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the outgoing data for a SetBlobData service call
    /// </summary>
    public class SetBlobDataResponseServiceData
    {
        /// <summary>
        /// The crc checksum of the current data chunk
        /// </summary>
        [JsonProperty("crc", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Crc;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="crc">The crc checksum of the current data chunk</param>
        public SetBlobDataResponseServiceData(string crc)
        {
            Crc = crc;
        }
    }
}
