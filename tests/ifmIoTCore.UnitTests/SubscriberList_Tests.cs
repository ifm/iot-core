using System.IO;
using ifmIoTCore.NetAdapter;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ifmIoTCore;
    using Converter.Json;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    internal class StartNetAdapterServerAndGetResponseHttp
    {
        public Message Do(RequestMessage requestMessage, IIoTCore ioTCore, Uri httpServerUri, TextWriter testContextTextWriter)
        {
            IServerNetAdapter httpserver = null;
            IClientNetAdapter httpclient = null;

            try
            {
                httpserver = new HttpServerNetAdapter(ioTCore, httpServerUri, new JsonConverter());
                ioTCore.RegisterServerNetAdapter(httpserver);
                httpclient = new HttpClientNetAdapter(httpServerUri, new JsonConverter(), TimeSpan.FromSeconds(30), false);
            
                httpserver.Start();
                var responseMessage = httpclient.SendRequest(requestMessage, null);

                if (responseMessage == null)
                {
                    throw new Exception($"Responsemessage is null. Message: '{requestMessage}', ServerUri: '{httpServerUri}'.");
                }

                return responseMessage;
            }
            catch (Exception exception)
            {
                testContextTextWriter.WriteLine("An exception has occured.");
                testContextTextWriter.WriteLine(exception.Message);
            }
            finally
            {
                ioTCore.RemoveServerNetAdapter(httpserver);
                httpserver.Dispose();
                httpclient.Dispose();
            }
            
            throw new Exception($"Client request yielded no response. ServerUri: {httpServerUri}");
        }
    }

    [TestFixture]
    public class SubscriberList_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T38")]
        public void SubscriberList_MultipleEvents()
        { // this test for subscriberlist assumes, subscribe, trigger event works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);

            using var clientNetAdapterFactory= new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                    (sender) => { return 42; },
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            var eventIDs = new List<string> { "myevent", "myevent2", "myevent3", "myevent4", "myevent5" };
            var randomCids = new List<int>(5);
            foreach (var id in eventIDs)
            {
                var myevent = ioTCore.CreateEventElement(ioTCore.Root, identifier:id);
                var svcaddr = string.Format("/{0}/subscribe", myevent.Identifier);
                var data = JToken.Parse(@" {
                                    'callback': 'callback/not/considered/on/subscribe', 
                                    'datatosend': ['/data1'] }");
                //var subscribeReq = ioTCore.HandleRequest( serviceAddress: svcaddr, data: data );
                // new message with explicit cid for test purpose
                var randomCid = new Random().Next(minValue:0, maxValue:int.MaxValue);
                randomCids.Add(randomCid);
                var subscribeReq = ioTCore.HandleRequest(new RequestMessage(cid: randomCid, address: svcaddr, data: data)); 
            }
            // check subscriptions were appended
            var subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(5, subscriptions.Data.Count());
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(subscriptions.Data.SelectToken($"$[{i}].subscribeid")?.ToObject<int>(), randomCids[i], "subscribeid should match the subscribe request. iot core compatibility");
            }

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T40")]
        public void SubscriberListService_accessibleThrough_http()
        { // integration test: this test for subscriberlist assumes, subscribe, trigger event works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            ioTCore.CreateDataElement<int>(ioTCore.Root, 
                "data1", (sender) => { return 42; }, 
                    null, 
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            var eventIDs = new List<string> { "myevent", "myevent2", "myevent3", "myevent4", "myevent5" };
            for (int id = 0; id < eventIDs.Count; id++)
            {
                var myevent = ioTCore.CreateEventElement(ioTCore.Root, eventIDs[id]);
                var svcaddr = string.Format("/{0}/subscribe", myevent.Identifier);
                var data = JToken.Parse(@" {
                                    'callback': 'callback/not/considered/on/subscribe', 
                                    'datatosend': ['/data1'], 
                                    'subscribeid': 0 } ");
                data["subscribeid"] = id;

                var subscribeReq = ioTCore.HandleRequest(0,
                    svcaddr,
                    data: data
                    );
            }

            var subscriptions = new StartNetAdapterServerAndGetResponseHttp().Do(
                new RequestMessage(1, "/getsubscriberlist", null),
                ioTCore,
                new Uri("http://127.0.0.1:10001"), TestContext.Out);
            // check subscriptions were appended
            Assert.AreEqual(5, subscriptions.Data.Count(), "expected 5 subscriptions"); 
            Assert.AreEqual(subscriptions.Data.SelectToken("$[0].subscribeid")?.ToObject<int>(), 0);
            Assert.AreEqual(subscriptions.Data.SelectToken("$[1].subscribeid")?.ToObject<int>(), 1);
            Assert.AreEqual(subscriptions.Data.SelectToken("$[2].subscribeid")?.ToObject<int>(), 2);
            Assert.AreEqual(subscriptions.Data.SelectToken("$[3].subscribeid")?.ToObject<int>(), 3);
            Assert.AreEqual(subscriptions.Data.SelectToken("$[4].subscribeid")?.ToObject<int>(), 4);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

    }
}
