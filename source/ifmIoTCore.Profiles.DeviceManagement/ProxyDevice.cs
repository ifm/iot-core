using System.Net;
using ifmIoTCore.NetAdapter;

namespace ifmIoTCore.Profiles.DeviceManagement
{
    using System;
    using System.Collections.Generic;
    using Elements;
    using Elements.ServiceData.Requests;
    using Elements.ServiceData.Responses;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json.Linq;
    using Utilities;

    //internal class RemoteContext
    //{
    //    public Uri Uri;
    //    public string Address;
    //    public AuthenticationInfo AuthenticationInfo;
    //}

    internal class ProxyDevice
    {
        private readonly IElementManager _elementManager;
        private readonly IMessageSender _messageSender;
        private readonly IServerNetAdapterManager _serverNetAdapterManager;

        private readonly IBaseElement _parentElement;
        private readonly Uri _uri;
        private readonly int? _cacheTimeout;
        private readonly AuthenticationInfo _authenticationInfo;

        public string Uri;
        public string CallbackUri;
        public string Alias;
        public IBaseElement RootElement;
        
        public ProxyDevice(IElementManager elementManager,
            IMessageSender messageSender,
            IServerNetAdapterManager serverNetAdapterManager,
            IBaseElement parentElement, 
            string uri,
            string callbackUri,
            string alias, 
            int? cacheTimeout,
            AuthenticationInfo authenticationInfo)
        {
            _elementManager = elementManager;
            _messageSender = messageSender;
            _serverNetAdapterManager = serverNetAdapterManager;
            _parentElement = parentElement;
            _uri = new Uri(uri);
            Uri = uri;
            CallbackUri = callbackUri;
            Alias = alias;
            _cacheTimeout = cacheTimeout;
            _authenticationInfo = authenticationInfo;
        }

        public void CreateElements(GetTreeResponseServiceData remoteTree)
        {
            // Create a tree response element for the root of the tree 
            var remoteElement = new GetTreeResponseServiceData.Element
            {
                Type = remoteTree.Type,
                Identifier = Alias ?? remoteTree.Identifier,
                Profiles = remoteTree.Profiles,
                Subs = remoteTree.Subs
            };

            RootElement = CreateElement(_parentElement, remoteElement);


            // ToDo: Create treechanged handler service and subscribe to remote tree changed event

            _elementManager.RaiseTreeChanged();
        }

        private IBaseElement CreateElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            IBaseElement element = null;
            switch (remoteElement.Type)
            {
                case Identifiers.Device:
                    element = CreateDeviceElement(parentElement, remoteElement);
                    break;
                case Identifiers.Structure:
                    element = CreateStructureElement(parentElement, remoteElement);
                    break;
                case Identifiers.Service:
                    element = CreateServiceElement(parentElement, remoteElement);
                    break;
                case Identifiers.Event:
                    element = CreateEventElement(parentElement, remoteElement);
                    break;
                case Identifiers.Data:
                    element = CreateDataElement(parentElement, remoteElement);
                    break;
            }

            if (element != null && remoteElement.Subs != null)
            {
                foreach (var item in remoteElement.Subs)
                {
                    CreateElement(element, item);
                }
            }
            return element;
        }

        private IBaseElement CreateDeviceElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            // Remove all standard child elements, because they are created along with the element
            remoteElement.Subs.RemoveAll(x => x.Identifier.Equals(Identifiers.GetIdentity, StringComparison.OrdinalIgnoreCase) ||
                                              x.Identifier.Equals(Identifiers.GetTree, StringComparison.OrdinalIgnoreCase) ||
                                              x.Identifier.Equals(Identifiers.QueryTree, StringComparison.OrdinalIgnoreCase) ||
                                              x.Identifier.Equals(Identifiers.GetDataMulti, StringComparison.OrdinalIgnoreCase) ||
                                              x.Identifier.Equals(Identifiers.SetDataMulti, StringComparison.OrdinalIgnoreCase) ||
                                              x.Identifier.Equals(Identifiers.GetSubscriberList, StringComparison.OrdinalIgnoreCase));

