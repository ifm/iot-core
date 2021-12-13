using System;
using System.Collections.Generic;

namespace ifmIoTCore.NetAdapter
{
    /// <summary>
    /// Provides functionality to interact with the client network adapter manager
    /// </summary>
    public interface IClientNetAdapterManager : IDisposable
    {
        /// <summary>
        /// Gets the collection of the registered client network adapter factories
        /// </summary>
        IEnumerable<IClientNetAdapterFactory> ClientNetAdapterFactories { get; }

        /// <summary>
        /// Registers a client network adapter factory
        /// </summary>
        /// <param name="clientNetAdapterFactory">The client network adapter factory to register</param>
        void RegisterClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory);

        /// <summary>
        /// Removes a client network adapter factory
        /// </summary>
        /// <param name="clientNetAdapterFactory">The client network adapter factory to remove</param>
        void RemoveClientNetAdapterFactory(IClientNetAdapterFactory clientNetAdapterFactory);

        /// <summary>
        /// Creates a client network adapter using the registered network adapter client factories which can connect to the provided remote uri
        /// </summary>
        /// <param name="remoteUri">The remote uri to which the created client can connect</param>
        /// <returns></returns>
        IClientNetAdapter CreateClientNetAdapter(Uri remoteUri);
    }
}