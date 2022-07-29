namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using Messages;
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

    }
}
