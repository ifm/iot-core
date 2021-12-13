namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.GetIdentity service call
    /// </summary>
    public class GetIdentityResponseServiceData
    {
        /// <summary>
        /// Represents a device info for a IDeviceElement.GetIdentity service call
        /// </summary>
        public class DeviceInfo
        {
            /// <summary>
            /// The serial number of the device
            /// </summary>
            [JsonProperty("serialnumber", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string SerialNumber;

            /// <summary>
            /// The hardware revision of the device
            /// </summary>
            [JsonProperty("hwrevision", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string HardwareRevision;

            /// <summary>
            /// The software revision of the device
            /// </summary>
            [JsonProperty("swrevision", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string SoftwareRevision;

            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            public DeviceInfo(string serialNumber, string hardwareRevision, string softwareRevision)
            {
                SerialNumber = serialNumber;
                HardwareRevision = hardwareRevision;
                SoftwareRevision = softwareRevision;
            }
        }

        /// <summary>
        /// Represents a server info for a IDeviceElement.GetIdentity service call
        /// </summary>
        public class CatalogInfo
        {
            /// <summary>
            /// The name of the catalogue
            /// </summary>
            [JsonProperty("name", Required = Required.Always)]
            public readonly string Name;

            /// <summary>
            /// The version of the catalogue
            /// </summary>
            [JsonProperty("version", Required = Required.Always)]
            public readonly string Version;

            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            /// <param name="name">The name of the catalogue</param>
            /// <param name="version">The version of the catalogue</param>
            public CatalogInfo(string name, string version)
            {
                Name = name;
                Version = version;
            }
        }


        /// <summary>
        /// Represents a server info for a IDeviceElement.GetIdentity service call
        /// </summary>
        public class ServerInfo
        {
            /// <summary>
            /// The type of the server
            /// </summary>
            [JsonProperty("type", Required = Required.Always)]
            public readonly string Type;

            /// <summary>
            /// The endpoint of the server
            /// </summary>
            [JsonProperty("uri", Required = Required.Always)]
            public readonly string Uri;

            /// <summary>
            /// The list of supported data formats of the server
            /// </summary>
            [JsonProperty("formats", Required = Required.Always)]
            public readonly List<string> Formats;

            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            /// <param name="type">The type of the server</param>
            /// <param name="uri">The uri of the server</param>
            /// <param name="formats">The list of supported data formats of the server</param>
            public ServerInfo(string type, string uri, List<string> formats)
            {
                Type = type;
                Uri = uri;
                Formats = formats;
            }
        }

        /// <summary>
        /// Represents an IoT info for a IDeviceElement.GetIdentity service call
        /// </summary>
        public class IoTInfo
        {
            /// <summary>
            /// The name of the IoTCore
            /// </summary>
            [JsonProperty("name", Required = Required.Always)]
            public readonly string Name;

            /// <summary>
            /// The version of the IoTCore
            /// </summary>
            [JsonProperty("version", Required = Required.Always)]
            public readonly string Version;

            // Ignore, because buggy in iolinkmaster and optional anyway
            // As discussed w/ Matthieu on 11-4-2021
            ///// <summary>
            ///// The list of available network adapter servers in the IoTCore
            ///// </summary>
            //[JsonProperty("serverlist", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            //public readonly List<ServerInfo> Servers;

            /// <summary>
            /// The unique id of the IoTCore
            /// </summary>
            [JsonProperty("uid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string UId;

            /// <summary>
            /// The device class of the IoTCore
            /// </summary>
            [JsonProperty("deviceclass", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string DeviceClass;

            /// <summary>
            /// The supported catalogs of the IoTCore
            /// </summary>
            [JsonProperty("catalogue", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly List<CatalogInfo> Catalogs;

            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            /// <param name="name">The name of the IoTCore</param>
            /// <param name="version">The version of the IoTCore</param>
            /// <param name="servers">The list of available network adapter servers in the IoTCore</param>
            /// <param name="uid">The unique id of the IoTCore</param>
            /// <param name="deviceClass">The device class of the IoTCore</param>
            /// <param name="catalogs">The supported catalogs of the IoTCore</param>
            public IoTInfo(string name, string version, List<ServerInfo> servers, string uid, string deviceClass, List<CatalogInfo> catalogs)
            {
                Name = name;
                Version = version;
                //Servers = servers;
                UId = uid;
                DeviceClass = deviceClass;
                Catalogs = catalogs;
            }
        }

        /// <summary>
        /// Represents a security info for a IDeviceElement.GetIdentity service call
        /// </summary>
        public class SecurityInfo
        {
            /// <summary>
            /// The security mode
            /// </summary>
            [JsonProperty("securitymode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string Mode;

            /// <summary>
            /// The authentication scheme
            /// </summary>
            [JsonProperty("authscheme", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string AuthenticationScheme;

            /// <summary>
            /// If a password is set true; otherwise false
            /// </summary>
            [JsonProperty("ispasswdset", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string IsPasswordSet;

            /// <summary>
            /// Describes which communication interface is currently used
            /// </summary>
            [JsonProperty("activeconnection", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public readonly string ActiveConnection;

            /// <summary>
            /// Initializes a new instance of the class
            /// </summary>
            /// <param name="mode">The security mode</param>
            /// <param name="authenticationScheme">The authentication scheme</param>
            /// <param name="isPasswordSet">If a password is set true; otherwise false</param>
            /// <param name="activeConnection">Describes which communication interface is currently used</param>
            public SecurityInfo(string mode, string authenticationScheme, string isPasswordSet, string activeConnection)
            {
                Mode = mode;
                AuthenticationScheme = authenticationScheme;
                IsPasswordSet = isPasswordSet;
                ActiveConnection = activeConnection;
            }
        }

        /// <summary>
        /// The device identity
        /// </summary>
        [JsonProperty("device", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public readonly DeviceInfo Device;

        /// <summary>
        /// The IoT identity
        /// </summary>
        [JsonProperty("iot", Required = Required.Always)]
        public readonly IoTInfo IoT;

        /// <summary>
        /// The security identity
        /// </summary>
        [JsonProperty("security", Required = Required.Default, NullValueHandling = NullValueHandling.Include)]
        public readonly SecurityInfo Security;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="deviceInfo">The device info</param>
        /// <param name="ioTInfo">The IoT info</param>
        /// <param name="securityInfo">The security info</param>
        public GetIdentityResponseServiceData(DeviceInfo deviceInfo, IoTInfo ioTInfo, SecurityInfo securityInfo)
        {
            Device = deviceInfo;
            IoT = ioTInfo;
            Security = securityInfo;
        }
    }
}
