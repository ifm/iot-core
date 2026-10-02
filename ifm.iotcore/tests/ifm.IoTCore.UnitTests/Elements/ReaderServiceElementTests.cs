namespace ifm.IoTCore.UnitTests.Elements
{
    using ifm.Common.Variant;
    using NUnit.Framework;

    [TestFixture]
    class ReaderServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsInt()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.That(serviceInvoked, Is.False);
            var service = ioTCore.ElementManager.CreateGetterServiceElement(struct1, 
                "readerServiceInt",
                (sender, cid) =>
                {
                    serviceInvoked = true;
                    return 42;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(0, "/struct1/readerserviceint");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((int)(VariantValue)response.Data, Is.EqualTo(42));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsString()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.That(serviceInvoked, Is.False);
            var service = ioTCore.ElementManager.CreateGetterServiceElement(struct1, 
                "readerServiceString",
                (_, _) =>
                {
                    serviceInvoked = true; 
                    return "Forty Two";
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(0, "/struct1/readerservicestring");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((string)(VariantValue)response.Data, Is.EqualTo("Forty Two"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsFloat()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.That(serviceInvoked, Is.False);
            var service = ioTCore.ElementManager.CreateGetterServiceElement(struct1, 
                "readerServiceFloat",
                (_, _) =>
                {
                    serviceInvoked = true; 
                    return 42f;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(0, "/struct1/readerservicefloat");
            
            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((float)(VariantValue)response.Data, Is.EqualTo(42f).Within(double.Epsilon));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsBool()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.That(serviceInvoked, Is.False);
            var service = ioTCore.ElementManager.CreateGetterServiceElement(struct1, 
                "readerServiceBool",
                (_, _) =>
                {
                    serviceInvoked = true; 
                    return "Forty Two";
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(0, "/struct1/readerserviceBool");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((string)(VariantValue)response.Data, Is.EqualTo("Forty Two"));
        }


        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsUserData()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.That(serviceInvoked, Is.False);
            var service = ioTCore.ElementManager.CreateGetterServiceElement(struct1, "readerServiceUserData",
                (sender, cid) =>
                {
                    serviceInvoked = true;
                    return new complexData();
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(0, "/struct1/readerserviceUserData");

            // Then
            Assert.That(serviceInvoked, Is.True);

            var data = Variant.ToObject<complexData>(response.Data);
            Assert.That(data, Is.EqualTo(new complexData()));
        }
    }
}
