namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality to interact with a network adapter server
    /// </summary>
    public interface IServerNetAdapter : IDisposable
    {
        /// <summary>
        /// Gets the IConverter type for the data format, which the network adapter server implements .
        /// </summary>
        string ConverterType { get; }

        /// <summary>
        /// Gets the uri of the network adapter server.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Starts the network adapter server synchronous.
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the network adapter server asynchronous.
        /// </summary>
        Task StartAsync();
        
        /// <summary>
        /// Stops the network adapter server synchronously.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops the network adapter server asynchronously.
        /// </summary>
        Task StopAsync();
    }
}