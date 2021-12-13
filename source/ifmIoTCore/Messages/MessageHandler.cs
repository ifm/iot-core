using System;
using System.Linq;
using ifmIoTCore.Elements;
using ifmIoTCore.Elements.ServiceData.Responses;
using ifmIoTCore.Exceptions;
using ifmIoTCore.Resources;
using ifmIoTCore.Utilities;
using Newtonsoft.Json.Linq;

namespace ifmIoTCore.Messages
{
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

        public ResponseMessage HandleRequest(int cid, string address, JToken data, string reply)
        {
            return HandleRequest(new RequestMessage(cid, address, data, reply));
        }

        public ResponseMessage HandleRequest(RequestMessage message)
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

            if (Helpers.CheckDeviceName(message.Address, _elementManager.GetRootElement().Identifier))
            {
                var element = _elementManager.GetElementByAddress(Helpers.RemoveDeviceName(message.Address));
                if (element != null)
                {
                    if (element is IServiceElement serviceElement)
                    {
                        try
                        {
                            Logger?.Debug($"Calling service '{message.Address}'");

                            var data = serviceElement.Invoke(message.Data, message.Cid);

                            response = new ResponseMessage(ResponseCodes.Success,
                                message.Cid,
                                string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                                data);
                        }
                        catch (IoTCoreException e)
                        {
                            var errorMessage = string.Format(Resource1.ServiceExecutionFailed, element.Address, e.Message, e.Code);
                            Logger?.Error(errorMessage);
                            response = new ResponseMessage(e.Code,
                                message.Cid,
                                string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                                Helpers.ToJson(new ErrorInfoResponseServiceData(errorMessage, e.Code)));
                        }
                        catch (ServiceException e)
                        {
                            var errorMessage = string.Format(Resource1.ServiceExecutionFailed, element.Address, e.Message, e.Code);
                            Logger?.Error(errorMessage);
                            response = new ResponseMessage(e.Code,
                                message.Cid,
                                string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                                Helpers.ToJson(new ErrorInfoResponseServiceData(errorMessage, e.Code, e.Hint)));
                        }
                        catch (Exception e)
                        {
                            var errorMessage = string.Format(Resource1.ServiceExecutionFailed, element.Address, e.Message, e.HResult);
                            Logger?.Error(errorMessage);
                            response = new ResponseMessage(ResponseCodes.InternalError,
                                message.Cid,
                                string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                                Helpers.ToJson(new ErrorInfoResponseServiceData(errorMessage)));
                        }
                    }
                    else
                    {
                        Logger?.Error($"{string.Format(Resource1.ElementNotService, element.Identifier)} (Code={ResponseCodes.BadRequest})");
                        response = new ResponseMessage(ResponseCodes.BadRequest,
                            message.Cid,
                            string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                            Helpers.ToJson(new ErrorInfoResponseServiceData(string.Format(Resource1.ElementNotService, element.Identifier))));
                    }
                }
                else
                {
                    Logger?.Error($"{string.Format(Resource1.ElementNotFound, message.Address)} (Code={ResponseCodes.NotFound})");
                    response = new ResponseMessage(ResponseCodes.NotFound,
                        message.Cid,
                        string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                        Helpers.ToJson(new ErrorInfoResponseServiceData(string.Format(Resource1.ElementNotFound, message.Address))));
                }
            }
            else
            {
                Logger?.Error($"{string.Format(Resource1.InvalidDeviceName, message.Address)} (Code={ResponseCodes.NotFound})");
                response = new ResponseMessage(ResponseCodes.NotFound,
                    message.Cid,
                    string.IsNullOrEmpty(message.Reply) ? message.Address : message.Reply,
                    Helpers.ToJson(new ErrorInfoResponseServiceData(string.Format(Resource1.InvalidDeviceName, message.Address))));
            }

            RaiseResponseMessageEvent(message, response);

            Logger?.Debug($"Send response: code={response.Code}, address={response.Address}, data={response.Data}");

            return response;
        }

        public void HandleEvent(EventMessage message)
        {
            if (message == null)
            {
                throw new IoTCoreException(ResponseCodes.DataInvalid, string.Format(Resource1.ArgumentNullOrEmpty, nameof(message)));
            }

            Logger?.Debug($"Receive event: code={message.Code}, address={message.Address}, data={message.Data}");

            RaiseEventMessageEvent(message);

            if (!Helpers.CheckDeviceName(message.Address, _elementManager.GetRootElement().Identifier))
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

        private ResponseMessage RaiseRequestMessageEvent(RequestMessage request)
        {
            ResponseMessage response = null;
            var eventHandlers = RequestMessageReceived?.GetInvocationList();
            if (eventHandlers != null)
            {
                foreach (var eventHandler in eventHandlers.Cast<EventHandler<RequestMessageEventArgs>>())
                {
                    try
                    {
                        var args = new RequestMessageEventArgs(request);
                        eventHandler(this, args);
                        if (args.Response != null)
                        {
                            response = args.Response;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        response = new ResponseMessage(ResponseCodes.InternalError,
                            request.Cid,
                            string.IsNullOrEmpty(request.Reply) ? request.Address : request.Reply,
                            Helpers.ToJson(new ErrorInfoResponseServiceData(e.Message)));
                    }
                }
            }
            return response;
        }

        private void RaiseEventMessageEvent(EventMessage evt)
        {
            EventMessageReceived.Raise(this, new EventMessageEventArgs(evt));
        }

        private void RaiseResponseMessageEvent(RequestMessage request, ResponseMessage response)
        {
            RequestMessageResponded.Raise(this, new RequestMessageEventArgs(request, response));
        }
    }
}