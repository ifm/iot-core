namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements;
    using ifmIoTCore.Common.Variant;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System.Linq;
    using Exceptions;
    using System.Collections.Generic;
    using System;

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
                        testiot.Root.AddLink(testiot.GetElementByAddress(targetElementAddress), linkId);
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
                        testiot.Root.RemoveLink(testiot.GetElementByAddress(linkElementAddress));
                    }
                ).SetName("{m}_viaRemoveLinkApi_GetLinkElement");

                yield return new TestCaseData(
                    delegate (IIoTCore testiot)
                    {
                        if (testiot is null)
                            throw new ArgumentNullException(nameof(testiot));
                        testiot.Root.RemoveLink(testiot.GetElementByAddress(targetElementAddress));
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
            using var ioTCore = IoTCoreFactory.Create("io0");
            var struct0 = ioTCore.Root.AddChild(new StructureElement("struct0"), true);
            var struct1 = ioTCore.Root.AddChild(new StructureElement("struct1"), true);
            var struct2 = ioTCore.Root.AddChild(new StructureElement("struct2"), true);

            // Act
            struct0.AddLink(struct1, null, true);
            struct1.AddLink(struct2, null, true);

            // Assert

            // Check references struct0 -> struct1
            var reference = struct0.References.ForwardReferences.First(x => x.TargetElement == struct1);
            Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Forward);
            reference = struct1.References.InverseReferences.First(x => x.SourceElement == struct0);
            Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Inverse);

            // Check references struct1 -> struct2
            reference = struct1.References.ForwardReferences.First(x => x.TargetElement == struct2);
            Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Forward);
            reference = struct2.References.InverseReferences.First(x => x.SourceElement == struct1);
            Assert.That(reference.Type == ReferenceType.Link && reference.Direction == ReferenceDirection.Inverse);

            // Check link addresses
            var element = ioTCore.GetElementByAddress("/struct0/struct1");
            Assert.That(element != null);
            Assert.AreEqual(element, struct1);

            element = ioTCore.GetElementByAddress("/struct1/struct2");
            Assert.That(element != null);
            Assert.AreEqual(element, struct2);

            element = ioTCore.GetElementByAddress("/struct0/struct1/struct2");
            Assert.That(element != null);
            Assert.AreEqual(element, struct2);

            //Check remove links
            struct0.RemoveLink(struct1, true);
            struct1.RemoveLink(struct2, true);

            element = ioTCore.GetElementByAddress("/struct0/struct1");
            Assert.That(element == null);

            element = ioTCore.GetElementByAddress("/struct1/struct2");
            Assert.That(element == null);

            element = ioTCore.GetElementByAddress("/struct0/struct1/struct2");
            Assert.That(element == null);
        }

        [Test]
        public void AddChild_Twice_Throws()
        {
            var struct1 = new StructureElement("struct1");

            // Add a child element
            var child1 = new StructureElement("child");
            struct1.AddChild(child1);

            // Add element to different parent is not allowed
            var struct2 = new StructureElement("struct2");
            Assert.Throws<IoTCoreException>(() => struct2.AddChild(child1));

            // Add same element twice is not allowed
            Assert.Throws<IoTCoreException>(() => struct1.AddChild(child1));

            // Add different element with same identifier is not allowed
            var child2 = new StructureElement("child");
            Assert.Throws<IoTCoreException>(() => struct1.AddChild(child2));

            // Add ancestor element is not allowed
            Assert.Throws<IoTCoreException>(() => child1.AddChild(struct1));
        }

        [Test]
        public void AddLink_Twice_Throws()
        {
            using var ioTCore = IoTCoreFactory.Create("io0");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1);

            struct0.AddLink(struct1, "01");

            Assert.Throws<IoTCoreException>(() => struct0.AddLink(struct1, "01"));
        }

        [Test]
        public void AddLink_CircularDependency_Throws()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var struct0 = new StructureElement("struct0");
            ioTCore.Root.AddChild(struct0);
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1);
            var struct2 = new StructureElement("struct2");
            ioTCore.Root.AddChild(struct2);

            struct0.AddLink(struct1, "linkToStruct1");
            struct1.AddLink(struct2);

            Assert.Throws<IoTCoreException>(() => struct2.AddLink(struct1));
            Assert.Throws<IoTCoreException>(() => struct2.AddLink(struct0));
            Assert.Throws<IoTCoreException>(() => struct0.AddLink(ioTCore.Root));
        }


        [Test]
        public void AddAndRemove_ElementsAndLinksTest_Success()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var a = ioTCore.Root.AddChild(new StructureElement("a"), true);
            var a1 = a.AddChild(new StructureElement("a1"), true);
            var b = ioTCore.Root.AddChild(new StructureElement("b"), true);
            var c = ioTCore.Root.AddChild(new StructureElement("c"), true);
            var c1 = c.AddChild(new StructureElement("c1"), true);
            var c2 = c1.AddChild(new StructureElement("c2"), true);

            a1.AddLink(b, null, true);
            c.AddLink(a, null, true);
            b.AddLink(c2, null, true);

            var element = ioTCore.GetElementByAddress("/a/a1/b");
            Assert.That(element != null);
            Assert.AreEqual(element, b);

            element = ioTCore.GetElementByAddress("/c/a/a1/b");
            Assert.That(element != null);
            Assert.AreEqual(element, b);

            element = ioTCore.GetElementByAddress("/b/c2");
            Assert.That(element != null);
            Assert.AreEqual(element, c2);

            var b1 = b.AddChild(new StructureElement("b1"), true);
            element = ioTCore.GetElementByAddress("/c/a/a1/b/b1");
            Assert.That(element != null);
            Assert.AreEqual(element, b1);

            a1.RemoveLink(b, true);
            element = ioTCore.GetElementByAddress("/a/a1/b");
            Assert.That(element == null);
            element = ioTCore.GetElementByAddress("/a/a1/b/b1");
            Assert.That(element == null);
            element = ioTCore.GetElementByAddress("/c/a/a1/b");
            Assert.That(element == null);
            element = ioTCore.GetElementByAddress("/c/a/a1/b/b1");
            Assert.That(element == null);

            element = ioTCore.GetElementByAddress("/b/b1");
            Assert.That(element != null);

            c1.Parent.RemoveChild(c1, true);
            element = ioTCore.GetElementByAddress("/c/c1/c11");
            Assert.That(element == null);
            element = ioTCore.GetElementByAddress("/b/c11");
            Assert.That(element == null);
        }

        [Test]
        public void TryGenerateSameAddressForLinkElement_Throws()
        {
            var ioTCore = IoTCoreFactory.Create("id0");
            var a = new StructureElement("a");
            ioTCore.Root.AddChild(a);

            var b = new StructureElement("b");
            ioTCore.Root.AddChild(b);
            var c = new StructureElement("c");
            ioTCore.Root.AddChild(c);

            const string identifier = "MyIdentifier";

            a.AddLink(b, identifier);


            Assert.Throws<IoTCoreException>(() => a.AddLink(c, identifier));


        }

        [Test]
        [TestCaseSource(typeof(LinkApiTestData), nameof(LinkApiTestData.AddLinkTxns))]
        public void AccessedViaLink_TargetElementService(Action<IIoTCore> AddLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("testiotcore");
            const int mydata_value = 42;
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement", value: mydata_value));

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            const string targetElementAddress = "/myplace/mydataelement";
            AddLinkTransaction(testiot);

            // Then: target element service can be accessed via link similar to directly accessing target element service
            int valueViaTargetElement = testiot.HandleRequest(1, targetElementAddress + "/getdata").Data.AsVariantObject()["value"].ToObject<int>();
            Assert.That(valueViaTargetElement, Is.EqualTo(mydata_value));
            int valueViaLinkElement = testiot.HandleRequest(2, "/" + linkId + "/getdata").Data.AsVariantObject()["value"].ToObject<int>();
            Assert.That(valueViaLinkElement, Is.EqualTo(valueViaTargetElement));
        }

        [Test]
        [TestCaseSource(typeof(LinkApiTestData), nameof(LinkApiTestData.AddLinkTxns))]
        public void gettree_outputs_LinkAs_ChildOfSourceElement_ItsLinkFieldSetTo_TargetElementAddress(Action<IIoTCore> AddLinkTransaction)
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("root_"+newid());
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement"));

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            const string targetElementAddress = "/myplace/mydataelement";
            AddLinkTransaction(testiot);

            // Then: gettree outputs link element in subs of Root element, with expected identifier and a 'link' field set to target element's address 
            Assert.That(testiot.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                $"$.subs..[?(@.identifier == '{linkId}' && @.link == '{targetElementAddress}')]").ToList().Count(),
                Is.EqualTo(1));
        }

        [Test]
        [Ignore("Waiting to be reviewed/fixed, Ignored Until...", Until = "2022-07-30 12:00:00Z")]
        public void gettree_outputs_LinkElement_WithAddressField_Having_SingleSeperatorWithIdentifier()
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("testiotcore");
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement"));

            // When: a link to data element is created in Root element
            const string linkId = "linktodataelement";
            testiot.Root.AddLink(testiot.GetElementByAddress("/myplace/mydataelement"), linkId);

            // Then: gettree request, outputs link element with 'adr' field containing its identifier
            var tree = testiot.HandleRequest(1, "/gettree", new VariantObject() { { "expand_links", new VariantValue(true) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            var linkElementJToken = testiot.HandleRequest(1, "/gettree", new VariantObject() { { "expand_links", new VariantValue(true) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            Assert.Multiple(() =>
            {
                Assert.That((string)linkElementJToken.SelectToken("$.adr"), Is.EqualTo("/" + linkId));
                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?@.identifier=='getdata')]")["adr"], Is.EqualTo("/" + linkId + "/getdata"));
                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?@.identifier=='setdata')]")["adr"], Is.EqualTo("/" + linkId + "/setdata"));
            });
        }

        [Test]
        public void gettree_expand_links_true_OutputsLinkAsTargetElement()
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("testiotcore");
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement"));

            // When: a link to data element is created in Root element
            const string linkId = "linktodataelement";
            testiot.Root.AddLink(testiot.GetElementByAddress("/myplace/mydataelement"), linkId);

            // Then: gettree request with expand_links=true, outputs link element expanded, similar to target element, matching type, format and subs
            var linkElementJToken = testiot.HandleRequest(1, "/gettree", new VariantObject() { { "expand_links", new VariantValue(true) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            var targetElementJToken = testiot.HandleRequest(1, "/gettree", new VariantObject() { { "adr", new VariantValue("/myplace/mydataelement") } }).Data.ToJToken();
            Assert.Multiple(() =>
            {
                Assert.AreEqual((string)targetElementJToken.SelectToken("$.type"), "data");
                Assert.AreEqual((string)linkElementJToken.SelectToken("$.type"), (string)targetElementJToken.SelectToken("$.type"));

                Assert.AreEqual((string)targetElementJToken.SelectToken("$.format.type"), "number");
                Assert.AreEqual((string)linkElementJToken.SelectToken("$.format.type"), (string)targetElementJToken.SelectToken("$.format.type"));

                Assert.AreEqual((string)linkElementJToken.SelectToken("$.format.encoding"), "integer");
                Assert.AreEqual((string)linkElementJToken.SelectToken("$.format.encoding"), (string)targetElementJToken.SelectToken("$.format.encoding"));

                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?(@.identifier=='getdata' && @.type=='service')]")["link"],
                    Is.EqualTo("/myplace/mydataelement/getdata"), "getdata element not found or 'link' field value mismatch");
                Assert.That((string)linkElementJToken.SelectToken("$.subs..[?(@.identifier=='setdata' && @.type=='service')]")["link"],
                    Is.EqualTo("/myplace/mydataelement/setdata"), "setdata element not found or 'link' field value mismatch");
            });

        }

        [Test]
        [Ignore("Waiting to be reviewed/fixed, Ignored Until...", Until = "2022-07-30 12:00:00Z")]
        public void gettree_expand_links_false_default_OutputsLinkElementAs_TypeUnknown_NoFormat_NoSubs()
        {
            // Given: iot core instance with a data element inside a struct element
            var testiot = IoTCoreFactory.Create("testiotcore");
            testiot.Root.AddChild(new StructureElement("myplace")).AddChild(new DataElement<int>("mydataelement"));

            // When: a link element is created in Root element
            const string linkId = "linktodataelement";
            testiot.Root.AddLink(testiot.GetElementByAddress("/myplace/mydataelement"), linkId);

            // Then: gettree request with expand_links=false, outputs minimal link element  
            var linkElementJToken = testiot.HandleRequest(1, "/gettree", new VariantObject() { { "expand_links", new VariantValue(false) } }).Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == '{linkId}')]");
            Assert.Multiple(() =>
            {
                Assert.That((string)linkElementJToken["type"], Is.EqualTo("unknown"));
                Assert.IsNull(linkElementJToken.SelectToken("$.format"));
                Assert.IsNull(linkElementJToken.SelectToken("$.subs"));
            });
        }

        [Test]
        public void AddingLink_WithIdentifier_MatchingExistingSubs_RaisesException_IoTCoreException()
        {
            // Given: iotcore instance with 2 nested struct elements /element1/element11
            var testiot = IoTCoreFactory.Create("root_"+newid());
            testiot.Root.AddChild(new StructureElement("element1")).AddChild(new StructureElement("element11"));

            // When: a link is added to root element, pointing to element11 however having same identifier as existing element
            // Then: adding link with existing identifier in the subs is not allowed - raising exception IoTCoreException
            Assert.Multiple(() => 
            {
               Assert.Throws<ifmIoTCore.Exceptions.IoTCoreException>(() => testiot.Root.AddLink(testiot.GetElementByAddress("/element1/element11"), "element1"));

               var MatchingIdResponse = testiot.HandleRequest(new MessageConverter.Json.Newtonsoft.MessageConverter().Deserialize(
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
            var testiot = IoTCoreFactory.Create("testIoTCore");
            var element = testiot.Root.AddChild(new StructureElement("singleElement"));
            Assert.Throws<ifmIoTCore.Exceptions.IoTCoreException>(() => element.AddLink(element, "newlink"));
            Assert.Throws<ifmIoTCore.Exceptions.IoTCoreException>(() => testiot.Root.AddLink(testiot.Root, "rootlink"));
        }

        [Test]
        public void LinkToLink_IsAllowed()
        {
            // Given: iotcore instance with 3 nested struct elements /parent/child/grandchild
            var testiot = IoTCoreFactory.Create("testIoTCore");
            testiot.Root.AddChild(new StructureElement("parent")).AddChild(new StructureElement("child")).AddChild(new StructureElement("grandchild"));
            // Given: a link is added to parent element, pointing to GrandChild (i.e. /parent/linkToGrandChild -> /parent/child/granchild  )
            testiot.GetElementByAddress("/parent").AddLink(testiot.GetElementByAddress("/parent/child/grandchild"), "linkToGrandChild");

            // When: a link is added to root element, pointing to existing link of Grand Child (i.e. /directLinkToGrandChild -> /parent/linkToGranChild)
            // Then: adding link to existing link is successful, does not raise any exception
            Assert.DoesNotThrow(() => testiot.Root.AddLink(testiot.GetElementByAddress("/parent/linkToGrandChild"), "directLinkToGrandChild"));

            // Then: both links point to the same element
            Assert.That(testiot.GetElementByAddress("/parent/linkToGrandChild"), Is.SameAs(testiot.GetElementByAddress("/parent/child/grandchild")));
            Assert.That(testiot.GetElementByAddress("/directLinkToGrandChild"), Is.SameAs(testiot.GetElementByAddress("/parent/linkToGrandChild")));
        }

        [Test]
        [Ignore("Waiting to be reviewed/fixed, Ignored Until...", Until = "2022-07-30 12:00:00Z")]
        public void AccessingStaleLink_IsAllowed()
        {
            // Given: iotcore instance with 3 nested struct elements /parent/child/grandchild
            var testiot = IoTCoreFactory.Create("testIoTCore");
            var parent = new StructureElement("parent");
            var child = new StructureElement("child");
            var grandchild = new DataElement<string>("grandchild", value: "grandchild");
            testiot.Root.AddChild(parent).AddChild(child).AddChild(grandchild);

            // Given: a link is added to Root element, pointing to GrandChild (i.e. /linkToGrandChild -> /parent/child/granchild  )
            testiot.Root.AddLink(grandchild, "linkToGrandChild");

            // When: The element, pointed by the link, is removed
            child.RemoveChild(testiot.GetElementByAddress("/parent/child/grandchild"));
            Assert.IsNull(testiot.GetElementByAddress("/parent/child/grandchild"));

            // Then: target element is not accessible
            Assert.That(testiot.HandleRequest(2, "/parent/child/grandchild/getdata").Code, Is.EqualTo(Messages.ResponseCodes.NotFound));

            // Then: stale link is available, however cannot access target element via link
            Assert.NotNull(testiot.HandleRequest(3, "/gettree").Data.ToJToken().SelectToken($"$.subs..[?(@.identifier == 'linkToGrandChild')]"));
            Assert.That(testiot.HandleRequest(4, "/linkToGrandChild/getdata").Code, Is.EqualTo(Messages.ResponseCodes.NotFound));
        }

        [Test]
        public void RemoveLink_withIdentifier()
        {
            // Given: iot core instance with tree: /a/a1/a2, /b
            var testiot = IoTCoreFactory.Create("root_"+newid());
            var a2 = testiot.Root.AddChild(new StructureElement("a")).AddChild(new StructureElement("a1")).AddChild(new StructureElement("a2"));
            var b = testiot.Root.AddChild(new StructureElement("b"));

            // Given: link created without identifier. /b/a2* -> /a/a1/a2
            b.AddLink(a2);
            Assert.NotNull(testiot.GetElementByAddress("/b/a2"));
            Assert.AreEqual(a2, testiot.GetElementByAddress("/b/a2"));
            // When: link is removed
            b.RemoveLink(a2);
            // Then: link element is no longer accessible
            Assert.Null(testiot.GetElementByAddress("/b/a2"));

            // Given: link created with identifier. /b/newlink* -> /a/a1/a2
            b.AddLink(a2, "newlink");
            Assert.NotNull(testiot.GetElementByAddress("/b/newlink"));
            Assert.AreEqual(a2, testiot.GetElementByAddress("/b/newlink"));
            // When: a link is removed
            Assert.DoesNotThrow(() => b.RemoveLink(a2));
            // Then: link element is no longer accessible
            Assert.Null(testiot.GetElementByAddress("/b/newlink"));
        }

        [Test]
        public void RemoveLink_RemovesLinkElement_NoGettreeOutput()
        {
            // Given: iot core instance with tree: /a/a1/a2, /b
            var testiot = IoTCoreFactory.Create("root_"+newid());
            var a2 = testiot.Root.AddChild(new StructureElement("a")).AddChild(new StructureElement("a1")).AddChild(new DataElement<int>("a2"));
            var b = testiot.Root.AddChild(new StructureElement("b"));

            // Given: link created. /b/a2* -> /a/a1/a2
            b.AddLink(a2);
            Assert.AreEqual(a2, testiot.GetElementByAddress("/b/a2"));
            Assert.That(testiot.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                $"$.subs..[?(@.identifier == 'a2' && @.link == '/a/a1/a2')]").ToList().Count(),
                Is.EqualTo(1));
            
            // When: link is removed
            b.RemoveLink(a2);
            // Then: link element is no longer accessible
            Assert.Null(testiot.GetElementByAddress("/b/a2"));
            Assert.That(testiot.HandleRequest(1, "/b/a2/getdata").Code, Is.EqualTo(Messages.ResponseCodes.NotFound));
            // Then: gettree output does not contain the link element
            Assert.That(testiot.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens(
                $"$.subs..[?(@.identifier == 'a2' && @.link == '/a/a1/a2')]").ToList().Count(),
                Is.EqualTo(0));

        }
    }

}
