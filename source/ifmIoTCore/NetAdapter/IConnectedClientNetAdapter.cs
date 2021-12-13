namespace ifmIoTCore.NetAdapter
{
    public interface IConnectedClientNetAdapter : IClientNetAdapter
    {
        /// <summary>
        /// Connects the client to the server
        /// </summary>
        void Connect();
    }
}