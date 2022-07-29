namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a SetBlobData service call
    /// </summary>
    public class SetBlobDataRequestServiceData
    {
        /// <summary>
        /// The byte position to start writing in the blob
        /// </summary>
        [VariantProperty("pos", Required = true)]
        public uint Pos { get; set; }

        /// <summary>
        /// The byte length of data to write in the blob
        /// </summary>
        [VariantProperty("length", Required = true)]
        public uint Length { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public SetBlobDataRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="pos">The byte position to start writing in the blob</param>
        /// <param name="length">The byte length of data to write in the blob</param>
        public SetBlobDataRequestServiceData(uint pos, uint length)
        {
            Pos = pos;
            Length = length;
        }
    }
}
