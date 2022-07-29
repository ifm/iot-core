namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Common.Variant;

    /// <summary>
    /// Represents the outgoing data for a GetData service call
    /// </summary>
    public class ReadAcyclicResponseServiceData
    {
        /// <summary>
        /// The value returned by the service
        /// </summary>
        [VariantProperty("value", Required = true)]
        public Variant Value { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public ReadAcyclicResponseServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value to set</param>
        public ReadAcyclicResponseServiceData(Variant value)
        {
            Value = value;
        }
    }
}
