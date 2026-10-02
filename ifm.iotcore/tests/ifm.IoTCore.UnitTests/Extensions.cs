namespace ifm.IoTCore.UnitTests;

using Common;
using ifm.Common.Variant;
using Message;
using MessageDispatcher.Contracts;

public static class Extensions
{
    public static Message HandleRequest(this IMessageDispatcher messageDispatcher,
        int cid,
        string address,
        Variant data = null,
        string reply = null)
    {
        return messageDispatcher.HandleRequest(new Message(RequestCodes.Request, cid, address, data, reply));
    }
}