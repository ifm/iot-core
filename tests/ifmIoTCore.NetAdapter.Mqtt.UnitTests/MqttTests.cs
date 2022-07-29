
using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements;
using ifmIoTCore.Elements.ServiceData.Responses;
using Newtonsoft.Json;

namespace ifmIoTCore.NetAdapter.Mqtt.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Elements.ServiceData.Requests;
    using log4net;
    using Logger;
    using Messages;
    using MQTTnet;
    using MQTTnet.Adapter;
    using MQTTnet.Client;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Publishing;
    using MQTTnet.Client.Receiving;
    using MQTTnet.Client.Subscribing;
    using MQTTnet.Diagnostics.Logger;
    using MQTTnet.Implementations;
    using MQTTnet.Protocol;
    using MQTTnet.Server;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Utilities;

    [TestFixture]
    public class MqttTests
    {
        private const int LocalBrokerPort = 1884;

        private IIoTCore _iotCore;
        private MqttServerNetAdapter _mqttServerNetAdapter;
        private MqttServer _mqttBroker;
        
        public ILog Logger { get; } = LogManager.GetLogger(typeof(MqttTests));

        [OneTimeSetUp]
        public async Task Setup()
        {
            this._mqttBroker = new MqttServer(
                new List<IMqttServerAdapter>
                {
                    new MqttTcpServerAdapter(new MqttNetNullLogger())
                }, new MqttNetNullLogger());
            await _mqttBroker.StartAsync(new MqttServerOptions { DefaultEndpointOptions = { BoundInterNetworkAddress = IPAddress.Loopback, Port = LocalBrokerPort , ReuseAddress = true} });
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await this._mqttBroker.StopAsync();
            this._mqttBroker.Dispose();

            await Task.Delay(TimeSpan.FromMilliseconds(300));
        }

        [SetUp]
        public void PerTestSetup()
        {
            this._iotCore = IoTCoreFactory.Create("id0", null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            this._mqttServerNetAdapter = new MqttServerNetAdapter(this._iotCore, this._iotCore.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, "testtopic");
            this._mqttServerNetAdapter.RequestReceived += MqttServerNetAdapterOnRequestReceived;
            this._mqttServerNetAdapter.EventReceived += MqttServerNetAdapterOnEventReceived;
            this._iotCore.RegisterServerNetAdapter(this._mqttServerNetAdapter);
        }

        [TearDown]
        public void PerTestTearDown()
        {
            this._iotCore.RemoveServerNetAdapter(this._mqttServerNetAdapter);
            this._mqttServerNetAdapter.RequestReceived -= MqttServerNetAdapterOnRequestReceived;
            this._mqttServerNetAdapter.EventReceived -= MqttServerNetAdapterOnEventReceived;
            //this._mqttServerNetAdapter.Stop();
            //this._mqttServerNetAdapter.Dispose();

            this._iotCore.Dispose();
            this._iotCore = null;
            this._mqttServerNetAdapter = null;
        }
        
        [Test, NonParallelizable]
        public void TestClientConnecting()
        {
            var clientHasConnected = false;

            this._mqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(_ =>
            {
                clientHasConnected = true;
            });

            this._mqttServerNetAdapter.Start();

            if (!clientHasConnected)
            {
                Assert.Fail("No connection occured!");
            }

            this._mqttBroker.ClientConnectedHandler = null;
        }

        [Test, NonParallelizable]
        public void TestClientSubscribing()
        {
            var targetTopic = "testtopic";

            var clientHasSubscribedToTopic = false;

            this._mqttBroker.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(args =>
            {
                if (args.TopicFilter.Topic == targetTopic)
                {
                    clientHasSubscribedToTopic = true;
                }
            });

            this._mqttServerNetAdapter.Start();

            if (!clientHasSubscribedToTopic)
            {
                Assert.Fail("No subscription occured!");
            }

            this._mqttBroker.ClientSubscribedTopicHandler = null;
        }

        [Test, Property("TestCaseKey", "IOTCS-T106"), NonParallelizable]
        public void Test_Setup_ProfilesElements_CreatedWhileStarting_MqttServerNetAdapter()
        {
            // Given: iot core created, gettree test passed and a randomly unique name for new mqtt topic 

            // When: mqtt net adapter created, started and registered 
            this._mqttServerNetAdapter.Start();

            var mqttConnectionItem = this._iotCore.Root.GetElementByProfile("commInterface");
            Assert.NotNull(mqttConnectionItem, "Not found: mqttconnection element with 'commInterface' profile");

            // Then: check if required profiles are created 
            /* 
            *  /connections (connections profile)
            *  /connections/mqttconnection (commInterface profile)
            *  /connections/mqttconnection/mqttsetup ((commifsetup) profile)
            *  /connections/mqttconnection/mqttsetup/QoS (parameter profile)
            *  /connections/mqttconnection/mqttcmdchannel (commchannel profile)
            *  /connections/mqttconnection/mqttcmdchannel/status (runcontrol profile)
            *  /connections/mqttconnection/mqttcmdchannel/mqttCmdChannelSetup (mqttCmdChannelSetup profile, data element)
            *  /connections/mqttconnection/mqttcmdchannel/mqttCmdChannelSetup/brokerIP (parameter profile, data element)
            *  /connections/mqttconnection/mqttcmdchannel/mqttCmdChannelSetup/brokerPort (parameter profile, data element)
            *  /connections/mqttconnection/mqttcmdchannel/mqttCmdChannelSetup/cmdTopic (parameter profile, data element)
            *  /connections/mqttconnection/mqttcmdchannel/mqttCmdChannelSetup/defaultReplyTopic (parameter profile, data element)
            */

            var gettreeResponse = this._iotCore.HandleRequest(0, "/gettree").Data;

            var g = Variant.ToObject<GetTreeResponseServiceData>(gettreeResponse);

            var mqttConnectionBaseAddress = $"/connections/{mqttConnectionItem.Identifier}";

            var checks = new Func<GetTreeResponseServiceData, bool>[]
            {
                gr => gr.Subs.SingleOrDefault(x => x.Identifier == "connections" && x.Profiles.Contains("connections")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier && x.Type == Identifiers.Structure && x.Profiles.Contains("commInterface")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status" && x.Type == Identifiers.Data && x.Profiles.Contains("runcontrol")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "start" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "stop" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "reset" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "suspend" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "getdata" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "datachanged" && x.Type == Identifiers.Event) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "datachanged")?.Subs?.SingleOrDefault(x=>x.Identifier == Identifiers.Subscribe && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "datachanged")?.Subs?.SingleOrDefault(x=>x.Identifier == Identifiers.Unsubscribe && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "type" && x.Type == Identifiers.Data) != null,

                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttsetup" && x.Type == Identifiers.Structure && x.Profiles.Contains("mqttsetup")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttsetup")?.Subs.SingleOrDefault(x=>x.Identifier == "QoS" && x.Type == Identifiers.Data && x.Profiles.Contains("parameter")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttsetup")?.Subs.SingleOrDefault(x=>x.Identifier == "version" && x.Type == Identifiers.Data) != null,

                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel" && x.Type == Identifiers.Structure && x.Profiles.Contains("commchannel")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "status" && x.Type == Identifiers.Data &&x.Profiles.Contains("runcontrol")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "start" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "start" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "start" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "preset" && x.Type == Identifiers.Data) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "status")?.Subs?.SingleOrDefault(x=>x.Identifier == "getdata" && x.Type == Identifiers.Service) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "type" && x.Type == Identifiers.Data) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttCmdChannelSetup" && x.Type == Identifiers.Structure && x.Profiles.Contains("mqttcmdchannelsetup")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttCmdChannelSetup" && x.Type == Identifiers.Structure && x.Profiles.Contains("mqttcmdchannelsetup"))?.Subs?.SingleOrDefault(x=>x.Identifier == "brokerIP" && x.Type == Identifiers.Data && x.Profiles.Contains("parameter")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttCmdChannelSetup" && x.Type == Identifiers.Structure && x.Profiles.Contains("mqttcmdchannelsetup"))?.Subs?.SingleOrDefault(x=>x.Identifier == "brokerPort" && x.Type == Identifiers.Data && x.Profiles.Contains("parameter")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttCmdChannelSetup" && x.Type == Identifiers.Structure && x.Profiles.Contains("mqttcmdchannelsetup"))?.Subs?.SingleOrDefault(x=>x.Identifier == "cmdTopic" && x.Type == Identifiers.Data && x.Profiles.Contains("parameter")) != null,
                gr => gr.Subs?.SingleOrDefault(x=>x.Identifier == "connections")?.Subs?.SingleOrDefault(x=>x.Identifier == mqttConnectionItem.Identifier)?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttcmdchannel")?.Subs?.SingleOrDefault(x=>x.Identifier == "mqttCmdChannelSetup" && x.Type == Identifiers.Structure && x.Profiles.Contains("mqttcmdchannelsetup"))?.Subs?.SingleOrDefault(x=>x.Identifier == "defaultReplyTopic" && x.Type == Identifiers.Data && x.Profiles.Contains("parameter")) != null,
            };

            string[] jpathqueriesAdrTypeProfile = {
                //"$..[?(@.adr == '/connections' && @.type == 'structure' && @.profiles[0] == 'connections')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress}' && @.type == 'structure' && @.profiles[0] == 'commInterface')]",

                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status"}' && @.type == 'data' && @.profiles[0] == 'runcontrol')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/start"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/stop"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/reset"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/suspend"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/getdata"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/datachanged"}' && @.type == 'event')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/datachanged/subscribe"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/status/datachanged/unsubscribe"}' && @.type == 'service')]",

                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/type"}' && @.type == 'data')]",

                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttsetup"}' && @.type == 'structure' && @.profiles[0] == 'mqttsetup')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttsetup/QoS"}' && @.type == 'data' && @.profiles[0] == 'parameter')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttsetup/version"}' && @.type == 'data')]",

                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel"}' && @.type == 'structure' && @.profiles[0] == 'commchannel')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/status"}' && @.type == 'data' && @.profiles[0] == 'runcontrol')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/status/start"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/status/stop"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/status/reset"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/status/preset"}' && @.type == 'data')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/status/getdata"}' && @.type == 'service')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/type"}' && @.type == 'data')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/mqttCmdChannelSetup"}' && @.type == 'structure' && @.profiles[0] == 'mqttcmdchannelsetup')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/mqttCmdChannelSetup/brokerIP"}' && @.type == 'data' && @.profiles[0] == 'parameter')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/mqttCmdChannelSetup/brokerPort"}' && @.type == 'data' && @.profiles[0] == 'parameter')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/mqttCmdChannelSetup/cmdTopic"}' && @.type == 'data' && @.profiles[0] == 'parameter')]",
                //$"$..[?(@.adr == '{mqttConnectionBaseAddress + "/mqttcmdchannel/mqttCmdChannelSetup/defaultReplyTopic"}' && @.type == 'data' && @.profiles[0] == 'parameter')]",
            };


            Assert.Multiple(() =>
            {

                foreach (var item in checks)
                {
                    Assert.True(item(g));
                }
            });

            Assert.Multiple(() =>
            {
                foreach (var queryAtp in jpathqueriesAdrTypeProfile)
                {
                    //Assert.NotNull(gettreeResponse.SelectToken(queryAtp), $"Failed to find element with query: {queryAtp}");
                }
            });

            // Then: check if profile elements have required structure

        }

        [Test, Property("TestCaseKey", "IOTCS-T109"), NonParallelizable]
        public void Test_Accepts_IoTMessage_From_MqttBroker_subscribed_cmdTopic()
        { // integration test not a system test
            // Given: 1 iot core, mqtt netadapter initialised and connected with mqtt broker (over local address port 1883)
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, "cmdTopic");

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            mqttServerNetAdapter1.Start();
            Message msgIn = null;
            var randomCid = new Random().Next();
            var msgSentOnMqtt = new ManualResetEventSlim();

            // When: iot request is sent to iot1 via mqtt cmdTopic
            try
            {
                MqttClient client = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
                var result = client.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
                Assert.That(result.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));
                iotCore1.RequestMessageReceived += (sender, eventargs) =>
                {
                    msgIn = eventargs.RequestMessage;
                    // ReSharper disable once AccessToDisposedClosure
                    msgSentOnMqtt.Set();
                };
                // send iot request to mqtt broker
                client.PublishAsync(new MqttApplicationMessage()
                {
                    Topic = "cmdTopic",
                    Payload = Encoding.UTF8.GetBytes($"{{'adr':'/getidentity', 'code':10, 'cid':{randomCid}}}")
                }).Wait();
                msgSentOnMqtt.Wait(TimeSpan.FromSeconds(8));
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                mqttServerNetAdapter1.Stop();
                mqttServerNetAdapter1.Dispose();
                msgSentOnMqtt.Dispose();
            }

            // Then: iot core receives the request 
            Assert.IsNotNull(msgIn);
            Assert.AreEqual(randomCid, msgIn.Cid);
            Assert.AreEqual("/getidentity", msgIn.Address);
        }

        [Test, Property("TestCaseKey", "IOTCS-T109"), NonParallelizable]
        public void Test_Publishes_IoTMessage_To_MqttBroker_defaultReplyTopic()
        { // system test
            // Given: 1 iot core, mqtt netadapter initialised and connected with mqtt broker (over local address port localBrokerPort )
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, "cmdTopic");

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            mqttServerNetAdapter1.Start();
            string iotResponseString = null;
            var randomCid = new Random().Next();
            var msgSentOnMqtt = new ManualResetEventSlim();
            var cmdTopicClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult = cmdTopicClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
            Assert.That(connectionResult.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));
            var defaultReplyTopicClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult2 = defaultReplyTopicClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
            Assert.That(connectionResult2.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

            // When: iot request is sent to mqtt cmdTopic
            try
            {
                var subscribeResult2 = defaultReplyTopicClient.SubscribeAsync("replyTopic").GetAwaiter().GetResult();
                Assert.AreEqual(MqttClientSubscribeResultCode.GrantedQoS0, subscribeResult2.Items[0].ResultCode);
                defaultReplyTopicClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(mqtteventargs =>
                {
                    iotResponseString = Encoding.UTF8.GetString(mqtteventargs.ApplicationMessage.Payload);
                    if (iotResponseString.Contains(randomCid.ToString()) && iotResponseString.Contains("profileData"))
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        msgSentOnMqtt.Set();
                    }
                });

                // send iot request to mqtt broker
                cmdTopicClient.PublishAsync(new MqttApplicationMessage()
                {
                    Topic = "cmdTopic",
                    Payload = Encoding.UTF8.GetBytes($"{{'adr':'/getidentity', 'code':10, 'cid':{randomCid}}}")
                }).Wait();

                msgSentOnMqtt.Wait(TimeSpan.FromSeconds(8));
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                defaultReplyTopicClient.DisconnectAsync().Wait();
                cmdTopicClient.DisconnectAsync().Wait();
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                mqttServerNetAdapter1.Stop();
                mqttServerNetAdapter1.Dispose();
                msgSentOnMqtt.Dispose();
            }

            // Then: iot core publishes iot response to mqtt broker, defaultReplyTopic, received by subscriber
            Assert.IsNotNull(iotResponseString);
            var iotResponseMessage = msgConverter.Deserialize(iotResponseString);
            Assert.That(iotResponseString, Contains.Substring(uniqueIotId));
            Assert.AreEqual(randomCid, iotResponseMessage.Cid);
            Assert.AreEqual("/getidentity", iotResponseMessage.Address);
        }

        [Test, Property("TestCaseKey", "IOTCS-T110"), NonParallelizable]
        public void Test_Publishes_IoTEventMessage_ToMqttUrls_Using_MqttNetAdapterClientFactory()
        { // system test
            // Given: 1 iot core (1 event element), mqtt netadapter initialised, local mqtt broker available for communication 
            var uniqueIotId = Guid.NewGuid().ToString();
            var randomCid = new Random().Next();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            iotCore1.RegisterClientNetAdapterFactory(new MqttNetAdapterClientFactory(msgConverter));
            var myevent = new EventElement(identifier: "myevent");
            iotCore1.Root.AddChild(myevent);
            string eventTopic = $"testEvent{randomCid}";

            var subscribeMessage = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 'random_cid_here', 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'mqtturl/unique_topic_here',
                                'datatosend': ['/getidentity'], 
                                'duration': 'uptime', 'uid': 'unique_id_here'}
                            }");

            var subscribeMessageAsVariant = new VariantObject()
            {
                { "code", (VariantValue) 10 },
                { "cid" , (VariantValue)randomCid },
                { "adr" , (VariantValue)"/myevent/subscribe" },
                { "data", new VariantObject
                    {
                        { "callback" , (VariantValue)$"mqtt://127.0.0.1:{LocalBrokerPort}/{eventTopic}" },
                        { "datatosend", new VariantArray(new [] { new VariantValue("/getidentity") })},
                        { "duration", (VariantValue) "uptime" },
                        { "uid" , (VariantValue)uniqueIotId }

                    }
                }
            };

            var message = new Message(RequestCodes.Request, randomCid, "/myevent/subscribe", new VariantObject
            {
                {"callback", (VariantValue) $"mqtt://127.0.0.1:{LocalBrokerPort}/{eventTopic}"},
                {"datatosend", new VariantArray(new[] {new VariantValue("/getidentity")})},
                {"duration", (VariantValue) "uptime"},
                {"uid", (VariantValue) uniqueIotId}

            });

            // ReSharper disable once PossibleNullReferenceException
            // subscribe to event with mqtt
            var subscribeReq = iotCore1.HandleRequest(message);

            string iotResponseString = null;
            var msgSentOnMqtt = new ManualResetEventSlim();
            var brokerAppMessagesClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult2 = brokerAppMessagesClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
            Assert.That(connectionResult2.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

            // When: iot request is sent to mqtt cmdTopic
            try
            {
                brokerAppMessagesClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(mqtteventargs =>
               {
                   iotResponseString = Encoding.UTF8.GetString(mqtteventargs.ApplicationMessage.Payload);
                   if (mqtteventargs.ApplicationMessage.Topic == eventTopic && iotResponseString.Contains(randomCid.ToString()))
                   {
                       msgSentOnMqtt.Set();
                   }
               });
                var subscribeResult2 = brokerAppMessagesClient.SubscribeAsync(eventTopic).GetAwaiter().GetResult();
                Assert.AreEqual(MqttClientSubscribeResultCode.GrantedQoS0, subscribeResult2.Items[0].ResultCode);
                myevent.RaiseEvent(); // trigger event internally
                msgSentOnMqtt.Wait(TimeSpan.FromSeconds(8));
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                brokerAppMessagesClient.DisconnectAsync().Wait();
                msgSentOnMqtt.Dispose();
            }

            // Then: iot core publishes iot response to mqtt broker, defaultReplyTopic, received by subscriber
            Assert.IsNotNull(iotResponseString);
            var iotResponseMessage = msgConverter.Deserialize(iotResponseString);
            Assert.That(iotResponseString, Contains.Substring("/myevent"));
            Assert.AreEqual(randomCid, iotResponseMessage.Cid);
            Assert.AreEqual("/" + eventTopic, iotResponseMessage.Address);
        }

        [Test, Property("TestCaseKey", "IOTCS-T113"), NonParallelizable]
        public void Test_Setup_mqttconnection_runcontrol_profile_CanStop_Single_MqttServerNetAdapter_Running()
        {
            /* Given: 
             * iot core created and registered single MqttServerNetAdapter instance, 
             * mqtt broker available at localaddress:localBrokerPort , all 4 events subscribed 
             * MqttServerNetAdapter started
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, "cmdTopic");

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent; 

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            mqttServerNetAdapter1.Start();
            var serverDisconnection = new ManualResetEventSlim();
            var serverUnsubscription = new ManualResetEventSlim();
            string[] entryEventsDone = new string[4] { "", "", "", "" };
            try
            {
                this._mqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[0] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Connected  id: {entryEventsDone[0]}");
               });
                this._mqttBroker.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[1] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Subscribed  id: {entryEventsDone[1]}");
               });
                this._mqttBroker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[2] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Unsubscribed  id: {entryEventsDone[2]}");
                   serverUnsubscription.Set();
               });
                this._mqttBroker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[3] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Disconnected  id: {entryEventsDone[3]}");
                   serverDisconnection.Set();
               });

                /* When: 
                 * invoking runcontrol Stop service (mqttconnection/status/stop) 
                 */

                var mqttConnectionItem = iotCore1.Root.GetElementByProfile("commInterface");
                var result = iotCore1.HandleRequest(0, $"/connections/{mqttConnectionItem.Identifier}/status/stop", null);

                Assert.AreEqual(ResponseCodes.Success, result.Code, "Calling stop service didnt succeed.");
                //mqttServerNetAdapter1.Stop(); // enable this to check if test works
                serverUnsubscription.Wait(TimeSpan.FromMilliseconds(500));
                serverDisconnection.Wait(TimeSpan.FromMilliseconds(500));

                /* Then: 
                 * MqttNetAdapter unsubscribes from mqtt broker topics and disconnects client:  
                 */
                Assert.IsTrue(serverUnsubscription.IsSet && serverDisconnection.IsSet,
                    "Event(s) not raised by mqtt client: Unsubscription and/or Disconnection");
                // all required events are done by same netadapterserver client TODO: compare with actual clientId of mqttnetserveradapter
                Assert.IsTrue(
                    entryEventsDone[2] == entryEventsDone[3] &&
                    entryEventsDone[0] == "" &&
                    entryEventsDone[1] == "",
                    $"Error: unsubscribe/disconnect not raised OR connect/subscribe raised, ClientId: {entryEventsDone[2]}");
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                this._mqttBroker.ClientConnectedHandler = null;
                this._mqttBroker.ClientSubscribedTopicHandler = null;
                this._mqttBroker.ClientUnsubscribedTopicHandler = null;
                this._mqttBroker.ClientDisconnectedHandler = null;
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                mqttServerNetAdapter1.Stop();
                mqttServerNetAdapter1.Dispose();

                serverUnsubscription.Dispose();
                serverDisconnection.Dispose();
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T113"), NonParallelizable]
        public void Test_Setup_mqttconnection_runcontrol_profile_CanStart_Single_MqttServerNetAdapter_Stopped()
        {
            /* Given: 
             * iot core created and registered single MqttServerNetAdapter instance, 
             * mqtt broker available at localaddress:localBrokerPort , required events subscribed 
             * MqttServerNetAdapter in stopped state
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, "cmdTopic");

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            mqttServerNetAdapter1.Stop();
            var serverConnection = new ManualResetEventSlim();
            var serverSubscription = new ManualResetEventSlim();
            string[] exitEventsDone = new string[4] { "", "", "", "" };
            try
            {
                this._mqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(mqtteventargs =>
               {
                   exitEventsDone[0] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Connected  id: {exitEventsDone[0]}");
                   serverConnection.Set();
               });
                this._mqttBroker.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   exitEventsDone[1] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Subscribed  id: {exitEventsDone[1]}");
                   serverSubscription.Set();
               });
                this._mqttBroker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   exitEventsDone[2] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Unsubscribed  id: {exitEventsDone[2]}");
               });
                this._mqttBroker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(mqtteventargs =>
               {
                   exitEventsDone[3] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Disconnected  id: {exitEventsDone[3]}");
               });

                /* When: 
                 * invoking runcontrol Start service (mqttconnection/status/start) 
                 */
                var commInterfaceItem = iotCore1.Root.GetElementByProfile("commInterface");

                iotCore1.HandleRequest(0, $"/connections/{commInterfaceItem.Identifier}/status/start", null);
                //mqttServerNetAdapter1.Start(); // enable this to check if test works
                serverSubscription.Wait(TimeSpan.FromMilliseconds(500));
                serverConnection.Wait(TimeSpan.FromMilliseconds(500));

                /* Then: 
                 * MqttNetAdapter unsubscribes from mqtt broker topics & disconnects clients  
                 */
                Assert.IsTrue(serverSubscription.IsSet && serverConnection.IsSet,
                    "Event(s) not raised by mqtt client: Unsubscription and/or Disconnection");
                // all 2 events are done by same netadapterserver client TODO: compare with actual clientId of mqttnetserveradapter
                Assert.IsTrue(
                    exitEventsDone[0] == exitEventsDone[1] &&
                    exitEventsDone[2] == "" &&
                    exitEventsDone[3] == "",
                    $"Error: connect/subscribe not raised OR unsubscribe/disconnect raised, ClientId: {exitEventsDone[0]}");
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                this._mqttBroker.ClientConnectedHandler = null;
                this._mqttBroker.ClientSubscribedTopicHandler = null;
                this._mqttBroker.ClientUnsubscribedTopicHandler = null;
                this._mqttBroker.ClientDisconnectedHandler = null;
                
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                mqttServerNetAdapter1.Stop();
                mqttServerNetAdapter1.Dispose();
                
                serverSubscription.Dispose();
                serverConnection.Dispose();
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T113"), NonParallelizable]
        public void Test_Setup_mqttconnection_runcontrol_profile_CanStop_All_MqttServerNetAdapter_Running()
        {
            Assert.Ignore("Not tested for iot core .net v1.2");
        }

        [Test, Property("TestCaseKey", "IOTCS-T113"), NonParallelizable]
        public void Test_Setup_mqttconnection_runcontrol_profile_CanStart_All_MqttServerNetAdapter_Stopped()
        {
            Assert.Ignore("Not tested for iot core .net v1.2");
        }

        [Test, Property("TestCaseKey", "IOTCS-T108"), NonParallelizable]
        public void Test_Connects_To_External_MqttBroker()
        {
            /* Given: 
             * iot core created and registered single MqttServerNetAdapter instance, 
             * MqttServerNetAdapter set to external broker (see below) and started
             * External mqtt brokers:
             *  our build pc - dett10v0037.intra.ifm:1883
             *  local domain (IT provided) - mqtt-one.intra.ifm:1883, mqtt-two.intra.ifm:1883
             *  hivemq  - tcp://broker.hivemq.com:1883 ("35.158.43.238") 
             *  mqtt.cool  IP - tcp://broker.mqtt.cool:1883 ("52.13.161.235")
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var uniquecmdTopic = uniqueIotId;
            var randomCid = new Random().Next();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            //var publicBroker = new IPEndPoint(System.Net.Dns.GetHostAddresses("broker.hivemq.com")[0], 1883); // proxy issues ?
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, uniquecmdTopic);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            mqttServerNetAdapter1.Start();
            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);

            Message iotResponse = null;
            var msgSentOnMqtt = new ManualResetEventSlim();
            MqttClient brokerAppMessagesClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult2 = brokerAppMessagesClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
            Assert.That(connectionResult2.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

            // When: iot request is sent to the unique cmdTopic on external mqtt broker
            try
            {
                iotCore1.RequestMessageReceived += (sender, eventargs) =>
                {
                    if (eventargs.RequestMessage.Cid == randomCid)
                    {
                        iotResponse = eventargs.RequestMessage;
                        msgSentOnMqtt.Set();
                    }
                };
                var subscribeResult2 = brokerAppMessagesClient.SubscribeAsync(uniquecmdTopic).GetAwaiter().GetResult();
                Assert.AreEqual(MqttClientSubscribeResultCode.GrantedQoS0, subscribeResult2.Items[0].ResultCode);
                var subscribeResult3 = brokerAppMessagesClient.PublishAsync(new MqttApplicationMessage
                {
                    ContentType = "mqtt",
                    Payload = Encoding.UTF8.GetBytes(string.Format("{{'code':10, 'cid':{0}, 'adr':'/{0}'}}", randomCid)),
                    Topic = uniquecmdTopic,
                }).GetAwaiter().GetResult();
                msgSentOnMqtt.Wait(TimeSpan.FromSeconds(2));
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                brokerAppMessagesClient.DisconnectAsync().Wait();
                msgSentOnMqtt.Dispose();
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
            }
            // Then: application message is recieved by iot core via mqtt netadapter
            Assert.AreEqual($"/{randomCid}", iotResponse.Address);
        }

        [Test, NonParallelizable]
        public void TestCmdTopicOnRestart()
        {
            var iotCore = IoTCoreFactory.Create("id0", null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            using var mqttNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, "myTopic");

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore.HandleRequest(args.RequestMessage);
            }

            mqttNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore.HandleEvent(args.EventMessage);
            }

            mqttNetAdapter.EventReceived += HandleEvent;

            iotCore.RegisterServerNetAdapter(mqttNetAdapter);

            mqttNetAdapter.Start();

            Assert.AreEqual(mqttNetAdapter.CommandTopic, "myTopic");
            mqttNetAdapter.Stop();
            mqttNetAdapter.Start();

            Assert.AreEqual(mqttNetAdapter.CommandTopic, "myTopic");

            mqttNetAdapter.RequestReceived -= HandleRequest;
            mqttNetAdapter.EventReceived -= HandleEvent;
        }

        [Test, Property("TestCaseKey", "IOTCS-T108"), NonParallelizable]
        public async Task Test_Setup_BrokerIp_UpdatedUsing_setdata_of_commchannel_brokerip()
        {
            /* Given: 
             * test passed: Test_Connects_To_Public_MqttBroker()
             * iot core created and registered single MqttServerNetAdapter instance, 
             * iot tree contains commInterface element (which contains commchannel profile element)
             * MqttServerNetAdapter set to a external broker and started
             * brokerPort.setdata test passes so it can be used for test setup issue of using some other port
             */

            var mqttBroker2 = new MqttServer(new[] {new MqttTcpServerAdapter(new MqttNetNullLogger()) }, new MqttNetNullLogger());
            await mqttBroker2.StartAsync(
                new MqttServerOptionsBuilder()
                    .WithDefaultEndpoint()
                    .WithDefaultEndpointPort(LocalBrokerPort + 1)
                    .WithDefaultEndpointReuseAddress().Build()
            );

            var uniqueIotId = Guid.NewGuid().ToString();
            var uniquecmdTopic = uniqueIotId;
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort), null, uniquecmdTopic);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;
            
            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            await mqttServerNetAdapter1.StartAsync();
            

            var serverConnection = new ManualResetEventSlim();
            var serverSubscription = new ManualResetEventSlim();
            string[] entryEventsDone = new string[4] { "", "", "", "" };
            try
            {
                var commInterfaceItem = iotCore1.Root.GetElementByProfile("commInterface");
                // changing port for test setup requirements. this assumes brokerPort.setdata test passes
                
                mqttBroker2.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[0] = (mqtteventargs.ClientId.ToString());
                   serverConnection.Set();
                   this.Logger.Debug($"mqttServer Client Connected  id: {entryEventsDone[0]}");
               });
                mqttBroker2.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[1] = (mqtteventargs.ClientId.ToString());
                   serverSubscription.Set();
                   this.Logger.Debug($"mqttServer Client Subscribed  id: {entryEventsDone[1]}");
               });
                this._mqttBroker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[2] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Unsubscribed  id: {entryEventsDone[2]}");
               });
                this._mqttBroker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[3] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Disconnected  id: {entryEventsDone[3]}");
               });

                iotCore1.HandleRequest(0, 
                    $"/connections/{commInterfaceItem.Identifier}/mqttcmdchannel/mqttCmdChannelSetup/brokerPort/setdata",
                    data: new VariantObject()
                    {
                        {"newvalue", new VariantValue(LocalBrokerPort + 1)}
                    });


                /* When: 
                 * IP address is changed to local address using commchannel >> brokerip >> setdata service, 
                 */
                var setIpRequestResult = iotCore1.HandleRequest(0, $"/connections/{commInterfaceItem.Identifier}/mqttcmdchannel/mqttCmdChannelSetup/brokerIP/setdata",
                     data: new VariantObject()
                     {
                         { "newvalue", new VariantValue(IPAddress.Loopback.ToString())}
                     });


                //Assert.AreEqual(ResponseCodes.Ok, setIpRequestResult.Code);

                serverSubscription.Wait(TimeSpan.FromMilliseconds(500));
                serverConnection.Wait(TimeSpan.FromMilliseconds(500));

                /* Then: 
                 * netadapter connects to local broker and subscribes to required topics
                 */
                var currentClientId = mqttServerNetAdapter1.CurrentClientId; 
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(serverConnection.IsSet, "Expected connected event @ mqttbroker2");
                    Assert.IsTrue(serverSubscription.IsSet, "Expected subscription event @ mqttbroker2");
                    if(serverConnection.IsSet) 
                        Assert.AreEqual(currentClientId, entryEventsDone[0], $"connect event by un-expected client id {entryEventsDone[0]}");
                    if(serverSubscription.IsSet) 
                        Assert.AreEqual(currentClientId, entryEventsDone[1], $"subscribe event by un-expected client id {entryEventsDone[1]}");
                    Assert.IsNotEmpty(entryEventsDone[2], $"missing unsubscribe event, client id {entryEventsDone[2]}");
                    Assert.IsNotEmpty(entryEventsDone[3], $"missing disconnect event, client id {entryEventsDone[3]}");
                });
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                mqttBroker2.ClientConnectedHandler = null;
                mqttBroker2.ClientSubscribedTopicHandler = null;
                this._mqttBroker.ClientUnsubscribedTopicHandler = null;
                this._mqttBroker.ClientDisconnectedHandler = null;
                
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                await mqttServerNetAdapter1.StopAsync();
                mqttServerNetAdapter1.Dispose();
                
                serverSubscription.Dispose();
                serverConnection.Dispose();

                mqttBroker2.Dispose();
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T108"), NonParallelizable]
        public async Task Test_Setup_BrokerPort_UpdatedUsing_setdata_of_commchannel_brokerport()
        {
            /* Given: 
             * iot core created and registered single MqttServerNetAdapter instance, 
             * iot tree contains commInterface element (which contains commchannel profile element)
             * MqttServerNetAdapter pointing to local mqtt broker @ localBrokerPort  
             * second local mqtt broker available at port 1885 - mqttBroker2
             */
            var mqttBroker2 = new MqttServer(new List<IMqttServerAdapter> { new MqttTcpServerAdapter(new MqttNetNullLogger()) }, new MqttNetNullLogger());
            int broker2Port = 1885;
            await mqttBroker2.StartAsync(new MqttServerOptions { DefaultEndpointOptions = { BoundInterNetworkAddress = IPAddress.Loopback, Port = broker2Port, ReuseAddress = true} });

            var uniqueIotId = Guid.NewGuid().ToString();
            var uniquecmdTopic = uniqueIotId;
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort ), null, uniquecmdTopic);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            await mqttServerNetAdapter1.StartAsync();
            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);

            var firstClient = mqttServerNetAdapter1.CurrentClientId;
            var serverConnection = new ManualResetEventSlim();
            var serverSubscription = new ManualResetEventSlim();
            string[] EntryEventsDone = new string[4] { "", "", "", "" };
            try
            {
                mqttBroker2.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(mqtteventargs =>
               {
                   EntryEventsDone[0] = (mqtteventargs.ClientId.ToString());
                   serverConnection.Set();
                   this.Logger.Debug($"mqttServer Client Connected  id: {EntryEventsDone[0]}");
               });
                mqttBroker2.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   EntryEventsDone[1] = (mqtteventargs.ClientId.ToString());
                   serverSubscription.Set();
                   this.Logger.Debug($"mqttServer Client Subscribed  id: {EntryEventsDone[1]}, topic: {mqtteventargs.TopicFilter.Topic}");
               });
                this._mqttBroker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   EntryEventsDone[2] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Unsubscribed  id: {EntryEventsDone[2]}, topic: {mqtteventargs.TopicFilter}");
               });
                this._mqttBroker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(mqtteventargs =>
               {
                   EntryEventsDone[3] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Disconnected  id: {EntryEventsDone[3]}");
               });

                /* When: 
                 * Port is changed to point mqttbroker2 using commchannel >> brokerPort >> setdata service, 
                 */
                var commInterfaceItem = iotCore1.Root.GetElementByProfile("commInterface");
                iotCore1.HandleRequest(0, $"/connections/{commInterfaceItem.Identifier}/mqttcmdchannel/mqttCmdChannelSetup/brokerPort/setdata",
                    new VariantObject()
                    {
                        {"newvalue", new VariantValue(broker2Port)}
                    });

                
                serverSubscription.Wait(TimeSpan.FromMilliseconds(500));
                serverConnection.Wait(TimeSpan.FromMilliseconds(500));

                /* Then: 
                 * netadapter connects to second broker (mqttbroker2) and subscribes to required topcics 
                 * netadapter unsubscriber and disconnectes from earlier mqttbroker @ localBrokerPort
                 */
                var currentClientId = mqttServerNetAdapter1.CurrentClientId; 
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(serverConnection.IsSet, "Expected connected event @ mqttbroker2");
                    Assert.IsTrue(serverSubscription.IsSet, "Expected subscription event @ mqttbroker2");
                    if(serverConnection.IsSet) 
                        Assert.AreEqual(currentClientId, EntryEventsDone[0], $"connect event by un-expected client id {EntryEventsDone[0]}");
                    if(serverSubscription.IsSet) 
                        Assert.AreEqual(currentClientId, EntryEventsDone[1], $"subscribe event by un-expected client id {EntryEventsDone[1]}");
                    Assert.AreEqual(EntryEventsDone[2], firstClient, message:"unsubscribe event not from earlier client");
                    Assert.AreEqual(EntryEventsDone[3], firstClient, message:"disconnect event not from earlier client");
                });
            }
            finally
            { // don't forget to close shared resources for other tests to reuse

                mqttBroker2.ClientConnectedHandler = null;
                mqttBroker2.ClientSubscribedTopicHandler = null;

                this._mqttBroker.ClientUnsubscribedTopicHandler = null;
                this._mqttBroker.ClientDisconnectedHandler = null;
               
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                await mqttServerNetAdapter1.StopAsync();
                mqttServerNetAdapter1.Dispose();
                
                serverSubscription.Dispose();
                serverConnection.Dispose();

                mqttBroker2.Dispose();
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T108"), NonParallelizable]
        public void Test_Setup_cmdTopic_UpdatedUsing_setdata_of_mqttCmdChannelSetup()
        {
            /* Given:
             * iot core created, mqttnetadpater registered and started
             * mqttNetAdapter subscribed to default topics. value of cmdTopic = "cmdTopic"
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var uniquecmdTopic = new Random().Next().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            
            var initialCommandTopic = "cmdTopic";
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort ), null, initialCommandTopic);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            mqttServerNetAdapter1.Start();
            var serverSubscription = new ManualResetEventSlim();
            var serverUnsubscription = new ManualResetEventSlim();
            string[] entryEventsDone = new string[4] { "", "", "", "" };
            string[] topicsSubUnsub = new string[2] { "", ""};

            try
            {
                this._mqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[0] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Connected  id: {entryEventsDone[0]}");
               });
                this._mqttBroker.ClientSubscribedTopicHandler = new MqttServerClientSubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[1] = (mqtteventargs.ClientId.ToString());
                   topicsSubUnsub[0] = mqtteventargs.TopicFilter.Topic;
                   serverSubscription.Set();
                   this.Logger.Debug($"mqttServer Client Subscribed  id: {entryEventsDone[1]}, topic: {mqtteventargs.TopicFilter.Topic}");
               });
                this._mqttBroker.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[2] = (mqtteventargs.ClientId.ToString());
                   topicsSubUnsub[1] = mqtteventargs.TopicFilter;
                   serverUnsubscription.Set();
                   this.Logger.Debug($"mqttServer Client Unsubscribed  id: {entryEventsDone[2]}, topic: {mqtteventargs.TopicFilter}");
               });
                this._mqttBroker.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(mqtteventargs =>
               {
                   entryEventsDone[3] = (mqtteventargs.ClientId.ToString());
                   this.Logger.Debug($"mqttServer Client Disconnected  id: {entryEventsDone[3]}");
               });

            /* When:
             * cmdTopic is updated to "new unique topic name" using its setdata service
             * /connections/<commInterface element>/mqttCmdChannel/mqttCmdChannelSetup/cmdTopic/setdata
             */
                var initialClientId = mqttServerNetAdapter1.CurrentClientId; 
                var commInterfaceItem = iotCore1.Root.GetElementByProfile("commInterface");
                iotCore1.HandleRequest(0, $"/connections/{commInterfaceItem.Identifier}/mqttcmdchannel/mqttCmdChannelSetup/cmdTopic/setdata",
                    new VariantObject()
                    {
                        {"newvalue" , new VariantValue(uniquecmdTopic)}
                    });

                serverSubscription.Wait(TimeSpan.FromMilliseconds(500));
                serverUnsubscription.Wait(TimeSpan.FromMilliseconds(500));

                /* Then:
                 * mqttnetadapter unsubscribes from "cmdTopic" and subscribes "new unique topic name" 
                 * mqttnetadapter sets "cmdTopic" value to "new unique topic name" 
                 */
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(serverUnsubscription.IsSet, "Expected un-subscription event ");
                    Assert.IsTrue(serverSubscription.IsSet, "Expected subscription event ");
                    if (serverSubscription.IsSet)
                    {
                        Assert.AreNotEqual(entryEventsDone[1], entryEventsDone[2], 
                            $"Unexpected subscribe and unsubscribe from same client {entryEventsDone[1]}");
                        Assert.AreEqual(uniquecmdTopic, topicsSubUnsub[0], $"Expected to subscribe to {uniquecmdTopic}");
                    }
                    if (serverUnsubscription.IsSet)
                    {
                        Assert.AreEqual(initialClientId, entryEventsDone[2], $"Expected unsubscribe by client {initialClientId}");
                        Assert.AreEqual(initialCommandTopic, topicsSubUnsub[1], $"Expected to unsubscribe from cmdTopic");
                    }

                    var newName = (string)(VariantValue) ((VariantObject)iotCore1.HandleRequest(0, $"/connections/{commInterfaceItem.Identifier}/mqttcmdchannel/mqttCmdChannelSetup/cmdTopic/getdata", null).Data)["value"];
                    Assert.AreEqual(uniquecmdTopic, newName, "Unexpected cmdTopic getdata value");
                });
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                this._mqttBroker.ClientConnectedHandler = null;
                this._mqttBroker.ClientSubscribedTopicHandler = null;
                this._mqttBroker.ClientUnsubscribedTopicHandler = null;
                this._mqttBroker.ClientDisconnectedHandler = null;
                
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                mqttServerNetAdapter1.Stop();
                mqttServerNetAdapter1.Dispose();
                
                serverSubscription.Dispose();
                serverUnsubscription.Dispose();
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T108"), NonParallelizable]
        public void Test_Setup_defaultReplyTopic_UpdatedUsing_setdata_of_mqttCmdChannelSetup()
        {
            /* Given:
             * iot core created, mqttnetadpater registered and started
             * mqttNetAdapter subscribed to default topics. value of cmdTopic = "cmdTopic"
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var uniqueReplyTopicName = new Random().Next().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort ), null, "cmdTopic");

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            mqttServerNetAdapter1.Start();
            string iotResponseString = null;
            var randomCid = new Random().Next();
            var msgSentOnMqtt = new ManualResetEventSlim();
            MqttClient cmdTopicClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult = cmdTopicClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort )).GetAwaiter().GetResult();
            Assert.That(connectionResult.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));
            MqttClient defaultReplyTopicClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult2 = defaultReplyTopicClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort )).GetAwaiter().GetResult();
            Assert.That(connectionResult2.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

            try
            {
                var subscribeResult2 = defaultReplyTopicClient.SubscribeAsync(uniqueReplyTopicName).GetAwaiter().GetResult(); // watch new reply topic name
                //var subscribeResult2 = defaultReplyTopicClient.SubscribeAsync("cmdTopic").GetAwaiter().GetResult();
                Assert.AreEqual(MqttClientSubscribeResultCode.GrantedQoS0, subscribeResult2.Items[0].ResultCode); 
                /* When:
                 * ReplyTopic is updated to "new unique topic name" using its setdata service 
                 * /connections/<commInterface element>/mqttCmdChannel/mqttCmdChannelSetup/defaultReplyTopic/setdata
                 * And
                 * an iot command is sent to cmdTopic
                 */
                var commInterfaceItem = iotCore1.Root.GetElementByProfile("commInterface");
                iotCore1.HandleRequest(0, $"/connections/{commInterfaceItem.Identifier}/mqttcmdchannel/mqttCmdChannelSetup/defaultReplyTopic/setdata",
                     data: new VariantObject()
                     {
                         {"newvalue", new VariantValue(uniqueReplyTopicName)}
                     });
                
                

                // send iot request to mqtt broker
                defaultReplyTopicClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(mqtteventargs =>
                {
                    iotResponseString = Encoding.UTF8.GetString(mqtteventargs.ApplicationMessage.Payload);
                    if (iotResponseString.Contains(randomCid.ToString()) && iotResponseString.Contains("profileData"))
                    {
                        msgSentOnMqtt.Set();
                    }
                });

                cmdTopicClient.PublishAsync(new MqttApplicationMessage()
                {
                    Topic = "cmdTopic",
                    Payload = Encoding.UTF8.GetBytes($"{{'adr':'/getidentity', 'code':10, 'cid':{randomCid}}}")
                }).Wait();
                
                msgSentOnMqtt.Wait(TimeSpan.FromSeconds(8));
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                defaultReplyTopicClient.DisconnectAsync().Wait();
                cmdTopicClient.DisconnectAsync().Wait();
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;

                mqttServerNetAdapter1.Stop();
                mqttServerNetAdapter1.Dispose();
                msgSentOnMqtt.Dispose();
            }

            /* Then: 
             * The iot response is received by the new defaultReplyTopic 
             */
            Assert.IsNotNull(iotResponseString);
            var iotResponseMessage = msgConverter.Deserialize(iotResponseString);
            Assert.That(iotResponseString, Contains.Substring(uniqueIotId));
            Assert.AreEqual(randomCid, iotResponseMessage.Cid);
            Assert.AreEqual("/getidentity", iotResponseMessage.Address);
        }

        [Test, Property("TestCaseKey", "IOTCS-T139"), NonParallelizable]
        public async Task Test_ConnectionFailureWithBroker_StartOnDisconnectionHandler_Reconnects()
        {
            var brokerPort = 10055;

            // see Given - When - Then comments for test scenario
            /* Given: 
             * assumed tests passing; Publishes_IoTMessage, publishes_IoTEventMessage
             * local MqttBroker created which will be stopped in between - TempMqttBroker
             * iot core created and registered single MqttServerNetAdapter instance pointing to local MqttBroker, 
             * mqtt clients created to publish iot command and receive iot response 
             */

            var mqttNetLogger = new MqttNetNullLogger();
            var tempMqttBroker = new MqttServer(new List<IMqttServerAdapter> { new MqttTcpServerAdapter(mqttNetLogger) }, mqttNetLogger);
            var tempBrokerPort = brokerPort + 1;
            await tempMqttBroker.StartAsync(new MqttServerOptions { DefaultEndpointOptions = { BoundInterNetworkAddress = IPAddress.Loopback, Port = tempBrokerPort, ReuseAddress = true}});

            tempMqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(_ =>
            {
                TestContext.Out.WriteLine($"Client with ID: '{_.ClientId}' connected to broker");
            });

            Assert.That(tempMqttBroker.IsStarted, "tempMqttBroker.IsStarted");
            
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));

            void RequestMessageReceived(object sender, RequestMessageEventArgs e)
            {
                TestContext.Out.WriteLine($"Message received: {e.RequestMessage}");
            }

            iotCore1.RequestMessageReceived += RequestMessageReceived;

            var brokerReconnected = new ManualResetEventSlim(false);
            MqttServerNetAdapter mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, tempBrokerPort), 
                disconnectionHandler:  ex => { 
                    brokerReconnected.Set();
                });

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);
            await mqttServerNetAdapter1.StartAsync();

            MqttClient cmdTopicClient = new MqttClient(new MqttClientAdapterFactory(mqttNetLogger), mqttNetLogger);
            MqttClient defaultReplyTopicClient = null;
            string iotResponseString = null;

            try
            {
                var randomCid = new Random().Next();

                /* When:
                 * broker is stopped and started back in 'timeout' seconds
                 */
                // ====DEBUG: comment this section to prove test passes without broker restart =====
                await tempMqttBroker.StopAsync();
                tempMqttBroker.Dispose();
                Thread.Sleep(500); // simulate broker not available temporarily
                
                tempMqttBroker =
                    new MqttServer(new List<IMqttServerAdapter> {new MqttTcpServerAdapter(mqttNetLogger)},
                        mqttNetLogger);
                await tempMqttBroker.StartAsync(new MqttServerOptions
                    {
                        DefaultEndpointOptions = {BoundInterNetworkAddress = IPAddress.Loopback, Port = tempBrokerPort, ReuseAddress = true}
                    });

                tempMqttBroker.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(_ =>
                {
                    TestContext.Out.WriteLine($"Client with ID: '{_.ClientId}' connected to broker");
                });

                Assert.That(tempMqttBroker.IsStarted, "tempMqttBroker.IsStarted");

                brokerReconnected.Wait(TimeSpan.FromSeconds(5));
                await mqttServerNetAdapter1.StartAsync(); 
                // ====DEBUG: comment this section to prove test passes without broker restart =====

                // subscribe to mqtt msg received event
                defaultReplyTopicClient = new MqttClient(new MqttClientAdapterFactory(mqttNetLogger), mqttNetLogger);

                var options = MqttHelper.BuildOptions(IPAddress.Loopback, tempBrokerPort, "defaultReplyTopicClient");
                var defaultReplyTopicClientConnectResult = await defaultReplyTopicClient.ConnectAsync(options);
                Assert.AreEqual(MqttClientConnectResultCode.Success, defaultReplyTopicClientConnectResult.ResultCode);

                Assert.That(defaultReplyTopicClient.IsConnected, "defaultReplyTopicClient.IsConnected");
                var subscribeResult = await defaultReplyTopicClient.SubscribeAsync("replyTopic");

                Assert.AreEqual(MqttClientSubscribeResultCode.GrantedQoS0, subscribeResult.Items.First().ResultCode);
                
                using (var msgReceivedBack = new ManualResetEventSlim())
                {
                    defaultReplyTopicClient.ApplicationMessageReceivedHandler =
                        new MqttApplicationMessageReceivedHandlerDelegate(mqtteventargs =>
                        {
                            var mqttMsgString = Encoding.UTF8.GetString(mqtteventargs.ApplicationMessage.Payload);
                            if (mqttMsgString.Contains(randomCid.ToString()) && mqttMsgString.Contains("data"))
                            {
                                iotResponseString = mqttMsgString;
                                msgReceivedBack.Set();
                            }
                        });

                    // send iot request to mqtt broker
                    var connectionResult = await cmdTopicClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, tempBrokerPort));
                    Assert.AreEqual(MqttClientConnectResultCode.Success,connectionResult.ResultCode);

                    Assert.That(cmdTopicClient.IsConnected, "cmdTopicClient.IsConnected");
                    var publishResult = await cmdTopicClient.PublishAsync(new MqttApplicationMessage
                    {
                        Topic = "cmdTopic",
                        Payload = Encoding.UTF8.GetBytes($"{{'adr':'/getidentity', 'code':10, 'cid':{randomCid}}}"),
                        QualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce
                    });

                    Assert.AreEqual(MqttClientPublishReasonCode.Success,publishResult.ReasonCode);

                    await cmdTopicClient.DisconnectAsync();
                    Assert.That(!cmdTopicClient.IsConnected, "!cmdTopicClient.IsConnected");

                    msgReceivedBack.Wait(millisecondsTimeout: 5000);
                }

                /* Then: 
                    * broker reconnected when its available - netadapter conveys iot core response message to mqtt client
                */
                Assert.IsNotNull(iotResponseString, "Expected broker reconnected and thus response available");
            }
            catch (Exception e)
            {
                await TestContext.Out.WriteLineAsync($"Error occured: {e.Message}");
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                await defaultReplyTopicClient.DisconnectAsync();
                defaultReplyTopicClient.ApplicationMessageReceivedHandler = null;
                defaultReplyTopicClient?.Dispose();
                await cmdTopicClient.DisconnectAsync();
                cmdTopicClient.Dispose();
                
                mqttServerNetAdapter1.RequestReceived -= HandleRequest;
                mqttServerNetAdapter1.EventReceived -= HandleEvent;
                
                await tempMqttBroker.StopAsync();
                tempMqttBroker.Dispose();
                tempMqttBroker.ClientConnectedHandler = null;
                brokerReconnected.Dispose();

                iotCore1.RequestMessageReceived -= RequestMessageReceived;
                iotCore1.Dispose();
            }   
        }

        [Test, Property("TestCaseKey", "IOTCS-T113"), NonParallelizable]
        public void Test_Setup_mqttCmdChannel_RunControlProfile_StartStopService_indicatedWith_Status_init_running_stopped()
        {
            /* Given: 
             * iot core initialised, mqtt server net adapter initialised.
             * all required elements for run profile are present. 
             * runcontrol profile element is mqttconnection/mqttcmdchannel/status
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort),null);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;


            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);

            var mqttConnectionAdr = (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commInterface'}")).Data)["adrlist"])[0];

            Assert.That(mqttConnectionAdr, Does.Contain("mqttconnection"), 
                message:"Expected First element to be mqttconnection. check element exists or modify query");

            Func<string,string> getStatus = mqttConnAdr => (string) (VariantValue) ((VariantObject)
                iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/mqttcmdchannel/status/getdata")).Data)["value"];

            /* Before mqtt server netadapter is started; Then mqttconnection/mqttcmdchannel/status/getdata is "init" */
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("init"));
            
            mqttServerNetAdapter1.Start();
            /* When invoked mqttcmdchannel/status/start; Then mqttconnection/mqttcmdchannel/status/getdata is "running" */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: mqttConnectionAdr + "/mqttcmdchannel/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("running"));

            /* When invoked mqttcmdchannel/status/stop; Then mqttconnection/mqttcmdchannel/status/getdata is "stopped" */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: mqttConnectionAdr + "/mqttcmdchannel/status/stop")).Code,
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("stopped"));

            mqttServerNetAdapter1.Stop();
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("stopped"));

            mqttServerNetAdapter1.RequestReceived -= HandleRequest;
            mqttServerNetAdapter1.EventReceived -= HandleEvent;
        }

        internal Message CreateRequestMessage(string adr ="/gettree", string data = "null", int code=10, MessageConverter.Json.Newtonsoft.MessageConverter converter = null)
        {
            if (converter == null) converter =  new MessageConverter.Json.Newtonsoft.MessageConverter();
            var cid = new Random().Next(0, int.MaxValue);
            return converter.Deserialize($"{{'code':{code}, 'cid':{cid}, 'adr':'{adr}', 'data':{data}}}");
        }

        [Test, Property("TestCaseKey", "IOTCS-T152"), NonParallelizable]
        public void Test_Setup_mqttCmdChannel_RunControlProfile_ResetService_Restarts() 
        {
            /* Given: 
             * iot core initialised, mqtt server net adapter initialised.
             * all required elements for run profile are present. 
             * mqtt server net adapter is started
             * commchannel runcontrol profile reset service is available - mqttconnection/mqttcmdchannel/status/reset
             */
            var uniqueIotId = Guid.NewGuid().ToString();
            var iotCore1 = IoTCoreFactory.Create(uniqueIotId, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            var msgConverter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            using var mqttServerNetAdapter1 = new MqttServerNetAdapter(iotCore1, iotCore1.Root, msgConverter, new IPEndPoint(IPAddress.Loopback, LocalBrokerPort),null);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter1.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter1.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter1);

            var mqttConnectionAdr = (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commInterface'}")).Data)["adrlist"])[0];
            Assert.That(mqttConnectionAdr, Does.Contain("mqttconnection"), 
                message:"Expected First element to be mqttconnection. check element exists or modify query");

            Func<string,string> getStatus = mqttConnAdr => (string) (VariantValue) ((VariantObject)
                iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/mqttcmdchannel/status/getdata")).Data)["value"];

            mqttServerNetAdapter1.Start();

            /* When in "init" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: mqttConnectionAdr + "/mqttcmdchannel/status/reset")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("running"));

            /* When in "stopped" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            mqttServerNetAdapter1.Stop();
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("stopped"), "test setup: expected 'stopped' state");
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: mqttConnectionAdr + "/mqttcmdchannel/status/reset")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("stopped")); // this is stopped since, the transition table in https://dettjira.intra.ifm/browse/IOTCS-691 stated, that reset from stopped goes to stopped.

            mqttServerNetAdapter1.Stop();
            Assert.That(getStatus(mqttConnectionAdr), Is.EqualTo("stopped"));

            mqttServerNetAdapter1.RequestReceived -= HandleRequest;
            mqttServerNetAdapter1.EventReceived -= HandleEvent;
        }

        [Test, Property("TestCaseKey", "IOTCS-T153"), NonParallelizable]
        public void Test_Setup_mqttCmdChannel_RunControlProfile_PresetService_Returns200Ok() 
        {
            /* Given: 
             * iot core initialised, mqtt server net adapter initialised.
             * all required elements for run profile are present. 
             * mqtt server net adapter is started
             * commchannel runcontrol profile preset service is available - mqttconnection/mqttcmdchannel/status/preset
             */
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            using var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort),null);

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var mqttConnectionAdr = (string) (VariantValue)((VariantArray)((VariantObject) iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commInterface'}")).Data)["adrlist"])[0];
            Assert.That(mqttConnectionAdr, Does.Contain("mqttconnection"), 
                message:"Expected First element to be mqttconnection. check element exists or modify query");

            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: mqttConnectionAdr + "/mqttcmdchannel/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommInterface_Running_Accepting_Messages()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            using  var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var commInterfaceAddress = (string) (VariantValue) ((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commInterface'}")).Data)["adrlist"])[0];

            Assert.That(commInterfaceAddress, Does.Contain("mqttconnection"), 
                message:"Expected First element to be mqttconnection. check element exists or modify query");

            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            Func<string,string> getStatus = mqttConnAdr => (string) (VariantValue) ((VariantObject)
                iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/mqttcmdchannel/status/getdata")).Data)["value"];

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/stop")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(commInterfaceAddress), Is.EqualTo("stopped"));
            
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(commInterfaceAddress), Is.EqualTo("running"));

            using (var manualResetEventSlim = new ManualResetEventSlim())
            using (var mqttClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger()))
            {
                var connectionResult = mqttClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
                Assert.That(connectionResult.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    manualResetEventSlim.Set();    
                });

                var subscribeResult = mqttClient.SubscribeAsync(new MqttClientSubscribeOptions()
                    {TopicFilters = {new MqttTopicFilter(){Topic = "replyTopic"}}}).GetAwaiter().GetResult();

                Assert.That(subscribeResult.Items.Any(x=>x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS1 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS2));

                var result = mqttClient.PublishAsync(new MqttApplicationMessage()
                {
                    ContentType = "json",
                    Payload = Encoding.ASCII.GetBytes(JObject.Parse(@"{'adr' : '/gettree', 'cid': 0, 'code': 10}").ToString()),
                    Topic = "cmdTopic/asdf"
                }).GetAwaiter().GetResult();

                Assert.That(result.ReasonCode, Is.EqualTo(MqttClientPublishReasonCode.Success));
                manualResetEventSlim.Wait(TimeSpan.FromSeconds(5));

                Assert.That(manualResetEventSlim.IsSet);
            }

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;

            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommInterface_Stopped_Not_Accepting_Messages()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            using var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var commInterfaceAddress =  (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commInterface'}")).Data)["adrlist"])[0];

            Assert.That(commInterfaceAddress, Does.Contain("mqttconnection"), 
                message:"Expected First element to be mqttconnection. check element exists or modify query");

            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            Func<string,string> getStatus = mqttConnAdr =>(string)(VariantValue)
                ((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/mqttcmdchannel/status/getdata")).Data)["value"];

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/stop")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(commInterfaceAddress), Is.EqualTo("stopped"));

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(commInterfaceAddress), Is.EqualTo("running"));

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commInterfaceAddress + "/mqttcmdchannel/status/stop")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(getStatus(commInterfaceAddress), Is.EqualTo("stopped"));

            using (var manualResetEventSlim = new ManualResetEventSlim())  {

                var mqttClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
                var connectionResult = mqttClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
                Assert.That(connectionResult.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    Assert.Fail("This message should not be received.");
                });

                var subscribeResult = mqttClient.SubscribeAsync(new MqttClientSubscribeOptions()
                    {TopicFilters = {new MqttTopicFilter(){Topic = "replyTopic"}}}).GetAwaiter().GetResult();

                Assert.That(subscribeResult.Items.Any(x=>x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS1 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS2));

                var result = mqttClient.PublishAsync(new MqttApplicationMessage()
                {
                    ContentType = "json",
                    Payload = Encoding.ASCII.GetBytes(JObject.Parse(@"{'adr' : '/gettree', 'cid': 8892, 'code': 10}").ToString()),
                    Topic = "cmdTopic/asdf"
                }).GetAwaiter().GetResult();

                Assert.That(result.ReasonCode, Is.EqualTo(MqttClientPublishReasonCode.Success));

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(2));

            }

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommChannel_Running_Accepting_Messages()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var commChannelAddress = (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commchannel'}")).Data)["adrlist"])[0];
            
            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            string GetStatus(string mqttConnAdr) => (string)(VariantValue)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/status/getdata")).Data)["value"];

            /* When in "init" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("running"));

            using (var manualResetEventSlim = new ManualResetEventSlim())   
            using (var mqttClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger())) {

                
                var connectionResult = mqttClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
                Assert.That(connectionResult.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    manualResetEventSlim.Set();    
                });

                var subscribeResult = mqttClient.SubscribeAsync(new MqttClientSubscribeOptions()
                    {TopicFilters = {new MqttTopicFilter(){Topic = "replyTopic"}}}).GetAwaiter().GetResult();

                Assert.That(subscribeResult.Items.Any(x=>x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS1 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS2));

                var result = mqttClient.PublishAsync(new MqttApplicationMessage()
                {
                    ContentType = "json",
                    Payload = Encoding.ASCII.GetBytes(JObject.Parse(@"{'adr' : '/gettree', 'cid': 0, 'code': 10}").ToString()),
                    Topic = "cmdTopic/asdf"
                }).GetAwaiter().GetResult();

                Assert.That(result.ReasonCode, Is.EqualTo(MqttClientPublishReasonCode.Success));

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(5));

                Assert.That(manualResetEventSlim.IsSet);
            }

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommChannel_Stopped_Not_Accepting_Messages()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var commChannelAddress = (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commchannel'}")).Data)["adrlist"])[0];
            
            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            string GetStatus(string mqttConnAdr) => (string)(VariantValue)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/status/getdata")).Data)["value"];

            /* When in "init" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("running"),"Status of commchannel runcontrol should be started.");

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/stop")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("stopped"), "Status of commchannel runcontrol should be stopped.");

            using (var manualResetEventSlim = new ManualResetEventSlim())   
            using (var mqttClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger())) {

                
                var connectionResult = mqttClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
                Assert.That(connectionResult.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));

                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    manualResetEventSlim.Set();
                    Assert.Fail("This message should not be received upon stopped.");
                });

                var subscribeResult = mqttClient.SubscribeAsync(new MqttClientSubscribeOptions()
                    {TopicFilters = {new MqttTopicFilter(){Topic = "replyTopic"}}}).GetAwaiter().GetResult();

                Assert.That(subscribeResult.Items.Any(x=>x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS1 || x.ResultCode == MqttClientSubscribeResultCode.GrantedQoS2));

                var result = mqttClient.PublishAsync(new MqttApplicationMessage()
                {
                    ContentType = "json",
                    Payload = Encoding.ASCII.GetBytes(JObject.Parse(@"{'adr' : '/gettree', 'cid': 0, 'code': 10}").ToString()),
                    Topic = "cmdTopic/asdf"
                }).GetAwaiter().GetResult();

                Assert.That(result.ReasonCode, Is.EqualTo(MqttClientPublishReasonCode.Success));

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(2));

                Assert.That(!manualResetEventSlim.IsSet);
            }

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommChannel_Running_To_Running_Transition()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var commChannelAddress = (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commchannel'}")).Data)["adrlist"])[0];
            
            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            string GetStatus(string mqttConnAdr) => (string)(VariantValue)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/status/getdata")).Data)["value"];

            /* When in "init" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("running"),"Status of commchannel runcontrol should be started.");

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("running"), "Status of commchannel runcontrol should be stopped.");

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommChannel_Stopped_To_Running_Transition()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, LocalBrokerPort));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            var commChannelAddress = (string)(VariantValue)((VariantArray)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commchannel'}")).Data)["adrlist"])[0];
            
            mqttServerNetAdapter.Start();

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            string GetStatus(string mqttConnAdr) => (string)(VariantValue)((VariantObject) iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/status/getdata")).Data)["value"];

            /* When in "init" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/stop")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("stopped"),"Status of commchannel runcontrol should be started.");

            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/start")).Code, 
                Is.EqualTo(ResponseCodes.Success));
            Assert.That(GetStatus(commChannelAddress), Is.EqualTo("running"), "Status of commchannel runcontrol should be stopped.");

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Stop();
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        public void Test_Statetransitions_CommChannel_Running_To_Error_Transition()
        {
            var iotCore1 = IoTCoreFactory.Create(Guid.NewGuid().ToString(), null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Debug));
            
            var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore1, iotCore1.Root, new MessageConverter.Json.Newtonsoft.MessageConverter(), new IPEndPoint(IPAddress.Loopback, 9999));

            void HandleRequest(object sender, RequestMessageEventArgs args)
            {
                args.ResponseMessage = iotCore1.HandleRequest(args.RequestMessage);
            }

            mqttServerNetAdapter.RequestReceived += HandleRequest;

            void HandleEvent(object sender, EventMessageEventArgs args)
            {
                iotCore1.HandleEvent(args.EventMessage);
            }

            mqttServerNetAdapter.EventReceived += HandleEvent;

            iotCore1.RegisterServerNetAdapter(mqttServerNetAdapter);

            //var commChannelAddress = iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commchannel'}")).Data?.SelectToken("$..adrlist[0]")?.Value<string>();

            var dataAsVariant = (VariantObject) iotCore1.HandleRequest(this.CreateRequestMessage(adr: "/querytree", data: "{'profile':'commchannel'}")).Data;
            var commChannelAddress = (VariantValue) ((VariantArray) dataAsVariant["adrlist"])[0];

            try
            {
                mqttServerNetAdapter.Start();
            }
            catch
            {
                this.Logger.Warn("Expected connectione error.");
            }

            /* When preset service invoked Then it returns successfully */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/preset/getdata")).Code, 
                Is.EqualTo(ResponseCodes.Success));

            string GetStatus(string mqttConnAdr) => (string)(VariantValue)((VariantObject)iotCore1.HandleRequest(this.CreateRequestMessage(adr: mqttConnAdr + "/status/getdata")).Data)["value"];

            /* When in "init" status (mqttcmdchannel/status), Then Reset (mqttcmdchannel/status/reset) will restart */
            Assert.That(
                iotCore1.HandleRequest(
                    this.CreateRequestMessage(adr: commChannelAddress + "/status/start")).Code, 
                Is.EqualTo(ResponseCodes.InternalError));
            Assert.That(GetStatus((string)commChannelAddress), Is.EqualTo("error"),"Status of commchannel runcontrol should be started.");

            try
            {
                mqttServerNetAdapter.Stop();
                
            }
            catch
            {
                Logger.Warn("Expected exception.");
            }

            mqttServerNetAdapter.RequestReceived -= HandleRequest;
            mqttServerNetAdapter.EventReceived -= HandleEvent;
            //mqttServerNetAdapter.Dispose();
            iotCore1.RemoveServerNetAdapter(mqttServerNetAdapter);
        }

        [Test, NonParallelizable]
        [Ignore("TODO postponed to next release")]
        public void Test_SendRequest_Mqtt_DoesNotThrowOnValidInput()
        {
            IClientNetAdapter client = null;
            try
            {
                client = new MqttClientNetAdapter(
                    "cmdTopic",
                    new IPEndPoint(
                        IPAddress.Loopback,
                        LocalBrokerPort),
                    new MessageConverter.Json.Newtonsoft.MessageConverter(),
                    TimeSpan.FromSeconds(1));

                Assert.DoesNotThrow(() =>
                {
                    client.SendRequest(new Message(RequestCodes.Request, 10, "/gettree",
                        Variant.FromObject(new GetTreeRequestServiceData("/", 0))));
                });
            }
            finally
            {
                client?.Dispose();
            }
        }

        [Test, NonParallelizable]
        [Ignore("TODO test after SendRequest DoesNotThrow Exception")]
        public void Test_SendRequest_Mqtt_sendsAppMessageTo_MqttBroker()
        {
            var randomCid = new Random().Next();
            string eventTopic = $"testEvent{randomCid}";
            string iotResponseString = null; 

            //* Given local mqtt broker running at loopback
            IClientNetAdapter client = new MqttClientNetAdapter(
                eventTopic, 
                new IPEndPoint(
                    IPAddress.Loopback,
                    LocalBrokerPort), 
                new MessageConverter.Json.Newtonsoft.MessageConverter(), 
                TimeSpan.FromSeconds(1));

            var msgSentOnMqtt = new ManualResetEventSlim();
            var brokerAppMessagesClient = new MqttClient(new MqttClientAdapterFactory(new MqttNetNullLogger()), new MqttNetNullLogger());
            var connectionResult2 = brokerAppMessagesClient.ConnectAsync(MqttHelper.BuildOptions(IPAddress.Loopback, LocalBrokerPort)).GetAwaiter().GetResult();
            Assert.That(connectionResult2.ResultCode, Is.EqualTo(MqttClientConnectResultCode.Success));
            try 
            { 
                brokerAppMessagesClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(mqtteventargs => 
                {
                   iotResponseString = Encoding.UTF8.GetString(mqtteventargs.ApplicationMessage.Payload);
                   if (mqtteventargs.ApplicationMessage.Topic == eventTopic && iotResponseString.Contains(randomCid.ToString()))
                   {
                       msgSentOnMqtt.Set();
                   } 
                });
                var subscribeResult2 = brokerAppMessagesClient.SubscribeAsync(eventTopic).GetAwaiter().GetResult();
                Assert.AreEqual(MqttClientSubscribeResultCode.GrantedQoS0, subscribeResult2.Items[0].ResultCode);
                //* When SendRequest sends

                client.SendRequest(new Message(RequestCodes.Request, 10, "/gettree", Variant.FromObject(new GetTreeRequestServiceData("/", 0))));
                msgSentOnMqtt.Wait(TimeSpan.FromSeconds(2));
            }
            finally
            { // don't forget to close shared resources for other tests to reuse
                brokerAppMessagesClient.DisconnectAsync().Wait();
                msgSentOnMqtt.Dispose();
                client.Dispose();
            }
            //* Then message is received by broker
            Assert.IsNotNull(iotResponseString, "Expected broker reconnected and thus response available");

        }

        [Test, NonParallelizable]
        [Ignore("TODO after SendRequest test passes")]
        public void Test_SendEvent_Mqtt_sendsAppMessageTo_MqttBroker()
        {
            //var jsconvert = new MessageConverter.Json.Newtonsoft.MessageConverter();
            //client.SendRequest(this.CreateRequestMessage());

            //client.SendEvent(jsconvert.Deserialize<EventMessage>($"{{'cid':{1}, 'code':{RequestCodes.Event}, 'profileData':{{'/identifier':0}}"));
        }

        private void MqttServerNetAdapterOnEventReceived(object sender, EventMessageEventArgs e)
        {
            this._iotCore.HandleEvent(e.EventMessage);
        }

        private void MqttServerNetAdapterOnRequestReceived(object sender, RequestMessageEventArgs e)
        {
            e.ResponseMessage = this._iotCore.HandleRequest(e.RequestMessage);
        }

    }
}
