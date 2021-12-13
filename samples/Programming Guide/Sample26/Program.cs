namespace Sample26
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using ifmIoTCore.Converter.Json;
    using ifmIoTCore.Elements.ServiceData;
    using ifmIoTCore.Elements.ServiceData.Events;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter;
    using ifmIoTCore.NetAdapter.Mqtt;
    using ifmIoTCore.Utilities;
    using Newtonsoft.Json.Linq;

    class Program
    {
        static void Main()
        {
            IClientNetAdapter client = new MqttClientNetAdapter(
                "cmdTopic", 
                new IPEndPoint(
                    IPAddress.Parse("192.168.1.1"), 
                    1883), 
                new JsonConverter(), 
                TimeSpan.FromSeconds(1));

            client.SendRequest(new RequestMessage(10, "/gettree", Helpers.ToJson(new GetTreeRequestServiceData("/", 0))));

            client.SendEvent(
                new EventMessage(10, "/serviceHandler", 
                    Helpers.ToJson(new EventServiceData(0, "/identifier", 
                        new Dictionary<string, CodeDataPair>
                        {
                            {
                                "/identifier",
                                new CodeDataPair(200,JToken.FromObject(3))
                            }
                        },
                        1))));
        }
    }
}