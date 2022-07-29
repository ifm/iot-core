namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Common;
    using Common.Variant;
    using Exceptions;
    using Logger;
    using Messages;
    using Resources;

    internal class WebSocketServer : WebSocketBase, IDisposable
    {
        private readonly HttpListener _httpListener = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly CancellationToken _cancellationToken;

        public readonly Uri LocalUri;
        public readonly IMessageConverter Converter;
        private readonly ILogger _logger;

        public readonly ConcurrentDictionary<string, WebSocket> ConnectedClients = new();

        public event EventHandler<ClientAddedEventArgs> ClientAdded;
        public event EventHandler<ClientRemovedEventArgs> ClientRemoved;

        public WebSocketServer(Uri localUri, IMessageConverter converter, ILogger logger)
        {
            LocalUri = localUri ?? throw new ArgumentNullException(nameof(localUri));
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _httpListener.Prefixes.Add(localUri.AbsoluteUri);

            _cancellationToken = _cancellationTokenSource.Token;
        }

        public event EventHandler<RequestMessageEventArgs> RequestMessageReceived;
        public event EventHandler<EventMessageEventArgs> EventMessageReceived;

        public bool IsListening => _httpListener.IsListening;

        public void Start()
        {
            if (_httpListener.IsListening) return;

            _httpListener.Start();

            // Listen on a separate thread so that Listener.Stop can interrupt GetContextAsync
            Task.Run(ListenAsync, _cancellationToken);
        }

        public void Stop()
        {
            if (!_httpListener.IsListening) return;

            _cancellationTokenSource.Cancel();
            _httpListener.Stop();

            // ToDo: Wait for all tasks to return

            ConnectedClients.Clear();
        }

        protected void RaiseClientAdded(string clientId)
        {
            this.ClientAdded?.Raise(this, new ClientAddedEventArgs(clientId));
        }

        protected void RaiseClientRemoved(string clientId)
        {
            this.ClientRemoved?.Raise(this, new ClientRemovedEventArgs(clientId));
        }

        private async Task ListenAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Returns when client connects
                    // Method does not support cancellation; it returns with an exception when HttpListener Abort() or Stop() called
                    var httpContext = await _httpListener.GetContextAsync();

                    if (!_cancellationToken.IsCancellationRequested)
                    {
                        if (httpContext.Request.IsWebSocketRequest)
                        {
                            // HTTP is only the initial connection; upgrade to a client-specific websocket
                            HttpListenerWebSocketContext webSocketContext;
                            try
                            {
                                webSocketContext = await httpContext.AcceptWebSocketAsync(null);

                                var clientId = GetClientId(webSocketContext);
                                if (clientId == null)
                                {
                                    throw new Exception(Resource1.InvalidClientId);
                                }

                                AddClient(clientId, webSocketContext.WebSocket);

                                _logger.Info(string.Format(Resource1.ClientConnected, clientId));

                                // Serve websocket on a separate task so that listener can accept new connections
                                // Do not await the task, because it runs as long as the client is connected
                                _ = Task.Run(
                                    () => ServeWebSocketAsync(clientId, webSocketContext.WebSocket)
                                        .ConfigureAwait(false), _cancellationToken);
                            }
                            catch (Exception e)
                            {
                                _logger.Error(e.Message);
                                httpContext.Response.StatusCode = 500;
                                httpContext.Response.StatusDescription = e.Message;
                                httpContext.Response.Close();
                            }
                        }
                        else
                        {
                            _logger.Error(Resource1.NoWebsocketRequest);
                            httpContext.Response.StatusCode = ResponseCodes.BadRequest;
                            httpContext.Response.StatusDescription = Resource1.NoWebsocketRequest;
                            httpContext.Response.Close();
                        }
                    }
                    else
                    {
                        _logger.Info(Resource1.ServerShuttingDown);
                        httpContext.Response.StatusCode = 409;
                        httpContext.Response.StatusDescription = Resource1.ServerShuttingDown;
                        httpContext.Response.Close();
                    }
                }
                catch (HttpListenerException httpListenerException)
                {
                    // This happens when _httpListener.Close() is beeing called.
                    if (httpListenerException?.ErrorCode == 995)
                    {
                        _logger.Info(httpListenerException.Message);
                        break;
                    }
                }
                catch (Exception e)
                {
                    if (_cancellationToken.IsCancellationRequested) return;
                    _logger.Info(e.Message);
                }
            }
        }

        private async Task ServeWebSocketAsync(string clientId, WebSocket webSocket)
        {
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    // Receive message
                    var messageBytes = await ReceiveAsync(webSocket, _cancellationToken);

                    if (webSocket.State is WebSocketState.Closed or WebSocketState.Aborted)
                    {
                        _logger.Info(string.Format(Resource1.ClientDisconnected, clientId));
                        RemoveClient(clientId);
                        webSocket.Dispose();
                        break;
                    }

                    // Handle message
                    try
                    {
                        var message = Converter.Deserialize(Encoding.UTF8.GetString(messageBytes));
                        switch (message.Code)
                        {
                            case RequestCodes.Request:
                                try
                                {
                                    var requestReceivedEventArgs = new RequestMessageEventArgs(message);

                                    RaiseRequestMessageReceived(requestReceivedEventArgs);

                                    await SendAsync(webSocket, Encoding.UTF8.GetBytes(Converter.Serialize(requestReceivedEventArgs.ResponseMessage)), _cancellationToken);
                                }
                                catch (Exception e)
                                {
                                    var responseMessage = e switch
                                    {
                                        HttpListenerException listenerException => new Message(listenerException.ErrorCode, 0, null, (VariantValue)listenerException.Message),

                                        IoTCoreException ioTCoreException => new Message(ioTCoreException.ResponseCode, 0, null, (VariantValue)ioTCoreException.Message),

                                        _ => new Message(ResponseCodes.InternalError, 0, null, (VariantValue)e.Message)
                                    };

                                    await SendAsync(webSocket, Encoding.UTF8.GetBytes(Converter.Serialize(responseMessage)), _cancellationToken);
                                }
                                break;

                            case RequestCodes.Event:
                                try
                                {
                                    RaiseEventMessageReceived(new EventMessageEventArgs(message));
                                }
                                catch (Exception e)
                                {
                                    // Events do not have a response, so no error message is returned
                                    _logger.Error(e.Message);
                                }
                                break;

                            default:
                                {
                                    // Try to return an error message
                                    var responseMessage = new Message(ResponseCodes.BadRequest,
                                        0,
                                        null,
                                        (VariantValue)string.Format(Resource1.InvalidMessageCode, message.Code));

                                    await SendAsync(webSocket, Encoding.UTF8.GetBytes(Converter.Serialize(responseMessage)), _cancellationToken);
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                    }
                }

                webSocket.Dispose();
                RemoveClient(clientId);
            }
            catch (Exception)
            {
                webSocket.Dispose();
                RemoveClient(clientId);
            }
        }

        private void AddClient(string clientId, WebSocket webSocket)
        {
            if (!ConnectedClients.TryAdd(clientId, webSocket))
            {
                throw new Exception(string.Format(Resource1.ClientAlreadyConnected, clientId));
            }

            this.RaiseClientAdded(clientId);
        }

        private void RemoveClient(string clientId)
        {
            if (ConnectedClients.TryRemove(clientId, out _))
            {
                this.RaiseClientRemoved(clientId);
            }
        }

        private static string GetClientId(WebSocketContext webSocketContext)
        {
            return HttpUtility.ParseQueryString(webSocketContext.RequestUri.Query).Get("clientid");
        }

        public async Task SendEventAsync(string clientId, Message message)
        {
            if (ConnectedClients.TryGetValue(clientId, out var webSocket))
            {
                await SendAsync(webSocket, Encoding.UTF8.GetBytes(Converter.Serialize(message)), _cancellationToken);
            }
            else
            {
                throw new Exception(string.Format(Resource1.ClientNotConnected, clientId));
            }
        }

        public void Dispose()
        {
            _httpListener.Close();
            _cancellationTokenSource.Dispose();
        }

        protected void RaiseEventMessageReceived(EventMessageEventArgs eventMessageEventArgs)
        {
            EventMessageReceived.Raise(this, eventMessageEventArgs);
        }

        protected void RaiseRequestMessageReceived(RequestMessageEventArgs requestReceivedEventArgs)
        {
            RequestMessageReceived.Raise(this, requestReceivedEventArgs);
        }
    }
}
