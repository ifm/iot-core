namespace ifm.IoTCore.UnitTests.Elements
{
    using Common;
    using ifm.Common.Variant;
    using Message;
    using NUnit.Framework;

    [TestFixture]
    class ServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_UserData1UserData2()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest =null;
            var service = ioTCore.ElementManager.CreateServiceElement<complexData, complexData>(struct1, 
                "ServiceElement_InOut_UserData1UserData2",
                (_, inputarg, _) =>
                {
                    argtest = inputarg; 
                    return new complexData();
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/ServiceElement_InOut_UserData1UserData2", Variant.FromObject(new complexData())));
            
            // Then
            Assert.That(argtest, Is.EqualTo(new complexData()));
            Assert.That(Variant.ToObject<complexData>(response.Data), Is.EqualTo(new complexData()));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_BoolString()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest =null;
            var service = ioTCore.ElementManager.CreateServiceElement<bool,string>(struct1, "ServiceElement_InOut_BoolString",
                (sender, inputarg, cid) => { argtest = inputarg; return "Forty Two!"; });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/ServiceElement_InOut_BoolString", Variant.FromObject(true)));
            
            // Then
            Assert.That((bool)argtest, Is.EqualTo(true));
            Assert.That((string)(VariantValue)response.Data, Is.EqualTo("Forty Two!"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ServiceElement_Invoked_InputOutput_IntInt()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest =null;
            var service = ioTCore.ElementManager.CreateServiceElement<int,int>(struct1, 
                "ServiceElement_InOut_IntInt",
                (_, inputarg, _) =>
                {
                    argtest = inputarg; 
                    return 43;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/ServiceElement_InOut_IntInt", Variant.FromObject(42)));
            
            // Then
            Assert.That((int)argtest, Is.EqualTo(42));
            Assert.That((int)(VariantValue)response.Data, Is.EqualTo(43));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ActionServiceElement_Invoked_NoInput_NoOutput()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked =false;
            Assert.That(serviceInvoked, Is.False);
            var service = ioTCore.ElementManager.CreateActionServiceElement(struct1, 
                "actionService",
                (_, _) =>
                {
                    serviceInvoked = true;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(0, "/struct1/actionService");
            
            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That(response.Data, Is.Null);
        }
    }
}
