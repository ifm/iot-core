namespace Sample30
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using ifmIoTCore.Common.Variant;
    using ifmIoTCore.Elements.ServiceData;
    using ifmIoTCore.Elements.ServiceData.Events;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter;
    using ifmIoTCore.NetAdapter.Mqtt;

    class Program
    {
        static void Main()
        {
            IClientNetAdapter client = new MqttClientNetAdapter(
                "cmdTopic",
                new IPEndPoint(
                    IPAddress.Parse("192.168.1.1"),
                    1883),
                new MessageConverter(),
                TimeSpan.FromSeconds(1));


            var eventData = new EventServiceData(0, "/identifier",
                new Dictionary<string, CodeDataPair>
                {
                    {
                        "/identifier",
                        new CodeDataPair(ResponseCodes.Success, Variant.FromObject(3))
                    }
                });

            client.SendEvent(new Message(10, 
                0, 
                "/serviceHandler", 
                Variant.FromObject(eventData)));
        }
    }
}