namespace Sample28
{
    using System.Net;
    using System.Threading;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.NetAdapter.Mqtt;

    internal class Program
    {
        static void Main()
        {
            using (var manualResetEvent = new ManualResetEventSlim())
            using (var iotCore = IoTCoreFactory.Create("id"))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, 
                       iotCore.Root, 
                       new MessageConverter(), 
                       new IPEndPoint(IPAddress.Loopback, 1883)))
            using (var mqttClientNetAdapterFactory = 
                   new MqttNetAdapterClientFactory(new MessageConverter()))
            {
                iotCore.RegisterClientNetAdapterFactory(mqttClientNetAdapterFactory);

                mqttServerNetAdapter.RequestReceived += (s, e) =>
                {
                    e.ResponseMessage = iotCore.HandleRequest(e.RequestMessage);
                };

                mqttServerNetAdapter.EventReceived += (s, e) =>
                {
                    iotCore.HandleEvent(e.EventMessage);
                };

                iotCore.RegisterServerNetAdapter(mqttServerNetAdapter);

                var serviceElement = new ActionServiceElement("stop", (element, i) =>
                {
                    manualResetEvent.Set();
                });

                iotCore.Root.AddChild(serviceElement);

                mqttServerNetAdapter.Start();

                manualResetEvent.Wait();

                mqttServerNetAdapter.Stop();
            }
        }
    }
}