namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Exceptions;
    using Formats;
    using Messages;
    using Resources;
    using ServiceData;
    using ServiceData.Events;
    using ServiceData.Requests;
    using ServiceData.Responses;
    using Utilities;

    /// <summary>
    /// The subscription class
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// The url to which the IoTCore is sending events
        /// </summary>
        public readonly string Callback;

        /// <summary>
        /// The local callback for events
        /// </summary>
        public readonly Action<IEventElement> CallbackFunc;

        /// <summary>
        /// List of data element addresses, whose values are sent with the event
        /// </summary>
        public readonly List<string> DataToSend;

        /// <summary>
        /// If true the subscription is persistent; otherwise not 
        /// </summary>
        public readonly bool Persist;

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        public readonly int SubscriptionId;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="callback">The url to which the IoTCore is sending events</param>
        /// <param name="callbackFunc">The local callback for events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">If true the subscription is persistent; otherwise not</param>
        /// <param name="subscriptionId">The id which identifies the subscription</param>
        public Subscription(string callback, Action<IEventElement> callbackFunc, List<string> dataToSend, bool persist, int subscriptionId)
        {
            Callback = callback;
            CallbackFunc = callbackFunc;
            DataToSend = dataToSend;
            Persist = persist;
            SubscriptionId = subscriptionId;
        }
    }

    internal class EventElement : BaseElement, IEventElement
    {
        private IElementManager _elementManager;
        private IMessageSender _messageSender;
        private ILogger _logger;

        private Action<IEventElement, SubscribeRequestServiceData, int?> _preSubscribeFunc;
        private Action<IEventElement, UnsubscribeRequestServiceData, int?> _postUnsubscribeFunc;

        public IDictionary<int, Subscription> Subscriptions { get; } = new Dictionary<int, Subscription>();

        public ReaderWriterLockSlim SubscriptionsLock { get; } = new ReaderWriterLockSlim();

        private static int _eventNumber;

        public EventElement(IElementManager elementManager,
            IMessageSender messageSender,
            ILogger logger,
            IBaseElement parent,
            string identifier,
            Action<IEventElement, SubscribeRequestServiceData, int?> preSubscribeFunc = null,
            Action<IEventElement, UnsubscribeRequestServiceData, int?> postUnsubscribeFunc = null,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false,
            object context = null) : 
            base(parent, Identifiers.Event, identifier, format, profiles, uid, isHidden, context)
        {
            _elementManager = elementManager;
            _messageSender = messageSender;
            _logger = logger;

            _preSubscribeFunc = preSubscribeFunc;
            _postUnsubscribeFunc = postUnsubscribeFunc;

            IServiceElement<SubscribeRequestServiceData, SubscribeResponseServiceData> subscribeServiceElement = null;
            ISetterServiceElement<UnsubscribeRequestServiceData> unsubscribeServiceElement = null;

            try
            {
                subscribeServiceElement = CreateSubscribeServiceElement();
                unsubscribeServiceElement = CreateUnsubscribeServiceElement();
            }
            catch
            {
                subscribeServiceElement?.Dispose();
                unsubscribeServiceElement?.Dispose();

                throw;
            }
        }

        private IServiceElement<SubscribeRequestServiceData, SubscribeResponseServiceData> CreateSubscribeServiceElement()
        {
            var subscribeServiceElement = new ServiceElement<SubscribeRequestServiceData, SubscribeResponseServiceData>(this, Identifiers.Subscribe, SubscribeServiceFunc);
            References.AddForwardReference(this, subscribeServiceElement, subscribeServiceElement.Identifier, ReferenceType.Child);
            return subscribeServiceElement;
        }

        private ISetterServiceElement<UnsubscribeRequestServiceData> CreateUnsubscribeServiceElement()
        {
            var unsubscribeServiceElement = new SetterServiceElement<UnsubscribeRequestServiceData>(this, Identifiers.Unsubscribe, UnsubscribeServiceFunc);
            References.AddForwardReference(this, unsubscribeServiceElement, unsubscribeServiceElement.Identifier, ReferenceType.Child);
            return unsubscribeServiceElement;
        }

        private SubscribeResponseServiceData SubscribeServiceFunc(IServiceElement _, SubscribeRequestServiceData data, int? cid)
        {
            return Subscribe(data, cid);
        }

        public SubscribeResponseServiceData Subscribe(SubscribeRequestServiceData data, int? cid = null)
        {
            ValidateSubscribeRequestData(data);

            _preSubscribeFunc?.Invoke(this, data, cid);

            var subscriptionId = GenerateSubscriptionId(data.SubscriptionId, cid);

            return new SubscribeResponseServiceData(AddSubscription(new Subscription(data.Callback, null, data.DataToSend, data.Persist, subscriptionId)));
        }

        public int Subscribe(Action<IEventElement> callbackFunc)
        {
            _preSubscribeFunc?.Invoke(this, new SubscribeRequestServiceData(null), null);

            var subscriptionId = GenerateSubscriptionId(null, null);

            return AddSubscription(new Subscription(null, callbackFunc, null, false, subscriptionId));
        }

        private int GenerateSubscriptionId(int? sid, int? cid)
        {
            const int autoCreateSubscriptionId = -1;

            var id = autoCreateSubscriptionId;
            if (sid != null)
            {
                id = sid.Value;
            }
            else if (cid != null)
            {
                id = cid.Value;
            }
            if (id == autoCreateSubscriptionId)
            {
                id = 0;
                while (id < int.MaxValue)
                {
                    if (Subscriptions.Keys.Any(x => x == id))
                    {
                        id++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return id;
        }

        private int AddSubscription(Subscription subscription)
        {
            if (!SubscriptionsLock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            try
            {
                if (Subscriptions.ContainsKey(subscription.SubscriptionId))
                {
                    Subscriptions.Remove(subscription.SubscriptionId);
                }
                Subscriptions.Add(subscription.SubscriptionId, subscription);
            }
            finally
            {
                SubscriptionsLock.ExitWriteLock();
            }

            return subscription.SubscriptionId;
        }

        private static void ValidateSubscribeRequestData(SubscribeRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, Identifiers.Subscribe));
            }

            //if (string.IsNullOrEmpty(data.Callback))
            //{
            //    throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ParameterNotSpecified, Identifiers.Callback));
            //}
        }

        private void UnsubscribeServiceFunc(IServiceElement _, UnsubscribeRequestServiceData data, int? cid)
        {
            Unsubscribe(data, cid);
        }

        public void Unsubscribe(UnsubscribeRequestServiceData data, int? cid = null)
        {
            ValidateUnsubscribeRequestData(data);

            RemoveSubscription(data, cid);

            _postUnsubscribeFunc?.Invoke(this, data, cid);
        }

        public void Unsubscribe(int subscriptionId)
        {
            Subscriptions.Remove(subscriptionId);
        }

        public void Unsubscribe(Action<IEventElement> callbackFunc)
        {
            if (Subscriptions.Values.Any(x => x.CallbackFunc == callbackFunc))
            {
                Subscriptions.RemoveAll(x => x.CallbackFunc == callbackFunc);
            }
        }

        private void RemoveSubscription(UnsubscribeRequestServiceData data, int? cid = null)
        {
            if (!SubscriptionsLock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }
            try
            {
                if (data.SubscriptionId != null)
                {
                    if (!Subscriptions.Remove(data.SubscriptionId.Value))
                    {
                        throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.SubscriptionNotFound, data.SubscriptionId.Value));
                    }
                }
                else
                {
                    if (Subscriptions.Values.Any(x => x.Callback == data.Callback && x.SubscriptionId == cid))
                    {
                        Subscriptions.RemoveAll(x => x.Callback == data.Callback && x.SubscriptionId == cid);
                    }
                    else
                    {
                        var msg = $"{data.Callback} - {cid}";
                        throw new ServiceException(ResponseCodes.DataInvalid, string.Format(Resource1.SubscriptionNotFound, msg));
                    }
                }
            }
            finally
            {
                SubscriptionsLock.ExitWriteLock();
            }
        }

        private static void ValidateUnsubscribeRequestData(UnsubscribeRequestServiceData data)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, Identifiers.Unsubscribe));
            }

            //if (string.IsNullOrEmpty(data.Callback))
            //{
            //    throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ParameterNotSpecified, Identifiers.Callback));
            //}
        }

        public void RaiseEvent()
        {
            if (!SubscriptionsLock.TryEnterReadLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, string.Format(Resource1.ElementLocked, Identifier));
            }

            try
            {
                foreach (var subscription in Subscriptions.Values)
                {
                    var payload = new Dictionary<string, CodeDataPair>();
                    if (subscription.DataToSend != null && subscription.DataToSend.Count > 0)
                    {
                        if (!_elementManager.Lock.TryEnterReadLock(Configuration.Settings.Timeout))
                        {
                            throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
                        }
                        try
                        {
                            foreach (var item in subscription.DataToSend)
                            {
                                // ToDo: Use element manager GetElementByAddress. How avoid deadlock in treechanged event?
                                var element = _elementManager.GetElementByAddress(Helpers.RemoveDeviceName(item));
                                if (element is IDataElement dataElement)
                                {
                                    try
                                    {
                                        payload.Add(item, new CodeDataPair(ResponseCodes.Success, dataElement.Value));
                                    }
                                    catch (IoTCoreException iotCoreException)
                                    {
                                        payload.Add(item, new CodeDataPair(iotCoreException.Code, null));
                                    }
                                    catch
                                    {
                                        payload.Add(item, new CodeDataPair(ResponseCodes.InternalError, null));
                                    }
                                }
                            }
                        }
                        finally
                        {
                            _elementManager.Lock.ExitReadLock();
                        }
                    }
                    
                    if (Uri.TryCreate(subscription.Callback, UriKind.Absolute, out var uri))
                    {
                        var eventMessage = new EventMessage(subscription.SubscriptionId,
                            uri.LocalPath,
                            Helpers.ToJson(new EventServiceData(Interlocked.Increment(ref _eventNumber), Address, payload, subscription.SubscriptionId)));
                        try
                        {
                            _logger?.Debug($"Send event to {subscription.Callback}");
                            _messageSender.SendEvent(new Uri(subscription.Callback), eventMessage);
                        }
                        catch (Exception e)
                        {
                            _logger?.Error(string.Format(Resource1.SendEventFailed, subscription.Callback, e.Message));
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(subscription.Callback))
                        {
                            var element = _elementManager.GetElementByAddress(Helpers.RemoveDeviceName(subscription.Callback));
                            if (element is ISetterServiceElement<EventServiceData> serviceElement)
                            {
                                serviceElement.Invoke(new EventServiceData(Interlocked.Increment(ref _eventNumber), Address, payload, subscription.SubscriptionId), null);
                            }
                        }
                    }

                    subscription.CallbackFunc?.Invoke(this);
                }
            }

            finally
            {
                SubscriptionsLock.ExitReadLock();
            }
        }

        protected override void Dispose(bool disposing)
        {
            _elementManager = null;
            _messageSender = null;
            _logger = null;

            _preSubscribeFunc = null;
            _postUnsubscribeFunc = null;

            base.Dispose(disposing);
        }
    }
}