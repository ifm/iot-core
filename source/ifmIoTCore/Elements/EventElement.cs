namespace ifmIoTCore.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Common;
    using Common.Variant;
    using EventArguments;
    using Exceptions;
    using Messages;
    using Resources;
    using ServiceData.Requests;
    using ServiceData.Responses;
    using Utilities;


    public class EventElement : BaseElement, IEventElement
    {
        public IServiceElement SubscribeServiceElement { get; }
        public IServiceElement UnsubscribeServiceElement { get; }

        public Action<IEventElement, SubscriptionEventArgs> RaiseEventFunc { get; set; }

        public ReaderWriterLockSlim SubscriptionsLock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public IEnumerable<Subscription> Subscriptions => _subscriptions.Values;
        protected int SubscriptionCount => _subscriptions.Count;
        private readonly Dictionary<int, Subscription> _subscriptions = new Dictionary<int, Subscription>();

        private static int _eventNumber;

        public EventElement(string identifier,
            Format format = null,
            List<string> profiles = null,
            string uid = null,
            bool isHidden = false) : 
            base(Identifiers.Event, identifier, format, profiles, uid, isHidden)
        {
            AddChild(SubscribeServiceElement = new ServiceElement(Identifiers.Subscribe, SubscribeServiceFunc));
            AddChild(UnsubscribeServiceElement = new ServiceElement(Identifiers.Unsubscribe, UnsubscribeServiceFunc));
        }

        private Variant SubscribeServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = Helpers.VariantToObject<SubscribeRequestServiceData>(data);

            var result = Subscribe(request, cid);

            return Helpers.VariantFromObject(result);
        }

        public virtual SubscribeResponseServiceData Subscribe(SubscribeRequestServiceData data, int? cid)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, Identifiers.Subscribe));
            }
            if (data.Callback == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.InvalidArgument, Identifiers.Callback));
            }

            var id = AddSubscription(data, cid);
            return new SubscribeResponseServiceData(id);
        }

        protected int AddSubscription(SubscribeRequestServiceData data, int? cid)
        {
            if (!SubscriptionsLock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var id = GenerateSubscriptionId(data.SubscriptionId, cid);
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions.Remove(id);
                }
                var subscription = new Subscription(id, data.Callback, null, data.DataToSend, data.Persist);
                _subscriptions.Add(id, subscription);

                return id;
            }
            finally
            {
                SubscriptionsLock.ExitWriteLock();
            }
        }

        public virtual void Subscribe(Action<IEventElement> callbackFunc)
        {
            AddSubscription(callbackFunc);
        }

        protected void AddSubscription(Action<IEventElement> callbackFunc)
        {
            if (!SubscriptionsLock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var id = GenerateSubscriptionId(null, null);
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions.Remove(id);
                }
                var subscription = new Subscription(id, null, callbackFunc, null, false);
                _subscriptions.Add(id, subscription);
            }
            finally
            {
                SubscriptionsLock.ExitWriteLock();
            }
        }

        private int GenerateSubscriptionId(int? subscriptionId, int? cid)
        {
            const int autoCreateSubscriptionId = -1;

            var id = autoCreateSubscriptionId;
            if (subscriptionId != null)
            {
                id = subscriptionId.Value;
            }
            else if (cid != null)
            {
                id = cid.Value;
            }
            if (id == autoCreateSubscriptionId)
            {
                id = 0;
                while (id < short.MaxValue)
                {
                    if (_subscriptions.Values.Any(x => x.Id == id))
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

        private Variant UnsubscribeServiceFunc(IServiceElement _, Variant data, int? cid)
        {
            var request = Helpers.VariantToObject<UnsubscribeRequestServiceData>(data);

            Unsubscribe(request, cid);
            return null;
        }

        public virtual void Unsubscribe(UnsubscribeRequestServiceData data, int? cid = null)
        {
            if (data == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ServiceDataEmpty, Identifiers.Unsubscribe));
            }

            RemoveSubscription(data, cid);
        }

        protected void RemoveSubscription(UnsubscribeRequestServiceData data, int? cid)
        {
            if (!SubscriptionsLock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                if (data.SubscriptionId != null)
                {
                    if (!_subscriptions.Remove(data.SubscriptionId.Value))
                    {
                        throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.SubscriptionNotFound, data.SubscriptionId.Value));
                    }
                }
                else
                {
                    if (_subscriptions.Values.Any(x => x.Callback == data.Callback && x.Id == cid))
                    {
                        _subscriptions.RemoveAll(x => x.Callback == data.Callback && x.Id == cid);
                    }
                    else
                    {
                        throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.SubscriptionNotFound, $"{data.Callback} - {cid}"));
                    }
                }
            }
            finally
            {
                SubscriptionsLock.ExitWriteLock();
            }
        }

        public virtual void Unsubscribe(Action<IEventElement> callbackFunc)
        {
            RemoveSubscription(callbackFunc);
        }

        protected void RemoveSubscription(Action<IEventElement> callbackFunc)
        {
            if (!SubscriptionsLock.TryEnterWriteLock(Configuration.Settings.Timeout))
            {
                throw new IoTCoreException(ResponseCodes.Locked, Resource1.ElementManagerLocked);
            }

            try
            {
                var subscriptionId = _subscriptions.FirstOrDefault(x => x.Value.CallbackFunc == callbackFunc).Key;
                _subscriptions.Remove(subscriptionId);
            }
            finally
            {
                SubscriptionsLock.ExitWriteLock();
            }
        }

        public bool HasSubscription(Action<IEventElement> callbackFunc)
        {
            return _subscriptions.Any(item => item.Value.CallbackFunc == callbackFunc);
        }

        public void RaiseEvent()
        {
            RaiseEventFunc?.Invoke(this, new SubscriptionEventArgs(Interlocked.Increment(ref _eventNumber)));
        }
    }
}