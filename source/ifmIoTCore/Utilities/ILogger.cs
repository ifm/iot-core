namespace ifmIoTCore.Utilities
{
    /// <summary>
    /// Specifies log levels for the logger
    /// </summary>
    public enum LogLevel
    {
        /// Debug log level
        Debug,
        /// Information log level
        Info,
        /// Warning log level
        Warning,
        /// Error log level
        Error,
        /// Disabled
        Off
    }

    /// <summary>
    /// The logger interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an information message to the current logger\n
        /// The message is only logged if the log level is set to LogLevel.Info or lower
        /// </summary>
        /// <param name="message">The message to log</param>
        void Info(string message);

        /// <summary>
        /// Logs a warning message to the current logger\n
        /// The message is only logged if the log level is set to LogLevel.Warning or lower
        /// </summary>
        /// <param name="message">The message to log</param>
        void Warning(string message);

        /// <summary>
        /// Logs an error message to the current logger\n
        /// The message is only logged if the log level is set to LogLevel.Error or lower
        /// </summary>
        /// <param name="message">The message to log</param>
        void Error(string message);

        /// <summary>
        /// Logs a debug message to the current logger\n
        /// The message is only logged if the log level is set to LogLevel.Debug
        /// </summary>
        /// <param name="message">The message to log</param>
        void Debug(string message);
    }
}