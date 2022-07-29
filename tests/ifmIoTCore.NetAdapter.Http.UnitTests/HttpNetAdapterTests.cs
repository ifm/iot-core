using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements.ServiceData.Responses;

namespace ifmIoTCore.NetAdapter.Http.UnitTests
{
    using System.Collections.Generic;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System.Net;
    using System.IO;
    using System;
    using ifmIoTCore.Elements.Valuations;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Threading;

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
    internal class StartNetAdapterServerAndGetResponseHttp
    {
        public Message Do(Message requestMessage, IIoTCore ioTCore, Uri httpServerUri, TextWriter testContextTextWriter)
        {
            IServerNetAdapter httpserver = null;
            IClientNetAdapter httpclient = null;

            try
            {
                httpserver = new HttpServerNetAdapter(ioTCore, httpServerUri, new MessageConverter.Json.Newtonsoft.MessageConverter());
                ioTCore.RegisterServerNetAdapter(httpserver);
                httpclient = new HttpClientNetAdapter(httpServerUri, new MessageConverter.Json.Newtonsoft.MessageConverter(), TimeSpan.FromSeconds(30), false);
            
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

    public class IoTTests_Using_HttpNetAdapter_Client
    {
        [Test, Property("TestCaseKey", "IOTCS-T24")]
        public void DataChangedEvent_IsSentOver_http()
        { // integration test. system reaching external http server / other iot core, here a mock http listener
            using var ioTCore = IoTCoreFactory.Create("myiotcore");
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var data1 = new DataElement<int>(
                "data1",
                (s) => { return 42; },
                null,
                format: new IntegerFormat(new IntegerValuation(0)),
                profiles: new List<string>(), 
                uid: "myuid_data1",
                createDataChangedEventElement: true);

            ioTCore.Root.AddChild(data1);
          
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8052/");
            var datachangeSubscribeResponse = ioTCore.HandleRequest(0, 
                "/data1/datachanged/subscribe",
                new VariantObject
                {
                    { "callback" , new VariantValue("http://127.0.0.1:8052/somepath/")},
                    { "datatosend", new VariantArray(new [] { new VariantValue("/data1") })},
                    { "duration", new VariantValue("uptime")},
                    { "uid", new VariantValue("ExternalSubscriber1")}

                });
            Assert.That(datachangeSubscribeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.AreEqual(200, ResponseCodes.Success);
            // trigger DataChanged Event implicitly by setdata
            data1.Value = new VariantValue(42);
            var reqcontent = receiver.Do();

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();

            Assert.AreEqual(reqcontent.SelectToken("$.code")?.ToObject<int>(), 80); // ensure code is 80 which is event 
            Assert.AreEqual(reqcontent.SelectToken("$.data.srcurl")?.ToObject<string>(), "myiotcore/data1/datachanged"); // check srcurl is treechanged
            Assert.AreEqual(reqcontent.SelectToken("$.data.payload./data1.data")?.ToObject<int>(),42); // check datatosend delivered
        }

        [Test, Property("TestCaseKey", "IOTCS-T19")]
        public void TreeChangedEvent_Message_SentOver_Http()
        { // Integration Test connecting system with external ioTCore/http application, here mocked by a simple http listener
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var de = new ReadOnlyDataElement<int>("data1", (s) => { return 42; }, format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.Root.AddChild(de);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8053/");
            var treechangeSubscribeResponse = ioTCore.HandleRequest(0,
                "/treechanged/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8053/somepath/")},
                    { "datatosend", new VariantArray(new [] { new VariantValue("/data1") })},
                    { "duration", new VariantValue("uptime")},
                    { "uid", new VariantValue("ExternalSubscriber1")}
                });

            Assert.NotNull(treechangeSubscribeResponse, "Got no response for the /treechanged/subscribe request.");
            Assert.That(treechangeSubscribeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.AreEqual(200, ResponseCodes.Success);
            // trigger TreeChanged Event implicitly by adding element
            var s = new StructureElement("struct1");
            ioTCore.Root.AddChild(s, true);
            var reqcontent = receiver.Do(10000);

            Assert.NotNull(reqcontent, "Got no response on http://127.0.0.1:8053/");
            Assert.AreEqual(reqcontent.SelectToken("$.code")?.ToObject<int>(), 80); // ensure code is 80 which is event 
            Assert.AreEqual(reqcontent.SelectToken("$.data.srcurl")?.ToObject<string>(), "ioTCore/treechanged"); // check srcurl is treechanged
            Assert.AreEqual(reqcontent.SelectToken("$.data.payload./data1.data")?.ToObject<int>(), 42); // check datatosend delivered

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T16")]
        public void EventData_ToSubcriberExternal_Contains_srcurl_eventno_payload()
        { // EventData means eventsrc, eventno and payload
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var data1 = new ReadOnlyDataElement<int>("data1",
                (sender) => { return 42; },
                format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.Root.AddChild(data1);

            var myevent = new EventElement("myevent");
            ioTCore.Root.AddChild(myevent);

            var a = new ActionServiceElement(
                Identifiers.TriggerEvent, 
                (s, cid) => { myevent.RaiseEvent(); });

            myevent.AddChild(a);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
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

            var converter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            var subscribeReq = ioTCore.HandleRequest(converter.Deserialize(request.ToString()));
            // trigger event explicitly 
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8000/");
            var triggerReq = ioTCore.HandleRequest(0, "/myevent/triggerevent"); 
            var eventData = receiver.Do();

            Assert.AreEqual(eventData.SelectToken("$.data.srcurl")?.ToObject<string>(),
                "ioTCore/myevent");
            Assert.GreaterOrEqual(eventData.SelectToken("$.data.eventno").ToObject<int>(), 0); // skiped value check as event no is dynamic
            Assert.AreEqual(42, eventData.SelectToken("$.data.payload./data1.data")?.ToObject<int>());

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T30")]
        public void SubscriberExternal_ReceivesEventData_OnTrigger()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var data1 = new ReadOnlyDataElement<string>("data1",
                _ => { return "hellotest"; },
                format: new IntegerFormat(new IntegerValuation(42)));

            ioTCore.Root.AddChild(data1);

            var myevent = new EventElement("myevent");
            ioTCore.Root.AddChild(myevent);

            var a = new ActionServiceElement(Identifiers.TriggerEvent, 
                (_, _) => { myevent.RaiseEvent(); });

            myevent.AddChild(a);

            var  clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
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
            var converter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            var subscribeReq = ioTCore.HandleRequest(converter.Deserialize(request.ToString()));
            // trigger event explicitly 
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8000/");
            var triggerReq = ioTCore.HandleRequest(0, "/myevent/triggerevent"); 
            var reqcontent = receiver.Do();
            Assert.AreEqual(reqcontent.SelectToken("$../data1.data").ToObject<string>(), "hellotest");

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }


        [Test, Property("TestCaseKey", "IOTCS-T33")]
        public void SubscriptionRemoved_IsNotServedFurther_RemovedFromSubscriptionList()
        { // this test for unsubscribe assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);

            var data1 = new ReadOnlyDataElement<int>("data1", 
                _ => 42, 
                format: new IntegerFormat(new IntegerValuation(0)));

            ioTCore.Root.AddChild(data1);

            var myevent = new EventElement("myevent");
            ioTCore.Root.AddChild(myevent);

            var a = new ActionServiceElement(Identifiers.TriggerEvent, (_, _) => { myevent.RaiseEvent(); });
            myevent.AddChild(a);

            var subscribeReq = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:9050/somepath/") }, 
                    { "datatosend", new VariantArray
                        { new VariantValue("/data1") } },
                    { "duration", new VariantValue("uptime") },
                    { "subscribeid", new VariantValue(1) },
                }
               );

            var subscribeReq2 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:9051/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "duration", new VariantValue("uptime") }, 
                    { "subscribeid", new VariantValue(2) },
                }
               );
            // check subscriptions were appended
            var subscriptionResponse = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));

            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionResponse.Data);

            Assert.AreEqual(subscriptions.Count(), 2);

            Assert.AreEqual(subscriptions.ToArray()[0].SubscriptionId, 1);
            Assert.AreEqual(subscriptions.ToArray()[1].SubscriptionId, 2);
            // trigger event to check ifoth subscriptions are delivered event info
            var receiver1 = new ReceiveIoTMsg("http://127.0.0.1:9050/somepath/");
            var receiver2 = new ReceiveIoTMsg("http://127.0.0.1:9051/somepath/");
            var triggerReq = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/myevent/triggerevent", null));
            var eventMsg1 = receiver1.Do();
            var eventMsg2 = receiver2.Do();
            Assert.AreEqual(eventMsg1.SelectToken("$../data1.data")?.ToObject<int>(), 42);
            Assert.AreEqual(eventMsg2.SelectToken("$../data1.data")?.ToObject<int>(), 42);

            // unsubscribe first subscriber using callback
            var unsubscribeResponse = ioTCore.HandleRequest(1, "/myevent/unsubscribe",
                new VariantObject() { { "callback", new VariantValue("http://127.0.0.1:9050/somepath/") }, });
            Assert.AreEqual(200, unsubscribeResponse.Code); Assert.AreEqual(ResponseCodes.Success, 200);  
            // check entry removed from subscription list
            var subscriptionsResponse = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));

            subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionsResponse.Data);

            Assert.AreEqual(1, subscriptions.Count()); // 1 subscriptions removed of 2  = remaining 1
            Assert.That(subscriptions.ToArray().Any(x => x.SubscriptionId == 2));
            Assert.AreEqual(2, subscriptions.ToArray()[0].SubscriptionId);
            Assert.That(subscriptions.All(x => x.SubscriptionId != 1));
            
            var receiver3 = new ReceiveIoTMsg("http://127.0.0.1:9051/somepath/");
            var receiver4 = new ReceiveIoTMsg("http://127.0.0.1:9050/somepath/");
            // trigger and check if event is not served to removed subscriber
            triggerReq = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/myevent/triggerevent", null));
            // check available subscriber receives event
            eventMsg2 = receiver3.Do();
            Assert.AreEqual(eventMsg2.SelectToken("$../data1.data")?.ToObject<int>(), 42);
            // check removed subscriber does not receive event
            eventMsg1 = receiver4.Do(1000);
            Assert.IsNull(eventMsg1);

            // unsubscribe second subscriber using uid
            var unsubscribeResponse2 = ioTCore.HandleRequest(2, "/myevent/unsubscribe",
                new VariantObject() { { "callback", new VariantValue("http://127.0.0.1:9051/somepath/") }, });
            Assert.AreEqual(200, unsubscribeResponse.Code); Assert.AreEqual(ResponseCodes.Success, 200);
            // check remaining entry removed from subscription list
            subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);

            Assert.That(subscriptions.All(x=> x.SubscriptionId != 2));
            // trigger and check if event is not served to removed subscriber
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:9051/somepath/");
            triggerReq = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/myevent/triggerevent", null));
            // check removed subscriber does not receive event
            eventMsg2 = receiver.Do();
            Assert.IsNull(eventMsg2);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T33")]
        public void Unsubscribe_RemovesSubscriptions_CallbackRemovesMultiple_WithUidRemovesSpecific()
        { // this test for unsubscribe assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var data1 = new ReadOnlyDataElement<int>("data1",
                    _ => 42,
                    format: new IntegerFormat(new IntegerValuation(0)));

            ioTCore.Root.AddChild(data1);

            var e = new EventElement(identifier: "myevent");
            ioTCore.Root.AddChild(e);


            var subscribeReq = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8050/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(1) },
                }
               );
            var subscribeReq2 = ioTCore.HandleRequest(2,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8050/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } },
                }
               );
            var subscribeReq3 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8051/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(3) },
                }
               );
            var subscribeReq4 = ioTCore.HandleRequest(4,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8051/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } },
                }
               );

            // check subscriptions were appended
            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.AreEqual(4, subscriptions.Count()); 

            // unsubscribe subscriber(s) using cid
            var unsubscribeResponse = ioTCore.HandleRequest(1,
                "/myevent/unsubscribe",
                new VariantObject() { { "callback", new VariantValue("http://127.0.0.1:8050/somepath/") }, }
                );
            Assert.AreEqual(200, unsubscribeResponse.Code); Assert.AreEqual(ResponseCodes.Success, 200);
            subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.AreEqual(3, subscriptions.Count()); // 2 subscriptions removed of 4  = remaining 2

            // unsubscribe single subscriber entry using subscribeid
            // now try uid with callback
            var unsubscribeResponse2 = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                new VariantObject() { { "callback", new VariantValue("http://127.0.0.1:8051/somepath/") }, { "subscribeid", new VariantValue(2) }, });
            Assert.AreEqual(200, unsubscribeResponse2.Code); Assert.AreEqual(ResponseCodes.Success, 200);
            subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.AreEqual(2, subscriptions.Count()); // 1 subscriptions removed of 2  = remaining 1

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
        }

        [Test, Property("TestCaseKey", "IOTCS-T34")]
        public void Unsubscribe_InvalidRequests_RejectedWithLogs()
        { // this test for unsubscribe assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());

            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var data1 = new ReadOnlyDataElement<int>("data1", _ => 42, format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.Root.AddChild(data1);

            var e = new EventElement("myevent");
            ioTCore.Root.AddChild(e);

            var subscribeReq = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8050/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(1) },
                }
               );
            var subscribeReq2 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8050/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } },
                }
               );
            var subscribeReq3 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8051/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(2) },
                }
               );

            Assert.AreEqual(Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 4, "/getsubscriberlist", null)).Data).Count(), 3);

            // invalid unsubscribe requests
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.AreEqual(404, ResponseCodes.NotFound);

            var incorrect_mandatory_callback = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                new VariantObject() {{"callback1",new VariantValue("http://127.0.0.1:8050/somepath/")},}
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, incorrect_mandatory_callback.Code, "incorrect_mandatory_callback");

            var uid_without_mandatory_callback = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                new VariantObject() { { "uid", new VariantValue("http://127.0.0.1:8050/somepath/") }, }
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, uid_without_mandatory_callback.Code, "uid_without_mandatory_callback expected 530 data invalid");

            var invalid_event_address = ioTCore.HandleRequest(0,
                "/myevent1/unsubscribe",
                new VariantObject() {{ "callback", new VariantValue("http://127.0.0.1:8050/somepath/") }}
            );
            Assert.AreEqual(ResponseCodes.NotFound, invalid_event_address.Code, "invalid_event_address");

            var mismatching_callback_uid = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",

                new VariantObject()
                {
                    { "callback" , new VariantValue("http://127.0.0.1:8051/somepath/") },
                    { "subscribeid", new VariantValue(1)}
                }
            );
            Assert.AreEqual(ResponseCodes.Success, mismatching_callback_uid.Code, "mismatching_callback_uid should be 200 OK without warning");

            var non_existing_callback = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",

                new VariantObject()
                {
                    { "callback" ,  new VariantValue("http://non.existing.path/somepath/")}
                }
            );
            Assert.AreEqual(ResponseCodes.DataInvalid, non_existing_callback.Code, "non_existing_callback should be 530");

            var non_existing_uid = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",

                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:8050/somepath/") },
                    { "subscribeid", new VariantValue(12345)}
                }
            );
            Assert.AreEqual(ResponseCodes.DataInvalid, non_existing_uid.Code, "non_existing_uid should be 530");

            var validUnsubscribe = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                new VariantObject()
                {
                    {"callback", new VariantValue("http://127.0.0.1:8050/somepath/")}
                });
            Assert.AreEqual(ResponseCodes.Success, validUnsubscribe.Code, "Expected valid unsubscribe result");
            var UnsubscribeTwice = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                new VariantObject()
                {
                    { "callback" , new VariantValue("http://127.0.0.1:8050/somepath/")}
                }
            );
            Assert.AreEqual(ResponseCodes.DataInvalid, UnsubscribeTwice.Code, "UnsubscribeTwice should be 530");

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T61")]
        public void Events_Triggered_AllTogether_10()
        { // integration test: trigger works if event data reaches subscribers
            const int MaxEvents = 10;
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var data1 = new ReadOnlyDataElement<int>("data1", _ => 42, format: new IntegerFormat(new IntegerValuation(0)));

            ioTCore.Root.AddChild(data1);

            var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter(), TimeSpan.FromSeconds(10), false);

            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);

            var eventAddresses = new List<string>();
            var triggerAddresses = new List<string>();
            var subscriberUrls = new List<string>();
            var subscriberDataList = new List<Variant>();
            for (var eventno = 0; eventno < MaxEvents; eventno++)
            {
                var eventId = string.Format("myevent{0}", eventno);
                eventAddresses.Add(ioTCore.Root.Identifier + "/" + eventId);
                var myevent = new EventElement(eventId);
                ioTCore.Root.AddChild(myevent);

                var a = new ActionServiceElement(Identifiers.TriggerEvent, (s, cid) => { myevent.RaiseEvent(); });
                myevent.AddChild(a);

                triggerAddresses.Add(string.Format("/{0}/triggerevent",eventId));
                var subscriberData = new VariantObject() { { "callback", new VariantValue("NotInitialisedYet") }, { "datatosend", new VariantArray() { new VariantValue("/data1") } }, { "duration", new VariantValue("uptime") }, };
                var host = $"http://127.0.0.1:{10006 + eventno}/";
                var url = string.Format("{0}somepath/",host);
                //var url = string.Format("http://127.0.0.1:{0}/somepath/", 10006); // single subscriber
                subscriberData["callback"] = new VariantValue(host);
                subscriberData["uid"] = new VariantValue("subscribe_" + eventId.ToString());
                subscriberUrls.Add(host);
                subscriberDataList.Add(subscriberData);
                var subscribeResult = ioTCore.HandleRequest(0, string.Format("/{0}/subscribe",eventId), subscriberData);
                Assert.AreEqual(ResponseCodes.Success, subscribeResult.Code);
            }
            var subscribers = ioTCore.HandleRequest(0, "/getsubscriberlist");
            Assert.AreEqual(ResponseCodes.Success, subscribers.Code);

            Assert.AreEqual(MaxEvents, Variant.ToObject<GetSubscriberListResponseServiceData>(subscribers.Data).Count());

            var EventsReceived = new BlockingCollection<JToken>();
            var IoTMsgReceivers = new List<Thread>();
            for (var eventno = 0; eventno < MaxEvents; eventno++)
            {
                var saved_eventno = eventno;
                var IotReceiver = new Thread(() =>
                {
                    try
                    {
                        var receiver = new ReceiveIoTMsg(subscriberUrls[saved_eventno]);
                        var response = receiver.Do(60000);
                        if (response == null) throw new Exception("No response received.");
                        EventsReceived.Add(response);
                    }
                    catch (Exception e)
                    {
                        TestContext.Out.WriteLine($"Error occured in Events_Triggered_AllTogether_10 receiver thread. Message: {e.Message}");
                    }
                });

                IoTMsgReceivers.Add(IotReceiver);
                IotReceiver.Start();
            }

            var triggerthreads = new List<Thread>();

            // trigger externally - through Command Interface 
            for (var eventno = 0; eventno < MaxEvents; eventno++)
            {
                var saved_eventno = eventno;
                var thread = new Thread(() =>
                {
                    try
                    {
                        var triggerEventString = triggerAddresses[saved_eventno];
                        var triggerResult = ioTCore.HandleRequest(0, triggerEventString);
                        Assert.AreEqual(ResponseCodes.Success, triggerResult.Code);
                    }
                    catch (Exception e)
                    {
                        TestContext.Out.WriteLine($"Error occured in Events_Triggered_AllTogether_10 trigger thread. Message: {e.Message}");
                    }
                });

                triggerthreads.Add(thread);
                thread.Start();
            }

            // Receive events all at once in multiple http listeners

            foreach (var item in triggerthreads)
            {
                item.Join();
            }

            foreach (var IotReceiver in IoTMsgReceivers)
            {
                IotReceiver.Join();  // wait till all finish
            }

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();

            // check no. of events and event data is as expected
            Assert.AreEqual(MaxEvents, EventsReceived.Count(), string.Format("expected to receive {0} IoT Messages",MaxEvents));
            Assert.Multiple(() =>
            {// check all messages have expected event data
                foreach (var eventData in EventsReceived.ToArray().Select(x=>
                {
                    var item = EventsReceived;
                    return x;
                }))
                {
                    Assert.IsNotNull(eventData);
                    Assert.That(eventAddresses, Does.Contain(eventData.SelectToken("$.data.srcurl")?.ToObject<string>()));
                    //Assert.AreEqual(eventData.SelectToken("$.data.srcurl")?.ToObject<string>(), eventAddresses[eventno]);
                    Assert.GreaterOrEqual(eventData.SelectToken("$.data.eventno").ToObject<int>(), 0); // skipped checking dynamic event no.
                    Assert.AreEqual(42, eventData.SelectToken("$.data.payload./data1.data")?.ToObject<int>());
                }
            });
            
        }

    }

    public class IoTTests_Using_HttpNetAdapter_Server
    {
        [Test, Property("TestCaseKey", "IOTCS-T69")]
        public void getdatamulti_Service_AccessibleThroughNetAdapter_http()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var int1 = new ReadOnlyDataElement<int>("int1", _ => 42);

            ioTCore.Root.AddChild(int1);

            Assert.AreEqual(200, ResponseCodes.Success);
            var getdatamultiResponse = new StartNetAdapterServerAndGetResponseHttp().Do( 
                new Message(RequestCodes.Request, 1, "/getdatamulti", new VariantObject() { { "datatosend", new VariantArray() { new VariantValue("/int1") } }, }),
                ioTCore,
                new Uri("http://127.0.0.1:10004"), TestContext.Out);
            Assert.AreEqual(ResponseCodes.Success, getdatamultiResponse.Code);

            var responseData = Variant.ToObject<GetDataMultiResponseServiceData>(getdatamultiResponse.Data);

            Assert.That(responseData.Any(x => x.Key == "/int1"));

            Assert.AreEqual(200, responseData["/int1"].Code);
            Assert.AreEqual(42, (int)(VariantValue)responseData["/int1"].Data);
        }

        [Test, Property("TestCaseKey", "IOTCS-T70")]
        public void setdatamulti_service_AccessibleThroughNetAdapter_http()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            object setDataValue = null;
            var int1 = new DataElement<long>("int1",
                    _ => 42, 
                    (_, incomingJson) => 
                    { 
                        setDataValue = incomingJson; 
                    },
                    format: new IntegerFormat(new IntegerValuation(0)));

            ioTCore.Root.AddChild(int1);

            var setdatamultiResponse = new StartNetAdapterServerAndGetResponseHttp().Do( 
                new Message(RequestCodes.Request, 1, "/setdatamulti", new VariantObject() { { "datatosend", new VariantObject() { { "/int1", new VariantValue(43) }, } }, }),
                ioTCore,
                new Uri("http://127.0.0.1:10005"), TestContext.Out);
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.That(setdatamultiResponse.Code, Is.EqualTo(ResponseCodes.Success));
            Assert.AreEqual(43, setDataValue);
        }

        [Test, Property("TestCaseKey", "IOTCS-T40")]
        public void SubscriberListService_accessibleThrough_http()
        { // integration test: this test for subscriberlist assumes, subscribe, trigger event works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var data1 = new ReadOnlyDataElement<int>("data1", (sender) => { return 42; }, 
                    format: new IntegerFormat(new IntegerValuation(0)));
            ioTCore.Root.AddChild(data1);

            var eventIDs = new List<string> { "myevent", "myevent2", "myevent3", "myevent4", "myevent5" };
            for (int id = 0; id < eventIDs.Count; id++)
            {
                var myevent = new EventElement(eventIDs[id]);
                ioTCore.Root.AddChild(myevent);

                var svcaddr = string.Format("/{0}/subscribe", myevent.Identifier);
                var data = new VariantObject()
                {
                    { "callback", new VariantValue("callback/not/considered/on/subscribe") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(0) },
                };
                data["subscribeid"] = new VariantValue(id);

                var subscribeReq = ioTCore.HandleRequest(0,
                    svcaddr,
                    data: data
                    );
            }

            var subscriptionsResponse = new StartNetAdapterServerAndGetResponseHttp().Do(
                new Message(RequestCodes.Request, 1, "/getsubscriberlist", null),
                ioTCore,
                new Uri("http://127.0.0.1:10001"), TestContext.Out);
            // check subscriptions were appended

            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionsResponse.Data);
            Assert.AreEqual(5, subscriptions.Count(), "expected 5 subscriptions");
            Assert.AreEqual(0,subscriptions.ToArray()[0].SubscriptionId);
            Assert.AreEqual(1, subscriptions.ToArray()[1].SubscriptionId);
            Assert.AreEqual(2, subscriptions.ToArray()[2].SubscriptionId);
            Assert.AreEqual(3, subscriptions.ToArray()[3].SubscriptionId);
            Assert.AreEqual(4, subscriptions.ToArray()[4].SubscriptionId);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T37")]
        public void TriggerService_AccessibleThroughNetAdapter_Http()
        { // integration test: trigger works if event data reaches subscribers
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var myevent = new EventElement("myevent");
            ioTCore.Root.AddChild(myevent);
            
            var a = new ActionServiceElement(Identifiers.TriggerEvent, (s, cid) => { myevent.RaiseEvent(); });

            myevent.AddChild(a);

            var data1 = new ReadOnlyDataElement<int>("data1",
                _ => 42,
                format: new IntegerFormat(new IntegerValuation(0))) ;

            ioTCore.Root.AddChild(data1);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter.Json.Newtonsoft.MessageConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var subscribeResult = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callback", new VariantValue("http://127.0.0.1:10002/somepath") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "duration", new VariantValue("uptime") }, 
                    { "uid", new VariantValue("subscribe1") },
                });
            Assert.AreEqual(ResponseCodes.Success, subscribeResult.Code);

            // receive subscribe through http net adapter (client)
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:10002/somepath/");

            // trigger externally - through http net adapter (server)
            var triggerResult = new StartNetAdapterServerAndGetResponseHttp().Do(
                new Message(RequestCodes.Request, 2, "/myevent/triggerevent", null),
                ioTCore,
                new Uri("http://127.0.0.1:10003"), TestContext.Out);

            if (triggerResult == null)
            {
                throw new Exception("Triggerresult is null.");
            }

            Assert.AreEqual(ResponseCodes.Success, triggerResult.Code);

            var eventData = receiver.Do();

            if (eventData == null)
            {
                throw new Exception("eventdata is null");
            }


            Assert.AreEqual(eventData.SelectToken("$.data.srcurl")?.ToObject<string>(),
                "ioTCore/myevent");
            Assert.GreaterOrEqual(eventData.SelectToken("$.data.eventno").ToObject<int>(), 0); // skipped checking dynamic event no.
            Assert.AreEqual(42, eventData.SelectToken("$.data.payload./data1.data")?.ToObject<int>());

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }
    }


}