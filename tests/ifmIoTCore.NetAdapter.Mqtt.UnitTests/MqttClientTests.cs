using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ifmIoTCore.Converter.Json;
using ifmIoTCore.Elements.ServiceData;
using ifmIoTCore.Elements.ServiceData.Events;
using ifmIoTCore.Messages;
using MQTTnet.Adapter;
using MQTTnet.Diagnostics;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Implementations;
using MQTTnet.Server;
using NUnit.Framework;

namespace ifmIoTCore.NetAdapter.Mqtt.UnitTests
{
    [TestFixture]
    public class MqttClientTests
    {
        private const int LocalBrokerPort = 1884;

        private MqttServer _mqttServer;

        [OneTimeSetUp]
        public async Task Setup()
        {
            this._mqttServer = new MqttServer(
                new List<IMqttServerAdapter>
                {
                    new MqttTcpServerAdapter(new MqttNetNullLogger())
                }, new MqttNetNullLogger());
            await _mqttServer.StartAsync(new MqttServerOptions { DefaultEndpointOptions = { BoundInterNetworkAddress = IPAddress.Loopback, Port = LocalBrokerPort, ReuseAddress = true } });
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await this._mqttServer.StopAsync();
            this._mqttServer.Dispose();

            await Task.Delay(TimeSpan.FromMilliseconds(300));
        }

        [Test, NonParallelizable]
        public void TestReplyTopicSuccess()
        {
            using (var iotCore = IoTCoreFactory.Create("id0"))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new JsonConverter(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), LocalBrokerPort)))
            using (var mqttClient = new MqttClientNetAdapter("cmdTopic", new IPEndPoint(IPAddress.Parse("127.0.0.1"), LocalBrokerPort), new JsonConverter(), TimeSpan.FromSeconds(1)))
            {
                mqttServerNetAdapter.Start();

                void OnMqttServerNetAdapterOnRequestMessageReceived(object s, RequestMessageEventArgs e)
                {
                    e.Response = iotCore.HandleRequest(e.Request);
                }

                mqttServerNetAdapter.RequestMessageReceived += OnMqttServerNetAdapterOnRequestMessageReceived;

                void OnMqttServerNetAdapterOnEventMessageReceived(object s, EventMessageEventArgs e)
                {
                    iotCore.HandleEvent(e.EventMessage);
                }

                mqttServerNetAdapter.EventMessageReceived += OnMqttServerNetAdapterOnEventMessageReceived;

                var response = mqttClient.SendRequest(new RequestMessage(0, "/getidentity", null, $":{mqttServerNetAdapter.ReplyTopic}"), TimeSpan.FromSeconds(2));

                Assert.NotNull(response);
            }
        }

        [Test, NonParallelizable]
        [TestCase("/gettree")]  // adr only
        [TestCase("/gettree: ")] // reply empty
        [TestCase(":")] // yikes!
        [TestCase(null)]
        public void TestReplyTopic(string reply)
        {
            using (var iotCore = IoTCoreFactory.Create("id0"))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new JsonConverter(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), LocalBrokerPort)))
            using (var mqttClient = new MqttClientNetAdapter("cmdTopic", new IPEndPoint(IPAddress.Parse("127.0.0.1"), LocalBrokerPort), new JsonConverter(), TimeSpan.FromSeconds(1)))
            {
                void OnMqttServerNetAdapterOnRequestMessageReceived(object s, RequestMessageEventArgs e)
                {
                    e.Response = iotCore.HandleRequest(e.Request);
                }

                mqttServerNetAdapter.RequestMessageReceived += OnMqttServerNetAdapterOnRequestMessageReceived;

                void OnMqttServerNetAdapterOnEventMessageReceived(object s, EventMessageEventArgs e)
                {
                    iotCore.HandleEvent(e.EventMessage);
                }

                mqttServerNetAdapter.EventMessageReceived += OnMqttServerNetAdapterOnEventMessageReceived;

                ResponseMessage response = null;

                Assert.Throws<ArgumentException>(() =>
                {
                    response = mqttClient.SendRequest(new RequestMessage(0, "/adr", null, reply: reply), TimeSpan.FromSeconds(1));
                });

                Assert.Null(response);
            }
        }
    }
}
