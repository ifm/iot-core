namespace ifm.IoTCore.UnitTests
{
    using System;
    using System.Collections;
    using System.Reflection;
    using Common;
    using Message;
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Repository.Hierarchy;
    using MessageConverter.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    internal class TemporaryMemoryAppender : IDisposable
    {
        private readonly MemoryAppender _memoryAppender;
        private string _appenderGuid;

        public TemporaryMemoryAppender()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetExecutingAssembly());
            _appenderGuid = Guid.NewGuid().ToString();
            _memoryAppender = new MemoryAppender { Name = _appenderGuid };
            hierarchy.Root.AddAppender(_memoryAppender);
        }

        public void Dispose()
        {
            _memoryAppender.Close();
            var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetExecutingAssembly());
            hierarchy.Root.RemoveAppender(_memoryAppender);
        }

        public LoggingEvent[] PopAllEvents()
        {
            return _memoryAppender.PopAllEvents();
        }
    }


    public class TestUnsubscribeMessages
    {
        public static IEnumerable SameData_AlternativeNames
        {
            get
            {
                yield return new TestCaseData(
                new MessageConverter().Deserialize(
                    JObject.Parse(@"{
                        'cid': 1,
                        'code': 10,
                        'adr': '/testevent/unsubscribe',
                        'data': { 
                                'callback': 'http://localhost:8000/test/handleevent'
                                }
                         }").ToString() 
                        )
                ).SetName("{m}");

                yield return new TestCaseData(
                new MessageConverter().Deserialize(
                    JObject.Parse(@"{
                        'cid': 1,
                        'code': 10,
                        'adr': '/testevent/unsubscribe',
                        'data': { 
                                'callbackurl': 'http://localhost:8000/test/handleevent'
                                }
                         }").ToString() 
                        ) 
                ).SetName("{m}_callbackurl");
            }
        }
    }

    [TestFixture]
    public class Event_Unsubscribe_Tests
    {
        [Test, Property("TestCaseKey", "IOTCS-T33")]
        [TestCaseSource(typeof(TestUnsubscribeMessages), nameof(TestUnsubscribeMessages.SameData_AlternativeNames))]
        public void Unsubscribe_ValidMessage_IsProcessed(Message unsubscribeMessage)
        {
            var iotcore = Helpers.CreateIoTCore("testIoTCore");
            var ev = iotcore.ElementManager.CreateEventElement(iotcore.Root, "testevent");

            var resultMessage = iotcore.MessageDispatcher.HandleRequest(unsubscribeMessage);
            Assert.That(resultMessage.Code, Is.EqualTo((int)ResponseCodes.DataInvalid));
        }

    }
}
