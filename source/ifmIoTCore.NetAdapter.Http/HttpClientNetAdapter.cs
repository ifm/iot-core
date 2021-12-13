namespace ifmIoTCore.NetAdapter.Http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Text;
    using Messages;

    public class HttpClientNetAdapter : IClientNetAdapter
    {
        private readonly HttpClient _client;
        private readonly IConverter _converter;
        private readonly Uri _remoteUri;
        private readonly bool _keepAlive;

        public HttpClientNetAdapter(Uri remoteUri, IConverter converter, TimeSpan timeout, bool keepAlive)
        {
            _remoteUri = remoteUri;
            _converter = converter;
            _client = new HttpClient
            {
                Timeout = timeout
            };
            _keepAlive = keepAlive;
            if (_keepAlive)
            {
                _client.DefaultRequestHeaders.ConnectionClose = false;
                _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            }
        }

        public DateTime LastUsed { get; private set; }

        public Uri GetLocalUri()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                return new Uri($"http://{GetLocalEndpoint(socket, _remoteUri)}");
            }
        }

        public Uri GetRemoteUri()
        {
            return _remoteUri;
        }

        public ResponseMessage SendRequest(RequestMessage message, TimeSpan? timeout)
        {
            LastUsed = DateTime.Now;
            var requestString = _converter.Serialize(message);

            var task = _client.PostAsync(_remoteUri, new StringContent(requestString, Encoding.UTF8, _converter.ContentType));
            var httpResponse = task.GetAwaiter().GetResult();
            var responseString = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return _converter.Deserialize<ResponseMessage>(responseString);
        }

        public void SendEvent(EventMessage message)
        {
            SendEventFireAndForget(message);
        }

        public void Disconnect()
        {
            if (!_keepAlive)
            {
                _client.Dispose();
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static IPEndPoint GetLocalEndpoint(Socket socket, Uri remoteUri)
        {
            socket.Connect(IPAddress.Parse(remoteUri.Host), remoteUri.Port);
            var localEndPoint = socket.LocalEndPoint;
            socket.Close();
            return localEndPoint as IPEndPoint;
        }

        private void SendEventExpectResponse(EventMessage message)
        {
            LastUsed = DateTime.Now;
            var requestString = _converter.Serialize(message);
            var task = _client.PostAsync(_remoteUri, new StringContent(requestString, Encoding.UTF8, _converter.ContentType));
            var response = task.GetAwaiter().GetResult();
        }

        private void SendEventFireAndForget(EventMessage message)
        {
            var httpNewLine = "\r\n";

            LastUsed = DateTime.Now;
            var requestString = _converter.Serialize(message);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(_remoteUri.Host, _remoteUri.Port);
                var content = $"POST {message.Address} HTTP/1.0{httpNewLine}Content-Type: {_converter.ContentType}{httpNewLine}Content-Length: {requestString.Length}{httpNewLine}{httpNewLine}{requestString}";
                var byteArrayToSend = Encoding.UTF8.GetBytes(content);
                socket.Send(byteArrayToSend);
                socket.Close();
            }
        }
    }
}