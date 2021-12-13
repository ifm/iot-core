using ifmIoTCore.Elements;
using ifmIoTCore.Messages;
using ifmIoTCore.NetAdapter;
using ifmIoTCore.Utilities;

namespace ifmIoTCore
{
    /// <summary>
    /// Exposes static methods for creating a new instance of the IoTCore class
    /// </summary>
    public class IoTCoreFactory
    {
        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        /// <param name="identifier">Specifies the identifier for the root device element</param>
        /// <param name="logger">The logger used in IoTCore</param>
        /// <returns>The newly created instance</returns>
        public static IIoTCore Create(string identifier, 
            ILogger logger = null)
        {
            var clientNetAdapterManager = new ClientNetAdapterManager();
            var serverNetAdapterManager = new ServerNetAdapterManager(clientNetAdapterManager);
            var messageSender = new MessageSender(clientNetAdapterManager, logger);
            var elementManager = new ElementManager(messageSender, logger);
            var messageHandler = new MessageHandler(elementManager, logger);
            return new IoTCore(identifier, elementManager, serverNetAdapterManager, clientNetAdapterManager, messageHandler, messageSender, logger);
        }
    }
}