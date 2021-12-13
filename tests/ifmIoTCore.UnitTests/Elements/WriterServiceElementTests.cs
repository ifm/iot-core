namespace ifmIoTCore.UnitTests.Elements
{
    using NUnit.Framework;
    using Messages;
    using Newtonsoft.Json.Linq;

    [TestFixture]
    class WriterServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsUserData()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.CreateSetterServiceElement<complexData>(struct1, 
                "writerServiceUserData",
                (sender, data, cid) =>
                {
                    argtest = data;
                });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/writerServiceUserData", JToken.FromObject(new complexData())));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.AreEqual(argtest, new complexData());
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsFloat()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.CreateSetterServiceElement<float>(struct1, 
                "writerServiceFloat",
                (sender, data, cid) => { argtest = data; });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/writerServiceFloat", 42f));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.AreEqual((float)argtest, 42f, double.Epsilon);
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsBool()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.CreateSetterServiceElement<bool>(struct1,
                "writerServiceBool",
                (sender, data, cid) => { argtest = data; });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/writerServiceBool", true));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.That(argtest, Is.EqualTo(true));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsString()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.CreateSetterServiceElement<string>(struct1, "writerServiceString",
                (sender, data, cid) => { argtest = data; });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid: 1, address: "/struct1/writerServiceString", "Forty Two"));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.That(argtest, Is.EqualTo("Forty Two"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.CreateSetterServiceElement<int>(struct1, 
                "writerServiceInt",
                (sender, data, cid) => { argtest = data; });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid:1, address:"/struct1/writerserviceint", 42));

            // Then
            Assert.IsNotNull(argtest);
            Assert.That(argtest, Is.EqualTo(42));

            // When
            Message response2 = ioTCore.HandleRequest(new RequestMessage(cid:1, address:"/struct1/writerserviceint", JToken.FromObject(42)));

            // Then
            Assert.IsNotNull(argtest);
            Assert.That(argtest, Is.EqualTo(42));
        }

        [Test, Property("TestCaseKey", "IOTCS-T86")]
        public void Error530InvalidData_WhenProvidedOtherDataTypeTo_WriterServiceElement_AcceptingInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            object argtest = null;
            var service = ioTCore.CreateSetterServiceElement<int>(struct1, "writerServiceInt",
                (sender, data, cid) => { argtest = data; });

            // When
            Message response = ioTCore.HandleRequest(new RequestMessage(cid:1, address:"/struct1/writerserviceint", JToken.FromObject(new complexData())));

            // Then
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.That(response.Code, Is.EqualTo(ResponseCodes.DataInvalid));
        }

    }
}
