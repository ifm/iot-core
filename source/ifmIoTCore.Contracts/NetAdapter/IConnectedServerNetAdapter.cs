namespace ifmIoTCore.NetAdapter
{
    using System.Collections.Generic;
    using Messages;

    public interface IConnectedServerNetAdapter : IServerNetAdapter
    {
        /// <summary>
        /// Gets the client identifiers which are connected to that server
        /// </summary>
        IEnumerable<string> ConnectedClients { get; }

        /// <summary>
        /// Checks if a client with specified identifier is connected to that server
        /// </summary>
        /// <param name="clientId">The client identifier</param>
        /// <returns>True, if a client with identifier is connected; otherwise false</returns>
        bool IsClientConnected(string clientId);

        /// <summary>
        /// Sends an event to the connected client with the specified identifier
        /// </summary>
        /// <param name="clientId">The client identifier</param>
        /// <param name="eventMessage">The event message</param>
        void SendEvent(string clientId, Message eventMessage);
    }
}