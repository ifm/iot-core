namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a ReadAcyclic service call
    /// </summary>
    public class ReadAcyclicRequestServiceData
    {
        /// <summary>
        /// The ISDU index to read from the device
        /// </summary>
        [VariantProperty("index", Required = true)]
        public int Index { get; set; }

        /// <summary>
        /// The ISDU subindex to read from the device
        /// </summary>
        [VariantProperty("subindex", Required = true)]
        public int SubIndex { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public ReadAcyclicRequestServiceData()
        {
        }

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
