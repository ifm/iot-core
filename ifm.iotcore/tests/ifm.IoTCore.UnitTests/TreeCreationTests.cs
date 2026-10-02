namespace ifm.IoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Common.Exceptions;
    using ElementManager.Contracts.Elements;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

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
            var ioTCore = Helpers.CreateIoTCore("testIot1");
            Assert.Throws<ArgumentNullException>(() =>
            {
                ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "");
            }); 

            Assert.Throws<ArgumentException>(() =>
            {
                ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "/");
            }); 

            Assert.Throws<ArgumentException>(() =>
            {
                ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "//");
            });

            Assert.Throws<ArgumentException>(() =>
            {
                ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1 ");
            }); 

            Assert.Throws<ArgumentException>(() =>
            {
                ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1/struct2");
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void Element_AddingChild_MatchingAddress_ThrowsException()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var testElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var subElement = ioTCore.ElementManager.CreateStructureElement(testElement, "struct1");
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => 
            {  // new instance (same type and same identifier)
                ioTCore.ElementManager.CreateStructureElement(testElement, "struct1");
            });
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => 
            {  // new instance (different type and same identifier)
                ioTCore.ElementManager.CreateSimpleDataElement<object>(testElement, "struct1");
            });
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void Element_AddingChild_SameIdentifier_ThrowsException()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var testElement = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct0");
            var subElement = ioTCore.ElementManager.CreateStructureElement(testElement, "struct0");

            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => 
            { 
                ioTCore.ElementManager.CreateSimpleDataElement<object>(testElement, "struct0");
            });
        }


        /// Remove Element Tests
        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_MakesElementNServices_InaccessibleFromTree()
        {
            // Given: iot tree created with an element (dataelement)
            var testiotcore = Helpers.CreateIoTCore("testiot");
            var dataelement = testiotcore.ElementManager.CreateSimpleDataElement(testiotcore.Root, "data1", value: "helloworld");

            // Given: data element is available in tree and accessible (getdata) 
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{dataelement.Identifier}')]").ToList().Count(), Is.EqualTo(1));
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/data1/getdata").Data.ToJToken().Value<string>("value"), Is.EqualTo("helloworld"));

            // When: element is removed using RemoveElement
            testiotcore.ElementManager.RemoveElement(testiotcore.Root, dataelement);

            // Then: data element is not available in iot tree, nor accessible through its service
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{dataelement.Identifier}')]").ToList().Count(), Is.EqualTo(0));
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/data1/getdata").Code, Is.EqualTo((int)ResponseCodes.NotFound));
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_Removes_SubscribedHandlers_NoMoreInvoked()
        {
            // Given: iot tree created with a dataelement
            var testiotcore = Helpers.CreateIoTCore("testiot");
            int getdataCalled = 0, setdataCalled = 0; 
            const int onlyOnce = 1;
            // Given: datalement has handlers for both services: getdata and setdata
            var dataelement = testiotcore.ElementManager.CreateDataElement(testiotcore.Root, identifier: "data1",
                (sender, i) => { getdataCalled++; return "data1/getdata"; },
                (sender, value, i) => { setdataCalled++; }, createDataChangedEventElement:true);
            // Given: dataelement has handler for its event: datachanged event 
            // Given: handlers are invoked when element is available and accessed
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/data1/getdata").Code, Is.EqualTo((int)ResponseCodes.Success));
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/data1/setdata", VariantConverter.FromJToken(JToken.Parse("{'newvalue':'/data1/setdata'}"))).Code, Is.EqualTo((int)ResponseCodes.Success));
            //dataelement.DataChangedEventElement.RaiseEvent(); // explicit event raise not required as datachanged event implicitly raised on setdata
            Assert.That(getdataCalled, Is.EqualTo(onlyOnce));
            Assert.That(setdataCalled, Is.EqualTo(onlyOnce));

            // When: element is removed using RemoveElement
            testiotcore.ElementManager.RemoveElement(testiotcore.Root, dataelement, true);

            // Then: getdata, setdata service handlers are not accessible 
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/data1/getdata").Code, Is.EqualTo((int)ResponseCodes.NotFound));
            Assert.That(testiotcore.MessageDispatcher.HandleRequest(1, $"/data1/setdata", VariantConverter.FromJToken(JToken.Parse("{'newvalue':'/data1/setdata'}"))).Code, Is.EqualTo((int)ResponseCodes.NotFound));
            // Then: getdata, setdata service handlers and datachanged event are not invoked
            Assert.That(getdataCalled, Is.EqualTo(onlyOnce));
            Assert.That(setdataCalled, Is.EqualTo(onlyOnce));
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_Recursive_MakesElementChildren_InaccessibleFromTree()
        {
            // Given: iot tree with hierarchy of (structure) elements
            var testIoT = Helpers.CreateIoTCore("testiot");
            const int elementsToBeCreated = 10, elementsToBeTested = 3;
            Assert.That(elementsToBeTested, Is.LessThan(elementsToBeCreated));

            var elementsCreated = new List<(IBaseElement, IBaseElement)>();
            var parentRe = testIoT.Root;
            for (var i = 0; i < elementsToBeCreated; i++)
            {
                var elementRe = testIoT.ElementManager.CreateStructureElement(parentRe, Guid.NewGuid().ToString("N"));
                elementsCreated.Add((parentRe, elementRe)); // using tuple as quick-n-dirty type, item1 - parent, item2 - element
                parentRe = elementRe;
            }

            for (var i = 0; i < elementsToBeCreated - elementsToBeTested; i++)
            { // keep few random elements in the list for 'RemoveElement' test
                elementsCreated.RemoveAt(new Random().Next(0, elementsCreated.Count));
            }
            for (var j = elementsCreated.Count-1; j > 0; j--)
            { // for remaining elements, do RemoveElement for parent element and check if parent and children are removed
                var parent = elementsCreated[j].Item1;
                var elementToBeRemoved = elementsCreated[j].Item2;
                Assert.That(testIoT.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{elementToBeRemoved.Identifier}')]").ToList().Count(), Is.EqualTo(1));
                testIoT.ElementManager.RemoveElement(parent, elementToBeRemoved);
                for (var k = j; k < elementsCreated.Count; k++)
                { // check all child elements are removed from the tree
                    var deletedElement = elementsCreated[k].Item2;
                    Assert.That(testIoT.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{deletedElement.Identifier}')]").ToList().Count(), Is.EqualTo(0));
                }
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T212")]
        public void RemoveElement_Invalid_ParentElement_Rejected()
        {
            // Given: iot tree created with an element (dataelement)
            var testIoT = Helpers.CreateIoTCore("testiot");
            var grandparent = testIoT.ElementManager.CreateStructureElement(testIoT.Root, "struct1"); 
            var parent = testIoT.ElementManager.CreateStructureElement(grandparent, "struct2");
            var theElement = testIoT.ElementManager.CreateSimpleDataElement(parent, "data1", value: "/struct1/struct2/data1");

            Assert.That(testIoT.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{theElement.Identifier}')]").ToList().Count(), Is.EqualTo(1));

            // When: RemoveElement is used with invalid args
            // Then: RemoveElement rejects with exception
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testIoT.ElementManager.RemoveElement(theElement, theElement));
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testIoT.ElementManager.RemoveElement(testIoT.Root, theElement));
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testIoT.ElementManager.RemoveElement(grandparent, theElement));
            Assert.Throws(Is.InstanceOf(typeof(IoTCoreException)), () => testIoT.ElementManager.RemoveElement(theElement, parent));

            // Then: Element remains available in the tree
            Assert.That(testIoT.MessageDispatcher.HandleRequest(1, "/gettree").Data.ToJToken().SelectTokens($"$..[?(@.identifier == '{theElement.Identifier}')]").ToList().Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateSubTree_CheckAddresses_Success()
        {
            var iotcore = Helpers.CreateIoTCore("iotcore");

            var loc1 = iotcore.ElementManager.CreateStructureElement(iotcore.Root, "loc1");
            var loc2 = iotcore.ElementManager.CreateStructureElement(iotcore.Root, "loc2");

            var level1 = iotcore.ElementManager.CreateStructureElement(null, "level1");
            var level2 = iotcore.ElementManager.CreateStructureElement(level1, "level2");
            var level3 = iotcore.ElementManager.CreateStructureElement(level2, "level3");
            var level4 = iotcore.ElementManager.CreateStructureElement(level3, "level4");
            var level5 = iotcore.ElementManager.CreateSimpleDataElement(level4, "level5", 0);
            var level61 = iotcore.ElementManager.GetElementByIdentifier(level5, "getdata");
            var level62 = iotcore.ElementManager.GetElementByIdentifier(level5, "setdata");

            Assert.That(level1.Address, Is.EqualTo("level1"));
            Assert.That(level2.Address, Is.EqualTo("level1/level2"));
            Assert.That(level3.Address, Is.EqualTo("level1/level2/level3"));
            Assert.That(level4.Address, Is.EqualTo("level1/level2/level3/level4"));
            Assert.That(level5.Address, Is.EqualTo("level1/level2/level3/level4/level5"));
            Assert.That(level61.Address, Is.EqualTo("level1/level2/level3/level4/level5/getdata"));
            Assert.That(level62.Address, Is.EqualTo("level1/level2/level3/level4/level5/setdata"));

            iotcore.ElementManager.AddElement(loc1, level1);

            Assert.That(level1.Address, Is.EqualTo("iotcore/loc1/level1"));
            Assert.That(level2.Address, Is.EqualTo("iotcore/loc1/level1/level2"));
            Assert.That(level3.Address, Is.EqualTo("iotcore/loc1/level1/level2/level3"));
            Assert.That(level4.Address, Is.EqualTo("iotcore/loc1/level1/level2/level3/level4"));
            Assert.That(level61.Address, Is.EqualTo("iotcore/loc1/level1/level2/level3/level4/level5/getdata"));
            Assert.That(level62.Address, Is.EqualTo("iotcore/loc1/level1/level2/level3/level4/level5/setdata"));

            iotcore.ElementManager.RemoveElement(loc1, level1);

            Assert.That(level1.Address, Is.EqualTo("level1"));
            Assert.That(level2.Address, Is.EqualTo("level1/level2"));
            Assert.That(level3.Address, Is.EqualTo("level1/level2/level3"));
            Assert.That(level4.Address, Is.EqualTo("level1/level2/level3/level4"));
            Assert.That(level61.Address, Is.EqualTo("level1/level2/level3/level4/level5/getdata"));
            Assert.That(level62.Address, Is.EqualTo("level1/level2/level3/level4/level5/setdata"));

            iotcore.ElementManager.AddElement(loc2, level1);

            Assert.That(level1.Address, Is.EqualTo("iotcore/loc2/level1"));
            Assert.That(level2.Address, Is.EqualTo("iotcore/loc2/level1/level2"));
            Assert.That(level3.Address, Is.EqualTo("iotcore/loc2/level1/level2/level3"));
            Assert.That(level4.Address, Is.EqualTo("iotcore/loc2/level1/level2/level3/level4"));
            Assert.That(level61.Address, Is.EqualTo("iotcore/loc2/level1/level2/level3/level4/level5/getdata"));
            Assert.That(level62.Address, Is.EqualTo("iotcore/loc2/level1/level2/level3/level4/level5/setdata"));

            iotcore.ElementManager.RemoveElement(loc2, level1);

            Assert.That(level1.Address, Is.EqualTo("level1"));
            Assert.That(level2.Address, Is.EqualTo("level1/level2"));
            Assert.That(level3.Address, Is.EqualTo("level1/level2/level3"));
            Assert.That(level4.Address, Is.EqualTo("level1/level2/level3/level4"));
            Assert.That(level61.Address, Is.EqualTo("level1/level2/level3/level4/level5/getdata"));
            Assert.That(level62.Address, Is.EqualTo("level1/level2/level3/level4/level5/setdata"));
        }
    }
}