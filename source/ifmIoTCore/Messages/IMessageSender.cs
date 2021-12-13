using System;

namespace ifmIoTCore.Messages
{
    /// <summary>
    /// Provides functionality to interact with the message sender
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends a request to the server with the specified uri
        /// </summary>
        /// <param name="remoteUri">The uri of the remote server</param>
        /// <param name="message">The request message</param>
        /// <param name="timeout">The timeout to wait for a response</param>
        /// <returns>The response message</returns>
        ResponseMessage SendRequest(Uri remoteUri, RequestMessage message, TimeSpan? timeout = null);

        /// <summary>
        /// Sends an event to the server with the specified uri
        /// </summary>
        /// <param name="remoteUri">The uri of the remote server</param>
        /// <param name="message">The event message</param>
        void SendEvent(Uri remoteUri, EventMessage message);
    }
}