using System.Collections.Concurrent;
using NUnit.Framework.Internal.Execution;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Converter.Json;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class Event_Trigger_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T37")]
        public void TriggerService_AccessibleThroughNetAdapter_Http()
        { // integration test: trigger works if event data reaches subscribers
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var myevent = ioTCore.CreateEventElement(ioTCore.Root, "myevent");
            ioTCore.CreateActionServiceElement(myevent, 
                Identifiers.TriggerEvent,
                (s, cid) => { myevent.RaiseEvent(); });

            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                (sender) => { return 42; }, 
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0))) ;

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var subscribeResult = ioTCore.HandleRequest(0,
                "/myevent/subscribe",
                JObject.Parse(@" { 
                            'callback': 'http://127.0.0.1:10002/somepath', 
                            'datatosend': ['/data1'], 
                            'duration': 'uptime', 'uid': 'subscribe1'}"));
            Assert.AreEqual(ResponseCodes.Success, subscribeResult.Code);

            // receive subscribe through http net adapter (client)
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:10002/somepath/");

            // trigger externally - through http net adapter (server)
            var triggerResult = new StartNetAdapterServerAndGetResponseHttp().Do(
                new RequestMessage(2, "/myevent/triggerevent", null),
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
                "/myevent");
            Assert.GreaterOrEqual(eventData.SelectToken("$.data.eventno").ToObject<int>(), 0); // skipped checking dynamic event no.
            Assert.AreEqual(42, eventData.SelectToken("$.data.payload./data1.data")?.ToObject<int>());

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T61")]
        public void Events_Triggered_AllTogether_10()
        { // integration test: trigger works if event data reaches subscribers
            const int MaxEvents = 10;
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                (sender) => { return 42; }, 
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)));

            var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter(), TimeSpan.FromSeconds(10), false);

            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);

            var eventAddresses = new List<string>();
            var triggerAddresses = new List<string>();
            var subscriberUrls = new List<string>();
            var subscriberDataList = new List<JToken>();
            for (var eventno = 0; eventno < MaxEvents; eventno++)
            {
                var eventId = string.Format("myevent{0}", eventno);
                eventAddresses.Add("/" + eventId);
                var myevent = ioTCore.CreateEventElement(ioTCore.Root, eventId);
                ioTCore.CreateActionServiceElement(myevent, 
                    Identifiers.TriggerEvent,
                    (s, cid) => { myevent.RaiseEvent(); });

                triggerAddresses.Add(string.Format("/{0}/triggerevent",eventId));
                var subscriberData = JObject.Parse(@" { 
                            'callback': 'NotInitialisedYet', 
                            'datatosend': ['/data1'], 
                            'duration': 'uptime'}");
                var host = $"http://127.0.0.1:{10006 + eventno}/";
                var url = string.Format("{0}somepath/",host);
                //var url = string.Format("http://127.0.0.1:{0}/somepath/", 10006); // single subscriber
                subscriberData["callback"] = host;
                subscriberData["uid"] = "subscribe_" + eventId.ToString();
                subscriberUrls.Add(host);
                subscriberDataList.Add(subscriberData);
                var subscribeResult = ioTCore.HandleRequest(0, string.Format("/{0}/subscribe",eventId), subscriberData);
                Assert.AreEqual(ResponseCodes.Success, subscribeResult.Code);
            }
            var subscribers = ioTCore.HandleRequest(0, "/getsubscriberlist");
            Assert.AreEqual(ResponseCodes.Success, subscribers.Code);
            Assert.AreEqual(MaxEvents, subscribers.Data.Count());

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
}
