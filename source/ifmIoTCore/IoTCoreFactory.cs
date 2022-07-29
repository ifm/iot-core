namespace ifmIoTCore
{
    using DataStore;
    using Logger;
    using System;
    using Elements;
    using Messages;
    using NetAdapter;

    /// <summary>
    /// Exposes static methods for creating a new instance of the IoTCore class
    /// </summary>
    public class IoTCoreFactory
    {
        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        /// <param name="identifier">Specifies the identifier for the root device element</param>
        /// <param name="dataStore">The data store used in IoTCore</param>
        /// <param name="logger">The logger used in IoTCore</param>
        /// <returns>The newly created instance</returns>
        public static IIoTCore Create(string identifier, 
            IDataStore dataStore = null,
            ILogger logger = null)
        {
            if (string.IsNullOrEmpty(identifier)) throw new ArgumentNullException(nameof(identifier));

            var clientNetAdapterManager = new ClientNetAdapterManager();
            var serverNetAdapterManager = new ServerNetAdapterManager();
            var elementManager = new ElementManager(clientNetAdapterManager, serverNetAdapterManager, logger);
            var messageHandler = new MessageHandler(elementManager, logger);

            return new IoTCore.IoTCore(identifier, elementManager, messageHandler, clientNetAdapterManager, serverNetAdapterManager, dataStore, logger);
        }
    }
}