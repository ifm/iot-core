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
            var testInstance = IoTCoreFactory.Create("testIoTCore", null);
            Assert.That(testInstance is IIoTCore);
        }

        [Test, Property("TestCaseKey", "IOTCS-T2")]
        public void IIoTCore_Creates_IDeviceElement_As_Root()
        {
            var testInstance = IoTCoreFactory.Create("testIot1", null);
            Assert.That(testInstance.Root is IDeviceElement);
            Assert.That(testInstance.Root.Identifier == "testIot1");
        }

        [Test, Property("TestCaseKey", "IOTCS-T2")]
        public void IIoTCore_Gets_VersionString_IsNonZero_AndMatches_getidentity()
        {
            var myiotcore = IoTCoreFactory.Create("testIot1", null);
            Assert.Multiple(() =>
            {
                Assert.That(myiotcore.Version, Is.TypeOf(typeof(string)));
                Assert.IsFalse("0.0.0".Equals(myiotcore.Version.Trim(), 
                    StringComparison.InvariantCultureIgnoreCase), 
                    "Version 0.0.0 unexpected");

                var getIdentityResponse = myiotcore.HandleRequest(0, "/getidentity", null).Data;
                Assert.IsTrue(
                    myiotcore.Version.Equals(
                        getIdentityResponse.SelectToken("$.iot.version").ToObject<string>(), 
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
                testInstances.Add(IoTCoreFactory.Create("testIot" + i, null));

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
                IoTCoreFactory.Create(basicElementIds[0], null).Root,
                new StructureElement(null, basicElementIds[1]),
                new ActionServiceElement(null, basicElementIds[2], null),
                new DataElement<object>(null, basicElementIds[3]),
                new EventElement(null, null, null, null, basicElementIds[4])
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
                IoTCoreFactory.Create(basicElementIds[0], null).Root,
                new StructureElement(null, basicElementIds[1]),
                new ActionServiceElement(null, basicElementIds[2], null),
                new DataElement<object>(null, basicElementIds[3]),
                new EventElement(null, null, null, null, basicElementIds[4])
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
            IBaseElement testElement = new StructureElement(null, "struct0");
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
            var iiotcore1 = IoTCoreFactory.Create("device0", null);
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
                new ActionServiceElement(null, "service1", null), 
                new GetterServiceElement<int>(null, "service2", null),
                new SetterServiceElement<int>(null, "service3", null), };

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
            IDataElement testElement = new DataElement<object>(null, "data0", null, null, createGetDataServiceElement: true, createSetDataServiceElement: true);

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
            IEventElement testElement = new EventElement(null, null, null, null, "event0");

            // Then - every event element interface defines specified members - Subscribe, Unsubscribe, RaiseEvent. 
            string[] expectedMembers = { "Subscribe", "Unsubscribe", "RaiseEvent"};
            var ElementMembers = testElement.GetType().GetMembers();
            var actualMember = from n in ElementMembers select n.Name;
            foreach (var expectedMember in expectedMembers)
                Assert.That(actualMember, Has.Member(expectedMember));
        }
    }
}
