using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests.Elements
{
    using ifmIoTCore.Elements;
    using NUnit.Framework;
    using Messages;

    [TestFixture]
    class ReaderServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = new GetterServiceElement("readerServiceInt", 
                (sender,cid) => { serviceInvoked = true; return new VariantValue(42); });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(0, "/struct1/readerserviceint", null);

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((int)(VariantValue)response.Data, Is.EqualTo(42));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsString()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = new GetterServiceElement("readerServiceString", 
                (sender,cid) => { serviceInvoked = true; return new VariantValue("Forty Two"); });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(0, "/struct1/readerservicestring");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((string)(VariantValue)response.Data, Is.EqualTo("Forty Two"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsFloat()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = new GetterServiceElement("readerServiceFloat",
                (sender,cid) => { serviceInvoked = true; return new VariantValue(42f); });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(0, "/struct1/readerservicefloat");
            
            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.AreEqual((float)(VariantValue)response.Data, 42f, double.Epsilon);
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsBool()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = new GetterServiceElement("readerServiceBool",
                (sender,cid) => { serviceInvoked = true; return new VariantValue("Forty Two"); });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(0, "/struct1/readerserviceBool");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((string)(VariantValue)response.Data, Is.EqualTo("Forty Two"));
        }


        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsUserData()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore");
            var struct1 = new StructureElement("struct1");
            ioTCore.Root.AddChild(struct1, true);
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = new GetterServiceElement("readerServiceUserData", 
                (sender, cid) =>
                {
                    serviceInvoked = true; 
                    return Variant.FromObject(new complexData());
                });
            struct1.AddChild(service, true);

            // When
            var response = ioTCore.HandleRequest(0, "/struct1/readerserviceUserData");

            // Then
            Assert.That(serviceInvoked, Is.True);

            var data = Variant.ToObject<complexData>(response.Data);
            Assert.That(data, Is.EqualTo(new complexData()));
        }
    }
}
