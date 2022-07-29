namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using Common.Variant;
    using Exceptions;
    using Messages;
    using Resources;

    public class ActionServiceElement : BaseElement, IServiceElement
    {
        protected Action<IServiceElement, int?> Func;

        public ActionServiceElement(string identifier,
            Action<IServiceElement, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(Identifiers.Service, identifier, format, profiles, uid, isHidden)
        {
            Func = func;
        }

        public Variant Invoke(Variant data, int? cid)
        {
            if (Func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }

            Func(this, cid);
            return null;
        }
    }

    public class GetterServiceElement : BaseElement, IServiceElement
    {
        protected Func<IServiceElement, int?, Variant> Func;

        public GetterServiceElement(string identifier,
            Func<IServiceElement, int?, Variant> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(Identifiers.Service, identifier, format, profiles, uid, isHidden)
        {
            Func = func;
        }

        public Variant Invoke(Variant data, int? cid)
        {
            if (Func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }

            return Func(this, cid);
        }
    }

    public class SetterServiceElement : BaseElement, IServiceElement
    {
        protected Action<IServiceElement, Variant, int?> Func;

        public SetterServiceElement(string identifier,
            Action<IServiceElement, Variant, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(Identifiers.Service, identifier, format, profiles, uid, isHidden)
        {
            Func = func;
        }

        public Variant Invoke(Variant data, int? cid)
        {
            if (Func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }

            Func(this, data, cid);
            return null;
        }
    }

    public class ServiceElement : BaseElement, IServiceElement
    {
        protected Func<IServiceElement, Variant, int?, Variant> Func;

        public ServiceElement(string identifier,
            Func<IServiceElement, Variant, int?, Variant> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(Identifiers.Service, identifier, format, profiles, uid, isHidden)
        {
            Func = func;
        }

        public Variant Invoke(Variant data, int? cid)
        {
            if (Func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }

            return Func(this, data, cid);
        }
    }
}