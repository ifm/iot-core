namespace ifmIoTCore.Elements.ServiceData.Responses
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the outgoing data for a IDeviceElement.GetDataMulti service call
    /// </summary>
    public class GetDataMultiResponseServiceData : IDictionary<string, CodeDataPair>
    {
        private readonly Dictionary<string, CodeDataPair> _dataToSend = new Dictionary<string, CodeDataPair>();

        public IEnumerator<KeyValuePair<string, CodeDataPair>> GetEnumerator()
        {
            return _dataToSend.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dataToSend).GetEnumerator();
        }

        public void Add(KeyValuePair<string, CodeDataPair> item)
        {
            _dataToSend.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dataToSend.Clear();
        }

        public bool Contains(KeyValuePair<string, CodeDataPair> item)
        {
            return _dataToSend.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, CodeDataPair>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<string, CodeDataPair> item)
        {
            return _dataToSend.Remove(item.Key);
        }

        public int Count => _dataToSend.Count;

        public bool IsReadOnly => ((IDictionary<string, CodeDataPair>)_dataToSend).IsReadOnly;

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

        public CodeDataPair this[string key]
        {
            get => _dataToSend[key];
            set => _dataToSend[key] = value;
        }

        public ICollection<string> Keys => ((IDictionary<string, CodeDataPair>)_dataToSend).Keys;

        public ICollection<CodeDataPair> Values => ((IDictionary<string, CodeDataPair>)_dataToSend).Values;
    }
}
