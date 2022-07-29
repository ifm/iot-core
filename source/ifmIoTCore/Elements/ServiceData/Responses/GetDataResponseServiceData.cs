namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Common.Variant;

    public class GetDataResponseServiceData
    {
        /// <summary>
        /// The value to get
        /// </summary>
        [VariantProperty("value", Required = true)]
        public Variant Value { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetDataResponseServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value to get</param>
        public GetDataResponseServiceData(Variant value)
        {
            Value = value;
        }
    }
}
