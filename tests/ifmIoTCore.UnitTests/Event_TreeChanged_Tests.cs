using ifmIoTCore.Elements.EventArguments;

namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Collections.Generic;
    
    using Exceptions;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Elements.Formats;
    using ifmIoTCore.Elements.Valuations;
    using Messages;
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
            testiotcore = IoTCoreFactory.Create("testiotcore");
            testiotcore.Root.TreeChanged += CopyEventArgs;
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
            testiotcore.Root.TreeChanged -= CopyEventArgs;
            testiotcore.Dispose();
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnElementCreation()
        {
            testiotcore.Root.AddChild(new StructureElement(Guid.NewGuid().ToString("N")), true);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnElementCreation_Supressable()
        {
            testiotcore.Root.AddChild(new StructureElement(Guid.NewGuid().ToString("N")), false);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsFalse(TreeChangedDone.IsSet, "expected no TreeChangedEvent trigger");
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_ElementAdded_OnCreateElement()
        {
            testiotcore.Root.AddChild(new StructureElement(Guid.NewGuid().ToString("N")), true);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet,TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementAdded));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_ElementRemoved_OnRemoveElement()
        {
            var element = new StructureElement(Guid.NewGuid().ToString("N"));
            testiotcore.Root.AddChild(element);
            testiotcore.Root.RemoveChild(element, true);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet,TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementRemoved));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnRaiseTreeChanged_ElementAdded()
        {
            testiotcore.Root.RaiseTreeChanged(null, TreeChangedAction.ElementAdded);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementAdded));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnRaiseTreeChanged_ElementRemoved()
        {
            testiotcore.Root.RaiseTreeChanged(null, TreeChangedAction.ElementRemoved);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.ElementRemoved));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Trigger_OnRaiseTreeChanged_TreeChanged()
        {
            testiotcore.Root.RaiseTreeChanged(null, TreeChangedAction.TreeChanged);
            TreeChangedDone.Wait(TreeChangedTimeoutms);
            Assert.IsTrue(TreeChangedDone.IsSet, TreeChangedTimeoutMessage);
            Assert.That(treechanged.Action, Is.EqualTo(TreeChangedAction.TreeChanged));
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_MultipleHandlers1000()
        {
            const int MaxHandlers = 1000;
            using (var ioTCore = IoTCoreFactory.Create("ioTCore"))
            {
                var handled = new List<bool>();

                void OnTreeChanged(object sender, TreeChangedEventArgs eventargs)
                {
                    handled.Add(true);
                }

                // add multiple handlers to TreeChanged Event 
                for (var i = 0; i < MaxHandlers; i++)
                {
                    ioTCore.Root.TreeChanged += OnTreeChanged;
                }

                // create an element for add / remove
                ioTCore.Root.AddChild(new StructureElement("struct0"), true);

                for (var i = 0; i < MaxHandlers; i++)
                {
                    ioTCore.Root.TreeChanged -= OnTreeChanged;
                }

                // check Add child with trigger
                Assert.That(handled.Count, Is.EqualTo(MaxHandlers));
                Assert.That(handled, Has.All.True);
            }
        }

        [Test, Property("TestCaseKey", "IOTCS-T17")]
        public void TreeChangedEvent_Triggered_Not_OnException()
        {
            using (var testiotcore = IoTCoreFactory.Create("myIot"))
            {
                var triggered = false;
                var struct0 = new StructureElement(Guid.NewGuid().ToString("N"));
                testiotcore.Root.AddChild(struct0, true);

                void OnTreeChanged(object sender, TreeChangedEventArgs eventargs)
                {
                    triggered = true;
                }

                testiotcore.Root.TreeChanged += OnTreeChanged;

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.Root.AddChild(new StructureElement((string)null), true)); //adding null
                Assert.IsFalse(triggered);

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.Root.AddChild(new StructureElement(struct0.Identifier), true)); //adding same identifier again
                Assert.IsFalse(triggered);

                triggered = false;
                testiotcore.Root.RemoveChild(struct0, true); //remove element
                Assert.IsTrue(triggered);

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.Root.RemoveChild(struct0, true)); //remove same element again
                Assert.IsFalse(triggered);

                triggered = false;
                Assert.Throws<IoTCoreException>(() => testiotcore.Root.RemoveChild(new StructureElement(Guid.NewGuid().ToString("N")), true)); //remove non element again
                Assert.IsFalse(triggered);

                triggered = false;
                Assert.Throws<ArgumentNullException>(() => testiotcore.Root.RemoveChild(null, true)); //remove null
                Assert.IsFalse(triggered);

                testiotcore.Root.TreeChanged -= OnTreeChanged;
            }
        }
    }
}
