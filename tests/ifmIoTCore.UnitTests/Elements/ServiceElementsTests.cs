namespace ifmIoTCore.UnitTests.Elements
{
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
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest=null;
            var service = ioTCore.CreateServiceElement<complexData,complexData>(struct1,
                "ServiceElement_InOut_UserData1UserData2",
                (sender, inputarg, cid) => { argtest = inputarg;  return new complexData();  });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/ServiceElement_InOut_UserData1UserData2", JToken.FromObject(new complexData())));
            
            // Then
            Assert.That(argtest, Is.EqualTo(new complexData()));
            Assert.That(response.Data.ToObject<complexData>(), Is.EqualTo(new complexData()));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_BoolString()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest=null;
            var service = ioTCore.CreateServiceElement<bool,string>(struct1,
                "ServiceElement_InOut_BoolString",
                (sender, inputarg, cid) => { argtest = inputarg;  return "Forty Two!";  });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/ServiceElement_InOut_BoolString", JToken.FromObject(true)));
            
            // Then
            Assert.That(argtest, Is.EqualTo(true));
            Assert.That((string)response.Data, Is.EqualTo("Forty Two!"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_IntInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest=null;
            var service = ioTCore.CreateServiceElement<int,int>(struct1,
                "ServiceElement_InOut_IntInt",
                (sender, inputarg, cid) => { argtest = inputarg;  return 43;  });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/ServiceElement_InOut_IntInt", JToken.FromObject(42)));
            
            // Then
            Assert.That(argtest, Is.EqualTo(42));
            Assert.That((int)response.Data, Is.EqualTo(43));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ActionServiceElement_Invoked_NoInput_NoOutput()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = ioTCore.CreateActionServiceElement(struct1, "actionService",
                (sender,cid) => { serviceInvoked = true; });

            // When
            Message response = ioTCore.HandleRequest(0, "/struct1/actionService");
            
            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That(response.Data, Is.Null);
        }
    }
}
