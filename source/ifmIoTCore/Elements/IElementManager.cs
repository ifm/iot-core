using System;
using System.Collections.Generic;
using System.Threading;
using ifmIoTCore.Elements.EventArguments;
using ifmIoTCore.Elements.Formats;
using ifmIoTCore.Elements.ServiceData.Requests;
using ifmIoTCore.Elements.ServiceData.Responses;

namespace ifmIoTCore.Elements
{
    /// <summary>
    /// Provides functionality to interact with the element manager
    /// </summary>
    public interface IElementManager
    {
        /// <summary>
        /// Locks access
        /// </summary>
        ReaderWriterLockSlim Lock { get; }

        /// <summary>
        /// Creates a new device element and adds a child reference to this element to the parent element
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="getIdentityFunc">The getidentity function</param>
        /// <param name="getTreeFunc">The gettree function</param>
        /// <param name="queryTreeFunc">The querytree function</param>
        /// <param name="getDataMultiFunc">The getdatamulti function</param>
        /// <param name="setDataMultiFunc">The setdatamulti function</param>
        /// <param name="getSubscriberListFunc">The getsubscriberlist function</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        IDeviceElement CreateDeviceElement(IBaseElement parentElement,
            string identifier,
            Func<IDeviceElement, GetIdentityResponseServiceData> getIdentityFunc,
            Func<IDeviceElement, GetTreeRequestServiceData, GetTreeResponseServiceData> getTreeFunc,
            Func<IDeviceElement, QueryTreeRequestServiceData, QueryTreeResponseServiceData> queryTreeFunc,
            Func<IDeviceElement, GetDataMultiRequestServiceData, GetDataMultiResponseServiceData> getDataMultiFunc,
            Action<IDeviceElement, SetDataMultiRequestServiceData> setDataMultiFunc,
            Func<IDeviceElement, GetSubscriberListRequestServiceData, GetSubscriberListResponseServiceData> getSubscriberListFunc,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new structure element and adds a child reference to this element to the parent element
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        IStructureElement CreateStructureElement(IBaseElement parentElement,
            string identifier,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new action service element and adds a child reference to this element to the parent element
        /// An action service has no input argument and returns no output value
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="func">The service function</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        IActionServiceElement CreateActionServiceElement(IBaseElement parentElement,
            string identifier,
            Action<IActionServiceElement, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new getter service element and adds a child reference to this element to the parent element
        /// A getter service has no input argument and returns an output value
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="func">The service function</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement,
            string identifier,
            Func<IGetterServiceElement<TOut>, int?, TOut> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new setter service element and adds a child reference to this element to the parent element
        /// A setter service has an input argument and returns no output value
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="func">The service function</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement,
            string identifier,
            Action<ISetterServiceElement<TIn>, TIn, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new service element and adds a child reference to this element to the parent element
        /// A service has an input argument and returns an output value
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="func">The service function</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement,
            string identifier,
            Func<IServiceElement<TIn, TOut>, TIn, int?, TOut> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new event element and adds a child reference to this element to the parent element
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="preSubscribeFunc">The pre-subscribe function; If provided the function will be called before the subscription is created</param>
        /// <param name="postUnsubscribeFunc">The post-unsubscribe function; If provided the function will be called after the subscription is removed</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">A treechanged event is raised after the element is created and added to the tree; otherwise not</param>
        /// <returns>The created element</returns>
        IEventElement CreateEventElement(IBaseElement parentElement,
            string identifier,
            Action<IEventElement, SubscribeRequestServiceData, int?> preSubscribeFunc = null,
            Action<IEventElement, UnsubscribeRequestServiceData, int?> postUnsubscribeFunc = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Creates a new data element and adds a child reference to this element to the parent element
        /// </summary>
        /// <param name="parentElement">The parent element of the newly created element</param>
        /// <param name="identifier">The identifier of the element</param>
        /// <param name="getDataFunc">The getdata function; if null the default implementation will be used</param>
        /// <param name="setDataFunc">The setdata function; if null the default implementation will be used</param>
        /// <param name="createGetDataServiceElement">If true a getdata child service element is created; otherwise not</param>
        /// <param name="createSetDataServiceElement">If true a setdata child service element is created; otherwise not</param>
        /// <param name="value">The initial value</param>
        /// <param name="cacheTimeout">The time span after which the value cache expires</param>
        /// <param name="format">The format of the element</param>
        /// <param name="profiles">The profiles of the element</param>
        /// <param name="uid">The unique identifier of the element</param>
        /// <param name="isHidden">The hidden status of the element</param>
        /// <param name="context">The context of the element (local, remote)</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised after the element is created; otherwise not</param>
        /// <returns>The created element</returns>
        IDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
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
            object context = null,
            bool raiseTreeChanged = false);

        /// <summary>
        /// Removes the child reference to the element from the parent element and disposes the element
        /// </summary>
        /// <param name="parentElement">The element from which the element is removed</param>
        /// <param name="element">The element to remove</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised after the element is removed; otherwise not</param>
        void RemoveElement(IBaseElement parentElement, IBaseElement element, bool raiseTreeChanged = false);

        /// <summary>
        /// Adds a link reference from the source element to the target element
        /// </summary>
        /// <param name="sourceElement">The source element which links the element</param>
        /// <param name="targetElement">The linked element</param>
        /// <param name="identifier">The identifier for the link; if null the identifier of the linked element is used</param>
        /// <param name="raiseTreeChanged">If true a tree-changed event is raised; otherwise not</param>
        void AddLink(IBaseElement sourceElement, IBaseElement targetElement, string identifier = null, bool raiseTreeChanged = true);

        /// <summary>
        /// Removes a link reference from the source element to the target element
        /// </summary>
        /// <param name="sourceElement">The parent element which links the element</param>
        /// <param name="targetElement">The linked element</param>
        /// <param name="raiseTreeChanged">If true a treechanged event is raised; otherwise not</param>
        void RemoveLink(IBaseElement sourceElement, IBaseElement targetElement, bool raiseTreeChanged = true);

        /// <summary>
        /// Gets the element with the specified address
        /// </summary>
        /// <param name="address">The address of the element</param>
        /// <returns>The requested element if it exists; otherwise null</returns>
        IBaseElement GetElementByAddress(string address);

        /// <summary>
        /// Raises a treechanged event
        /// </summary>
        /// <param name="parentElement">The parent element affected by the change; null, if multiple changes done (action == treechanged)</param>
        /// <param name="childElement">The child element affected by the change; null, if multiple changes done (action == treechanged)</param>
        /// <param name="action">The action that caused the event</param>
        void RaiseTreeChanged(IBaseElement parentElement = null, IBaseElement childElement = null, TreeChangedAction action = TreeChangedAction.TreeChanged);

        /// <summary>
        /// The treechanged event handler
        /// </summary>
        event EventHandler<TreeChangedEventArgs> TreeChanged;
        
        /// <summary>
        /// Gets the root element.
        /// </summary>
        IBaseElement GetRootElement();
    }
}