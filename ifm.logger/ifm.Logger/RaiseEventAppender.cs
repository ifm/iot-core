namespace ifm.Logger;

using System;
using System.IO;
using log4net.Appender;
using log4net.Core;

/// <summary>
/// An appender that raises an event whenever a log message is appended. This allows external subscribers to handle log messages in real-time.
/// </summary>
public class RaiseEventAppender : AppenderSkeleton
{
    /// <summary>
    /// Occurs when a log message is appended. Subscribers can handle this event to process log messages in real-time.
    /// </summary>
    public event EventHandler<RaiseEventAppenderEventArgs> LogAppended;

    /// <summary>
    /// Appends a logging event by raising the LogAppended event with the formatted log message.
    /// </summary>
    /// <param name="loggingEvent">The logging event to append.</param>
    protected override void Append(LoggingEvent loggingEvent)
    {
        using var writer = new StringWriter();
        Layout?.Format(writer, loggingEvent);
        OnLogAppended(writer.ToString());
    }

    private void OnLogAppended(string message)
    {
        LogAppended?.Invoke(this, new RaiseEventAppenderEventArgs { Message = message });
    }
}

/// <summary>
/// Provides data for the LogAppended event, containing the formatted log message.
/// </summary>
public class RaiseEventAppenderEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the formatted log message associated with the event.
    /// </summary>
    public string Message { get; set; }
}