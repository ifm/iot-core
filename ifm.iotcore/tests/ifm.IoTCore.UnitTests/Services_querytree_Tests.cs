namespace ifm.IoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using ElementManager.Contracts.Elements;
    using ElementManager.Contracts.Elements.ServiceData.Requests;
    using ElementManager.Contracts.Elements.ServiceData.Responses;
    using ifm.Common.Variant;
    using Message;
    using MessageConverter.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

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
            "device0/getidentity",
            "device0/gettree",
            "device0/querytree",
            "device0/getdatamulti",
            "device0/setdatamulti",
            "device0/getsubscriberlist",
            "device0/treechanged/subscribe",
            "device0/treechanged/unsubscribe",
            };

        [Test, Property("TestCaseKey", "IOTCS-T56")]
        public void querytree_query_type_name()
        { 
            var iiotcore1 = Helpers.CreateIoTCore("device0");
            var rootDevice = (IBaseElement)iiotcore1.Root;

            var queryTreeService = rootDevice.Subs.Single(x => x.Identifier == "querytree") as IServiceElement<QueryTreeRequestServiceData, QueryTreeResponseServiceData>;

            var qResult = queryTreeService.Invoke(new QueryTreeRequestServiceData(type: "device"));
            Assert.That(qResult.Addresses.Count, Is.EqualTo(1));
            Assert.That(rootDevice.Address,Is.EqualTo(qResult.Addresses[0]));

            var qResult2 = queryTreeService.Invoke(new QueryTreeRequestServiceData(type: "service"));
            Assert.That(qResult2.Addresses.Count, Is.GreaterThanOrEqualTo(8)); 
            Assert.Multiple(() =>
            {
                foreach (var serviceAddress in DeviceElementExpectedServices)
                    Assert.That(qResult2.Addresses.Any(e => e == serviceAddress), string.Format("Not found service {0}",serviceAddress));
            });

            var type_and_name_query = queryTreeService.Invoke(new QueryTreeRequestServiceData(type: "service", identifier: "subscribe"));
            Assert.That(type_and_name_query.Addresses.Count, Is.GreaterThanOrEqualTo(1)); 
            Assert.That(type_and_name_query.Addresses.Any(e => e == "device0/treechanged/subscribe"));

            var s1 = iiotcore1.ElementManager.CreateStructureElement(iiotcore1.Root, "struct1");
            iiotcore1.ElementManager.CreateSimpleDataElement<int>(s1, "int1");
            iiotcore1.ElementManager.CreateSimpleDataElement<object>(s1, "string1");
            iiotcore1.ElementManager.CreateSimpleDataElement<object>(iiotcore1.Root, "int1");
            var type_and_name_query2 = queryTreeService.Invoke(new QueryTreeRequestServiceData(type: "data", identifier: "int1"));
            Assert.That(type_and_name_query2.Addresses.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(type_and_name_query2.Addresses.All(e => e.EndsWith("int1")));
        }

        [Test, Property("TestCaseKey", "IOTCS-T55")]
        public void querytree_query_type_name_CommandInterface()
        { 
            var iiotcore1 = Helpers.CreateIoTCore("device0");
            var rootDevice = iiotcore1.Root;

            var qResult = iiotcore1.MessageDispatcher.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse("{type: 'device'}")));

            var qresultData = Variant.ToObject<QueryTreeResponseServiceData>(qResult.Data);

            Assert.That(qresultData.Addresses.Count, Is.EqualTo(1));
            Assert.That(qResult.Data.ToJToken().SelectToken("$.adrlist[0]").ToObject<string>(), Is.EqualTo(rootDevice.Address));

            var qResult2 = iiotcore1.MessageDispatcher.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse("{'type': 'service'}")));
            var qresult2Data = Variant.ToObject<QueryTreeResponseServiceData>(qResult2.Data);


            Assert.That(qResult2.Data.ToJToken().SelectToken("$.adrlist"), Is.Not.Null);
            Assert.That(qresult2Data.Addresses.Count(), Is.GreaterThanOrEqualTo(8)); 
            Assert.Multiple(() =>
            {
                foreach (var serviceAddress in DeviceElementExpectedServices)
                {
                    //Assert.That(qResult2.Data["adrlist"].Contains(new JValue(serviceAddress)), string.Format("Did not find serviceAddress in adrlist json: {0}", serviceAddress));
                    Assert.That(qresult2Data.Addresses.Any(e => e == serviceAddress), Is.Not.Null, string.Format("Did not find serviceAddress in adrlist json: {0}", serviceAddress));
                }
            });

            var type_and_name_query = iiotcore1.MessageDispatcher.HandleRequest(0, "/querytree", VariantConverter.FromJToken(JToken.Parse("{'type': 'service', 'name': 'subscribe'}")));

            var type_and_name_queryData = Variant.ToObject<QueryTreeResponseServiceData>(type_and_name_query.Data);

            Assert.That(type_and_name_query.Data.ToJToken().SelectToken("$.adrlist"), Is.Not.Null);
            Assert.That(type_and_name_queryData.Addresses.Count(), Is.GreaterThanOrEqualTo(1)); 
            Assert.That(type_and_name_queryData.Addresses.Any(e => e == "device0/treechanged/subscribe"));

        }

        [Test, Property("TestCaseKey", "IOTCS-T55")]
        [TestCaseSource(typeof(querytreeMessages), nameof(querytreeMessages.samples_alternativenames))]
        public void querytree_ValidMessage_IsProcessed(Message querytreeMessage)
        {
            var iiotcore1 = Helpers.CreateIoTCore("device0");
            var type_and_name_query = iiotcore1.MessageDispatcher.HandleRequest(querytreeMessage);

            var type_and_name_queryData = Variant.ToObject<QueryTreeResponseServiceData>(type_and_name_query.Data);

            Assert.That(type_and_name_query.Data.ToJToken().SelectToken("$.adrlist"), Is.Not.Null);
            Assert.That(type_and_name_queryData.Addresses.Count(), Is.GreaterThanOrEqualTo(1)); 
            Assert.That(type_and_name_queryData.Addresses.Any(e => e == "device0/treechanged/subscribe"));
        }
    }
}
