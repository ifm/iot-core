namespace ifm.IoTCore.ElementManager.Elements;

using System.Collections.Generic;
using ifm.Common.Variant;
using Common.Exceptions;
using Contracts.Elements;
using Contracts.Elements.Formats;
using Contracts.Elements.ServiceData.Requests;
using Contracts.Elements.ServiceData.Responses;

internal sealed class SimpleDataElement<T>(string identifier,
    T value,
    Format format,
    IEnumerable<string> profiles,
    string uid,
    bool isHidden) : DataElementBase(identifier, format ?? FormatFactory.Create(typeof(T)), profiles, uid, isHidden), IReadWriteDataElement<T>
{
    private T _value = value;

    private T InnerGetValue()
    {
        return _value;
    }

    private void InnerSetValue(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        _value = value;
        RaiseDataChanged();
    }

    T IReadDataElement<T>.Value => InnerGetValue();

    T IWriteDataElement<T>.Value
    {
        set => InnerSetValue(value);
    }

    T IReadWriteDataElement<T>.Value
    {
        get => InnerGetValue();
        set => InnerSetValue(value);
    }
    public Variant Value
    {
        get => VariantFromObject(InnerGetValue());
        set
        {
            FormatValidator.ThrowIfInvalid(Format, value);
            InnerSetValue(VariantToObject<T>(value));
        }
    }

    public GetDataResponseServiceData GetData(ServiceInfo _)
    {
        return new GetDataResponseServiceData(VariantFromObject(InnerGetValue()), TimeStamp);
    }

    public void SetData(SetDataRequestServiceData data, ServiceInfo _)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        FormatValidator.ThrowIfInvalid(Format, data.Value);
        InnerSetValue(VariantToObject<T>(data.Value));
    }
}