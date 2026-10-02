namespace ifm.IoTCore.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using ifm.Common.Variant;
    using NUnit.Framework;
    using Message;
    using MessageConverter.Json;

    [TestFixture]
    public class MessageTests
    {
        [Test]
        public void TestSubscribeMessageDeserializing()
        {
            var MessageContaining_adr_cide_code = "{\"adr\":\"treechanged/subscribe\",\"data\":{\"callback\":\"http://127.0.0.1:8001/HandleEvent\",\"datatosend\":[\"/getidentity\"]},\"code\":10,\"cid\":123}";

            var jsonConverter= new MessageConverter();
            var subScribeMessage = jsonConverter.Deserialize(MessageContaining_adr_cide_code);

            Assert.That(subScribeMessage,Is.Not.Null);
            Assert.That(subScribeMessage.Cid, Is.EqualTo(123));
            Assert.That(subScribeMessage.Code, Is.EqualTo(10));

            Assert.That(subScribeMessage.GetType(), Is.EqualTo(typeof(Message)));
            Assert.That(subScribeMessage.Data.GetType(), Is.EqualTo(typeof(VariantObject)));
        }

        [Test, Property("TestCaseKey", "IOTCS-T14")]
        public void Message_HasRequiredFields_cid_code_adr_data_reply()
        {
            // Create Message using json string with required and see if it is constructed successfully
            var subScribeMessageAsString = "{\"adr\":\"treechanged/subscribe\",\"data\":{\"callback\":\"http://127.0.0.1:8001/HandleEvent\",\"datatosend\":[\"/getidentity\"]},\"code\":10,\"cid\":123}";
            var jsonConverter= new MessageConverter();
            var CommandInterfaceMessage1 = jsonConverter.Deserialize(subScribeMessageAsString);
            Assert.That(CommandInterfaceMessage1, Is.Not.Null, "Pre-Condition step, assuming deserialization works");

            // Check if fields similar to json string are available in constructed object
            Assert.That(CommandInterfaceMessage1, Is.InstanceOf(typeof(Message)));

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
                Assert.That(CommandInterfaceMessage.Cid, Is.EqualTo(123));
                Assert.That(CommandInterfaceMessage.Code, Is.EqualTo(10));
                Assert.That(CommandInterfaceMessage.Address, Is.EqualTo("treechanged/subscribe"));
                Assert.That(CommandInterfaceMessage.Data.GetType(), Is.EqualTo(typeof(VariantObject)));
                
                Assert.That(((VariantObject)CommandInterfaceMessage.Data)["callback"], Is.EqualTo(new VariantValue("http://127.0.0.1:8001/HandleEvent")));
                Assert.That(((VariantArray)((VariantObject)CommandInterfaceMessage.Data)["datatosend"])[0], Is.EqualTo(new VariantValue("/getidentity")));
                Assert.That(CommandInterfaceMessage.Address, Is.EqualTo("treechanged/subscribe"));
            }

        }
    }
}
