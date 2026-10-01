namespace ifm.IoTCore.NetAdapter.Contracts.Server;

using System;

/// <summary>
/// Provides functionality to interact with a server network adapter.
/// </summary>
public interface IServerNetAdapter : IDisposable
{
    /// <summary>
    /// Gets the scheme which the server supports.
    /// </summary>
    string Scheme { get; }

    /// <summary>
    /// Gets the data format which the server supports.
    /// </summary>
    string Format { get; }

    /// <summary>
    /// Gets the uri of the server.
    /// </summary>
    Uri Uri { get; }

    /// <summary>
    /// true if the server is listening; otherwise false.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// Starts the server.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    void Stop();

    /// <summary>
    /// Resumes the server state. 
    /// </summary>
    void Resume();
}