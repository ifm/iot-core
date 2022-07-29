namespace ifmIoTCore.NetAdapter.Websocket.UnitTests
{
    using Common;
    using Common.Variant;
    using Elements.ServiceData.Requests;
    using Logger;
    using MessageConverter.Json.Microsoft;
    using Messages;
    using WebSocket;

    [TestFixture]
    public class Tests
    {
        private IIoTCore? _iotCore;
        private WebSocketServerNetAdapter? _serverNetAdapter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _iotCore = IoTCoreFactory.Create("test");
            this._serverNetAdapter = new WebSocketServerNetAdapter(_iotCore, new Uri("http://127.0.0.1:8090"), new MessageConverter(), new NullLogger());
            _iotCore.RegisterServerNetAdapter(_serverNetAdapter);
            _serverNetAdapter.Start();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _serverNetAdapter?.Dispose();
            _iotCore?.Dispose();
        }

        [Test]
        public async Task TestRequestAndDispose()
        {
            using (var client = new WebSocketClientNetAdapter(new TestingMessageHandler(),
                       new Uri("ws://127.0.0.1:8090?clientId=testing"), new MessageConverter(), new NullLogger(),
                       TimeSpan.FromMilliseconds(20)))
            {
                var response = client.SendRequest(new Message(RequestCodes.Request, 1, "/gettree", null), TimeSpan.FromMilliseconds(20));
                Assert.IsNotNull(response);
                Assert.That(response.Code, Is.EqualTo(ResponseCodes.Success));
            }

            await Task.Delay(TimeSpan.FromMilliseconds(1));

            Assert.That(_serverNetAdapter?.ConnectedClients != null);
            Assert.That(!_serverNetAdapter.ConnectedClients.Any());
        }

        [Test]
        public void Test2()
        {
            var client = new WebSocketClientNetAdapter(new TestingMessageHandler(),
                new Uri("ws://127.0.0.1:8090?clientId=testing"), new MessageConverter(), new NullLogger(),
                TimeSpan.FromMilliseconds(50));

            client.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { client.SendEvent(new Message(RequestCodes.Event, 0, "/test", null)); });
        }

        [Test]
        public void SubscribeTest()
        {
            var testingMessageHandler = new TestingMessageHandler();

            using (var client = new WebSocketClientNetAdapter(testingMessageHandler,
                       new Uri("ws://127.0.0.1:8090?clientId=testSubscribe"), new MessageConverter(), new NullLogger(),
                       TimeSpan.FromMilliseconds(20)))
                using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                var response = client.SendRequest(new Message(RequestCodes.Request, 1, "/treechanged/subscribe", Variant.FromObject(new SubscribeRequestServiceData("ws:?clientId=testSubscribe"))), TimeSpan.FromMilliseconds(20));
                Assert.IsNotNull(response);
                Assert.That(response.Code, Is.EqualTo(ResponseCodes.Success));

                void HandleEvent(object? sender, EventMessageEventArgs e)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEventSlim.Set();
                }

                testingMessageHandler.EventMessageReceived += HandleEvent;
                
                _iotCore.Root.RaiseTreeChanged();

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(1));

                Assert.That(manualResetEventSlim.IsSet);

                testingMessageHandler.EventMessageReceived -= HandleEvent;
            }
        }

        [Test]
        public void SubscribeAndDisconnectClientTest()
        {
            var testingMessageHandler = new TestingMessageHandler();

            using (var manualResetEventSlim = new ManualResetEventSlim())
            using (var manualResetEventSlimDisconnect = new ManualResetEventSlim())
            {
                var client = new WebSocketClientNetAdapter(testingMessageHandler,
                    new Uri("ws://127.0.0.1:8090?clientId=testSubscribe"), new MessageConverter(), new NullLogger(),
                    TimeSpan.FromMilliseconds(20));

                var response = client.SendRequest(new Message(RequestCodes.Request, 1, "/treechanged/subscribe", Variant.FromObject(new SubscribeRequestServiceData("ws:?clientId=testSubscribe"))), TimeSpan.FromMilliseconds(20));
                Assert.IsNotNull(response);
                Assert.That(response.Code, Is.EqualTo(ResponseCodes.Success));

                void HandleEvent(object? sender, EventMessageEventArgs e)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEventSlim.Set();
                }

                testingMessageHandler.EventMessageReceived += HandleEvent;

                _iotCore?.Root.RaiseTreeChanged();

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(1));

                Assert.That(manualResetEventSlim.IsSet);

                testingMessageHandler.EventMessageReceived -= HandleEvent;

                manualResetEventSlim.Reset();

                client.Dispose();

                void ClientRemovedHandler(object? sender, ClientRemovedEventArgs e)
                {
                    if (e.ClientId == "testSubscribe")
                    {
                        manualResetEventSlimDisconnect.Set();
                    }
                }

                _serverNetAdapter.ClientRemoved += ClientRemovedHandler;

                manualResetEventSlimDisconnect.Wait(TimeSpan.FromSeconds(1));

                try
                {
                    Assert.That(manualResetEventSlimDisconnect.IsSet);
                    Assert.That(_serverNetAdapter?.ConnectedClients.Count(), Is.EqualTo(0));
                }
                finally
                {
                    _serverNetAdapter.ClientRemoved -= ClientRemovedHandler;
                }
            }
        }

        [Test]
        public void SubscribeAndDisconnectAndReconnectClientTest()
        {
            var testingMessageHandler = new TestingMessageHandler();

            using (var manualResetEventSlimDisconnect = new ManualResetEventSlim())
            using (var manualResetEventSlimConnect = new ManualResetEventSlim())
            using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                var client = new WebSocketClientNetAdapter(testingMessageHandler,
                    new Uri("ws://127.0.0.1:8090?clientId=testSubscribe"), new MessageConverter(), new NullLogger(),
                    TimeSpan.FromMilliseconds(20));

                var response = client.SendRequest(new Message(RequestCodes.Request, 1, "/treechanged/subscribe", Variant.FromObject(new SubscribeRequestServiceData("ws:?clientId=testSubscribe"))), TimeSpan.FromMilliseconds(20));
                Assert.IsNotNull(response);
                Assert.That(response.Code, Is.EqualTo(ResponseCodes.Success));

                void HandleEvent(object? sender, EventMessageEventArgs e)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEventSlim.Set();
                }

                testingMessageHandler.EventMessageReceived += HandleEvent;

                _iotCore?.Root.RaiseTreeChanged();

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(1));

                Assert.That(manualResetEventSlim.IsSet);

                testingMessageHandler.EventMessageReceived -= HandleEvent;

                manualResetEventSlim.Reset();

                void ClientRemovedHandler(object? sender, ClientRemovedEventArgs e)
                {
                    if (e.ClientId == "testSubscribe")
                    {
                        manualResetEventSlimDisconnect.Set();
                    }
                }

                _serverNetAdapter.ClientRemoved += ClientRemovedHandler;

                client.Dispose();

                manualResetEventSlimDisconnect.Wait(TimeSpan.FromSeconds(2));

                Assert.That(manualResetEventSlimDisconnect.IsSet);

                _serverNetAdapter.ClientRemoved -= ClientRemovedHandler;

                var testingMessageHandler2 = new TestingMessageHandler();

                void HandleEvent2(object? sender, EventMessageEventArgs e)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEventSlim.Set();
                }

                testingMessageHandler2.EventMessageReceived += HandleEvent2;

                var client2 = new WebSocketClientNetAdapter(testingMessageHandler2,
                    new Uri("ws://127.0.0.1:8090?clientId=testSubscribe"), new MessageConverter(), new NullLogger(),
                    TimeSpan.FromMilliseconds(320));

                Assert.That(_serverNetAdapter?.ConnectedClients.Count(), Is.EqualTo(0));

                void ClientAddedHandler(object sender, ClientAddedEventArgs e)
                {
                    manualResetEventSlimConnect.Set();
                }

                _serverNetAdapter.ClientAdded += ClientAddedHandler;

                client2.Connect();

                manualResetEventSlimConnect.Wait(TimeSpan.FromSeconds(1));

                Assert.That(manualResetEventSlimConnect.IsSet);

                _serverNetAdapter.ClientAdded -= ClientAddedHandler;

                _iotCore.Root.RaiseTreeChanged();

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(10));

                Assert.That(manualResetEventSlim.IsSet);

                testingMessageHandler2.EventMessageReceived -= HandleEvent2;

                client2.Dispose();
            }
        }

        [Test]
        [TestCase("?clientId=test_plainclientId")]
        [TestCase("?clientId=test_plainclientId1")]
        [TestCase("?clientId=test_plainclientId2")]
        [Ignore("", Until = "2022-07-30")]
        public void CheckValidQueries(string query)
        {
            using (var client = new WebSocketClientNetAdapter(
                       new TestingMessageHandler(),
                       new Uri("ws://127.0.0.1:8090" + query),
                       new MessageConverter(),
                       new NullLogger(),
                       TimeSpan.FromMilliseconds(30)))
                using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                void ServerNetAdapterOnClientRemoved(object? sender, ClientRemovedEventArgs e)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEventSlim.Set();
                }

                _serverNetAdapter.ClientRemoved += ServerNetAdapterOnClientRemoved;

                client.Connect();
                client.Disconnect();

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(1));
                _serverNetAdapter.ClientRemoved -= ServerNetAdapterOnClientRemoved;
            }

            Assert.That(_serverNetAdapter.ConnectedClients.Count() == 0);
        }

        [Test]
        [TestCase("?clientId==")]
        [TestCase("?clientId=?clientId=clientId")]
        [TestCase("?clientId=")]
        [TestCase("")]
        [TestCase("?clientId")]
        [Ignore("", Until = "2022-07-30")]
        public void CheckInValidQueries(string query)
        {
            using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                _serverNetAdapter.ClientRemoved += (sender, args) =>
                {
                    manualResetEventSlim.Set();
                };
                
                var client = new WebSocketClientNetAdapter(
                    new TestingMessageHandler(),
                    new Uri("ws://127.0.0.1:8090" + query),
                    new MessageConverter(),
                    new NullLogger(),
                    TimeSpan.FromMilliseconds(330));
                
                Assert.Throws<System.Net.WebSockets.WebSocketException>(() =>
                {
                    client.Connect();
                });
                
                    try
                    {
                        client.Dispose();
                    }
                    catch (Exception)
                    {
                    
                    }

                manualResetEventSlim.Wait(TimeSpan.FromSeconds(1));

                Assert.That(manualResetEventSlim.IsSet);
            }
        }

        private class TestingMessageHandler : IMessageHandler
        {
            public event EventHandler<RequestMessageEventArgs>? RequestMessageReceived;
            public event EventHandler<EventMessageEventArgs>? EventMessageReceived;
            public event EventHandler<RequestMessageEventArgs>? RequestMessageResponded;
            public Message HandleRequest(Message message)
            {
                var response = new Message(ResponseCodes.NotImplemented, 0, message.Address, null);
                RequestMessageReceived.Raise(this, new RequestMessageEventArgs(message, response));
                return response;
            }

            public void HandleEvent(Message message)
            {
                EventMessageReceived.Raise(this, new EventMessageEventArgs(message));
            }
        }
    }
}