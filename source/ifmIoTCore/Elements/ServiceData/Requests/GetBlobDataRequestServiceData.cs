namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a GetBlobData service call
    /// </summary>
    public class GetBlobDataRequestServiceData
    {
        /// <summary>
        /// The byte position to start reading in the blob
        /// </summary>
        [VariantProperty("pos", Required = true)]
        public uint Pos { get; set; }

        /// <summary>
        /// The byte length of data to read from from blob
        /// </summary>
        [VariantProperty("length", Required = true)]
        public uint Length { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetBlobDataRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="pos">The byte position to start reading in the blob</param>
        /// <param name="length">The byte length of data to read from from blob</param>
        public GetBlobDataRequestServiceData(uint pos, uint length)
        {
            Pos = pos;
            Length = length;
        }
    }
}
