namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using System.Collections.Generic;
    using ifmIoTCore.Elements;

    internal class ProxyStructureElement : StructureElement, IProxyElement
    {
        public RemoteContext RemoteContext { get; }

        public ProxyStructureElement(RemoteContext remoteContext,
            string identifier,
            Format format,
            List<string> profiles,
            string uid) :
            base(identifier, format, profiles, uid)
        {
            RemoteContext = remoteContext;
        }
    }
}
