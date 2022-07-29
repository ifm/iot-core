namespace Sample25
{
    using System;
    using System.Net;
    using System.Threading;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Logger;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.NetAdapter.Mqtt;

    class Program
    {
        static void Main(string[] args)
        {
            var ioTCore = IoTCoreFactory.Create("test", null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));

            MqttServerNetAdapter mqttServerNetAdapter = new MqttServerNetAdapter(ioTCore, ioTCore.Root, new MessageConverter(), new IPEndPoint(IPAddress.Loopback, 1883), DisconnectionHandler, "commandTopic");

            void HandleRequestReceived (object sender, RequestMessageEventArgs e)
            {
                e.ResponseMessage = ioTCore.HandleRequest(e.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequestReceived;

            void HandleEventReceived(object sender, EventMessageEventArgs e)
            {
                ioTCore.HandleEvent(e.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEventReceived;

            ioTCore.RegisterServerNetAdapter(mqttServerNetAdapter);

            HttpServerNetAdapter httpServerNetAdapter = new HttpServerNetAdapter(ioTCore, new Uri("http://127.0.0.1:8090"), new MessageConverter());
            ioTCore.RegisterServerNetAdapter(httpServerNetAdapter);

            httpServerNetAdapter.Start();
            mqttServerNetAdapter.Start();

            using (var manualResetEvent = new ManualResetEventSlim()) {

                var stopServiceElement = new ActionServiceElement("stop",
                    (element, i) => { manualResetEvent.Set(); }
                    );

                ioTCore.Root.AddChild(stopServiceElement);

                manualResetEvent.Wait();
            }

            httpServerNetAdapter.Dispose();
            mqttServerNetAdapter.RequestReceived -= HandleRequestReceived;
            mqttServerNetAdapter.EventReceived -= HandleEventReceived;
            mqttServerNetAdapter.Dispose();


        }

        private static void DisconnectionHandler(Exception exception)
        {
            // Handle disconnection here.
        }
    }
}