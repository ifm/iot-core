namespace ifm.IoTCore.ElementManager.Contracts.Elements.ServiceData.Responses;

using System.Collections.Generic;
using System.Linq;
using ifm.Common.Variant;

/// <summary>
/// Represents the outgoing data for a IDeviceElement.GetIdentity service call.
/// </summary>
public class GetIdentityResponseServiceData
{
    /// <summary>
    /// Represents a catalogue info for a IDeviceElement.GetIdentity service call.
    /// </summary>
    public class CatalogInfo
    {
        /// <summary>
        /// The name of the catalogue.
        /// </summary>
        [VariantProperty("name", Required = true)]
        public string Name { get; set; }

        /// <summary>
        /// The version of the catalogue.
        /// </summary>
        [VariantProperty("version", Required = true)]
        public string Version { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter.
        /// </summary>
        [VariantConstructor]
        public CatalogInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The name of the catalogue.</param>
        /// <param name="version">The version of the catalogue.</param>
        public CatalogInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }
    }

    /// <summary>
    /// Represents a component info for a IDeviceElement.GetIdentity service call.
    /// </summary>
    public class ComponentInfo
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        [VariantProperty("name", Required = true)]
        public string Name { get; set; }

        /// <summary>
        /// The version of the component.
        /// </summary>
        [VariantProperty("version", Required = true)]
        public string Version { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter.
        /// </summary>
        [VariantConstructor]
        public ComponentInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="version">The version of the component.</param>
        public ComponentInfo(string name, string version)
        {
            Name = name;
            Version = version ?? "-";
        }
    }

    /// <summary>
    /// Represents a server info for a IDeviceElement.GetIdentity service call.
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// The type of the server.
        /// </summary>
        [VariantProperty("type", Required = true)]
        public string Type { get; set; }

        /// <summary>
        /// The endpoint of the server.
        /// </summary>
        [VariantProperty("uri", Required = true)]
        public string Uri { get; set; }

        /// <summary>
        /// The list of supported data formats of the server.
        /// </summary>
        [VariantProperty("formats", Required = true)]
        public List<string> Formats { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter.
        /// </summary>
        [VariantConstructor]
        public ServerInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="type">The type of the server.</param>
        /// <param name="uri">The uri of the server.</param>
        /// <param name="formats">The list of supported data formats of the server.</param>
        public ServerInfo(string type, string uri, List<string> formats)
        {
            Type = type;
            Uri = uri;
            Formats = formats;
        }
    }

    /// <summary>
    /// Represents an IoT info for a IDeviceElement.GetIdentity service call.
    /// </summary>
    public class IoTInfo
    {
        /// <summary>
        /// The name of the IoTCore root element.
        /// </summary>
        [VariantProperty("name", Required = true)]
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier of the IoTCore root element.
        /// </summary>
        [VariantProperty("uid", IgnoredIfNull = true)]
        public string Uid { get; set; }

        /// <summary>
        /// The version of the IoTCore API.
        /// </summary>
        [VariantProperty("version", Required = true)]
        public string Version { get; set; }

        /// <summary>
        /// The supported catalog versions.
        /// </summary>
        [VariantProperty("catalogue", IgnoredIfNull = true)]
        public List<CatalogInfo> Catalogs { get; set; }

        /// <summary>
        /// The component versions.
        /// </summary>
        [VariantProperty("component", IgnoredIfNull = true)]
        public List<ComponentInfo> Components { get; set; }

        /// <summary>
        /// The list of available network adapter servers in the IoTCore.
        /// </summary>
        [VariantProperty("serverlist", IgnoredIfNull = true)]
        public List<ServerInfo> Servers { get; set; }

        /// <summary>
        /// The device class of the IoTCore.
        /// </summary>
        [VariantProperty("deviceclass", IgnoredIfNull = true)]
        public List<string> DeviceClasses { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter.
        /// </summary>
        [VariantConstructor]
        public IoTInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="name">The name of the IoTCore.</param>
        /// <param name="uid">The unique id of the IoTCore.</param>
        /// <param name="version">The version of the IoTCore.</param>
        /// <param name="catalogs">The supported catalogs of the IoTCore.</param>
        /// <param name="components">The components.</param>
        /// <param name="servers">The list of available network adapter servers in the IoTCore.</param>
        /// <param name="deviceClasses">The device class of the IoTCore.</param>
        public IoTInfo(string name, string uid, string version, IEnumerable<CatalogInfo> catalogs, IEnumerable<ComponentInfo> components, IEnumerable<ServerInfo> servers, IEnumerable<string> deviceClasses)
        {
            Name = name;
            Uid = uid;
            Version = version;
            Catalogs = catalogs?.ToList();
            Components = components?.ToList();
            Servers = servers?.ToList();
            DeviceClasses = deviceClasses?.ToList();
        }
    }

    /// <summary>
    /// Represents a security info for a IDeviceElement.GetIdentity service call.
    /// </summary>
    public class SecurityInfo
    {
        /// <summary>
        /// The security mode.
        /// </summary>
        [VariantProperty("securitymode", IgnoredIfNull = true)]
        public string Mode { get; set; }

        /// <summary>
        /// The authentication scheme.
        /// </summary>
        [VariantProperty("authscheme", IgnoredIfNull = true)]
        public string AuthenticationScheme { get; set; }

        /// <summary>
        /// If a password is set true; otherwise false.
        /// </summary>
        [VariantProperty("ispasswdset", IgnoredIfNull = true)]
        public string IsPasswordSet { get; set; }

        /// <summary>
        /// The password status.
        /// </summary>
        [VariantProperty("passwordstatus", IgnoredIfNull = true)]
        public string PasswordStatus { get; set; }

        /// <summary>
        /// Describes which communication interface is currently used.
        /// </summary>
        [VariantProperty("activeconnection", IgnoredIfNull = true)]
        public string ActiveConnection { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter.
        /// </summary>
        [VariantConstructor]
        public SecurityInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mode">The security mode.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="isPasswordSet">If a password is set true; otherwise false.</param>
        /// <param name="passwordStatus">The password status.</param>
        /// <param name="activeConnection">Describes which communication interface is currently used.</param>
        public SecurityInfo(string mode, string authenticationScheme, string isPasswordSet, string passwordStatus, string activeConnection)
        {
            Mode = mode;
            AuthenticationScheme = authenticationScheme;
            IsPasswordSet = isPasswordSet;
            PasswordStatus = passwordStatus;
            ActiveConnection = activeConnection;
        }
    }

    /// <summary>
    /// The IoT identity.
    /// </summary>
    [VariantProperty("iot", Required = true)]
    public IoTInfo IoT { get; set; }

    /// <summary>
    /// The security identity.
    /// </summary>
    [VariantProperty("security", IgnoredIfNull = true)]
    public SecurityInfo Security { get; set; }

    /// <summary>
    /// The parameterless constructor for the variant converter.
    /// </summary>
    [VariantConstructor]
    public GetIdentityResponseServiceData()
    {
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="ioTInfo">The IoT info.</param>
    /// <param name="securityInfo">The security info.</param>
    public GetIdentityResponseServiceData(IoTInfo ioTInfo, SecurityInfo securityInfo)
    {
        IoT = ioTInfo;
        Security = securityInfo;
    }
}