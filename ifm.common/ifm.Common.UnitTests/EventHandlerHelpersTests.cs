namespace ifm.Common.UnitTests;

using System;
using System.ComponentModel;
using NUnit.Framework;

public class TestEventArgs : CancelEventArgs
{
}

public class EventHandlerHelpersTests
{
    [Test]
    public void RaiseEvent_NoHandler_DoNothing()
    {
        ((EventHandler<EventArgs>)null).Raise(this);
        Assert.That(true);
    }

    [Test]
    public void RaiseEvent_OneHandler_Success()
    {
        EventHandler<TestEventArgs> event1 = null;

        var handlerCalled = false;
        event1 += (_, _) =>
        {
            handlerCalled = true;
        };

        event1.Raise(this, new TestEventArgs());

        Assert.That(handlerCalled);
    }

    [Test]
    public void RaiseEvent_MultipleHandlers_AllHandlersCalled()
    {
        EventHandler<TestEventArgs> event1 = null;

        var handler1Called = false;
        event1 += (_, _) =>
        {
            handler1Called = true;
        };

        var handler2Called = false;
        event1 += (_, _) =>
        {
            handler2Called = true;
        };

        var handler3Called = false;
        event1 += (_, _) =>
        {
            handler3Called = true;
        };

        event1.Raise(this, new TestEventArgs());

        Assert.That(handler1Called && handler2Called && handler3Called);
    }

    [Test]
    public void RaiseEvent_MultipleHandlers_CancelInHandler_SubsequentHandlersNotCalled()
    {
        EventHandler<TestEventArgs> event1 = null;

        var handler1Called = false;
        event1 += (_, _) =>
        {
            handler1Called = true;
        };

        var handler2Called = false;
        event1 += (_, args) =>
        {
            handler2Called = true;
            args.Cancel = true;
        };

        var handler3Called = false;
        event1 += (_, _) =>
        {
            handler3Called = true;
        };

        event1.Raise(this, new TestEventArgs());

        Assert.That(handler1Called && handler2Called && !handler3Called);
    }

    [Test]
    public void RaiseEvent_MultipleHandlers_ThrowInHandler_AllHandlersCalledAndThrows()
    {
        EventHandler<TestEventArgs> event1 = null;

        var handler1Called = false;
        event1 += (_, _) =>
        {
            handler1Called = true;
            throw new Exception("Exception in handler 1");
        };

        var handler2Called = false;
        event1 += (_, _) =>
        {
            handler2Called = true;
            throw new Exception("Exception in handler 2");
        };

        var handler3Called = false;
        event1 += (_, _) =>
        {
            handler3Called = true;
            throw new Exception("Exception in handler 3");
        };

        Assert.Throws<AggregateException>(() => event1.Raise(this, new TestEventArgs()));

        Assert.That(handler1Called && handler2Called && handler3Called);
    }
}