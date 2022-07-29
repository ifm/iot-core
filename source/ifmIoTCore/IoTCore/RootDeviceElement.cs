namespace ifmIoTCore.IoTCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common.Variant;
    using Elements;
    using Elements.ServiceData;
    using Elements.ServiceData.Requests;
    using Elements.ServiceData.Responses;
    using Exceptions;
    using Messages;
    using NetAdapter;
    using Resources;
    using Utilities;

    internal class RootDeviceElement : DeviceElement
    {
        private readonly string _version;
        private readonly IElementManager _elementManager;
        private readonly IServerNetAdapterManager _serverNetAdapterManager;

        public RootDeviceElement(string identifier,
            string version,
            IElementManager elementManager,
            IServerNetAdapterManager serverNetAdapterManager) :
            base(identifier)
        {
            _version = version;
            _elementManager = elementManager;
            _serverNetAdapterManager = serverNetAdapterManager;

            AddChild(new StructureElement(Identifiers.Remote));

            TreeChangedEventElement = new EventElement(Identifiers.TreeChanged);

            AddChild(TreeChangedEventElement);

            TreeChanged += (sender, args) =>
            {
                TreeChangedEventElement.RaiseEvent();
            };
        }

        public override GetIdentityResponseServiceData GetIdentity()
        {
            var servers = _serverNetAdapterManager.ServerNetAdapters.Select(x => new GetIdentityResponseServiceData.ServerInfo(x.Scheme, x.Uri.ToString(), new List<string> { x.Format })).ToList();
            var deviceInfo = new GetIdentityResponseServiceData.DeviceInfo(null, null, _version);
            var ioTInfo = new GetIdentityResponseServiceData.IoTInfo(Identifier, _version, servers, null, null, null);
            return new GetIdentityResponseServiceData(deviceInfo, ioTInfo, null);
        }

        public override GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data)
        {
            var element = string.IsNullOrEmpty(data?.Address) ? _elementManager.Root : _elementManager.GetElementByAddress(data.Address);
            if (element == null)
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, data?.Address));
            }

            if (data != null && data.ExpandConstValues && element is IDataElement && element.HasProfile("const_value"))
            {
                Variant value = null;
                try
                {
                    value = GetDataElementValue(element);
                }
                catch (Exception)
                {
                    // Ignore
                    // ToDo: Use logger
                }

                return new GetTreeResponseServiceData(element.Type, element.Identifier, element.Address, element.Format, element.Profiles, element.UId, element.ForwardReferences, data?.Level, data?.ExpandConstValues ?? false,data?.ExpandLinks ?? false, value);
            }

            return new GetTreeResponseServiceData(element.Type, element.Identifier, element.Address, element.Format, element.Profiles, element.UId, element.ForwardReferences, data?.Level, data?.ExpandConstValues ?? false, data?.ExpandLinks ?? false);
        }

        private static Variant GetDataElementValue(IBaseElement element)
        {
            Variant result = null;

            if (element is IDataElement dataElement)
            {
                result = dataElement.Value;
            }

            return result;
        }

        public override QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data)
        {
            var predicateProfile = string.IsNullOrEmpty(data?.Profile) ? x => true : new Predicate<IBaseElement>(x => x.HasProfile(data.Profile));
            var predicateType = string.IsNullOrEmpty(data?.Type) ? x => true : new Predicate<IBaseElement>(x => x.Type.Equals(data.Type));
            var predicateName = string.IsNullOrEmpty(data?.Identifier) ? x => true : new Predicate<IBaseElement>(x => x.Identifier.Equals(data.Identifier));

            var result = _elementManager.Root.GetElementsByPredicate(x => predicateProfile(x) && predicateType(x) && predicateName(x));
            return new QueryTreeResponseServiceData(result.Select(x => x.Address));
        }

        public override GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data)
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
                        response.Add(address, new CodeDataPair(ResponseCodes.Success, GetDataElementValue(element)));
                    }
                    catch (IoTCoreException iotCoreException)
                    {
                        response.Add(address, new CodeDataPair(iotCoreException.ResponseCode, null));
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

        public override void SetDataMulti(SetDataMultiRequestServiceData data)
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
                var element = _elementManager.GetElementByAddress(item.Key);
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

        public override GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data)
        {
            if (!_elementManager.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            try
            {
                var response = new GetSubscriberListResponseServiceData();
                if (string.IsNullOrEmpty(data?.Address))
                {
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
                            foreach (var subscription in eventElement.Subscriptions)
                            {
                                response.Add(Helpers.AddDeviceName(eventElement.Address, this.Identifier), subscription.Callback ?? "local", subscription.DataToSend, subscription.Persist, subscription.Id);
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
                    else if (element is IEventElement eventElement)
                    {
                        if (!eventElement.SubscriptionsLock.TryEnterReadLock(Configuration.Settings.Timeout))
                        {
                            throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
                        }
                        try
                        {
                            foreach (var subscription in eventElement.Subscriptions)
                            {
                                response.Add(eventElement.Address, subscription.Callback ?? "local", subscription.DataToSend, subscription.Persist, subscription.Id);
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
            finally
            {
                _elementManager.Lock.ExitReadLock();
            }
        }
    }
}
