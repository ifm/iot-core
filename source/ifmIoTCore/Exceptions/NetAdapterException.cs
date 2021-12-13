namespace ifmIoTCore.Exceptions
{
    using System;

    /// <summary>
    /// Represents errors that occur within network adapters
    /// </summary>
    public class NetAdapterException : Exception
    {
        /// <summary>
        /// Gets the code that is assigned to the current exception
        /// </summary>
        public int Code => HResult;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="code">The code that is assigned to the current exception</param>
        /// <param name="message">The message that describes the current exception</param>
        public NetAdapterException(int code, string message) : base(message)
        {
            HResult = code;
        }
    }
}
