namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using System.Collections.Generic;
    using Messages;
    using Logger;

    public class WebSocketClientNetAdapterFactory : IClientNetAdapterFactory
    {
        private readonly IMessageHandler _messageHandler;
        private readonly IMessageConverter _converter;
        private readonly ILogger _logger;
        private readonly TimeSpan _timeout;

        private readonly Dictionary<Uri, WebSocketClientNetAdapter> _clients = new();

        public string Scheme => "ws";

        public string Format => _converter.Type;

        public WebSocketClientNetAdapterFactory(IMessageHandler messageHandler,
            IMessageConverter converter) :
            this(messageHandler, converter, null, TimeSpan.FromSeconds(30))
        {
        }

        public WebSocketClientNetAdapterFactory(IMessageHandler messageHandler,
            IMessageConverter converter,
            ILogger logger,
            TimeSpan timeout)
        {
            _messageHandler = messageHandler;
            _converter = converter;
            _logger = logger;
            _timeout = timeout;
        }

        public IClientNetAdapter CreateClient(Uri remoteUri)
        {
            lock (_clients)
            {
                if (!_clients.TryGetValue(remoteUri, out var client))
                {
                    client = new WebSocketClientNetAdapter(_messageHandler, remoteUri, _converter, _logger, _timeout);
                    _clients.Add(remoteUri, client);
                }

                return client;
            }
        }

        public void Dispose()
        {
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    client.Value.Dispose();
                }
            }
        }
    }
}