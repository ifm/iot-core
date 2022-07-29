namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.GetDataMulti service call
    /// </summary>
    public class GetDataMultiResponseServiceData : IDictionary<string, CodeDataPair>, IDictionary
    {
        private readonly Dictionary<string, CodeDataPair> _dataToSend = new Dictionary<string, CodeDataPair>();

        #region IDictionary<,>

        public CodeDataPair this[string key]
        {
            get => _dataToSend[key];
            set => _dataToSend[key] = value;
        }

        public ICollection<string> Keys => ((IDictionary<string, CodeDataPair>)_dataToSend).Keys;

        public ICollection<CodeDataPair> Values => ((IDictionary<string, CodeDataPair>)_dataToSend).Values;

        public IEnumerator<KeyValuePair<string, CodeDataPair>> GetEnumerator()
        {
            return _dataToSend.GetEnumerator();
        }

        public void Add(KeyValuePair<string, CodeDataPair> item)
        {
            _dataToSend.Add(item.Key, item.Value);
        }

        public void Add(string key, CodeDataPair value)
        {
            _dataToSend.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dataToSend.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dataToSend.Remove(key);
        }

        public bool TryGetValue(string key, out CodeDataPair value)
        {
            return _dataToSend.TryGetValue(key, out value);
        }

        public bool Contains(KeyValuePair<string, CodeDataPair> item)
        {
            return _dataToSend.ContainsKey(item.Key);
        }

        public bool Remove(KeyValuePair<string, CodeDataPair> item)
        {
            return _dataToSend.Remove(item.Key);
        }

        public void CopyTo(KeyValuePair<string, CodeDataPair>[] array, int arrayIndex)
        {
            ((IDictionary<string, CodeDataPair>)_dataToSend).CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dataToSend).GetEnumerator();
        }

        public void Clear()
        {
            _dataToSend.Clear();
        }

        public int Count => _dataToSend.Count;


        #endregion

        #region IDictionary

        public bool IsFixedSize => ((IDictionary)_dataToSend).IsFixedSize;

        public bool IsReadOnly => ((IDictionary)_dataToSend).IsReadOnly;

        public object this[object key]
        {
            get => ((IDictionary)_dataToSend)[key];
            set => ((IDictionary)_dataToSend)[key] = value;
        }

        ICollection IDictionary.Keys => ((IDictionary)_dataToSend).Keys;

        ICollection IDictionary.Values => ((IDictionary)_dataToSend).Values;

        public void Add(object key, object value)
        {
            ((IDictionary)_dataToSend).Add(key, value);
        }

        public bool Contains(object key)
        {
            return ((IDictionary)_dataToSend).Contains(key);
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)_dataToSend).CopyTo(array, index);
        }

        public object SyncRoot => ((IDictionary)_dataToSend).SyncRoot;

        public bool IsSynchronized => ((IDictionary)_dataToSend).IsSynchronized;

        public void Remove(object key)
        {
            ((IDictionary)_dataToSend).Remove(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dataToSend).GetEnumerator();
        }

        #endregion

    }
}
