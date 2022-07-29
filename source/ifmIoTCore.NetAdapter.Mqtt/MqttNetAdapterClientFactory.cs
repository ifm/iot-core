namespace ifmIoTCore.NetAdapter.Mqtt
{
    using System.Collections.Generic;
    using System;
    using System.Net;
    using Messages;

    public class MqttNetAdapterClientFactory : IClientNetAdapterFactory
    {
        private readonly IMessageConverter _converter;
        private readonly IDictionary<MqttRemoteInfo, IClientNetAdapter> _clients = new Dictionary<MqttRemoteInfo, IClientNetAdapter>();
        private bool _disposed;

        public MqttNetAdapterClientFactory(IMessageConverter converter, TimeSpan defaultTimeout)
        {
            this._converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.DefaultTimeout = defaultTimeout;
        }

        public MqttNetAdapterClientFactory(IMessageConverter converter)
            : this(converter, TimeSpan.FromSeconds(30))
        {
        }

        public TimeSpan DefaultTimeout { get; }

        public string Scheme => "mqtt";

        public string Format => _converter.Type;

        public IClientNetAdapter CreateClient(Uri remoteUri)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException($"Access to disposed instance of {typeof(MqttNetAdapterClientFactory)}.");
            }

            var remoteInfo = new MqttRemoteInfo(remoteUri, remoteUri.PathAndQuery.TrimStart('/'));

            if (_clients.TryGetValue(remoteInfo, out var client))
            {
                return client;
            }

            var result = new MqttClientNetAdapter(remoteUri.PathAndQuery.TrimStart('/'), new IPEndPoint(IPAddress.Parse(remoteUri.Host), remoteUri.Port), this._converter, this.DefaultTimeout);
            this._clients.Add(remoteInfo, result);

            return result;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            var items = _clients.Values;
            _clients.Clear();
            
            foreach (var item in items)
            {
                try
                {
                    item.Dispose();
                }
                catch (Exception)
                {
                    //Ignore
                }

                item.Dispose();
            }

            _disposed = true;
        }

        internal class MqttRemoteInfo : IEquatable<MqttRemoteInfo>
        {
            public Uri RemoteUri { get; }
            public string Topic { get; }

            public MqttRemoteInfo(Uri remoteUri, string topic)
            {
                RemoteUri = remoteUri;
                Topic = topic;
            }

            public bool Equals(MqttRemoteInfo other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(RemoteUri, other.RemoteUri) && Topic == other.Topic;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((MqttRemoteInfo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((RemoteUri != null ? RemoteUri.GetHashCode() : 0) * 397) ^ (Topic != null ? Topic.GetHashCode() : 0);
                }
            }
        }
    }
}