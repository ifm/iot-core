namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using Common;
    using EventArguments;
    using Exceptions;
    using Logger;
    using Messages;
    using NetAdapter;
    using Resources;
    using ServiceData;
    using ServiceData.Events;
    using Utilities;

    internal class ElementManager : IElementManager
    {
        private readonly IClientNetAdapterManager _clientNetAdapterManager;
        private readonly IServerNetAdapterManager _serverNetAdapterManager;
        private readonly ILogger _logger;

        private readonly Dictionary<string, IBaseElement> _elements = new Dictionary<string, IBaseElement>(StringComparer.OrdinalIgnoreCase);
        private IDeviceElement _rootElement;

        public ElementManager(IClientNetAdapterManager clientNetAdapterManager,
            IServerNetAdapterManager serverNetAdapterManager,
            ILogger logger)
        {
            _clientNetAdapterManager = clientNetAdapterManager;
            _serverNetAdapterManager = serverNetAdapterManager;
            _logger = logger;
        }

        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public IDeviceElement Root
        {
            get => _rootElement;
            set
            {
                if (_rootElement == value) return;

                if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
                {
                    throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
                }

                try
                {
                    if (_rootElement != null)
                    {
                        _rootElement.ElementAdded -= OnElementAdded;
                        _rootElement.ElementRemoved -= OnElementRemoved;
                        _rootElement.LinkAdded -= OnLinkAdded;
                        _rootElement.LinkRemoved -= OnLinkRemoved;
                    }

                    _elements.Clear();

                    _rootElement = value;
                    _rootElement.ElementAdded += OnElementAdded;
                    _rootElement.ElementRemoved += OnElementRemoved;
                    _rootElement.LinkAdded += OnLinkAdded;
                    _rootElement.LinkRemoved += OnLinkRemoved;

                    AddElement(null, _rootElement);
                }
                finally
                {
                    Lock.ExitWriteLock();
                }
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

        public IEnumerable<IBaseElement> GetElementsByProfile(string profile)
        {
            if (string.IsNullOrEmpty(profile)) return null;

            if (!Lock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                return _elements.Values.Where(element => element.HasProfile(profile)).ToList();
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        private void OnElementAdded(object _, TreeChangedEventArgs e)
        {
            AddElement(e.ParentElement, e.ChildElement);
        }

        private void OnElementRemoved(object _, TreeChangedEventArgs e)
        {
            RemoveElement(e.ParentElement, e.ChildElement);
        }

        private void OnLinkAdded(object _, TreeChangedEventArgs e)
        {
            AddLinkedElement(e.ParentElement, e.ChildElement, e.Identifier);
        }

        private void OnLinkRemoved(object _, TreeChangedEventArgs e)
        {
            RemoveLinkedElement(e.ParentElement, e.ChildElement, e.Identifier);
        }

        private void AddElement(IBaseElement parentElement, IBaseElement childElement)
        {
            if (childElement == null) throw new ArgumentNullException(nameof(childElement));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                SetRaiseEventFunc(childElement);

                if (parentElement != null)
                {
                    var parentAddresses = GetAllAddressesFromCache(parentElement);
                    foreach (var parentAddress in parentAddresses)
                    {
                        AddElementToCache(parentAddress, childElement);
                    }
                }
                else
                {
                    AddElementToCache(null, childElement);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private void RemoveElement(IBaseElement parentElement, IBaseElement childElement)
        {
            if (parentElement == null) throw new ArgumentNullException(nameof(parentElement));
            if (childElement == null) throw new ArgumentNullException(nameof(childElement));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var parentAddresses = GetAllAddressesFromCache(parentElement);
                foreach (var parentAddress in parentAddresses)
                {
                    RemoveElementFromCache(parentAddress, childElement);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private void AddLinkedElement(IBaseElement sourceElement, IBaseElement targetElement, string identifier)
        {
            if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
            if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var parentAddresses = GetAllAddressesFromCache(sourceElement);
                foreach (var parentAddress in parentAddresses)
                {
                    AddLinkedElementToCache(parentAddress, targetElement, identifier);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private void RemoveLinkedElement(IBaseElement sourceElement, IBaseElement targetElement, string identifier)
        {
            if (sourceElement == null) throw new ArgumentNullException(nameof(sourceElement));
            if (targetElement == null) throw new ArgumentNullException(nameof(targetElement));

            if (!Lock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var parentAddresses = GetAllAddressesFromCache(sourceElement);
                foreach (var parentAddress in parentAddresses)
                {
                    RemoveLinkedElementFromCache(parentAddress, targetElement, identifier);
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        private IEnumerable<string> GetAllAddressesFromCache(IBaseElement element)
        {
            var addresses = new List<string>();
            foreach (var item in _elements)
            {
                if (item.Value == element)
                {
                    addresses.Add(item.Key);
                }
            }
            return addresses;
            //return _elements.Where(x => x.Value == element).Select(x => x.Key);
        }

        private void AddElementToCache(string parentAddress, IBaseElement element)
        {
            var address = parentAddress != null ? Helpers.CreateAddress(parentAddress, element.Identifier) : element.Address;
            _elements.Add(address, element);

            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                AddElementToCache(address, item.TargetElement);
            }
        }

        private void AddLinkedElementToCache(string parentAddress, IBaseElement element, string identifier)
        {
            var address = Helpers.CreateAddress(parentAddress, identifier);
            _elements.Add(address, element);

            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                AddLinkedElementToCache(address, item.TargetElement, item.TargetElement.Identifier);
            }
        }

        private void RemoveElementFromCache(string parentAddress, IBaseElement element)
        {
            var address = Helpers.CreateAddress(parentAddress, element.Identifier);
            _elements.Remove(address);

            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                RemoveElementFromCache(address, item.TargetElement);
            }
        }

        private void RemoveLinkedElementFromCache(string parentAddress, IBaseElement element, string identifier)
        {
            var address = Helpers.CreateAddress(parentAddress, identifier);
            if (address != element.Address)
            {
                _elements.Remove(address);
            }

            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                RemoveLinkedElementFromCache(address, item.TargetElement, item.TargetElement.Identifier);
            }
        }

        private void SetRaiseEventFunc(IBaseElement element)
        {
            if (element is IEventElement eventElement)
            {
                if (eventElement.RaiseEventFunc == null)
                {
                    eventElement.RaiseEventFunc = SendEvents;
                }
            }

            if (element.ForwardReferences == null) return;
            foreach (var item in element.ForwardReferences)
            {
                SetRaiseEventFunc(item.TargetElement);
            }
        }

        private void SendEvents(IEventElement eventElement, SubscriptionEventArgs args)
        {
            if (!eventElement.SubscriptionsLock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                foreach (var subscription in eventElement.Subscriptions)
                {
                    try
                    {
                        var eventServiceData = CreateEventServiceData(eventElement, subscription, args);

                        if (!TryInvokeCallbackFunc(eventElement, subscription))
                        {
                            if (!TrySendFromServerToConnectedClient(subscription, eventServiceData))
                            {
                                if (!TrySendFromClientToServer(subscription, eventServiceData))
                                {
                                    if (!TryInvokeService(subscription, eventServiceData))
                                    {
                                        _logger?.Error(string.Format(Resource1.SendEventFailed, subscription.Callback, "No suitable method to dispatch event available"));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger?.Error(string.Format(Resource1.SendEventFailed, subscription.Callback, e.Message));
                    }
                }
            }
            finally
            {
                eventElement.SubscriptionsLock.ExitReadLock();
            }
        }

        private static bool TryInvokeCallbackFunc(IEventElement eventElement, Subscription subscription)
        {
            if (subscription.CallbackFunc != null)
            {
                subscription.CallbackFunc.Invoke(eventElement);

                return true;
            }

            return false;
        }

        private bool TrySendFromServerToConnectedClient(Subscription subscription, EventServiceData eventServiceData)
        {
            // If authority is provided event is not for a connected client
            if (subscription.Callback.IndexOf("://", StringComparison.Ordinal) != -1)
            {
                return false;
            }

            var scheme = subscription.Callback.Left(":");
            if (scheme == null)
            {
                return false;
            }

            var target = subscription.Callback.Right(":");
            if (target == null)
            {
                return false;
            }

            var path = target.Left('?');

            var query = target.Right('?');
            if (query == null)
            {
                return false;
            }

            var clientId = HttpUtility.ParseQueryString(query).Get("clientid");
            if (clientId == null)
            {
                return false;
            }

            var servers = _serverNetAdapterManager.FindServerNetAdapters(scheme);
            foreach (var server in servers)
            {
                if (server is IConnectedServerNetAdapter connectedServer)
                {
                    if (connectedServer.IsClientConnected(clientId))
                    {
                        var eventMessage = new Message(RequestCodes.Event,
                            subscription.Id,
                            path,
                            Helpers.VariantFromObject(eventServiceData));

                        connectedServer.SendEvent(clientId, eventMessage);

                        return true;
                    }
                }
            }

            return false;
        }

        private bool TrySendFromClientToServer(Subscription subscription, EventServiceData eventServiceData)
        {
            if (Uri.TryCreate(subscription.Callback, UriKind.Absolute, out var uri))
            {
                var eventMessage = new Message(RequestCodes.Event,
                    subscription.Id,
                    uri.LocalPath,
                    Helpers.VariantFromObject(eventServiceData));

                var client = _clientNetAdapterManager.CreateClientNetAdapter(new Uri(subscription.Callback));
                client.SendEvent(eventMessage);

                return true;
            }

            return false;
        }

        private bool TryInvokeService(Subscription subscription, EventServiceData eventServiceData)
        {
            var element = GetElementByAddress(Helpers.RemoveDeviceName(subscription.Callback));
            if (element is IServiceElement serviceElement)
            {
                serviceElement.Invoke(Helpers.VariantFromObject(eventServiceData));

                return true;
            }

            return false;
        }

        private EventServiceData CreateEventServiceData(IEventElement eventElement, Subscription subscription, SubscriptionEventArgs args)
        {
            var payload = new Dictionary<string, CodeDataPair>();
            if (subscription.DataToSend != null && subscription.DataToSend.Count > 0)
            {
                foreach (var item in subscription.DataToSend)
                {
                    var element = GetElementByAddress(Helpers.RemoveDeviceName(item));
                    if (element is IDataElement dataElement)
                    {
                        try
                        {
                            payload.Add(item, new CodeDataPair(ResponseCodes.Success, dataElement.Value));
                        }
                        catch (IoTCoreException iotCoreException)
                        {
                            payload.Add(item, new CodeDataPair(iotCoreException.ResponseCode, null));
                        }
                        catch
                        {
                            payload.Add(item, new CodeDataPair(ResponseCodes.InternalError, null));
                        }
                    }
                    else
                    {
                        payload.Add(item, new CodeDataPair(ResponseCodes.NotFound, null));
                    }
                }
            }

            return new EventServiceData(args.EventNumber,
                Helpers.AddDeviceName(eventElement.Address, this._rootElement.Identifier),
                payload.Any() ? payload : null,
                subscription.Id);
        }
    }
}