namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections.Generic;
    using Common.Variant;

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
            [VariantProperty("serialnumber", IgnoredIfNull = true)]
            public string SerialNumber { get; set; }

            /// <summary>
            /// The hardware revision of the device
            /// </summary>
            [VariantProperty("hwrevision", IgnoredIfNull = true)]
            public string HardwareRevision { get; set; }

            /// <summary>
            /// The software revision of the device
            /// </summary>
            [VariantProperty("swrevision", IgnoredIfNull = true)]
            public string SoftwareRevision { get; set; }

            /// <summary>
            /// The parameterless constructor for the variant converter
            /// </summary>
            [VariantConstructor]
            public DeviceInfo()
            {
            }

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
            [VariantProperty("name", Required = true)]
            public string Name { get; set; }

            /// <summary>
            /// The version of the catalogue
            /// </summary>
            [VariantProperty("version", Required = true)]
            public string Version { get; set; }

            /// <summary>
            /// The parameterless constructor for the variant converter
            /// </summary>
            [VariantConstructor]
            public CatalogInfo()
            {
            }

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
            [VariantProperty("type", Required = true)]
            public string Type { get; set; }

            /// <summary>
            /// The endpoint of the server
            /// </summary>
            [VariantProperty("uri", Required = true)]
            public string Uri { get; set; }

            /// <summary>
            /// The list of supported data formats of the server
            /// </summary>
            [VariantProperty("formats", Required = true)]
            public List<string> Formats { get; set; }

            /// <summary>
            /// The parameterless constructor for the variant converter
            /// </summary>
            [VariantConstructor]
            public ServerInfo()
            {
            }

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
            [VariantProperty("name", Required = true)]
            public string Name { get; set; }

            /// <summary>
            /// The version of the IoTCore
            /// </summary>
            [VariantProperty("version", Required = true)]
            public string Version { get; set; }

            // Ignore, because buggy in iolinkmaster and optional anyway
            // As discussed w/ Matthieu on 11-4-2021
            ///// <summary>
            ///// The list of available network adapter servers in the IoTCore
            ///// </summary>
            //[VariantPropertyName("serverlist", IgnoredIfNull = true)]
            //public List<ServerInfo> Servers { get; set; }

            /// <summary>
            /// The unique id of the IoTCore
            /// </summary>
            [VariantProperty("uid", IgnoredIfNull = true)]
            public string UId { get; set; }

            /// <summary>
            /// The device class of the IoTCore
            /// </summary>
            [VariantProperty("deviceclass", IgnoredIfNull = true)]
            public string DeviceClass { get; set; }

            /// <summary>
            /// The supported catalogs of the IoTCore
            /// </summary>
            [VariantProperty("catalogue", IgnoredIfNull = true)]
            public List<CatalogInfo> Catalogs { get; set; }

            /// <summary>
            /// The parameterless constructor for the variant converter
            /// </summary>
            [VariantConstructor]
            public IoTInfo()
            {
            }

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
            [VariantProperty("securitymode", IgnoredIfNull = true)]
            public string Mode { get; set; }

            /// <summary>
            /// The authentication scheme
            /// </summary>
            [VariantProperty("authscheme", IgnoredIfNull = true)]
            public string AuthenticationScheme { get; set; }

            /// <summary>
            /// If a password is set true; otherwise false
            /// </summary>
            [VariantProperty("ispasswdset", IgnoredIfNull = true)]
            public string IsPasswordSet { get; set; }

            /// <summary>
            /// Describes which communication interface is currently used
            /// </summary>
            [VariantProperty("activeconnection", IgnoredIfNull = true)]
            public string ActiveConnection { get; set; }

            /// <summary>
            /// The parameterless constructor for the variant converter
            /// </summary>
            [VariantConstructor]
            public SecurityInfo()
            {
            }

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
        [VariantProperty("device", IgnoredIfNull = true)]
        public DeviceInfo Device { get; set; }

        /// <summary>
        /// The IoT identity
        /// </summary>
        [VariantProperty("iot", Required = true)]
        public IoTInfo IoT { get; set; }

        /// <summary>
        /// The security identity
        /// </summary>
        [VariantProperty("security", IgnoredIfNull = true)]
        public SecurityInfo Security { get; set; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetIdentityResponseServiceData()
        {
        }

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
