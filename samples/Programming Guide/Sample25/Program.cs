namespace Sample25
{
    using System;
    using System.Net;
    using System.Threading;
    using ifmIoTCore;
    using ifmIoTCore.Converter.Json;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.NetAdapter.Mqtt;
    using ifmIoTCore.Utilities;

    class Program
    {
        static void Main(string[] args)
        {
            var ioTCore = IoTCoreFactory.Create("test", new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));

            MqttServerNetAdapter mqttServerNetAdapter = new MqttServerNetAdapter(ioTCore, ioTCore.Root, new JsonConverter(), new IPEndPoint(IPAddress.Loopback, 1883), DisconnectionHandler, "commandTopic");

            void HandleRequestReceived (object sender, RequestMessageEventArgs e)
            {
                e.Response = ioTCore.HandleRequest(e.Request);
            }

            mqttServerNetAdapter.RequestMessageReceived += HandleRequestReceived;

            void HandleEventReceived(object sender, EventMessageEventArgs e)
            {
                ioTCore.HandleEvent(e.EventMessage);
            }

            mqttServerNetAdapter.EventMessageReceived += HandleEventReceived;

            ioTCore.RegisterServerNetAdapter(mqttServerNetAdapter);

            HttpServerNetAdapter httpServerNetAdapter = new HttpServerNetAdapter(ioTCore, new Uri("http://127.0.0.1:8090"),new JsonConverter());
            ioTCore.RegisterServerNetAdapter(httpServerNetAdapter);

            httpServerNetAdapter.Start();
            mqttServerNetAdapter.Start();

            using (var manualResetEvent = new ManualResetEventSlim()) {

                var stopServiceElement = ioTCore.CreateActionServiceElement(ioTCore.Root, "stop", 
                    (element, i) => { manualResetEvent.Set(); });

                manualResetEvent.Wait();
            }

            httpServerNetAdapter.Dispose();
            mqttServerNetAdapter.RequestMessageReceived -= HandleRequestReceived;
            mqttServerNetAdapter.EventMessageReceived -= HandleEventReceived;
            mqttServerNetAdapter.Dispose();


        }

        private static void DisconnectionHandler(Exception exception)
        {
            // Handle disconnection here.
        }
    }
}