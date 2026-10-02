namespace ifm.IoTCore.ElementManager.Elements;

using System;
using System.Collections.Generic;
using ifm.Common.Variant;
using Common.Exceptions;
using Contracts.Elements;
using Contracts.Elements.Formats;
using Contracts.Elements.ServiceData.Requests;
using Contracts.Elements.ServiceData.Responses;

internal sealed class ReadOnlyDataElement<T> : DataElementBase, IReadDataElement<T>
{
    private readonly Func<IBaseElement, ServiceInfo, T> _getDataFunc;
    private readonly Func<IBaseElement, T> _getDataFunc2;
    private T _cacheValue;
    private readonly TimeSpan? _cacheTimeout;
    private DateTime? _cacheLastRefreshTime;

    public ReadOnlyDataElement(string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden)
    {
        _getDataFunc = getDataFunc ?? throw new ArgumentNullException(nameof(getDataFunc));
        _cacheValue = value;
        _cacheTimeout = cacheTimeout;
    }

    public ReadOnlyDataElement(string identifier,
        Func<IBaseElement, T> getDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden)
    {
        _getDataFunc2 = getDataFunc ?? throw new ArgumentNullException(nameof(getDataFunc));
        _cacheValue = value;
        _cacheTimeout = cacheTimeout;
    }

    private T InnerGetValue(ServiceInfo info)
    {
        if (_cacheTimeout == null || _cacheLastRefreshTime == null || _cacheLastRefreshTime + _cacheTimeout < DateTime.Now)
        {
            var value = _getDataFunc2 != null ? _getDataFunc2(this) : _getDataFunc(this, info);
            _cacheLastRefreshTime = DateTime.Now;
            if (!EqualityComparer<T>.Default.Equals(_cacheValue, value))
            {
                _cacheValue = value;
                RaiseDataChanged();
            }
        }
        return _cacheValue;
    }

    T IReadDataElement<T>.Value => InnerGetValue(null);

    public Variant Value => VariantFromObject(InnerGetValue(null));

    public GetDataResponseServiceData GetData(ServiceInfo info)
    {
        return new GetDataResponseServiceData(VariantFromObject(InnerGetValue(info)), TimeStamp);
    }
}

internal sealed class WriteOnlyDataElement<T> : DataElementBase, IWriteDataElement<T>
{
    private readonly Action<IBaseElement, T, ServiceInfo> _setDataFunc;
    private readonly Action<IBaseElement, T> _setDataFunc2;
    private T _cacheValue;

    public WriteOnlyDataElement(string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden)
    {
        _setDataFunc = setDataFunc ?? throw new ArgumentNullException(nameof(setDataFunc));
    }

    public WriteOnlyDataElement(string identifier,
        Action<IBaseElement, T> setDataFunc,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden)
    {
        _setDataFunc2 = setDataFunc ?? throw new ArgumentNullException(nameof(setDataFunc));
    }

    private void InnerSetValue(T value, ServiceInfo info)
    {
        if (_setDataFunc2 != null)
        {
            _setDataFunc2.Invoke(this, value);
        } 
        else
        {
            _setDataFunc(this, value, info);
        }
        if (!EqualityComparer<T>.Default.Equals(_cacheValue, value))
        {
            _cacheValue = value;
            RaiseDataChanged();
        }
    }

    T IWriteDataElement<T>.Value
    {
        set => InnerSetValue(value, null);
    }

    public Variant Value
    {
        set => InnerSetValue(Variant.ToObject<T>(value), null);
    }

    public void SetData(SetDataRequestServiceData data, ServiceInfo info)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        InnerSetValue(VariantToObject<T>(data.Value), info);
    }
}

internal sealed class DataElement<T> : DataElementBase, IReadWriteDataElement<T>
{
    private readonly Func<IBaseElement, ServiceInfo, T> _getDataFunc;
    private readonly Func<IBaseElement, T> _getDataFunc2;
    private readonly Action<IBaseElement, T, ServiceInfo> _setDataFunc;
    private readonly Action<IBaseElement, T> _setDataFunc2;
    private T _cacheValue;
    private readonly TimeSpan? _cacheTimeout;
    private DateTime? _cacheLastRefreshTime;

    public DataElement(string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden = false) : base(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden)
    {
        _getDataFunc = getDataFunc ?? throw new ArgumentNullException(nameof(getDataFunc));
        _setDataFunc = setDataFunc ?? throw new ArgumentNullException(nameof(setDataFunc));
        _cacheValue = value;
        _cacheTimeout = cacheTimeout;
    }

    public DataElement(string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden)
    {
        _getDataFunc2 = getDataFunc ?? throw new ArgumentNullException(nameof(getDataFunc));
        _setDataFunc2 = setDataFunc ?? throw new ArgumentNullException(nameof(setDataFunc));
        _cacheValue = value;
        _cacheTimeout = cacheTimeout;
    }

    private T InnerGetValue(ServiceInfo info)
    {
        if (_cacheTimeout == null || _cacheLastRefreshTime == null || _cacheLastRefreshTime + _cacheTimeout < DateTime.Now)
        {
            var value = _getDataFunc2 != null ? _getDataFunc2(this) : _getDataFunc(this, info);
            _cacheLastRefreshTime = DateTime.Now;
            if (!EqualityComparer<T>.Default.Equals(_cacheValue, value))
            {
                _cacheValue = value;
                RaiseDataChanged();
            }
        }
        return _cacheValue;
    }

    private void InnerSetValue(T value, ServiceInfo info)
    {
        if (_setDataFunc2 != null)
        {
            _setDataFunc2.Invoke(this, value);
        } 
        else
        {
            _setDataFunc(this, value, info);
        }
        _cacheLastRefreshTime = DateTime.Now;
        if (!EqualityComparer<T>.Default.Equals(_cacheValue, value))
        {
            _cacheValue = value;
            RaiseDataChanged();
        }
    }

    T IReadDataElement<T>.Value => InnerGetValue(null);

    T IWriteDataElement<T>.Value
    {
        set => InnerSetValue(value, null);
    }

    T IReadWriteDataElement<T>.Value
    {
        get => InnerGetValue(null);
        set => InnerSetValue(value, null);
    }

    public Variant Value
    {
        get => VariantFromObject(InnerGetValue(null));
        set => InnerSetValue(VariantToObject<T>(value), null);
    }

    public GetDataResponseServiceData GetData(ServiceInfo info)
    {
        return new GetDataResponseServiceData(VariantFromObject(InnerGetValue(info)), TimeStamp);
    }

    public void SetData(SetDataRequestServiceData data, ServiceInfo info)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        InnerSetValue(VariantToObject<T>(data.Value), info);
    }
}
