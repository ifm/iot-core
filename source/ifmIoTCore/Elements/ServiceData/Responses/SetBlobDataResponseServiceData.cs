namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Common.Variant;

    /// <summary>
    /// Represents the outgoing data for a SetBlobData service call
    /// </summary>
    public class SetBlobDataResponseServiceData
    {
        /// <summary>
        /// The crc checksum of the current data chunk
        /// </summary>
        [VariantProperty("crc", IgnoredIfNull = true)]
        public string Crc { get; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public SetBlobDataResponseServiceData()
        {
        }

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
