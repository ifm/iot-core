namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Collections.Generic;
    using Messages;

    public interface IConnectedServerNetAdapter : IServerNetAdapter
    {

        /// <summary>
        /// Gets the client uris which are connected to that server
        /// </summary>
        IEnumerable<Uri> ConnectedClients { get; }

        /// <summary>
        /// Sends an event to a connected client
        /// </summary>
        /// <param name="remoteUri">The uri of the connected client</param>
        /// <param name="eventMessage">The event message</param>
        void SendEvent(Uri remoteUri, EventMessage eventMessage);
    }
}