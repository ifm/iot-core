namespace ifm.IoTCore;

using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Exceptions;
using Contracts;
using ElementManager.Contracts;
using ElementManager.Contracts.Elements;
using ElementManager.Contracts.Elements.ServiceData.Requests;
using ElementManager.Contracts.Elements.ServiceData.Responses;
using ifm.Common;
using ifm.Common.Tree;
using ifm.Common.Variant;
using NetAdapterManager.Contracts;
using UserManager.Contracts;

internal class TreeChangedEventData(int action,
    string address,
    string type,
    int eventNumber)
{
    [VariantProperty("action", Required = true)]
    public int Action { get; } = action;

    [VariantProperty("adr", IgnoredIfNull = true)]
    public string Address { get; } = address;

    [VariantProperty("type", IgnoredIfNull = true)]
    public string Type { get; } = type;

    [VariantProperty("eventno", Required = true)]
    public int EventNumber { get; } = eventNumber;

    [VariantConstructor]
    public TreeChangedEventData() : this(2, null, null, 0)
    { }
}

internal class DeviceElement
{
    public IDeviceElement Root;
    private readonly string _apiVersion;
    private readonly IReadOnlyList<CatalogInfo> _catalogs;
    private readonly IReadOnlyList<ComponentInfo> _components;
    private readonly IElementManager _elementManager;
    private readonly IServerNetAdapterManager _serverNetAdapterManager;
    private readonly IUserManager _userManager;

    public DeviceElement(string identifier,
        string apiVersion,
        IReadOnlyList<CatalogInfo> catalogs,
        IReadOnlyList<ComponentInfo> components,
        IElementManager elementManager,
        IServerNetAdapterManager serverNetAdapterManager,
        IUserManager userManager)
    {
        _apiVersion = apiVersion;
        _catalogs = catalogs;
        _components = components;
        _elementManager = elementManager;
        _serverNetAdapterManager = serverNetAdapterManager;
        _userManager = userManager;

        Root = _elementManager.CreateDeviceElement(null, identifier,
            _ => GetIdentity(),
            (_, d) => GetTree(d),
            (_, d) => QueryTree(d),
            (_, d) => GetDataMulti(d),
            (_, d) => SetDataMulti(d),
            (_, d) => GetSubscriberList(d),
            true
        );

        _elementManager.CreateStructureElement(Root, Identifiers.Types, isHidden: true);

        var treeChangedEventListElement = _elementManager.CreateSimpleDataElement(Root.TreeChangedEventElement,
            Identifiers.EventList,
            true,
            false,
            false,
            new RingBuffer<TreeChangedEventData>(1000));

        Root.TreeChanged += (_, args) =>
        {
            var action = args.Action switch
            {
                TreeChangedActions.ChildAdded or TreeChangedActions.LinkAdded => 0,
                TreeChangedActions.ChildRemoved or TreeChangedActions.LinkRemoved => 1,
                _ => 2
            };
            var eventNumber = Root.TreeChangedEventElement.EventCounterDataElement.Value + 1;
            var treeChangedEventServiceData = new TreeChangedEventData(action,
                args.TargetNode.Address,
                args.TargetNode.Type,
                eventNumber);
            treeChangedEventListElement.Value.Add(treeChangedEventServiceData);
        };

        Root.TreeChanged += (_, _) => Root.TreeChangedEventElement.Raise();
    }

    private GetIdentityResponseServiceData GetIdentity()
    {
        var ioTInfo = new GetIdentityResponseServiceData.IoTInfo(_elementManager.Root.Identifier,
            _elementManager.Root.UId,
            _apiVersion,
            _catalogs.Count != 0 ? _catalogs.Select(x => new GetIdentityResponseServiceData.CatalogInfo(x.Name, x.Version)) : null,
            _components.Count != 0 ? _components.Select(x => new GetIdentityResponseServiceData.ComponentInfo(x.Name, x.Version)) : null,
            _serverNetAdapterManager.ServerNetAdapters.Select(x => new GetIdentityResponseServiceData.ServerInfo(x.Scheme, x.Uri?.ToString(), [x.Format])),
            null);

        var mode = _serverNetAdapterManager.ServerNetAdapters.Any(x => string.Equals(x.Scheme, "https", StringComparison.InvariantCultureIgnoreCase) && x.IsListening)
            ? "enabled" : "disabled";
        var isPasswdSet = _userManager.IsAuthenticationRequired
            ? "TRUE" : "FALSE";
        var passwordStatus = _userManager.IsAuthenticationRequired
            ? _userManager.IsInitialPassword ? "must_change" : "required" : "not_required";

        var securityInfo = new GetIdentityResponseServiceData.SecurityInfo(mode, "standard", isPasswdSet, passwordStatus, null);
        return new GetIdentityResponseServiceData(ioTInfo, securityInfo);
    }

