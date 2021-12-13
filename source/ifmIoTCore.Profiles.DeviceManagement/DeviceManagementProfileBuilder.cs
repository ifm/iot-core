namespace ifmIoTCore.Profiles.DeviceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Elements;
    using Elements.ServiceData.Responses;
    using Exceptions;
    using Messages;
    using ServiceData.Requests;
    using Utilities;

    public class DeviceManagementProfileBuilder
    {
        public const string ProfileName = "device_management";

        private readonly IIoTCore _ioTCore;
        private readonly IBaseElement _remoteFolder;

        private readonly List<ProxyDevice> _devices = new List<ProxyDevice>();

        public DeviceManagementProfileBuilder(IIoTCore iotCore)
        {
            _ioTCore = iotCore;
            _remoteFolder = _ioTCore.Root.GetElementByIdentifier(Identifiers.Remote);
        }

        public void Build()
        {
            var element = GetDeviceManagementElement();

            _ioTCore.CreateSetterServiceElement<MirrorRequestServiceData>(element, "mirror", MirrorFunc);
            _ioTCore.CreateSetterServiceElement<UnmirrorRequestServiceData>(element, "unmirror", UnmirrorFunc);
            _ioTCore.RaiseTreeChanged();
        }

        private IBaseElement GetDeviceManagementElement()
        {
            var deviceManagement = _ioTCore.Root.GetElementByProfile(ProfileName);
            if (deviceManagement == null)
            {
                deviceManagement = _ioTCore.CreateStructureElement(_ioTCore.Root, ProfileName, profiles: new List<string> { ProfileName });
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

        private void MirrorFunc(IBaseElement element, MirrorRequestServiceData data, int? cid = null)
        {
            Mirror(data);
        }

        public IBaseElement Mirror(MirrorRequestServiceData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrEmpty(data.Uri)) throw new ArgumentNullException(nameof(data.Uri));

            var remoteTree = GetRemoteTree(new Uri(data.Uri), data.Authentication);

            var device = new ProxyDevice(_ioTCore, _ioTCore, _ioTCore, _remoteFolder,
                data.Uri, data.CallbackUri, data.Alias, data.CacheTimeout, data.Authentication);
            device.CreateElements(remoteTree);
            _devices.Add(device);
            return device.RootElement;
        }

        private GetTreeResponseServiceData GetRemoteTree(Uri remoteUri,
            AuthenticationInfo authenticationInfo)
        {
            var response = _ioTCore.SendRequest(remoteUri, new RequestMessage(1, $"/{Identifiers.GetTree}", null, null, authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }

            return Helpers.FromJson<GetTreeResponseServiceData>(response.Data);
        }


        private void UnmirrorFunc(IBaseElement element, UnmirrorRequestServiceData data, int? cid = null)
        {
            Unmirror(data);
        }

        public void Unmirror(UnmirrorRequestServiceData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Uri == null && data.Alias == null) throw new ServiceException(ResponseCodes.DataInvalid, "An uri or and alias must be provided");

            ProxyDevice device = null;
            if (!string.IsNullOrEmpty(data.Alias))
            {
                device = _devices.FirstOrDefault(x => x.Alias == data.Alias);
            }
            if (device == null)
            {
                if (!string.IsNullOrEmpty(data.Uri))
                {
                    device = _devices.FirstOrDefault(x => x.Uri == data.Uri);
                }
            }
            if (device == null)
            {
                throw new ServiceException(ResponseCodes.NotFound, $"No mirrored device with Uri {data.Uri} or alias {data.Alias} found");
            }
            device.RemoveElements();
            _devices.Remove(device);
        }
    }
}
