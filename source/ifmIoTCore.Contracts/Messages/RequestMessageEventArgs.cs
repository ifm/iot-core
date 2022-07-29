namespace ifmIoTCore.Messages
{
    public class RequestMessageEventArgs
    {
        public Message RequestMessage { get; }

        public Message ResponseMessage { get; set; }

        public RequestMessageEventArgs(Message requestMessage, Message responseMessage = null)
        {
            RequestMessage = requestMessage;
            ResponseMessage = responseMessage;
        }
    }
}
