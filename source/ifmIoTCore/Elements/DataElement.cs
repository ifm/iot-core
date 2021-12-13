namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using Formats;
    using Messages;
    using Newtonsoft.Json.Linq;
    using Resources;
    using ServiceData.Requests;
    using ServiceData.Responses;
    using Utilities;

    internal class DataElement<T> : BaseElement, IDataElement<T>
    {
        private Func<IDataElement<T>, T> _getDataFunc;
        private Action<IDataElement<T>, T> _setDataFunc;

        private T _value;
        private readonly TimeSpan? _cacheTimeout;
        private DateTime _cacheLastRefreshTime;

        public IEventElement DataChangedEventElement { get; set; }

        public DataElement(IBaseElement parent,
            string identifier,
            Func<IDataElement<T>, T> getDataFunc = null,
            Action<IDataElement<T>, T> setDataFunc = null,
            bool createGetDataServiceElement = true,
            bool createSetDataServiceElement = true,
            T value = default,
            TimeSpan? cacheTimeout = null,
            Format format = null, 
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) : 
            base(parent, Identifiers.Data, identifier, format, profiles, uid, isHidden, context)
        {
            _getDataFunc = getDataFunc;
            _setDataFunc = setDataFunc;

            _value = value;
            _cacheTimeout = cacheTimeout;

            IGetterServiceElement<GetDataResponseServiceData> getDataServiceElement = null;
            ISetterServiceElement<SetDataRequestServiceData> setDataServiceElement = null;

            try
            {
                if (createGetDataServiceElement)
                {
                    getDataServiceElement = CreateGetDataServiceElement();
                }

                if (createSetDataServiceElement)
                {
                    setDataServiceElement = CreateSetDataServiceElement();
                }
            }
            catch
            {
                getDataServiceElement?.Dispose();
                setDataServiceElement?.Dispose();

                throw;
            }

            if (Format == null)
            {
                var type = typeof(T);
                if (type == typeof(bool))
                {
                    Format = new BooleanFormat();
                }
                else if (type == typeof(int) || type == typeof(uint))
                {
                    Format = new IntegerFormat(null);
                }
                else if (type == typeof(float) || type == typeof(double))
                {
                    Format = new FloatFormat(null);
                }
                else if (type == typeof(string))
                {
                    Format = new StringFormat(null);
                }
                else if (type == typeof(bool[]))
                {
                    Format = new ArrayFormat(new Valuations.ArrayValuation(Format.Types.Boolean, new BooleanFormat()));
                }
                else if (type == typeof(int[]) || type == typeof(uint[]))
                {
                    Format = new ArrayFormat(new Valuations.ArrayValuation(Format.Types.Number, new IntegerFormat(null)));
                }
                else if (type == typeof(float[]) || type == typeof(double[]))
                {
                    Format = new ArrayFormat(new Valuations.ArrayValuation(Format.Types.Number, new FloatFormat(null)));
                }
                else if (type == typeof(string[]))
                {
                    Format = new ArrayFormat(new Valuations.ArrayValuation(Format.Types.String, new StringFormat(null)));
                }
            }
        }

        protected IGetterServiceElement<GetDataResponseServiceData> CreateGetDataServiceElement()
        {
            var getDataServiceElement = new GetterServiceElement<GetDataResponseServiceData>(this, Identifiers.GetData, GetDataServiceFunc);
            References.AddForwardReference(this, getDataServiceElement, getDataServiceElement.Identifier, ReferenceType.Child);
            return getDataServiceElement;
        }

        protected ISetterServiceElement<SetDataRequestServiceData> CreateSetDataServiceElement()
        {
            var setDataServiceElement = new SetterServiceElement<SetDataRequestServiceData>(this, Identifiers.SetData, SetDataServiceFunc);
            References.AddForwardReference(this, setDataServiceElement, setDataServiceElement.Identifier, ReferenceType.Child);
            return setDataServiceElement;
        }

        private GetDataResponseServiceData GetDataServiceFunc(IServiceElement _, int? cid)
        {
            return new GetDataResponseServiceData(Value);
        }

        private void SetDataServiceFunc(IServiceElement _, SetDataRequestServiceData data, int? cid)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.ServiceDataEmpty, Identifiers.SetData));
            }
            Value = data.GetValue<T>();
        }

        public T Value
        {
            get
            {
                if (_getDataFunc != null)
                {
                    if (_cacheTimeout == null || _cacheLastRefreshTime + _cacheTimeout < DateTime.UtcNow)
                    {
                        var value = _getDataFunc(this);
                        if (_value == null || !_value.Equals(value))
                        {
                            _value = value;

                            // ToDo: Raise here or not, if getdata reads other value than stored?
                            //RaiseDataChanged();
                        }
                        _cacheLastRefreshTime = DateTime.UtcNow;
                    }
                }
                return _value;
            }

            set
            {
                _setDataFunc?.Invoke(this, value);
                if (_value == null || !_value.Equals(value))
                {
                    _value = value;
                    _cacheLastRefreshTime = DateTime.UtcNow;
                    RaiseDataChanged();
                }
            }
        }

        JToken IDataElement.Value
        {
            get => Helpers.ToJson(Value);
            set => Value = Helpers.FromJson<T>(value);
        }

        public void RaiseDataChanged()
        {
            DataChangedEventElement?.RaiseEvent();
        }

        protected override void Dispose(bool disposing)
        {
            _getDataFunc = null;
            _setDataFunc = null;
            DataChangedEventElement = null;

            base.Dispose(disposing);
        }
    }
}