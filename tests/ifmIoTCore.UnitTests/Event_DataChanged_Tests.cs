namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    using Converter.Json;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class Event_DataChanged_Tests
    {
        //[Test, Property("TestCaseKey", "IOTCS-T22")]
        //public void DataChangedEvent_IsTriggered_ForInternalSubscribers()
        //{
        //    IIoTCore ioTCore = IoTCoreFactory.Create("myiotcore");
        //    bool triggered = false;
        //    IDataElement data1 = new DataElement<int>(null, (element, i) => true, false, "data1", new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)), new List<string>(),"data1uid");
        //    ioTCore.Root.AddChild(data1, false);
        //    // LocalEvent should also be triggered when it is subscribed internal and not just SubscriptionList
        //    data1.DataChangedEvent += (sender, args) => { triggered = true; };

        //    Assert.Multiple(() =>
        //    {
        //        triggered = false;
        //        data1.Value = 42;
        //        Assert.IsTrue(triggered);
        //    });
        //}

        //[Test, Property("TestCaseKey", "IOTCS-T21")]
        //public void DataChangedEvent_IsSuppressable_byApplication()
        //{
        //    IIoTCore ioTCore = IoTCoreFactory.Create("myiotcore");
        //    bool triggered = false;
        //    var data1 = new DataElement<int>(
        //        getDataFunc: (s) => 0,
        //        setDataFunc: (s, e) => false, false,
        //        "data1",
        //        new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)),
        //        new List<string>());  

        //    ioTCore.Root.AddChild(data1);

        //    data1.DataChangedEvent += (sender, args) => { triggered = true; };

        //    Assert.Multiple(() =>
        //    {
        //        triggered = false;
        //        data1.Value = 42;
        //        Assert.IsFalse(triggered); // works as expected - DataChangedEvent callback is not called

        //    });
        //}

        [Test, Property("TestCaseKey", "IOTCS-T24")]
        public void DataChangedEvent_IsSentOver_http()
        { // integration test. system reaching external http server / other iot core, here a mock http listener
            using var ioTCore = IoTCoreFactory.Create("myiotcore", null);
            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var data1 = ioTCore.CreateDataElement<int>(ioTCore.Root,
                "data1",
                (s) => { return 42; },
                null,
                format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0)),
                profiles: new List<string>(), 
                uid: "myuid_data1");
            data1.DataChangedEventElement = ioTCore.CreateEventElement(data1, Identifiers.DataChanged);
            

            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8052/");
            var datachangeSubscribeResponse = ioTCore.HandleRequest(0, 
                "/data1/datachanged/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8052/somepath/', 
                                'datatosend': ['/data1'], 
                                'duration': 'uptime', 
                                'uid': 'ExternalSubscriber1'} "));
            Assert.That(datachangeSubscribeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.AreEqual(200, ResponseCodes.Success);
            // trigger DataChanged Event implicitly by setdata
            data1.Value = 42;
            var reqcontent = receiver.Do();

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();

            Assert.AreEqual(reqcontent.SelectToken("$.code")?.ToObject<int>(), 80); // ensure code is 80 which is event 
            Assert.AreEqual(reqcontent.SelectToken("$.data.srcurl")?.ToObject<string>(), "/data1/datachanged"); // check srcurl is treechanged
            Assert.AreEqual(reqcontent.SelectToken("$.data.payload./data1.data")?.ToObject<int>(),42); // check datatosend delivered
        }

    }
}
