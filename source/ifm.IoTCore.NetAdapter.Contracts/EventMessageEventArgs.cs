namespace ifm.IoTCore.NetAdapter.Contracts;

using Message;

/// <summary>
/// Provides data for an event message event.
/// </summary>
public class EventMessageEventArgs : System.ComponentModel.CancelEventArgs
{
    /// <summary>
    /// The event message.
    /// </summary>
    public Message Message { get; }

    /// <summary>
    /// The identifier of the event sender.
    /// </summary>
    public string SenderId { get; }

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="message">The event message.</param>
    /// <param name="senderId">The identifier of the event sender.</param>
    public EventMessageEventArgs(Message message, string senderId = null)
    {
        Message = message;
        SenderId = senderId;
    }
}