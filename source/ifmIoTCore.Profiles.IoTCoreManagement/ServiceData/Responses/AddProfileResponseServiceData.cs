using System;
using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Responses
{
    using System.Collections;
    using System.Collections.Generic;

    public class AddProfileResponseServiceData : IDictionary<string, List<ProfileAddResult>>, IDictionary
    {
        private readonly IDictionary<string, List<ProfileAddResult>> _dictionaryImplementation = new Dictionary<string, List<ProfileAddResult>>();
        public bool Contains(object key)
        {
            return ((IDictionary)_dictionaryImplementation).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (IDictionaryEnumerator) _dictionaryImplementation.GetEnumerator();
        }

        public void Remove(object key)
        {
            ((IDictionary)_dictionaryImplementation).Remove(key);
        }

        public bool IsFixedSize => ((IDictionary)_dictionaryImplementation).IsFixedSize;

        public IEnumerator<KeyValuePair<string, List<ProfileAddResult>>> GetEnumerator()
        {
            return this._dictionaryImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this._dictionaryImplementation).GetEnumerator();
        }

        public void Add(KeyValuePair<string, List<ProfileAddResult>> item)
        {
            this._dictionaryImplementation.Add(item);
        }

        public void Add(object key, object value)
        {
            ((IDictionary)_dictionaryImplementation).Add(key, value);
        }

        public void Clear()
        {
            this._dictionaryImplementation.Clear();
        }

        public bool Contains(KeyValuePair<string, List<ProfileAddResult>> item)
        {
            return this._dictionaryImplementation.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, List<ProfileAddResult>>[] array, int arrayIndex)
        {
            this._dictionaryImplementation.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, List<ProfileAddResult>> item)
        {
            return this._dictionaryImplementation.Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            ((IDictionary)_dictionaryImplementation).CopyTo(array, index);
        }

        public int Count => this._dictionaryImplementation.Count;
        public bool IsSynchronized => ((IDictionary)_dictionaryImplementation).IsSynchronized;

        public object SyncRoot => ((IDictionary)_dictionaryImplementation).SyncRoot;

        public bool IsReadOnly => this._dictionaryImplementation.IsReadOnly;
        public object this[object key]
        {
            get => ((IDictionary)_dictionaryImplementation)[key];
            set => ((IDictionary)_dictionaryImplementation)[key] = value;
        }

        public void Add(string key, List<ProfileAddResult> value)
        {
            this._dictionaryImplementation.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return this._dictionaryImplementation.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return this._dictionaryImplementation.Remove(key);
        }

        public bool TryGetValue(string key, out List<ProfileAddResult> value)
        {
            return this._dictionaryImplementation.TryGetValue(key, out value);
        }

        public List<ProfileAddResult> this[string key]
        {
            get => this._dictionaryImplementation[key];
            set => this._dictionaryImplementation[key] = value;
        }

        public ICollection<string> Keys => this._dictionaryImplementation.Keys;
        ICollection IDictionary.Values => ((IDictionary)_dictionaryImplementation).Values;

        ICollection IDictionary.Keys => ((IDictionary)_dictionaryImplementation).Keys;

        public ICollection<List<ProfileAddResult>> Values => this._dictionaryImplementation.Values;
    }

    public class ProfileAddResult
    {
        [VariantProperty("code", Required = true)]
        public ProfileAddCode Code { get; set; }

        [VariantProperty("profile", Required = false)]
        public string Profile { get; set; }

        [VariantProperty("message", Required = false)]
        public string Message { get; set; }

        [VariantConstructor]
        public ProfileAddResult()
        {
        }

        public ProfileAddResult(ProfileAddCode code, string profile = null, string message = null)
        {
            this.Code = code;
            this.Profile = profile;
            this.Message = message;
        }
    }

    public enum ProfileAddCode
    {
        Default = 0,
        Ok = 1,
        ElementNotFound = 2,
        ProfileAlreadyExistsOnElement = 4
    }
}
