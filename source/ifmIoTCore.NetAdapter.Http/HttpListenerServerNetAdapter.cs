using System.Threading;

namespace ifmIoTCore.NetAdapter.Http
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Elements.ServiceData.Responses;
    using Exceptions;
    using Messages;
    using Resources;
    using Utilities;

    public class HttpListenerServerNetAdapter : EventBasedServerNetAdapterBase
    {
        private readonly HttpListener _httpListener;
        private readonly IConverter _converter;
        private readonly ILogger _logger;
        private bool _disposed;

        protected HttpListenerServerNetAdapter(Uri uri, IConverter converter, ILogger logger = null)
        {
            this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            this._converter = converter ?? throw new ArgumentNullException(nameof(converter));
            this.ConverterType = this._converter.Type;

            this._logger = logger;
            this._httpListener = new HttpListener();
            this._httpListener.Prefixes.Add($"{uri.Scheme}://{(uri.Host == IPAddress.Any.ToString() ? "*" : uri.Host)}:{uri.Port}/");
        }

        public bool IsListening => this._httpListener?.IsListening ?? false;
        public override string ConverterType { get; }
        public override Uri Uri { get; }
        
        public override void Start()
        {
            if (this.IsListening)
            {
                this.Stop();
            }

            this._httpListener.Start();
            Task.Run(this.Listen);
        }

        public override Task StartAsync()
        {
            return Task.Run(this.Start);
        }

        public override void Stop()
        {
            if (!this.IsListening) return;
            this._httpListener.Stop();
        }

        public override Task StopAsync()
        {
            return Task.Run(this.Stop);
        }

        public override void Dispose()
        {
            this._disposed = true;
            this._httpListener.Close();
            ((IDisposable)_httpListener).Dispose();
        }

        protected void HandleRequest(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "GET")
                {
                    if (context.Request.Url.AbsolutePath == "/")
                    {
                        if (!context.Request.HasEntityBody)
                        {
                            throw new Exception("GET Requests to '/' are not yet supported.");
                        }
                    }
                    else if (context.Request.Url.AbsolutePath == "/web/subscribe" ||
                             context.Request.Url.AbsolutePath == "/browse")
                    {
                        var webPage = this.GetIotCoreBrowserWebPage();
                        context.Response.AppendHeader("Content-Encoding", "gzip");
                        context.Response.ContentType = "text/html";
                        context.Response.ContentLength64 = webPage.Length;
                        context.Response.OutputStream.Write(webPage, 0, webPage.Length);
                    }
                    else
                    {
                        var requestMessage = new RequestMessage(1, context.Request.Url.AbsolutePath, null);
                        var requestMessageEventArgs = new RequestMessageEventArgs(requestMessage);

                        this.RaiseRequestReceived(requestMessageEventArgs);
                        var responseMessage = requestMessageEventArgs.Response;
                        var responseString = this._converter.Serialize(responseMessage);

                        context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                        context.Response.ContentType = this._converter.ContentType;
                        var bytesToSend = Encoding.UTF8.GetBytes(responseString);
                        context.Response.OutputStream.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                }
                else if (context.Request.HttpMethod == "POST")
                {
                    if (context.Request.HasEntityBody)
                    {
                        var inputStream = new MemoryStream();
                        context.Request.InputStream.CopyTo(inputStream);

                        var requestString = Encoding.UTF8.GetString(inputStream.ToArray());
                        var message = this._converter.Deserialize<Message>(requestString);

                        string responseString;
                        if (message.Code == RequestCodes.Request)
                        {
                            var requestMessage = this._converter.Deserialize<RequestMessage>(requestString);

                            var requestMessageEventArgs = new RequestMessageEventArgs(requestMessage);
                            this.RaiseRequestReceived(requestMessageEventArgs);
                            var responseMessage = requestMessageEventArgs.Response;
                            responseString = this._converter.Serialize(responseMessage);
                        }
                        else if (message.Code == RequestCodes.Event)
                        {
                            var eventMessage = this._converter.Deserialize<EventMessage>(requestString);
                            this.RaiseEventReceived(eventMessage);
                            responseString = string.Empty;
                        }
                        else
                        {
                            throw new ServiceException(ResponseCodes.BadRequest, string.Format(Resource1.InvalidMessageCode, message.Code));
                        }

                        context.Response.ContentType = this._converter.ContentType;
                        var bytesToSend = Encoding.UTF8.GetBytes(responseString);
                        context.Response.OutputStream.Write(bytesToSend, 0, bytesToSend.Length);
                    }
                    else
                    {
                        throw new HttpListenerException((int)HttpStatusCode.BadRequest);
                    }
                }
                else
                {
                    throw new HttpListenerException((int)HttpStatusCode.BadRequest);
                }
            }
            catch (Exception e)
            {
                ResponseMessage responseMessage;
                if (e is HttpListenerException listenerException)
                {
                    if (listenerException.ErrorCode < 100 || listenerException.ErrorCode > 999)
                    {
                        this._logger?.Error($"HttpListenerException with incompatible error code occured. Error code was: {listenerException.ErrorCode}. Translating to HTTP response code 500.");
                        context.Response.StatusCode = 500;
                    }
                    else
                    {
                        context.Response.StatusCode = listenerException.ErrorCode;
                    }
                    
                    responseMessage = new ResponseMessage(
                        listenerException.ErrorCode,
                        0,
                        null,
                        Helpers.ToJson(new ErrorInfoResponseServiceData(listenerException.Message)));
                } else if (e is AggregateException aggregateException)
                {
                    var errorMessages = new List<string>();
                    if (aggregateException.InnerException?.Message != null)
                    {
                        errorMessages.Add(aggregateException.InnerException.Message);
                    }

                    if (aggregateException.InnerExceptions != null && aggregateException.InnerExceptions.Any())
                    {
                        foreach (var innerException in aggregateException.InnerExceptions)
                        {
                            errorMessages.Add(innerException.Message);
                        }
                    }

                    var serviceException = new ServiceException(ResponseCodes.InternalError, $"{aggregateException.Message}, {string.Join(",", errorMessages)}");

                    responseMessage = new ResponseMessage(serviceException.Code/*ResponseCodes.ServiceFailed IoTCore 2.0*/,
                        0,
                        null,
                        Helpers.ToJson(new ErrorInfoResponseServiceData(e.Message, serviceException.Code, serviceException.Hint)));
                }
                else
                {
                    context.Response.StatusCode = 500;
                    responseMessage = new ResponseMessage(
                        ResponseCodes.InternalError, 
                        0, 
                        null,
                        Helpers.ToJson(new ErrorInfoResponseServiceData(e.Message)));
                }

                var responseString = this._converter.Serialize(responseMessage);
                context.Response.ContentType = this._converter.ContentType;
                var bytesToSend = Encoding.UTF8.GetBytes(responseString);
                context.Response.OutputStream.Write(bytesToSend, 0, bytesToSend.Length);
            }

            try
            {
                context.Response.OutputStream.Close();
            }
            catch (HttpListenerException e)
            {
                _logger?.Error($"HttpListenerException occured when closing response outputstream of httpcontext. The message was: '{e.Message}'. The httplistener errorcode was: '{e.ErrorCode}'.");
            }
        }

        private void Listen()
        {
            while (this._httpListener.IsListening && !_disposed)
            {
                try
                {
                    var context = this._httpListener.GetContext();
                    Task.Run(() => this.HandleRequest(context));
                }
                catch (Exception e)
                {
                    this._logger?.Error(e.Message);
                }
            }
        }

        private byte[] GetIotCoreBrowserWebPage()
        {
            var fileStream = this.GetType().Assembly.GetManifestResourceStream("ifmIoTCore.NetAdapter.Http.Resources.full.ifm-iot-core-visualizer.html.gz");
            if (fileStream == null)
            {
                throw new HttpListenerException(404, "IoTCore Browser Not Found.");
            }
            var webPage = new byte[fileStream.Length];
            fileStream.Read(webPage, 0, webPage.Length);
            return webPage;
        }
    }
}
