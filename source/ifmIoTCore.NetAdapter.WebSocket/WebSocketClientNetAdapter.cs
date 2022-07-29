namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using Messages;
    using Logger;

    public class WebSocketClientNetAdapter : IConnectedClientNetAdapter
    {
        private readonly IMessageHandler _messageHandler;
        private readonly WebSocketClient _client;

        public WebSocketClientNetAdapter(IMessageHandler messageHandler, Uri remoteUri, IMessageConverter converter, ILogger logger, TimeSpan timeout)
        {
            _messageHandler = messageHandler;
            _client = new WebSocketClient(remoteUri, messageHandler, converter, logger, timeout);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public string Type => "ws";

        public string Format => _client.Converter?.Type;

        public Message SendRequest(Message message, TimeSpan? timeout)
        {
            LastUsed = DateTime.Now;
            return _client.SendRequest(message).GetAwaiter().GetResult();
        }

        public void SendEvent(Message message)
        {
            LastUsed = DateTime.Now;
            _client.SendEvent(message).GetAwaiter().GetResult();
        }

        public Uri GetRemoteUri()
        {
            return _client.RemoteUri;
        }

        public async void Connect()
        {
            await _client.ConnectAsync();
        }

        public async void Disconnect()
        {
            await _client.DisconnectAsync();
        }

        public DateTime LastUsed { get; private set; }
    }
}