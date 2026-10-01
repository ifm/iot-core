namespace ifm.IoTCore.NetAdapterManager;

using Contracts;

public class NetAdapterManager : INetAdapterManager
{
    public IClientNetAdapterManager Client { get; } = new ClientNetAdapterManager();
    public IServerNetAdapterManager Server { get; } = new ServerNetAdapterManager();
}