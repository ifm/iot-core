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
        private readonly IMessageConverter _converter;
        private readonly Uri _remoteUri;
        private readonly bool _keepAlive;

        public HttpClientNetAdapter(Uri remoteUri, IMessageConverter converter, TimeSpan timeout, bool keepAlive)
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

        public Uri GetRemoteUri()
        {
            return _remoteUri;
        }

        public Message SendRequest(Message requestMessage, TimeSpan? timeout)
        {
            LastUsed = DateTime.Now;
            var requestString = _converter.Serialize(requestMessage);

            var task = _client.PostAsync(_remoteUri, new StringContent(requestString, Encoding.UTF8, _converter.ContentType));
            var httpResponse = task.GetAwaiter().GetResult();
            var responseString = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return _converter.Deserialize(responseString);
        }

        public void SendEvent(Message eventMessage)
        {
            SendEventFireAndForget(eventMessage);
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

        private void SendEventFireAndForget(Message eventMessage)
        {
            var httpNewLine = "\r\n";

            LastUsed = DateTime.Now;
            var requestString = _converter.Serialize(eventMessage);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Connect(_remoteUri.Host, _remoteUri.Port);
                var content = $"POST {eventMessage.Address} HTTP/1.0{httpNewLine}Content-Type: {_converter.ContentType}{httpNewLine}Content-Length: {requestString.Length}{httpNewLine}{httpNewLine}{requestString}";
                var byteArrayToSend = Encoding.UTF8.GetBytes(content);
                socket.Send(byteArrayToSend);
                socket.Close();
            }
        }
    }
}