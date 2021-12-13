namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Converter.Json;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    internal class ReceiveIoTMsg
    {
        private HttpListener _singleRequestListener;
        public ReceiveIoTMsg(string uriToListen)
        {
            _singleRequestListener = new HttpListener();
            _singleRequestListener.Prefixes.Add(uriToListen);
            _singleRequestListener.Start();
        }

        public JToken Do(int receiveTimeoutMilliseconds = 10000)
        {
            string result = null;

            try
            {
                // utility to receive http message; acting as a minimal http server
                // use httplistener to listen UriToListen
                var task = _singleRequestListener.GetContextAsync();
                task.Wait(millisecondsTimeout: receiveTimeoutMilliseconds);
                if (!task.IsCompleted)
                {
                    _singleRequestListener.Close();
                    return null; // return null if result not availalble in time specified by ReceiveTimeoutMilliseconds
                }
                var clientRequest = task.Result.Request;
                using (var streamReader  = new StreamReader(clientRequest.InputStream, System.Text.Encoding.UTF8))
                {
                    result = streamReader.ReadToEnd();
                    _singleRequestListener.Close(); //singleRequestListener.Stop(); // Stop can re(Start) later
                    _singleRequestListener = null;
                }
                
            }
            finally
            {
                _singleRequestListener?.Close(); //singleRequestListener.Stop(); // Stop can re(Start) later
                ((IDisposable)_singleRequestListener)?.Dispose();
                _singleRequestListener = null;
            }

            return JToken.Parse(result);
        }
    }

    [TestFixture]
    public class Event_Subscribe_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void Subscribe_ValidRequest_AcknowledgedWith_uid_Response()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/getidentity'], 
                                'subscribeid': 1}
                            }");
            var converter = new JsonConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            Assert.AreEqual(ResponseCodes.Success, 200);
            Assert.AreEqual(res.Code, ResponseCodes.Success);
            Assert.AreEqual(res.Cid, 1);
            Assert.AreEqual(res.Address, "/myevent/subscribe");
            Assert.That(res.Data, Does.ContainKey("subscribeid"));
            Assert.AreEqual(res.Data["subscribeid"].ToObject<int>(), 1);
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void Subscribe_ValidRequest_AddedTo_SubscriptionInfo()
        { // pre-condition: assumes getsubscriberlist service tests pass
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/getidentity'], 
                                'subscribeid': 1}
                            }");
            var converter = new JsonConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            var subscriptionInfo = ioTCore.HandleRequest(0, "/getsubscriberlist").Data;
            var subscription_jpath_query = string.Format("$..[?(@.adr == '{0}' && @.subscribeid == {1} && @.callbackurl == '{2}')]",
                "/myevent",
                request.SelectToken("$.data.subscribeid").ToObject<int>(),
                request.SelectToken("$.data.callback").ToObject<string>());
            Assert.NotNull(subscriptionInfo.SelectToken(subscription_jpath_query));
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void SubscriberInternal_ReceivesEvent_OnTrigger_invalidDataToSend()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var triggered = false;
            myevent.Subscribe((e) => { triggered = true; });

            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/nonexistingelement/service'], 
                                'duration': 'uptime', 'uid': 'subscribe1'}
                            }");
            var converter = new JsonConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            triggered = false;
            myevent.RaiseEvent();
            Assert.IsTrue(triggered);
        }

        [Test, Property("TestCaseKey", "IOTCS-T16")]
        public void EventData_ToSubcriberExternal_Contains_srcurl_eventno_payload()
        { // EventData means eventsrc, eventno and payload
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                (sender) => { return 42; },
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, "myevent");
            ioTCore.CreateActionServiceElement(myevent, 
                Identifiers.TriggerEvent, 
                (s, cid) => { myevent.RaiseEvent(); });

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8000/testsubscribe', 
                                'datatosend': ['/data1'], 
                                'duration': 'uptime', 'uid': 'subscribe1'}
                            }");
            var converter = new JsonConverter();
            var subscribeReq = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            // trigger event explicitly 
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8000/");
            var triggerReq = ioTCore.HandleRequest(0, "/myevent/triggerevent"); 
            var eventData = receiver.Do();

            Assert.AreEqual(eventData.SelectToken("$.data.srcurl")?.ToObject<string>(),
                "/myevent");
            Assert.GreaterOrEqual(eventData.SelectToken("$.data.eventno").ToObject<int>(), 0); // skiped value check as event no is dynamic
            Assert.AreEqual(42, eventData.SelectToken("$.data.payload./data1.data")?.ToObject<int>());

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T30")]
        public void SubscriberExternal_ReceivesEventData_OnTrigger()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<string>(ioTCore.Root, 
                "data1",
                (sender) => { return "hellotest"; },
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(42)));
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, "myevent");
            ioTCore.CreateActionServiceElement(myevent, 
                Identifiers.TriggerEvent,
                (_, _) => { myevent.RaiseEvent(); });

            var  clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8000/testsubscribe', 
                                'datatosend': ['/data1'], 
                                'duration': 'uptime', 'uid': 'subscribe1'}
                            }");
            var converter = new JsonConverter();
            var subscribeReq = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            // trigger event explicitly 
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8000/");
            var triggerReq = ioTCore.HandleRequest(0, "/myevent/triggerevent"); 
            var reqcontent = receiver.Do();
            Assert.AreEqual(reqcontent.SelectToken("$../data1.data").ToObject<string>(), "hellotest");

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void SubscribeRequest_uid_generated_if_not_provided()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8000/somepath', 
                                'datatosend': ['/nonexistingelement/service'], 
                                'duration': 'uptime'}
                            }");
            var converter = new JsonConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            Assert.IsNotNull(res.Data.SelectToken("$.subscribeid"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T27")]
        public void Subscribe_InvalidRequests_InvalidAddress_DataFieldMissing()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");

            var subscribeResponse = ioTCore.HandleRequest(0, "/myevent1/subscribe"); // invalid address
            Assert.AreEqual(404, ResponseCodes.NotFound);
            Assert.AreEqual(subscribeResponse.Code, ResponseCodes.NotFound);

            subscribeResponse = ioTCore.HandleRequest(0, "/myevent/subscribe", data: null); // data field is missing
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.AreEqual(ResponseCodes.DataInvalid, subscribeResponse.Code);
        }

        [Test, Property("TestCaseKey", "IOTCS-T29")]
        public void SubscribedEventRemoved_SubscriberListUpdated_SubscriptionsNoMoreServed()
        { // this test assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());

            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                    (sender) => 42 ,
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var subscribeReq = ioTCore.HandleRequest(0, 
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8000/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 1} ")
               );
            var subscribeReq2 = ioTCore.HandleRequest(0, 
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8001/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 2} ")
               );
            var subscriptionsBeforeRemove = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            // check subscriptions were added to subscriberlist
            Assert.AreEqual(subscriptionsBeforeRemove.Data.Count(), 2);
            Assert.AreEqual(subscriptionsBeforeRemove.Data.SelectToken("$[0].subscribeid")?.ToObject<int>(), 1);
            Assert.AreEqual(subscriptionsBeforeRemove.Data.SelectToken("$[1].subscribeid")?.ToObject<int>(), 2);

            // remove subscribed event element
            ioTCore.RemoveElement(ioTCore.Root, myevent);

            // check subscriptions were removed from subscriberlist
            var subscriptionsAfterRemove = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.IsNull(subscriptionsAfterRemove.Data.SelectToken("$..[?(@.subscribeid == 2)]"));
            Assert.IsNull(subscriptionsAfterRemove.Data.SelectToken("$..[?(@.subscribeid == 2)]"));

            // trigger and check if event is not served to removed subscribers
            var triggerReq = ioTCore.HandleRequest(new RequestMessage(1, "/myevent/triggerevent", null));
            Assert.That(triggerReq.Code == ResponseCodes.NotFound); 

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }


        [Test, Property("TestCaseKey", "IOTCS-T59")]
        public void SubscriptionTwice_SameRequest_RejectedSecondTime()
        { // this test assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                    (sender) => { return 42; },
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var singleSubscribeMessage = new RequestMessage(1234, "/myevent/subscribe", JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 1} "));
            var subscribeReq = ioTCore.HandleRequest( singleSubscribeMessage);
            var subscribeRepeat = ioTCore.HandleRequest( singleSubscribeMessage);

            var subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            // check if second subscription was not added in subscriber list
            Assert.AreEqual(subscriptions.Data.Count(), 1);
            Assert.AreEqual(subscriptions.Data.SelectToken("$[0].subscribeid")?.ToObject<int>(), 1);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T59")]
        public void SubscribeRequest_Overwritten_ByKeepingUidSame()
        { // this test assume, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var overwriteSubscribeMsgs = new List<RequestMessage> {
                new RequestMessage(
                    cid: 1,
                    address: "/myevent/subscribe",
                    data: JToken.Parse(@" {
                                    'callback': 'http://127.0.0.1:8050/somepath/', 
                                    'datatosend': ['/data1'], 
                                    'duration': 'uptime', 
                                    'subscribeid': 1}")),
                new RequestMessage(
                    cid: 2,
                    address: "/myevent/subscribe",
                    data: JToken.Parse(@" {
                                    'callback': 'http://127.0.0.1:8050/somepath/', 
                                    'datatosend': ['/data2'], 
                                    'duration': 'uptime', 
                                    'subscribeid': 1}")), 
            };
            Assert.Multiple(() =>
            {
                foreach (var overwriteSubscribeMsg in overwriteSubscribeMsgs)
                {
                    var invalidSubscribeReq = ioTCore.HandleRequest(overwriteSubscribeMsg);
                    Assert.AreEqual(invalidSubscribeReq.Code, ResponseCodes.Success); Assert.AreEqual(ResponseCodes.Success, 200);
                }

            });
            // check if first subscribe (/data1) is overwritten with (/data2) request
            var subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(subscriptions.Data.Count(), 1);
            Assert.AreEqual(subscriptions.Data.SelectToken("$..datatosend"), JToken.Parse(@"['/data2']"));
            Assert.IsNotNull(subscriptions.Data.SelectToken("$..[?(@.datatosend[0] == '/data2')]"));
            Assert.IsNull(subscriptions.Data.SelectToken("$..[?(@.datatosend[1] == '/data1')]"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T28")]
        public void SubscribeRequest_NewCreated_ByNewUid()
        { // this test assumes, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var overwriteSubscribeMsgs = new List<RequestMessage> {
                new RequestMessage(
                    cid: 1,
                    address: "/myevent/subscribe",
                    data: JToken.Parse(@" {
                                    'callback': 'http://127.0.0.1:8050/somepath1/', 
                                    'datatosend': ['/data1'], 
                                    'duration': 'uptime', 
                                    'uid': 'subscribe1'}")),
                new RequestMessage(
                    cid: 2,
                    address: "/myevent/subscribe",
                    data: JToken.Parse(@" {
                                    'callback': 'http://127.0.0.1:8050/somepath1/', 
                                    'datatosend': ['/data1'], 
                                    'duration': 'uptime', 
                                    'uid': 'subscribe2'}")), 
                new RequestMessage(
                    cid: 3,
                    address: "/myevent/subscribe",
                    data: JToken.Parse(@" {
                                    'callback': 'http://127.0.0.1:8050/somepath1/', 
                                    'datatosend': ['/data1'], 
                                    'duration': 'uptime', 
                                    'uid': 'subscribe3'}")),
                new RequestMessage(
                    cid: 4,
                    address: "/myevent/subscribe",
                    data: JToken.Parse(@" {
                                    'callback': 'http://127.0.0.1:8050/somepath1/', 
                                    'datatosend': ['/data1'], 
                                    'duration': 'uptime', 
                                    'uid': 'subscribe4'}")), 
            };
            Assert.Multiple(() =>
            {
                foreach (var overwriteSubscribeMsg in overwriteSubscribeMsgs)
                {
                    var invalidSubscribeReq = ioTCore.HandleRequest(overwriteSubscribeMsg);
                    Assert.AreEqual(invalidSubscribeReq.Code, ResponseCodes.Success); Assert.AreEqual(ResponseCodes.Success, 200);
                }

            });
            // check if first subscribe (/data1) is overwritten with (/data2) request
            var subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(subscriptions.Data.Count(), 4); // 4 subscription requests
        }

        [Test, Property("TestCaseKey", "IOTCS-T57")]
        public void SubscriptionList_Appended_OnNew_SubscribeIDRequest()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                (sender) => { return 42; },
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callback': 'http://127.0.0.1:8050/path1', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 1}
                            }");
            var converter = new JsonConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            var subscriptionlist = ioTCore.HandleRequest(0, "/getsubscriberlist");
            Assert.That(subscriptionlist.Code, Is.EqualTo(200));
            Assert.That(subscriptionlist.Data.Count(), Is.EqualTo(1));
            Assert.IsTrue(subscriptionlist.Data[0]["subscribeid"]?.ToObject<int>().Equals(1));
            Assert.IsTrue(subscriptionlist.Data[0]["callbackurl"]?.ToObject<string>().Equals("http://127.0.0.1:8050/path1"));
            request["data"]["subscribeid"] = 2;
            request["data"]["callback"] = "http://127.0.0.1:8050/path2";
            var res2 = ioTCore.HandleRequest(converter.Deserialize<RequestMessage>(request.ToString()));
            subscriptionlist = ioTCore.HandleRequest(0, "/getsubscriberlist");
            Assert.That(subscriptionlist.Code, Is.EqualTo(200));
            Assert.That(subscriptionlist.Data.Count(), Is.EqualTo(2));
            Assert.IsTrue(subscriptionlist.Data[1]["subscribeid"]?.ToObject<int>().Equals(2));
            Assert.IsTrue(subscriptionlist.Data[1]["callbackurl"]?.ToObject<string>().Equals("http://127.0.0.1:8050/path2"));
        }
    }
}
