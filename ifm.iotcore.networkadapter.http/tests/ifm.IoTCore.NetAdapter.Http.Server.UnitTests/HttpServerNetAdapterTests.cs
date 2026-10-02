using System.Security.Cryptography.X509Certificates;
using ifm.IoTCore.MessageConverter.Contracts;
using ifm.IoTCore.MessageDispatcher.Contracts;
using NUnit.Framework;

namespace ifm.IoTCore.NetAdapter.Http.Server.UnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestConstructorHttp_Success()
        {
            var httpServer = new HttpServerNetAdapter(new TestMessageHandler(), new Uri("http://127.0.0.1:8090"),
                new TestMessageConverter(), new HttpServerNetAdapterConfiguration(), null, null, null);
            Assert.That(httpServer, Is.Not.Null);
        }

        [Test]
        public void TestConstructorHttps_Fail()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ = new HttpServerNetAdapter(new TestMessageHandler(), new Uri("https://127.0.0.1:8090"),
                    new TestMessageConverter(), new HttpServerNetAdapterConfiguration(), null, null, null);
            });
        }

        [Test]
        public void TestConstructorHttps_Success()
        {
            Assert.DoesNotThrow(() =>
            {
                _ = new HttpServerNetAdapter(new TestMessageHandler(), new Uri("https://127.0.0.1:8090"),
                    new TestMessageConverter(), new HttpServerNetAdapterConfiguration(), null, new X509Certificate2Collection(), null);
            });
        }
    }

    public class TestMessageConverter : IMessageConverter
    {
        public string Serialize(Message.Message message)
        {
            throw new NotImplementedException();
        }

        public Message.Message Deserialize(string message)
        {
            throw new NotImplementedException();
        }

        public string Type { get; } = "null";
        public string ContentType { get; } = "schmarf";
    }

    public class TestMessageHandler : IMessageDispatcher
    {
        public Message.Message HandleRequest(Message.Message message)
        {
            throw new NotImplementedException();
        }

        public void HandleEvent(Message.Message message)
        {
            throw new NotImplementedException();
        }
    }
}