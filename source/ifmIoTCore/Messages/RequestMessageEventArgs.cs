namespace ifmIoTCore.Messages
{
    public class RequestMessageEventArgs
    {
        public RequestMessage Request { get; }
        public ResponseMessage Response { get; set; }

        public RequestMessageEventArgs(RequestMessage request, ResponseMessage response = null)
        {
            Request = request;
            Response = response;
        }
    }
}
