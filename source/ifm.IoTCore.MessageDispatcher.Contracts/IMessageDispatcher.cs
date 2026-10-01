namespace ifm.IoTCore.MessageDispatcher.Contracts;

using Message;

/// <summary>
/// Provides functionality to interact with the message dispatcher.
/// </summary>
public interface IMessageDispatcher
{
    /// <summary>
    /// Handles a request.
    /// </summary>
    /// <param name="message">The request message.</param>
    /// <returns>The response message.</returns>
    Message HandleRequest(Message message);

    /// <summary>
    /// Handles an event.
    /// </summary>
    /// <param name="message">The event message.</param>
    void HandleEvent(Message message);
}