    private GetTreeResponseServiceData GetTree(GetTreeRequestServiceData data)
    {
        IBaseElement element = Root;
        var expandConstants = false;
        var expandLinks = false;
        var expandServiceFormat = false;
        var level = int.MaxValue;

        //_treeChanges.Clear();

        if (data != null)
        {
            element = _elementManager.GetElementByAddress(data.Address);
            if (element == null) throw new DataInvalidException($"Element {data.Address} not found");

            expandConstants = data.ExpandConstValues;
            expandLinks = data.ExpandLinks;
            expandServiceFormat = data.ExpandServiceFormat;
            if (data.Level != null)
            {
                if (data.Level < -1) throw new DataInvalidException($"Invalid level {data.Level.Value}");
                if (data.Level > -1) level = data.Level.Value;
            }
        }
        return new GetTreeResponseServiceData(element, level, expandConstants, expandLinks, expandServiceFormat);
    }

    private QueryTreeResponseServiceData QueryTree(QueryTreeRequestServiceData data)
    {
        var profilePredicate = data?.Profile == null ? _ => true : new Predicate<IBaseElement>(x => x.HasProfile(data.Profile) && !x.IsHidden);
        var typePredicate = data?.Type == null ? _ => true : new Predicate<IBaseElement>(x => x.Type.Equals(data.Type, StringComparison.OrdinalIgnoreCase) && !x.IsHidden);
        var identifierPredicate = data?.Identifier == null ? _ => true : new Predicate<IBaseElement>(x => x.Identifier.Equals(data.Identifier, StringComparison.OrdinalIgnoreCase) && !x.IsHidden);

        var result = _elementManager.GetElementsByPredicate(x => profilePredicate(x) && typePredicate(x) && identifierPredicate(x) && !x.IsHidden);
        return new QueryTreeResponseServiceData(result.Select(x => x.Address));
    }

    private GetDataMultiResponseServiceData GetDataMulti(GetDataMultiRequestServiceData data)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        if (data.DataToSend.HasDuplicates()) throw new DataInvalidException("Parameter 'datatosend' has duplicates");

        var response = new GetDataMultiResponseServiceData();
        foreach (var address in data.DataToSend)
        {
            var element = _elementManager.GetElementByAddress(address);
            if (element is IReadDataElement dataElement)
            {
                try
                {
                    var elementData = dataElement.GetData();
                    response[address] = new CodeDataPair(ResponseCodes.Success, elementData.Value, elementData.TimeStamp);
                }
                catch (IoTCoreException ex)
                {
                    response[address] = new CodeDataPair(ex.ResponseCode, Variant.FromObject(new ErrorInfoResponseServiceData(ex.Message, ex.ErrorCode, ex.ErrorDetails, ex.HexError)));
                }
                catch (Exception ex)
                {
                    response[address] = new CodeDataPair(ResponseCodes.InternalError, Variant.FromObject(new ErrorInfoResponseServiceData(ex.Message)));
                }
            }
            else
            {
                response[address] = new CodeDataPair(ResponseCodes.NotFound, null);
            }
        }
        return response;
    }

    private SetDataMultiResponseServiceData SetDataMulti(SetDataMultiRequestServiceData data)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        if (data.DataToSend == null) throw new DataInvalidException("Parameter 'datatosend' missing");
        if (data.DataToSend.Keys.ToList().HasDuplicates()) throw new DataInvalidException("Parameter 'datatosend' has duplicates");

        var response = new SetDataMultiResponseServiceData();
        foreach (var item in data.DataToSend)
        {
            var element = _elementManager.GetElementByAddress(item.Key);
            if (element is IWriteDataElement dataElement)
            {
                try
                {
                    dataElement.Value = item.Value;
                    response[item.Key] = new CodeDataPair(ResponseCodes.Success, null);
                }
                catch (IoTCoreException ex)
                {
                    response[item.Key] = new CodeDataPair(ex.ResponseCode, Variant.FromObject(new ErrorInfoResponseServiceData(ex.Message, ex.ErrorCode, ex.ErrorDetails, ex.HexError)));
                }
                catch (Exception ex)
                {
                    response[item.Key] = new CodeDataPair(ResponseCodes.InternalError, Variant.FromObject(new ErrorInfoResponseServiceData(ex.Message)));
                }
            }
            else
            {
                response[item.Key] = new CodeDataPair(ResponseCodes.NotFound, null);
            }
        }
        return response;
    }

    private GetSubscriberListResponseServiceData GetSubscriberList(GetSubscriberListRequestServiceData data)
    {
        var response = new GetSubscriberListResponseServiceData();
        if (data?.Address == null)
        {
            var elements = _elementManager.GetElementsByPredicate(x => x.Type == Identifiers.Event);
            foreach (var element in elements)
            {
                var eventElement = (IEventElement)element;
                foreach (var subscription in eventElement.GetSubscriptions())
                {
                    response.Add(eventElement.Address, subscription.Callback, subscription.DataToSend, subscription.Persist, subscription.Id);
                }
            }
        }
        else
        {
            var element = _elementManager.GetElementByAddress(data.Address);
            if (element == null)
            {
                throw new DataInvalidException($"Element {data.Address} not found");
            }
            if (element is IEventElement eventElement)
            {
                foreach (var subscription in eventElement.GetSubscriptions())
                {
                    response.Add(eventElement.Address, subscription.Callback, subscription.DataToSend, subscription.Persist, subscription.Id);
                }
            }
            else
            {
                throw new DataInvalidException($"Element '{data.Address}' is not an event element");
            }
        }
        return response;
    }
}
