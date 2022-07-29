namespace ifmIoTCore.NetAdapter
{
    public interface IConnectedClientNetAdapter : IClientNetAdapter
    {
        /// <summary>
        /// Connects the client to the server
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects the client from the server
        /// </summary>
        void Disconnect();
    }
}