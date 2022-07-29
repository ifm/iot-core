namespace ifmIoTCore.Profiles.IoTCoreManagement.UnitTests
{
    using ifmIoTCore.Elements;
    using ifmIoTCore.Common.Variant;
    using Profiles.IoTCoreManagement;
    using ifmIoTCore.Profiles.Base;
    using MessageConverter.Json.Newtonsoft;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class MessageExtensions
    {
        internal static JToken ToJToken(this Messages.Message message)
        {
            return JToken.Parse(new MessageConverter(type: "json").Serialize(message));
        }
    }

    [TestFixture]
    internal class LinkServiceTests
    {
        internal class LinkServiceTestData
        {
            public static IEnumerable<TestCaseData> AddLinkTxns
            {
                get
                {
                    const string linkId = "linktodataelement";
                    const string targetElementAddress = "/myplace/mydataelement";

                    yield return new TestCaseData(
                        delegate (IIoTCore testiot)
                        {
                            if (testiot is null)
                                throw new ArgumentNullException(nameof(testiot));
                            new IoTCoreManagementProfileBuilder(new ProfileBuilderConfiguration(testiot, testiot.Root.Address)).Build();
                            testiot.HandleRequest(
                                new MessageConverter().Deserialize(
                                    JObject.Parse($@"{{
                                    'cid': 1,
                                    'code': 10,
                                    'adr': '/iotcore_management/addlink',
                                    'data': {{'adr': '/', 'identifier': '{linkId}', 'target_adr': '{targetElementAddress}', 'persist': true}} 
                                     }}").ToString())
                                );
                        }
                    ).SetName("{m}_viaAddLinkService_targetadr");

                    yield return new TestCaseData(
                        delegate (IIoTCore testiot)
                        {
                            if (testiot is null)
                                throw new ArgumentNullException(nameof(testiot));
                            new IoTCoreManagementProfileBuilder(new ProfileBuilderConfiguration(testiot, testiot.Root.Address)).Build();
                            testiot.HandleRequest(
                                new MessageConverter().Deserialize(
                                    JObject.Parse($@"{{
                                    'cid': 1,
                                    'code': 10,
                                    'adr': '/iotcore_management/addlink',
                                    'data': {{'adr': '/', 'identifier': '{linkId}', 'target_adr': '{targetElementAddress}', 'persist': true}} 
                                     }}").ToString()
                                        )
                                );
                        }
                    ).SetName("{m}_viaAddLinkService_target_adr");
                }
            }
            public static IEnumerable<TestCaseData> RemoveLinkTxns
            {
                get
                {
                    const string linkId = "linktodataelement";
                    const string linkElementAddress = "/linktodataelement";
                    const string targetElementAddress = "/myplace/mydataelement";

                    yield return new TestCaseData(
                        delegate (IIoTCore testiot)
                        {
                            if (testiot is null)
                                throw new ArgumentNullException(nameof(testiot));
                            new IoTCoreManagementProfileBuilder(new ProfileBuilderConfiguration(testiot, testiot.Root.Address)).Build();
                            testiot.HandleRequest(
                                new MessageConverter().Deserialize(
                                    JObject.Parse($@"{{
                                    'cid': 1,
                                    'code': 10,
                                    'adr': '/iotcore_management/removelink',
                                    'data': {{'adr': '/', 'identifier': '{linkId}', 'target_adr': '{targetElementAddress}', 'persist': true}} 
                                     }}").ToString()
                                        )
                                );
                        }
                    ).SetName("{m}_viaRemoveLinkService");
                }
            }
        }

        internal string newid() { return Guid.NewGuid().ToString("N"); }


        [Test]
        [TestCaseSource(typeof(LinkServiceTestData), nameof(LinkServiceTestData.AddLinkTxns))]
        public void AccessedViaLink_TargetElementService(Action<IIoTCore> AddLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("root_"+newid());
            const int mydata_value = 42;
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement", value: mydata_value));

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            const string targetElementAddress = "/myplace/mydataelement";
            AddLinkTransaction(testiot);

            // Then: target element service can be accessed via link similar to directly accessing target element service
            int valueViaTargetElement = testiot.HandleRequest(1, targetElementAddress + "/getdata").Data.AsVariantObject()["value"].ToObject<int>();
            Assert.That(valueViaTargetElement, Is.EqualTo(mydata_value));
            Assert.That(testiot.HandleRequest(2, "/" + linkId + "/getdata").Code, Is.EqualTo(Messages.ResponseCodes.Success), "link element or its service, not found");
            int valueViaLinkElement = testiot.HandleRequest(2, "/" + linkId + "/getdata").Data.AsVariantObject()["value"].ToObject<int>();
            Assert.That(valueViaLinkElement, Is.EqualTo(valueViaTargetElement));
        }

        [Test]
        [TestCaseSource(typeof(LinkServiceTestData), nameof(LinkServiceTestData.AddLinkTxns))]
        public void gettree_outputs_LinkAs_ChildOfSourceElement_ItsLinkFieldSetTo_TargetElementAddress(Action<IIoTCore> AddLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("root_"+newid());
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement"));

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            const string targetElementAddress = "/myplace/mydataelement";
            AddLinkTransaction(testiot);
            Assert.NotNull(testiot.GetElementByAddress("/" + linkId));

            // Then: gettree outputs link element in subs of Root element, with expected identifier and a 'link' field set to target element's address 
            Assert.That(testiot.HandleRequest(1, "/gettree").ToJToken().SelectTokens(
                $"$.data.subs..[?(@.identifier == '{linkId}' && @.link == '{targetElementAddress}')]").ToList().Count(),
                Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource(typeof(LinkServiceTestData), nameof(LinkServiceTestData.RemoveLinkTxns))]
        public void RemoveLink_RemovesLinkElement_NoGettreeOutput(Action<IIoTCore> RemoveLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("root_"+newid());
            const string linkId = "linktodataelement";
            const string targetElementAddress = "/myplace/mydataelement";
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement"));

            // Given: iot core Root element has a link pointing to data element. 
            testiot.Root.AddLink(testiot.GetElementByAddress(targetElementAddress), linkId);
            Assert.NotNull(testiot.GetElementByAddress("/" + linkId), "expected AddLink to work");

            // When: a link element is removed in Root element
            RemoveLinkTransaction(testiot);

            // Then: link element is no longer accessible
            Assert.Null(testiot.GetElementByAddress("/" + linkId), "expected removelink to work");
            Assert.That(testiot.HandleRequest(1, "/"+linkId+"/getdata").Code, Is.EqualTo(Messages.ResponseCodes.NotFound), "expected removelink to work");

            // Then: gettree output does not contain the link element
            Assert.That(testiot.HandleRequest(1, "/gettree").ToJToken().SelectTokens(
                $"$.data.subs..[?(@.identifier == '{linkId}' && @.link == '{targetElementAddress}')]").ToList().Count(),
                Is.EqualTo(0));
        }
    }
}
