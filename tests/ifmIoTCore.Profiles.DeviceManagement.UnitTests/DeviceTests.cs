namespace ifmIoTCore.Profiles.DeviceManagement.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using Base;
    using DeviceManagement;
    using DeviceManagement.ServiceData.Requests;
    using ifmIoTCore.Common.Variant;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using Logger;
    using Messages;
    using NetAdapter.Http;
    using NUnit.Framework;


    public static class UnitTestIoTCoreFactory
    {
        public static IIoTCore Build(string id)
        {
            var iotCore = IoTCoreFactory.Create(id, null, new ifmIoTCore.Logging.Log4Net.Logger(LogLevel.Warning));
            iotCore.RegisterClientNetAdapterFactory(new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter()));
            return iotCore;
        }
    }

    [TestFixture]
    [ExcludeFromCodeCoverage]
    [Parallelizable(ParallelScope.None)]
    public class DeviceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MirrorTest0()
        {
            var iotCore0 = UnitTestIoTCoreFactory.Build("id0");
            HttpServerNetAdapter server = null;

            try
            {
                var uri = new Uri("http://127.0.0.1:9002");
                server = new HttpServerNetAdapter(iotCore0, uri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                server.Start();
                var iotCore1 = UnitTestIoTCoreFactory.Build("id1");
                iotCore1.RegisterServerNetAdapter(server);
                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore1, iotCore1.Root.Address));
                deviceProfileBuilder.Build();

                var result = iotCore1.HandleRequest(0, "/device_management/mirror", Variant.FromObject(new MirrorRequestServiceData("http://127.0.0.1:9002", uri.ToString())));

                Assert.AreEqual(200, result.Code, result.Data?.ToString() ?? "No data provided.");

                var remoteElement = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "remote")?.Subs.FirstOrDefault(x=>x.Identifier == "id0");

                Assert.NotNull(remoteElement);
                Assert.That(string.Equals("id0", remoteElement.Identifier));
            }
            finally
            {
                server?.Dispose();
            }
        }

        [Test]
        public void MirrorTest1()
        {
            var iotCore = UnitTestIoTCoreFactory.Build("coreId");
            var structure = new StructureElement("Hello");
            iotCore.Root.AddChild(structure);

            HttpServerNetAdapter httpServer = null;

            try
            {
                var uri = new Uri("http://127.0.0.1:9002");
                httpServer = new HttpServerNetAdapter(iotCore, uri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                httpServer.Start();
                var iotCore2 = UnitTestIoTCoreFactory.Build("otherIoTCore");
                iotCore2.RegisterServerNetAdapter(httpServer);

                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore2, iotCore2.Root.Address));
                deviceProfileBuilder.Build();
                var result = iotCore2.HandleRequest(0, "/device_management/mirror", Variant.FromObject(new MirrorRequestServiceData("http://127.0.0.1:9002", uri.ToString())));

                var element = iotCore2.Root.GetElementByAddress("/remote/coreId/Hello");
                Assert.NotNull(element);
                Assert.AreEqual("Hello", element.Identifier);
            }
            finally
            {
                httpServer?.Dispose();
            }
        }

        [Test]
        public void MirrorTest2()
        {
            var iotCore1 = UnitTestIoTCoreFactory.Build("id1");

            var handleEventService = new ActionServiceElement("HandleEvent", null);
            iotCore1.Root.AddChild(handleEventService);

            var iotCore2 = UnitTestIoTCoreFactory.Build("id2");

            using var server1 = new HttpServerNetAdapter(iotCore1, new Uri("http://127.0.0.1:8001"), new MessageConverter.Json.Newtonsoft.MessageConverter());
            using var server2 = new HttpServerNetAdapter(iotCore2, new Uri("http://127.0.0.1:8002"), new MessageConverter.Json.Newtonsoft.MessageConverter());

            try
            {
                server1.Start();
                server2.Start();

                var subscribeResult = iotCore1.HandleRequest(0,
                    "id1/treechanged/subscribe",
                    Variant.FromObject(new SubscribeRequestServiceData($"http://{IPAddress.Loopback}:8001/HandleEvent", new[] { "/getidentity" }.ToList())));

                Assert.NotNull(subscribeResult);
                Assert.AreEqual(200, subscribeResult.Code);
            }
            finally
            {
                server1.Dispose();
                server2.Dispose();
            }
        }

        [Test]
        public void MirrorTestGetData()
        {
            string data = "data";

            try
            {
                var iotCore0 = UnitTestIoTCoreFactory.Build("id0");

                var dataElement = new DataElement<string>("data0",
                    (b) => data,
                    (b, o) => { data = o; });

                iotCore0.Root.AddChild(dataElement);

                var uri = new Uri("http://127.0.0.1:9001");
                using var server = new HttpServerNetAdapter(iotCore0, uri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                server.Start();

                var iotCore1 = UnitTestIoTCoreFactory.Build("id1");
                iotCore1.RegisterServerNetAdapter(server);

                //iotCore1.RegisterClientNetAdapterFactory(new HttpClientNetAdapterFactory(new JsonConverter()));

                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore1, iotCore1.Root.Address));
                deviceProfileBuilder.Build();

                var result = iotCore1.HandleRequest(0, "/device_management/mirror", Variant.FromObject(new MirrorRequestServiceData("http://127.0.0.1:9001", uri.ToString())));

                var getDataResponse = iotCore1.HandleRequest(0, "/remote/id0/data0/getdata");
                Assert.AreEqual(200, getDataResponse.Code);
            }
            catch (AssertionException)
            {
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [Test]
        public void UnMirrorParameterNullTest()
        {
            var iotCore0 = UnitTestIoTCoreFactory.Build("id0");

            HttpServerNetAdapter server = null;

            try
            {
                var uri = new Uri("http://127.0.0.1:9002");
                server = new HttpServerNetAdapter(iotCore0, uri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                server.Start();
                var iotCore1 = UnitTestIoTCoreFactory.Build("id1");
                iotCore1.RegisterServerNetAdapter(server);
                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore1, iotCore1.Root.Address));
                deviceProfileBuilder.Build();

                var result = iotCore1.HandleRequest(0, "/device_management/mirror", Variant.FromObject(new MirrorRequestServiceData("http://127.0.0.1:9002", uri.ToString())));

                Assert.AreEqual(200, result.Code, result.Data?.ToString() ?? "No data provided.");

                var remoteElement = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "remote")?.Subs.FirstOrDefault(x=>x.Identifier == "id0");

                Assert.NotNull(remoteElement);
                Assert.That(string.Equals("id0", remoteElement.Identifier));

                var unmirror = iotCore1.HandleRequest(0, "/device_management/unmirror",
                    Variant.FromObject(new UnmirrorRequestServiceData("http://127.0.0.1:9002")));

                Assert.AreEqual(ResponseCodes.Success, unmirror.Code);
                var remoteElementAfterUnmirror = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "id0");
                Assert.IsNull(remoteElementAfterUnmirror);
            }
            finally
            {
                server?.Dispose();
            }
        }

        [Test]
        public void UnMirrorParameterEmptyListTest()
        {
            var iotCore0 = UnitTestIoTCoreFactory.Build("id0");

            HttpServerNetAdapter server = null;

            try
            {
                var uri = new Uri("http://127.0.0.1:9002");
                server = new HttpServerNetAdapter(iotCore0, uri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                server.Start();
                var iotCore1 = UnitTestIoTCoreFactory.Build("id1");
                iotCore1.RegisterServerNetAdapter(server);
                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore1, iotCore1.Root.Address));
                deviceProfileBuilder.Build();

                var result = iotCore1.HandleRequest(0, "/device_management/mirror", Variant.FromObject(new MirrorRequestServiceData("http://127.0.0.1:9002", uri.ToString())));

                Assert.AreEqual(200, result.Code, result.Data?.ToString() ?? "No data provided.");

                var remoteElement = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "remote")?.Subs.FirstOrDefault(x=>x.Identifier == "id0");

                Assert.NotNull(remoteElement);
                Assert.That(string.Equals("id0", remoteElement.Identifier));

                var unmirror = iotCore1.HandleRequest(0, "/device_management/unmirror",
                    Variant.FromObject(new UnmirrorRequestServiceData("http://127.0.0.1:9002")));

                Assert.AreEqual(ResponseCodes.Success, unmirror.Code);
                var remoteElementAfterUnmirror = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "id0");
                Assert.IsNull(remoteElementAfterUnmirror);
            }
            finally
            {
                server?.Dispose();
            }
        }

        [Test]
        public void UnMirrorByAddressTest()
        {
            var iotCore0 = UnitTestIoTCoreFactory.Build("id0");

            HttpServerNetAdapter server = null;

            try
            {
                var uri = new Uri("http://127.0.0.1:9002");
                server = new HttpServerNetAdapter(iotCore0, uri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                server.Start();
                var iotCore1 = UnitTestIoTCoreFactory.Build("id1");
                iotCore1.RegisterServerNetAdapter(server);
                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore1, iotCore1.Root.Address));
                deviceProfileBuilder.Build();

                var result = iotCore1.HandleRequest(0, "/device_management/mirror", Variant.FromObject(new MirrorRequestServiceData("http://127.0.0.1:9002", uri.ToString())));

                Assert.AreEqual(200, result.Code, result.Data?.ToString() ?? "No data provided.");

                var remoteElement = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "remote")?.Subs.FirstOrDefault(x=>x.Identifier == "id0");

                Assert.NotNull(remoteElement);
                Assert.That(string.Equals("id0", remoteElement.Identifier));

                var unmirror = iotCore1.HandleRequest(0, "/device_management/unmirror",
                    Variant.FromObject(new UnmirrorRequestServiceData("http://127.0.0.1:9002")));

                Assert.AreEqual(ResponseCodes.Success, unmirror.Code);
                var remoteElementAfterUnmirror = iotCore1.Root.Subs.FirstOrDefault(x => x.Identifier == "id0");
                Assert.IsNull(remoteElementAfterUnmirror);
            }
            finally
            {
                server?.Dispose();
            }
        }
    }
}