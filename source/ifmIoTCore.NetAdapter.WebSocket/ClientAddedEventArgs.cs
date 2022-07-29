namespace ifmIoTCore.NetAdapter.WebSocket
{
    public class ClientAddedEventArgs
    {
        public string ClientId { get; }

        public ClientAddedEventArgs(string clientId)
        {
            ClientId = clientId;
        }
    }
}