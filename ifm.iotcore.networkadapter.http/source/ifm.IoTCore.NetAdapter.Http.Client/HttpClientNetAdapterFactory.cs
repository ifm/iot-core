namespace ifm.IoTCore.NetAdapter.Http.Client
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Logger.Contracts;

    using MessageConverter.Contracts;
    using NetAdapter.Contracts.Client;

    public class HttpClientNetAdapterFactory : IClientNetAdapterFactory
    {
        private readonly IMessageConverter _converter;
        private readonly TimeSpan _timeout;
        private readonly bool _keepAlive;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<Uri, HttpClientNetAdapter> _clients = new();
        private readonly CancellationTokenSource _cancelCleanup = new();

        public string Scheme => "http";
        public string[] Schemes => [];

        public string Format => _converter.Type;

        public HttpClientNetAdapterFactory(IMessageConverter converter, ILogger logger = null) : this(converter, TimeSpan.FromSeconds(30), true, logger)
        {
        }

        public HttpClientNetAdapterFactory(IMessageConverter converter, bool keepAlive, ILogger logger = null) : this(converter, TimeSpan.FromSeconds(30), keepAlive, logger)
        {
        }

        public HttpClientNetAdapterFactory(IMessageConverter converter, TimeSpan timeout, bool keepAlive, ILogger logger = null)
        {
            _converter = converter;
            _timeout = timeout;
            _keepAlive = keepAlive;
            _logger = logger;

            if (_keepAlive)
            {
                RunCleanupTaskAsync();
            }
        }

        public IClientNetAdapter CreateClient(Uri remoteUri)
        {
            HttpClientNetAdapter client;
            if (_keepAlive)
            {
                if (!_clients.TryGetValue(remoteUri, out client))
                {
                    client = new HttpClientNetAdapter(remoteUri, _converter, _timeout, _keepAlive, null, _logger);
                    _clients.TryAdd(remoteUri, client);
                }
            }
            else
            {
                client = new HttpClientNetAdapter(remoteUri, _converter, _timeout, _keepAlive, null, _logger);
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
    }
}