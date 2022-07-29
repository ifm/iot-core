namespace ifmIoTCore.NetAdapter.Base
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Messages;

    public abstract class ServerNetAdapterBase : IServerNetAdapter
    {
        public abstract string Scheme { get; }
        public abstract string Format { get; }
        public abstract Uri Uri { get; }
        public abstract bool IsListening { get; }
        public abstract void Start();
        public abstract Task StartAsync();
        public abstract void Stop();
        public abstract Task StopAsync();

        public virtual void Dispose()
        {
        }

        public event EventHandler<RequestMessageEventArgs> RequestReceived;
        public event EventHandler<EventMessageEventArgs> EventReceived;

        protected virtual void RaiseRequestReceived(RequestMessageEventArgs requestEventArgs)
        {
            RequestReceived.Raise(this, requestEventArgs, true);
        }

        protected virtual void RaiseEventReceived(Message eventMessage)
        {
            EventReceived.Raise(this, new EventMessageEventArgs(eventMessage), true);
        }
    }
}