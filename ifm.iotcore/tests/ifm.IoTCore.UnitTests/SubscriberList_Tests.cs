namespace ifm.IoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ifm.Common.Variant;
    using Message;
    using NUnit.Framework;

    [TestFixture]
    public class SubscriberList_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T38")]
        public void SubscriberList_MultipleEvents()
        { 
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data1", 42);

            var eventIDs = new List<string> { "myevent", "myevent2", "myevent3", "myevent4", "myevent5" };
            var randomCids = new List<int>(5);
            var randomCid = 0; // Random sometimes creates same value
            foreach (var id in eventIDs)
            {
                var myevent = ioTCore.ElementManager.CreateEventElement(ioTCore.Root, id);
                var svcaddr = string.Format("/{0}/subscribe", myevent.Identifier);
                var data = new VariantObject
                {
                    { "callback", new VariantValue("http://callback/not/considered/on/subscribe") }, 
                    { "datatosend", new VariantArray { new VariantValue("/data1") } }
                };
                ++randomCid;
                randomCids.Add(randomCid);

                ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, randomCid, svcaddr, data)); 
            }
            // check subscriptions were appended
            var subscriptions = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/getsubscriberlist", null));
            var subscriptionsData = Variant.ToObject<GetSubscriberListResponseServiceData>(subscriptions.Data);

            Assert.That(subscriptionsData.Count(), Is.EqualTo(5));
            for (var i = 0; i < 5; i++)
            {
                Assert.That(randomCids[i],Is.EqualTo(subscriptionsData[i].SubscriptionId), "subscribeid should match the subscribe request. iot core compatibility");
            }
        }
    }
}
