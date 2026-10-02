using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ifm.IoTCore.Common;
using ifm.IoTCore.Common.Exceptions;
using ifm.IoTCore.MessageConverter.Contracts;
using ifm.IoTCore.MessageDispatcher.Contracts;
using ifm.IoTCore.NetAdapter.Http.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal static class IoTCoreApiMiddlewareExtensions
    {
        public static void UseIoTCoreApiMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<IoTCoreApiMiddleware>();
        }
    }

    internal class IoTCoreApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<IoTCoreApiOptions> _options;
        private readonly IMessageConverter _messageConverter;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly MiddlewareExceptionHandlerService _middlewareExceptionHandlerService;

        public IoTCoreApiMiddleware(RequestDelegate next, IOptions<IoTCoreApiOptions> options, IMessageConverter messageConverter, IMessageDispatcher messageDispatcher, MiddlewareExceptionHandlerService middlewareExceptionHandlerService)
        {
            _next = next;
            _options = options;
            _messageConverter = messageConverter;
            _messageDispatcher = messageDispatcher;
            _middlewareExceptionHandlerService = middlewareExceptionHandlerService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {

                if (httpContext.Request.Method == "GET")
                {
                    string? address;

                    switch (_options.Value.GetRequestPathEscapeMode)
                    {
                        case RequestPathEscapeModeEnum.Default:
                        case RequestPathEscapeModeEnum.Escape:
                            address = httpContext.Request.Path;
                            break;
                        case RequestPathEscapeModeEnum.DoNotEscape:
                            address = httpContext.Request.Path.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var requestMessage = new Message.Message(RequestCodes.Request,
                        1,
                        address,
                        null);

                    var response = _messageDispatcher.HandleRequest(requestMessage);

                    var responseString = _messageConverter.Serialize(response);
                    httpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    httpContext.Response.ContentType = _messageConverter.ContentType;
                    await httpContext.Response.WriteAsync(responseString, Encoding.UTF8);
                }
                else if (httpContext.Request.Method == "POST")
                {
                    await using var inputStream = new MemoryStream();
                    await httpContext.Request.Body.CopyToAsync(inputStream);

                    if (inputStream.Length == 0)
                    {
                        throw new HttpListenerException((int)HttpStatusCode.BadRequest);
                    }

                    var requestString = Encoding.UTF8.GetString(inputStream.ToArray());
                    var message = _messageConverter.Deserialize(requestString);

                    string responseString;
                    if (message.Code == RequestCodes.Request)
                    {
                        var responseMessage = _messageDispatcher.HandleRequest(message);
                        responseString = _messageConverter.Serialize(responseMessage);
                    }
                    else if (message.Code == RequestCodes.Event)
                    {
                        _messageDispatcher.HandleEvent(message);
                        responseString = string.Empty;
                    }
                    else
                    {
                        throw new IoTCoreException(ResponseCodes.BadRequest,
                            $"Invalid message code {message.Code}");
                    }

                    httpContext.Response.ContentType = _messageConverter.ContentType;
                    await httpContext.Response.WriteAsync(responseString, Encoding.UTF8);
                }
                else
                {
                    throw new HttpListenerException((int)HttpStatusCode.MethodNotAllowed);
                }

            }
            catch (Exception e)
            {
                await _middlewareExceptionHandlerService.HandleException(httpContext, e);
            }
        }
    }

    public class IoTCoreApiOptions
    {
        public RequestPathEscapeModeEnum GetRequestPathEscapeMode { get; set; }
    }
}
