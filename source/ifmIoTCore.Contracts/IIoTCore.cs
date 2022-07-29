namespace ifmIoTCore
{
    using DataStore;
    using Elements;
    using Logger;
    using Messages;
    using NetAdapter;

    /// <summary>
    /// Provides functionality to interact with the IoTCore
    /// </summary>
    public interface IIoTCore : IElementManager, IMessageHandler, IClientNetAdapterManager, IServerNetAdapterManager
    {
        /// <summary>
        /// Gets the IoTCore identifier
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Gets the IoTCore version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the element manager
        /// </summary>
        IElementManager ElementManager { get; }

        /// <summary>
        /// Gets the message handler
        /// </summary>
        IMessageHandler MessageHandler { get; }

        /// <summary>
        /// Gets the client network adapter manager
        /// </summary>
        IClientNetAdapterManager ClientNetAdapterManager { get; }

        /// <summary>
        /// Gets the server network adapter manager
        /// </summary>
        IServerNetAdapterManager ServerNetAdapterManager { get; }

        /// <summary>
        /// Gets the data store
        /// </summary>
        IDataStore DataStore { get; }

        /// <summary>
        /// Gets the logger
        /// </summary>
        ILogger Logger { get; }
    }
}