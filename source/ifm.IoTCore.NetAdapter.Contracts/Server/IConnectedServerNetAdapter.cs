namespace ifm.IoTCore.NetAdapter.Contracts.Server;

using System;
using System.Collections.Generic;
using Message;

/// <summary>
/// Provides functionality to interact with a connected server network adapter.
/// </summary>
public interface IConnectedServerNetAdapter : IServerNetAdapter
{
    /// <summary>
    /// Gets the client identifiers which are connected to that server.
    /// </summary>
    IEnumerable<string> ConnectedClients { get; }

    /// <summary>
    /// Checks if a client with specified identifier is connected to that server.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <returns>true, if a client with identifier is connected; otherwise false.</returns>
    bool IsClientConnected(string clientId);

    /// <summary>
    /// Disconnects the client with the specified identifier.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    void DisconnectClient(string clientId);

    /// <summary>
    /// Sends an event message to the connected client with the specified identifier.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="message">The event message.</param>
    void SendEvent(string clientId, Message message);

    /// <summary>
    /// The event that is raised after a client connected to the server.
    /// </summary>
    public event EventHandler<ConnectionEventArgs> ClientConnected;

    /// <summary>
    /// The event that is raised after a client disconnected from the server.
    /// </summary>
    public event EventHandler<ConnectionEventArgs> ClientDisconnected;

    /// <summary>
    /// The event that is raised after a request message was received from a client and before the request message is processed.
    /// </summary>
    public event EventHandler<RequestMessageEventArgs> RequestReceived;

    /// <summary>
    /// The event that is raised after an event message was received from a client and before the event message is processed.
    /// </summary>
    public event EventHandler<EventMessageEventArgs> EventReceived;
}