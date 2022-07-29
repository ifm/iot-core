namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides functionality to interact with the server network adapter manager
    /// </summary>
    public interface IServerNetAdapterManager : IDisposable
    {
        /// <summary>
        /// Gets the collection of the registered server network adapters
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
        /// <param name="uri">The uri of the server network adapter</param>
        /// <returns>The server network adapters found; otherwise null</returns>
        IServerNetAdapter FindServerNetAdapter(Uri uri);

        /// <summary>
        /// Finds all server network adapters that support the specified scheme
        /// </summary>
        /// <param name="scheme">The scheme that the server network adapter supports</param>
        /// <returns>The server network adapters found; otherwise null</returns>
        IEnumerable<IServerNetAdapter> FindServerNetAdapters(string scheme);
    }
}