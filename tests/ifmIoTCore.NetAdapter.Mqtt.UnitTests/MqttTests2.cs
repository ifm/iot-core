using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ifmIoTCore.Converter.Json;
using ifmIoTCore.Messages;
using MQTTnet.Diagnostics;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Implementations;
using MQTTnet.Server;
using NUnit.Framework;

namespace ifmIoTCore.NetAdapter.Mqtt.UnitTests
{
    [TestFixture]
    public class MqttTests2
    {
        [Test, NonParallelizable]
        public async Task TestMqttClient()
        {
            var factory = new MqttNetAdapterClientFactory(new JsonConverter());
            
            using (var iotCore = IoTCoreFactory.Create("id"))
            using (var client = factory.CreateClient(new Uri("mqtt://127.0.0.1:1884/cmdTopic")))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new JsonConverter(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1884)))
            using (var mqttBroker = new MqttServer(new[] {new MqttTcpServerAdapter(new MqttNetNullLogger())}, new MqttNetNullLogger()))
            {
                await mqttBroker.StartAsync(
                new MqttServerOptionsBuilder()
                    .WithDefaultEndpointBoundIPAddress(IPAddress.Parse("127.0.0.1"))
                    .WithDefaultEndpointReuseAddress()
                    .WithDefaultEndpointPort(1884)
                    .Build()
                );

                mqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e => { TestContext.Out.WriteLine(e.ClientId); });

                await mqttServerNetAdapter.StartAsync();
                string message = null;
                mqttServerNetAdapter.EventMessageReceived += (s, e) =>
                {
                    message = e.EventMessage.ToString();
                };

                client.SendEvent(new EventMessage(10, "/Hello", null));
                await Task.Delay(1000);
                Assert.IsNotNull(message);
                mqttBroker.ClientConnectedHandler = null;
            }
        }

        [Test, NonParallelizable]
        public async Task TestMqttClientGetIdentity()
        {
            var factory = new MqttNetAdapterClientFactory(new JsonConverter());

            using (var iotCore = IoTCoreFactory.Create("id"))
            using (var client = factory.CreateClient(new Uri("mqtt://127.0.0.1:1884/cmdTopic")))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new JsonConverter(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1884)))
            using (var mqttBroker = new MqttServer(new[] { new MqttTcpServerAdapter(new MqttNetNullLogger()) }, new MqttNetNullLogger()))
            {
                await mqttBroker.StartAsync(
                    new MqttServerOptionsBuilder()
                        .WithDefaultEndpointBoundIPAddress(IPAddress.Parse("127.0.0.1"))
                        .WithDefaultEndpointReuseAddress()
                        .WithDefaultEndpointPort(1884)
                        .Build()
                );

                mqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(e => { TestContext.Out.WriteLine(e.ClientId); });

                await mqttServerNetAdapter.StartAsync();
                mqttServerNetAdapter.RequestMessageReceived += (s, e) =>
                {
                    e.Response = iotCore.HandleRequest(e.Request);

                    TestContext.Out.WriteLine(e.Request);
                    TestContext.Out.WriteLine(e.Response);
                };

                var response = client.SendRequest(new RequestMessage(0, "/getidentity", null, reply:":replyTopic"));

                await Task.Delay(1000);
                Assert.IsNotNull(response);
                mqttBroker.ClientConnectedHandler = null;
            }
        }
    }
}