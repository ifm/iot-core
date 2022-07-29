namespace ifmIoTCore
{
    using Common.Variant;
    using Messages;

    public static class IoTCoreExtensions
    {
        /// <summary>
        /// Handles a request on a service element in the IoTCore tree
        /// </summary>
        /// <param name="ioTCore">The extended instance.</param>
        /// <param name="cid">The cid for the request</param>
        /// <param name="address">The target service address for the request</param>
        /// <param name="data">The data for the request</param>
        /// <param name="reply">The reply address for the request's response</param>
        /// <returns>The response message</returns>
        public static Message HandleRequest(this IIoTCore ioTCore, int cid, string address, Variant data = null, string reply = null)
        {
            return ioTCore.HandleRequest(new Message(RequestCodes.Request, cid, address, data, reply));
        }
    }
}
