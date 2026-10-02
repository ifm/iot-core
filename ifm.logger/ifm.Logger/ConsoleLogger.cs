namespace ifm.Logger;

using System;
using System.Text;
using Contracts;

/// <summary>
/// A logger implementation that writes log messages to the console. This class can be used for simple logging scenarios or during development and debugging.
/// </summary>
public class ConsoleLogger : ILogger
{
    /// <summary>
    /// Logs a debug message to the console. The message is prefixed with "Debug: ". This implementation writes the message to the standard output.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Debug(string message)
    {
        Console.WriteLine($"Debug: {message}");
    }

    /// <summary>
    /// Logs an information message to the console. The message is prefixed with "Info: ". This implementation writes the message to the standard output.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Info(string message)
    {
        Console.WriteLine($"Info: {message}");
    }

    /// <summary>
    /// Logs a warning message to the console. The message is prefixed with "Warning: ". This implementation writes the message to the standard output.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Warning(string message)
    {
        Console.WriteLine($"Warning: {message}");
    }

    /// <summary>
    /// Logs an error message to the console. The message is prefixed with "Error: ". This implementation writes the message to the standard output.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Error(string message)
    {
        Console.WriteLine($"Error: {message}");
    }
    
    /// <summary>
    /// Logs an exception to the console. The message is prefixed with "Error: ". This implementation writes the message to the standard output.
    /// </summary>
    /// <param name="ex">The exception to log.</param> 
    public void Error(Exception ex)
    {
        var sb = new StringBuilder(ex.Message);
        if (ex is AggregateException ex1)
        {
            foreach (var item in ex1.InnerExceptions)
            {
                sb.AppendLine($"  {item.Message}");
            }
        }
        else
        {
            if (ex.InnerException != null)
            {
                sb.AppendLine($"  {ex.InnerException.Message}");
            }
        }
        Console.WriteLine($"Error: {sb}");
    }
}