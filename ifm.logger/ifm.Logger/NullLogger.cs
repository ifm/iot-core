namespace ifm.Logger;

using System;
using Contracts;

/// <summary>
/// A logger implementation that does not perform any logging operations. This class can be used when logging is disabled or not required.
/// </summary>
public class NullLogger : ILogger
{
    /// <summary>
    /// Logs a debug message to the current logger. This implementation does not perform any logging operations.
    /// </summary>
    /// <param name="message"></param>
    public void Debug(string message)
    {
    }

    /// <summary>
    /// Logs an information message to the current logger. This implementation does not perform any logging operations.
    /// </summary>
    /// <param name="message"></param>
    public void Info(string message)
    {
    }

    /// <summary>
    /// Logs a warning message to the current logger. This implementation does not perform any logging operations.
    /// </summary>
    /// <param name="message"></param>
    public void Warning(string message)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void Error(string message)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ex"></param>
    public void Error(Exception ex)
    {
    }
}