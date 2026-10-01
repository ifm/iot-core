namespace ifm.IoTCore.Contracts;

using System.Collections.Generic;
using ifm.Logger.Contracts;
using ifm.DataStore.Contracts;
using ifm.IoTCore.ElementManager.Contracts;
using ifm.IoTCore.ElementManager.Contracts.Elements;
using ifm.IoTCore.EventSender.Contracts;
using ifm.IoTCore.MessageDispatcher.Contracts;
using ifm.IoTCore.NetAdapterManager.Contracts;
using ifm.IoTCore.PersistenceManager.Contracts;
using ifm.IoTCore.UserManager.Contracts;

/// <summary>
/// Represents a catalog information entry.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="name">The name of the catalogue.</param>
/// <param name="version">The version of the catalogue.</param>
public class CatalogInfo(string name, string version)
{
    /// <summary>
    /// The name of the catalogue.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The version of the catalogue.
    /// </summary>
    public string Version { get; } = version;
}

/// <summary>
/// Represents a component information entry.
/// Initializes a new instance of the class.
/// </summary>
/// <param name="name">The name of the component.</param>
/// <param name="version">The version of the component.</param>
public class ComponentInfo(string name, string version)
{
    /// <summary>
    /// The name of the component.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The version of the component.
    /// </summary>
    public string Version { get; } = version ?? "-";
}

/// <summary>
/// Provides functionality to interact with the IoTCore.
/// </summary>
public interface IIoTCore
{
    /// <summary>
    /// Gets the API version.
    /// </summary>
    string ApiVersion { get; }

    /// <summary>
    /// Gets the identifier of the root element.
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Gets the root device element.
    /// </summary>
    IBaseElement Root { get; }

    /// <summary>
    /// Gets the catalogs.
    /// </summary>
    IReadOnlyList<CatalogInfo> Catalogs { get; }

    /// <summary>
    /// Adds a catalog to the catalogs.
    /// </summary>
    /// <param name="catalogInfo">The catalog info.</param>
    void AddCatalog(CatalogInfo catalogInfo);

    /// <summary>
    /// Gets the components.
    /// </summary>
    IReadOnlyList<ComponentInfo> Components { get; }

    /// <summary>
    /// Adds a component to the components.
    /// </summary>
    /// <param name="componentInfo">The component info.</param>
    void AddComponent(ComponentInfo componentInfo);

    /// <summary>
    /// Gets the device classes.
    /// </summary>
    IReadOnlyList<string> DeviceClasses { get; }

    /// <summary>
    /// Adds a device class to the device classes.
    /// </summary>
    /// <param name="deviceClass">The device class.</param>
    void AddDeviceClass(string deviceClass);

    /// <summary>
    /// Gets the element manager.
    /// </summary>
    IElementManager ElementManager { get; }

    /// <summary>
    /// Gets the event sender.
    /// </summary>
    IEventSender EventSender { get; }

    /// <summary>
    /// Gets the message dispatcher.
    /// </summary>
    IMessageDispatcher MessageDispatcher { get; }

    /// <summary>
    /// Gets the network adapter manager.
    /// </summary>
    INetAdapterManager NetAdapterManager { get; }

    /// <summary>
    /// Gets the client network adapter manager.
    /// </summary>
    IClientNetAdapterManager ClientNetAdapterManager { get; }

    /// <summary>
    /// Gets the server network adapter manager.
    /// </summary>
    IServerNetAdapterManager ServerNetAdapterManager { get; }

    /// <summary>
    /// Gets the user manager.
    /// </summary>
    IUserManager UserManager { get; }

    /// <summary>
    /// Gets the session manager.
    /// </summary>
    public ISessionManager SessionManager { get; }

    /// <summary>
    /// Gets the persistence manager.
    /// </summary>
    IPersistenceManager PersistenceManager { get; }

    /// <summary>
    /// Gets the data store.
    /// </summary>
    IDataStore DataStore { get; }

    /// <summary>
    /// Gets the logger.
    /// </summary>
    ILogger Logger { get; }
}