namespace ifm.IoTCore.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class ErrorResponseTests
    {
        [Test, Property("TestCaseKey", "IOTCS-T80")]
        public void Response200_Success()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var dataElement = ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data0", "data123");

            var getDataResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/data0/getdata");
            Assert.That(getDataResponse,Is.Not.Null);
            Assert.That(getDataResponse.Code, Is.EqualTo(200));
        }

        [Test, Property("TestCaseKey", "IOTCS-T81")]
        public void Response400_BadRequest_NonServiceRequest()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var dataElement = ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data0", "data123");

            var getDataResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/data0");
            Assert.That(getDataResponse,Is.Not.Null);
            Assert.That(getDataResponse.Code, Is.EqualTo(400));
        }

        [Test, Property("TestCaseKey", "IOTCS-T82")]
        public void Response404_NotFound_NonExistingService()
        {
            var ioTCore = Helpers.CreateIoTCore("id0");
            var dataElement = ioTCore.ElementManager.CreateSimpleDataElement(ioTCore.Root, "data0", "data123");

            var getDataResponse = ioTCore.MessageDispatcher.HandleRequest(0, "/data0/nonexistingservice");
            Assert.That(getDataResponse,Is.Not.Null);
            Assert.That(getDataResponse.Code, Is.EqualTo(404));
        }
    }
}
