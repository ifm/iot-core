namespace DemoMqtt
{
    using System;
    using System.Net;
    using System.Threading;
    using ifmIoTCore;
    using ifmIoTCore.Converter.Json;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.NetAdapter.Mqtt;
    using ifmIoTCore.Utilities;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var uri = new Uri("mqtt://127.0.0.1:1883");

            var logger = new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug);

            using (var manualResetEvent = new ManualResetEventSlim())
            using (var iotCore = IoTCoreFactory.Create("id", logger))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new JsonConverter(), new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port)))
            using (var httpServerNetAdapter = new HttpServerNetAdapter(iotCore, new Uri("http://127.0.0.1:8090"), new JsonConverter(), logger))
            using (var mqttClientNetAdapterFactory = new MqttNetAdapterClientFactory(new JsonConverter()))
            {
                iotCore.RegisterClientNetAdapterFactory(mqttClientNetAdapterFactory);

                mqttServerNetAdapter.RequestMessageReceived += (s, e) =>
                {
                    e.Response = iotCore.HandleRequest(e.Request);
                };

                mqttServerNetAdapter.EventMessageReceived += (s, e) =>
                {
                    iotCore.HandleEvent(e.EventMessage);
                };

                iotCore.RegisterServerNetAdapter(mqttServerNetAdapter);
                iotCore.RegisterServerNetAdapter(httpServerNetAdapter);

                var eventElement = iotCore.CreateEventElement(iotCore.Root, "myEvent1", null, null, null, raiseTreeChanged: true);

                iotCore.CreateActionServiceElement(iotCore.Root, "raiseMyEvent1", (element, i) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    eventElement.RaiseEvent();
                });

                iotCore.CreateActionServiceElement(iotCore.Root, "stop", (element, i) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEvent.Set();
                });

                mqttServerNetAdapter.Start();
                httpServerNetAdapter.Start();

                manualResetEvent.Wait();

                mqttServerNetAdapter.Stop();
                httpServerNetAdapter.Stop();
            }
        }
    }
}