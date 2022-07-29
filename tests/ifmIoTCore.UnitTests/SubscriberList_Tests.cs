using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements.ServiceData.Responses;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class SubscriberList_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T38")]
        public void SubscriberList_MultipleEvents()
        { // this test for subscriberlist assumes, subscribe, trigger event works.
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            ioTCore.Root.AddChild(new ReadOnlyDataElement<int>("data1",
                    (sender) => { return 42; },
                    
                    format: new IntegerFormat(new ifmIoTCore.Elements.Valuations.IntegerValuation(0))), true);
            var eventIDs = new List<string> { "myevent", "myevent2", "myevent3", "myevent4", "myevent5" };
            var randomCids = new List<int>(5);
            var randomCid = 0; // Random sometimes creates same value
            foreach (var id in eventIDs)
            {
                var myevent = new EventElement(id);
                ioTCore.Root.AddChild(myevent, true);
                var svcaddr = string.Format("/{0}/subscribe", myevent.Identifier);
                var data = new VariantObject() { { "callback", new VariantValue("callback/not/considered/on/subscribe") }, { "datatosend", new VariantArray() { new VariantValue("/data1") } }, };
                //var subscribeReq = ioTCore.HandleRequest( serviceAddress: svcaddr, data: data );
                // new message with explicit cid for test purpose
                ++randomCid;
                randomCids.Add(randomCid);
                var subscribeReq = ioTCore.HandleRequest(new Message(RequestCodes.Request, randomCid, svcaddr, data)); 
            }
            // check subscriptions were appended
            var subscriptions = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));
            var subscriptionsData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptions.Data);

            Assert.AreEqual(5, subscriptionsData.Count());
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(randomCids[i], subscriptionsData[i].SubscriptionId, "subscribeid should match the subscribe request. iot core compatibility");
            }
        }
    }
}
