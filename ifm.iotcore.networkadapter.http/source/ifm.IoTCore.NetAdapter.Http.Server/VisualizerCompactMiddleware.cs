using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ifm.IoTCore.MessageConverter.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal static class VisualizerCompactMiddlewareExtensions
    {
        public static void UseVisualizerCompactMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<VisualizerCompactMiddleware>();
        }
    }

    internal class VisualizerCompactMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<VisualizerCompactMiddlewareOptions> _options;
        private readonly MiddlewareExceptionHandlerService _middlewareExceptionHandlerService;

        public VisualizerCompactMiddleware(RequestDelegate next, 
            IOptions<VisualizerCompactMiddlewareOptions> options,
            IMessageConverter messageConverter, 
            MiddlewareExceptionHandlerService middlewareExceptionHandlerService)
        {
            _next = next;
            _options = options;
            _middlewareExceptionHandlerService = middlewareExceptionHandlerService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try {

                if (httpContext.Request.Method == "GET")
                {
                    if (_options.Value.MappedPaths != null &&
                        _options.Value.MappedPaths.Contains(httpContext.Request.Path.ToString()))
                    {
                        if (_options.Value.FilePath != null)
                        {
                            var webPage = await File.ReadAllBytesAsync(_options.Value.FilePath);

                            httpContext.Response.Headers["Content-Encoding"] = "gzip";
                            httpContext.Response.ContentType = "text/html";
                            httpContext.Response.ContentLength = webPage.Length;
                            await httpContext.Response.Body.WriteAsync(webPage, 0, webPage.Length);

                            return;
                        }
                        else
                        {
                            throw new FileNotFoundException("File path for VisualizerCompactMiddleware is not set.");
                        }
                    }
                }

                await _next(httpContext);
            }
            catch (Exception e)
            {
                await _middlewareExceptionHandlerService.HandleException(httpContext, e);
            }
        }
    }

    public class VisualizerCompactMiddlewareOptions
    {
        public string? FilePath { get; set; }
        public string[]? MappedPaths { get; set; }
    }

    
}
