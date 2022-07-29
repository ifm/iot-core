using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements;
    using NUnit.Framework;
    using Messages;
    using Newtonsoft.Json.Linq;
    using Utilities;

    [TestFixture]
    class WriterServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsUserData()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest = null;
            var service = new SetterServiceElement("writerServiceUserData",
                (sender, data, cid) =>
                {
                    argtest = data;
                });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceUserData", Variant.FromObject(new complexData())));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.AreEqual(Variant.ToObject<complexData>((Variant)argtest), new complexData());
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsFloat()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest = null;
            var service = new SetterServiceElement("writerServiceFloat",
                (sender, data, cid) => { argtest = data; });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceFloat", new VariantValue(42f)));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.AreEqual((float)(VariantValue)argtest, 42f, double.Epsilon);
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsBool()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest = null;
            var service = new SetterServiceElement("writerServiceBool",
                (sender, data, cid) =>
                {
                    argtest = data;
                });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceBool", new VariantValue(true)));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.That((bool)(VariantValue)argtest, Is.EqualTo(true));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsString()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest = null;
            var service = new SetterServiceElement("writerServiceString",
                (sender, data, cid) => { argtest = data; });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerServiceString", new VariantValue("Forty Two")));
            
            // Then
            Assert.IsNotNull(argtest);
            Assert.That((string)(VariantValue)argtest, Is.EqualTo("Forty Two"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void WriterServiceElement_Invoked_AcceptsInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            object argtest = null;
            var service = new SetterServiceElement("writerServiceInt",
                (sender, data, cid) => { argtest = data; });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerserviceint", new VariantValue(42)));

            // Then
            Assert.IsNotNull(argtest);
            Assert.That((int)(VariantValue)argtest, Is.EqualTo(42));

            // When
            var response2 = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerserviceint", Variant.FromObject(42)));

            // Then
            Assert.IsNotNull(argtest);
            Assert.That((int)(VariantValue)argtest, Is.EqualTo(42));
        }

        [Ignore("Since there are no more generics on the servicelelements, they are not bound to a single type. Please refactor or delete test.")]
        [Test, Property("TestCaseKey", "IOTCS-T86")]
        public void Error530InvalidData_WhenProvidedOtherDataTypeTo_WriterServiceElement_AcceptingInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1);
            object argtest = null;
            var service = new SetterServiceElement("writerServiceInt",
                (sender, data, cid) =>
                {
                    argtest = data;
                });
            struct1.AddChild(service);

            // When
            var response = ioTCore.HandleRequest(new Message(RequestCodes.Request, 1, "/struct1/writerserviceint", Variant.FromObject(new complexData())));

            // Then
            Assert.AreEqual(422, ResponseCodes.DataInvalid);
            Assert.That(response.Code, Is.EqualTo(ResponseCodes.DataInvalid));
        }

    }
}
