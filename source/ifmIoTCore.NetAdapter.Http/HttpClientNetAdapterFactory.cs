namespace ifmIoTCore.NetAdapter.Http
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpClientNetAdapterFactory : IClientNetAdapterFactory
    {
        public string Protocol => "http";

        private readonly IConverter _converter;
        private readonly TimeSpan _timeout;
        private readonly bool _keepAlive;
        private readonly ConcurrentDictionary<Uri, HttpClientNetAdapter> _clients = new ConcurrentDictionary<Uri, HttpClientNetAdapter>();
        private readonly CancellationTokenSource _cancelCleanup = new CancellationTokenSource();

        public HttpClientNetAdapterFactory(IConverter converter) : this(converter, TimeSpan.FromSeconds(30), true)
        {
        }

        public HttpClientNetAdapterFactory(IConverter converter, bool keepAlive) : this(converter, TimeSpan.FromSeconds(30), keepAlive)
        {
        }

        public HttpClientNetAdapterFactory(IConverter converter, TimeSpan timeout, bool keepAlive)
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

        private async void RunCleanupTaskAsync()
        {
            await Task.Run(() =>
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
                    Thread.Sleep(100);
                }
            });
        }

        public void Dispose()
        {
            _cancelCleanup.Cancel();
        }
    }
}