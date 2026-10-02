namespace ifm.IoTCore;

using ifm.Common;
using DataStore.Contracts;
using Logger.Contracts;
using Contracts;
using UserManager.Contracts;

/// <summary>
/// Exposes static methods for creating a new instance of the IoTCore class.
/// </summary>
public static class IoTCoreFactory
{
    /// <summary>
    /// Creates a new instance of an IoTCore.
    /// </summary>
    /// <param name="identifier">The identifier of the root element.</param>
    /// <param name="dataStore">The data store.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The IoTCore.</returns>
    public static IIoTCore Create(string identifier, 
        IDataStore dataStore, 
        ILogger logger)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(identifier, nameof(identifier));
        ExceptionHelpers.ThrowIfNull(dataStore, nameof(dataStore));
        ExceptionHelpers.ThrowIfNull(logger, nameof(logger));

        var sessionManager = new SessionManager.SessionManager(dataStore);
        var userManager = new UserManager.UserManager(dataStore, false, "admin", null, true);
        var netAdapterManager = new NetAdapterManager.NetAdapterManager();
        var eventSender = new EventSender.EventSender(netAdapterManager.Client, netAdapterManager.Server, logger);
        var elementManager = new ElementManager.ElementManager(dataStore, eventSender);
        var messageDispatcher = new MessageDispatcher.MessageDispatcher(elementManager, userManager, sessionManager, logger);
        var persistenceManager = new PersistenceManager.PersistenceManager($"{identifier}_persist.txt", elementManager, logger);

        return new IoTCore(identifier,
            elementManager,
            eventSender,
            messageDispatcher,
            netAdapterManager,
            userManager,
            sessionManager,
            persistenceManager,
            dataStore,
            logger);
    }

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="identifier">Specifies the identifier for the root device element.</param>
    /// <param name="enableAuthentication">If true authentication is enabled; otherwise not.</param>
    /// <param name="user">The default user.</param>
    /// <param name="password">The password for the default user.</param>
    /// <param name="changePassword">If true the password needs to be changed; otherwise not.</param>
    /// <param name="dataStore">The data store.</param>
    /// <param name="logger">The logger used in the IoTCore modules.</param>
    /// <returns>The created object.</returns>
    public static IIoTCore Create(string identifier,
        bool enableAuthentication, 
        string user, 
        string password,
        bool changePassword,
        IDataStore dataStore,
        ILogger logger)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(identifier, nameof(identifier));
        ExceptionHelpers.ThrowIfNull(dataStore, nameof(dataStore));
        ExceptionHelpers.ThrowIfNull(logger, nameof(logger));

        var sessionManager = new SessionManager.SessionManager(dataStore);
        var userManager = new UserManager.UserManager(dataStore, enableAuthentication, user, password, changePassword);
        var netAdapterManager = new NetAdapterManager.NetAdapterManager();
        var eventSender = new EventSender.EventSender(netAdapterManager.Client, netAdapterManager.Server, logger);
        var elementManager = new ElementManager.ElementManager(dataStore, eventSender);
        var messageDispatcher = new MessageDispatcher.MessageDispatcher(elementManager, userManager, sessionManager, logger);
        var persistenceManager = new PersistenceManager.PersistenceManager($"{identifier}_persist.txt", elementManager, logger);

        return new IoTCore(identifier,
            elementManager,
            eventSender,
            messageDispatcher,
            netAdapterManager,
            userManager,
            sessionManager,
            persistenceManager,
            dataStore,
            logger);
    }

    /// <summary>
    /// Creates a new instance of an IoTCore.
    /// </summary>
    /// <param name="identifier">The identifier of the root element.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="dataStore">The data store.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>The IoTCore.</returns>
    public static IIoTCore Create(string identifier,
        IUserManager userManager, 
        IDataStore dataStore,
        ILogger logger)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(identifier, nameof(identifier));
        ExceptionHelpers.ThrowIfNull(userManager, nameof(userManager));
        ExceptionHelpers.ThrowIfNull(dataStore, nameof(dataStore));
        ExceptionHelpers.ThrowIfNull(logger, nameof(logger));

        var sessionManager = new SessionManager.SessionManager(dataStore);
        var netAdapterManager = new NetAdapterManager.NetAdapterManager();
        var eventSender = new EventSender.EventSender(netAdapterManager.Client, netAdapterManager.Server, logger);
        var elementManager = new ElementManager.ElementManager(dataStore, eventSender);
        var messageDispatcher = new MessageDispatcher.MessageDispatcher(elementManager, userManager, sessionManager, logger);
        var persistenceManager = new PersistenceManager.PersistenceManager($"{identifier}_persist.txt", elementManager, logger);

        return new IoTCore(identifier,
            elementManager,
            eventSender,
            messageDispatcher,
            netAdapterManager,
            userManager,
            sessionManager,
            persistenceManager,
            dataStore,
            logger);
    }
}