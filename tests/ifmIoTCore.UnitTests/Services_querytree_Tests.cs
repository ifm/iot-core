using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.ServiceData.Requests;
    using ifmIoTCore.Elements.ServiceData.Responses;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Utilities;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;

    public class querytreeMessages
    {
        public static IEnumerable<TestCaseData> samples_alternativenames
        {
            get
            {
                yield return new TestCaseData(
                new MessageConverter().Deserialize(
                    JObject.Parse(@"{
                        'cid': 1,
                        'code': 10,
                        'adr': '/querytree',
                        'data': { 
                                'identifier': 'subscribe'
                                }
                         }").ToString()
                        )
                ).SetName("{m}_identifier");

                yield return new TestCaseData(
                new MessageConverter().Deserialize(
                    JObject.Parse(@"{
                        'cid': 1,
                        'code': 10,
                        'adr': '/querytree',
                        'data': { 
                                'name': 'subscribe'
                                }
                         }").ToString()
                        )
                ).SetName("{m}_name");
            }

        }
    }


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
            var iiotcore1 = IoTCoreFactory.Create("device0");
            var rootDevice = iiotcore1.Root;

            var queryTreeService = rootDevice.QueryTreeServiceElement;

            var qResult = Variant.ToObject<QueryTreeResponseServiceData>(queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(type: "device"))));
            Assert.AreEqual(qResult.Addresses.Count, 1);
            Assert.AreEqual(rootDevice.Address, qResult.Addresses[0]);

            var qResult2 = Variant.ToObject<QueryTreeResponseServiceData>(queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(type: "service"))));
            Assert.GreaterOrEqual(qResult2.Addresses.Count, 8); 
            Assert.Multiple(() =>
            {
                foreach (var serviceAddress in DeviceElementExpectedServices)
                    Assert.IsTrue(qResult2.Addresses.Any(e => e == serviceAddress), string.Format("Not found service {0}",serviceAddress));
            });

            var type_and_name_query = Variant.ToObject<QueryTreeResponseServiceData>(queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(type: "service", identifier: "subscribe"))));
            Assert.GreaterOrEqual(type_and_name_query.Addresses.Count, 1); 
            Assert.IsTrue(type_and_name_query.Addresses.Any(e => e == "/treechanged/subscribe"));

            var s1 = new StructureElement("struct1");
            iiotcore1.Root.AddChild(s1);
            s1.AddChild(new DataElement<object>("int1"));
            s1.AddChild(new DataElement<object>("string1"));
            iiotcore1.Root.AddChild(new DataElement<object>("int1"));
            var type_and_name_query2 = Variant.ToObject<QueryTreeResponseServiceData>(queryTreeService.Invoke(Variant.FromObject(new QueryTreeRequestServiceData(type: "data", identifier: "int1"))));
            Assert.GreaterOrEqual(type_and_name_query2.Addresses.Count, 2);
            Assert.IsTrue(type_and_name_query2.Addresses.All(e => e.EndsWith("int1")));
        }

        [Test, Property("TestCaseKey", "IOTCS-T56")]
        public void querytree_query_null()
        { 
            var iiotcore1 = IoTCoreFactory.Create("device0");
            var queryTreeService = iiotcore1.Root.QueryTreeServiceElement;

            Assert.DoesNotThrow(() => queryTreeService.Invoke(null));
        }

        [Test, Property("TestCaseKey", "IOTCS-T55")]
        public void querytree_query_type_name_CommandInterface()
        { 
            var iiotcore1 = IoTCoreFactory.Create("device0");
            var rootDevice = iiotcore1.Root;

            var qResult = iiotcore1.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse("{type: 'device'}")));

            var qresultData = Variant.ToObject<QueryTreeResponseServiceData>(qResult.Data);

            Assert.AreEqual(qresultData.Addresses.Count, 1);
            Assert.AreEqual(rootDevice.Address, qResult.Data.ToJToken().SelectToken("$.adrlist[0]").ToObject<string>());

            var qResult2 = iiotcore1.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse("{'type': 'service'}")));
            var qresult2Data = Variant.ToObject<QueryTreeResponseServiceData>(qResult2.Data);


            Assert.IsNotNull(qResult2.Data.ToJToken().SelectToken("$.adrlist"));
            Assert.GreaterOrEqual(qresult2Data.Addresses.Count(), 8); 
            Assert.Multiple(() =>
            {
                foreach (var serviceAddress in DeviceElementExpectedServices)
                {
                    //Assert.IsTrue(qResult2.Data["adrlist"].Contains(new JValue(serviceAddress)), string.Format("Did not find serviceAddress in adrlist json: {0}", serviceAddress));
                    Assert.IsNotNull(qresult2Data.Addresses.Any(e => e == serviceAddress), string.Format("Did not find serviceAddress in adrlist json: {0}", serviceAddress));
                }
            });

            var type_and_name_query = iiotcore1.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse("{'type': 'service', 'name': 'subscribe'}")));

            var type_and_name_queryData = Variant.ToObject<QueryTreeResponseServiceData>(type_and_name_query.Data);

            Assert.IsNotNull(type_and_name_query.Data.ToJToken().SelectToken("$.adrlist"));
            Assert.GreaterOrEqual(type_and_name_queryData.Addresses.Count(), 1); 
            Assert.IsTrue(type_and_name_queryData.Addresses.Any(e => e == "/treechanged/subscribe"));

        }

        [Test, Property("TestCaseKey", "IOTCS-T55")]
        public void querytree_query_null_viaCommandInterface()
        {
            var iiotcore1 = IoTCoreFactory.Create("device0");
            Assert.AreEqual(200, ResponseCodes.Success);
            Assert.AreEqual(ResponseCodes.Success, iiotcore1.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse(@"null"))).Code);
            Assert.AreEqual(ResponseCodes.Success, iiotcore1.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse(@"{}"))).Code);
        }


        [Test, Property("TestCaseKey", "IOTCS-T55")]
        [TestCaseSource(typeof(querytreeMessages), nameof(querytreeMessages.samples_alternativenames))]
        public void querytree_ValidMessage_IsProcessed(Message querytreeMessage)
        {
            var iiotcore1 = IoTCoreFactory.Create("device0");
            var type_and_name_query = iiotcore1.HandleRequest(querytreeMessage);

            var type_and_name_queryData = Variant.ToObject<QueryTreeResponseServiceData>(type_and_name_query.Data);

            Assert.IsNotNull(type_and_name_query.Data.ToJToken().SelectToken("$.adrlist"));
            Assert.GreaterOrEqual(type_and_name_queryData.Addresses.Count(), 1); 
            Assert.IsTrue(type_and_name_queryData.Addresses.Any(e => e == "/treechanged/subscribe"));
        }


        [Test, Property("TestCaseKey", "IOTCS-T56")]
        [Ignore("Not part of 1.0 release testing")]
        public void querytree_query_profile()
        { // this test for subscriberlist assumes, subscribe, trigger event works.
        }
    }
}
