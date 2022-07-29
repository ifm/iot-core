namespace ifmIoTCore.Elements.ServiceData
{
    using Common.Variant;

    public class CodeDataPair
    {
        /// <summary>
        /// The code of the query
        /// </summary>
        [VariantProperty("code", Required = true)]
        public int Code { get; set; }

        /// <summary>
        /// The value of the element
        /// </summary>
        [VariantProperty("data", Required = true)]
        public Variant Data { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public CodeDataPair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public CodeDataPair(int code, Variant data)
        {
            Code = code;
            Data = data;
        }
    }
}