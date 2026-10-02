namespace ifm.IoTCore.NetAdapter.Http.Contracts
{
    using NetAdapter.Contracts.Server;

    public interface IHttpServerNetAdapter : IServerNetAdapter
    {
        public AllowedServicesMode AllowedServicesMode { get; set; }
    }
}
