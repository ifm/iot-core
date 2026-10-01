namespace ifm.IoTCore.ElementManager.Elements;

using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Exceptions;
using Contracts;
using Contracts.Elements;
using Contracts.Elements.Formats;
using Contracts.Elements.ServiceData.Requests;
using Contracts.Elements.ServiceData.Responses;
using DataStore.Contracts;
using EventSender.Contracts;
using ifm.Common;
using ifm.Common.Variant;

internal sealed class EventElement : BaseElement, IEventElement
{
    private const string DataStoreSection = "events";

    private readonly IDataStore _dataStore;
    private readonly IEventSender _eventSender;
    private readonly IElementManager _elementManager;
    private EventHandler _eventRaisedHandler;
    private List<SubscriptionInfo> _subscriptions;

    public EventElement(string identifier,
        IDataStore dataStore,
        IEventSender eventSender,
        IElementManager elementManager,
        Format format,
        IEnumerable<string> profiles,
        string uid,
        bool isHidden) : base(Identifiers.Event, identifier, format, profiles, uid, isHidden)
    {
        _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        _eventSender = eventSender ?? throw new ArgumentNullException(nameof(eventSender));
        _elementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));

        ParentChanged += (_, _) =>
        {
            _subscriptions = _dataStore.Get<List<SubscriptionInfo>>(DataStoreSection, Address) ?? [];
        };

        EventRaised += (_, _) =>
        {
            if (EventCounterDataElement != null)
            {
                ((IReadWriteDataElement<int>)EventCounterDataElement).Value++;
            }
            SendEvents();
        };
    }

    public event EventHandler<SubscriptionInfo> Subscribed;

    public event EventHandler<SubscriptionInfo> Unsubscribed;

    public event EventHandler EventRaised
    {
        add
        {
            _eventRaisedHandler += value;
            Subscribed.Raise(this);
        }

        remove
        {
            _eventRaisedHandler -= value;
            Unsubscribed.Raise(this);
        }
    }

    public List<SubscriptionInfo> GetSubscriptions()
    {
        lock (_subscriptions)
        {
            return [.. _subscriptions];
        }
    }

    public int SubscriptionCount
    {
        get
        {
            lock (_subscriptions)
            {
                return _subscriptions.Count;
            }
        }
    }

    public IReadDataElement<int> EventCounterDataElement { get; set; }

    public void Raise()
    {
        _eventRaisedHandler.Raise(this);
    }

    public SubscribeResponseServiceData SubscribeFunc(IBaseElement sender, SubscribeRequestServiceData data, ServiceInfo info)
    {
        return Subscribe(data, info?.Cid);
    }

    public SubscribeResponseServiceData Subscribe(SubscribeRequestServiceData data, int? cid)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        if (!CallbackParser.IsValid(data.Callback)) throw new DataInvalidException($"{data.Callback} is not a valid uri");
        if (data.DataToSend != null && data.DataToSend.HasDuplicates()) throw new DataInvalidException("Parameter 'datatosend' has duplicates");
        if (data.SubscriptionId is < -1) throw new DataInvalidException("Parameter 'subscribeid' is invalid");

        lock (_subscriptions)
        {
            var subscription = data.SubscriptionId != null ? _subscriptions.FirstOrDefault(x => x.Id == data.SubscriptionId) : _subscriptions.FirstOrDefault(x => x.Id == cid && x.Callback == data.Callback);
            if (subscription != null)
            {
                _subscriptions.Remove(subscription);
            }

            subscription = new SubscriptionInfo(GenerateSubscriptionId(data.SubscriptionId, cid), data.CallbackUrl, data.DataToSend, data.Persist);
            _subscriptions.Add(subscription);

            if (data.Persist)
            {
                _dataStore.Set(DataStoreSection, Address, _subscriptions.Where(x => x.Persist));
            }

            try
            {
                Subscribed.Raise(this, subscription, true, true);
            }
            catch
            {
                _subscriptions.Remove(subscription);
                throw;
            }
            return new SubscribeResponseServiceData(subscription.Id);
        }
    }

    private int GenerateSubscriptionId(int? subscriptionId, int? cid)
    {
        const int autoCreateSubscriptionId = -1;
        var id = subscriptionId ?? cid ?? autoCreateSubscriptionId;
        if (id != autoCreateSubscriptionId) return id;
        id = 1;
        while (id < short.MaxValue)
        {
            if (_subscriptions.Any(x => x.Id == id))
            {
                id++;
            }
            else
            {
                break;
            }
        }
        return id;
    }

    public void UnsubscribeFunc(IBaseElement sender, UnsubscribeRequestServiceData data, ServiceInfo info)
    {
        Unsubscribe(data, info?.Cid);
    }

    public void Unsubscribe(UnsubscribeRequestServiceData data, int? cid)
    {
        if (data == null) throw new DataInvalidException("Service data empty");
        if (data.Callback != null && !CallbackParser.IsValid(data.Callback)) throw new DataInvalidException($"{data.Callback} is not a valid uri");

        lock (_subscriptions)
        {
            var subscription = data.SubscriptionId != null ? _subscriptions.FirstOrDefault(x => x.Id == data.SubscriptionId) : _subscriptions.FirstOrDefault(x => x.Id == cid && x.Callback == data.Callback);
            if (subscription == null)
            {
                throw new DataInvalidException("Subscription not found");
            }

            _subscriptions.Remove(subscription);
            _dataStore.Set(DataStoreSection, Address, _subscriptions.Where(x => x.Persist));

            try
            {
                Unsubscribed.Raise(this, subscription);
            }
            catch
            {
                // Ignore and log. Unsubscribe should never fail
            }
        }
    }

    private void SendEvents()
    {
        foreach (var subscription in _subscriptions)
        {
            _eventSender.SendEvent(subscription.Callback, CreateEventMessageData(subscription));
        }
    }

    private EventMessageData CreateEventMessageData(SubscriptionInfo subscription)
    {
        Dictionary<string, CodeDataPair> payload = null;
        if (subscription.DataToSend is { Count: > 0 })
        {
            payload = [];
            foreach (var address in subscription.DataToSend)
            {
                var element = _elementManager.GetElementByAddress(address, false);
                if (element is IReadDataElement dataElement)
                {
                    try
                    {
                        var data = dataElement.GetData();
                        payload[address] = new CodeDataPair(ResponseCodes.Success, data.Value, data.TimeStamp);
                    }
                    catch (IoTCoreException ex)
                    {
                        payload[address] = new CodeDataPair(ex.ResponseCode, Variant.FromObject(new ErrorInfoResponseServiceData(ex.Message, ex.ErrorCode, ex.ErrorDetails, ex.HexError)));
                    }
                    catch
                    {
                        payload[address] = new CodeDataPair(ResponseCodes.InternalError, null);
                    }
                }
                else
                {
                    payload[address] = new CodeDataPair(ResponseCodes.NotFound, null);
                }
            }
        }
        if (Identifier == Identifiers.DataChanged && Parent is IReadDataElement parentElement)
        {
            payload ??= [];
            try
            {
                var data = parentElement.GetData();
                payload[AddressParser.RemoveRootIdentifier(parentElement.Address)] = new CodeDataPair(ResponseCodes.Success, data.Value, data.TimeStamp);
            }
            catch (IoTCoreException ex)
            {
                payload[AddressParser.RemoveRootIdentifier(parentElement.Address)] = new CodeDataPair(ex.ResponseCode, Variant.FromObject(new ErrorInfoResponseServiceData(ex.Message, ex.ErrorCode, ex.ErrorDetails, ex.HexError)));
            }
            catch
            {
                payload[AddressParser.RemoveRootIdentifier(parentElement.Address)] = new CodeDataPair(ResponseCodes.InternalError, null);
            }
        }

        return new EventMessageData(Address, subscription.Id, payload);
    }
}
