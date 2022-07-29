namespace ifmIoTCore.UnitTests
{
    using ifmIoTCore.Elements;
    using NUnit.Framework;


    [TestFixture]
    public class ErrorResponseTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T80")]
        public void Response200_Success()
        {
                using var ioTCore = IoTCoreFactory.Create("id0");
                var dataElement = new ReadOnlyDataElement<string>("data0", (b) => "data123");
                ioTCore.Root.AddChild(dataElement, true);

                var getDataResponse = ioTCore.HandleRequest(0, "/data0/getdata");
                Assert.NotNull(getDataResponse);
                Assert.That(getDataResponse.Code, Is.EqualTo(200));
        }

        [Test, Property("TestCaseKey", "IOTCS-T81")]
        public void Response400_BadRequest_NonServiceRequest()
        {
                using var ioTCore = IoTCoreFactory.Create("id0");
                var dataElement = new ReadOnlyDataElement<string>("data0", (b) => "data123");
                ioTCore.Root.AddChild(dataElement, true);

                var getDataResponse = ioTCore.HandleRequest(0, "/data0");
                Assert.NotNull(getDataResponse);
                Assert.AreEqual(400, getDataResponse.Code);
        }

        [Test, Property("TestCaseKey", "IOTCS-T82")]
        public void Response404_NotFound_NonExistingService()
        {
                using var ioTCore = IoTCoreFactory.Create("id0");
                var dataElement = new ReadOnlyDataElement<string>("data0", (b) => "data123");
                ioTCore.Root.AddChild(dataElement, true);

                var getDataResponse = ioTCore.HandleRequest(0, "/data0/nonexistingservice");
                Assert.NotNull(getDataResponse);
                Assert.AreEqual(404, getDataResponse.Code);
        }

    }
}
