namespace ifm.IoTCore.ElementManager;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using ifm.Common.Tree;
using DataStore.Contracts;
using Common;
using Common.Exceptions;
using Contracts;
using Contracts.Elements;
using Contracts.Elements.Formats;
using Contracts.Elements.Valuations;
using Contracts.Elements.ServiceData.Requests;
using Contracts.Elements.ServiceData.Responses;
using Elements;
using EventSender.Contracts;

public class ElementManager(IDataStore dataStore,
    IEventSender eventSender,
    int timeout = 10000) : IElementManager
{
    private readonly IDataStore _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
    private readonly IEventSender _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));

    private readonly ReaderWriterLockSlim _lock = new();
    private readonly ConcurrentDictionary<string, IBaseElement> _cache = new(StringComparer.OrdinalIgnoreCase);

    public void EnterReadLock()
    {
        if (!_lock.TryEnterReadLock(timeout))
        {
            throw new LockedException("The tree manager is locked");
        }
    }

    public void ExitReadLock()
    {
        _lock.ExitReadLock();
    }

    public void EnterUpgradeableReadLock()
    {
        if (!_lock.TryEnterUpgradeableReadLock(timeout))
        {
            throw new LockedException("The tree manager is locked");
        }
    }

    public void ExitUpgradeableReadLock()
    {
        _lock.ExitUpgradeableReadLock();
    }

    public void EnterWriteLock()
    {
        if (!_lock.TryEnterWriteLock(timeout))
        {
            throw new LockedException("The tree manager is locked");
        }
    }

    public void ExitWriteLock()
    {
        _lock.ExitWriteLock();
    }

    public bool IsReadLockHeld => _lock.IsReadLockHeld;

    public bool IsWriteLockHeld => _lock.IsWriteLockHeld;

    public IBaseElement Root
    {
        get => _root;
        set
        {
            if (_root != null)
            {
                _root.TreeChanged -= OnTreeChanged;
            }
            _root = value;
            if (_root != null)
            {
                _root.TreeChanged += OnTreeChanged;
            }
        }
    }
    private IBaseElement _root;

    private void OnTreeChanged(object sender, TreeChangedEventArgs<IBaseElement> e)
    {
        _cache.Clear();
    }

    public IBaseElement GetElementByIdentifier(IBaseElement parentElement,
        string identifier,
        bool acquireLock)
    {
        if (acquireLock) EnterReadLock();
        try
        {
            return parentElement.GetElementByIdentifier(identifier);
        }
        finally
        {
            if (acquireLock) ExitReadLock();
        }
    }

    public IBaseElement GetElementByAddress(string address,
        bool acquireLock)
    {
        if (acquireLock) EnterReadLock();
        try
        {
            address = AddressParser.PatchAddress(_root.Identifier, address);

            if (_cache.TryGetValue(address, out var value))
            {
                return value;
            }

            IBaseElement element = null;
            var tokens = AddressParser.SplitAddress(address);
            if (tokens[0].Equals(_root.Identifier, StringComparison.OrdinalIgnoreCase))
            {
                element = _root;
                for (var level = 1; level < tokens.Length; level++)
                {
                    element = element.GetElementByIdentifier(tokens[level]);
                    if (element == null) break;
                }
                if (element != null)
                {
                    _cache.TryAdd(address, element);
                }
            }
            return element;
        }
        finally
        {
            if (acquireLock) ExitReadLock();
        }
    }

    public IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate,
        bool recurse,
        bool acquireLock)
    {
        if (acquireLock) EnterReadLock();
        try
        {
            return _root.GetElementByPredicate(predicate, recurse);
        }
        finally
        {
            if (acquireLock) ExitReadLock();
        }
    }

    public IReadOnlyList<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate,
        bool recurse,
        bool acquireLock)
    {
        if (acquireLock) EnterReadLock();
        try
        {
            return _root.GetElementsByPredicate(predicate, recurse);
        }
        finally
        {
            if (acquireLock) ExitReadLock();
        }
    }

    public IBaseElement CreateElement(IBaseElement parentElement,
        string type,
        string identifier,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new BaseElement(type,
            identifier,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IDeviceElement CreateDeviceElement(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, GetIdentityResponseServiceData> getIdentityFunc,
        Func<IBaseElement, GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeFunc,
        Func<IBaseElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeFunc,
        Func<IBaseElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiFunc,
        Func<IBaseElement, SetDataMultiRequestServiceData, SetDataMultiResponseServiceData> setDataMultiFunc,
        Func<IBaseElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListFunc,
        bool createTreeChangedEventElement,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new DeviceElement(identifier,
            getIdentityFunc,
            getTreeFunc,
            queryTreeFunc,
            getDataMultiFunc,
            setDataMultiFunc,
            getSubscriberListFunc,
            format,
            profiles,
            uid,
            isHidden);

        var getIdentityServiceElement = CreateGetterServiceElement(element,
            Identifiers.GetIdentity,
            _ => element.GetIdentity(),
            null,
            null,
            null,
            false,
            false);
        getIdentityServiceElement.AccessRestrictions = AccessRestrictions.None;
        getIdentityServiceElement.AccessControl.UserRoles |= UserRole.Anonymous;

        var getTreeServiceElement = CreateServiceElement<GetTreeRequestServiceData, GetTreeResponseServiceData>(element,
            Identifiers.GetTree,
            (_, d, _) => element.GetTree(d),
            new ServiceFormat(new ServiceInputFormat(FormatFactory.Create(typeof(GetTreeRequestServiceData)), true),
                new ObjectFormat(new ObjectValuation([
                    new ObjectValuation.Field("type", new StringFormat()),
                    new ObjectValuation.Field("identifier", new StringFormat()),
                    new ObjectValuation.Field("adr", new StringFormat(), true),
                    new ObjectValuation.Field("link", new StringFormat(), true),
                    new ObjectValuation.Field("format", new ObjectFormat(), true),
                    new ObjectValuation.Field("uid", new StringFormat(), true),
                    new ObjectValuation.Field("profiles", new ArrayFormat(new ArrayValuation(new StringFormat())), true),
                    new ObjectValuation.Field("subs", new ArrayFormat(new ArrayValuation(new SelfFormat())), true),
                    new ObjectValuation.Field("value", new AnyFormat(), true)
                ]))),
            null,
            null,
            false,
            false);
        getTreeServiceElement.AccessRestrictions = AccessRestrictions.InitialPassword;

        if (queryTreeFunc != null)
        {
            CreateServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData>(element,
                Identifiers.QueryTree,
                (_, d, _) => element.QueryTree(d),
                null,
                null,
                null,
                false,
                false);
        }

        if (getDataMultiFunc != null)
        {
            CreateServiceElement<GetDataMultiRequestServiceData, GetDataMultiResponseServiceData>(element,
                Identifiers.GetDataMulti,
                (_, d, _) => element.GetDataMulti(d),
                null,
                null,
                null,
                false,
                false);
        }

        if (setDataMultiFunc != null)
        {
            CreateServiceElement<SetDataMultiRequestServiceData, SetDataMultiResponseServiceData>(element,
                Identifiers.SetDataMulti,
                (_, d, _) => element.SetDataMulti(d),
                null,
                null,
                null,
                false,
                false);
        }

        if (getSubscriberListFunc != null)
        {
            CreateServiceElement<GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData>(element,
                Identifiers.GetSubscriberList,
                (_, d, _) => element.GetSubscriberList(d),
                null,
                null,
                null,
                false,
                false);
        }

        if (createTreeChangedEventElement)
        {
            element.TreeChangedEventElement = CreateEventElement(element,
                Identifiers.TreeChanged,
                true,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IStructureElement CreateStructureElement(IBaseElement parentElement,
        string identifier,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new StructureElement(identifier,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IActionServiceElement CreateActionServiceElement(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, ServiceInfo> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new ActionServiceElement(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IActionServiceElement CreateActionServiceElement(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new ActionServiceElement(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new GetterServiceElement<TOut>(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new GetterServiceElement<TOut>(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, TIn, ServiceInfo> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new SetterServiceElement<TIn>(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, TIn> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new SetterServiceElement<TIn>(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, TIn, ServiceInfo, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new ServiceElement<TIn, TOut>(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, TIn, TOut> func,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new ServiceElement<TIn, TOut>(identifier,
            func,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IEventElement CreateEventElement(IBaseElement parentElement,
        string identifier,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateEventElement(parentElement,
            identifier,
            false,
            format, profiles, uid, isHidden, acquireLock);
    }

    public IEventElement CreateEventElement(IBaseElement parentElement,
        string identifier,
        bool createEventCounterDataElement,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new EventElement(identifier,
            _dataStore,
            _eventSender,
            this,
            format,
            profiles,
            uid,
            isHidden);

        CreateServiceElement<SubscribeRequestServiceData, SubscribeResponseServiceData>(element,
            Identifiers.Subscribe,
            element.SubscribeFunc,
            null,
            null,
            null,
            false,
            false);

        CreateSetterServiceElement<UnsubscribeRequestServiceData>(element,
            Identifiers.Unsubscribe,
            element.UnsubscribeFunc,
            null, null,
            null,
            false,
            false);

        if (createEventCounterDataElement)
        {
            element.EventCounterDataElement = CreateSimpleDataElement(element,
                Identifiers.EventCounter,
                true,
                false,
                false,
                0,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IReadWriteDataElement<T> CreateSimpleDataElement<T>(IBaseElement parentElement,
        string identifier,
        T value,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateSimpleDataElement(parentElement,
            identifier,
            true,
            true,
            false,
            value, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadWriteDataElement<T> CreateSimpleDataElement<T>(IBaseElement parentElement,
        string identifier,
        bool createDataChangedEventElement,
        T value,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateSimpleDataElement(parentElement,
            identifier,
            true,
            true,
            createDataChangedEventElement,
            value, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadWriteDataElement<T> CreateSimpleDataElement<T>(IBaseElement parentElement,
        string identifier,
        bool createGetDataServiceElement,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        T value,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new SimpleDataElement<T>(identifier,
            value,
            format,
            profiles,
            uid,
            isHidden);

        if (createGetDataServiceElement)
        {
            CreateGetterServiceElement(element,
                Identifiers.GetData,
                _ => element.GetData(null),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createSetDataServiceElement)
        {
            CreateSetterServiceElement<SetDataRequestServiceData>(element,
                Identifiers.SetData,
                (_, d) => element.SetData(d, null),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateReadOnlyDataElement(parentElement,
            identifier,
            getDataFunc,
            true,
            false,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateReadOnlyDataElement(parentElement,
            identifier,
            getDataFunc,
            true,
            createDataChangedEventElement,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        bool createGetDataServiceElement,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new ReadOnlyDataElement<T>(identifier,
            getDataFunc,
            value,
            cacheTimeout,
            format,
            profiles,
            uid,
            isHidden);

        if (createGetDataServiceElement)
        {
            CreateGetterServiceElement(element,
                Identifiers.GetData, _ => element.GetData(null),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateReadOnlyDataElement(parentElement,
            identifier,
            getDataFunc,
            true,
            false,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateReadOnlyDataElement(parentElement,
            identifier,
            getDataFunc,
            true,
            createDataChangedEventElement,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        bool createGetDataServiceElement,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new ReadOnlyDataElement<T>(identifier,
            getDataFunc,
            value,
            cacheTimeout,
            format,
            profiles,
            uid,
            isHidden);

        if (createGetDataServiceElement)
        {
            CreateGetterServiceElement(element,
                Identifiers.GetData,
                _ => element.GetData(null),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateWriteOnlyDataElement(parentElement,
            identifier,
            setDataFunc,
            true,
            false,
            format, profiles, uid, isHidden, acquireLock);
    }

    public IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createDataChangedEventElement,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateWriteOnlyDataElement(parentElement,
            identifier,
            setDataFunc,
            true,
            createDataChangedEventElement,
            format, profiles, uid, isHidden, acquireLock);
    }

    public IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new WriteOnlyDataElement<T>(identifier,
            setDataFunc,
            format,
            profiles,
            uid,
            isHidden);

        if (createSetDataServiceElement)
        {
            CreateSetterServiceElement<SetDataRequestServiceData>(element,
                Identifiers.SetData,
                (_, d, i) => element.SetData(d, i),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T> setDataFunc,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateWriteOnlyDataElement(parentElement,
            identifier,
            setDataFunc,
            true,
            false,
            format, profiles, uid, isHidden, acquireLock);
    }

    public IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T> setDataFunc,
        bool createDataChangedEventElement,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateWriteOnlyDataElement(parentElement,
            identifier,
            setDataFunc,
            true,
            createDataChangedEventElement,
            format, profiles, uid, isHidden, acquireLock);
    }

    public IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T> setDataFunc,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new WriteOnlyDataElement<T>(identifier,
            setDataFunc,
            format,
            profiles,
            uid,
            isHidden);

        if (createSetDataServiceElement)
        {
            CreateSetterServiceElement<SetDataRequestServiceData>(element,
                Identifiers.SetData,
                (_, d, i) => element.SetData(d, i),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateDataElement(parentElement,
            identifier,
            getDataFunc,
            setDataFunc,
            true,
            true,
            false,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateDataElement(parentElement,
            identifier,
            getDataFunc,
            setDataFunc,
            true,
            true,
            createDataChangedEventElement,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createGetDataServiceElement,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new DataElement<T>(identifier,
            getDataFunc,
            setDataFunc,
            value,
            cacheTimeout,
            format,
            profiles,
            uid,
            isHidden);

        if (createGetDataServiceElement)
        {
            CreateGetterServiceElement(element,
                Identifiers.GetData,
                _ => element.GetData(null),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createSetDataServiceElement)
        {
            CreateSetterServiceElement<SetDataRequestServiceData>(element,
                Identifiers.SetData,
                (_, d, i) => element.SetData(d, i),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateDataElement(parentElement,
            identifier,
            getDataFunc,
            setDataFunc,
            true,
            true,
            false,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        return CreateDataElement(parentElement,
            identifier,
            getDataFunc,
            setDataFunc,
            true,
            true,
            createDataChangedEventElement,
            value, cacheTimeout, format, profiles, uid, isHidden, acquireLock);
    }

    public IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        bool createGetDataServiceElement,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        T value,
        TimeSpan? cacheTimeout,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new DataElement<T>(identifier,
            getDataFunc,
            setDataFunc,
            value,
            cacheTimeout,
            format,
            profiles,
            uid,
            isHidden);

        if (createGetDataServiceElement)
        {
            CreateGetterServiceElement(element,
                Identifiers.GetData,
                _ => element.GetData(null),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createSetDataServiceElement)
        {
            CreateSetterServiceElement<SetDataRequestServiceData>(element,
                Identifiers.SetData,
                (_, d, i) => element.SetData(d, i),
                new NoFormat(),
                null,
                null,
                false,
                false);
        }

        if (createDataChangedEventElement)
        {
            element.DataChangedEventElement = CreateEventElement(element,
                Identifiers.DataChanged,
                null,
                null,
                null,
                false,
                false);
        }

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public ITypeElement CreateTypeElement(IBaseElement parentElement,
        string identifier,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden,
        bool acquireLock)
    {
        var element = new TypeElement(identifier,
            format,
            profiles,
            uid,
            isHidden);

        if (parentElement == null) return element;
        InnerAddChild(parentElement, element, acquireLock);
        return element;
    }

    public void AddElement(IBaseElement parentElement,
        IBaseElement element,
        bool acquireLock)
    {
        if (parentElement == null) throw new ArgumentNullException(nameof(parentElement));
        if (element == null) throw new ArgumentNullException(nameof(element));

        InnerAddChild(parentElement, element, acquireLock);
    }

    public void RemoveElement(IBaseElement parentElement,
        IBaseElement element,
        bool acquireLock)
    {
        if (parentElement == null) throw new ArgumentNullException(nameof(parentElement));
        if (element == null) throw new ArgumentNullException(nameof(element));

        InnerRemoveChild(parentElement, element, acquireLock);
    }

    public void AddLink(IBaseElement sourceElement,
        IBaseElement targetElement,
        string identifier,
        bool acquireLock)
    {
        if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
        if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

        InnerAddLink(sourceElement, targetElement, identifier ?? targetElement.Identifier, acquireLock);
    }

    public void RemoveLink(IBaseElement sourceElement,
        IBaseElement targetElement,
        bool acquireLock)
    {
        if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
        if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

        InnerRemoveLink(sourceElement, targetElement, acquireLock);
    }

    private void InnerAddChild(IBaseElement parentElement, IBaseElement childElement, bool acquireLock)
    {
        if (acquireLock) EnterUpgradeableReadLock();
        try
        {
            if (acquireLock) EnterWriteLock();
            try
            {
                parentElement.AddChild(childElement);
            }
            finally
            {
                if (acquireLock) ExitWriteLock();
            }
        }
        finally
        {
            if (acquireLock) ExitUpgradeableReadLock();
        }
    }

    private void InnerRemoveChild(IBaseElement parentElement, IBaseElement childElement, bool acquireLock)
    {
        if (acquireLock) EnterUpgradeableReadLock();
        try
        {
            if (acquireLock) EnterWriteLock();
            try
            {
                parentElement.RemoveChild(childElement);
            }
            finally
            {
                if (acquireLock) ExitWriteLock();
            }
        }
        finally
        {
            if (acquireLock) ExitUpgradeableReadLock();
        }
    }

    private void InnerAddLink(IBaseElement sourceElement, IBaseElement targetElement, string identifier, bool acquireLock)
    {
        if (acquireLock) EnterUpgradeableReadLock();
        try
        {
            if (acquireLock) EnterWriteLock();
            try
            {
                sourceElement.AddLink(targetElement, identifier);
            }
            finally
            {
                if (acquireLock) ExitWriteLock();
            }
        }
        finally
        {
            if (acquireLock) ExitUpgradeableReadLock();
        }
    }

    private void InnerRemoveLink(IBaseElement sourceElement, IBaseElement targetElement, bool acquireLock)
    {
        if (acquireLock) EnterUpgradeableReadLock();
        try
        {
            if (acquireLock) EnterWriteLock();
            try
            {
                sourceElement.RemoveLink(targetElement);
            }
            finally
            {
                if (acquireLock) ExitWriteLock();
            }
        }
        finally
        {
            if (acquireLock) ExitUpgradeableReadLock();
        }
    }
}