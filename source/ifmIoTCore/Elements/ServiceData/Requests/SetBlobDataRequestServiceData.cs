namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the incoming data for a SetBlobData service call
    /// </summary>
    public class SetBlobDataRequestServiceData
    {
        /// <summary>
        /// The byte position to start writing in the blob
        /// </summary>
        [JsonProperty("pos", Required = Required.Always)]
        public readonly uint Pos;

        /// <summary>
        /// The byte length of data to write in the blob
        /// </summary>
        [JsonProperty("length", Required = Required.Always)]
        public readonly uint Length;

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
