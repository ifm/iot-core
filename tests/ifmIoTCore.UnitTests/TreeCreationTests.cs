using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Newtonsoft.Json.Linq;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using NUnit.Framework;
    using ifmIoTCore.Elements;

    [TestFixture]
    public class TreeCreationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void Element_Creation_WithInvalidIdentifier_ThrowsException()
        {
            using var ioTCore = IoTCoreFactory.Create("testIot1");
            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.Root.AddChild(new StructureElement(""));
            }); 

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.Root.AddChild(new StructureElement("/"));
            }); 

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.Root.AddChild(new StructureElement("//"));
            });

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.Root.AddChild(new StructureElement("struct1 "));
            });

            Assert.Throws<IoTCoreException>(() =>
            {
                ioTCore.Root.AddChild(new StructureElement("struct1/struct2"));
            });
        }

        [Test]
        public void Element_AddingChild_Self_ThrowsException()
        {
            var testElement = new StructureElement("struct0");
            Assert.Throws<IoTCoreException>(() =>
            {
                testElement.AddChild(testElement);
            });
        }

        [Test]
        public void Element_AddingChild_Parent_ThrowsException()
        {
            var testElement = new StructureElement("struct0");
            var testElement1 = new StructureElement("struct1");
            testElement.AddChild(testElement1);
            Assert.That(() => testElement1.AddChild(testElement), Throws.Exception);
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void Element_AddingChild_MatchingAddress_ThrowsException()
        {
            using var ioTCore = IoTCoreFactory.Create("id0");
            var testElement = new StructureElement("struct0");
            ioTCore.Root.AddChild(testElement);
            var subElement = new StructureElement("struct1");
            testElement.AddChild(subElement);
            Assert.Throws<IoTCoreException>(() => 
            {  // new instance (same type and same identifier)
                testElement.AddChild(new StructureElement("struct1"));
            });
            Assert.Throws<IoTCoreException>(() => 
            {  // new instance (different type and same identifier)
                testElement.AddChild(new DataElement<object>("struct1")); 
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void Element_AddingChild_SameIdentifier_ThrowsException()
        {
            using var ioTCore = IoTCoreFactory.Create("id0");
            var testElement = new StructureElement("struct0");
            ioTCore.Root.AddChild(testElement);
            var subElement = new StructureElement("struct0");
            testElement.AddChild(subElement);

            Assert.Throws<IoTCoreException>(() => 
            { 
                testElement.AddChild(new DataElement<object>("struct0"));
            });
        }


        /// Remove Element Tests
        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_MakesElementNServices_InaccessibleFromTree()
        {
            // Given: iot tree created with an element (dataelement)
            var testiotcore = IoTCoreFactory.Create("testiot");
            var dataelement = new DataElement<string>("data1", value: "helloworld");
            testiotcore.Root.AddChild(dataelement, true);

            // Given: data element is available in tree and accessible (getdata) 
            Assert.That(testiotcore.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{dataelement.Identifier}')]").ToList().Count(), Is.EqualTo(1));
            Assert.That(testiotcore.HandleRequest(1,$"/data1/getdata").Data.ToJToken().Value<string>("value"), Is.EqualTo("helloworld"));

            // When: element is removed using RemoveElement
            testiotcore.Root.RemoveChild(dataelement, true);

            // Then: data element is not available in iot tree, nor accessible through its service
            Assert.That(testiotcore.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{dataelement.Identifier}')]").ToList().Count(), Is.EqualTo(0));
            Assert.That(testiotcore.HandleRequest(1,$"/data1/getdata").Code, Is.EqualTo(Messages.ResponseCodes.NotFound));
        }
        
        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_Removes_SubscribedHandlers_NoMoreInvoked()
        {
            // Given: iot tree created with a dataelement
            var testiotcore = IoTCoreFactory.Create("testiot");
            int getdataCalled=0, setdataCalled=0, datachangedCalled=0; const int onlyOnce = 1;
            // Given: datalement has handlers for both services: getdata and setdata
            var dataelement = new DataElement<string>(identifier: "data1",
                (sender) => { getdataCalled++; return "data1/getdata"; },
                (sender,value) => { setdataCalled++; },
                createDataChangedEventElement: true,
                value: "helloworld");
            testiotcore.Root.AddChild(dataelement, true);
            // Given: dataelement has handler for its event: datachanged event 
            dataelement.DataChangedEventElement.Subscribe(_ => datachangedCalled++);
            // Given: handlers are invoked when element is available and accessed
            Assert.That(testiotcore.HandleRequest(1,$"/data1/getdata").Code, Is.EqualTo(Messages.ResponseCodes.Success));
            Assert.That(testiotcore.HandleRequest(1,$"/data1/setdata", VariantConverter.FromJToken(JToken.Parse("{'newvalue':'/data1/setdata'}"))).Code, Is.EqualTo(Messages.ResponseCodes.Success));
            //dataelement.DataChangedEventElement.RaiseEvent(); // explicit event raise not required as datachanged event implicitly raised on setdata
            Assert.That(getdataCalled, Is.EqualTo(onlyOnce));
            Assert.That(setdataCalled, Is.EqualTo(onlyOnce));

            // When: element is removed using RemoveElement
            testiotcore.Root.RemoveChild(dataelement, true);

            // Then: getdata, setdata service handlers are not accessible 
            Assert.That(testiotcore.HandleRequest(1, $"/data1/getdata").Code, Is.EqualTo(Messages.ResponseCodes.NotFound));
            Assert.That(testiotcore.HandleRequest(1, $"/data1/setdata", VariantConverter.FromJToken(JToken.Parse("{'newvalue':'/data1/setdata'}"))).Code, Is.EqualTo(Messages.ResponseCodes.NotFound));
            // Then: getdata, setdata service handlers and datachanged event are not invoked
            Assert.That(getdataCalled, Is.EqualTo(onlyOnce));
            Assert.That(setdataCalled, Is.EqualTo(onlyOnce));
        }
        
        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_Recursive_MakesElementChildren_InaccessibleFromTree()
        {
            // Given: iot tree with hierarchy of (structure) elements
            var testIoT = IoTCoreFactory.Create("testiot");
            const int elementsToBeCreated = 10, elementsToBeTested = 3;
            Assert.Less(elementsToBeTested, elementsToBeCreated);
            List<(IBaseElement, IBaseElement)> elementsCreated = new List<(IBaseElement, IBaseElement)>();
            IBaseElement parentRe = testIoT.Root;
            for (int i = 0; i < elementsToBeCreated; i++)
            {
                var elementRe = new StructureElement(System.Guid.NewGuid().ToString("N"));
                parentRe.AddChild(elementRe);
                elementsCreated.Add((parentRe, elementRe)); // using tuple as quick-n-dirty type, item1 - parent, item2 - element
                parentRe = elementRe;
            }

            for (int i = 0; i < elementsToBeCreated - elementsToBeTested; i++)
            { // keep few random elements in the list for 'RemoveElement' test
                elementsCreated.RemoveAt(new Random().Next(0, elementsCreated.Count));
            }
            for (int j = elementsCreated.Count-1; j > 0; j--)
            { // for remaining elements, do RemoveElement for parent element and check if parent and children are removed
                var parent = elementsCreated[j].Item1;
                var elementToBeRemoved = elementsCreated[j].Item2;
                Assert.That(testIoT.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{elementToBeRemoved.Identifier}')]").ToList().Count(), Is.EqualTo(1));
                parent.RemoveChild(elementToBeRemoved);
                for (int k = j; k < elementsCreated.Count; k++)
                { // check all child elements are removed from the tree
                    var deletedElement = elementsCreated[k].Item2;
                    Assert.That(testIoT.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{deletedElement.Identifier}')]").ToList().Count(), Is.EqualTo(0));
                }
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_Invalid_ParentElement_Rejected()
        {
            // Given: iot tree created with an element (dataelement)
            var testIoT = IoTCoreFactory.Create("testiot");
            var grandparent = new StructureElement("struct1");
            testIoT.Root.AddChild(grandparent); 
            var parent = new StructureElement("struct2");
            grandparent.AddChild(parent);
            var theElement = new DataElement<string>("data1", value: "/struct1/struct2/data1");
            parent.AddChild(theElement);

            Assert.That(testIoT.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{theElement.Identifier}')]").ToList().Count(), Is.EqualTo(1));

            // When: RemoveElement is used with invalid args
            // Then: RemoveElement rejects with exception
            Assert.Throws<IoTCoreException>(() => theElement.RemoveChild(theElement));
            Assert.Throws<IoTCoreException>(() => testIoT.Root.RemoveChild(theElement));
            Assert.Throws<IoTCoreException>(() => grandparent.RemoveChild(theElement));
            Assert.Throws<IoTCoreException>(() => theElement.RemoveChild(parent));

            // Then: Element remains available in the tree
            Assert.That(testIoT.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{theElement.Identifier}')]").ToList().Count(), Is.EqualTo(1));
        }
    }
}