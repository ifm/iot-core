namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using Logger;
    using Messages;

    internal class AggregatingWebSocketReceiver : WebSocketBase
    {
        private readonly IMessageConverter _messageConverter;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ConcurrentDictionary<int, Message> _receivedResponses = new ConcurrentDictionary<int, Message>();

        public AggregatingWebSocketReceiver(IMessageConverter messageConverter, ILogger logger, CancellationTokenSource cancellationTokenSource)
        {
            _messageConverter = messageConverter;
            _logger = logger;
            _cancellationTokenSource = cancellationTokenSource;
        }

        public bool Running { get; private set; }
        public event EventHandler<RequestMessageEventArgs> RequestReceived;
        public event EventHandler<EventMessageEventArgs> EventReceived;

        public async Task StartAsync(ClientWebSocket webSocket, Uri uri)
        {
            if (webSocket.State != WebSocketState.Open)
            {
                await webSocket.ConnectAsync(uri, _cancellationTokenSource.Token);
            }

            this.Running = true;

            while (webSocket.State == WebSocketState.Open && !_cancellationTokenSource.IsCancellationRequested)
            {
                var result = await ReceiveAsync(webSocket, _cancellationTokenSource.Token);
                var messageString = Encoding.UTF8.GetString(result);
                var message = _messageConverter.Deserialize(messageString);

                if (message.Code == RequestCodes.Event)
                {
                    this.RaiseEventReceived(new EventMessageEventArgs(message));
                } 
                else if (message.Code == RequestCodes.Request)
                {
                    this.RaiseRequestReceived(new RequestMessageEventArgs(message));
                }
                else if (!this._receivedResponses.TryAdd(message.Cid, message))
                {
                    if (_receivedResponses.ContainsKey(message.Cid))
                    {
                        _logger.Error($"Could not add received message with cid: {message.Cid} and address {message.Address} because there is a pending request with cid {message.Cid}");
                    }
                    else
                    {
                        _logger.Error($"Could not add message with cid: {message.Cid} and address {message.Address}");
                    }
                }
                else
                {
                    _logger.Debug($"Received message with cid {message.Cid} with aggregating receiver.");
                }
            }

            if (webSocket.State != WebSocketState.Open)
            {
                throw new IOException($"The websocket is not open anymore. Current state is: {webSocket.State}");
            }
            else if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new TaskCanceledException("The receiver task has been cancelled by cancellationtoken.");
            }
        }

        public void Stop()
        {
            this.Running = false;
            this._cancellationTokenSource.Cancel();
        }

        public async Task<Message> GetResultAsync(int cid, TimeSpan timeout, CancellationToken token)
        {
            if (_receivedResponses.TryRemove(cid, out var responseMessage))
            {
                _logger.Debug($"Returning response for pending request with cid {cid}.");
                return responseMessage;
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed < timeout && !token.IsCancellationRequested)
            {
                if (_receivedResponses.TryRemove(cid, out responseMessage))
                {
                    stopWatch.Stop();

                    _logger.Debug($"Returning response for pending request with cid {cid}.");
                    return responseMessage;
                }

                await Task.Delay(1, token);
            }

            stopWatch.Stop();
            throw new TimeoutException(
                $"No message received with cid: {cid} within timeout of: {timeout.ToString("c", CultureInfo.InvariantCulture)}");
        }

        protected void RaiseEventReceived(EventMessageEventArgs args)
        {
            this.EventReceived.Raise(this, args);
        }

        protected void RaiseRequestReceived(RequestMessageEventArgs args)
        {
            this.RequestReceived.Raise(this, args);
        }
    }
}