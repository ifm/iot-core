namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using Formats;
    using Exceptions;
    using Messages;
    using Resources;
    using ServiceData.Requests;
    using ServiceData.Responses;

    internal class DeviceElement : BaseElement, IDeviceElement
    {
        private Func<IDeviceElement, GetIdentityResponseServiceData> _getIdentityFunc;
        private Func<IDeviceElement, GetTreeRequestServiceData, GetTreeResponseServiceData> _getTreeFunc;
        private Func<IDeviceElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> _queryTreeFunc;
        private Func<IDeviceElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> _getDataMultiFunc;
        private Action<IDeviceElement, SetDataMultiRequestServiceData> _setDataMultiFunc;
        private Func<IDeviceElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> _getSubscriberListFunc;

        public DeviceElement(IBaseElement parent,
            string identifier,
            Func<IDeviceElement, GetIdentityResponseServiceData> getIdentityFunc,
            Func<IDeviceElement, GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeFunc,
            Func<IDeviceElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeFunc,
            Func<IDeviceElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiFunc,
            Action<IDeviceElement, SetDataMultiRequestServiceData> setDataMultiFunc,
            Func<IDeviceElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListFunc,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) : 
            base(parent, Identifiers.Device, identifier, format, profiles, uid, isHidden, context)
        {
            _getIdentityFunc = getIdentityFunc;
            _getTreeFunc = getTreeFunc;
            _queryTreeFunc = queryTreeFunc;
            _getDataMultiFunc = getDataMultiFunc;
            _setDataMultiFunc = setDataMultiFunc;
            _getSubscriberListFunc = getSubscriberListFunc;

            IGetterServiceElement<GetIdentityResponseServiceData> getIdentityServiceElement = null;
            IServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeServiceElement = null;
            IServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeServiceElement = null;
            IServiceElement<GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiServiceElement = null;
            ISetterServiceElement<SetDataMultiRequestServiceData> setDataMultiServiceElement = null;
            IServiceElement<GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListServiceElement = null;

            try
            {
                getIdentityServiceElement = CreateGetIdentityServiceElement();
                getTreeServiceElement = CreateGetTreeServiceElement();
                queryTreeServiceElement = CreateQueryTreeServiceElement();
                getDataMultiServiceElement = CreateGetDataMultiServiceElement();
                setDataMultiServiceElement = CreateSetDataMultiServiceElement();
                getSubscriberListServiceElement = CreateGetSubscriberListServiceElement();
            }
            catch
            {
                // ToDo: Check if this is really required -> Ask Sascha
                // Creating elements does not fail, except in low memory condition?
                // Does GC collect the elements, even if they have a reference to this (which is not constructed)?
                // Does GC collect the elements, even if they have a reference to itself in the references?

                getIdentityServiceElement?.Dispose();
                getTreeServiceElement?.Dispose();
                queryTreeServiceElement?.Dispose();
                getDataMultiServiceElement?.Dispose();
                setDataMultiServiceElement?.Dispose();
                getSubscriberListServiceElement?.Dispose();

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

        private GetIdentityResponseServiceData GetIdentityServiceFunc(IServiceElement _, int? cid)
        {
            return GetIdentity();
        }

        public GetIdentityResponseServiceData GetIdentity()
        {
            if (_getIdentityFunc == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifiers.GetIdentity));
            }
            return _getIdentityFunc(this);
        }

        private GetTreeResponseServiceData GetTreeServiceFunc(IServiceElement _, GetTreeRequestServiceData data, int? cid)
        {
            return GetTree(data);
        }

        public GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data)
        {
            if (_getTreeFunc == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifiers.GetTree));
            }
            return _getTreeFunc(this, data);
        }

        private QueryTreeResponseServiceData QueryTreeServiceFunc(IServiceElement _, QueryTreeRequestServiceData data, int? cid)
        {
            return QueryTree(data);
        }

        public QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data)
        {
            if (_queryTreeFunc == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifiers.QueryTree));
            }
            return _queryTreeFunc(this, data);
        }

        private GetDataMultiResponseServiceData GetDataMultiServiceFunc(IServiceElement _, GetDataMultiRequestServiceData data, int? cid)
        {
            return GetDataMulti(data);
        }

        public GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data)
        {
            if (_getDataMultiFunc == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifiers.GetDataMulti));
            }
            return _getDataMultiFunc(this, data);
        }

        private void SetDataMultiServiceFunc(IServiceElement _, SetDataMultiRequestServiceData data, int? cid)
        {
            SetDataMulti(data);
        }

        public void SetDataMulti(SetDataMultiRequestServiceData data)
        {
            if (_setDataMultiFunc == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifiers.SetDataMulti));
            }
            _setDataMultiFunc(this, data);
        }

        private GetSubscriberListResponseServiceData GetSubscriberListServiceFunc(IServiceElement _, GetSubscriberListRequestServiceData data, int? cid)
        {
            return GetSubscriberList(data);
        }

        public GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data)
        {
            if (_getSubscriberListFunc == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifiers.GetSubscriberList));
            }
            return _getSubscriberListFunc(this, data);
        }

        protected override void Dispose(bool disposing)
        {
            // ToDo: Why do we need this -> ask Sascha
            _getDataMultiFunc = null;
            _getSubscriberListFunc = null;
            _getIdentityFunc = null;
            _getTreeFunc = null;
            _setDataMultiFunc = null;
            _queryTreeFunc = null;

            base.Dispose(disposing);
        }
    }
}