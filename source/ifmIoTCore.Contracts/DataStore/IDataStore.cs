namespace ifmIoTCore.DataStore
{
    public interface IDataStore
    {
        string GetDataSource();
        string GetValue(string section, string key);
        void SetValue(string section, string key, string value);
        void RemoveValue(string section, string key);
        void RemoveSection(string sectionKey);
    }
}