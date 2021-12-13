namespace ifmIoTCore.Messages
{
    public class EventMessageEventArgs
    {
        public EventMessage EventMessage { get; }

        public EventMessageEventArgs(EventMessage eventMessage)
        {
            EventMessage = eventMessage;
        }
    }
}
