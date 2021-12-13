using ifmIoTCore.Elements.EventArguments;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Converter.Json;
    using Exceptions;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
    using ifmIoTCore.NetAdapter.Http;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System.Threading;

    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class Event_TreeChanged_Tests
    {
        IIoTCore testiotcore;
        BaseElement sender; TreeChangedEventArgs treechanged;
        ManualResetEventSlim TreeChangedDone;
        const int TreeChangedTimeoutms = 100; // milliseconds
        const string TreeChangedTimeoutMessage = "TreeChangedEvent not triggered before wait timeout";

        void CopyEventArgs(object s, TreeChangedEventArgs tce)
        {
            sender = s as BaseElement;
            treechanged = tce;
            TreeChangedDone?.Set();
        }

        [OneTimeSetUp]
        public void BeforeAll_TreeChangedTests()
        {
            testiotcore = IoTCoreFactory.Create("testiotcore", null);
            testiotcore.TreeChanged += CopyEventArgs;
            TreeChangedDone = new ManualResetEventSlim();
        }

        [SetUp]
        public void BeforeEach_TreechangedTests()
        {
            sender = null;
            treechanged = null;
            TreeChangedDone.Reset();
        }

        [OneTimeTearDown]
        public void AfterAll_TreeChangedTests()
        {
            testiotcore.TreeChanged -= CopyEventArgs;
            testiotcore.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnElementCreation()
        {
            testiotcore.CreateStructureElement(testiotcore.Root, Guid.NewGuid().ToString("N"), raiseTreeChanged:true);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnElementCreation_Supressable()
        {
            testiotcore.CreateStructureElement(testiotcore.Root, Guid.NewGuid().ToString("N"), raiseTreeChanged:false);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsFalse(TreeChangedDone.IsSet, "expected no TreeChangedEvent trigger");
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_ElementAdded_OnCreateElement()
        {
            testiotcore.CreateStructureElement(testiotcore.Root, Guid.NewGuid().ToString("N"), raiseTreeChanged:true);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet,TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementAdded));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_ElementRemoved_OnRemoveElement()
        {
            var element = testiotcore.CreateStructureElement(testiotcore.Root, Guid.NewGuid().ToString("N"), raiseTreeChanged:false);
            testiotcore.RemoveElement(testiotcore.Root, element, raiseTreeChanged:true);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet,TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementRemoved));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnRaiseTreeChanged_ElementAdded()
        {
            testiotcore.RaiseTreeChanged(null, null, TreeChangedAction.ElementAdded);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementAdded));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnRaiseTreeChanged_ElementRemoved()
        {
            testiotcore.RaiseTreeChanged(null, null, TreeChangedAction.ElementRemoved);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementRemoved));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnRaiseTreeChanged_TreeChanged()
        {
            testiotcore.RaiseTreeChanged(null, null, TreeChangedAction.TreeChanged);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.TreeChanged));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_MultipleHandlers1000()
        {
            const int MaxHandlers = 1000;
            using (var ioTCore = IoTCoreFactory.Create("ioTCore", null))
            {
                var handled = new List<bool>();

                void OnTreeChanged(object sender, TreeChangedEventArgs eventargs)
                {
                    handled.Add(true);
                }

                // add multiple handlers to TreeChanged Event 
                for (var i = 0; i < MaxHandlers; i++)
                {
                    ioTCore.TreeChanged += OnTreeChanged;
                }

                // create an element for add / remove
                var struct0 = ioTCore.CreateStructureElement(ioTCore.Root, "struct0", raiseTreeChanged: true);

                for (var i = 0; i < MaxHandlers; i++)
                {
                    ioTCore.TreeChanged -= OnTreeChanged;
                }

                // check Add child with trigger
                Assert.That(handled.Count, Is.EqualTo(MaxHandlers));
                Assert.That(handled, Has.All.True);
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Triggered_Not_OnException()
        {
            using (var testiotcore = IoTCoreFactory.Create("myIot", null))
            {
                var triggered = false;
                var struct0 = testiotcore.CreateStructureElement(testiotcore.Root, Guid.NewGuid().ToString("N"), raiseTreeChanged:true);

                void OnTreeChanged(object sender, TreeChangedEventArgs eventargs)
                {
                    triggered = true;
                }

                testiotcore.TreeChanged += OnTreeChanged;

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.CreateStructureElement(testiotcore.Root, null)); //adding null
                Assert.IsFalse(triggered);

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.CreateStructureElement(testiotcore.Root, struct0.Identifier)); //adding same identifier again
                Assert.IsFalse(triggered);

                triggered = false;
                testiotcore.RemoveElement(testiotcore.Root, struct0, true); //remove element
                Assert.IsTrue(triggered);

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.RemoveElement(testiotcore.Root, struct0)); //remove same element again
                Assert.IsFalse(triggered);

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.RemoveElement(testiotcore.Root, new StructureElement(null, Guid.NewGuid().ToString("N")))); //remove non element again
                Assert.IsFalse(triggered);

                triggered = false;
                Assert.Throws<ArgumentNullException>(() => testiotcore.RemoveElement(testiotcore.Root, null)); //remove null
                Assert.IsFalse(triggered);

                testiotcore.TreeChanged -= OnTreeChanged;
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T19")]
        public void TreeChangedEvent_Message_SentOver_Http()
        { // Integration Test connecting system with external ioTCore/http application, here mocked by a simple http listener
            using var ioTCore = IoTCoreFactory.Create("ioTCore", null);
            ioTCore.CreateDataElement<int>(ioTCore.Root, "data1", (s) => { return 42; }, format: new IntegerFormat(new IntegerValuation(0)));

            using var clientNetAdapterFactory = new HttpClientNetAdapterFactory(new JsonConverter());
            ioTCore.RegisterClientNetAdapterFactory(clientNetAdapterFactory);
            var receiver = new ReceiveIoTMsg("http://127.0.0.1:8053/");
            var treechangeSubscribeResponse = ioTCore.HandleRequest(0,
                "/treechanged/subscribe",
                JToken.Parse(@" {
                                'callback': 'http://127.0.0.1:8053/somepath/', 
                                'datatosend': ['/data1'], 
                                'duration': 'uptime', 
                                'uid': 'ExternalSubscriber1'} "));

            Assert.NotNull(treechangeSubscribeResponse, "Got no response for the /treechanged/subscribe request.");
            Assert.That(treechangeSubscribeResponse.Code, Is.EqualTo(ResponseCodes.Success)); Assert.AreEqual(200, ResponseCodes.Success);
            // trigger TreeChanged Event implicitly by adding element
            ioTCore.CreateStructureElement(ioTCore.Root, "struct1", raiseTreeChanged: true);
            var reqcontent = receiver.Do(10000);

            Assert.NotNull(reqcontent, "Got no response on http://127.0.0.1:8053/");
            Assert.AreEqual(reqcontent.SelectToken("$.code")?.ToObject<int>(), 80); // ensure code is 80 which is event 
            Assert.AreEqual(reqcontent.SelectToken("$.data.srcurl")?.ToObject<string>(), "/treechanged"); // check srcurl is treechanged
            Assert.AreEqual(reqcontent.SelectToken("$.data.payload./data1.data")?.ToObject<int>(), 42); // check datatosend delivered

            ioTCore.RemoveClientNetAdapterFactory(clientNetAdapterFactory);
            clientNetAdapterFactory.Dispose();
        }
    }
}
