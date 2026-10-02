namespace ifm.IoTCore.UnitTests.Elements
{
    using Common;
    using ifm.Common.Variant;
    using Message;
    using NUnit.Framework;

    [TestFixture]
    class WriterServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsUserData()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.ElementManager.CreateSetterServiceElement<complexData>(struct1, "writerServiceUserData",
                (sender, data, cid) =>
                {
                    argtest = data;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceUserData", Variant.FromObject(new complexData())));
            
            // Then
            Assert.That(argtest, Is.Not.Null);
            Assert.That((complexData)argtest, Is.EqualTo(new complexData()));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsFloat()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.ElementManager.CreateSetterServiceElement<float>(struct1, "writerServiceFloat", (sender, data, cid) => { argtest = data; });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceFloat", new VariantValue(42f)));
            
            // Then
            Assert.That(argtest, Is.Not.Null);
            Assert.That((float)argtest, Is.EqualTo(42f).Within(double.Epsilon));

        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsBool()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.ElementManager.CreateSetterServiceElement<bool>(struct1, "writerServiceBool",
                (sender, data, cid) =>
                {
                    argtest = data;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceBool", new VariantValue(true)));
            
            // Then
            Assert.That(argtest, Is.Not.Null);
            Assert.That((bool)argtest, Is.EqualTo(true));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsString()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.ElementManager.CreateSetterServiceElement<string>(struct1, "writerServiceString",
                (sender, data, cid) => { argtest = data; });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceString", new VariantValue("Forty Two")));
            
            // Then
            Assert.That(argtest, Is.Not.Null);
            Assert.That((string)argtest, Is.EqualTo("Forty Two"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsInt()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.ElementManager.CreateSetterServiceElement<int>(struct1, "writerServiceInt",
                (sender, data, cid) => { argtest = data; });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerserviceint", new VariantValue(42)));

            // Then
            Assert.That(argtest, Is.Not.Null);
            Assert.That((int)argtest, Is.EqualTo(42));

            // When
            var response2 = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerserviceint", Variant.FromObject(42)));

            // Then
            Assert.That(argtest, Is.Not.Null);
            Assert.That((int)argtest, Is.EqualTo(42));
        }

        [Ignore("Since there are no more generics on the servicelelements, they are not bound to a single type. Please refactor or delete test.")]
        [Test, Property("TestCaseKey", "IOTCS-T86")]
        public void Error530InvalidData_WhenProvidedOtherDataTypeTo_WriterServiceElement_AcceptingInt()
        {
            // Given
            var ioTCore = Helpers.CreateIoTCore("ioTCore");
            var struct1 = ioTCore.ElementManager.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.ElementManager.CreateSetterServiceElement<object>(struct1, "writerServiceInt",
                (sender, data, cid) =>
                {
                    argtest = data;
                });

            // When
            var response = ioTCore.MessageDispatcher.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerserviceint", Variant.FromObject(new complexData())));

            // Then
            Assert.That(ResponseCodes.DataInvalid, Is.EqualTo(422));
            Assert.That(response.Code, Is.EqualTo(ResponseCodes.DataInvalid));
        }

    }
}
