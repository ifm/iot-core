namespace ifmIoTCore.NetAdapter.Http
{
    using System;
    using Logger;
    using Messages;

    public class HttpServerNetAdapter : HttpListenerServerNetAdapter
    {
        private IMessageHandler _messageHandler;
        
        public HttpServerNetAdapter(IMessageHandler ioTCore, Uri uri, IMessageConverter converter, ILogger logger = null)
            : base(uri, converter, logger)
        {
            this._messageHandler = ioTCore ?? throw new ArgumentNullException(nameof(ioTCore));
            this.RequestReceived += OnRequestMessageReceived;
            this.EventReceived += OnEventMessageReceived;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.RequestReceived -= this.OnRequestMessageReceived;
            this.EventReceived -= this.OnEventMessageReceived;
            this._messageHandler = null;
        }

        private void OnEventMessageReceived(object s, EventMessageEventArgs e)
        {
            this._messageHandler.HandleEvent(e.EventMessage);
        }

        private void OnRequestMessageReceived(object s, RequestMessageEventArgs e)
        {
            e.ResponseMessage = this._messageHandler.HandleRequest(e.RequestMessage);
        }
    }
}