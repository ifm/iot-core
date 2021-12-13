namespace ifmIoTCore.UnitTests.NetAdapter
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Converter.Json;
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;


    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class MessageTests
    {
        [Test]
        public void TestSubscribeMessageDeserializing()
        {
            var MessageContaining_adr_cide_code = "{\"adr\":\"treechanged/subscribe\",\"data\":{\"callback\":\"http://127.0.0.1:8001/HandleEvent\",\"datatosend\":[\"/getidentity\"]},\"code\":10,\"cid\":123}";

            var jsonConverter = new JsonConverter();
            var subScribeMessage = jsonConverter.Deserialize<Message>(MessageContaining_adr_cide_code);

            Assert.NotNull(subScribeMessage);
            Assert.AreEqual(123, subScribeMessage.Cid);
            Assert.AreEqual(10, subScribeMessage.Code);

            Assert.AreEqual(typeof(Message), subScribeMessage.GetType());

            Assert.AreEqual(typeof(JObject), subScribeMessage.Data.GetType());
        }

        [Test, Property("TestCaseKey", "IOTCS-T14")]
        public void Message_HasRequiredFields_cid_code_adr_data_reply()
        {
            // Create Message using json string with required and see if it is constructed successfully
            var subScribeMessageAsString = "{\"adr\":\"treechanged/subscribe\",\"data\":{\"callback\":\"http://127.0.0.1:8001/HandleEvent\",\"datatosend\":[\"/getidentity\"]},\"code\":10,\"cid\":123}";
            ifmIoTCore.NetAdapter.IConverter jsonConverter = new JsonConverter();
            var CommandInterfaceMessage1 = jsonConverter.Deserialize<RequestMessage>(subScribeMessageAsString);
            Assert.NotNull(CommandInterfaceMessage1, "Pre-Condition step, assuming deserialization works");

            // Check if fields similar to json string are available in constructed object
            Assert.IsInstanceOf(typeof(RequestMessage), CommandInterfaceMessage1);
            string[] expectedFields = { "Code", "Cid", "Address", "Data", "Reply" };
            var ActualMembers = CommandInterfaceMessage1.GetType().GetMembers();
            var actualFields = from n in ActualMembers select n.Name;
            foreach (var expectedField in expectedFields)
                Assert.That(actualFields, Has.Member(expectedField));

            // 2nd Message Object to check if Message Object to Message Json String conversion also has the requred fields
            var CommandInterfaceMessage2 = jsonConverter.Deserialize<Message>(jsonConverter.Serialize(CommandInterfaceMessage1));

            foreach (var CommandInterfaceMessage in new List<Message> { CommandInterfaceMessage1, CommandInterfaceMessage2 })
            {
                // Check values of the required fields of json string match to members of constructed object
                Assert.AreEqual(123, CommandInterfaceMessage.Cid);
                Assert.AreEqual(10, CommandInterfaceMessage.Code);
                Assert.AreEqual("treechanged/subscribe", CommandInterfaceMessage.Address);
                Assert.AreEqual(typeof(JObject), CommandInterfaceMessage.Data.GetType());
                Assert.AreEqual(new JValue("http://127.0.0.1:8001/HandleEvent"), CommandInterfaceMessage.Data["callback"]);
                Assert.AreEqual(new JValue("/getidentity"), CommandInterfaceMessage.Data["datatosend"][0]);
                Assert.AreEqual("treechanged/subscribe", CommandInterfaceMessage.Address);
            }

        }
    }

    [TestFixture]
    public class ResponseTypes
    {
        [Test, Property("TestCaseKey", "IOTCS-T80")]
        public void Response200_Success()
        {
                using var ioTCore = IoTCoreFactory.Create("id0", null);
                var dataElement = ioTCore.CreateDataElement<string>(ioTCore.Root, "data0", (b) => "data123");

                var getDataResponse = ioTCore.HandleRequest(0, "/data0/getdata");
                Assert.NotNull(getDataResponse);
                Assert.That(getDataResponse.Code, Is.EqualTo(200));
        }

        [Test, Property("TestCaseKey", "IOTCS-T81")]
        public void Response400_BadRequest_NonServiceRequest()
        {
                using var ioTCore = IoTCoreFactory.Create("id0", null);
                var dataElement = ioTCore.CreateDataElement<string>(ioTCore.Root, "data0", (b) => "data123");

                var getDataResponse = ioTCore.HandleRequest(0, "/data0");
                Assert.NotNull(getDataResponse);
                Assert.AreEqual(400, getDataResponse.Code);
        }

        [Test, Property("TestCaseKey", "IOTCS-T82")]
        public void Response404_NotFound_NonExistingService()
        {
                using var ioTCore = IoTCoreFactory.Create("id0", null);
                var dataElement = ioTCore.CreateDataElement<string>(ioTCore.Root, "data0", (b) => "data123");

                var getDataResponse = ioTCore.HandleRequest(0, "/data0/nonexistingservice");
                Assert.NotNull(getDataResponse);
                Assert.AreEqual(404, getDataResponse.Code);
        }

    }
}