            return _elementManager.CreateDeviceElement(parentElement,
                remoteElement.Identifier,
                GetIdentityFunc,
                GetTreeFunc,
                QueryTreeFunc,
                GetDataMultiFunc,
                SetDataMultiFunc,
                GetSubscriberListFunc,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId,
                false,
                Helpers.CreateAddress((string)parentElement.Context, remoteElement.Identifier));
        }

        private IBaseElement CreateStructureElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            return _elementManager.CreateStructureElement(parentElement,
                remoteElement.Identifier,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId,
                false,
                Helpers.CreateAddress((string)parentElement.Context, remoteElement.Identifier));
        }

        private IBaseElement CreateServiceElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            return _elementManager.CreateServiceElement<JToken, JToken>(parentElement,
                remoteElement.Identifier,
                InvokeServiceFunc,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId,
                false,
                Helpers.CreateAddress((string)parentElement.Context, remoteElement.Identifier));
        }

        private IBaseElement CreateEventElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            remoteElement.Subs.Remove(x => x.Identifier == Identifiers.Subscribe);
            remoteElement.Subs.Remove(x => x.Identifier == Identifiers.Unsubscribe);

            var element = _elementManager.CreateEventElement(parentElement,
                remoteElement.Identifier,
                PreSubscribeFunc,
                PostUnsubscribeFunc,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId,
                false,
                Helpers.CreateAddress((string)parentElement.Context, remoteElement.Identifier));

            _elementManager.CreateActionServiceElement(element,
                Identifiers.CallBackTrigger,
                (sender, cid) =>
                {
                    element.RaiseEvent();
                },
                isHidden: true,
                context: Helpers.CreateAddress((string)element.Context, Identifiers.CallBackTrigger));

            return element;
        }

        private IBaseElement CreateDataElement(IBaseElement parentElement,
            GetTreeResponseServiceData.Element remoteElement)
        {
            var createGetDataServiceElement = remoteElement.Subs.Remove(x => x.Identifier == Identifiers.GetData);
            var createSetDataServiceElement = remoteElement.Subs.Remove(x => x.Identifier == Identifiers.SetData);

            var element = _elementManager.CreateDataElement<JToken>(parentElement,
                remoteElement.Identifier,
                GetDataFunc,
                SetDataFunc,
                createGetDataServiceElement,
                createSetDataServiceElement,
                null,
                _cacheTimeout != null ? TimeSpan.FromMilliseconds(_cacheTimeout.Value) : default,
                remoteElement.Format,
                remoteElement.Profiles,
                remoteElement.UId,
                false,
                Helpers.CreateAddress((string)parentElement.Context, remoteElement.Identifier));

            return element;
        }

        public void RemoveElements()
        {
            // Usubscribe from all remote event elements

            _elementManager.RemoveElement(_parentElement, RootElement);
        }


        private GetIdentityResponseServiceData GetIdentityFunc(IDeviceElement element)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(0,
                    Helpers.CreateAddress((string)element.Context, Identifiers.GetIdentity),
                    null,
                    null,
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }

            return Helpers.FromJson<GetIdentityResponseServiceData>(response.Data);
        }

        private GetTreeResponseServiceData GetTreeFunc(IDeviceElement element, GetTreeRequestServiceData data)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(0, 
                    Helpers.CreateAddress((string)element.Context, Identifiers.GetTree), 
                    Helpers.ToJson(data), 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
            return Helpers.FromJson<GetTreeResponseServiceData>(response.Data);
        }

        private QueryTreeResponseServiceData QueryTreeFunc(IDeviceElement element, QueryTreeRequestServiceData data)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(0, 
                    Helpers.CreateAddress((string)element.Context, Identifiers.QueryTree), 
                    Helpers.ToJson(data), 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
            return Helpers.FromJson<QueryTreeResponseServiceData>(response.Data);
        }

        private GetDataMultiResponseServiceData GetDataMultiFunc(IDeviceElement element, GetDataMultiRequestServiceData data)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(0, 
                    Helpers.CreateAddress((string)element.Context, Identifiers.GetDataMulti), 
                    Helpers.ToJson(data), 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
            return Helpers.FromJson<GetDataMultiResponseServiceData>(response.Data);
        }

        private void SetDataMultiFunc(IDeviceElement element, SetDataMultiRequestServiceData data)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(0, 
                    Helpers.CreateAddress((string)element.Context, Identifiers.GetDataMulti), 
                    Helpers.ToJson(data), 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
        }

        private GetSubscriberListResponseServiceData GetSubscriberListFunc(IDeviceElement element, GetSubscriberListRequestServiceData data)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(0, 
                    Helpers.CreateAddress((string)element.Context, Identifiers.GetDataMulti), 
                    Helpers.ToJson(data), 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
            return Helpers.FromJson<GetSubscriberListResponseServiceData>(response.Data);
        }

        // Service functions
        private JToken InvokeServiceFunc(IServiceElement element, JToken data, int? cid)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(cid ?? 0,
                    (string)element.Context, 
                    data, 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
            return response.Data;
        }

        // Data element functions
        private JToken GetDataFunc(IDataElement<JToken> element)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(1,
                    Helpers.CreateAddress((string)element.Context, Identifiers.GetData), 
                    null, 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data?.ToString());
            }

            return Helpers.FromJson<GetDataResponseServiceData>(response.Data).GetValue<JToken>();
        }

        private void SetDataFunc(IDataElement<JToken> element, JToken value)
        {
            var response = _messageSender.SendRequest(_uri,
                new RequestMessage(1,
                    Helpers.CreateAddress((string)element.Context, Identifiers.SetData), 
                    Helpers.ToJson(new SetDataRequestServiceData(value)), 
                    null, 
                    _authenticationInfo));
            if (ResponseCodes.IsError(response.Code))
            {
                throw new ServiceException(response.Code, response.Data.ToString());
            }
        }

        // Event functions
        private void PreSubscribeFunc(IEventElement element, SubscribeRequestServiceData data, int? cid = null)
        {
            if (element.Subscriptions.Count == 0)
            {
                var callbackUri = GetCallbackUri();
                
                string query = null;
                if (!string.IsNullOrWhiteSpace(data.Callback))
                {
                    query = data.Callback.RightIncludeSeparator('?');
                }

                var uriBuilder = new UriBuilder(callbackUri.Scheme,
                    callbackUri.Host,
                    callbackUri.Port,
                    $"{element.Address}/{Identifiers.CallBackTrigger}",
                    query);
                var remoteData = new SubscribeRequestServiceData(uriBuilder.Uri.AbsoluteUri,
                    new List<string> { "" },
                    data.SubscriptionId,
                    data.Persist);

                try
                {
                    var response = _messageSender.SendRequest(_uri,
                        new RequestMessage(cid ?? 0,
                            Helpers.CreateAddress((string)element.Context, Identifiers.Subscribe),
                            Helpers.ToJson(remoteData),
                            null, _authenticationInfo));
                    if (ResponseCodes.IsError(response.Code))
                    {
                        throw new ServiceException(response.Code, response.Data.ToString());
                    }
                }
                catch (Exception e)
                {
                    throw new ServiceException(ResponseCodes.InternalError, e.Message);
                }
            }
        }
        
        private void PostUnsubscribeFunc(IEventElement element, UnsubscribeRequestServiceData data, int? cid = null)
        {
            if (element.Subscriptions.Count == 0)
            {
                var callbackUri = GetCallbackUri();

                var uriBuilder = new UriBuilder(callbackUri.Scheme,
                    callbackUri.Host,
                    callbackUri.Port,
                    $"{element.Address}/{Identifiers.CallBackTrigger}");

                var remoteData = new UnsubscribeRequestServiceData(uriBuilder.Uri.ToString(),
                    data.SubscriptionId,
                    data.Persist);

                try
                {
                    var response = _messageSender.SendRequest(_uri,
                        new RequestMessage(cid ?? 0,
                            Helpers.CreateAddress((string)element.Context, Identifiers.Unsubscribe),
                            Helpers.ToJson(remoteData),
                            null,
                            _authenticationInfo));
                    if (ResponseCodes.IsError(response.Code))
                    {
                        throw new ServiceException(response.Code, response.Data.ToString());
                    }
                }
                catch (Exception e)
                {
                    throw new ServiceException(ResponseCodes.InternalError, e.Message);
                }
                
            }
        }

        private Uri GetCallbackUri()
        {
            if (CallbackUri != null)
            {
                return new Uri(CallbackUri);
            }

            var callBackNetAdapterServer = _serverNetAdapterManager.FindReverseServerNetAdapter(_uri);
            if (callBackNetAdapterServer != null)
            {
                return callBackNetAdapterServer.Uri;
            }
            else
            {
                throw new ServiceException(ResponseCodes.InternalError, "No callback network adapter available");
            }
        }
    }
}
