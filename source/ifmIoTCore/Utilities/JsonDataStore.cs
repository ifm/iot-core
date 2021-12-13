namespace ifmIoTCore.Utilities
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class JsonDataStore : IDataStore
    {
        private readonly string _dataStoreFile;

        private readonly Dictionary<string, Dictionary<string, JToken>> _dataStore;

        public JsonDataStore(string fileName)
        {
            _dataStoreFile = fileName;

            if (File.Exists(_dataStoreFile))
            {
                var json = File.ReadAllText(_dataStoreFile);
                _dataStore = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, JToken>>>(json);
            }
            else
            {
                _dataStore = new Dictionary<string, Dictionary<string, JToken>>();
            }
        }

        public string GetDataSource()
        {
            return _dataStoreFile;
        }

        public JToken GetValue(string sectionKey, string valueKey)
        {
            lock (_dataStore)
            {
                if (_dataStore.TryGetValue(sectionKey, out var section))
                {
                    return section.ContainsKey(valueKey) ? section[valueKey] : null;
                }
                return null;
            }
        }

        public void SetValue(string sectionKey, string valueKey, JToken value)
        {
            lock (_dataStore)
            {
                if (!_dataStore.TryGetValue(sectionKey, out var section))
                {
                    section = new Dictionary<string, JToken>();
                    _dataStore.Add(sectionKey, section);
                }
                if (section.ContainsKey(valueKey))
                {
                    section[valueKey] = value;
                }
                else
                {
                    section.Add(valueKey, value);
                }
                SaveData();
            }
        }

        public void RemoveValue(string sectionKey, string valueKey)
        {
            lock (_dataStore)
            {
                if (_dataStore.TryGetValue(sectionKey, out var section))
                {
                    section.Remove(valueKey);
                    SaveData();
                }
            }
        }

        public void RemoveSection(string sectionKey)
        {
            lock (_dataStore)
            {
                _dataStore.Remove(sectionKey);
                SaveData();
            }
        }

        private void SaveData()
        {
            lock (_dataStore)
            {
                var json = JsonConvert.SerializeObject(_dataStore, Formatting.Indented);
                File.WriteAllText(_dataStoreFile, json);
            }
        }
    }
}
