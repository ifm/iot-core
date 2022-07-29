namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using System.Collections.Generic;
    using Common.Variant;
    using Exceptions;
    using ifmIoTCore.Elements;
    using Messages;
    using NetAdapter;

    internal class ProxyServiceElement : ServiceElement, IProxyElement
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        public RemoteContext RemoteContext { get; }

        public ProxyServiceElement(IClientNetAdapterManager clientNetAdapterManager,
            RemoteContext remoteContext,
            string identifier,
            Format format,
            List<string> profiles,
            string uid) :
            base(identifier, null, format, profiles, uid, false)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
            RemoteContext = remoteContext;

            Func = InvokeService;
        }

        public Variant InvokeService(IServiceElement _, Variant data, int? cid)
        {
            var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
            var response = client.SendRequest(new Message(RequestCodes.Request, 
                cid ?? 0, 
                RemoteContext.Address, 
                data, 
                null, 
                RemoteContext.AuthenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new IoTCoreException(response.Code, response.Data?.ToString());
            }
            return response.Data;
        }
    }
}
