namespace ifm.IoTCore.ElementManager.Contracts;

using System;
using System.Collections.Generic;
using Elements;
using Elements.Formats;
using Elements.ServiceData.Requests;
using Elements.ServiceData.Responses;

/// <summary>
/// Provides functionality to interact with an element manager.
/// </summary>
public interface IElementManager
{
    /// <summary>
    /// Enters the read lock.
    /// </summary>
    void EnterReadLock();

    /// <summary>
    /// Exits the read lock.
    /// </summary>
    void ExitReadLock();

    /// <summary>
    /// Enter upgradable read lock.
    /// </summary>
    void EnterUpgradeableReadLock();

    /// <summary>
    /// Exit upgradable read lock.
    /// </summary>
    void ExitUpgradeableReadLock();

    /// <summary>
    ///  Enters the write lock.
    /// </summary>
    void EnterWriteLock();

    /// <summary>
    /// Exists the write lock.
    /// </summary>
    void ExitWriteLock();

    /// <summary>
    /// Returns true if a read lock is held; otherwise false.
    /// </summary>
    bool IsReadLockHeld { get; }

    /// <summary>
    /// Returns true if a write lock is held; otherwise false.
    /// </summary>
    bool IsWriteLockHeld { get; }

    /// <summary>
    /// Gets or sets the root element.
    /// </summary>
    IBaseElement Root { get; set; }

    /// <summary>
    /// Gets the child element with the specified identifier from the specified parent element.
    /// </summary>
    /// <param name="parentElement">The parent element.</param>
    /// <param name="identifier">The identifier of the element to get.</param>
    /// <param name="acquireLock">If true a read lock is acquired; otherwise not.</param>
    /// <returns>The element with the specified identifier if the element is found; otherwise null.</returns>
    IBaseElement GetElementByIdentifier(IBaseElement parentElement,
        string identifier,
        bool acquireLock = true);

    /// <summary>
    /// Gets the element with the specified address.
    /// </summary>
    /// <param name="address">The address of the element to get.</param>
    /// <param name="acquireLock">If true a read lock is acquired; otherwise not.</param>
    /// <returns>The element with the specified address if the element is found; otherwise null.</returns>
    IBaseElement GetElementByAddress(string address,
        bool acquireLock = true);

    /// <summary>
    /// Gets the first element with the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <param name="recurse">If true the tree is recursively searched; otherwise not.</param>
    /// <param name="acquireLock">If true a read lock is acquired; otherwise not.</param>
    /// <returns>The requested element if it exists; otherwise null.</returns>
    IBaseElement GetElementByPredicate(Predicate<IBaseElement> predicate,
        bool recurse = true,
        bool acquireLock = true);

    /// <summary>
    /// Gets all elements with the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match.</param>
    /// <param name="recurse">If true the tree is recursively searched; otherwise not.</param>
    /// <param name="acquireLock">If true a read lock is acquired; otherwise not.</param>
    /// <returns>The elements that match the predicate.</returns>
    IReadOnlyList<IBaseElement> GetElementsByPredicate(Predicate<IBaseElement> predicate,
        bool recurse = true,
        bool acquireLock = true);

