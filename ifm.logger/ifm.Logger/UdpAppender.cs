namespace ifm.Logger;

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using log4net.Appender;
using log4net.Core;

/// <summary>
/// An appender that sends log events to a specified remote host and port using UDP.
/// </summary>
public class UdpAppender : AppenderSkeleton, IDisposable
{
    private readonly UdpClient _udpClient = new();

    /// <summary>
    /// Gets or sets the remote host name or IP address that log events are sent to.
    /// </summary>
    public string RemoteHost { get; set; }

    /// <summary>
    /// Gets or sets the remote port that log events are sent to.
    /// </summary>
    public int RemotePort { get; set; }

    /// <summary>
    /// Gets or sets the encoding used to convert the formatted message into bytes. Defaults to UTF-8.
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Appends a logging event by sending it to the configured remote host and port via UDP.
    /// </summary>
    /// <param name="loggingEvent">The logging event to append.</param>
    protected override void Append(LoggingEvent loggingEvent)
    {
        if (string.IsNullOrWhiteSpace(RemoteHost))
        {
            ErrorHandler.Error("UdpAppender requires the RemoteHost property to be set.");
            return;
        }

        if (RemotePort <= 0 || RemotePort > 65535)
        {
            ErrorHandler.Error("UdpAppender requires a valid RemotePort (1-65535).");
            return;
        }

        using var writer = new StringWriter();
        Layout?.Format(writer, loggingEvent);

        Send(writer.ToString());
    }

    private void Send(string message)
    {
        try
        {
            var bytes = Encoding.GetBytes(message);
            _udpClient.Send(bytes, bytes.Length, RemoteHost, RemotePort);
        }
        catch (Exception ex)
        {
            ErrorHandler.Error("Failed to send log event via UDP.", ex);
        }
    }

    /// <summary>
    /// Disposes the UdpClient and releases any resources used by the appender.
    /// </summary>
    public void Dispose()
    {
        _udpClient.Close();
        _udpClient?.Dispose();
    }
}
