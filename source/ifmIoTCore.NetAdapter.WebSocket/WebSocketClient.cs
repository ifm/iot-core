namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Logger;
    using Messages;

    internal class WebSocketClient : WebSocketBase, IDisposable
    {
        private readonly ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private readonly TimeSpan _timeout;

        public readonly Uri RemoteUri;
        private readonly IMessageHandler _messageHandler;
        public readonly IMessageConverter Converter;
        private readonly ILogger _logger;
        private readonly AggregatingWebSocketReceiver _receiver;

        public WebSocketClient(Uri remoteUri, IMessageHandler messageHandler, IMessageConverter converter, ILogger logger, TimeSpan timeout)
        {
            RemoteUri = remoteUri;
            _messageHandler = messageHandler;
            Converter = converter;
            _logger = logger;
            _timeout = timeout;

            _webSocket = new ClientWebSocket();

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            this._receiver = new AggregatingWebSocketReceiver(converter, logger, _cancellationTokenSource);
            this._receiver.RequestReceived += ReceiverOnRequestReceived;
            this._receiver.EventReceived += ReceiverOnEventReceived;
        }

        public async Task ConnectAsync()
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await _webSocket.ConnectAsync(RemoteUri, CancellationToken.None);
                _ = _receiver.StartAsync(_webSocket, RemoteUri);
            }
        }

        public async Task DisconnectAsync()
        {
            // Close the socket first, because ReceiveAsync leaves an invalid socket (state = aborted) when the token is cancelled
            var timeoutCancellationToken = new CancellationTokenSource(_timeout).Token;
            try
            {
                if (_webSocket.State != WebSocketState.Aborted && _webSocket.State != WebSocketState.Closed && _webSocket.State != WebSocketState.None)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", timeoutCancellationToken);
                }

                // Now we wait for the server response, which will close the socket
                while (_webSocket.State == WebSocketState.Open && !timeoutCancellationToken.IsCancellationRequested)
                {
                    // Do nothing
                }
            }
            catch (OperationCanceledException)
            {
                // Normal upon task/token cancellation, disregard
            }
            catch (NullReferenceException)
            {
                // This can happen when calling _websocket.CloseAsync()
            }

            // Whether we closed the socket or timed out, we cancel the token causing ReceiveAsync to abort the socket
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public WebSocketState State => _webSocket?.State ?? WebSocketState.None;

        public async Task<Message> SendRequest(Message requestMessage)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await ConnectAsync();
            }

            if (!_receiver.Running)
            {
                _ = _receiver.StartAsync(_webSocket, RemoteUri);
            }

            // Send request
            var requestString = Converter.Serialize(requestMessage);
            await SendAsync(_webSocket, Encoding.UTF8.GetBytes(requestString), _cancellationToken);

            // Receive response
            return await _receiver.GetResultAsync(requestMessage.Cid, TimeSpan.FromSeconds(30), _cancellationToken);

            //// Receive response
            //var responseBytes = await ReceiveAsync(_webSocket, _cancellationToken);
            //var responseString = Encoding.UTF8.GetString(responseBytes);
            //return Converter.Deserialize(responseString);
        }

        public async Task SendEvent(Message eventMessage)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                await ConnectAsync();
            }

            // Send request
            var requestString = Converter.Serialize(eventMessage);
            await SendAsync(_webSocket, Encoding.UTF8.GetBytes(requestString), _cancellationToken);
        }

        public async void Dispose()
        {
            this._receiver.RequestReceived -= ReceiverOnRequestReceived;
            this._receiver.EventReceived -= ReceiverOnEventReceived;
            _receiver.Stop();

            try
            {
                await this.DisconnectAsync();
            }
            catch (Exception)
            {
                // Ignore
            }
            
            _webSocket?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        private void ReceiverOnRequestReceived(object sender, RequestMessageEventArgs e)
        {
            e.ResponseMessage = this._messageHandler.HandleRequest(e.RequestMessage);

            var serializedMessage = Converter.Serialize(e.ResponseMessage);
            _ = SendAsync(_webSocket, Encoding.UTF8.GetBytes(serializedMessage), _cancellationToken);
        }

        private void ReceiverOnEventReceived(object sender, EventMessageEventArgs e)
        {
            this._messageHandler.HandleEvent(e.EventMessage);
        }
    }
}
