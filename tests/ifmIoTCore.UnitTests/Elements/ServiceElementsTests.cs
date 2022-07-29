using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements;
    using NUnit.Framework;
    using Messages;
    using Newtonsoft.Json.Linq;

    [TestFixture]
    class ServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_UserData1UserData2()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest =null;
            var service = new ServiceElement("ServiceElement_InOut_UserData1UserData2",
                (sender, inputarg, cid) => { argtest = inputarg;  return Variant.FromObject(new complexData());  });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/ServiceElement_InOut_UserData1UserData2", Variant.FromObject(new complexData())));
            
            // Then
            Assert.That(Variant.ToObject<complexData>((VariantObject)argtest), Is.EqualTo(new complexData()));
            Assert.That(Variant.ToObject<complexData>(response.Data), Is.EqualTo(new complexData()));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_BoolString()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest =null;
            var service = new ServiceElement("ServiceElement_InOut_BoolString",
                (sender, inputarg, cid) => { argtest = inputarg;  return new VariantValue("Forty Two!");  });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/ServiceElement_InOut_BoolString", Variant.FromObject(true)));
            
            // Then
            Assert.That((bool)((VariantValue)argtest), Is.EqualTo(true));
            Assert.That((string)(VariantValue)response.Data, Is.EqualTo("Forty Two!"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_IntInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest =null;
            var service = new ServiceElement("ServiceElement_InOut_IntInt",
                (sender, inputarg, cid) => { argtest = inputarg;  return new VariantValue(43);  });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/ServiceElement_InOut_IntInt", Variant.FromObject(42)));
            
            // Then
            Assert.That((int)(VariantValue)argtest, Is.EqualTo(42));
            Assert.That((int)(VariantValue)response.Data, Is.EqualTo(43));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ActionServiceElement_Invoked_NoInput_NoOutput()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            var serviceInvoked =false;
            Assert.IsFalse(serviceInvoked);
            var service = new ActionServiceElement("actionService",
                (sender,cid) => { serviceInvoked = true; });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(0, "/struct1/actionService");
            
            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That(response.Data, Is.Null);
        }
    }
}
