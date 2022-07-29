using ifmIoTCore.Common.Variant;
using ifmIoTCore.Elements.ServiceData.Responses;
using Microsoft.VisualBasic;

namespace ifmIoTCore.UnitTests.Elements
{
    using System;
    using System.Collections.Generic;

    using ifmIoTCore.Elements;
    using NUnit.Framework;
    using System.Linq;
    using System.Collections;

    [TestFixture]
    class BasicElementCreationTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T2")]
        public void IIoTCore_CreatedBy_IoTCoreFactory()
        {
            var testInstance = IoTCoreFactory.Create("testIoTCore");
            Assert.That(testInstance is IIoTCore);
        }

        [Test, Property("TestCaseKey", "IOTCS-T2")]
        public void IIoTCore_Creates_IDeviceElement_As_Root()
        {
            var testInstance = IoTCoreFactory.Create("testIot1");
            Assert.That(testInstance.Root is IDeviceElement);
            Assert.That(testInstance.Root.Identifier == "testIot1");
        }

        [Test, Property("TestCaseKey", "IOTCS-T2")]
        public void IIoTCore_Gets_VersionString_IsNonZero_AndMatches_getidentity()
        {
            var myiotcore = IoTCoreFactory.Create("testIot1");
            Assert.Multiple(() =>
            {
                Assert.That(myiotcore.Version, Is.TypeOf(typeof(string)));
                Assert.IsFalse("0.0.0".Equals(myiotcore.Version.Trim(), 
                    StringComparison.InvariantCultureIgnoreCase), 
                    "Version 0.0.0 unexpected");

                var getIdentityResponse = Variant.ToObject<GetIdentityResponseServiceData>(myiotcore.HandleRequest(0, "/getidentity", null).Data);
                Assert.IsTrue(
                    myiotcore.Version.Equals(
                        getIdentityResponse.IoT.Version, 
                        StringComparison.InvariantCultureIgnoreCase),
                    "ioTCore version mismatches with getidentity version");
            });
        }