    /// <summary>
    /// Creates an element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="type">The type of the element.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IBaseElement CreateElement(IBaseElement parentElement,
        string type,
        string identifier,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a device element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getIdentityFunc">The getidentity service function.</param>
    /// <param name="getTreeFunc">The gettree service function.</param>
    /// <param name="queryTreeFunc">The querytree service function.</param>
    /// <param name="getDataMultiFunc">The getdatamulti service function.</param>
    /// <param name="setDataMultiFunc">The setdatamulti service function.</param>
    /// <param name="getSubscriberListFunc">The getsubscriberlist service function.</param>
    /// <param name="createTreeChangedEventElement">If true a treechanged event element is created; otherwise not.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IDeviceElement CreateDeviceElement(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, GetIdentityResponseServiceData> getIdentityFunc,
        Func<IBaseElement, GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeFunc,
        Func<IBaseElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeFunc,
        Func<IBaseElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiFunc,
        Func<IBaseElement, SetDataMultiRequestServiceData, SetDataMultiResponseServiceData> setDataMultiFunc,
        Func<IBaseElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListFunc,
        bool createTreeChangedEventElement,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a structure element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IStructureElement CreateStructureElement(IBaseElement parentElement,
        string identifier,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates an action service element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IActionServiceElement CreateActionServiceElement(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, ServiceInfo> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates an action service element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IActionServiceElement CreateActionServiceElement(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a getter service element.
    /// </summary>
    /// <typeparam name="TOut">The return type of the service function.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, TOut> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a getter service element.
    /// </summary>
    /// <typeparam name="TOut">The return type of the service function.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, TOut> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a setter service element.
    /// </summary>
    /// <typeparam name="TIn">The parameter type of the service function.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, TIn, ServiceInfo> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a setter service element.
    /// </summary>
    /// <typeparam name="TIn">The parameter type of the service function.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, TIn> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a service element.
    /// </summary>
    /// <typeparam name="TIn">The parameter type of the service function.</typeparam>
    /// <typeparam name="TOut">The return type of the service function.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, TIn, ServiceInfo, TOut> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a service element.
    /// </summary>
    /// <typeparam name="TIn">The parameter type of the service function.</typeparam>
    /// <typeparam name="TOut">The return type of the service function.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="func">The service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, TIn, TOut> func,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates an event element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IEventElement CreateEventElement(IBaseElement parentElement,
        string identifier,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates an event element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="createEventCounterDataElement">If true an event counter data element is created; otherwise not.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IEventElement CreateEventElement(IBaseElement parentElement,
        string identifier,
        bool createEventCounterDataElement,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a simple data element.
    /// A getdata and a setdata service element are created, but no datachanged event element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateSimpleDataElement<T>(IBaseElement parentElement,
        string identifier,
        T value = default,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a simple data element.
    /// A getdata and a setdata service element are created, and optionally a datachanged event element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateSimpleDataElement<T>(IBaseElement parentElement,
        string identifier,
        bool createDataChangedEventElement,
        T value = default,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a simple data element.
    /// Optionally a getdata service, setdata service or datachanged event element are created.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="createGetDataServiceElement">If true a getdata service element is created; otherwise not.</param>
    /// <param name="createSetDataServiceElement">If true a setdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateSimpleDataElement<T>(IBaseElement parentElement,
        string identifier,
        bool createGetDataServiceElement,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        T value = default,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// A getdata service element is created, but no datachanged event element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// A getdata service element is created, but no datachanged event element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="createGetDataServiceElement">If true a getdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        bool createGetDataServiceElement,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="createGetDataServiceElement">If true a getdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadDataElement<T> CreateReadOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        bool createGetDataServiceElement,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createDataChangedEventElement,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createSetDataServiceElement">If true a setdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T> setDataFunc,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T> setDataFunc,
        bool createDataChangedEventElement,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createSetDataServiceElement">If true a setdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IWriteDataElement<T> CreateWriteOnlyDataElement<T>(IBaseElement parentElement,
        string identifier,
        Action<IBaseElement, T> setDataFunc,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createGetDataServiceElement">If true a getdata service element is created; otherwise not.</param>
    /// <param name="createSetDataServiceElement">If true a setdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, ServiceInfo, T> getDataFunc,
        Action<IBaseElement, T, ServiceInfo> setDataFunc,
        bool createGetDataServiceElement,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a data element.
    /// </summary>
    /// <typeparam name="T">The type of the value represented by the element.</typeparam>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="getDataFunc">The getdata service function.</param>
    /// <param name="setDataFunc">The setdata service function.</param>
    /// <param name="createGetDataServiceElement">If true a getdata service element is created; otherwise not.</param>
    /// <param name="createSetDataServiceElement">If true a setdata service element is created; otherwise not.</param>
    /// <param name="createDataChangedEventElement">If true a datachanged event element is created; otherwise not.</param>
    /// <param name="value">The initial value of the element.</param>
    /// <param name="cacheTimeout">The data cache timeout.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    IReadWriteDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
        string identifier,
        Func<IBaseElement, T> getDataFunc,
        Action<IBaseElement, T> setDataFunc,
        bool createGetDataServiceElement,
        bool createSetDataServiceElement,
        bool createDataChangedEventElement,
        T value = default,
        TimeSpan? cacheTimeout = null,
        Format format = null,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Creates a type element.
    /// </summary>
    /// <param name="parentElement">The parent element. Use null if the element has not yet a parent.</param>
    /// <param name="identifier">The identifier of the element.</param>
    /// <param name="format">The format of the element.</param>
    /// <param name="profiles">The profiles of the element.</param>
    /// <param name="uid">The unique identifier of the element.</param>
    /// <param name="isHidden">The hidden status of the element.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not</param>
    /// <returns>The created element.</returns>
    ITypeElement CreateTypeElement(IBaseElement parentElement,
        string identifier,
        Format format,
        IEnumerable<string> profiles = null,
        string uid = null,
        bool isHidden = false,
        bool acquireLock = true);

    /// <summary>
    /// Adds a child element to an element.
    /// </summary>
    /// <param name="parentElement">The parent element.</param>
    /// <param name="element">The child element to add.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not.</param>
    void AddElement(IBaseElement parentElement,
        IBaseElement element,
        bool acquireLock = true);

    /// <summary>
    /// Removes a child element from an element.
    /// </summary>
    /// <param name="parentElement">The parent element.</param>
    /// <param name="element">The child element to remove.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not.</param>
    void RemoveElement(IBaseElement parentElement,
        IBaseElement element,
        bool acquireLock = true);

    /// <summary>
    /// Adds a link to an element.
    /// </summary>
    /// <param name="sourceElement">The source element of the link.</param>
    /// <param name="targetElement">The target element of the link.</param>
    /// <param name="identifier">The identifier of the link; if null the identifier of the target element is used.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not.</param>
    void AddLink(IBaseElement sourceElement,
        IBaseElement targetElement,
        string identifier = null,
        bool acquireLock = true);

    /// <summary>
    /// Removes a link to an element.
    /// </summary>
    /// <param name="sourceElement">The source element of the link.</param>
    /// <param name="targetElement">The target element of the link.</param>
    /// <param name="acquireLock">If true a write lock is acquired; otherwise not.</param>
    void RemoveLink(IBaseElement sourceElement,
        IBaseElement targetElement,
        bool acquireLock = true);
}