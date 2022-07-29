using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements.ServiceData.Responses;
using Newtonsoft.Json;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    public class TestSubscribeMessages
    {
        public static IEnumerable SameData_AlternativeNames
        {
            get
            {
                yield return new TestCaseData(
                    new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject() {
                        {"callback", new VariantValue("http://127.0.0.1:8000/mydevice/someeventhandler")},
                        {"datatosend", new VariantArray() {new VariantValue("/getidentity")}},
                        {"subscribeid", new VariantValue(1)},})).SetName("{m}_mandatoryfield_callback"); 

                yield return new TestCaseData(
                    new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject() {
                        {"callbackurl", new VariantValue("http://127.0.0.1:8000/mydevice/someeventhandler")},
                        {"datatosend", new VariantArray() {new VariantValue("/getidentity")}},
                        {"subscribeid", new VariantValue(1)},})).SetName("{m}_AlternativeName_callbackurl"); 
            }
        }
    }

    [TestFixture]
    public class Event_Subscribe_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void Subscribe_ValidRequest_AcknowledgedWith_uid_Response()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new EventElement(identifier: "myevent"), true);
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callbackurl': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/getidentity'], 
                                'subscribeid': 1}
                            }");
            var converter = new MessageConverter.Json.Newtonsoft.MessageConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize(request.ToString()));
            Assert.AreEqual(ResponseCodes.Success, 200);
            Assert.AreEqual(res.Code, ResponseCodes.Success);
            Assert.AreEqual(res.Cid, 1);
            Assert.AreEqual(res.Address, "/myevent/subscribe");

            var subscribeResponseData = Variant.ToObject<SubscribeResponseServiceData>(res.Data);

            Assert.NotNull(subscribeResponseData.SubscriptionId);
            Assert.AreEqual(subscribeResponseData.SubscriptionId, 1);
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        [TestCaseSource(typeof(TestSubscribeMessages), nameof(TestSubscribeMessages.SameData_AlternativeNames))]
        public void Subscribe_ValidRequest_AddedTo_SubscriptionInfo(Message requestMessage)
        { // pre-condition: assumes getsubscriberlist service tests pass
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new EventElement("myevent"), true);

            var converter= new MessageConverter.Json.Newtonsoft.MessageConverter();
            var res = ioTCore.HandleRequest(requestMessage);
            var subscriptionInfo = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(0, "/getsubscriberlist").Data);
            
            Assert.That(subscriptionInfo.Any(x=>x.Address == "ioTCore/myevent" && 
                                                x.SubscriptionId == 1 && 
                                                x.Callback == "http://127.0.0.1:8000/mydevice/someeventhandler"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void SubscriberInternal_ReceivesEvent_OnTrigger_invalidDataToSend()
        { 
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var myevent = new EventElement(identifier: "myevent");
            ioTCore.Root.AddChild(myevent, true);
            var triggered = false;
            myevent.Subscribe((element => triggered = true));
            var request = JObject.Parse(@" {
                            'code': 10, 
                            'cid': 1, 
                            'adr': '/myevent/subscribe', 
                            'data': { 
                                'callbackurl': 'http://127.0.0.1:8000/mydevice/someeventhandler', 
                                'datatosend': ['/nonexistingelement/service'], 
                                'duration': 'uptime', 'uid': 'subscribe1'}
                            }");
            var converter= new MessageConverter.Json.Newtonsoft.MessageConverter();
            var res = ioTCore.HandleRequest(converter.Deserialize(request.ToString()));
            triggered = false;
            myevent.RaiseEvent();
            Assert.IsTrue(triggered);
        }

        [Test, Property("TestCaseKey", "IOTCS-T25")]
        public void SubscribeRequest_uid_generated_if_not_provided()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new EventElement("myevent"), true);

            var requestMessage = new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject()
            {
                {"callbackurl", new VariantValue("http://127.0.0.1:8000/somepath")},
                {"datatosend", new VariantArray() {new VariantValue("/nonexistingelement/service")}},
                {"duration", new VariantValue("uptime")},
            });

            var converter= new MessageConverter.Json.Newtonsoft.MessageConverter();
            var res = ioTCore.HandleRequest(requestMessage);
            Assert.NotNull(Variant.ToObject<SubscribeResponseServiceData>(res.Data).SubscriptionId);
            Assert.NotZero(Variant.ToObject<SubscribeResponseServiceData>(res.Data).SubscriptionId);
        }

        [Test, Property("TestCaseKey", "IOTCS-T27")]
        public void Subscribe_InvalidRequests_InvalidAddress_DataFieldMissing()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new EventElement("myevent"), true);

            var subscribeResponse = ioTCore.HandleRequest(0, "/myevent1/subscribe"); // invalid address
            Assert.AreEqual(404, ResponseCodes.NotFound);
            Assert.AreEqual(subscribeResponse.Code, ResponseCodes.NotFound);

            subscribeResponse = ioTCore.HandleRequest(0, "/myevent/subscribe", data: null); // profileData field is missing
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.AreEqual(ResponseCodes.DataInvalid, subscribeResponse.Code);
        }

        [Test, Property("TestCaseKey", "IOTCS-T29")]
        public void SubscribedEventRemoved_SubscriberListUpdated()
        { // this test assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new ReadOnlyDataElement<int>("data1",
                    _
                    => 42,
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0))), true);
            var myevent = new EventElement("myevent");
            ioTCore.Root.AddChild(myevent, true);
            var subscribeReq = ioTCore.HandleRequest(0, 
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callbackurl", new VariantValue("http://127.0.0.1:8000/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(1) },
                }
               );
            var subscribeReq2 = ioTCore.HandleRequest(0, 
                "/myevent/subscribe",
                new VariantObject()
                {
                    { "callbackurl", new VariantValue("http://127.0.0.1:8001/somepath/") }, 
                    { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                    { "subscribeid", new VariantValue(2) },
                }
               );
            var subscriptionsBeforeRemove = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));

            var subscriptionsBeforeRemoveData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionsBeforeRemove.Data);
            
            // check subscriptions were added to subscriberlist
            Assert.AreEqual(subscriptionsBeforeRemoveData.Count(), 2);

            Assert.AreEqual(1, subscriptionsBeforeRemoveData[0].SubscriptionId);
            Assert.AreEqual(2, subscriptionsBeforeRemoveData[1].SubscriptionId);

            // remove subscribed event element
            ioTCore.Root.RemoveChild(myevent);

            // check subscriptions were removed from subscriberlist
            var subscriptionsAfterRemove = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));
            var subscriptionsAfterRemoveData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionsAfterRemove.Data);

            Assert.That(subscriptionsAfterRemoveData.All(x => x.SubscriptionId != 2));

            // trigger and check if event is not served to removed subscribers
            var triggerReq = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/myevent/triggerevent", null));
            Assert.That(triggerReq.Code == ResponseCodes.NotFound); 
        }

        [Test, Property("TestCaseKey", "IOTCS-T59")]
        public void SubscriptionTwice_SameRequest_RejectedSecondTime()
        { // this test assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new ReadOnlyDataElement<int>("data1",
                    (sender) => { return 42; },
                
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0))), true);
            ioTCore.Root.AddChild(new EventElement("myevent"), true);

            var singleSubscribeMessage = new Message(RequestCodes.Request, 1234, "/myevent/subscribe", new VariantObject()
            {
                { "callbackurl", new VariantValue("http://127.0.0.1:8050/somepath/") },
                { "datatosend", new VariantArray() { new VariantValue("/data1") } },
                { "subscribeid", new VariantValue(1) },
            });
            var subscribeReq = ioTCore.HandleRequest( singleSubscribeMessage);
            var subscribeRepeat = ioTCore.HandleRequest( singleSubscribeMessage);

            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            // check if second subscription was not added in subscriber list
            Assert.AreEqual(1, subscriptions.Count());
            Assert.AreEqual(1, subscriptions[0].SubscriptionId);
        }

        [Test, Property("TestCaseKey", "IOTCS-T59")]
        public void SubscribeRequest_Overwritten_ByKeepingUidSame()
        { // this test assume, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new EventElement("myevent"), true);
            var overwriteSubscribeMsgs = new List<Message> {
                new Message(RequestCodes.Request,
                    cid: 1,
                    address: "/myevent/subscribe",
                    data: new VariantObject() {{"callbackurl",new VariantValue("http://127.0.0.1:8050/somepath/")},{"datatosend",new VariantArray () {new VariantValue("/data1")}},{"duration",new VariantValue("uptime")},{"subscribeid",new VariantValue(1)},}),
                new Message(RequestCodes.Request,
                    cid: 2,
                    address: "/myevent/subscribe",
                    data: new VariantObject() {{"callbackurl",new VariantValue("http://127.0.0.1:8050/somepath/")},{"datatosend",new VariantArray () {new VariantValue("/data2")}},{"duration",new VariantValue("uptime")},{"subscribeid",new VariantValue(1)},}), 
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
            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.AreEqual(1, subscriptions.Count());

            Assert.That(subscriptions.Any(x => x.DataToSend.Contains("/data2")));
            Assert.That(subscriptions[0].DataToSend.Contains("/data2"));

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Assert.IsNull(subscriptions[1].DataToSend.SingleOrDefault(x => x.Equals("/data1")));
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T28")]
        public void SubscribeRequest_NewCreated_ByNewUid()
        {
            // this test assumes, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new EventElement(identifier: "myevent"), true);
            var overwriteSubscribeMsgs = new List<Message> {
                new Message(RequestCodes.Request,
                    cid: 1,
                    address: "/myevent/subscribe",
                    data: new VariantObject()
                    {
                        {"callbackurl",new VariantValue("http://127.0.0.1:8050/somepath1/")},
                        {"datatosend",new VariantArray () {new VariantValue("/data1")}},
                        {"duration",new VariantValue("uptime")},
                        {"uid",new VariantValue("subscribe1")}
                    }),
                new Message(RequestCodes.Request,
                    cid: 2,
                    address: "/myevent/subscribe",
                    data: new VariantObject()
                    {
                        { "callbackurl", new VariantValue("http://127.0.0.1:8050/somepath1/") }, 
                        { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                        { "duration", new VariantValue("uptime") }, 
                        { "uid", new VariantValue("subscribe2") },
                    }), 
                new Message(RequestCodes.Request,
                    cid: 3,
                    address: "/myevent/subscribe",
                    data: new VariantObject()
                    {
                        { "callbackurl", new VariantValue("http://127.0.0.1:8050/somepath1/") }, 
                        { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                        { "duration", new VariantValue("uptime") }, 
                        { "uid", new VariantValue("subscribe3") },
                    }),
                new Message(RequestCodes.Request,
                    cid: 4,
                    address: "/myevent/subscribe",
                    data: new VariantObject()
                    {
                        { "callback", new VariantValue("http://127.0.0.1:8050/somepath1/") }, 
                        { "datatosend", new VariantArray() { new VariantValue("/data1") } }, 
                        { "duration", new VariantValue("uptime") }, 
                        { "uid", new VariantValue("subscribe4") },
                    })};

            Assert.Multiple(() =>
            {
                foreach (var overwriteSubscribeMsg in overwriteSubscribeMsgs)
                {
                    var invalidSubscribeReq = ioTCore.HandleRequest(overwriteSubscribeMsg);
                    Assert.AreEqual(invalidSubscribeReq.Code, ResponseCodes.Success); Assert.AreEqual(ResponseCodes.Success, 200);
                }

            });
            // check if first subscribe (/data1) is overwritten with (/data2) request
            var subscriptions = Variant.ToObject<GetSubscriberListResponseServiceData>(ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null)).Data);
            Assert.AreEqual(4 , subscriptions.Count()); // 4 subscription requests
        }

        [Test, Property("TestCaseKey", "IOTCS-T57")]
        public void SubscriptionList_Appended_OnNew_SubscribeIDRequest()
        {
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new ReadOnlyDataElement<int>("data1",
                (sender) => { return 42; },
                
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0))), true);
            ioTCore.Root.AddChild(new EventElement("myevent"), true);


            var requestMessage = new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject()
            {
                {"callbackurl", new VariantValue("http://127.0.0.1:8050/path1")},
                {"datatosend", new VariantArray() {new VariantValue("/data1")}},
                {"subscribeid", new VariantValue(1)},
            });

            var converter= new MessageConverter.Json.Newtonsoft.MessageConverter();
            var res = ioTCore.HandleRequest(requestMessage);
            var subscriptionlist = ioTCore.HandleRequest(0, "/getsubscriberlist");
            Assert.That(subscriptionlist.Code, Is.EqualTo(200));
            
            var subscriptionlistData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionlist.Data);
            Assert.That(subscriptionlistData.Count(), Is.EqualTo(1));

            Assert.AreEqual(1, subscriptionlistData[0].SubscriptionId);
            Assert.AreEqual("http://127.0.0.1:8050/path1", subscriptionlistData[0].Callback);


            var requestMessage2 = new Message(RequestCodes.Request, 1, "/myevent/subscribe", new VariantObject()
            {
                {"callbackurl", new VariantValue("http://127.0.0.1:8050/path2")},
                {"datatosend", new VariantArray() {new VariantValue("/data1")}},
                {"subscribeid", new VariantValue(2)},
            });

            var res2 = ioTCore.HandleRequest(requestMessage2);
            subscriptionlist = ioTCore.HandleRequest(0, "/getsubscriberlist");
            Assert.That(subscriptionlist.Code, Is.EqualTo(200));

            var subscriptionListData2 = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptionlist.Data);

            Assert.That(subscriptionListData2.Count(), Is.EqualTo(2));

            Assert.AreEqual(2, subscriptionListData2[1].SubscriptionId);
            Assert.AreEqual("http://127.0.0.1:8050/path2", subscriptionListData2[1].Callback);
        }
    }

}
