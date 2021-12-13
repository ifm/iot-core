namespace ifmIoTCore.Configuration
{
    using Utilities;

    public class Settings
    {
        private readonly IDataStore _dataStore;

        public Settings(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public int ElementLockTimeout => _dataStore.GetValue("IoTCore", "ElementLockTimeout")?.ToObject<int>() ?? 3000;

        public const int Timeout = 3000;
    }
}
