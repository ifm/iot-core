using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ifm.Common.Variant;
using ifm.IoTCore.Common;
using ifm.IoTCore.Common.Exceptions;
using ifm.IoTCore.MessageConverter.Contracts;
using Microsoft.AspNetCore.Http;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal class MiddlewareExceptionHandlerService
    {
        private readonly IMessageConverter _messageConverter;
        private readonly IMessageFromExceptionBuilder _messageFromExceptionBuilder;

        public MiddlewareExceptionHandlerService(IMessageConverter messageConverter,
            IMessageFromExceptionBuilder messageFromExceptionBuilder)
        {
            _messageConverter = messageConverter;
            _messageFromExceptionBuilder = messageFromExceptionBuilder;
        }

        public async Task HandleException(HttpContext httpContext, Exception e)
        {
            var responseMessage = _messageFromExceptionBuilder.BuildMessageFromException(e);

            if (e is HttpListenerException listenerException)
            {
                httpContext.Response.StatusCode = listenerException.ErrorCode;
            }
            else if (e is AggregateException)
            {
                httpContext.Response.StatusCode = 500;
            }
            else if (e is IoTCoreException ioTCoreException)
            {
                httpContext.Response.StatusCode = ioTCoreException.ResponseCode;
            }
            else
            {
                httpContext.Response.StatusCode = 500;
            }

            var responseString = _messageConverter.Serialize(responseMessage);
            httpContext.Response.ContentType = _messageConverter.ContentType;

            await httpContext.Response.WriteAsync(responseString, Encoding.UTF8);
        }

    }
}
