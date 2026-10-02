using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ifm.IoTCore.Common;
using ifm.IoTCore.Common.Exceptions;
using ifm.IoTCore.MessageConverter.Contracts;
using ifm.IoTCore.NetAdapter.Http.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal static class AllowedServicesMiddlewareExtensions
    {
        public static void UseAllowedServicesMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<AllowedServicesMiddleware>();
        }
    }

    internal class AllowedServicesMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<AllowedServicesMiddlewareOptions> _options;
        private readonly MiddlewareExceptionHandlerService _middlewareExceptionHandlerService;
        private readonly IMessageConverter _messageConverter;

        public AllowedServicesMiddleware(RequestDelegate next,
            IOptions<AllowedServicesMiddlewareOptions> options,
            MiddlewareExceptionHandlerService middlewareExceptionHandlerService,
            IMessageConverter messageConverter)
        {
            this._next = next;
            this._options = options;
            _middlewareExceptionHandlerService = middlewareExceptionHandlerService;
            _messageConverter = messageConverter ?? throw new ArgumentNullException(nameof(messageConverter));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (_options.Value.AllowedServicesMode == AllowedServicesMode.All)
            {
                await _next(httpContext);
            }
            else if (_options.Value.AllowedServicesMode == AllowedServicesMode.GetIdentityOnly)
            {
                if (httpContext.Request.Method == "GET")
                {
                    if (httpContext.Request.Path.ToString().Equals("/getidentity"))
                    {
                        await _next(httpContext);
                    }
                    else
                    {
                        await _middlewareExceptionHandlerService.HandleException(httpContext,
                            new IoTCoreException(ResponseCodes.BadRequest, "Only getidentity service allowed."));
                    }
                }
                else if (httpContext.Request.Method == "POST")
                {
                    httpContext.Request.EnableBuffering();

                    await using var inputStream = new MemoryStream();

                    var position = httpContext.Request.Body.Position;
                    await httpContext.Request.Body.CopyToAsync(inputStream);
                    httpContext.Request.Body.Position = position;

                    if (inputStream.Length == 0)
                    {
                        throw new HttpListenerException((int)HttpStatusCode.BadRequest);
                    }

                    var requestString = Encoding.UTF8.GetString(inputStream.ToArray());
                    var message = _messageConverter.Deserialize(requestString);

                    if (message.Address.EndsWith("/getidentity"))
                    {
                        await _next(httpContext);
                    }
                    else
                    {
                        await _middlewareExceptionHandlerService.HandleException(httpContext,
                            new IoTCoreException(ResponseCodes.BadRequest, "Only getidentity service allowed."));
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            else
            {
                await _middlewareExceptionHandlerService.HandleException(httpContext,
                    new IoTCoreException(ResponseCodes.BadRequest, "Unknown settings for allowed service."));
            }
        }
    }

    public class AllowedServicesMiddlewareOptions
    {
        public AllowedServicesMode AllowedServicesMode { get; set; }
    }
}
