using ifmIoTCore.Utilities;

namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Messages;
    using Newtonsoft.Json.Linq;
    using Resources;
    using ServiceData;
    using ServiceData.Requests;
    using ServiceData.Responses;

    internal class RootElement : BaseElement, IDeviceElement
    {
        private readonly IElementManager _elementManager;
        private readonly IMessageSender _messageSender;
        private readonly Func<IEnumerable<GetIdentityResponseServiceData.ServerInfo>> _serverInfoProvider;
        private readonly Func<string> _versionProvider;
        private readonly ILogger _logger;
        
        public readonly IEventElement TreeChangedEventElement;

        public RootElement(string identifier,
            IElementManager elementManager,
            IMessageSender messageSender,
            Func<IEnumerable<GetIdentityResponseServiceData.ServerInfo>> serverInfoProvider,
            Func<string> versionProvider,
            ILogger logger) :
            base(null, Identifiers.Device, identifier)
        {
            _elementManager = elementManager;
            _messageSender = messageSender;
            _serverInfoProvider = serverInfoProvider;
            _versionProvider = versionProvider;
            _logger = logger;

            IGetterServiceElement<GetIdentityResponseServiceData> getIdentityServiceElement = null;
            IServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeServiceElement = null;
            IServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeServiceElement = null;
            IServiceElement<GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiServiceElement = null;
            ISetterServiceElement<SetDataMultiRequestServiceData> setDataMultiServiceElement = null;
            IServiceElement<GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListServiceElement = null;
            IStructureElement remoteStructureElement = null;

            try
            {
                getIdentityServiceElement = CreateGetIdentityServiceElement();
                getTreeServiceElement = CreateGetTreeServiceElement();
                queryTreeServiceElement = CreateQueryTreeServiceElement();
                getDataMultiServiceElement = CreateGetDataMultiServiceElement();
                setDataMultiServiceElement = CreateSetDataMultiServiceElement();
                getSubscriberListServiceElement = CreateGetSubscriberListServiceElement();
                remoteStructureElement = CreateRemoteStructureElement();
                TreeChangedEventElement = CreateTreeChangedEventElement();
            }
            catch
            {
                // ToDo: Check if this is really required
                // Creating elements does not fail, except in low memory condition?
                // Does GC collect the elements, even if they have a reference to this (which is not constructed)?
                // Does GC collect the elements, even if they have a reference to itself in the references?

                getIdentityServiceElement?.Dispose();
                getTreeServiceElement?.Dispose();
                queryTreeServiceElement?.Dispose();
                getDataMultiServiceElement?.Dispose();
                setDataMultiServiceElement?.Dispose();
                getSubscriberListServiceElement?.Dispose();
                remoteStructureElement?.Dispose();
                TreeChangedEventElement?.Dispose();

                throw;
            }
        }

        private IGetterServiceElement<GetIdentityResponseServiceData> CreateGetIdentityServiceElement()
        {
            var getIdentityServiceElement = new GetterServiceElement<GetIdentityResponseServiceData>(this, Identifiers.GetIdentity, GetIdentityServiceFunc);
            References.AddForwardReference(this, getIdentityServiceElement, getIdentityServiceElement.Identifier, ReferenceType.Child);
            return getIdentityServiceElement;
        }

        private IServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData> CreateGetTreeServiceElement()
        {
            var getTreeServiceElement = new ServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData>(this, Identifiers.GetTree, GetTreeServiceFunc);
            References.AddForwardReference(this, getTreeServiceElement, getTreeServiceElement.Identifier, ReferenceType.Child);
            return getTreeServiceElement;
        }

        private IServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData> CreateQueryTreeServiceElement()
        {
            var queryTreeServiceElement = new ServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData>(this, Identifiers.QueryTree, QueryTreeServiceFunc);
            References.AddForwardReference(this, queryTreeServiceElement, queryTreeServiceElement.Identifier, ReferenceType.Child);
            return queryTreeServiceElement;
        }

        private IServiceElement<GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> CreateGetDataMultiServiceElement()
        {
            var getDataMultiServiceElement = new ServiceElement<GetDataMultiRequestServiceData, GetDataMultiResponseServiceData>(this, Identifiers.GetDataMulti, GetDataMultiServiceFunc);
            References.AddForwardReference(this, getDataMultiServiceElement, getDataMultiServiceElement.Identifier, ReferenceType.Child);
            return getDataMultiServiceElement;
        }

        private ISetterServiceElement<SetDataMultiRequestServiceData> CreateSetDataMultiServiceElement()
        {
            var setDataMultiServiceElement = new SetterServiceElement<SetDataMultiRequestServiceData>(this, Identifiers.SetDataMulti, SetDataMultiServiceFunc);
            References.AddForwardReference(this, setDataMultiServiceElement, setDataMultiServiceElement.Identifier, ReferenceType.Child);
            return setDataMultiServiceElement;
        }

        private IServiceElement<GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> CreateGetSubscriberListServiceElement()
        {
            var getSubscriberListServiceElement = new ServiceElement<GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData>(this, Identifiers.GetSubscriberList, GetSubscriberListServiceFunc);
            References.AddForwardReference(this, getSubscriberListServiceElement, getSubscriberListServiceElement.Identifier, ReferenceType.Child);
            return getSubscriberListServiceElement;
        }

        private IStructureElement CreateRemoteStructureElement()
        {
            var remoteStructureElement = new StructureElement(this, Identifiers.Remote);
            References.AddForwardReference(this, remoteStructureElement, remoteStructureElement.Identifier, ReferenceType.Child);
            return remoteStructureElement;
        }

        private IEventElement CreateTreeChangedEventElement()
        {
            var treeChangedEventElement = new EventElement(this._elementManager, this._messageSender, this._logger, this, Identifiers.TreeChanged);
            References.AddForwardReference(this, treeChangedEventElement, treeChangedEventElement.Identifier, ReferenceType.Child);
            return treeChangedEventElement;
        }

        private GetIdentityResponseServiceData GetIdentityServiceFunc(IServiceElement _, int? cid)
        {
            return GetIdentity();
        }

        public GetIdentityResponseServiceData GetIdentity()
        {
            var servers = _serverInfoProvider().ToList();
            var deviceInfo = new GetIdentityResponseServiceData.DeviceInfo(null, null, _versionProvider());
            var ioTInfo = new GetIdentityResponseServiceData.IoTInfo(Identifier, _versionProvider(), servers, null, null, null);
            return new GetIdentityResponseServiceData(deviceInfo, ioTInfo, null);
        }

        private GetTreeResponseServiceData GetTreeServiceFunc(IServiceElement _, GetTreeRequestServiceData data, int? cid)
        {
            return GetTree(data);
        }

        public GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data)
        {
            var element = string.IsNullOrEmpty(data?.Address) ? this : GetElementByAddress(data.Address);
            if (element == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data?.Address));
            }

            if (data != null && data.ExpandConstValues && element is IDataElement dataElement && element.HasProfile("const_value"))
            {
                JToken value = null;
                try
                {
                    value = dataElement.Value;
                }
                catch (Exception)
                {
                    // Ignore
                    // ToDo: Use logger
                }

                return new GetTreeResponseServiceData(element.Type, element.Identifier, element.Address, element.ForwardReferences, element.Profiles, data?.Level, data?.ExpandConstValues ?? false, value);
            }

            return new GetTreeResponseServiceData(element.Type, element.Identifier, element.Address, element.ForwardReferences, element.Profiles, data?.Level, data?.ExpandConstValues ?? false);
        }

        private QueryTreeResponseServiceData QueryTreeServiceFunc(IServiceElement _, QueryTreeRequestServiceData data, int? cid)
        {
            return QueryTree(data);
        }

        public QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data)
        {
            var predicateProfile = string.IsNullOrEmpty(data?.Profile) ? x => true : new Predicate<IBaseElement>(x => x.HasProfile(data.Profile));
            var predicateType = string.IsNullOrEmpty(data?.Type) ? x => true : new Predicate<IBaseElement>(x => x.Type.Equals(data.Type));
            var predicateName = string.IsNullOrEmpty(data?.Name) ? x => true : new Predicate<IBaseElement>(x => x.Identifier.Equals(data.Name));

            // ToDo: Use element manager
            var result = GetElementsByPredicate(x => predicateProfile(x) && predicateType(x) && predicateName(x));
            return new QueryTreeResponseServiceData(result.Select(x => x.Address));
        }

        private GetDataMultiResponseServiceData GetDataMultiServiceFunc(IServiceElement _, GetDataMultiRequestServiceData data, int? cid)
        {
            return GetDataMulti(data);
        }

        public GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, Identifiers.GetDataMulti));
            }

            if (data.DataToSend == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ParameterNotSpecified, Identifiers.DataToSend));
            }

            var response = new GetDataMultiResponseServiceData();
            foreach (var address in data.DataToSend)
            {
                var element = _elementManager.GetElementByAddress(address);
                if (element == null)
                {
                    response.Add(address, new CodeDataPair(ResponseCodes.NotFound, null));
                }
                else if (element is IDataElement dataElement)
                {
                    try
                    {
                        response.Add(address, new CodeDataPair(ResponseCodes.Success, dataElement.Value));
                    }
                    catch (IoTCoreException iotCoreException)
                    {
                        response.Add(address, new CodeDataPair(iotCoreException.Code, null));
                    }
                    catch
                    {
                        response.Add(address, new CodeDataPair(ResponseCodes.InternalError, null));
                    }
                }
                else
                {
                    response.Add(address, new CodeDataPair(ResponseCodes.BadRequest, null));
                }
            }
            return response;
        }

        private void SetDataMultiServiceFunc(IServiceElement _, SetDataMultiRequestServiceData data, int? cid)
        {
            SetDataMulti(data);
        }

        public void SetDataMulti(SetDataMultiRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, Identifiers.SetDataMulti));
            }
            if (data.DataToSend == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ParameterNotSpecified, Identifiers.DataToSend));
            }

            foreach (var item in data.DataToSend)
            {
                var element = GetElementByAddress(item.Key);
                if (element is IDataElement dataElement)
                {
                    try
                    {
                        dataElement.Value = item.Value;
                    }
                    catch (Exception)
                    {
                        // Ignore
                        // ToDo: Add logging
                    }
                }
            }
        }

        private GetSubscriberListResponseServiceData GetSubscriberListServiceFunc(IServiceElement _, GetSubscriberListRequestServiceData data, int? cid)
        {
            return GetSubscriberList(data);
        }

        public GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data)
        {
            var response = new GetSubscriberListResponseServiceData();
            if (string.IsNullOrEmpty(data?.Address))
            {
                // ToDo: Use element manager
                var elements = GetElementsByType(Identifiers.Event);
                foreach (var element in elements)
                {
                    var eventElement = (IEventElement)element;
                    if (!eventElement.SubscriptionsLock.TryEnterReadLock(Configuration.Settings.Timeout))
                    {
                        throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                    }
                    try
                    {
                        foreach (var item in eventElement.Subscriptions)
                        {
                            response.Add(eventElement.Address, item.Value.Callback ?? "local", item.Value.DataToSend, item.Value.Persist, item.Value.SubscriptionId);
                        }
                    }
                    finally
                    {
                        eventElement.SubscriptionsLock.ExitReadLock();
                    }
                }
            }
            else
            {
                var element = GetElementByAddress(data.Address);
                if (element == null)
                {
                    throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data.Address));
                }

                if (element is IEventElement eventElement)
                {
                    if (!eventElement.SubscriptionsLock.TryEnterReadLock(Configuration.Settings.Timeout))
                    {
                        throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                    }
                    try
                    {
                        foreach (var item in eventElement.Subscriptions)
                        {
                            response.Add(eventElement.Address, item.Value.Callback, item.Value.DataToSend, item.Value.Persist, item.Value.SubscriptionId);
                        }
                    }
                    finally
                    {
                        eventElement.SubscriptionsLock.ExitReadLock();
                    }
                }
                else
                {
                    throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.ElementNotService, data.Address));
                }
            }
            return response;
        }
    }
}
