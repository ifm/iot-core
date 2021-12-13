namespace ifmIoTCore.Profiles.IoTCoreManagement.ServiceData.Responses
{
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class RemoveProfileResponseServiceData : IDictionary<string, List<ProfileRemoveResult>>
    {
        private readonly IDictionary<string, List<ProfileRemoveResult>> _dictionaryImplementation = new Dictionary<string, List<ProfileRemoveResult>>();
        
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

        public int Count => _dictionaryImplementation.Count;

        public bool IsReadOnly => _dictionaryImplementation.IsReadOnly;

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

        public ICollection<List<ProfileRemoveResult>> Values => _dictionaryImplementation.Values;
    }

    public class ProfileRemoveResult
    {
        [JsonProperty("code", Required = Required.Always)]
        public ProfileRemoveCode Code;

        [JsonProperty("profile", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Profile;

        [JsonProperty("message", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Message;

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
