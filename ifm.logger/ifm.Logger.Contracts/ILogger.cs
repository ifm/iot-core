namespace ifm.Logger.Contracts;

using System;

/// <summary>
/// Specifies the log levels.
/// </summary>
public enum LogLevel
{
    /// <summary>Debug log level.</summary>
    Debug,
    /// <summary>Information log level.</summary>
    Info,
    /// <summary>Warning log level.</summary>
    Warning,
    /// <summary>Error log level.</summary>
    Error,
    /// <summary>Disabled.</summary>
    Off
}

/// <summary>
/// Provides functionality to interact with a logger.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Logs a debug message to the current logger. The message is only logged if the log level is set to LogLevel.Debug.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(string message);

    /// <summary>
    /// Logs an information message to the current logger. The message is only logged if the log level is set to LogLevel.Info or lower.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(string message);

    /// <summary>
    /// Logs a warning message to the current logger. The message is only logged if the log level is set to LogLevel.Warning or lower.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warning(string message);

    /// <summary>
    /// Logs an error message to the current logger. The message is only logged if the log level is set to LogLevel.Error or lower.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(string message);

    /// <summary>
    /// Logs an exception as error message to the current logger. The message is only logged if the log level is set to LogLevel.Error or lower.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    void Error(Exception ex);
}
