using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    internal static class VisualizerEnhancedMiddlewareExtensions
    {
        public static void UseVisualizerEnhancedMiddleware(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<VisualizerEnhancedMiddleware>();
        }
    }

    internal class VisualizerEnhancedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOptions<VisualizerEnhancedMiddlewareOptions> _options;

        public VisualizerEnhancedMiddleware(RequestDelegate next, IOptions<VisualizerEnhancedMiddlewareOptions> options)
        {
            this._next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Method != "GET")
            {
                await _next(httpContext);
            }
            else if (_options.Value is { ClientRoutePrefix: not null, BasePath: not null } && httpContext.Request.Path.ToString().ToLower().StartsWith(_options.Value.ClientRoutePrefix))
            {
                await ReplyWithWebPageAsync(httpContext, _options.Value.BasePath, "/index.html");
            }
            else if (_options.Value is { MappedPaths: not null, BasePath: not null } && _options.Value.MappedPaths.Contains(httpContext.Request.Path.ToString()))
            {
                await ReplyWithWebPageAsync(httpContext, _options.Value.BasePath, "/index.html");
            }
            else if (_options.Value is { BasePath: not null } && IsValidFile(_options.Value.BasePath, httpContext.Request.Path.ToString()))
            {
                await ReplyWithWebPageAsync(httpContext, _options.Value.BasePath, httpContext.Request.Path.ToString());
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task ReplyWithWebPageAsync(HttpContext context, string baseDir, string path)
        {
            var combinedPath = CombinePaths(baseDir, path);
            var webPage = await File.ReadAllBytesAsync(combinedPath);

            if (TryCreateContentTypeByPath(combinedPath, out var contentType))
            {
                context.Response.ContentType = contentType;
            }

            context.Response.ContentLength = webPage.Length;
            await context.Response.Body.WriteAsync(webPage, 0, webPage.Length);
        }

        private static string CombinePaths(string baseDir, string path)
        {
            var parts = path.Split('/');
            return parts.Aggregate(baseDir, Path.Combine);
        }
        private static bool TryCreateContentTypeByPath(string fullPath, out string? contentType)
        {
            var fileExtension = Path.GetExtension(fullPath);
            var fileType = fileExtension.Substring(1);

            contentType = null;

            switch (fileType)
            {
                case "htm": goto case "html";
                case "shtml": goto case "html";
                case "html": contentType = "text/html"; break;
                case "css": contentType = "text/css"; break;
                case "xml": contentType = "text/xml"; break;
                case "gif": contentType = "image/gif"; break;
                case "jpg": goto case "jpeg";
                case "jpeg": contentType = "image/jpeg"; break;
                case "js": contentType = "application/javascript"; break;
                case "atom": contentType = "application/atom+xml"; break;
                case "rss": contentType = "application/rss+xml"; break;

                case "mml": contentType = "text/mathml"; break;
                case "txt": contentType = "text/plain"; break;
                case "jad": contentType = "text/vnd.sun.j2me.app-descriptor"; break;
                case "wml": contentType = "text/vnd.wap.wml"; break;
                case "htc": contentType = "text/x-component"; break;

                case "avif": contentType = "image/avif"; break;
                case "png": contentType = "image/png"; break;
                case "svgz": goto case "svg";
                case "svg": contentType = "image/svg+xml"; break;
                case "tif": goto case "tiff";
                case "tiff": contentType = "image/tiff"; break;
                case "wbmp": contentType = "image/vnd.wap.wbmp"; break;
                case "webp": contentType = "image/webp"; break;
                case "ico": contentType = "image/x-icon"; break;
                case "jng": contentType = "image/x-jng"; break;
                case "bmp": contentType = "image/x-ms-bmp"; break;

                case "woff": contentType = "font/woff"; break;
                case "woff2": contentType = "image/woff2"; break;

                case "war": goto case "jar";
                case "ear": goto case "jar";
                case "jar": contentType = "application/java-archive"; break;
                case "json": contentType = "application/json"; break;
                case "hqx": contentType = "application/mac-binhex40"; break;
                case "doc": contentType = "application/msword"; break;
                case "pdf": contentType = "application/pdf"; break;
                case "eps": goto case "ps";
                case "ai": goto case "ps";
                case "ps": contentType = "application/postscript"; break;
                case "rtf": contentType = "application/rtf"; break;
                case "m3u8": contentType = "application/vnd.apple.mpegurl"; break;
                case "kml": contentType = "application/vnd.google-earth.kml+xml"; break;
                case "kmz": contentType = "application/vnd.google-earth.kmz"; break;
                case "xls": contentType = "application/vnd.ms-excel"; break;
                case "eot": contentType = "application/vnd.ms-fontobject"; break;
                case "ppt": contentType = "application/vnd.ms-powerpoint"; break;
                case "odg": contentType = "application/vnd.oasis.opendocument.graphics"; break;
                case "odp": contentType = "application/vnd.oasis.opendocument.presentation"; break;
                case "ods": contentType = "application/vnd.oasis.opendocument.spreadsheet"; break;
                case "odt": contentType = "application/vnd.oasis.opendocument.text"; break;
                case "pptx": contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; break;
                case "xlsx": contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                case "docx": contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                case "wmlc": contentType = "application/vnd.wap.wmlc"; break;
                case "wasm": contentType = "application/wasm"; break;
                case "7z": contentType = "application/x-7z-compressed"; break;
                case "cco": contentType = "application/x-cocoa"; break;
                case "jardiff": contentType = "application/x-java-archive-diff"; break;
                case "jnlp": contentType = "application/x-java-jnlp-file"; break;
                case "run": contentType = "application/x-makeself"; break;
                case "pl": goto case "pm";
                case "pm": contentType = "application/x-perl"; break;
                case "prc": goto case "pdb";
                case "pdb": contentType = "application/x-pilot"; break;
                case "rar": contentType = "application/x-rar-compressed"; break;
                case "rpm": contentType = "application/x-redhat-package-manager"; break;
                case "sea": contentType = "application/x-sea"; break;
                case "swf": contentType = "application/x-shockwave-flash"; break;
                case "sit": contentType = "application/x-stuffit"; break;
                case "tcl": goto case "tcl";
                case "tk": contentType = "application/x-tcl"; break;
                case "der": goto case "crt";
                case "pem": goto case "crt";
                case "crt": contentType = "application/x-x509-ca-cert"; break;
                case "xpi": contentType = "application/x-xpinstall"; break;
                case "xhtml": contentType = "application/xhtml+xml"; break;
                case "xspf": contentType = "application/xspf+xml"; break;
                case "zip": contentType = "application/zip"; break;

                case "bin": goto case "dll";
                case "exe": goto case "dll";
                case "dll": contentType = "application/octet-stream"; break;
                case "deb": contentType = "application/octet-stream"; break;
                case "dmg": contentType = "application/octet-stream"; break;
                case "iso": goto case "img";
                case "img": contentType = "application/octet-stream"; break;
                case "msi": goto case "msm";
                case "msp": goto case "msm";
                case "msm": contentType = "application/octet-stream"; break;

                case "mid": goto case "kar";
                case "midi": goto case "kar";
                case "kar": contentType = "audio/midi"; break;
                case "mp3": contentType = "audio/mpeg"; break;
                case "ogg": contentType = "audio/ogg"; break;
                case "m4a": contentType = "audio/x-m4a"; break;
                case "ra": contentType = "audio/x-realaudio"; break;

                case "3gpp": goto case "3gp";
                case "3gp": contentType = "video/3gpp"; break;
                case "ts": contentType = "video/mp2t"; break;
                case "mp4": contentType = "video/mp4"; break;
                case "mpeg": goto case "mpg";
                case "mpg": contentType = "video/mpeg"; break;
                case "mov": contentType = "video/quicktime"; break;
                case "webm": contentType = "video/webm"; break;
                case "flv": contentType = "video/x-flv"; break;
                case "m4v": contentType = "video/x-m4v"; break;
                case "mng": contentType = "video/x-mng"; break;
                case "asx": goto case "asf";
                case "asf": contentType = "video/x-ms-asf"; break;
                case "wmv": contentType = "video/x-ms-wmv"; break;
                case "avi": contentType = "video/x-msvideo"; break;
            }

            return !string.IsNullOrEmpty(contentType);
        }

        private static bool IsValidFile(string baseDir, string filePath)
        {
            var absoluteBaseDir = Path.GetFullPath(baseDir);
            var absoluteFilePath = Path.GetFullPath(Path.Combine(absoluteBaseDir, filePath.TrimStart('/')));

            if (!absoluteFilePath.StartsWith(absoluteBaseDir))
            {
                return false;
            }

            return File.Exists(absoluteFilePath);
        }
    }

    public class VisualizerEnhancedMiddlewareOptions
    {
        public string? BasePath { get; set; }
        public string? ClientRoutePrefix { get; set; }
        public string[]? MappedPaths { get; set; }
    }


}
