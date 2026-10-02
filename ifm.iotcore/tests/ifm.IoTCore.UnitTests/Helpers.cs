namespace ifm.IoTCore.UnitTests;

using Contracts;
using ifm.DataStore.Contracts;
using Logger;

internal class DataStore : IDataStore
{
    public T Get<T>(string section, string key)
    {
        return default;
    }

    public void Set<T>(string section, string key, T value)
    {
    }

    public void Delete(string section)
    {
    }

    public void Delete(string section, string key)
    {
    }
}

public static class Helpers
{

    public static IIoTCore CreateIoTCore(string identifier)
    {
        return IoTCoreFactory.Create(identifier, 
            new DataStore(), 
            new NullLogger());
    }
}