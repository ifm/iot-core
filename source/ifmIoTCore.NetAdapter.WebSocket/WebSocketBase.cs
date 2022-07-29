namespace ifmIoTCore.NetAdapter.WebSocket
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    internal class WebSocketBase
    {
        protected const int BufferSize = 4096;

        protected async Task SendAsync(WebSocket webSocket, byte[] data, CancellationToken cancellationToken)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cancellationToken);
        }

        protected async Task<byte[]> ReceiveAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = WebSocket.CreateServerBuffer(BufferSize);
            WebSocketReceiveResult receiveResult = null;
            var requestBytes = new List<byte>();
            while (webSocket.State == WebSocketState.Open && (receiveResult == null || !receiveResult.EndOfMessage))
            {
                receiveResult = await webSocket.ReceiveAsync(buffer, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }
                if (webSocket.State == WebSocketState.CloseReceived && receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Acknowledge close frame", CancellationToken.None);
                }

                if (buffer.Array != null && receiveResult.Count > 0)
                {
                    requestBytes.AddRange(buffer.Take(receiveResult.Count));
                }
            }

            return requestBytes.ToArray();
        }
    }
}