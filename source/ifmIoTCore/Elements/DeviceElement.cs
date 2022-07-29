namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using Common.Variant;
    using EventArguments;
    using ServiceData.Requests;
    using ServiceData.Responses;
    using Utilities;

    public abstract class DeviceElement : BaseElement, IDeviceElement
    {
        public IServiceElement GetIdentityServiceElement { get; }
        public IServiceElement GetTreeServiceElement { get; }
        public IServiceElement QueryTreeServiceElement { get; }
        public IServiceElement GetDataMultiServiceElement { get; }
        public IServiceElement SetDataMultiServiceElement { get; }
        public IServiceElement GetSubscriberListServiceElement { get; }
        public IEventElement TreeChangedEventElement { get; set; }

        protected DeviceElement(string identifier,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) : 
            base(Identifiers.Device, identifier, format, profiles, uid, isHidden)
        {
            AddChild(GetIdentityServiceElement = new GetterServiceElement(Identifiers.GetIdentity, GetIdentityServiceFunc));
            AddChild(GetTreeServiceElement = new ServiceElement(Identifiers.GetTree, GetTreeServiceFunc));
            AddChild(QueryTreeServiceElement = new ServiceElement(Identifiers.QueryTree, QueryTreeServiceFunc));
            AddChild(GetDataMultiServiceElement = new ServiceElement(Identifiers.GetDataMulti, GetDataMultiServiceFunc));
            AddChild(SetDataMultiServiceElement = new SetterServiceElement(Identifiers.SetDataMulti, SetDataMultiServiceFunc));
            AddChild(GetSubscriberListServiceElement = new ServiceElement(Identifiers.GetSubscriberList, GetSubscriberListServiceFunc));
        }

        private Variant GetIdentityServiceFunc(IServiceElement _, int? cid)
        {
            var result = GetIdentity();

            return Helpers.VariantFromObject(result);
        }

        public abstract GetIdentityResponseServiceData GetIdentity();

        private Variant GetTreeServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = data != null ? Helpers.VariantToObject<GetTreeRequestServiceData>(data) : null;

            var result = GetTree(request);

            return Helpers.VariantFromObject(result);
        }

        public abstract GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data);

        private Variant QueryTreeServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = data != null ? Helpers.VariantToObject<QueryTreeRequestServiceData>(data) : null;

            var result = QueryTree(request);

            return Helpers.VariantFromObject(result);
        }

        public abstract QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data);

        private Variant GetDataMultiServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = data != null ? Helpers.VariantToObject<GetDataMultiRequestServiceData>(data) : null;

            var result = GetDataMulti(request);

            return Helpers.VariantFromObject(result);
        }

        public abstract GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data);


        private void SetDataMultiServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = data != null ? Helpers.VariantToObject<SetDataMultiRequestServiceData>(data) : null;

            SetDataMulti(request);
        }

        public abstract void SetDataMulti(SetDataMultiRequestServiceData data);

        private Variant GetSubscriberListServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = data != null ? Helpers.VariantToObject<GetSubscriberListRequestServiceData>(data) : null;

            var result = GetSubscriberList(request);

            return Helpers.VariantFromObject(result);
        }

        public abstract GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data);

        public void RaiseTreeChanged()
        {
            base.RaiseTreeChanged(null, TreeChangedAction.TreeChanged);
        }
    }
}