namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Common.Variant;

    public class GetSubscriberListItem
    {
        /// <summary>
        /// The address of the event element
        /// </summary>
        [VariantProperty("adr", IgnoredIfNull = true)]
        public string Address { get; set; }

        /// <summary>
        /// The url to which the IoTCore is sending events
        /// </summary>
        [VariantProperty("callbackurl", Required = true)]
        public string Callback { get; set; }

        /// <summary>
        /// List of data element addresses, whose values are sent with the event
        /// </summary>
        [VariantProperty("datatosend", IgnoredIfNull = true)]
        public List<string> DataToSend { get; set; }

        /// <summary>
        /// Specifies the persistence duration type
        /// </summary>
        [VariantProperty("persist", IgnoredIfNull = true)]
        public bool Persist { get; set; }

        /// <summary>
        /// The id which identifies the subscription
        /// </summary>
        [VariantProperty("subscribeid", IgnoredIfNull = true)]
        public int SubscriptionId { get; set; }

        /// <summary>
        /// The old id which identifies the subscription
        /// </summary>
        [VariantProperty("cid", IgnoredIfNull = true)]
        public int Cid { get => SubscriptionId; set => SubscriptionId = value; }

        /// <summary>
        /// The parameterless constructor for the variant converter
        /// </summary>
        [VariantConstructor]
        public GetSubscriberListItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="address">The address of the event element</param>
        /// <param name="callback">The url to which the IoTCore is sending events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">Specifies the persistence duration type</param>
        /// <param name="subscriptionId">The uid which identifies the subscription</param>
        public GetSubscriberListItem(string address, string callback, List<string> dataToSend, bool persist, int subscriptionId)
        {
            Address = address;
            Callback = callback;
            DataToSend = dataToSend;
            Persist = persist;
            SubscriptionId = subscriptionId;
        }
    }

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.GetSubscriberList service call
    /// </summary>
    public class GetSubscriberListResponseServiceData: IList<GetSubscriberListItem>, IList
    {
        private readonly List<GetSubscriberListItem> _subscriptions = new List<GetSubscriberListItem>();
        

        /// <summary>
        /// Add a new subscription info item 
        /// </summary>
        /// <param name="address">The address of the event element</param>
        /// <param name="callback">The url to which the IoTCore is sending events</param>
        /// <param name="dataToSend">List of data element addresses, whose values are sent with the event</param>
        /// <param name="persist">Specifies the persistence duration type</param>
        /// <param name="sid">The id which identifies the subscription</param>
        public void Add(string address, string callback, List<string> dataToSend, bool persist, int sid)
        {
            _subscriptions.Add(new GetSubscriberListItem(address,
                callback,
                dataToSend,
                persist,
                sid));
        }

        public IEnumerator<GetSubscriberListItem> GetEnumerator()
        {
            return _subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _subscriptions).GetEnumerator();
        }

        public void Add(GetSubscriberListItem item)
        {
            _subscriptions.Add(item);
        }

        public int Add(object value)
        {
            return ((IList) _subscriptions).Add(value);
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }

        public bool Contains(object value)
        {
            return ((IList) _subscriptions).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList) _subscriptions).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList) _subscriptions).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList) _subscriptions).Remove(value);
        }

        public bool Contains(GetSubscriberListItem item)
        {
            return _subscriptions.Contains(item);
        }

        public void CopyTo(GetSubscriberListItem[] array, int arrayIndex)
        {
            _subscriptions.CopyTo(array, arrayIndex);
        }

        public bool Remove(GetSubscriberListItem item)
        {
            return _subscriptions.Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection) _subscriptions).CopyTo(array, index);
        }

        public int Count => _subscriptions.Count;
        public bool IsSynchronized => ((ICollection) _subscriptions).IsSynchronized;

        public object SyncRoot => ((ICollection) _subscriptions).SyncRoot;

        public bool IsReadOnly => ((ICollection<GetSubscriberListItem>) _subscriptions).IsReadOnly;
        
        object IList.this[int index]
        {
            get => ((IList) _subscriptions)[index];
            set => ((IList) _subscriptions)[index] = value;
        }

        public int IndexOf(GetSubscriberListItem item)
        {
            return _subscriptions.IndexOf(item);
        }

        public void Insert(int index, GetSubscriberListItem item)
        {
            _subscriptions.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _subscriptions.RemoveAt(index);
        }

        public bool IsFixedSize => ((IList) _subscriptions).IsFixedSize;

        public GetSubscriberListItem this[int index]
        {
            get => _subscriptions[index];
            set => _subscriptions[index] = value;
        }
    }
}
