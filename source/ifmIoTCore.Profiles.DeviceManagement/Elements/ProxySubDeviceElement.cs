namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using Messages;
    using NetAdapter;
    using Utilities;
    using Common;
    using Common.Variant;

    internal class ProxySubDeviceElement : SubDeviceElement, IProxyElement
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        public RemoteContext RemoteContext { get; }

        public ProxySubDeviceElement(IClientNetAdapterManager clientNetAdapterManager,
            RemoteContext remoteContext,
            string identifier,
            Format format,
            List<string> profiles,
            string uid) :
            base(identifier, format, profiles, uid)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
            RemoteContext = remoteContext;
        }

        public override GetIdentityResponseServiceData GetIdentity()
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.GetIdentity), 
                null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
            return Variant.ToObject<GetIdentityResponseServiceData>(response.Data);
        }
    }
}
