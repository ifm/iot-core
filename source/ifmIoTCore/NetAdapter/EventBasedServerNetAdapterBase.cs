namespace ifmIoTCore.NetAdapter
{
    using System;
    using Messages;
    using Utilities;

    public abstract class EventBasedServerNetAdapterBase : ServerNetAdapterBase, IEventBasedServerNetAdapter
    {
        public event EventHandler<RequestMessageEventArgs> RequestMessageReceived;
        public event EventHandler<EventMessageEventArgs> EventMessageReceived;

        protected virtual void RaiseRequestReceived(RequestMessageEventArgs requestEventArgs)
        {
            this.RequestMessageReceived.Raise(this, requestEventArgs, true);
        }

        protected virtual void RaiseEventReceived(EventMessage eventMessage)
        {
            this.EventMessageReceived.Raise(this, new EventMessageEventArgs(eventMessage), true);
        }
    }
}