namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Services_querytree_Tests
    {
        List<string> DeviceElementExpectedServices = new List<string>{ 
            "/getidentity",
            "/gettree", 
            "/querytree",
            "/getdatamulti", 
            "/setdatamulti",
            "/getsubscriberlist",
            "/treechanged/subscribe",
            "/treechanged/unsubscribe",
            };

        [Test, Property("TestCaseKey", "IOTCS-T56")]
        public void querytree_query_type_name()
        { 
            var iiotcore1 = IoTCoreFactory.Create("device0", null);
            var rootDevice = iiotcore1.Root;

            var qResult = rootDevice.QueryTree(new ifmIoTCore.Elements.ServiceData.Requests.QueryTreeRequestServiceData(type: "device"));
            Assert.AreEqual(qResult.Addresses.Count, 1);
            Assert.AreEqual(rootDevice.Address, qResult.Addresses[0]);

            var qResult2 = rootDevice.QueryTree(new ifmIoTCore.Elements.ServiceData.Requests.QueryTreeRequestServiceData(type: "service"));
            Assert.GreaterOrEqual(qResult2.Addresses.Count, 8); 
            Assert.Multiple(() =>
            {
                foreach (var serviceAddress in DeviceElementExpectedServices)
                    Assert.IsTrue(qResult2.Addresses.Any(e => e == serviceAddress), string.Format("Not found service {0}",serviceAddress));
            });

            var type_and_name_query = rootDevice.QueryTree(new ifmIoTCore.Elements.ServiceData.Requests.QueryTreeRequestServiceData(type: "service", name: "subscribe"));
            Assert.GreaterOrEqual(type_and_name_query.Addresses.Count, 1); 
            Assert.IsTrue(type_and_name_query.Addresses.Any(e => e == "/treechanged/subscribe"));

            var s1 = iiotcore1.CreateStructureElement(iiotcore1.Root, "struct1");
            iiotcore1.CreateDataElement<object>(s1, "int1");
            iiotcore1.CreateDataElement<object>(s1, "string1");
            iiotcore1.CreateDataElement<object>(iiotcore1.Root, "int1");
            var type_and_name_query2 = rootDevice.QueryTree(new ifmIoTCore.Elements.ServiceData.Requests.QueryTreeRequestServiceData(type: "data", name: "int1"));
            Assert.GreaterOrEqual(type_and_name_query2.Addresses.Count, 2);
            Assert.IsTrue(type_and_name_query2.Addresses.All(e => e.EndsWith("int1")));
        }

        [Test, Property("TestCaseKey", "IOTCS-T56")]
        public void querytree_query_null()
        { 
            var iiotcore1 = IoTCoreFactory.Create("device0", null);
            Assert.DoesNotThrow(() => iiotcore1.Root.QueryTree(null));
        }

        [Test, Property("TestCaseKey", "IOTCS-T55")]
        public void querytree_query_type_name_CommandInterface()
        { 
            var iiotcore1 = IoTCoreFactory.Create("device0", null);
            var rootDevice = iiotcore1.Root;

            var qResult = iiotcore1.HandleRequest(0, "/querytree", JToken.Parse("{type: 'device'}"));
            Assert.AreEqual(qResult.Data.Count(), 1);
            Assert.AreEqual(rootDevice.Address, qResult.Data.SelectToken("$.adrlist[0]").ToObject<string>());

            var qResult2 = iiotcore1.HandleRequest(0, "/querytree", JToken.Parse("{'type': 'service'}"));
            Assert.IsNotNull(qResult2.Data.SelectToken("$.adrlist"));
            Assert.GreaterOrEqual(qResult2.Data["adrlist"].Count(), 8); 
            Assert.Multiple(() =>
            {
                foreach (var serviceAddress in DeviceElementExpectedServices)
                {
                    //Assert.IsTrue(qResult2.Data["adrlist"].Contains(new JValue(serviceAddress)), string.Format("Did not find serviceAddress in adrlist json: {0}", serviceAddress));
                    Assert.IsNotNull(qResult2.Data["adrlist"].Any(e => e.ToObject<string>() == serviceAddress), string.Format("Did not find serviceAddress in adrlist json: {0}", serviceAddress));
                }
            });

            var type_and_name_query = iiotcore1.HandleRequest(0, "/querytree", JToken.Parse("{'type': 'service', 'name': 'subscribe'}"));
            Assert.IsNotNull(type_and_name_query.Data.SelectToken("$.adrlist"));
            Assert.GreaterOrEqual(type_and_name_query.Data["adrlist"].Count(), 1); 
            Assert.IsTrue(type_and_name_query.Data["adrlist"].Any(e => e.ToObject<string>() == "/treechanged/subscribe"));

        }

        [Test, Property("TestCaseKey", "IOTCS-T55")]
        public void querytree_query_null_viaCommandInterface()
        {
            var iiotcore1 = IoTCoreFactory.Create("device0", null);
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, iiotcore1.HandleRequest(0, "/querytree", JToken.Parse(@"null")).Code);
            Assert.AreEqual(ResponseCodes.Success, iiotcore1.HandleRequest(0, "/querytree", JToken.Parse(@"{}")).Code);
        }

        [Test, Property("TestCaseKey", "IOTCS-T56")]
        [Ignore("Not part of 1.0 release testing")]
        public void querytree_query_profile()
        { // this test for subscriberlist assumes, subscribe, trigger event works.
        }
    }
}
