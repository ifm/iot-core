namespace ifmIoTCore.UnitTests.Elements
{
    using NUnit.Framework;
    using Messages;

    [TestFixture]
    class ReaderServiceElementTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsInt()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = ioTCore.CreateGetterServiceElement<int>(struct1, 
                "readerServiceInt",
                (sender,cid) => { serviceInvoked = true; return 42; });

            // When
            Message response = ioTCore.HandleRequest(0, "/struct1/readerserviceint", null);

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((int)response.Data, Is.EqualTo(42));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsString()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = ioTCore.CreateGetterServiceElement<string>(struct1, 
                "readerServiceString",
                (sender,cid) => { serviceInvoked = true; return "Forty Two"; });

            // When
            Message response = ioTCore.HandleRequest(0, "/struct1/readerservicestring");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((string)response.Data, Is.EqualTo("Forty Two"));
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsFloat()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = ioTCore.CreateGetterServiceElement<float>(struct1,
                "readerServiceFloat",
                (sender,cid) => { serviceInvoked = true; return 42f; });

            // When
            Message response = ioTCore.HandleRequest(0, "/struct1/readerservicefloat");
            
            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.AreEqual((float)response.Data, 42f, double.Epsilon);
        }

        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsBool()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = ioTCore.CreateGetterServiceElement<string>(struct1,
                "readerServiceBool",
                (sender,cid) => { serviceInvoked = true; return "Forty Two"; });

            // When
            Message response = ioTCore.HandleRequest(0, "/struct1/readerserviceBool");

            // Then
            Assert.That(serviceInvoked, Is.True);
            Assert.That((string)response.Data, Is.EqualTo("Forty Two"));
        }


        [Test, Property("TestCaseKey", "IOTCS-T211")]
        public void ReaderServiceElement_Invoked_OutputsUserData()
        {
            // Given
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            var struct1 = ioTCore.CreateStructureElement(ioTCore.Root, "struct1");
            var serviceInvoked=false;
            Assert.IsFalse(serviceInvoked);
            var service = ioTCore.CreateGetterServiceElement<complexData>(struct1, 
                "readerServiceUserData",
                (sender, cid) =>
                {
                    serviceInvoked = true; 
                    return new complexData();
                });

            // When
            Message response = ioTCore.HandleRequest(0, "/struct1/readerserviceUserData");

            // Then
            Assert.That(serviceInvoked, Is.True);
            var data = response.Data.ToObject<complexData>();
            Assert.That(data, Is.EqualTo(new complexData()));
        }
    }
}
