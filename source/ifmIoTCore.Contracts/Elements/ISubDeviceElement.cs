namespace ifmIoTCore.Elements
{
    public interface ISubDeviceElement : IBaseElement
    {
        /// <summary>
        /// Gets the getidentity service element
        /// </summary>
        IServiceElement GetIdentityServiceElement { get; }
    }
}