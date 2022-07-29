namespace ifmIoTCore.Profiles.DeviceManagement.Elements
{
    using ifmIoTCore.Elements;
    
    public interface IProxyElement : IBaseElement
    {
        RemoteContext RemoteContext { get; }
    }
}