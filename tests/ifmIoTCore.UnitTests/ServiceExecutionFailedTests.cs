using System;

namespace ifmIoTCore.UnitTests
{
    using Exceptions;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceExecutionFailedTests
    {
        [Test]
        public void IoTCoreErrorResponseTest()
        {
            using var ioTCore = IoTCoreFactory.Create("id0", null);

            var response = ioTCore.HandleRequest(0, "/non_existing_service", null);

            var jobject = JObject.FromObject(response);

            Assert.That(jobject.ContainsKey("code"));
            Assert.That(((JObject)jobject["data"]).ContainsKey("msg"));
        }

        [Test]
        public void ServiceErrorResponseTest()
        {
            using var ioTCore = IoTCoreFactory.Create("id0", null);

            ioTCore.CreateActionServiceElement(ioTCore.Root, "failing", (element, i) =>
            {
                throw new Exception("Service Failed");
            });

            var response = ioTCore.HandleRequest(0, "/failing", null);

            Assert.AreEqual(500, response.Code);

            var jobject = JObject.FromObject(response);

            Assert.That(jobject.ContainsKey("code"));
            Assert.That(((JObject)jobject["data"]).ContainsKey("msg"));
        }

        [Test]
        [Ignore("New error handling concept will be implemented in IoTCore 2.0")]
        public void ServiceErrorResponseTest2()
        {
            using var ioTCore = IoTCoreFactory.Create("id0", null);

            var errorMessage = "Cannot make coffee. Please empty water tank.";

            ioTCore.CreateActionServiceElement(ioTCore.Root, "failing", (element, i) =>
            {
                throw new ServiceException(10032, errorMessage);
            });

            var response = ioTCore.HandleRequest(0, "/failing", null);

            Assert.AreEqual(ResponseCodes.ExecutionFailed, response.Code);

            var jobject = JObject.FromObject(response);

            Assert.That(jobject.ContainsKey("code"));
            Assert.That(((JObject)jobject["data"]).ContainsKey("msg"));
            Assert.That(((JObject)jobject["data"]).ContainsKey("error"));
            Assert.AreEqual("10032", jobject["data"]["error"].ToString());
        }
    }
}
