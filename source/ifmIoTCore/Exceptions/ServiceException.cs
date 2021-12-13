namespace ifmIoTCore.Exceptions
{
    using System;

    /// <summary>
    /// Represents errors that occur during service execution
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Gets the code that is assigned to the current exception
        /// </summary>
        public int Code => HResult;

        /// The hint what caused the error or how to fix the problem
        public readonly string Hint;

        /// The name of the service that caused the error
        public readonly string Origin;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code">The code that is assigned to the current exception</param>
        /// <param name="message">The message that describes the current exception</param>
        /// <param name="hint">The hint what caused the error or how to fix it</param>
        /// <param name="origin">The name of the service that caused the error</param>
        public ServiceException(int code, string message, string hint = null, string origin = null) : base(message)
        {
            HResult = code;
            Hint = hint;
            Origin = origin;
        }
    }
}