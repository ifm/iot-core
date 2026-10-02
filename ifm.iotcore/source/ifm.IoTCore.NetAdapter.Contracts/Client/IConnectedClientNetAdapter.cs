namespace ifm.IoTCore.NetAdapter.Contracts.Client;

using System;

/// <summary>
/// Provides functionality to interact with a connected network adapter client.
/// </summary>
public interface IConnectedClientNetAdapter : IClientNetAdapter
{
    /// <summary>
    /// Connects the client to the server.
    /// </summary>
    void Connect();

    /// <summary>
    /// Disconnects the client from the server.
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Gets the connection status of the client.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// The event that is raised after the client connected to the server.
    /// </summary>
    public event EventHandler<ConnectionEventArgs> Connected;

    /// <summary>
    /// The event that is raised after the client disconnected from the server.
    /// </summary>
    public event EventHandler<ConnectionEventArgs> Disconnected;

    /// <summary>
    /// The event that is raised after an event message was received from the server and before the event message is processed.
    /// </summary>
    event EventHandler<EventMessageEventArgs> EventReceived;
}