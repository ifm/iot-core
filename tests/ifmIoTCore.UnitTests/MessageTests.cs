using ifmIoTCore.Common.Variant;

namespace ifmIoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    
    using Messages;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;


    [TestFixture]
    public class MessageTests
    {
        [Test]
        public void TestSubscribeMessageDeserializing()
        {
            var MessageContaining_adr_cide_code = "{\"adr\":\"treechanged/subscribe\",\"data\":{\"callback\":\"http://127.0.0.1:8001/HandleEvent\",\"datatosend\":[\"/getidentity\"]},\"code\":10,\"cid\":123}";

            var jsonConverter= new MessageConverter.Json.Newtonsoft.MessageConverter();
            var subScribeMessage = jsonConverter.Deserialize(MessageContaining_adr_cide_code);

            Assert.NotNull(subScribeMessage);
            Assert.AreEqual(123, subScribeMessage.Cid);
            Assert.AreEqual(10, subScribeMessage.Code);

            Assert.AreEqual(typeof(Message), subScribeMessage.GetType());

            Assert.AreEqual(typeof(VariantObject), subScribeMessage.Data.GetType());
        }

        [Test, Property("TestCaseKey", "IOTCS-T14")]
        public void Message_HasRequiredFields_cid_code_adr_data_reply()
        {
            // Create Message using json string with required and see if it is constructed successfully
            var subScribeMessageAsString = "{\"adr\":\"treechanged/subscribe\",\"data\":{\"callback\":\"http://127.0.0.1:8001/HandleEvent\",\"datatosend\":[\"/getidentity\"]},\"code\":10,\"cid\":123}";
            var jsonConverter= new MessageConverter.Json.Newtonsoft.MessageConverter();
            var CommandInterfaceMessage1 = jsonConverter.Deserialize(subScribeMessageAsString);
            Assert.NotNull(CommandInterfaceMessage1, "Pre-Condition step, assuming deserialization works");

            // Check if fields similar to json string are available in constructed object
            Assert.IsInstanceOf(typeof(Message), CommandInterfaceMessage1);
            string[] expectedFields = { "Code", "Cid", "Address", "Data", "Reply" };
            var ActualMembers = CommandInterfaceMessage1.GetType().GetMembers();
            var actualFields = from n in ActualMembers select n.Name;
            foreach (var expectedField in expectedFields)
                Assert.That(actualFields, Has.Member(expectedField));

            // 2nd Message Object to check if Message Object to Message Json String conversion also has the requred fields
            var CommandInterfaceMessage2 = jsonConverter.Deserialize(jsonConverter.Serialize(CommandInterfaceMessage1));

            foreach (var CommandInterfaceMessage in new List<Message> { CommandInterfaceMessage1, CommandInterfaceMessage2 })
            {
                // Check values of the required fields of json string match to members of constructed object
                Assert.AreEqual(123, CommandInterfaceMessage.Cid);
                Assert.AreEqual(10, CommandInterfaceMessage.Code);
                Assert.AreEqual("treechanged/subscribe", CommandInterfaceMessage.Address);
                Assert.AreEqual(typeof(VariantObject), CommandInterfaceMessage.Data.GetType());




                Assert.AreEqual(new VariantValue("http://127.0.0.1:8001/HandleEvent"), ((VariantObject)CommandInterfaceMessage.Data)["callback"]);
                Assert.AreEqual(new VariantValue("/getidentity"), ((VariantArray)((VariantObject)CommandInterfaceMessage.Data)["datatosend"])[0]);
                Assert.AreEqual("treechanged/subscribe", CommandInterfaceMessage.Address);
            }

        }
    }
}
