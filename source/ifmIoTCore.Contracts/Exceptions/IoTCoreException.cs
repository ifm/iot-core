namespace ifmIoTCore.Exceptions
{
    using System;
    using Messages;

    /// <summary>
    /// Represents error information
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Gets the error message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the error code
        /// </summary>
        public int? Code { get; }

        /// Gets the details what caused the error or how to fix the problem
        public string Details { get; }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="code">The error code</param>
        /// <param name="details">The details what caused the error or how to fix the problem</param>
        public ErrorInfo(string message,
            int? code,
            string details)
        {
            Message = message;
            Code = code;
            Details = details;
        }
    }

    /// <summary>
    /// Represents errors that occur during service execution
    /// </summary>
    public class IoTCoreException : Exception
    {
        /// <summary>
        /// Gets the response code
        /// </summary>
        public int ResponseCode { get; }

        public ErrorInfo ErrorInfo { get; }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="responseCode">The response code</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="errorCode">The service specific error</param>
        /// <param name="errorDetails">The details what caused the error or how to fix it</param>
        public IoTCoreException(int responseCode, string errorMessage, int? errorCode = null, string errorDetails = null) : base(errorMessage)
        {
            ResponseCode = responseCode;
            ErrorInfo = new ErrorInfo(errorMessage, errorCode, errorDetails);
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        /// <param name="errorCode">The service specific error</param>
        /// <param name="errorDetails">The details what caused the error or how to fix it</param>
        public IoTCoreException(string errorMessage, int? errorCode = null, string errorDetails = null) : base(errorMessage)
        {
            ResponseCode = ResponseCodes.ExecutionFailed;
            ErrorInfo = new ErrorInfo(errorMessage, errorCode, errorDetails);
        }
    }
}