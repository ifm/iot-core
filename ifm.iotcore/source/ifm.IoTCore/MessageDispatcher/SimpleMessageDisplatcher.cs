namespace ifm.IoTCore.MessageDispatcher;

using Common;
using Common.Exceptions;
using Contracts;
using ifm.Common;
using ifm.Common.Variant;
using ifm.IoTCore.ElementManager.Contracts;
using ifm.IoTCore.ElementManager.Contracts.Elements;
using Message;
using System;

public class SimpleMessageDispatcher(IElementManager elementManager) : IMessageDispatcher
{
    public Message HandleRequest(Message message)
    {
        ExceptionHelpers.ThrowIfNull(message, nameof(message));

        Message response;
        try
        {
            response = InvokeService(GetServiceElement(message.Address), message);
        }
        catch (IoTCoreException e)
        {
            response = new Message(e.ResponseCode,
                message.Cid,
                message.Reply ?? message.Address,
                Variant.FromObject(new ErrorInfoResponseServiceData(e.Message, e.ErrorCode, e.ErrorDetails, e.HexError)));
        }
        catch (Exception e)
        {
            response = new Message(ResponseCodes.InternalError,
                message.Cid,
                message.Reply ?? message.Address,
                Variant.FromObject(new ErrorInfoResponseServiceData(e.Message)));
        }

        return response;
    }

    public void HandleEvent(Message message)
    {
        ExceptionHelpers.ThrowIfNull(message, nameof(message));

        InvokeService(GetServiceElement(message.Address), message);
    }

    private IServiceElement GetServiceElement(string address)
    {
        var element = elementManager.GetElementByAddress(address);
        if (element == null)
        {
            throw new NotFoundException($"Element '{address}' not found");
        }
        return element as IServiceElement ?? throw new BadRequestException($"Element '{address}' is not a service");
    }

    private static Message InvokeService(IServiceElement serviceElement, Message message)
    {
        try
        {
            var data = serviceElement.Invoke(message.Data);
            return new Message(ResponseCodes.Success, message.Cid, message.Reply ?? message.Address, data);
        }
        catch (IoTCoreException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new IoTCoreException(ResponseCodes.InternalError, $"Service '{serviceElement.Address}' failed", e.Message);
        }
    }
}
