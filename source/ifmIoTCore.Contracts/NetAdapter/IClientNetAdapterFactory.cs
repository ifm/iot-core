namespace ifmIoTCore.NetAdapter
{
    using System;

    /// <summary>
    /// Provides functionality to interact with a client network adapter factory
    /// </summary>
    public interface IClientNetAdapterFactory : IDisposable
    {
        /// <summary>
        /// Gets the scheme that the client supports
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Gets the data format, which the network adapter client supports
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Creates a client network adapter
        /// </summary>
        /// <param name="remoteUri">The remote uri to which the created client connects</param>
        /// <returns>The client network adapter</returns>
        IClientNetAdapter CreateClient(Uri remoteUri);
    }
}
