namespace ifm.IoTCore.NetAdapter.Contracts.Client;

using System;

/// <summary>
/// Provides functionality to interact with a client network adapter factory.
/// </summary>
public interface IClientNetAdapterFactory : IDisposable
{
    /// <summary>
    /// Gets the scheme that the client factory supports.
    /// </summary>
    string Scheme { get; }

    /// <summary>
    /// Gets the schemes that the client factory supports.
    /// </summary>
    string[] Schemes { get; }

    /// <summary>
    /// Gets the data format, which the client factory supports.
    /// </summary>
    string Format { get; }

    /// <summary>
    /// Creates a client network adapter.
    /// </summary>
    /// <param name="remoteUri">The remote uri to which the created client connects.</param>
    /// <returns>The client network adapter.</returns>
    IClientNetAdapter CreateClient(Uri remoteUri);

    /// <summary>
    /// Removes and disposes the client.
    /// Required for factories that maintain internal lists of created clients.
    /// </summary>
    /// <param name="remoteUri">The remote uri for which a client was created.</param>
    /// <param name="dispose">If true the client is disposed; otherwise not.</param>
    void RemoveClient(Uri remoteUri, bool dispose = true);
}