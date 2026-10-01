namespace ifm.IoTCore.NetAdapterManager.Contracts;

/// <summary>
/// Provides functionality to interact with the network adapter manager.
/// </summary>
public interface INetAdapterManager
{
    /// <summary>
    /// Gets the client network adapter manager.
    /// </summary>
    IClientNetAdapterManager Client { get; }

    /// <summary>
    /// Gets the server network adapter manager.
    /// </summary>
    IServerNetAdapterManager Server { get; }
}