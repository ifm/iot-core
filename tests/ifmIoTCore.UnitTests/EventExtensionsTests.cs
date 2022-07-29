namespace ifmIoTCore.UnitTests
{
    using System;
    using System.Threading;
    using Common;
    using Utilities;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class EventExtensionsTests
    {
        private EventHandler TestEvent;

        private EventHandler<object> TestEvent2;

        private EventHandler<RequestMessageEventArgs> TestEvent3;

        private void RaiseTestEvent()
        {
            TestEvent.Raise(this, (EventArgs) null);
        }

        private void RaiseTestEvent2(object parameter)
        {
            TestEvent2.Raise(this, parameter);
        }

        private void RaiseTestEvent3(RequestMessageEventArgs evenArgs)
        {
            TestEvent3.Raise(this, evenArgs);
        }

        [Test]
        public void TestFireMethodGenericEventArgsDerivedNull()
        {
            using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                void OnTestEvent3(object sender, object e)
                {
                    Assert.IsNull(e);
                    manualResetEventSlim.Set();
                }

                TestEvent3 += OnTestEvent3;

                RaiseTestEvent3(null);

                manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(50));

                TestEvent3 -= OnTestEvent3;

                Assert.IsTrue(manualResetEventSlim.IsSet);
            }
        }

        [Test]
        public void TestFireMethodGenericEventArgsDerived()
        {
            using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                void OnTestEvent3(object sender, object e)
                {
                    manualResetEventSlim.Set();
                }

                TestEvent3 += OnTestEvent3;

                RaiseTestEvent3(new RequestMessageEventArgs(new Message(RequestCodes.Request, 1, "/", null), null));

                manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(50));

                TestEvent3 -= OnTestEvent3;

                Assert.IsTrue(manualResetEventSlim.IsSet);
            }
        }

        [Test]
        public void TestFireMethodGenericObject()
        {
            using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                void OnTestEvent2(object sender, object e)
                {
                    manualResetEventSlim.Set();
                }

                TestEvent2 += OnTestEvent2;

                RaiseTestEvent2("lkajsdlkjölkj");

                manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(50));

                TestEvent2 -= OnTestEvent2;

                Assert.IsTrue(manualResetEventSlim.IsSet);
            }
        }

        [Test]
        public void TestFireMethodNonGeneric()
        {
            using (var manualResetEventSlim = new ManualResetEventSlim())
            {
                void OnTestEvent(object sender, EventArgs e)
                {
                    manualResetEventSlim.Set();
                }

                TestEvent += OnTestEvent;

                RaiseTestEvent();

                manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(50));

                TestEvent -= OnTestEvent;

                Assert.IsTrue(manualResetEventSlim.IsSet);
            }
        }
    }
}