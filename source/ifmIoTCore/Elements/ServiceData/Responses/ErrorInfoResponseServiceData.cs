namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents error information returned from failing service requests
    /// </summary>
    public class ErrorInfoResponseServiceData
    {
        /// <summary>
        /// The error message
        /// </summary>
        [JsonProperty("msg", Required = Required.Always)]
        public readonly string Message;

        /// <summary>
        /// The error code
        /// </summary>
        [JsonProperty("error", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Error;

        /// <summary>
        /// The hint what caused the error or how to fix it
        /// </summary>
        [JsonProperty("hint", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly string Hint;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="error">The error code</param>
        /// <param name="hint">The help hint what caused the error or how to fix the problem</param>
        public ErrorInfoResponseServiceData(string message, string error = null, string hint = null)
        {
            Message = message;
            Error = error;
            Hint = hint;
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="error">The error code</param>
        /// <param name="hint">The help hint what caused the error or how to fix the problem</param>
        public ErrorInfoResponseServiceData(string message, int error, string hint = null)
        {
            Message = message;
            Error = error.ToString();
            Hint = hint;
        }
    }
}
