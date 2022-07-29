namespace ifmIoTCore.Elements.EventArguments
{
    using System;

    public class SubscriptionEventArgs : EventArgs
    {
        public int EventNumber { get; }

        public SubscriptionEventArgs(int eventNumber)
        {
            EventNumber = eventNumber;
        }
    }
}
