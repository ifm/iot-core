namespace ifmIoTCore.Elements.ServiceData.Requests
{
    using Common.Variant;

    public class SetDataRequestServiceData
    {
        /// <summary>
        /// The value to set
        /// </summary>
        [VariantProperty("value", Required = true, AlternativeNames = new[] { "newvalue" })]
        public Variant Value { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public SetDataRequestServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="value">The value to set</param>
        public SetDataRequestServiceData(Variant value)
        {
            Value = value;
        }
    }
}
