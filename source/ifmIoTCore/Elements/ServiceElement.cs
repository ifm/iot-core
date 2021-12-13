namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Newtonsoft.Json.Linq;
    using Formats;
    using Messages;
    using Resources;
    using Utilities;

    internal class ActionServiceElement : BaseElement, IActionServiceElement
    {
        private Action<IActionServiceElement, int?> _func;

        public ActionServiceElement(IBaseElement parent,
            string identifier,
            Action<IActionServiceElement, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) :
            base(parent, Identifiers.Service, identifier, format, profiles, uid, isHidden, context)
        {
            _func = func;
        }

        public void Invoke(int? cid)
        {
            if (_func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }
            _func(this, cid);
        }

        JToken IServiceElement.Invoke(JToken data, int? cid)
        {
            Invoke(cid);
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            _func = null;

            base.Dispose(disposing);
        }
    }

    internal class GetterServiceElement<TOut> : BaseElement, IGetterServiceElement<TOut>
    {
        private Func<IGetterServiceElement<TOut>, int?, TOut> _func;

        public GetterServiceElement(IBaseElement parent,
            string identifier,
            Func<IGetterServiceElement<TOut>, int?, TOut> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) :
            base(parent, Identifiers.Service, identifier, format, profiles, uid, isHidden, context)
        {
            _func = func;
        }

        public TOut Invoke(int? cid)
        {
            if (_func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }
            return _func(this, cid);
        }

        JToken IServiceElement.Invoke(JToken data, int? cid)
        {
            return Helpers.ToJson(Invoke(cid));
        }

        protected override void Dispose(bool disposing)
        {
            _func = null;

            base.Dispose(disposing);
        }
    }

    internal class SetterServiceElement<TIn> : BaseElement, ISetterServiceElement<TIn>
    {
        private Action<ISetterServiceElement<TIn>, TIn, int?> _func;

        public SetterServiceElement(IBaseElement parent,
            string identifier,
            Action<ISetterServiceElement<TIn>, TIn, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) :
            base(parent, Identifiers.Service, identifier, format, profiles, uid, isHidden, context)
        {
            _func = func;
        }

        public void Invoke(TIn data, int? cid)
        {
            if (_func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }
            _func(this, data, cid);
        }

        JToken IServiceElement.Invoke(JToken data, int? cid)
        {
            Invoke(Helpers.FromJson<TIn>(data), cid);
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            _func = null;

            base.Dispose(disposing);
        }
    }

    internal class ServiceElement<TIn, TOut> : BaseElement, IServiceElement<TIn, TOut>
    {
        private Func<IServiceElement<TIn, TOut>, TIn, int?, TOut> _func;

        public ServiceElement(IBaseElement parent,
            string identifier,
            Func<IServiceElement<TIn, TOut>, TIn, int?, TOut> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) : 
            base(parent, Identifiers.Service, identifier, format, profiles, uid, isHidden, context)
        {
            _func = func;
        }

        public TOut Invoke(TIn data, int? cid)
        {
            if (_func == null)
            {
                throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Identifier));
            }
            return _func(this, data, cid);
        }

        JToken IServiceElement.Invoke(JToken data, int? cid)
        {
            return Helpers.ToJson(Invoke(Helpers.FromJson<TIn>(data), cid));
        }

        protected override void Dispose(bool disposing)
        {
            _func = null;

            base.Dispose(disposing);
        }
    }
}