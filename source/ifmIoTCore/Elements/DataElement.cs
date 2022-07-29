namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Common.Variant;
    using Exceptions;
    using Formats;
    using Messages;
    using Resources;
    using ServiceData.Requests;
    using ServiceData.Responses;
    using Utilities;

    public class DataElementBase<T> : BaseElement, IDataElement
    {
        private T _value;
        private readonly TimeSpan? _cacheTimeout;
        private DateTime _cacheLastRefreshTime;

        protected Func<IDataElement, T> GetDataFunc;
        protected Action<IDataElement, T> SetDataFunc;

        public IServiceElement GetDataServiceElement { get; }

        public IServiceElement SetDataServiceElement { get; }

        public IEventElement DataChangedEventElement { get; set; }

        protected DataElementBase(string identifier,
            bool createGetDataServiceElement = true,
            Func<IDataElement, T> getDataFunc = null,
            bool createSetDataServiceElement = true,
            Action<IDataElement, T> setDataFunc = null,
            bool createDataChangedEventElement = false,
            T value = default,
            TimeSpan? cacheTimeout = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(Identifiers.Data, identifier, format, profiles, uid, isHidden)
        {
            _value = value;
            _cacheTimeout = cacheTimeout;

            if (createGetDataServiceElement)
            {
                GetDataFunc = getDataFunc;
                AddChild(GetDataServiceElement = new GetterServiceElement(Identifiers.GetData, GetDataServiceFunc));
            }

            if (createSetDataServiceElement)
            {
                SetDataFunc = setDataFunc;
                AddChild(SetDataServiceElement = new SetterServiceElement(Identifiers.SetData, SetDataServiceFunc));
            }

            if (createDataChangedEventElement)
            {
                AddChild(DataChangedEventElement = new EventElement(Identifiers.DataChanged));
            }

            if (Format == null)
            {
                var type = typeof(T);
                if (type == typeof(bool))
                {
                    Format = new BooleanFormat();
                }
                else if (type == typeof(char) || 
                         type == typeof(sbyte) || type == typeof(byte) ||
                         type == typeof(short) || type == typeof(ushort) ||
                         type == typeof(int) || type == typeof(uint) ||
                         type == typeof(long) || type == typeof(ulong))
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
                else if (type == typeof(char[]) || 
                         type == typeof(sbyte[]) || type == typeof(byte[]) ||
                         type == typeof(short[]) || type == typeof(ushort[]) ||
                         type == typeof(int[]) || type == typeof(uint[]) ||
                         type == typeof(long[]) || type == typeof(ulong[]))
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

        private Variant GetDataServiceFunc(IServiceElement _, int? cid)
        {
            var value = GetData();

            return Helpers.VariantFromObject(new GetDataResponseServiceData(value));
        }

        protected virtual Variant GetData()
        {
            if (GetDataFunc != null)
            {
                if (_cacheTimeout == null || _cacheLastRefreshTime + _cacheTimeout < DateTime.UtcNow)
                {
                    var value = GetDataFunc(this);
                    if (_value == null || !_value.Equals(value))
                    {
                        _value = value;
                    }
                    _cacheLastRefreshTime = DateTime.UtcNow;
                }
            }
            return Helpers.VariantFromObject(_value);
        }

        private void SetDataServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var value = Helpers.VariantToObject<SetDataRequestServiceData>(data).Value;

            SetData(value);
        }

        protected virtual void SetData(Variant data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.ServiceDataEmpty, Identifiers.SetData));
            }

            var value = Helpers.VariantToObject<T>(data);
            SetDataFunc?.Invoke(this, value);
            if (_value == null || !_value.Equals(value))
            {
                _value = value;
                _cacheLastRefreshTime = DateTime.UtcNow;
                RaiseDataChanged();
            }
        }

        public Variant Value
        {
            get => GetData();
            set => SetData(value);
        }

        public void RaiseDataChanged()
        {
            OnPropertyChanged();
            DataChangedEventElement?.RaiseEvent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ReadOnlyDataElement<T> : DataElementBase<T>
    {
        public ReadOnlyDataElement(string identifier,
            Func<IDataElement, T> getDataFunc = null,
            bool createDataChangedEventElement = false,
            T value = default,
            TimeSpan? cacheTimeout = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(identifier, 
                true,
                getDataFunc,
                false,
                null,
                createDataChangedEventElement,
                value, 
                cacheTimeout, 
                format, profiles, uid, isHidden)
        {
        }

        protected override void SetData(Variant value)
        {
            throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Address));
        }
    }

    public class WriteOnlyDataElement<T> : DataElementBase<T>
    {
        public WriteOnlyDataElement(string identifier,
            Action<IDataElement, T> setDataFunc = null,
            bool createDataChangedEventElement = false,
            T value = default,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(identifier,
                false,
                null,
                true,
                setDataFunc,
                createDataChangedEventElement,
                value,
                null,
                format, profiles, uid, isHidden)
        {
        }

        protected override Variant GetData()
        {
            throw new IoTCoreException(ResponseCodes.NotImplemented, string.Format(Resource1.ServiceNotImplemented, Address));
        }
    }

    public class DataElement<T> : DataElementBase<T>
    {
        public DataElement(string identifier,
            Func<IDataElement, T> getDataFunc = null,
            Action<IDataElement, T> setDataFunc = null,
            bool createDataChangedEventElement = false,
            T value = default,
            TimeSpan? cacheTimeout = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) :
            base(identifier,
                true,
                getDataFunc,
                true,
                setDataFunc,
                createDataChangedEventElement,
                value,
                cacheTimeout,
                format, profiles, uid, isHidden)
        {
        }
    }
}