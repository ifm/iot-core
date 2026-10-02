namespace ifm.IoTCore.UnitTests.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using ifm.Common.Tree;
    using ifm.Common.Variant;
    using Common;
    using Common.Exceptions;
    using Contracts;
    using MessageConverter.Json;

    public class LinkApiTestData
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
                        testiot.ElementManager.AddLink(testiot.Root, testiot.ElementManager.GetElementByAddress(targetElementAddress), linkId);
                    }
                ).SetName("{m}_viaAddLinkApi");
            }
        }

        public static IEnumerable<TestCaseData> RemoveLinkTxns
        {
            get
            {
                const string linkId = "linktodataelement";
                const string linkElementAddress = "/" + linkId;
                const string targetElementAddress = "/myplace/mydataelement";

                yield return new TestCaseData(
                    delegate (IIoTCore testiot)
                    {
                        if (testiot is null)
                            throw new ArgumentNullException(nameof(testiot));
                        testiot.ElementManager.RemoveLink(testiot.Root, testiot.ElementManager.GetElementByAddress(linkElementAddress));
                    }
                ).SetName("{m}_viaRemoveLinkApi_GetLinkElement");

                yield return new TestCaseData(
                    delegate (IIoTCore testiot)
                    {
                        if (testiot is null)
                            throw new ArgumentNullException(nameof(testiot));
                        testiot.ElementManager.RemoveLink(testiot.Root, testiot.ElementManager.GetElementByAddress(targetElementAddress));
                    }
                ).SetName("{m}_viaRemoveLinkApi_GetTargetElement");
            }
        }

    }

    [TestFixture]
    public class LinkElementsTests
    {
        string newid() { return Guid.NewGuid().ToString("N"); }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AddLink_CheckRelationsAndAddresses_Success()
        {
            // Arrange
            var ioTCore = Helpers.CreateIoTCore("io0");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var struct2 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct2");

            // Act
            ioTCore.ElementManager.AddLink(struct0, struct1, null, true);
            ioTCore.ElementManager.AddLink(struct1, struct2, null, true);

            // Assert

            // Check references struct0 -> struct1
            var reference = struct0.ForwardReferences.First(x => x.TargetNode == struct1);
            Assert.That(reference.Type == ReferenceTypes.Link && reference.Direction == ReferenceDirections.Forward);
            reference = struct1.InverseReferences.First(x => x.SourceNode == struct0);
            Assert.That(reference.Type == ReferenceTypes.Link && reference.Direction == ReferenceDirections.Inverse);

            // Check references struct1 -> struct2
            reference = struct1.ForwardReferences.First(x => x.TargetNode == struct2);
            Assert.That(reference.Type == ReferenceTypes.Link && reference.Direction == ReferenceDirections.Forward);
            reference = struct2.InverseReferences.First(x => x.SourceNode == struct1);
            Assert.That(reference.Type == ReferenceTypes.Link && reference.Direction == ReferenceDirections.Inverse);

            // Check link addresses
            var element = ioTCore.ElementManager.GetElementByAddress("/struct0/struct1");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(struct1));
            
            element = ioTCore.ElementManager.GetElementByAddress("/struct1/struct2");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(struct2));

            element = ioTCore.ElementManager.GetElementByAddress("/struct0/struct1/struct2");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(struct2));

            //Check remove links
            ioTCore.ElementManager.RemoveLink(struct0, struct1);
            ioTCore.ElementManager.RemoveLink(struct1, struct2);

            element = ioTCore.ElementManager.GetElementByAddress("/struct0/struct1");
            Assert.That(element == null);

            element = ioTCore.ElementManager.GetElementByAddress("/struct1/struct2");
            Assert.That(element == null);

            element = ioTCore.ElementManager.GetElementByAddress("/struct0/struct1/struct2");
            Assert.That(element == null);
        }

        [Test]
        public void AddChild_Twice_Throws()
        {
            var ioTCore = Helpers.CreateIoTCore("io0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");

            // Add a child element
            var child1 = ioTCore.ElementManager.CreateStructureElement(struct1, "child");

            // Add element to different parent is not allowed
            var struct2 = ioTCore.ElementManager.CreateStructureElement(null, "struct2");
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddElement(struct2, child1));

            // Add same element twice is not allowed
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddElement(struct1, child1));

            // Add different element with same identifier is not allowed
            var child2 = ioTCore.ElementManager.CreateStructureElement(null, "child");
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddElement(struct1, child2));

            // Add ancestor element is not allowed
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddElement(child1, struct1));
        }

        [Test]
        public void AddLink_Twice_Throws()
        {
            var ioTCore = Helpers.CreateIoTCore("io0");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(null, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(null, "struct1");

            ioTCore.ElementManager.AddLink(struct0, struct1, "01");

            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddLink(struct0, struct1, "01"));
        }

        [Test]
        public void AddLink_CircularDependency_Throws()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var struct0 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var struct2 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct2");

            ioTCore.ElementManager.AddLink(struct0, struct1, "linkToStruct1");
            ioTCore.ElementManager.AddLink(struct1, struct2);

            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddLink(struct2, struct1));
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddLink(struct2, struct0));
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddLink(struct0, ioTCore.Root));
        }


        [Test]
        public void AddAndRemove_ElementsAndLinksTest_Success()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var a = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "a");
            var a1 = ioTCore.ElementManager.CreateStructureElement(a, "a1");
            var b = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "b");
            var c = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "c");
            var c1 = ioTCore.ElementManager.CreateStructureElement(c, "c1");
            var c2 = ioTCore.ElementManager.CreateStructureElement(c1, "c2");

            ioTCore.ElementManager.AddLink(a1, b, null, true);
            ioTCore.ElementManager.AddLink(c, a, null, true);
            ioTCore.ElementManager.AddLink(b, c2, null, true);

            var element = ioTCore.ElementManager.GetElementByAddress("/a/a1/b");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(b));

            element = ioTCore.ElementManager.GetElementByAddress("/c/a/a1/b");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(b));

            element = ioTCore.ElementManager.GetElementByAddress("/b/c2");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(c2));

            var b1 = ioTCore.ElementManager.CreateStructureElement(b, "b1");
            element = ioTCore.ElementManager.GetElementByAddress("/c/a/a1/b/b1");
            Assert.That(element != null);
            Assert.That(element, Is.EqualTo(b1));

            ioTCore.ElementManager.RemoveLink(a1, b);
            element = ioTCore.ElementManager.GetElementByAddress("/a/a1/b");
            Assert.That(element == null);
            element = ioTCore.ElementManager.GetElementByAddress("/a/a1/b/b1");
            Assert.That(element == null);
            element = ioTCore.ElementManager.GetElementByAddress("/c/a/a1/b");
            Assert.That(element == null);
            element = ioTCore.ElementManager.GetElementByAddress("/c/a/a1/b/b1");
            Assert.That(element == null);

            element = ioTCore.ElementManager.GetElementByAddress("/b/b1");
            Assert.That(element != null);

            ioTCore.ElementManager.RemoveLink(b, c2);
            ioTCore.ElementManager.RemoveElement(c1.Parent, c1, true);
            element = ioTCore.ElementManager.GetElementByAddress("/c/c1/c11");
            Assert.That(element == null);
            element = ioTCore.ElementManager.GetElementByAddress("/b/c11");
            Assert.That(element == null);
        }

        [Test]
        public void TryGenerateSameAddressForLinkElement_Throws()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var a = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "a");

            var b = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "b");
            var c = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "c");

            const string identifier = "MyIdentifier";

            ioTCore.ElementManager.AddLink(a, b, identifier);


            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => ioTCore.ElementManager.AddLink(a, c, identifier));


        }

        [Test]
        [TestCaseSource(typeof(LinkApiTestData), nameof(LinkApiTestData.AddLinkTxns))]
        public void AccessedViaLink_TargetElementService(Action<IIoTCore> AddLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = Helpers.CreateIoTCore("testiotcore");
            const int mydata_value = 42;
            var myplace = testiot.ElementManager.CreateStructureElement(testiot.Root, "myplace");
            testiot.ElementManager.CreateSimpleDataElement(myplace, "mydataelement", value: mydata_value);

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            const string targetElementAddress = "/myplace/mydataelement";
            AddLinkTransaction(testiot);

            // Then: target element service can be accessed via link similar to directly accessing target element service
            var valueViaTargetElement = testiot.MessageDispatcher.HandleRequest(1, targetElementAddress + "/getdata").Data.AsVariantObject()["value"].ToObject<int>();
            Assert.That(valueViaTargetElement, Is.EqualTo(mydata_value));
            var valueViaLinkElement = testiot.MessageDispatcher.HandleRequest(2, "/" + linkId + "/getdata").Data.AsVariantObject()["value"].ToObject<int>();
            Assert.That(valueViaLinkElement, Is.EqualTo(valueViaTargetElement));
        }

        [Test]
        [TestCaseSource(typeof(LinkApiTestData), nameof(LinkApiTestData.AddLinkTxns))]
        public void gettree_outputs_LinkAs_ChildOfSourceElement_ItsLinkFieldSetTo_TargetElementAddress(Action<IIoTCore> AddLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var coreName = "root_" + newid();
            var testiot = Helpers.CreateIoTCore(coreName);
            var myplace = testiot.ElementManager.CreateStructureElement(testiot.Root, "myplace");
            testiot.ElementManager.CreateSimpleDataElement<int>(myplace, "mydataelement");

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            var targetElementAddress = coreName + "/myplace/mydataelement";
            AddLinkTransaction(testiot);

            // Then: gettree outputs link element in subs of Root element, with expected identifier and a 'link' field set to target element's address 
            Assert.That(testiot.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                $"$.subs..[?(@.identifier == '{linkId}' && @.link == '{targetElementAddress}')]").ToList().Count(),
                Is.EqualTo(1));
        }

        [Test]
        public void gettree_outputs_LinkElement_WithAddressField_Having_SingleSeperatorWithIdentifier()
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = Helpers.CreateIoTCore("testiotcore");
            var myplace = testiot.ElementManager.CreateStructureElement(testiot.Root, "myplace");
            testiot.ElementManager.CreateSimpleDataElement<int>(myplace, "mydataelement");

            // When: a link to data element is created in Root element
            const string linkId = "linktodataelement";
            testiot.ElementManager.AddLink(testiot.Root, testiot.ElementManager.GetElementByAddress("/myplace/mydataelement"), linkId);

            // Then: gettree request, outputs link element with 'adr' field containing its identifier
            var linkElementJToken = testiot.MessageDispatcher.HandleRequest(1, "/gettree", new VariantObject { { "expand_links", new VariantValue(true) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            Assert.Multiple(() =>
            {
                Assert.That((string)linkElementJToken.SelectToken("$.adr"), Is.EqualTo("testiotcore/" + linkId));
                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?(@.identifier=='getdata')]")["adr"], Is.EqualTo("testiotcore/" + linkId + "/getdata"));
                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?(@.identifier=='setdata')]")["adr"], Is.EqualTo("testiotcore/" + linkId + "/setdata"));
            });
        }

        [Test]
        public void gettree_expand_links_true_OutputsLinkAsTargetElement()
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = Helpers.CreateIoTCore("testiotcore");
            var myplace = testiot.ElementManager.CreateStructureElement(testiot.Root, "myplace");
            testiot.ElementManager.CreateSimpleDataElement<int>(myplace, "mydataelement");

            // When: a link to data element is created in Root element
            const string linkId = "linktodataelement";
            testiot.ElementManager.AddLink(testiot.Root, testiot.ElementManager.GetElementByAddress("/myplace/mydataelement"), linkId);

            // Then: gettree request with expand_links=true, outputs link element expanded, similar to target element, matching type, format and subs
            var linkElementJToken = testiot.MessageDispatcher.HandleRequest(1, "/gettree", new VariantObject { { "expand_links", new VariantValue(true) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            var targetElementJToken = testiot.MessageDispatcher.HandleRequest(1, "/gettree", new VariantObject { { "adr", new VariantValue("/myplace/mydataelement") } }).Data.ToJToken();
            Assert.Multiple(() =>
            {
                Assert.That((string)targetElementJToken.SelectToken("$.type"), Is.EqualTo("data"));
                Assert.That((string)linkElementJToken.SelectToken("$.type"), Is.EqualTo((string)targetElementJToken.SelectToken("$.type")));

                Assert.That((string)targetElementJToken.SelectToken("$.format.type"), Is.EqualTo("number"));
                Assert.That((string)linkElementJToken.SelectToken("$.format.type"), Is.EqualTo((string)targetElementJToken.SelectToken("$.format.type")));

                Assert.That((string)linkElementJToken.SelectToken("$.format.encoding"), Is.EqualTo("int32"));
                Assert.That((string)linkElementJToken.SelectToken("$.format.encoding"), Is.EqualTo((string)targetElementJToken.SelectToken("$.format.encoding")));

                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?(@.identifier=='getdata' && @.type=='service')]")["link"],
                    Is.EqualTo("testiotcore/myplace/mydataelement/getdata"), "getdata element not found or 'link' field value mismatch");
                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?(@.identifier=='setdata' && @.type=='service')]")["link"],
                    Is.EqualTo("testiotcore/myplace/mydataelement/setdata"), "setdata element not found or 'link' field value mismatch");
            });

        }

        [Test]
        public void gettree_expand_links_false_default_OutputsLinkElementAs_TypeUnknown_NoFormat_NoSubs()
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = Helpers.CreateIoTCore("testiotcore");
            var myplace = testiot.ElementManager.CreateStructureElement(testiot.Root, "myplace");
            testiot.ElementManager.CreateSimpleDataElement<int>(myplace, "mydataelement");

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            testiot.ElementManager.AddLink(testiot.Root, testiot.ElementManager.GetElementByAddress("/myplace/mydataelement"), linkId);

            // Then: gettree request with expand_links=false, outputs minimal link element  
            var linkElementJToken = testiot.MessageDispatcher.HandleRequest(1, "/gettree", new VariantObject { { "expand_links", new VariantValue(false) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            Assert.Multiple(() =>
            {
                Assert.That((string)linkElementJToken["type"], Is.EqualTo("unknown"));
                Assert.That(linkElementJToken.SelectToken("$.format"), Is.Null);
                Assert.That(linkElementJToken.SelectToken("$.subs"), Is.Null);
            });
        }

        [Test]
        public void AddingLink_WithIdentifier_MatchingExistingSubs_RaisesException_IoTCoreException()
        {
            // Given: iotcore instance with 2 nested struct elements /element1/element11
            var testiot = Helpers.CreateIoTCore("root_" + newid());
            var element1 = testiot.ElementManager.CreateStructureElement(testiot.Root, "element1");
            testiot.ElementManager.CreateStructureElement(element1, "element11");

            // When: a link is added to root element, pointing to element11 however having same identifier as existing element
            // Then: adding link with existing identifier in the subs is not allowed - raising exception IoTCoreException
            Assert.Multiple(() =>
            {
                Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testiot.ElementManager.AddLink(testiot.Root, testiot.ElementManager.GetElementByAddress("/element1/element11"), "element1"));

                var MatchingIdResponse = testiot.MessageDispatcher.HandleRequest(new MessageConverter().Deserialize(
                    JObject.Parse($@"{{
                    'cid': 1,
                    'code': 10,
                    'adr': '/iotcore_management/addlink',
                    'data': {{'adr': '/', 'identifier': 'element1', 'targetadr': '/element1/element11'}} 
                     }}").ToString()));
            });
        }

        [Test]
        public void Link_SameSourceAndTarget_RaisesException_IoTCoreException()
        {
            // Given: iotcore instance with single element
            var testiot = Helpers.CreateIoTCore("testIoTCore");
            var element = testiot.ElementManager.CreateStructureElement(testiot.Root, "singleElement");
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testiot.ElementManager.AddLink(element, element, "newlink"));
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testiot.ElementManager.AddLink(testiot.Root, testiot.Root, "rootlink"));
        }

        [Test]
        public void LinkToLink_IsAllowed()
        {
            // Given: iotcore instance with 3 nested struct elements /parent/child/grandchild
            var testiot = Helpers.CreateIoTCore("testIoTCore");
            var parent = testiot.ElementManager.CreateStructureElement(testiot.Root, "parent");
            var child = testiot.ElementManager.CreateStructureElement(parent, "child");
            var grandchild = testiot.ElementManager.CreateStructureElement(child, "grandchild");
            // Given: a link is added to parent element, pointing to GrandChild (i.e. /parent/linkToGrandChild -> /parent/child/granchild  )
            testiot.ElementManager.AddLink(parent, grandchild, "linkToGrandChild");

            // When: a link is added to root element, pointing to existing link of Grand Child (i.e. /directLinkToGrandChild -> /parent/linkToGranChild)
            // Then: adding link to existing link is successful, does not raise any exception
            Assert.DoesNotThrow(() => testiot.ElementManager.AddLink(testiot.Root, testiot.ElementManager.GetElementByAddress("/parent/linkToGrandChild"), "directLinkToGrandChild"));

            // Then: both links point to the same element
            Assert.That(testiot.ElementManager.GetElementByAddress("/parent/linkToGrandChild"), Is.SameAs(testiot.ElementManager.GetElementByAddress("/parent/child/grandchild")));
            Assert.That(testiot.ElementManager.GetElementByAddress("/directLinkToGrandChild"), Is.SameAs(testiot.ElementManager.GetElementByAddress("/parent/linkToGrandChild")));
        }

        [Test]
        public void RemovedLinkedElement_Throws()
        {
            // Given: iotcore instance with 3 nested struct elements /parent/child/grandchild
            var testiot = Helpers.CreateIoTCore("testIoTCore");
            var parent = testiot.ElementManager.CreateStructureElement(testiot.Root, "parent");
            var child = testiot.ElementManager.CreateStructureElement(parent, "child");
            var grandchild = testiot.ElementManager.CreateSimpleDataElement(child, "grandchild", value: "grandchild");
            
            // Given: a link is added to Root element, pointing to GrandChild (i.e. /linkToGrandChild -> /parent/child/granchild  )
            testiot.ElementManager.AddLink(testiot.Root, grandchild, "linkToGrandChild");

            // When: The element, pointed by the link, is removed
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testiot.ElementManager.RemoveElement(child, testiot.ElementManager.GetElementByAddress("/parent/child/grandchild")));
        }

        [Test]
        public void RemoveLink_withIdentifier()
        {
            // Given: iot core instance with tree: /a/a1/a2, /b
            var testiot = Helpers.CreateIoTCore("root_"+newid());
            var a = testiot.ElementManager.CreateStructureElement(testiot.Root, "a");
            var a1 = testiot.ElementManager.CreateStructureElement(a, "a1");
            var a2 = testiot.ElementManager.CreateStructureElement(a1, "a2");
            var b = testiot.ElementManager.CreateStructureElement(testiot.Root, "b");

            // Given: link created without identifier. /b/a2* -> /a/a1/a2
            testiot.ElementManager.AddLink(b, a2);
            Assert.That(testiot.ElementManager.GetElementByAddress("/b/a2"),Is.Not.Null);
            Assert.That(a2, Is.EqualTo(testiot.ElementManager.GetElementByAddress("/b/a2")));
            // When: link is removed
            testiot.ElementManager.RemoveLink(b, a2);
            // Then: link element is no longer accessible
            Assert.That(testiot.ElementManager.GetElementByAddress("/b/a2"),Is.Null);

            // Given: link created with identifier. /b/newlink* -> /a/a1/a2
            testiot.ElementManager.AddLink(b, a2, "newlink");
            Assert.That(testiot.ElementManager.GetElementByAddress("/b/newlink"),Is.Not.Null);
            Assert.That(a2, Is.EqualTo(testiot.ElementManager.GetElementByAddress("/b/newlink")));
            // When: a link is removed
            Assert.DoesNotThrow(() => testiot.ElementManager.RemoveLink(b, a2));
            // Then: link element is no longer accessible
            Assert.That(testiot.ElementManager.GetElementByAddress("/b/newlink"),Is.Null);
        }

        [Test]
        public void RemoveLink_RemovesLinkElement_NoGettreeOutput()
        {
            // Given: iot core instance with tree: /a/a1/a2, /b
            var testiot = Helpers.CreateIoTCore("iotcore");
            var a = testiot.ElementManager.CreateStructureElement(testiot.Root, "a");
            var a1 = testiot.ElementManager.CreateStructureElement(a, "a1");
            var a2 = testiot.ElementManager.CreateSimpleDataElement<int>(a1, "a2");
            var b = testiot.ElementManager.CreateStructureElement(testiot.Root, "b");

            // Given: link created. /b/a2* -> /a/a1/a2
            testiot.ElementManager.AddLink(b, a2);
            Assert.That(a2,Is.EqualTo(testiot.ElementManager.GetElementByAddress("/b/a2")));
            Assert.That(testiot.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                $"$.subs..[?(@.identifier == 'a2' && @.link == 'iotcore/a/a1/a2')]").ToList().Count(),
                Is.EqualTo(1));
            
            // When: link is removed
            testiot.ElementManager.RemoveLink(b, a2);
            // Then: link element is no longer accessible
            Assert.That(testiot.ElementManager.GetElementByAddress("/b/a2"),Is.Null);
            Assert.That(testiot.MessageDispatcher.HandleRequest(1, "/b/a2/getdata").Code, Is.EqualTo((int)ResponseCodes.NotFound));
            // Then: gettree output does not contain the link element
            Assert.That(testiot.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                $"$.subs..[?(@.identifier == 'a2' && @.link == 'iotcore/a/a1/a2')]").ToList().Count(),
                Is.EqualTo(0));

            // Given: link created. /b/a2* -> /a/a1/a2
            testiot.ElementManager.AddLink(b, a2, "link1");
            Assert.That(a2,Is.EqualTo(testiot.ElementManager.GetElementByAddress("/b/link1")));
            Assert.That(testiot.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                    $"$.subs..[?(@.identifier == 'link1' && @.link == 'iotcore/a/a1/a2')]").ToList().Count(),
                Is.EqualTo(1));
        }
    }

}
