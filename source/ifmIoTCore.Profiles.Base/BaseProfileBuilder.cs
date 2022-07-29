namespace ifmIoTCore.Profiles.Base
{
    public abstract class BaseProfileBuilder : IProfileBuilder
    {
        protected readonly IIoTCore IoTCore;

        protected readonly string BaseAddress;

        protected BaseProfileBuilder(ProfileBuilderConfiguration config)
        {
            IoTCore = config.IoTCore;
            BaseAddress = config.Address;
            
        }

        public abstract void Build();

    }
}
