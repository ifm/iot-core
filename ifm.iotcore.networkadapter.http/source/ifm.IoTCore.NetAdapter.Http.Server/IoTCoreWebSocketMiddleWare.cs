using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ifm.IoTCore.Common;
using ifm.IoTCore.MessageConverter.Contracts;
using ifm.IoTCore.MessageDispatcher.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal class IoTCoreWebSocketMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<WebSocketOptions> _options;
        private readonly IWebsocketContextManager _websocketContextManager;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IMessageConverter _messageConverter;
        private readonly IMessageFromExceptionBuilder _messageFromExceptionBuilder;
        private readonly ILogger<IoTCoreWebSocketMiddleWare> _logger;

        public IoTCoreWebSocketMiddleWare(
            RequestDelegate next,
            IOptions<WebSocketOptions> options,
            IWebsocketContextManager websocketContextManager,
            IMessageDispatcher messageDispatcher,
            IMessageConverter messageConverter,
            IMessageFromExceptionBuilder messageFromExceptionBuilder,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _options = options;
            _websocketContextManager = websocketContextManager;
            _messageDispatcher = messageDispatcher;
            _messageConverter = messageConverter;
            _messageFromExceptionBuilder = messageFromExceptionBuilder;
            _logger = loggerFactory.CreateLogger<IoTCoreWebSocketMiddleWare>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Query.TryGetValue("clientId", out var clientId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            // Step 1: Accept the WebSocket. This sends the 101 response automatically.
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            // Step 2: Register so other parts of your app can send TO this client.
            _websocketContextManager.RegisterWebsocketContext(clientId!, webSocket);

            try
            {
                // Step 3: Receive loop — this keeps the connection alive.
                // The method must NOT return until the WebSocket is closed,
                // otherwise ASP.NET Core tears down the connection.
                await ReceiveLoopAsync(clientId!, webSocket, context.RequestAborted);
            }
            finally
            {
                // Step 4: Clean up when the connection ends.
                // Remove from manager so nobody tries to send on a dead socket.
                _websocketContextManager.TryGetWebsocketContext(clientId!, out _);
                // TODO: Add a Remove method to IWebsocketContextManager
            }
        }

        private async Task ReceiveLoopAsync(string clientId, WebSocket webSocket, CancellationToken ct)
        {
            var buffer = new byte[4096];

            while (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    // This awaits until the client sends a message or closes.
                    var result = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("Client {ClientId} sent close frame", clientId);
                        await webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Closing",
                            CancellationToken.None);
                        return;
                    }

                    // Process the received message
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation("Received from {ClientId}: {Message}", clientId, message);

                    try
                    {
                        var serializedRequest = _messageConverter.Deserialize(message);

                        switch (serializedRequest.Code)
                        {
                            case RequestCodes.Event:
                                _messageDispatcher.HandleEvent(serializedRequest);
                                break;
                            case RequestCodes.Request:
                                var resp = _messageDispatcher.HandleRequest(serializedRequest);

                                var serializedResponse = _messageConverter.Serialize(resp);

                                var responseBytes = Encoding.UTF8.GetBytes(serializedResponse);
                                await webSocket.SendAsync(responseBytes, WebSocketMessageType.Text, true,
                                    CancellationToken.None);
                                break;
                        }
                    }
                    catch (Exception innerException)
                    {
                        _logger.LogError(innerException, "Error processing message from client {ClientId}", clientId);
                        var response = _messageFromExceptionBuilder.BuildMessageFromException(innerException);
                        var serializedErrorReponse = _messageConverter.Serialize(response);
                        var responseBytes = Encoding.UTF8.GetBytes(serializedErrorReponse);
                        await webSocket.SendAsync(responseBytes, WebSocketMessageType.Text, true,
                            CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in WebSocket receive loop for client {ClientId}", clientId);
                    break;
                }
            }
        }
    }
}
