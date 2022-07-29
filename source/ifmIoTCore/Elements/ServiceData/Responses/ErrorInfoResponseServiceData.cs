namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Common.Variant;

    /// <summary>
    /// Represents error information returned from failing service requests
    /// </summary>
    public class ErrorInfoResponseServiceData
    {
        /// <summary>
        /// Gets the error message
        /// </summary>
        [VariantProperty("msg", IgnoredIfNull = true)]
        public string Message { get; set; }

        /// <summary>
        /// Gets the error code
        /// </summary>
        [VariantProperty("code", IgnoredIfNull = true)]
        public int? Code { get; set; }

        /// <summary>
        /// Gets the details what caused the error or how to fix it
        /// </summary>
        [VariantProperty("details", IgnoredIfNull = true)]
        public string Details { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public ErrorInfoResponseServiceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="code">The error code</param>
        /// <param name="details">The details what caused the error or how to fix the problem</param>
        public ErrorInfoResponseServiceData(string message, int? code = null, string details = null)
        {
            Message = message;
            Code = code;
            Details = details;
        }
    }
}
