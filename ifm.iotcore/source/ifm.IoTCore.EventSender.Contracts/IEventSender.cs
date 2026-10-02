namespace ifm.IoTCore.EventSender.Contracts;

using System;

/// <summary>
/// Provides functionality to interact with the event sender.
/// </summary>
public interface IEventSender : IDisposable
{
    /// <summary>
    /// Sends an event to the recipient at target URL.
    /// </summary>
    /// <param name="targetUrl">The target URL.</param>
    /// <param name="data">The event data to send.</param>
    void SendEvent(string targetUrl, EventMessageDataBase data);
}