namespace ifmIoTCore.Utilities
{
    using Newtonsoft.Json.Linq;

    public interface IDataStore
    {
        string GetDataSource();
        JToken GetValue(string section, string key);
        void SetValue(string section, string key, JToken value);
        void RemoveValue(string section, string key);
        void RemoveSection(string sectionKey);
    }
}