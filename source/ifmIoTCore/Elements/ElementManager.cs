using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ifmIoTCore.Elements.EventArguments;
using ifmIoTCore.Elements.Formats;
using ifmIoTCore.Elements.ServiceData.Requests;
using ifmIoTCore.Elements.ServiceData.Responses;
using ifmIoTCore.Exceptions;
using ifmIoTCore.Messages;
using ifmIoTCore.Resources;
using ifmIoTCore.Utilities;

namespace ifmIoTCore.Elements
{
    internal class ElementManager : IElementManager
    {
        private readonly IMessageSender _messageSender;
        private ILogger _logger;
        private readonly Dictionary<string, IBaseElement> _elements = new Dictionary<string, IBaseElement>(StringComparer.OrdinalIgnoreCase);

        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public event EventHandler<TreeChangedEventArgs> TreeChanged;

        public ElementManager(IMessageSender messageSender, ILogger logger)
        {
            _messageSender = messageSender;
            _logger = logger;
        }

        public IDeviceElement CreateRootDeviceElement(string identifier,
            Func<IEnumerable<GetIdentityResponseServiceData.ServerInfo>> serverInfoProvider,
            Func<string> versionProvider)
        {
            var element = new RootElement(identifier,
                this,
                this._messageSender,
                serverInfoProvider,
                versionProvider,
                this._logger);


            AddElementToCache(element);

            TreeChanged += (sender, args) =>
            {
                element.TreeChangedEventElement.RaiseEvent();
            };

            return element;
        }
        
        public IBaseElement GetRootElement()
        {
            var root = (from element in _elements
                where element.Value.Parent == null
                select element.Value).FirstOrDefault();

            return root;
        }

