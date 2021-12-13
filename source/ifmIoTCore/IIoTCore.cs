namespace ifmIoTCore
{
    using System;
    using Elements;
    using Messages;
    using NetAdapter;
    using Persistence;
    using Utilities;

    /// <summary>
    /// Provides functionality to interact with the IoTCore
    /// </summary>
    public interface IIoTCore : IElementManager, IMessageHandler, IMessageSender, IServerNetAdapterManager, IClientNetAdapterManager, IDisposable
    {
        /// <summary>
        /// Get the version of the IoTCore
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the root element of the IoTCore element tree
        /// </summary>
        IDeviceElement Root { get; }

        /// <summary>
        /// gets or sets the persistence manager
        /// </summary>
        IPersistenceManager PersistenceManager { get; set; }

        /// <summary>
        /// Gets the datastore.
        /// </summary>
        IDataStore DataStore { get; }

        /// <summary>
        /// The logging instance to use.
        /// </summary>
        ILogger Logger { get; }
    }
}