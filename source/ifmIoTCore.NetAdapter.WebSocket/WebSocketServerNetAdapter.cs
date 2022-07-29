namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Base;
    using Common;
    using Logger;
    using Messages;

    public class WebSocketServerNetAdapter : ServerNetAdapterBase, IConnectedServerNetAdapter
    {
        private readonly WebSocketServer _server;
        private readonly IMessageHandler _messageHandler;

        public event EventHandler<ClientAddedEventArgs> ClientAdded;
        public event EventHandler<ClientRemovedEventArgs> ClientRemoved;

        public WebSocketServerNetAdapter(IMessageHandler messageHandler, Uri localUri, IMessageConverter converter, ILogger logger)
        {
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            _server = new WebSocketServer(localUri, converter, logger);
            _server.RequestMessageReceived += OnRequestMessageReceived;
            _server.EventMessageReceived += OnEventMessageReceived;
            _server.ClientAdded += OnClientAdded;
            _server.ClientRemoved += OnClientRemoved;
        }

        protected void RaiseClientAdded(string clientId)
        {
            this.ClientAdded?.Raise(this, new ClientAddedEventArgs(clientId));
        }

        protected void RaiseClientRemoved(string clientId)
        {
            this.ClientRemoved?.Raise(this, new ClientRemovedEventArgs(clientId));
        }

        private void OnClientRemoved(object sender, ClientRemovedEventArgs e)
        {
            this.RaiseClientRemoved(e.ClientId);
        }

        private void OnClientAdded(object sender, ClientAddedEventArgs e)
        {
            this.RaiseClientAdded(e.ClientId);
        }

        private void OnRequestMessageReceived(object sender, RequestMessageEventArgs e)
        {
            e.ResponseMessage = _messageHandler.HandleRequest(e.RequestMessage);
        }

        private void OnEventMessageReceived(object sender, EventMessageEventArgs e)
        {
            _messageHandler?.HandleEvent(e.EventMessage);
        }

        public IEnumerable<string> ConnectedClients => _server.ConnectedClients.Keys;

        public bool IsClientConnected(string clientId)
        {
            return _server.ConnectedClients.ContainsKey(clientId);
        }

        public override string Scheme => "ws";

        public override string Format => _server.Converter.Type;

        public override Uri Uri => _server.LocalUri;

        public override bool IsListening => _server.IsListening;

        public override void Start()
        {
            _server.Start();
        }

        public override Task StartAsync()
        {
            return Task.Run(Start);
        }

        public override void Stop()
        {
            _server.Stop();
        }

        public override Task StopAsync()
        {
            return Task.Run(Stop);
        }

        public void SendEvent(string clientId, Message eventMessage)
        {
            _server.SendEventAsync(clientId, eventMessage).GetAwaiter().GetResult();
        }

        public override void Dispose()
        {
            _server.ClientRemoved -= OnClientRemoved;
            _server.ClientAdded -= OnClientAdded;
            _server.RequestMessageReceived -= OnRequestMessageReceived;
            _server.EventMessageReceived -= OnEventMessageReceived;
            _server.Dispose();
        }
    }
}
