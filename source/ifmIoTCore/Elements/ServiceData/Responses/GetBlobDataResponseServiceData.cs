namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Common.Variant;

    /// <summary>
    /// Represents the outgoing data for a GetBlobData service call
    /// </summary>
    public class GetBlobDataResponseServiceData
    {
        /// <summary>
        /// The data read from the blob
        /// </summary>
        [VariantProperty("data", Required = true)]
        public string Data { get; set; }

        /// <summary>
        /// The crc checksum of the current data chunk
        /// </summary>
        [VariantProperty("crc", IgnoredIfNull = true)]
        public string Crc { get; set; }

        /// <summary>
        /// The md5 checksum of the current data chunk
        /// </summary>
        [VariantProperty("md5", IgnoredIfNull = true)]
        public string Md5 { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetBlobDataResponseServiceData()
        {
        }

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
