namespace ifm.IoTCore.MessageConverter.Json.UnitTests
{
    using Common;
    using Common.Exceptions;
    using NUnit.Framework;

    [TestFixture]
    internal class MessageConverterTests
    {
        [Test]
        public void TestAllowNull()
        {
            var messageConverter = new MessageConverter(MessageDeserializationMode.AllowNull);

            Assert.DoesNotThrow(() =>
            {
                var result = messageConverter.Deserialize("{\"code\" : 10, \"cid\" : 10, \"adr\": \"asdf\", \"data\": {\"something\" : null}}");
            });
        }

        [Test]
        public void TestThrowOnNull()
        {
            var messageConverter = new MessageConverter(MessageDeserializationMode.ThrowOnNull);

            var exception = Assert.Throws<IoTCoreException>(() =>
            {
                var result = messageConverter.Deserialize("{\"code\" : 10, \"cid\" : 10, \"adr\": \"asdf\", \"data\": {\"something\" : null}}");
            });

            Assert.That(exception.ResponseCode, Is.EqualTo(ResponseCodes.DataInvalid));
        }

        [Test]
        public void TestThrowsDataInvalidOnDuplicateKey()
        {
            var messageConverter = new MessageConverter();

            Assert.Throws<IoTCoreException>(() =>
            {
                try
                {

                    var result = messageConverter.Deserialize(
                        "{\"code\" : 10, \"cid\" : 10, \"adr\": \"asdf\", \"data\": {\"something\" : null, \"something\" : null}}");
                }
                catch (IoTCoreException e)
                {
                    Assert.That(e.ResponseCode, Is.EqualTo(ResponseCodes.DataInvalid));
                    throw;
                }
            });
        }
    }
}
