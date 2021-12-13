namespace ifmIoTCore.NetAdapter.Mqtt
{
    using System;
    using System.Net;

    public class MqttNetAdapterClientFactory : IClientNetAdapterFactory
    {
        private readonly IConverter _converter;

        public MqttNetAdapterClientFactory(IConverter converter, TimeSpan defaultTimeout)
        {
            this._converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.DefaultTimeout = defaultTimeout;
        }

        public MqttNetAdapterClientFactory(IConverter converter)
            : this(converter, TimeSpan.FromSeconds(30))
        {
        }

        public TimeSpan DefaultTimeout { get; }
        public string Protocol => "mqtt";

        public IClientNetAdapter CreateClient(Uri remoteUri)
        {
            var result = new MqttClientNetAdapter(remoteUri.PathAndQuery.TrimStart('/'), new IPEndPoint(IPAddress.Parse(remoteUri.Host), remoteUri.Port), this._converter, this.DefaultTimeout);
            return result;
        }

        public void Dispose()
        {
        }
    }
}