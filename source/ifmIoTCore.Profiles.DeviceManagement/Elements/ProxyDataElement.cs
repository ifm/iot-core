namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using System;
    using System.Collections.Generic;
    using Common.Variant;
    using Exceptions;
    using Messages;
    using NetAdapter;
    using Utilities;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;

    internal class ProxyDataElement : DataElement<Variant>, IProxyElement
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        public RemoteContext RemoteContext { get; }

        public ProxyDataElement(IClientNetAdapterManager clientNetAdapterManager,
            RemoteContext remoteContext,
            TimeSpan? cacheTimeout,
            string identifier,
            Format format,
            List<string> profiles,
            string uid) :
            base(identifier, null, null, false, null, cacheTimeout, format, profiles, uid, false)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
            RemoteContext = remoteContext;

            GetDataFunc = GetData;
            SetDataFunc = SetData;
        }

        private Variant GetData(IDataElement _)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.GetData), 
                null, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
            return Variant.ToObject<GetDataResponseServiceData>(response.Data).Value;
        }

        public void SetData(IDataElement _, Variant data)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                1, 
                Helpers.CreateAddress(RemoteContext.Address, Identifiers.SetData),
                Variant.FromObject(new SetDataRequestServiceData(data)), 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
        }
    }
}
