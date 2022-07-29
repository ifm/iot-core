namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using Exceptions;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using Messages;
    using NetAdapter;
    using System;
    using System.Collections.Generic;
    using Common.Variant;
    using Utilities;

    internal class ProxyEventElement : EventElement, IProxyElement
    {
        public const int DefaultCid = 1;

        private readonly IClientNetAdapterManager _clientNetAdapterManager;

        public RemoteContext RemoteContext { get; }

        public ProxyEventElement(IClientNetAdapterManager clientNetAdapterManager,
            RemoteContext remoteContext,
            string identifier,
            Format format,
            List<string> profiles,
            string uid) :
            base(identifier, format, profiles, uid)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
            RemoteContext = remoteContext;

            AddChild( new SetterServiceElement(Identifiers.CallBackTrigger, CallbackTriggerFunc, isHidden: true), false);
        }
        
        public override SubscribeResponseServiceData Subscribe(SubscribeRequestServiceData data, int? cid)
        {
            // Subscribe to remote event
            // If subscribe to remote event fails subscription cannot be added, because it will never be triggered
            if (SubscriptionCount == 0)
            {
                var remoteData = new SubscribeRequestServiceData(CreateCallbackUri(RemoteContext.Callback, Helpers.CreateAddress(Address, Identifiers.CallBackTrigger)),
                    new List<string> { "" },
                    data.SubscriptionId,
                    data.Persist);

                try
                {
                    var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
                    var response = client.SendRequest(new Message(RequestCodes.Request, 
                            cid ?? DefaultCid,
                            Helpers.CreateAddress(RemoteContext.Address, Identifiers.Subscribe),
                            Variant.FromObject(remoteData),
                            null, 
                            RemoteContext.AuthenticationInfo));
                    if (ResponseCodes.IsError(response.Code))
                    {
                        throw new IoTCoreException(response.Code, response.Data?.ToString());
                    }
                }
                catch (Exception e)
                {
                    throw new IoTCoreException(ResponseCodes.InternalError, e.Message);
                }
            }

            // Add subscription
            return base.Subscribe(data, cid);
        }

        public override void Subscribe(Action<IEventElement> callbackFunc)
        {
            // Subscribe to remote event
            // If subscribe to remote event fails subscription cannot be added, because it will never be triggered
            if (SubscriptionCount == 0)
            {
                var remoteData = new SubscribeRequestServiceData(CreateCallbackUri(RemoteContext.Callback, Helpers.CreateAddress(Address, Identifiers.CallBackTrigger)),
                    new List<string> { "" },
                    -1);

                try
                {
                    var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
                    var response = client.SendRequest(new Message(RequestCodes.Request,
                            DefaultCid,
                            Helpers.CreateAddress(RemoteContext.Address, Identifiers.Subscribe),
                            Variant.FromObject(remoteData),
                            null, 
                            RemoteContext.AuthenticationInfo));
                    if (ResponseCodes.IsError(response.Code))
                    {
                        throw new IoTCoreException(response.Code, response.Data?.ToString());
                    }
                }
                catch (Exception e)
                {
                    throw new IoTCoreException(ResponseCodes.InternalError, e.Message);
                }
            }

            // Add subscription
            base.Subscribe(callbackFunc);
        }

        public override void Unsubscribe(UnsubscribeRequestServiceData data, int? cid = null)
        {
            // Unsubscribe from remote event
            // If unsubscribe from remote event fails continue
            if (SubscriptionCount == 0)
            {
                var remoteData = new UnsubscribeRequestServiceData(CreateCallbackUri(RemoteContext.Callback, Helpers.CreateAddress(Address, Identifiers.CallBackTrigger)),
                    data.SubscriptionId,
                    data.Persist);

                try
                {
                    var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
                    var response = client.SendRequest(new Message(RequestCodes.Request, 
                            cid ?? DefaultCid,
                            Helpers.CreateAddress(RemoteContext.Address, Identifiers.Unsubscribe),
                            Variant.FromObject(remoteData),
                            null,
                            RemoteContext.AuthenticationInfo));
                }
                catch
                {
                    // Ignore
                }
            }

            // Remove subscription
            base.Unsubscribe(data, cid);
        }

        public override void Unsubscribe(Action<IEventElement> callbackFunc)
        {
            // Unsubscribe from remote event
            // If unsubscribe from remote event fails continue
            if (SubscriptionCount == 0)
            {
                var remoteData = new UnsubscribeRequestServiceData(CreateCallbackUri(RemoteContext.Callback, Helpers.CreateAddress(Address, Identifiers.CallBackTrigger)),
                    -1,
                    false);

                try
                {
                    var client = _clientNetAdapterManager.CreateClientNetAdapter(RemoteContext.Uri);
                    var response = client.SendRequest(new Message(RequestCodes.Request,
                            DefaultCid,
                            Helpers.CreateAddress(RemoteContext.Address, Identifiers.Unsubscribe),
                            Variant.FromObject(remoteData),
                            null,
                            RemoteContext.AuthenticationInfo));
                }
                catch
                {
                    // Ignore
                }
            }

            // Remove subscription
            base.Unsubscribe(callbackFunc);
        }

        private static string CreateCallbackUri(string callback, string address)
        {
            callback = callback.TrimEnd('/');
            address = address.TrimStart('/');
            return $"{callback}/{address}";
        }

        private void CallbackTriggerFunc(IServiceElement serviceElement, Variant data, int? cid)
        {
            RaiseEvent();
        }
    }
}
