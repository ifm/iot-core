using System;
using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using Exceptions;
    using ifmIoTCore.Elements;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceExecutionFailedTests
    {
        [Test]
        public void IoTCoreErrorResponseTest_HasDataMessage()
        {
            using var ioTCore = IoTCoreFactory.Create("id0");

            var response = ioTCore.HandleRequest(0, "/non_existing_service", null);
            Assert.NotNull(response.Code, "response should contain code.");

            Assert.That(response.Data.AsVariantObject().ContainsKey("msg"), "responseData should contain msg tag.");
        }

        [Test]
        public void ServiceErrorResponseTest_InternalError()
        {
            using var ioTCore = IoTCoreFactory.Create("id0");

            ioTCore.Root.AddChild(new ActionServiceElement("failing", (element, i) =>
            {
                throw new Exception("Service Failed");
            }), true);

            var response = ioTCore.HandleRequest(0, "/failing", null);

            Assert.AreEqual(ResponseCodes.InternalError, response.Code);

            Assert.NotNull(response.Code);
            Assert.That(response.Data.AsVariantObject().ContainsKey("msg"));
        }

        [Test]
        public void ServiceErrorResponseTest_ServiceExecutionFailed_withInternalErrorcodeAndMessage()
        {
            using var ioTCore = IoTCoreFactory.Create("id0");

            var errorMessage = "Cannot make coffee. Please empty water tank.";

            ioTCore.Root.AddChild(new ActionServiceElement("failing", (element, i) =>
            {
                throw new IoTCoreException(ResponseCodes.ExecutionFailed, errorMessage, 10032);
            }), true);

            var response = ioTCore.HandleRequest(0, "/failing", null);

            Assert.AreEqual(ResponseCodes.ExecutionFailed, response.Code);

            Assert.NotNull(response.Code);
            Assert.That(response.Data.AsVariantObject().ContainsKey("msg"));
            Assert.That(response.Data.AsVariantObject().ContainsKey("code"));

            Assert.AreEqual("10032", (string)response.Data.AsVariantObject()["code"].AsVariantValue());

        
        }
    }
}
