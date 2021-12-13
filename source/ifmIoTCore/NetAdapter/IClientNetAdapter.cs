namespace ifmIoTCore.NetAdapter
{
    using System;
    using Messages;

    /// <summary>
    /// Provides functionality to interact with a network adapter client
    /// </summary>
    public interface IClientNetAdapter : IDisposable
    {
        /// <summary>
        /// Gets the local uri of the connection to the remote uri
        /// </summary>
        /// <returns>The local uri which was used to connect to the remote uri</returns>
        Uri GetLocalUri();

        /// <summary>
        /// Gets the remote uri to which the client connects
        /// </summary>
        /// <returns>The remote uri to which the client connects</returns>
        Uri GetRemoteUri();

        /// <summary>
        /// Sends a request to the server with the specified uri
        /// </summary>
        /// <param name="message">The request message</param>
        /// <param name="timeout">The timeout to wait for a response</param>
        /// <returns>The response message</returns>
        ResponseMessage SendRequest(RequestMessage message, TimeSpan? timeout = null);

        /// <summary>
        /// Sends an event to the server with the specified uri
        /// </summary>
        /// <param name="message">The event message</param>
        void SendEvent(EventMessage message);

        /// <summary>
        /// Disconnects the client
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Time when client was last used
        /// </summary>
        DateTime LastUsed { get; }
    }
}