        [Test, Property("TestCaseKey", "unittests")]
        public void IIoTCore_Instance_Multiple_IoTRoots_AreCreated()
        {
            const int MAX_ROOTS = 100;
            var testInstances = new List<IIoTCore>();
            for (var i = 0; i < MAX_ROOTS; i++)
                testInstances.Add(IoTCoreFactory.Create("testIot" + i));

            for (var i = 0; i < MAX_ROOTS; i++)
            {
                var tI = testInstances[i];
                Assert.That(tI is IIoTCore);
                Assert.That(tI.Root is IDeviceElement);
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T1")]
        public void Element_Common_Properties_DefinedBy_IBaseElement()
        {
            string[] expectedElementProperties = { "Type", "Identifier", "Format", "Profiles", "UId", "Subs" };
            var ElementMembers = typeof(IBaseElement).GetMembers();
            var actualProperties = from n in ElementMembers select n.Name;
            foreach (var expectedProperty in expectedElementProperties)
                Assert.That(actualProperties, Has.Member(expectedProperty));
        }

        [Test, Property("TestCaseKey", "IOTCS-T1")]
        public void Element_Common_Properties_NOT_DefinedBy_IBaseElement()
        {
            string[] expectedElementProperties = { "Acl" };
            var ElementMembers = typeof(IBaseElement).GetMembers();
            var actualProperties = from n in ElementMembers select n.Name;
            Assert.Multiple(() =>
            {
                foreach (var expectedProperty in expectedElementProperties)
                    Assert.That(actualProperties, Has.No.Member(expectedProperty));
            });
        }

        [Test, Property("TestCaseKey", "unittests")]
        public void Element_IsConstructedWith_Identfier()
        {
            string[] basicElementIds = { "device0", "struct0", "service0", "data0", "event0" };
            IBaseElement[] basicElements = {
                IoTCoreFactory.Create(basicElementIds[0]).Root,
                new StructureElement(basicElementIds[1]),
                new ActionServiceElement(basicElementIds[2], null),
                new DataElement<object>(basicElementIds[3], null,null,false ,null, null,null,null,null,false),
                new EventElement(basicElementIds[4])
            };

            for (var i = 0; i < basicElementIds.Length; i++)
                Assert.That(basicElements[i].Identifier, Is.EqualTo(basicElementIds[i]));
        }

        [Test, Property("TestCaseKey", "IOTCS-T1")]
        public void Element_Common_Properties_AvailableWhen_Instantiated()
        {
            string[] expectedElementProperties = { "Type", "Identifier", "Format", "Profiles", "UId", "Subs" };
            string[] basicElementIds = { "device0", "struct0", "service0", "data0", "event0" };
            IBaseElement[] basicElements = {
                IoTCoreFactory.Create(basicElementIds[0]).Root,
                new StructureElement(basicElementIds[1]),
                new ActionServiceElement(basicElementIds[2], null),
                new DataElement<object>(basicElementIds[3], null,null,false ,null, null,null,null,null,false),
                new EventElement(basicElementIds[4])
            };

            foreach (var elementInstance in basicElements)
            {
                var ElementMembers = elementInstance.GetType().GetMembers();
                var actualProperties = from n in ElementMembers select n.Name;
                foreach (var expectedProperty in expectedElementProperties)
                    Assert.That(actualProperties, Has.Member(expectedProperty));
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T4")]
        public void StructureElement_IsCreatedBy_ElementFactory()
        {
            IBaseElement testElement = new StructureElement("struct0");
            Assert.That(testElement is IStructureElement);
        }

        internal static  List<string> DeviceElementExpectedMembers = new List<string>{
                "GetIdentity",
                "GetTree", "QueryTree",
                "GetDataMulti", "SetDataMulti",
                "GetSubscriberList"
                //"AddElement", "RemoveElement",
                //"GetElementInfo", "SetelementInfo",
                //"AddProfile", "RemoveProfile",
                //"Mirror", "Unmirror",
            };

        [Test, Property("TestCaseKey", "IOTCS-T5")]
        public void DeviceElement_HasRequired_ChildElements_IsCreatedBy_IoTCoreFactory_Indirectly()
        {
            var iiotcore1 = IoTCoreFactory.Create("device0");
            var testElement = (IBaseElement)iiotcore1.Root;
            Assert.That(testElement is IDeviceElement);
            // check all required fields are available. see test case
            var expectedMembers = DeviceElementExpectedMembers;

            var ElementMembers = testElement.GetType().GetMembers();
            var actualMember = from n in ElementMembers select n.Name;
            foreach (var expectedMember in expectedMembers)
                Assert.That(actualMember, Has.Member(expectedMember));
        }

        [Test, Property("TestCaseKey","IOTCS-T6")] 
        public void IServiceElement_DefinesMember_Invoke()
        {
            // Given  - create service element types without device tree, internal api access
            List<IServiceElement> AllServiceElementTypes = new List<IServiceElement> { 
                new ActionServiceElement("service1", null), 
                new GetterServiceElement("service2", null),
                new SetterServiceElement("service3", null,null), };

            // Then - every service element interface defines specified members - Invoke.
            foreach (var testElement in AllServiceElementTypes)
            {
                string[] expectedMembers = { "Invoke" };
                var ElementMembers = testElement.GetType().GetMembers();
                var actualMember = from n in ElementMembers select n.Name;
                foreach (var expectedMember in expectedMembers)
                    Assert.That(actualMember, Has.Member(expectedMember));
            }
        }

        [Test, Property("TestCaseKey","IOTCS-T7")] 
        public void IDataElement_DefinesMembers_Value_DataChangedEventElement()
        {
            // Given  - create data element types without device tree, internal api access
            IDataElement testElement = new DataElement<object>("data0", null, null, false, null, null, null, null, null, false);

            // Then - every data element interface defines specified members - Value, DataChangedEventElement.
            string[] expectedMembers = { "Value", "DataChangedEventElement" };
            var ElementMembers = testElement.GetType().GetMembers();
            var actualMember = from n in ElementMembers select n.Name;
            foreach (var expectedMember in expectedMembers)
                Assert.That(actualMember, Has.Member(expectedMember));
        }

        [Test, Property("TestCaseKey","IOTCS-T8")] 
        public void IEventElement_DefinesMembers_Subscribe_Unsubscribe_RaiseEvent()
        {
            // Given  - create data element types without device tree, internal api access
            IEventElement testElement = new EventElement("event0");

            // Then - every event element interface defines specified members - Subscribe, Unsubscribe, RaiseEvent. 
            string[] expectedMembers = { "Subscribe", "Unsubscribe", "RaiseEvent"};
            var ElementMembers = testElement.GetType().GetMembers();
            var actualMember = from n in ElementMembers select n.Name;
            foreach (var expectedMember in expectedMembers)
                Assert.That(actualMember, Has.Member(expectedMember));
        }

        [Test]
        public void DataElement_Ensure_GetData_SetData_Service_In_Subs()
        {
            var iotCore = IoTCoreFactory.Create("test");
            var dataElement = new DataElement<object>("data1", element => new object(), (element, o) => { });
            iotCore.Root.AddChild(dataElement, true);

            var getDataService =  dataElement.Subs.SingleOrDefault(x => x.Identifier == Identifiers.GetData);
            var setDataService = dataElement.Subs.SingleOrDefault(x => x.Identifier == Identifiers.SetData);

            Assert.NotNull(getDataService);
            Assert.NotNull(setDataService);

            var getDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/getdata");
            var setDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/setdata");

            Assert.NotNull(getDataServiceByGetElementByAddressMethod, "GetData service not found in subs.");
            Assert.NotNull(setDataServiceByGetElementByAddressMethod, "SetData service not found in subs.");
        }

        [Test]
        public void DataElement_Ensure_GetData_SetData_Service_Found_By_GetElementByAddress()
        {
            var iotCore = IoTCoreFactory.Create("test");
            var dataElement = new DataElement<object>("data1", element => new object(), (element, o) => { /* ignore */ });
            iotCore.Root.AddChild(dataElement, true);

            var getDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/getdata");
            var setDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/setdata");

            Assert.NotNull(getDataServiceByGetElementByAddressMethod, "GetData Service not found.");
            Assert.NotNull(setDataServiceByGetElementByAddressMethod, "SetData Service not found.");
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void ReadOnlyDataElement_Ensure_GetData_Service_Found_By_GetElementByAddress()
        {
            var iotCore = IoTCoreFactory.Create("test");

            var dataElement = new ReadOnlyDataElement<object>("data1", element => new object());
            iotCore.Root.AddChild(dataElement, true);

            var getDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/getdata");
            var setDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/setdata");

            Assert.NotNull(getDataServiceByGetElementByAddressMethod, "GetData Service not found.");
            Assert.Null(setDataServiceByGetElementByAddressMethod, "SetData Service found unexpectedly.");
        }

        [Test, Property("TestCaseKey", "IOTCS-T7")]
        public void WriteOnlyDataElement_Ensure_SetData_Service_Found_By_GetElementByAddress()
        {
            var iotCore = IoTCoreFactory.Create("test");

            var dataElement = new WriteOnlyDataElement<object>("data1", (element, o) => { /* ignore */ });
            iotCore.Root.AddChild(dataElement, true);

            var getDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/getdata");
            var setDataServiceByGetElementByAddressMethod = iotCore.GetElementByAddress("/data1/setdata");

            Assert.Null(getDataServiceByGetElementByAddressMethod, "GetData Service found unexpectedly.");
            Assert.NotNull(setDataServiceByGetElementByAddressMethod, "SetData Service not found.");
        }


        [Test]
        public void DataElementValueConsistencyObject()
        {
            var iotCore = IoTCoreFactory.Create("test");

            var data = new object();
            var dataElement = new DataElement<object>("dataElement", element => data, (element, o) => { data = o; });
            iotCore.Root.AddChild(dataElement);

            var nonGenericDataElement = (IDataElement) dataElement;

            Assert.AreEqual(dataElement.Value, nonGenericDataElement.Value);
        }

        [Ignore("There type consistencies don´t work anymore with the Variant mechanism. Please refactor or delete test.")]
        [Test]
        public void DataElementTypeConsistencyObject()
        {
            var iotCore = IoTCoreFactory.Create("test");

            var data = new object();
            var dataElement = new DataElement<object>("dataElement", element => data, (element, o) => { data = o; });
            iotCore.Root.AddChild(dataElement);

            Assert.AreEqual(data.GetType(), dataElement.Value.GetType());
        }

        [Test]
        public void DataElementValueConsistencyString()
        {
            var iotCore = IoTCoreFactory.Create("test");

            var data = "Hello";
            var dataElement = new DataElement<string>("dataElement", element => data, (element, o) => { data = o; });
            iotCore.Root.AddChild(dataElement);

            var nonGenericDataElement = (IDataElement)dataElement;

            Assert.AreEqual(dataElement.Value, nonGenericDataElement.Value);
        }

        [Test]
        public void DataElementTypeConsistencyString()
        {
            var iotCore = IoTCoreFactory.Create("test");

            var data = "Hello";
            var dataElement = new DataElement<string>("dataElement", element => data, (element, o) => { data = o; });
            iotCore.Root.AddChild(dataElement);

            Assert.AreEqual(VariantValue.ValueType.String, dataElement.Value.AsVariantValue().Type);
        }

        [Ignore("This test is deactivated now for the refactoring of the datamodel. Afterwards it may be activated to make the dataelement type more accurate.")]
        [Test]
        public void TestIDataElementPropertyGetterAccessibility()
        {
            var value = "Hello";
            var writeOnlyValue = "WriteOnly";

            var dataElements = new List<IDataElement>
            {
                new DataElement<string>("data1", element => value, (element, s) => { value = s;}),
                new WriteOnlyDataElement<string>("data2", (element, s) => { writeOnlyValue = s;} )
            };

            foreach (var item in dataElements)
            {
                Assert.DoesNotThrow(() =>
                {
                    // Access getter of property does not throw
                    var value = item.Value;
                });
            }
        }
    }
}
