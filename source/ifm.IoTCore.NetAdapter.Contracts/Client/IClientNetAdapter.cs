namespace ifm.IoTCore.NetAdapter.Contracts.Client;

using System;
using Message;

/// <summary>
/// Provides functionality to interact with a network adapter client.
/// </summary>
public interface IClientNetAdapter : IDisposable
{
    /// <summary>
    /// Gets the remote uri to which the client connects.
    /// </summary>
    /// <returns>The remote uri to which the client connects.</returns>
    Uri RemoteUri { get; }

    /// <summary>
    /// Sends a request to the server with the specified uri.
    /// </summary>
    /// <param name="message">The request message.</param>
    /// <returns>The response message.</returns>
    Message SendRequest(Message message);

    /// <summary>
    /// Sends an event to the server with the specified uri.
    /// </summary>
    /// <param name="message">The event message.</param>
    void SendEvent(Message message);
}