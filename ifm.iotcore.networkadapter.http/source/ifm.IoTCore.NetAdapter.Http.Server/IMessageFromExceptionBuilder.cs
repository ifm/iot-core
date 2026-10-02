using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ifm.Common.Variant;
using ifm.IoTCore.Common;
using ifm.IoTCore.Common.Exceptions;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal interface IMessageFromExceptionBuilder
    {
        Message.Message BuildMessageFromException(Exception e);
    }

    internal class MessageFromExceptionBuilder : IMessageFromExceptionBuilder
    {
        public Message.Message BuildMessageFromException(Exception e)
        {
            Message.Message responseMessage;
            if (e is HttpListenerException listenerException)
            {
                responseMessage = new Message.Message(listenerException.ErrorCode,
                    0,
                    null,
                    null);
            }
            else if (e is AggregateException aggregateException)
            {
                var errorMessages = new List<string>();
                if (aggregateException.InnerException?.Message != null)
                {
                    errorMessages.Add(aggregateException.InnerException.Message);
                }

                if (aggregateException.InnerExceptions.Any())
                {
                    foreach (var innerException in aggregateException.InnerExceptions)
                    {
                        errorMessages.Add(innerException.Message);
                    }
                }

                var serviceException = new IoTCoreException(ResponseCodes.InternalError, $"{aggregateException.Message}, {string.Join(",", errorMessages)}");

                responseMessage = new Message.Message(
                    serviceException.ResponseCode,
                    0,
                    null,
                    null);
            }
            else if (e is IoTCoreException ioTCoreException)
            {
                responseMessage = new Message.Message(ioTCoreException.ResponseCode,
                    0,
                    null,
                    CreateErrorResponse(ioTCoreException.Message, ioTCoreException.ErrorCode, ioTCoreException.ErrorDetails));
            }
            else
            {
                responseMessage = new Message.Message(ResponseCodes.InternalError,
                    0,
                    null,
                    null);
            }

            return responseMessage;
        }


        private static Variant CreateErrorResponse(string msg, int? code = null, string? details = null)
        {
            var ret = new VariantObject { { "msg", new VariantValue(msg) } };
            if (code != null)
            {
                ret.Add("code", new VariantValue(code.Value));
            }
            if (details != null)
            {
                ret.Add("details", new VariantValue(details));
            }
            return ret;
        }
    }
}