        public IDeviceElement CreateDeviceElement(IBaseElement parentElement,
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
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new DeviceElement(parentElement,
                    identifier,
                    getIdentityFunc,
                    getTreeFunc,
                    queryTreeFunc,
                    getDataMultiFunc,
                    setDataMultiFunc,
                    getSubscriberListFunc,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IStructureElement CreateStructureElement(IBaseElement parentElement,
            string identifier,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new StructureElement(parentElement,
                    identifier,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IActionServiceElement CreateActionServiceElement(IBaseElement parentElement,
            string identifier,
            Action<IActionServiceElement, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new ActionServiceElement(parentElement,
                    identifier,
                    func,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IGetterServiceElement<TOut> CreateGetterServiceElement<TOut>(IBaseElement parentElement,
            string identifier,
            Func<IGetterServiceElement<TOut>, int?, TOut> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new GetterServiceElement<TOut>(parentElement,
                    identifier,
                    func,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public ISetterServiceElement<TIn> CreateSetterServiceElement<TIn>(IBaseElement parentElement,
            string identifier,
            Action<ISetterServiceElement<TIn>, TIn, int?> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new SetterServiceElement<TIn>(parentElement,
                    identifier,
                    func,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IServiceElement<TIn, TOut> CreateServiceElement<TIn, TOut>(IBaseElement parentElement,
            string identifier,
            Func<IServiceElement<TIn, TOut>, TIn, int?, TOut> func,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new ServiceElement<TIn, TOut>(parentElement,
                    identifier,
                    func,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IEventElement CreateEventElement(IBaseElement parentElement,
            string identifier,
            Action<IEventElement, SubscribeRequestServiceData, int?> preSubscribeFunc = null,
            Action<IEventElement, UnsubscribeRequestServiceData, int?> postUnsubscribeFunc = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new EventElement(this,
                    this._messageSender,
                    this._logger,
                    parentElement,
                    identifier,
                    preSubscribeFunc,
                    postUnsubscribeFunc,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementAdded);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IDataElement<T> CreateDataElement<T>(IBaseElement parentElement,
            string identifier,
            Func<IDataElement<T>, T> getDataFunc,
            Action<IDataElement<T>, T> setDataFunc,
            bool createGetDataServiceElement = true,
            bool createSetDataServiceElement = true,
            T value = default,
            TimeSpan? cacheTimeout = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null,
            bool raiseTreeChanged = false)
        {
            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var element = new DataElement<T>(parentElement,
                    identifier,
                    getDataFunc,
                    setDataFunc,
                    createGetDataServiceElement,
                    createSetDataServiceElement,
                    value,
                    cacheTimeout,
                    format, profiles, uid, isHidden, context);

                try
                {
                    AddElement(parentElement, element);
                }
                catch (Exception)
                {
                    element.Dispose();
                    throw;
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element);
                }

                return element;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private void AddElement(IBaseElement parentElement, IBaseElement element)
        {
            if (!parentElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, parentElement.Identifier));
            }

            try
            {
                if (parentElement.References.ForwardReferences?.FirstOrDefault(x => string.Equals(x.Identifier, element.Identifier, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    throw new IoTCoreException(ResponseCodes.AlreadyExists, string.Format(Resource1.ElementAlreadyExists, element.Identifier, parentElement.Identifier));
                }

                parentElement.References.AddForwardReference(parentElement, element, element.Identifier, ReferenceType.Child);

                AddElementToCache(element);
            }
            finally
            {
                parentElement.Lock.ExitWriteLock();
            }
        }

        public void RemoveElement(IBaseElement parentElement, IBaseElement element, bool raiseTreeChanged = false)
        {
            if (parentElement == null) throw new ArgumentNullException(nameof(parentElement));
            if (element == null) throw new ArgumentNullException(nameof(element));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                if (!parentElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, parentElement.Identifier));
                }

                try
                {
                    if (parentElement.References.ForwardReferences?.FirstOrDefault(x => x.TargetElement == element && x.IsChild) == null)
                    {
                        throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotChild, element.Identifier, parentElement.Identifier));
                    }

                    // First remove the element from the cache
                    RemoveElementFromCache(element);

                    // Then remove the reference to the element from the parent
                    parentElement.References.RemoveForwardReference(parentElement, element);

                    // Then dispose the element
                    // Dispose element removes all event handlers and references from the element and disposes all child elements
                    element.Dispose();
                }

                finally
                {
                    parentElement.Lock.ExitWriteLock();
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementRemoved);
                }
            }

            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public void AddLink(IBaseElement sourceElement, IBaseElement targetElement, string identifier = null, bool raiseTreeChanged = true)
        {
            if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
            if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                if (!sourceElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, sourceElement.Identifier));
                }

                try
                {
                    if (!targetElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                    {
                        throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, targetElement.Identifier));
                    }

                    //    try
                    //    {
                    //        identifier ??= element.Identifier;

                    //        // Check if reference to element already exists
                    //        if (parentElement.References.ForwardReferences?.FirstOrDefault(x => x.TargetElement == element) != null ||
                    //            parentElement.References.ForwardReferences?.FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase)) != null)
                    //        {
                    //            throw new IoTCoreException(ResponseCodes.AlreadyExists, string.Format(Resource1.ElementAlreadyExists, parentElement.Identifier, identifier));
                    //        }

                    //        //// Check for circular dependency
                    //        //if (IsCircularDependency(this, element))
                    //        //{
                    //        //    throw new IoTCoreException(ResponseCodes.NotAllowed, string.Format(Resource1.AddAncestorElementNotAllowed, identifier, Identifier));
                    //        //}

                    //        // Then set child context
                    //        element.References.AddInverseReference(element, parentElement, identifier, ReferenceType.Link);

                    //        // Then add a reference to the element 
                    //        parentElement.References.AddForwardReference(parentElement, element, identifier, ReferenceType.Link);
                    //    }
                    //    finally
                    //    {
                    //        element.Lock.ExitWriteLock();
                    //    }
                }

                finally
                {
                    sourceElement.Lock.ExitWriteLock();
                }

                if (raiseTreeChanged)
                {
                    RaiseTreeChanged(sourceElement, targetElement, TreeChangedAction.ElementAdded);
                }
            }

            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public void RemoveLink(IBaseElement sourceElement, IBaseElement targetElement, bool raiseTreeChanged = true)
        {
            if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
            if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                //if (!parentElement.Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                //{
                //    throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, parentElement.Identifier));
                //}

                //try
                //{
                //}

                //finally
                //{
                //    parentElement.Lock.ExitWriteLock();
                //}

                //if (raiseTreeChanged)
                //{
                //    RaiseTreeChanged(parentElement, element, TreeChangedAction.ElementRemoved);
                //}
            }

            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public IBaseElement GetElementByAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;

            if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                address = Helpers.RemoveDeviceName(address);
                _elements.TryGetValue(address, out var element);
                return element;
            }

            finally
            {
                Lock.ExitReadLock();
            }
        }

        private void AddElementToCache(IBaseElement element)
        {
            _elements.Add(element.Address, element);

            // Add any referenced elements
            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                AddElementToCache(item.TargetElement);
            }
        }

        private void RemoveElementFromCache(IBaseElement element)
        {
            _elements.Remove(element.Address);

            // Remove any referenced elements
            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                RemoveElementFromCache(item.TargetElement);
            }
        }

        public void RaiseTreeChanged(IBaseElement parentElement = null, IBaseElement childElement = null, TreeChangedAction action = TreeChangedAction.TreeChanged)
        {
            var treeChangedEventArgs = new TreeChangedEventArgs
            {
                Action = action,
                ParentElement = parentElement,
                ChildElement = childElement
            };
            TreeChanged.Raise(this, treeChangedEventArgs);
        }
    }
}
