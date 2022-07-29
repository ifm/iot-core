namespace ifmIoTCore.NetAdapter
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality to interact with a server network adapter
    /// </summary>
    public interface IServerNetAdapter : IDisposable
    {
        /// <summary>
        /// Gets the scheme that the server supports
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Gets the data format, which the server supports
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Gets the uri of the server
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// If true the server is listening; otherwise not
        /// </summary>
        bool IsListening { get; }

        /// <summary>
        /// Starts the server synchronous
        /// </summary>
        void Start();

        /// <summary>
        /// Starts the server asynchronously
        /// </summary>
        Task StartAsync();
        
        /// <summary>
        /// Stops the server synchronously
        /// </summary>
        void Stop();

        /// <summary>
        /// Stops the server asynchronously
        /// </summary>
        Task StopAsync();
    }
}