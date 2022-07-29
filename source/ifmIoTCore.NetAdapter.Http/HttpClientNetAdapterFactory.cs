namespace ifmIoTCore.NetAdapter.Http
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Messages;

    public class HttpClientNetAdapterFactory : IClientNetAdapterFactory
    {
        private readonly IMessageConverter _converter;
        private readonly TimeSpan _timeout;
        private readonly bool _keepAlive;
        private readonly ConcurrentDictionary<Uri, HttpClientNetAdapter> _clients = new ConcurrentDictionary<Uri, HttpClientNetAdapter>();
        private readonly CancellationTokenSource _cancelCleanup = new CancellationTokenSource();

        public string Scheme => "http";

        public string Format => _converter.Type;

        public HttpClientNetAdapterFactory(IMessageConverter converter) : this(converter, TimeSpan.FromSeconds(30), true)
        {
        }

        public HttpClientNetAdapterFactory(IMessageConverter converter, bool keepAlive) : this(converter, TimeSpan.FromSeconds(30), keepAlive)
        {
        }

        public HttpClientNetAdapterFactory(IMessageConverter converter, TimeSpan timeout, bool keepAlive)
        {
            _converter = converter;
            _timeout = timeout;
            _keepAlive = keepAlive;

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
                    client = new HttpClientNetAdapter(remoteUri, _converter, _timeout, _keepAlive);
                    _clients.TryAdd(remoteUri, client);
                }
            }
            else
            {
                client = new HttpClientNetAdapter(remoteUri, _converter, _timeout, _keepAlive);
            }
            return client;
        }

        private Task RunCleanupTaskAsync()
        {
            return Task.Run(async () =>
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