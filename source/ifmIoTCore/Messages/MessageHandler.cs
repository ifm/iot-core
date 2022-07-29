namespace ifmIoTCore.Messages
{
    using System;
    using System.Linq;
    using Common;
    using Common.Variant;
    using Elements;
    using Elements.ServiceData.Responses;
    using Exceptions;
    using Logger;
    using Resources;
    using Utilities;

    internal class MessageHandler : IMessageHandler
    {
        private readonly IElementManager _elementManager;
        public ILogger Logger { get; }
        public event EventHandler<RequestMessageEventArgs> RequestMessageReceived;
        public event EventHandler<RequestMessageEventArgs> RequestMessageResponded;
        public event EventHandler<EventMessageEventArgs> EventMessageReceived;

        public MessageHandler(IElementManager elementManager, ILogger logger)
        {
            _elementManager = elementManager;
            Logger = logger;
        }
        
        public Message HandleRequest(Message message)
        {
            if (message == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(message)));
            }

            Logger?.Debug($"Receive request: code={message.Code}, address={message.Address}, data={message.Data}");

            var response = RaiseRequestMessageEvent(message);
            if (response != null)
            {
                return response;
            }

            if (string.IsNullOrEmpty(message.Address))
            {
                Logger?.Error(Resource1.InvalidMessageAddress);
                response = CreateErrorResponseMessage(message, 
                    ResponseCodes.DataInvalid, 
                    Resource1.InvalidMessageAddress);
            }
            else if (!Helpers.CheckDeviceName(message.Address, _elementManager.Root.Identifier))
            {
                var errorMessage = string.Format(Resource1.InvalidDeviceName, message.Address);
                Logger?.Error(errorMessage);
                response = CreateErrorResponseMessage(message,
                    ResponseCodes.NotFound,
                    errorMessage);
            }
            else
            {
                var element = _elementManager.GetElementByAddress(Helpers.RemoveDeviceName(message.Address));
                if (element == null)
                {
                    var errorMessage = string.Format(Resource1.ElementNotFound, message.Address);
                    Logger?.Error(errorMessage);
                    response = CreateErrorResponseMessage(message,
                        ResponseCodes.NotFound,
                        errorMessage);
                }
                else
                {
                    if (element is IServiceElement serviceElement)
                    {
                        try
                        {
                            Logger?.Debug($"Calling service '{message.Address}'");

                            var data = serviceElement.Invoke(message.Data, message.Cid);

                            response = CreateSuccessResponseMessage(message, data);
                        }
                        catch (IoTCoreException e)
                        {
                            var errorMessage = string.Format(Resource1.ServiceExecutionFailed, element.Address, e.ErrorInfo.Message);
                            Logger?.Error(errorMessage);
                            response = CreateErrorResponseMessage(message,
                                e.ResponseCode,
                                errorMessage,
                                e.ErrorInfo.Code,
                                e.ErrorInfo.Details);
                        }
                        catch (Exception e)
                        {
                            var errorMessage = string.Format(Resource1.ServiceExecutionFailed, element.Address, e.Message);
                            Logger?.Error(errorMessage);
                            response = CreateErrorResponseMessage(message,
                                ResponseCodes.InternalError,
                                errorMessage,
                                e.HResult);
                        }
                    }
                    else
                    {
                        var errorMessage = string.Format(Resource1.ElementNotService, element.Address);
                        Logger?.Error(errorMessage);
                        response = CreateErrorResponseMessage(message,
                            ResponseCodes.BadRequest,
                            errorMessage);
                    }
                }
            }

            RaiseResponseMessageEvent(message, response);

            Logger?.Debug($"Return response: code={response.Code}, address={response.Address}, data={response.Data}");

            return response;
        }

        private static Message CreateSuccessResponseMessage(Message message, Variant data)
        {
            return new Message(ResponseCodes.Success,
                message.Cid,
                message.Reply ?? message.Address,
                data);
        }

        private static Message CreateErrorResponseMessage(Message message, 
            int responseCode, 
            string errorMessage, 
            int? errorCode = null, 
            string errorDetails = null)
        {
            return new Message(responseCode,
                message.Cid,
                message.Reply ?? message.Address,
                Helpers.VariantFromObject(new ErrorInfoResponseServiceData(errorMessage, errorCode, errorDetails)));
        }

        public void HandleEvent(Message message)
        {
            if (message == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(message)));
            }

            Logger?.Debug($"Receive event: code={message.Code}, address={message.Address}, data={message.Data}");

            RaiseEventMessageEvent(message);

            if (!Helpers.CheckDeviceName(message.Address, _elementManager.Root.Identifier))
            {
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, message.Address));
            }

            var element = _elementManager.GetElementByAddress(Helpers.RemoveDeviceName(message.Address));
            if (element != null)
            {
                if (element is IServiceElement serviceElement)
                {
                    try
                    {
                        Logger?.Debug($"Calling service '{message.Address}'.");

                        serviceElement.Invoke(message.Data, message.Cid);
                    }
                    catch (Exception e)
                    {
                        var errorMessage = string.Format(Resource1.ServiceExecutionFailed, element.Address, e.Message, e.HResult);
                        Logger?.Error(errorMessage);
                        throw;
                    }
                }
                else
                {
                    Logger?.Error($"{string.Format(Resource1.ElementNotService, element.Identifier)} (Code={ResponseCodes.BadRequest})");
                    throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.ElementNotService, message.Address));
                }
            }
            else
            {
                Logger?.Error($"{string.Format(Resource1.ElementNotFound, message.Address)} (Code={ResponseCodes.NotFound})");
                throw new IoTCoreException(ResponseCodes.NotFound, string.Format(Resource1.ElementNotFound, message.Address));
            }
        }

        private Message RaiseRequestMessageEvent(Message request)
        {
            Message response = null;
            var eventHandlers = RequestMessageReceived?.GetInvocationList();
            if (eventHandlers != null)
            {
                foreach (var eventHandler in eventHandlers.Cast<EventHandler<RequestMessageEventArgs>>())
                {
                    try
                    {
                        var args = new RequestMessageEventArgs(request);
                        eventHandler(this, args);
                        if (args.ResponseMessage != null)
                        {
                            response = args.ResponseMessage;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        response = new Message(ResponseCodes.InternalError,
                            request.Cid,
                            request.Reply ?? request.Address,
                            Helpers.VariantFromObject(new ErrorInfoResponseServiceData(e.Message)));
                    }
                }
            }
            return response;
        }

        private void RaiseEventMessageEvent(Message eventMessage)
        {
            EventMessageReceived.Raise(this, new EventMessageEventArgs(eventMessage));
        }

        private void RaiseResponseMessageEvent(Message requestMessage, Message responseMessage)
        {
            RequestMessageResponded.Raise(this, new RequestMessageEventArgs(requestMessage, responseMessage));
        }
    }
}