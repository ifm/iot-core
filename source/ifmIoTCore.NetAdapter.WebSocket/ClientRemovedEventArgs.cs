namespace ifmIoTCore.NetAdapter.WebSocket
{
    public class ClientRemovedEventArgs
    {
        public string ClientId { get; }

        public ClientRemovedEventArgs(string clientId)
        {
            ClientId = clientId;
        }
    }
}