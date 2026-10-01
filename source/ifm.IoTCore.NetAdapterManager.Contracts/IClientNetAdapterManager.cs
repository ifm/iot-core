namespace ifm.IoTCore.NetAdapterManager.Contracts;

using System;
using System.Collections.Generic;
using ifm.IoTCore.NetAdapter.Contracts.Client;

/// <summary>
/// Provides functionality to interact with the client network adapter manager.
/// </summary>
public interface IClientNetAdapterManager : IDisposable
{
    /// <summary>
    /// Gets the collection of the registered client network adapter factories.
    /// </summary>
    IEnumerable<IClientNetAdapterFactory> ClientNetAdapterFactories { get; }

    /// <summary>
    /// Registers a client network adapter factory.
    /// </summary>
    /// <param name="clientNetAdapterFactory">The client network adapter factory to register.</param>
    void RegisterClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory);

    /// <summary>
    /// Removes a client network adapter factory.
    /// </summary>
    /// <param name="clientNetAdapterFactory">The client network adapter factory to remove.</param>
    void RemoveClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory);

    /// <summary>
    /// Creates a client network adapter using the registered client network adapter factories which can connect to the provided uri.
    /// </summary>
    /// <param name="targetUri">The remote uri to which the created client network adapter can connect.</param>
    /// <returns></returns>
    IClientNetAdapter CreateClientNetAdapter(Uri targetUri);

    /// <summary>
    /// Finds all client network adapter factories that support the specified scheme.
    /// </summary>
    /// <param name="scheme">The scheme that the client network adapter factories support.</param>
    /// <returns>The client network adapter factories found.</returns>
    IEnumerable<IClientNetAdapterFactory> FindClientNetAdapterFactories(string scheme);
}