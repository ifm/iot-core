using System;
using System.Collections.Generic;

namespace ifmIoTCore.NetAdapter
{
    /// <summary>
    /// Provides functionality to interact with the server network adapter manager
    /// </summary>
    public interface IServerNetAdapterManager : IDisposable
    {
        /// <summary>
        /// Gets the collection of the registered network adapter servers
        /// </summary>
        IEnumerable<IServerNetAdapter> ServerNetAdapters { get; }

        /// <summary>
        /// Registers a server network adapter
        /// </summary>
        /// <param name="serverNetAdapter">The server network adapter to register</param>
        void RegisterServerNetAdapter(IServerNetAdapter serverNetAdapter);

        /// <summary>
        /// Removes a server network adapter
        /// </summary>
        /// <param name="serverNetAdapter">The server network adapter to remove</param>
        void RemoveServerNetAdapter(IServerNetAdapter serverNetAdapter);

        /// <summary>
        /// Finds the server network adapter with the specified uri
        /// </summary>
        /// <returns>The server network adapter if successful; otherwise null</returns>
        /// <param name="uri">The uri of the network adapter server</param>
        IServerNetAdapter FindServerNetAdapter(Uri uri);

        /// <summary>
        /// Searches the registered server network adapters for a server network adapter which can be connected from the remote uri
        /// </summary>
        /// <param name="remoteUri">The remote uri</param>
        /// <returns>The server network adapter if successful; otherwise null</returns>
        IServerNetAdapter FindReverseServerNetAdapter(Uri remoteUri);
    }
}