namespace ifmIoTCore.Configuration
{
    using DataStore;

    public class Settings
    {
        private readonly IDataStore _dataStore;

        public Settings(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public int ElementLockTimeout
        {
            get
            {
                if (_dataStore != null && int.TryParse(_dataStore.GetValue("IoTCore", "ElementLockTimeout"), out var value))
                {
                    return value;
                }
                return Timeout;
            }
        }

        public const int Timeout = 30000;
    }
}
