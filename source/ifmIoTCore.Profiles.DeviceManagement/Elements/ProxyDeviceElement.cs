namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Variant;
    using Exceptions;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using Messages;
    using NetAdapter;
    using Utilities;

    internal class ProxyDeviceElement : DeviceElement, IProxyElement
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        public RemoteContext RemoteContext { get; }

        public ProxyDeviceElement(IClientNetAdapterManager clientNetAdapterManager,
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

        public override GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.GetTree),
                data != null ? Variant.FromObject(data) : null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }

            return Variant.ToObject<GetTreeResponseServiceData>(response.Data);
        }

        public override QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.QueryTree),
                data != null ? Variant.FromObject(data) : null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
            return Variant.ToObject<QueryTreeResponseServiceData>(response.Data);
        }

        public override GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.GetDataMulti),
                data != null ? Variant.FromObject(data) : null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
            return Variant.ToObject<GetDataMultiResponseServiceData>(response.Data);
        }

        public override void SetDataMulti(SetDataMultiRequestServiceData data)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.SetDataMulti),
                data != null ? Variant.FromObject(data) : null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
        }

        public override GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.GetSubscriberList),
                data != null ? Variant.FromObject(data) : null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
            return Variant.ToObject<GetSubscriberListResponseServiceData>(response.Data);
        }
    }
}
