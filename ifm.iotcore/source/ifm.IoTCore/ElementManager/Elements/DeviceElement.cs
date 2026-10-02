namespace ifm.IoTCore.ElementManager.Elements;

using System;
using System.Collections.Generic;
using Common;
using Contracts.Elements;
using Contracts.Elements.Formats;
using Contracts.Elements.ServiceData.Requests;
using Contracts.Elements.ServiceData.Responses;

internal sealed class DeviceElement(string identifier,
    Func<IBaseElement, GetIdentityResponseServiceData> getIdentityFunc,
    Func<IBaseElement, GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeFunc,
    Func<IBaseElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeFunc,
    Func<IBaseElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiFunc,
    Func<IBaseElement, SetDataMultiRequestServiceData, SetDataMultiResponseServiceData> setDataMultiFunc,
    Func<IBaseElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListFunc,
    Format format,
    IEnumerable<string> profiles,
    string uid,
    bool isHidden) : BaseElement(Identifiers.Device, identifier, format, profiles, uid, isHidden), IDeviceElement
{
    private readonly Func<IBaseElement, GetIdentityResponseServiceData> _getIdentityFunc = getIdentityFunc ?? throw new ArgumentNullException(nameof(getIdentityFunc));
    private readonly Func<IBaseElement, GetTreeRequestServiceData, GetTreeResponseServiceData> _getTreeFunc = getTreeFunc ?? throw new ArgumentNullException(nameof(getTreeFunc));

    public IEventElement TreeChangedEventElement { get; set; }

    public GetIdentityResponseServiceData GetIdentity()
    {
        return _getIdentityFunc(this);
    }

    public GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data)
    {
        return _getTreeFunc(this, data);
    }

    public QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data)
    {
        return queryTreeFunc != null ? queryTreeFunc(this, data) : throw new NotImplementedException(nameof(queryTreeFunc));
    }

    public GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data)
    {
        return getDataMultiFunc != null ? getDataMultiFunc(this, data) : throw new NotImplementedException(nameof(getDataMultiFunc));
    }

    public SetDataMultiResponseServiceData SetDataMulti(SetDataMultiRequestServiceData data)
    {
        return setDataMultiFunc != null ? setDataMultiFunc(this, data) : throw new NotImplementedException(nameof(setDataMultiFunc));
    }

    public GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data)
    {
        return getSubscriberListFunc != null ? getSubscriberListFunc(this, data) : throw new NotImplementedException(nameof(getSubscriberListFunc));
    }
}
