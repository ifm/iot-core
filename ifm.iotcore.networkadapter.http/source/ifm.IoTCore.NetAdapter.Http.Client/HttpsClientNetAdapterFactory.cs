namespace ifm.IoTCore.NetAdapter.Http.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using MessageConverter.Contracts;
    using NetAdapter.Contracts.Client;

    public class HttpsClientNetAdapterFactory : IClientNetAdapterFactory
    {
        private readonly IMessageConverter _converter;
        private readonly TimeSpan _timeout;
        private readonly bool _keepAlive;
        private readonly Func<X509Certificate2[]> _clientCertificateProvider;
        private readonly Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> _serverCertificateValidation;
        private readonly ConcurrentDictionary<Uri, HttpsClientNetAdapter> _clients = new();
        private readonly CancellationTokenSource _cancelCleanup = new();

        /// <summary>
        /// Initializes a new instance of <see cref="HttpsClientNetAdapterFactory"/>.
        /// </summary>
        /// <param name="converter">The messageconverter to de/serialize messages.</param>
        /// <param name="serverCertificateValidation">A callback that validates the server certificate of the server the clients of this factory will connect to.</param>
        /// <param name="clientCertificateProvider">If the clients created by this factory must present their own certificates to the server, this collection contains the client certificates, otherwise null.</param>
        public HttpsClientNetAdapterFactory(IMessageConverter converter,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> serverCertificateValidation,
            Func<X509Certificate2[]> clientCertificateProvider) 
            : this(converter, TimeSpan.FromSeconds(30), true, serverCertificateValidation, clientCertificateProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpsClientNetAdapterFactory"/>.
        /// </summary>
        /// <param name="converter">The messageconverter to de/serialize messages.</param>
        /// <param name="keepAlive">if true, keep the client connection alive.</param>
        /// <param name="serverCertificateValidation">A callback that validates the server certificate of the server the clients of this factory will connect to.</param>
        /// <param name="clientCertificateProvider">If the clients created by this factory must present their own certificates to the server, this collection contains the client certificates, otherwise null.</param>
        public HttpsClientNetAdapterFactory(IMessageConverter converter, 
            bool keepAlive, 
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> serverCertificateValidation, 
            Func<X509Certificate2[]> clientCertificateProvider) 
            : this(converter, TimeSpan.FromSeconds(30), keepAlive, serverCertificateValidation, clientCertificateProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HttpsClientNetAdapterFactory"/>.
        /// </summary>
        /// <param name="converter">The messageconverter to de/serialize messages.</param>
        /// <param name="timeout">The timeout for client connections.</param>
        /// <param name="keepAlive">if true, keep the client connection alive.</param>
        /// <param name="serverCertificateValidation">A callback that validates the server certificate of the server the clients of this factory will connect to.</param>
        /// <param name="clientCertificateProvider">If the clients created by this factory must present their own certificates to the server, this collection contains the client certificates, otherwise null.</param>
        public HttpsClientNetAdapterFactory(IMessageConverter converter, 
            TimeSpan timeout, 
            bool keepAlive, 
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> serverCertificateValidation, 
            Func<X509Certificate2[]> clientCertificateProvider)
        {
            _converter = converter;
            _timeout = timeout;
            _keepAlive = keepAlive;
            _clientCertificateProvider = clientCertificateProvider;
            _serverCertificateValidation = serverCertificateValidation;

            if (_keepAlive)
            {
                RunCleanupTaskAsync();
            }
        }

        private void RunCleanupTaskAsync()
        {
            Task.Run(async () =>
            {
                while (!_cancelCleanup.IsCancellationRequested)
                {
                    foreach (var client in _clients)
                    {
                        if (DateTime.Now - client.Value.LastUsed > TimeSpan.FromSeconds(60))
                        {
                            _clients.TryRemove(client.Key, out _);
                        }
                    }

                    await Task.Delay(100);
                }
            });
        }

        public void Dispose()
        {
            _cancelCleanup.Cancel();
        }

        public string Scheme => "https";
        public string[] Schemes => [];
        public string Format => _converter.Type;


        public IClientNetAdapter CreateClient(Uri remoteUri)
        {
            HttpsClientNetAdapter client;
            if (_keepAlive)
            {
                if (!_clients.TryGetValue(remoteUri, out client))
                {
                    client = new HttpsClientNetAdapter(remoteUri, 
                        _converter, 
                        _timeout, 
                        _keepAlive, 
                        _serverCertificateValidation, _clientCertificateProvider == null ? new X509Certificate2Collection() : new X509Certificate2Collection(_clientCertificateProvider()));
                    _clients.TryAdd(remoteUri, client);
                }
            }
            else
            {
                client = new HttpsClientNetAdapter(remoteUri, 
                    _converter, 
                    _timeout, 
                    _keepAlive, 
                    _serverCertificateValidation, _clientCertificateProvider == null ? new X509Certificate2Collection() : new X509Certificate2Collection(_clientCertificateProvider()));
            }

            return client;
        }

        public void RemoveClient(Uri remoteUri, bool dispose)
        {
            if (_clients.TryRemove(remoteUri, out var client))
            {
                if (dispose) client.Dispose();
            }
        }
    }
}
