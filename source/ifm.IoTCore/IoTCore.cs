namespace ifm.IoTCore;

using System;
using System.Collections.Generic;
using ifm.Common;
using DataStore.Contracts;
using Logger.Contracts;
using Contracts;
using ElementManager.Contracts;
using ElementManager.Contracts.Elements;
using EventSender.Contracts;
using MessageDispatcher.Contracts;
using NetAdapterManager.Contracts;
using PersistenceManager.Contracts;
using UserManager.Contracts;

/// <summary>
/// Represents an IoTCore.
/// </summary>
public class IoTCore : IIoTCore
{
    private readonly DeviceElement _deviceElement;
    private readonly List<CatalogInfo> _catalogs = [];
    private readonly List<ComponentInfo> _components = [];
    private readonly List<string> _deviceClasses = [];

    /// <summary>
    /// Gets the API version.
    /// </summary>
    public string ApiVersion => "V2.0";

    /// <summary>
    /// Gets the identifier of the root element.
    /// </summary>
    public string Identifier => _deviceElement.Root.Identifier;

    /// <summary>
    /// Gets the root device element.
    /// </summary>
    public IBaseElement Root => _deviceElement.Root;

    /// <summary>
    /// Gets the catalogs.
    /// </summary>
    public IReadOnlyList<CatalogInfo> Catalogs => _catalogs;

    /// <summary>
    /// Adds a catalog to the catalogs.
    /// </summary>
    /// <param name="catalogInfo">The catalog info.</param>
    public void AddCatalog(CatalogInfo catalogInfo)
    {
        if (catalogInfo == null) throw new ArgumentNullException(nameof(catalogInfo));

        _catalogs.Add(catalogInfo);
    }

    /// <summary>
    /// Gets the components.
    /// </summary>
    public IReadOnlyList<ComponentInfo> Components => _components;

    /// <summary>
    /// Adds a component to the components.
    /// </summary>
    /// <param name="componentInfo">The component info.</param>
    public void AddComponent(ComponentInfo componentInfo)
    {
        if (componentInfo == null) throw new ArgumentNullException(nameof(componentInfo));

        _components.Add(componentInfo);
    }

    /// <summary>
    /// Gets the device classes.
    /// </summary>
    public IReadOnlyList<string> DeviceClasses => _deviceClasses;

    /// <summary>
    /// Adds a device class to the device classes.
    /// </summary>
    /// <param name="deviceClass">The device class.</param>
    public void AddDeviceClass(string deviceClass)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(deviceClass, nameof(deviceClass));

        _deviceClasses.Add(deviceClass);
    }

    /// <summary>
    /// Gets the element manager.
    /// </summary>
    public IElementManager ElementManager { get; }

    /// <summary>
    /// Gets the event sender.
    /// </summary>
    public IEventSender EventSender { get; }

    /// <summary>
    /// Gets the message dispatcher.
    /// </summary>
    public IMessageDispatcher MessageDispatcher { get; }

    /// <summary>
    /// Gets the network adapter manager.
    /// </summary>
    public INetAdapterManager NetAdapterManager { get; }

    /// <summary>
    /// Gets the client network adapter manager.
    /// </summary>
    public IClientNetAdapterManager ClientNetAdapterManager => NetAdapterManager.Client;

    /// <summary>
    /// Gets the server network adapter manager.
    /// </summary>
    public IServerNetAdapterManager ServerNetAdapterManager => NetAdapterManager.Server;

    /// <summary>
    /// Gets the user manager.
    /// </summary>
    public IUserManager UserManager { get; }

    /// <summary>
    /// Gets the session manager.
    /// </summary>
    public ISessionManager SessionManager { get; }

    /// <summary>
    /// Gets the persistence manager.
    /// </summary>
    public IPersistenceManager PersistenceManager { get; }

    /// <summary>
    /// Gets the data store.
    /// </summary>
    public IDataStore DataStore { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="identifier">The identifier of the root element.</param>
    /// <param name="elementManager">The element manager.</param>
    /// <param name="eventSender">The event sender.</param>
    /// <param name="messageDispatcher">The message dispatcher.</param>
    /// <param name="netAdapterManager">The network adapter manager.</param>
    /// <param name="userManager">The user manager.</param>
    /// <param name="sessionManager">The session manager.</param>
    /// <param name="persistenceManager">The persistence manager.</param>
    /// <param name="dataStore">The data store.</param>
    /// <param name="logger">The logger.</param>
    public IoTCore(string identifier,
        IElementManager elementManager,
        IEventSender eventSender,
        IMessageDispatcher messageDispatcher,
        INetAdapterManager netAdapterManager,
        IUserManager userManager,
        ISessionManager sessionManager,
        IPersistenceManager persistenceManager,
        IDataStore dataStore, 
        ILogger logger)
    {
        ExceptionHelpers.ThrowIfNullOrEmpty(identifier, nameof(identifier));

        ElementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));
        EventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
        NetAdapterManager = netAdapterManager ?? throw new ArgumentNullException(nameof(netAdapterManager));
        UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        SessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
        PersistenceManager = persistenceManager ?? throw new ArgumentNullException(nameof(persistenceManager));
        DataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _deviceElement = new DeviceElement(identifier, 
            ApiVersion, 
            _catalogs,
            _components,
            ElementManager, 
            NetAdapterManager.Server, 
            UserManager);
        ElementManager.Root = _deviceElement.Root;
    }
}
