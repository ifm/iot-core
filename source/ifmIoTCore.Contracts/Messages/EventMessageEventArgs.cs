namespace ifmIoTCore.Messages
{
    public class EventMessageEventArgs
    {
        public Message EventMessage { get; }

        public EventMessageEventArgs(Message eventMessage)
        {
            EventMessage = eventMessage;
        }
    }
}
