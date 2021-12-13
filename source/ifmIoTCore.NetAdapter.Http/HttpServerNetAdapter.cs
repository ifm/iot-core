namespace ifmIoTCore.NetAdapter.Http
{
    using System;
    using Messages;
    using Utilities;

    public class HttpServerNetAdapter : HttpListenerServerNetAdapter
    {
        private IIoTCore _ioTCore;
        
        public HttpServerNetAdapter(IIoTCore ioTCore, Uri uri, IConverter converter, ILogger logger = null)
            : base(uri, converter, logger)
        {
            this._ioTCore = ioTCore ?? throw new ArgumentNullException(nameof(ioTCore));
            this.RequestMessageReceived += OnRequestMessageReceived;
            this.EventMessageReceived += OnEventMessageReceived;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.RequestMessageReceived -= this.OnRequestMessageReceived;
            this.EventMessageReceived -= this.OnEventMessageReceived;
            this._ioTCore = null;
        }

        private void OnEventMessageReceived(object s, EventMessageEventArgs e)
        {
            this._ioTCore?.HandleEvent(e.EventMessage);
        }

        private void OnRequestMessageReceived(object s, RequestMessageEventArgs e)
        {
            e.Response = this._ioTCore?.HandleRequest(e.Request);
        }
    }
}