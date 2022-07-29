namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    /// <summary>
    /// Represents the incoming data for a WriteAcyclic service call
    /// </summary>
    public class WriteAcyclicRequestServiceData
    {
        /// <summary>
        /// The ISDU index to write to the device
        /// </summary>
        [VariantProperty("index", Required = true)]
        public int Index { get; set; }

        /// <summary>
        /// The ISDU subindex to write to the device
        /// </summary>
        [VariantProperty("subindex", Required = true)]
        public int SubIndex { get; set; }

        /// <summary>
        /// The value  to write to the device
        /// </summary>
        [VariantProperty("value", Required = true)]
        public string Value { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public WriteAcyclicRequestServiceData()
        {
        }

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
