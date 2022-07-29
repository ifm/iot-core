namespace ifmIoTCore.Elements
{
    using System.Collections.Generic;
    using Common.Variant;
    using ServiceData.Responses;
    using Utilities;

    public abstract class SubDeviceElement : BaseElement, ISubDeviceElement
    {
        public IServiceElement GetIdentityServiceElement { get; }

        protected SubDeviceElement(string identifier,
            Format format = null, 
            List<string> profiles = null, 
            string uid = null, 
            bool isHidden = false) : base(Identifiers.SubDevice, identifier, format, profiles, uid, isHidden)
        {
            AddChild(GetIdentityServiceElement = new GetterServiceElement(Identifiers.GetIdentity, GetIdentityServiceFunc));
        }

        private Variant GetIdentityServiceFunc(IServiceElement _, int? cid)
        {
            var result = GetIdentity();

            return Helpers.VariantFromObject(result);

        }

        public abstract GetIdentityResponseServiceData GetIdentity();
    }
}