namespace ifm.IoTCore.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using ElementManager.Contracts.Elements.Formats;
    using ElementManager.Contracts.Elements.ServiceData.Requests;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ifm.Common.Variant;
    using Message;
    using MessageConverter.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public class TestSubscribeMessages
    {
        public static IEnumerable SameData_AlternativeNames
        {
            get
            {
                yield return new TestCaseData(
                    new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject {
                        {"callback", new VariantValue("http://127.0.0.1:8000/mydevice/someeventhandler")},
                        {"datatosend", new VariantArray {new VariantValue("/getidentity")}},
                        {"subscribeid", new VariantValue(1)},})).SetName("{m}_mandatoryfield_callback"); 

                yield return new TestCaseData(
                    new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject {
                        {"callbackurl", new VariantValue("http://127.0.0.1:8000/mydevice/someeventhandler")},
                        {"datatosend", new VariantArray {new VariantValue("/getidentity")}},
                        {"subscribeid", new VariantValue(1)},})).SetName("{m}_AlternativeName_callbackurl"); 
            }
        }
    }

    [TestFixture]
    public class Event_Subscribe_Tests
    {
        [Test]
        public void CheckSuccessResponseCode()
        {
            Assert.That(ResponseCodes.Success, Is.EqualTo(200));
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void Subscribe_ValidRequest_AcknowledgedWith_uid_Response()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callbackurl': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/getidentity'], 
                                'subscribeid': 1}
                            }");
            var converter = new MessageConverter();
            var res = ioTCore.MessageDispatcher.HandleRequest(converter.Deserialize(request.ToString()));
            
            Assert.That(res.Code, Is.EqualTo(ResponseCodes.Success));
            Assert.That(res.Cid, Is.EqualTo(1));
            Assert.That(res.Address, Is.EqualTo("/myevent/subscribe"));

            var subscribeResponseData = Variant.ToObject<SubscribeResponseServiceData>(res.Data);

            Assert.That(subscribeResponseData.SubscriptionId,Is.Not.Null);
            Assert.That(subscribeResponseData.SubscriptionId,Is.EqualTo(1));
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        [TestCaseSource(typeof(TestSubscribeMessages), nameof(TestSubscribeMessages.SameData_AlternativeNames))]
        public void Subscribe_ValidRequest_AddedTo_SubscriptionInfo(Message requestMessage)
        { // pre-condition: assumes getsubscriberlist service tests pass
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");

            var converter= new MessageConverter();
            var res = ioTCore.MessageDispatcher.HandleRequest(requestMessage);
            var subscriptionInfo = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.MessageDispatcher.HandleRequest(0, "/getsubscriberlist").Data);
            
            Assert.That(subscriptionInfo.Any(x=>x.Address == "ioTCore/myevent" && 
                                                x.SubscriptionId == 1 && 
                                                x.CallbackUrl == "http://127.0.0.1:8000/mydevice/someeventhandler"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void SubscriberInternal_ReceivesEvent_OnTrigger_invalidDataToSend()
        { 
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");
            var triggered = false;
            myevent.EventRaised += (sender, args) => triggered = true;
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callbackurl': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/nonexistingelement/service'], 
                                'duration': 'uptime', 'uid': 'subscribe1'}
                            }");
            var converter= new MessageConverter();
            var res = ioTCore.MessageDispatcher.HandleRequest(converter.Deserialize(request.ToString()));
            triggered = false;
            myevent.Raise();
            Assert.That(triggered);
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void SubscribeRequest_uid_generated_if_not_provided()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");

            var requestMessage = new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject
            {
                {"callbackurl", new VariantValue("http://127.0.0.1:8000/somepath")},
                {"datatosend", new VariantArray {new VariantValue("/nonexistingelement/service")}},
                {"duration", new VariantValue("uptime")},
            });

            var converter= new MessageConverter();
            var res = ioTCore.MessageDispatcher.HandleRequest(requestMessage);
            Assert.That(Variant.ToObject<SubscribeResponseServiceData>(res.Data).SubscriptionId,Is.Not.Null);
            Assert.That(Variant.ToObject<SubscribeResponseServiceData>(res.Data).SubscriptionId, Is.Not.Zero);
        }

        [Test, Property("TestCaseKey", "IOTCS-T27")]
        public void Subscribe_InvalidRequests_InvalidAddress_DataFieldMissing()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");

            var subscribeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/myevent1/subscribe"); // invalid address
            Assert.That(ResponseCodes.NotFound, Is.EqualTo(404));
            Assert.That(subscribeResponse.Code, Is.EqualTo(ResponseCodes.NotFound));

            subscribeResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/myevent/subscribe", data: null); // profileData field is missing
            Assert.That(ResponseCodes.DataInvalid, Is.EqualTo(422));
            Assert.That(subscribeResponse.Code, Is.EqualTo(ResponseCodes.DataInvalid));
        }

        [Test, Property("TestCaseKey", "IOTCS-T29")]
        public void SubscribedEventRemoved_SubscriberListUpdated()
        { // this test assumes, subscribe, getsubscriberlist works.
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data1", 42);

            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");
            var subscribeReq = ioTCore.MessageDispatcher.HandleRequest(0, 
                "/myevent/subscribe",
                new VariantObject
                {
                    { "callbackurl", new VariantValue("http://127.0.0.1:8000/somepath/") }, 
                    { "datatosend", new VariantArray { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(1) },
                }
               );
            var subscribeReq2 = ioTCore.MessageDispatcher.HandleRequest(0, 
                "/myevent/subscribe",
                new VariantObject
                {
                    { "callbackurl", new VariantValue("http://127.0.0.1:8001/somepath/") }, 
                    { "datatosend", new VariantArray { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(2) },
                }
               );
            var subscriptionsBeforeRemove = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));

            var subscriptionsBeforeRemoveData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionsBeforeRemove.Data);
            
            // check subscriptions were added to subscriberlist
            Assert.That(subscriptionsBeforeRemoveData.Count(), Is.EqualTo(2));

            Assert.That(subscriptionsBeforeRemoveData[0].SubscriptionId, Is.EqualTo(1));
            Assert.That(subscriptionsBeforeRemoveData[1].SubscriptionId, Is.EqualTo(2));

            // remove subscribed event element
            ioTCore.ElementManager.RemoveElement(ioTCore.Root, myevent);

            // check subscriptions were removed from subscriberlist
            var subscriptionsAfterRemove = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));
            var subscriptionsAfterRemoveData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionsAfterRemove.Data);

            Assert.That(subscriptionsAfterRemoveData.All(x => x.SubscriptionId != 2));

            // trigger and check if event is not served to removed subscribers
            var triggerReq = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/myevent/triggerevent", null));
            Assert.That(triggerReq.Code == (int)ResponseCodes.NotFound);
        }

        [Test, Property("TestCaseKey", "IOTCS-T59")]
        public void SubscriptionTwice_SameRequest_RejectedSecondTime()
        { // this test assumes, subscribe, getsubscriberlist works.
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data1", 42, format: new Int32Format());

            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");

            var singleSubscribeMessage = new Message(RequestCodes.Request, 1234, "/myevent/subscribe", new VariantObject
            {
                { "callbackurl", new VariantValue("http://127.0.0.1:8050/somepath/") },
                { "datatosend", new VariantArray { new VariantValue("/data1") } },
                { "subscribeid", new VariantValue(1) },
            });
            var subscribeReq = ioTCore.MessageDispatcher.HandleRequest( singleSubscribeMessage);
            var subscribeRepeat = ioTCore.MessageDispatcher.HandleRequest( singleSubscribeMessage);

            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            // check if second subscription was not added in subscriber list
            Assert.That(subscriptions.Count(), Is.EqualTo(1));
            Assert.That(subscriptions[0].SubscriptionId, Is.EqualTo(1));
        }

        [Test, Property("TestCaseKey", "IOTCS-T59")]
        public void SubscribeRequest_Overwritten_ByKeepingUidSame()
        { // this test assume, getsubscriberlist works.
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");
            var overwriteSubscribeMsgs = new List<Message> {
                new Message(RequestCodes.Request,
                    cid: 1,
                    address: "/myevent/subscribe",
                    data: new VariantObject
                    {
                        {"callbackurl",new VariantValue("http://127.0.0.1:8050/somepath/")},
                        {"datatosend",new VariantArray {new VariantValue("/data1")}},
                        {"duration",new VariantValue("uptime")},
                        {"subscribeid",new VariantValue(1)},
                    }),
                new Message(RequestCodes.Request,
                    cid: 2,
                    address: "/myevent/subscribe",
                    data: new VariantObject
                    {
                        {"callbackurl",new VariantValue("http://127.0.0.1:8050/somepath/")},
                        {"datatosend",new VariantArray {new VariantValue("/data2")}},
                        {"duration",new VariantValue("uptime")},
                        {"subscribeid",new VariantValue(1)},
                    }), 
            };
            Assert.Multiple(() =>
            {
                foreach (var overwriteSubscribeMsg in overwriteSubscribeMsgs)
                {
                    var invalidSubscribeReq = ioTCore.MessageDispatcher.HandleRequest(overwriteSubscribeMsg);
                    Assert.That(invalidSubscribeReq.Code, Is.EqualTo(ResponseCodes.Success));
                    Assert.That(ResponseCodes.Success, Is.EqualTo(200));
                }

            });
            // check if first subscribe (/data1) is overwritten with (/data2) request
            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.That(subscriptions.Count(), Is.EqualTo(1));

            Assert.That(subscriptions.Any(x => x.DataToSend.Contains("/data2")));
            Assert.That(subscriptions[0].DataToSend.Contains("/data2"));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Assert.That(subscriptions[1].DataToSend.SingleOrDefault(x => x.Equals("/data1")), Is.Null);
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T28")]
        public void SubscribeRequest_NewCreated_ByNewUid()
        {
            // this test assumes, getsubscriberlist works.
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");
            var overwriteSubscribeMsgs = new List<Message> {
                new Message(RequestCodes.Request,
                    cid: 1,
                    address: "/myevent/subscribe",
                    data: new VariantObject
                    {
                        {"callbackurl",new VariantValue("http://127.0.0.1:8050/somepath1/")},
                        {"datatosend",new VariantArray {new VariantValue("/data1")}},
                        {"duration",new VariantValue("uptime")},
                        {"uid",new VariantValue("subscribe1")}
                    }),
                new Message(RequestCodes.Request,
                    cid: 2,
                    address: "/myevent/subscribe",
                    data: new VariantObject
                    {
                        { "callbackurl", new VariantValue("http://127.0.0.1:8050/somepath1/") }, 
                        { "datatosend", new VariantArray { new VariantValue("/data1") } }, 
                        { "duration", new VariantValue("uptime") }, 
                        { "uid", new VariantValue("subscribe2") },
                    }), 
                new Message(RequestCodes.Request,
                    cid: 3,
                    address: "/myevent/subscribe",
                    data: new VariantObject
                    {
                        { "callbackurl", new VariantValue("http://127.0.0.1:8050/somepath1/") }, 
                        { "datatosend", new VariantArray { new VariantValue("/data1") } }, 
                        { "duration", new VariantValue("uptime") }, 
                        { "uid", new VariantValue("subscribe3") },
                    }),
                new Message(RequestCodes.Request,
                    cid: 4,
                    address: "/myevent/subscribe",
                    data: new VariantObject
                    {
                        { "callback", new VariantValue("http://127.0.0.1:8050/somepath1/") }, 
                        { "datatosend", new VariantArray { new VariantValue("/data1") } }, 
                        { "duration", new VariantValue("uptime") }, 
                        { "uid", new VariantValue("subscribe4") },
                    })};

            Assert.Multiple(() =>
            {
                foreach (var overwriteSubscribeMsg in overwriteSubscribeMsgs)
                {
                    var invalidSubscribeReq = ioTCore.MessageDispatcher.HandleRequest(overwriteSubscribeMsg);
                    Assert.That(invalidSubscribeReq.Code, Is.EqualTo(ResponseCodes.Success));
                    Assert.That(ResponseCodes.Success, Is.EqualTo(200));
                }

            });
            // check if first subscribe (/data1) is overwritten with (/data2) request
            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.That(subscriptions.Count(), Is.EqualTo(4)); // 4 subscription requests
        }

        [Test, Property("TestCaseKey", "IOTCS-T57")]
        public void SubscriptionList_Appended_OnNew_SubscribeIDRequest()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data1", 42, format: new Int32Format());

            var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "myevent");

            var requestMessage = new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject
            {
                {"callbackurl", new VariantValue("http://127.0.0.1:8050/path1")},
                {"datatosend", new VariantArray {new VariantValue("/data1")}},
                {"subscribeid", new VariantValue(1)},
            });

            var converter= new MessageConverter();
            var res = ioTCore.MessageDispatcher.HandleRequest(requestMessage);
            var subscriptionlist = ioTCore.MessageDispatcher.HandleRequest(0, "/getsubscriberlist");
            Assert.That(subscriptionlist.Code, Is.EqualTo(200));
            
            var subscriptionlistData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionlist.Data);
            Assert.That(subscriptionlistData.Count(), Is.EqualTo(1));

            Assert.That(subscriptionlistData[0].SubscriptionId, Is.EqualTo(1));
            Assert.That(subscriptionlistData[0].CallbackUrl, Is.EqualTo("http://127.0.0.1:8050/path1"));


            var requestMessage2 = new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject
            {
                {"callbackurl", new VariantValue("http://127.0.0.1:8050/path2")},
                {"datatosend", new VariantArray {new VariantValue("/data1")}},
                {"subscribeid", new VariantValue(2)},
            });

            var res2 = ioTCore.MessageDispatcher.HandleRequest(requestMessage2);
            subscriptionlist = ioTCore.MessageDispatcher.HandleRequest(0, "/getsubscriberlist");
            Assert.That(subscriptionlist.Code, Is.EqualTo(200));

            var subscriptionListData2 = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionlist.Data);

            Assert.That(subscriptionListData2.Count(), Is.EqualTo(2));

            Assert.That(subscriptionListData2[1].SubscriptionId, Is.EqualTo(2));
            Assert.That(subscriptionListData2[1].CallbackUrl, Is.EqualTo("http://127.0.0.1:8050/path2"));
        }

        /// <summary>
        /// Tests of one subscription will be visible only one time with getsubscriberlist.
        /// </summary>
        [Test]
        public void SubscriptionList_GetSubscriberList_Count_Linked_EventElements()
        {
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var eventElement = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, "test");
            var structure = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "container");
            ioTCore.ElementManager.AddLink(structure, eventElement);

            var result = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1234, "/test/subscribe",
                Variant.FromObject(new SubscribeRequestServiceData("http://127.0.0.1:8090"))));

            var getSubscriberList = ioTCore.MessageDispatcher.HandleRequest(0, "/getsubscriberlist");

            var getSubscriberListResponse= getSubscriberList.Data.ToObject<GetSubscriberListResponseServiceData>();

            Assert.That(getSubscriberListResponse.Count(), Is.EqualTo(1));

        }
    }
}
