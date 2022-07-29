namespace ifmIoTCore.Profiles.DeviceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Common.Variant;
    using Exceptions;
    using ifmIoTCore.Elements;
    using Messages;
    using ServiceData.Requests;

    public class DeviceManagementProfileBuilder : BaseProfileBuilder
    {
        public const string ProfileName = "device_management";

        private readonly IBaseElement _remoteFolder;

        private readonly List<ProxyDevice> _devices = new List<ProxyDevice>();

        public DeviceManagementProfileBuilder(ProfileBuilderConfiguration config): base(config)
        {
            
            _remoteFolder = IoTCore.Root.GetElementByIdentifier(Identifiers.Remote);
        }
        
        public override void Build()
        {
            var element = GetDeviceManagementElement();

            element.AddChild(new SetterServiceElement("mirror", MirrorFunc), false);
            element.AddChild(new SetterServiceElement("unmirror", UnmirrorFunc), false);
            IoTCore.Root.RaiseTreeChanged(element);
        }

        private IBaseElement GetDeviceManagementElement()
        {
            var deviceManagement = IoTCore.Root.GetElementByProfile(ProfileName);
            if (deviceManagement == null)
            {
                deviceManagement = IoTCore.Root.AddChild(new StructureElement(ProfileName, profiles: new List<string> { ProfileName }), false);
            }
            else
            {
                if (!deviceManagement.HasProfile(ProfileName))
                {
                    deviceManagement.AddProfile(ProfileName);
                }
            }
            return deviceManagement;
        }

        private void MirrorFunc(IBaseElement element, Variant data, int? cid = null)
        {
            var request = data != null ? Variant.ToObject<MirrorRequestServiceData>(data) : null;
            Mirror(request);
        }

        public IBaseElement Mirror(MirrorRequestServiceData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrEmpty(data.RemoteUri)) throw new ArgumentNullException(nameof(data.RemoteUri));

            var device = new ProxyDevice(IoTCore.ClientNetAdapterManager, 
                data.RemoteUri, 
                data.Alias,
                data.Callback, 
                data.CacheTimeout, 
                data.AuthenticationInfo);

            device.CreateElements(_remoteFolder);

            _devices.Add(device);
            return device.RootElement;
        }

        private void UnmirrorFunc(IBaseElement element, Variant data, int? cid = null)
        {
            var request = data != null ? Variant.ToObject<UnmirrorRequestServiceData>(data) : null;
            Unmirror(request);
        }

        public void Unmirror(UnmirrorRequestServiceData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.RemoteUri == null && data.Alias == null) throw new IoTCoreException(ResponseCodes.DataInvalid, "An uri or and alias must be provided");

            ProxyDevice device = null;
            if (!string.IsNullOrEmpty(data.Alias))
            {
                device = _devices.FirstOrDefault(x => x.Alias == data.Alias);
            }
            if (device == null)
            {
                if (!string.IsNullOrEmpty(data.RemoteUri))
                {
                    device = _devices.FirstOrDefault(x => x.RemoteUri == data.RemoteUri);
                }
            }
            if (device == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, $"No mirrored device with Uri {data.RemoteUri} or alias {data.Alias} found");
            }
            device.RemoveElements(_remoteFolder);
            _devices.Remove(device);
        }
    }
}
