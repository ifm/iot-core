using System;
using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Responses
{
    using System.Collections;
    using System.Collections.Generic;

    public class RemoveProfileResponseServiceData : IDictionary<string, List<ProfileRemoveResult>>, IDictionary
    {
        private readonly IDictionary<string, List<ProfileRemoveResult>> _dictionaryImplementation = new Dictionary<string, List<ProfileRemoveResult>>();

        public bool Contains(object key)
        {
            return ((IDictionary)_dictionaryImplementation).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_dictionaryImplementation).GetEnumerator();
        }

        public void Remove(object key)
        {
            ((IDictionary)_dictionaryImplementation).Remove(key);
        }

        public bool IsFixedSize => ((IDictionary)_dictionaryImplementation).IsFixedSize;

        public IEnumerator<KeyValuePair<string, List<ProfileRemoveResult>>> GetEnumerator()
        {
            return _dictionaryImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this._dictionaryImplementation).GetEnumerator();
        }

        public void Add(KeyValuePair<string, List<ProfileRemoveResult>> item)
        {
            _dictionaryImplementation.Add(item);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)_dictionaryImplementation).Add(key, value);
        }

        public void Clear()
        {
            _dictionaryImplementation.Clear();
        }

        public bool Contains(KeyValuePair<string, List<ProfileRemoveResult>> item)
        {
            return _dictionaryImplementation.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, List<ProfileRemoveResult>>[] array, int arrayIndex)
        {
            _dictionaryImplementation.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, List<ProfileRemoveResult>> item)
        {
            return _dictionaryImplementation.Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)_dictionaryImplementation).CopyTo(array, index);
        }

        public int Count => _dictionaryImplementation.Count;
        public bool IsSynchronized => ((IDictionary)_dictionaryImplementation).IsSynchronized;

        public object SyncRoot => ((IDictionary)_dictionaryImplementation).SyncRoot;

        public bool IsReadOnly => _dictionaryImplementation.IsReadOnly;
        public object this[object key]
        {
            get => ((IDictionary)_dictionaryImplementation)[key];
            set => ((IDictionary)_dictionaryImplementation)[key] = value;
        }

        public void Add(string key, List<ProfileRemoveResult> value)
        {
            _dictionaryImplementation.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionaryImplementation.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _dictionaryImplementation.Remove(key);
        }

        public bool TryGetValue(string key, out List<ProfileRemoveResult> value)
        {
            return _dictionaryImplementation.TryGetValue(key, out value);
        }

        public List<ProfileRemoveResult> this[string key]
        {
            get => _dictionaryImplementation[key];
            set => _dictionaryImplementation[key] = value;
        }

        public ICollection<string> Keys => _dictionaryImplementation.Keys;
        ICollection IDictionary.Values => ((IDictionary)_dictionaryImplementation).Values;

        ICollection IDictionary.Keys => ((IDictionary)_dictionaryImplementation).Keys;

        public ICollection<List<ProfileRemoveResult>> Values => _dictionaryImplementation.Values;
    }

    public class ProfileRemoveResult
    {
        [ifmIoTCore.Common.Variant.VariantPropertyAttribute("code", Required = true)]
        public ProfileRemoveCode Code { get; set; }

        [ifmIoTCore.Common.Variant.VariantPropertyAttribute("profile", Required = false)]
        public string Profile { get; set; }

        [ifmIoTCore.Common.Variant.VariantPropertyAttribute("message", Required = false)]
        public string Message { get; set; }

        [VariantConstructor]
        public ProfileRemoveResult()
        {
        }

        public ProfileRemoveResult(ProfileRemoveCode code, string profile = null, string message = null)
        {
            this.Code = code;
            this.Profile = profile;
            this.Message = message;
        }
    }

    public enum ProfileRemoveCode
    {
        Default = 0,
        Ok = 1,
        ElementNotFound = 2,
        ProfileNotFoundOnElement = 4
    }
}
