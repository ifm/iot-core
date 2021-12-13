namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the outgoing data for a GetBlobData service call
    /// </summary>
    public class GetBlobDataResponseServiceData
    {
        /// <summary>
        /// The data read from the blob
        /// </summary>
        [JsonProperty("data", Required = Required.Always)]
        public readonly string Data;

        /// <summary>
        /// The crc checksum of the current data chunk
        /// </summary>
        [JsonProperty("crc", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Crc;

        /// <summary>
        /// The md5 checksum of the current data chunk
        /// </summary>
        [JsonProperty("md5", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Md5;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="data">The data read from the blob</param>
        /// <param name="crc">The crc checksum of the current data chunk</param>
        /// <param name="md5">The md5 checksum of the current data chunk</param>
        public GetBlobDataResponseServiceData(string data, string crc, string md5)
        {
            Data = data;
            Crc = crc;
            Md5 = md5;
        }
    }
}
