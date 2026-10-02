namespace ifm.IoTCore.NetAdapter.Contracts;

using Message;

/// <summary>
/// Provides data for a request message event.
/// </summary>
public class RequestMessageEventArgs : System.ComponentModel.CancelEventArgs
{
    /// <summary>
    /// The request message.
    /// </summary>
    public Message RequestMessage { get; }

    /// <summary>
    /// The response message.
    /// </summary>
    public Message ResponseMessage { get; set; }

    /// <summary>
    /// The identifier of the event sender.
    /// </summary>
    public string SenderId { get; }

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="requestMessage">The request message.</param>
    /// <param name="responseMessage">The response message.</param>
    /// <param name="senderId">The identifier of the event sender.</param>
    public RequestMessageEventArgs(Message requestMessage, Message responseMessage = null, string senderId = null)
    {
        RequestMessage = requestMessage;
        ResponseMessage = responseMessage;
        SenderId = senderId;
    }
}