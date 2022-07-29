namespace Sample26
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using ifmIoTCore.Common.Variant;
    using ifmIoTCore.Elements.ServiceData;
    using ifmIoTCore.Elements.ServiceData.Events;
    using ifmIoTCore.Elements.ServiceData.Requests;
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

            client.SendRequest(new Message(RequestCodes.Request, 10, "/gettree", Variant.FromObject(new GetTreeRequestServiceData("/", 0))));

            client.SendEvent(
                new Message(RequestCodes.Event, 10, "/serviceHandler", 
                    Variant.FromObject(new EventServiceData(0, "/identifier", 
                        new Dictionary<string, CodeDataPair>
                        {
                            {
                                "/identifier",
                                new CodeDataPair(200, Variant.FromObject(3))
                            }
                        },
                        1))));
        }
    }
}