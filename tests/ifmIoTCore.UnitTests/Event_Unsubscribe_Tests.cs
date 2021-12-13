using System;
using log4net.Core;

namespace ifmIoTCore.UnitTests
{
    using System.Linq;
    using Converter.Json;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    using log4net;
    using log4net.Appender;
    using log4net.Repository.Hierarchy;
    using System.Reflection;
    using ifmIoTCore.Elements;

    internal class TemporaryMemoryAppender : IDisposable
    {
        private readonly MemoryAppender _memoryAppender;
        private string _appenderGuid;

        public TemporaryMemoryAppender()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetExecutingAssembly());
            _appenderGuid = Guid.NewGuid().ToString();
            _memoryAppender = new MemoryAppender { Name = _appenderGuid };
            hierarchy.Root.AddAppender(_memoryAppender);
        }

        public void Dispose()
        {
            _memoryAppender.Close();
            var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetExecutingAssembly());
            hierarchy.Root.RemoveAppender(_memoryAppender);
        }

        public LoggingEvent[] PopAllEvents()
        {
            return _memoryAppender.PopAllEvents();
        }
    }


    [TestFixture]
    public class Event_Unsubscribe_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T33")]
        public void SubscriptionRemoved_IsNotServedFurther_RemovedFromSubscriptionList()
        { // this test for unsubscribe assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);

            ioTCore.CreateDataElement<int>(ioTCore.Root, 
                "data1",
                    (sender) => { return 42; },
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, "myevent");
            ioTCore.CreateActionServiceElement(myevent, 
                Identifiers.TriggerEvent,
                (_, _) => { myevent.RaiseEvent(); });


            var subscribeReq = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:9050/somepath/', 
                                'datatosend': ['/data1'], 
                                'duration': 'uptime', 
                                'subscribeid': 1} ")
               );
            var subscribeReq2 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:9051/somepath/', 
                                'datatosend': ['/data1'], 
                                'duration': 'uptime', 
                                'subscribeid': 2} ")
               );
            // check subscriptions were appended
            var subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(subscriptions.Data.Count(), 2); 
            Assert.AreEqual(subscriptions.Data.SelectToken("$[0].subscribeid")?.ToObject<int>(), 1);
            Assert.AreEqual(subscriptions.Data.SelectToken("$[1].subscribeid")?.ToObject<int>(), 2);
            // trigger event to check ifoth subscriptions are delivered event info
            var receiver1 = new ReceiveIoTMsg("http://127.0.0.1:9050/somepath/");
            var receiver2 = new ReceiveIoTMsg("http://127.0.0.1:9051/somepath/");
            var triggerReq = ioTCore.HandleRequest(new RequestMessage(1, "/myevent/triggerevent", null));
            var eventMsg1 = receiver1.Do();
            var eventMsg2 = receiver2.Do();
            Assert.AreEqual(eventMsg1.SelectToken("$../data1.data")?.ToObject<int>(), 42);
            Assert.AreEqual(eventMsg2.SelectToken("$../data1.data")?.ToObject<int>(), 42);

            // unsubscribe first subscriber using callback
            var unsubscribeResponse = ioTCore.HandleRequest(1, "/myevent/unsubscribe",
                JToken.Parse(@"{'callback': 'http://127.0.0.1:9050/somepath/'}"));
            Assert.AreEqual(200, unsubscribeResponse.Code); Assert.AreEqual(ResponseCodes.Success, 200);  
            // check entry removed from subscription list
            subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(1, subscriptions.Data.Count()); // 1 subscriptions removed of 2  = remaining 1
            Assert.NotNull(subscriptions.Data.SelectToken("$..[?(@.subscribeid == 2)]"));
            Assert.AreEqual(subscriptions.Data.SelectToken("$[0].subscribeid")?.ToObject<int>(), 2);
            Assert.IsNull(subscriptions.Data.SelectToken("$..[?(@.subscribeid == 1)]"));

            var receiver3 = new ReceiveIoTMsg("http://127.0.0.1:9051/somepath/");
            var receiver4 = new ReceiveIoTMsg("http://127.0.0.1:9050/somepath/");
            // trigger and check if event is not served to removed subscriber
            triggerReq = ioTCore.HandleRequest(new RequestMessage(1, "/myevent/triggerevent", null));
            // check available subscriber receives event
            eventMsg2 = receiver3.Do();
            Assert.AreEqual(eventMsg2.SelectToken("$../data1.data")?.ToObject<int>(), 42);
            // check removed subscriber does not receive event
            eventMsg1 = receiver4.Do(1000);
            Assert.IsNull(eventMsg1);

            // unsubscribe second subscriber using uid
            var unsubscribeResponse2 = ioTCore.HandleRequest(2, "/myevent/unsubscribe",
                JToken.Parse(@"{'callback': 'http://127.0.0.1:9051/somepath/'}"));
            Assert.AreEqual(200, unsubscribeResponse.Code); Assert.AreEqual(ResponseCodes.Success, 200);
            // check remaining entry removed from subscription list
            subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.IsNull(subscriptions.Data.SelectToken("$..[?(@.subscribeid == 2)]"));

            // trigger and check if event is not served to removed subscriber
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:9051/somepath/");
            triggerReq = ioTCore.HandleRequest(new RequestMessage(1, "/myevent/triggerevent", null));
            // check removed subscriber does not receive event
            eventMsg2 = receiver.Do();
            Assert.IsNull(eventMsg2);

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T33")]
        public void Unsubscribe_RemovesSubscriptions_CallbackRemovesMultiple_WithUidRemovesSpecific()
        { // this test for unsubscribe assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                    (sender) => { return 42; },
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");
            var subscribeReq = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 1} ")
               );
            var subscribeReq2 = ioTCore.HandleRequest(2,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'datatosend': ['/data1']} ")
               );
            var subscribeReq3 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8051/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 3} ")
               );
            var subscribeReq4 = ioTCore.HandleRequest(4,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8051/somepath/', 
                                'datatosend': ['/data1'] } ")
               );

            // check subscriptions were appended
            var subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(4, subscriptions.Data.Count()); 

            // unsubscribe subscriber(s) using cid
            var unsubscribeResponse = ioTCore.HandleRequest(1, "/myevent/unsubscribe",
                JToken.Parse(@"{'callback': 'http://127.0.0.1:8050/somepath/'}"));
            Assert.AreEqual(200, unsubscribeResponse.Code); Assert.AreEqual(ResponseCodes.Success, 200);
            subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(3, subscriptions.Data.Count()); // 2 subscriptions removed of 4  = remaining 2

            // unsubscribe single subscriber entry using subscribeid
            // now try uid with callback
            var unsubscribeResponse2 = ioTCore.HandleRequest(0, "/myevent/unsubscribe",
                JToken.Parse(@"{'callback': 'http://127.0.0.1:8051/somepath/', 'subscribeid': 2}"));
            Assert.AreEqual(200, unsubscribeResponse2.Code); Assert.AreEqual(ResponseCodes.Success, 200);
            subscriptions = ioTCore.HandleRequest(new RequestMessage(1, "/getsubscriberlist", null));
            Assert.AreEqual(2, subscriptions.Data.Count()); // 1 subscriptions removed of 2  = remaining 1

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
        }

        [Test, Property("TestCaseKey", "IOTCS-T34")]
        public void Unsubscribe_InvalidRequests_RejectedWithLogs()
        { // this test for unsubscribe assumes, subscribe, getsubscriberlist works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());

            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                    (sender) => 42,
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));
            ioTCore.CreateEventElement(ioTCore.Root, identifier: "myevent");

            var subscribeReq = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 1} ")
               );
            var subscribeReq2 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'datatosend': ['/data1'] } ")
               );
            var subscribeReq3 = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                data: JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8051/somepath/', 
                                'datatosend': ['/data1'], 
                                'subscribeid': 2} ")
               );
            Assert.AreEqual(ioTCore.HandleRequest(new RequestMessage(4, "/getsubscriberlist", null)).Data.Count(), 3);

            // invalid unsubscribe requests
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.AreEqual(404, ResponseCodes.NotFound);

            var incorrect_mandatory_callback = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'callback1': 'http://127.0.0.1:8050/somepath/' }")
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, incorrect_mandatory_callback.Code, "incorrect_mandatory_callback");

            var uid_without_mandatory_callback = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'uid': 'http://127.0.0.1:8050/somepath/' }")
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, uid_without_mandatory_callback.Code, "uid_without_mandatory_callback expected 530 data invalid");

            var invalid_event_address = ioTCore.HandleRequest(0,
                "/myevent1/unsubscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/' }")
                );
            Assert.AreEqual(ResponseCodes.NotFound, invalid_event_address.Code, "invalid_event_address");

            var mismatching_callback_uid = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8051/somepath/', 
                                'subscribeid': 1} ")
                );
            Assert.AreEqual(ResponseCodes.Success, mismatching_callback_uid.Code, "mismatching_callback_uid should be 200 OK without warning");

            var non_existing_callback = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'callback': 'http://non.existing.path/somepath/'} ")
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, non_existing_callback.Code, "non_existing_callback should be 530");

            var non_existing_uid = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/', 
                                'subscribeid': 12345} ") // Does not exist
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, non_existing_uid.Code, "non_existing_uid should be 530");

            var validUnsubscribe = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/'} ")
                );
            Assert.AreEqual(ResponseCodes.Success, validUnsubscribe.Code, "Expected valid unsubscribe result");
            var UnsubscribeTwice = ioTCore.HandleRequest(0,
                "/myevent/unsubscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8050/somepath/'} ")
                );
            Assert.AreEqual(ResponseCodes.DataInvalid, UnsubscribeTwice.Code, "UnsubscribeTwice should be 530");

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }
    }
}
