namespace ifmIoTCore.NetAdapter
{
    using System;

    /// <summary>
    /// Provides functionality to interact with a network adapter client factory
    /// </summary>
    public interface IClientNetAdapterFactory : IDisposable
    {
        /// <summary>
        /// Gets the protocol, which the network adapter client implements
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Creates a network adapter client
        /// </summary>
        /// <param name="remoteUri">The remote uri to connect to</param>
        /// <returns>
        /// The INetAdapterClient interface of the created network adapter client
        /// </returns>
        IClientNetAdapter CreateClient(Uri remoteUri);
    }
}
