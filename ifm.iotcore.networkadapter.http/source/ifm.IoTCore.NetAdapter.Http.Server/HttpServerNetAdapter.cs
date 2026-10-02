using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ifm.Common;
using ifm.IoTCore.MessageConverter.Contracts;
using ifm.IoTCore.MessageDispatcher.Contracts;
using ifm.IoTCore.NetAdapter.Contracts;
using ifm.IoTCore.NetAdapter.Contracts.Server;
using ifm.IoTCore.NetAdapter.Http.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ifm.IoTCore.NetAdapter.Http.Server
{
    /// <summary>
    /// Implements a IServerNetAdapter that handles https.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CS0067:Event is never used", Justification = "Events are not used from within this library, but from outside.")]
    public class HttpServerNetAdapter : IHttpsServerNetAdapter, IConnectedServerNetAdapter
    {
        private IHostBuilder? _hostBuilder;
        private CancellationTokenSource? _cancellationToken;
        private readonly IMessageConverter _converter;
        private readonly ifm.Logger.Contracts.ILogger? _logger;
        private readonly IMessageDispatcher _messageDispatcher;
        private IHost? _host;
        private bool _isListening;
        private readonly Func<X509Certificate2, X509Chain?, SslPolicyErrors, bool>? _clientCertificateValidation;
        private Uri _uri;
        
        private X509Certificate2Collection? _certificateChain;
        private readonly HttpServerNetAdapterConfiguration _httpServerNetAdapterConfiguration;
        private readonly IWebsocketContextManager _websocketContextManager = new WebsocketContextManager();

        /// <summary>
        /// Initializes a new instance of <see cref="HttpServerNetAdapter"/>.
        /// </summary>
        /// <param name="messageDispatcher">The <seealso cref="IMessageDispatcher"/> to forward the received messages to.</param>
        /// <param name="uri">The uri on which this server should listen.</param>
        /// <param name="converter">The <seealso cref="IMessageConverter"/> to de/serialize messages.</param>
        /// <param name="httpServerNetAdapterConfiguration">The configuration.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="certificateChain">The certificate chain, that this server uses to represent himself.</param>
        /// <param name="clientCertificateValidation">If not null, a client certificate will be required, and this callback will validate the client certificate. If null, no client certificate will be required, and no validation will be done.</param>
        public HttpServerNetAdapter(IMessageDispatcher messageDispatcher, 
            Uri uri, 
            IMessageConverter converter,
            HttpServerNetAdapterConfiguration httpServerNetAdapterConfiguration,
            ifm.Logger.Contracts.ILogger? logger,
            X509Certificate2Collection? certificateChain,
            Func<X509Certificate2, X509Chain?, SslPolicyErrors, bool>? clientCertificateValidation)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _httpServerNetAdapterConfiguration = httpServerNetAdapterConfiguration ?? throw new ArgumentNullException(nameof(httpServerNetAdapterConfiguration));
            _logger = logger;

            _certificateChain = _uri.Scheme == "https"
                ? certificateChain ?? throw new ArgumentException($"When using scheme https, please provide an instance for {nameof(certificateChain)}.") 
                : null;
            
            _clientCertificateValidation = clientCertificateValidation;

            _websocketContextManager.ClientConnected += OnWebsocketContextManagerOnClientConnected;
            _websocketContextManager.ClientDisconnected += OnWebsocketContextManagerOnClientDisconnected;
            
        }

        private void OnWebsocketContextManagerOnClientConnected(object? sender, string e)
        {
            this.ClientConnected.Raise(this, new ConnectionEventArgs(ConnectionEventArgs.Types.Connected, e));
        }

        private void OnWebsocketContextManagerOnClientDisconnected(object? s, string clientId)
        {
            this.ClientDisconnected?.Invoke(this, new ConnectionEventArgs(ConnectionEventArgs.Types.Disconnected, clientId));
        }

        /// <inheritdoc />
        public string Scheme => _uri.Scheme;

        /// <inheritdoc />
        public string Format => _converter.Type;

        /// <inheritdoc />
        public Uri Uri
        {
            get => _uri;
            set
            {
                if (value.Equals(_uri)) return;
                _uri = value;
            }
        }

        /// <inheritdoc />
        public bool IsListening => _isListening;

        public RequestPathEscapeModeEnum GetRequestPathEscapeMode { get; set; }

        public X509Certificate2Collection? CertificateChain
        {
            get => _certificateChain;
            set
            {
                Stop();
                this._certificateChain = value;
                Start();
            }
        }

        public string? CertificateThumbPrint => _certificateChain?.ExtractCertificateChainLeaf()?.Thumbprint;

        public event EventHandler<ConnectionEventArgs>? ClientConnected;
        public event EventHandler<ConnectionEventArgs>? ClientDisconnected;

        public event EventHandler<RequestMessageEventArgs>? RequestReceived;
        public event EventHandler<EventMessageEventArgs>? EventReceived;

        /// <inheritdoc />
        public void Start()
        {
            Task.Run(StartAsync).GetAwaiter().GetResult();
        }

        public Task StartAsync()
        {
            _hostBuilder = CreateHostBuilder([]);
            if (_httpServerNetAdapterConfiguration.UseConsoleLifeTime)
            {
                _hostBuilder = _hostBuilder.UseConsoleLifetime();
            }
            
            _cancellationToken = new CancellationTokenSource();
            _host = _hostBuilder.Build();
            var task = _host.StartAsync(_cancellationToken.Token).ContinueWith((_) =>
            {
                _isListening = true;
            });
            
            return task;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _cancellationToken?.Cancel();
            Task.Run(StopAsync).GetAwaiter().GetResult();
            _isListening = false;
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            _cancellationToken?.Cancel();
            return _host  == null ? Task.CompletedTask : _host.StopAsync();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _websocketContextManager.ClientConnected -= OnWebsocketContextManagerOnClientConnected;
            _websocketContextManager.ClientDisconnected -= OnWebsocketContextManagerOnClientDisconnected;
            foreach (var item in _websocketContextManager.ClientIds)
            {
                DisconnectClient(item);
            }

            _host?.Dispose();
            _host = null;
        }

        protected void RaiseRequestReceived(RequestMessageEventArgs args)
        {
            RequestReceived?.Invoke(this, args);
        }

        protected void RaiseEventReceived(EventMessageEventArgs args)
        {
            EventReceived?.Invoke(this, args);
        }

        protected IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(o =>
                    {
                        if (_logger != null)
                        {
                            o.ClearProviders();
                            o.AddProvider(new MicrosoftExtensionsLoggerProvider(_logger));
                        }
                    });

                    webBuilder.UseKestrel(o =>
                    {
                        if (_uri.Scheme == "https")
                        {
                            this._logger?.Debug("Activating https configuration for kestrel server.");

                            o.ConfigureHttpsDefaults(a =>
                            {
                                try
                                {
                                    a.ClientCertificateMode = _clientCertificateValidation == null
                                        ? ClientCertificateMode.NoCertificate
                                        : ClientCertificateMode.RequireCertificate;

                                    if (_clientCertificateValidation != null)
                                    {
                                        a.ClientCertificateValidation = _clientCertificateValidation;
                                    }

                                    if (CertificateChain != null)
                                    {
                                        var serverCertificate = _certificateChain.ExtractCertificateChainLeaf();
                                        if (serverCertificate == null)
                                        {
                                            throw new InvalidOperationException("Server certificate is not set.");
                                        }

                                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                        {
                                            a.ServerCertificate = serverCertificate.EphemeralToNonEphemeral();
                                        }
                                        else
                                        {
                                            a.ServerCertificate = serverCertificate;
                                        }

                                        a.ServerCertificateChain = _certificateChain;
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Certificate chain is not set.");
                                    }
                                } catch(Exception e)
                                {
                                    this._logger?.Error($"Error configuring https defaults: {e.Message}");
                                    throw;
                                }
                            });
                        }
                    });

                    webBuilder.UseUrls(this.Uri.ToString());

                    webBuilder.ConfigureServices(s =>
                    {
                        s.AddSingleton<IMessageFromExceptionBuilder>(new MessageFromExceptionBuilder());
                        s.AddSingleton(_messageDispatcher);
                        s.AddSingleton(this);
                        s.AddSingleton(_ => _converter);
                        s.AddSingleton(_websocketContextManager);

                        if (_httpServerNetAdapterConfiguration.VisualizerCompactMiddlewareEnabled)
                        {
                            var vco = _httpServerNetAdapterConfiguration.VisualizerCompactMiddlewareOptions;
                            if (vco != null)
                            {
                                _logger?.Debug("Adding visualizer compact configuration.");

                                var defaultMappedPaths = new[] { "/web/subscribe", "/browse", "/"};

                                if (vco.MappedPaths == null)
                                {
                                    _logger?.Debug("No mapped paths found. Using defaults.");
                                    foreach (var path in defaultMappedPaths)
                                    {
                                        _logger?.Debug($"Default mapped path: {path}");
                                    }
                                }

                                s.Configure<VisualizerCompactMiddlewareOptions>(options =>
                                {
                                    options.FilePath = vco.FilePath;
                                    options.MappedPaths = vco.MappedPaths ?? defaultMappedPaths;
                                });
                            }
                        }

                        if (_httpServerNetAdapterConfiguration.VisualizerEnhancedMiddlewareEnabled)
                        {
                            var veo = _httpServerNetAdapterConfiguration.VisualizerEnhancedMiddlewareOptions;
                            if (veo != null)
                            {
                                _logger?.Debug("Adding visualizer enhanced configuration.");

                                var defaultMappedPaths = new[] { "/visualizer-enhanced" };

                                if (veo.MappedPaths == null)
                                {
                                    _logger?.Debug("No mapped paths found. Using defaults.");
                                    foreach (var path in defaultMappedPaths)
                                    {
                                        _logger?.Debug($"Default mapped path: {path}");
                                    }
                                }

                                var defaultClientRoutePrefix = "/api";

                                if (veo.ClientRoutePrefix == null)
                                {
                                    _logger?.Debug($"No client route prefix found. Using default: {defaultClientRoutePrefix}");
                                }

                                s.Configure<VisualizerEnhancedMiddlewareOptions>(options =>
                                {
                                    options.BasePath = veo.BasePath;
                                    options.MappedPaths = veo.MappedPaths ?? defaultMappedPaths;
                                    options.ClientRoutePrefix = veo.ClientRoutePrefix ?? defaultClientRoutePrefix;
                                });
                            }
                        }

                        if (_httpServerNetAdapterConfiguration.AllowedServicesMiddlewareEnabled)
                        {
                            var allowedServiceOptions = _httpServerNetAdapterConfiguration.AllowedServicesMiddlewareOptions;
                            if (allowedServiceOptions != null)
                            {
                                _logger?.Debug("Adding allowed services configuration.");

                                s.Configure<AllowedServicesMiddlewareOptions>(options =>
                                {
                                    options.AllowedServicesMode = allowedServiceOptions.AllowedServicesMode;
                                });
                            }
                        }

                        s.AddSingleton<MiddlewareExceptionHandlerService>();
                    });

                    webBuilder.Configure(Application);
                });

        

        private void Application(WebHostBuilderContext context, IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions();
            foreach (var item in _httpServerNetAdapterConfiguration.WebsocketAllowedOrigins)
            {
                webSocketOptions.AllowedOrigins.Add(item);
            }

            webSocketOptions.KeepAliveInterval = TimeSpan.FromSeconds(_httpServerNetAdapterConfiguration.WebsocketKeepAliveIntervalSeconds);

            app.UseWebSockets(webSocketOptions);
            app.UseMiddleware<IoTCoreWebSocketMiddleWare>();

            if (_httpServerNetAdapterConfiguration.VisualizerCompactMiddlewareEnabled)
            {
                app.UseVisualizerCompactMiddleware();
            }

            if (_httpServerNetAdapterConfiguration.VisualizerEnhancedMiddlewareEnabled)
            {
                app.UseVisualizerEnhancedMiddleware();
            }

            if (_httpServerNetAdapterConfiguration.AllowedServicesMiddlewareEnabled)
            {
                app.UseAllowedServicesMiddleware();
            }

            app.UseIoTCoreApiMiddleware();
        }


        public bool IsClientConnected(string clientId)
        {
            var result = (_websocketContextManager.TryGetWebsocketContext(clientId, out var webSocket) && webSocket is
            {
                State: WebSocketState.Open
            });

            return result;
        }

        public void DisconnectClient(string clientId)
        {
            if (_websocketContextManager.TryGetWebsocketContext(clientId, out var webSocket))
            {
                Task.Run(() =>
                {
                    webSocket?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server",
                        CancellationToken.None);
                }).GetAwaiter().GetResult();
                
            }
        }

        public void SendEvent(string clientId, Message.Message message)
        {
            if (_websocketContextManager.TryGetWebsocketContext(clientId, out var webSocket))
            {
                var serializedMessage = _converter.Serialize(message);

                Task.Run(() =>
                    {
                        webSocket?.SendAsync(Encoding.UTF8.GetBytes(serializedMessage), WebSocketMessageType.Text,
                            WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
                    }).GetAwaiter().GetResult();
            }
        }

        public IEnumerable<string> ConnectedClients => _websocketContextManager.ClientIds;
    }

    internal interface IWebsocketContextManager
    {
        bool RegisterWebsocketContext(string clientId, WebSocket context);
        bool TryGetWebsocketContext(string clientId, out WebSocket? websocket);
        IEnumerable<string> ClientIds { get; }
        event EventHandler<string> ClientDisconnected;
        event EventHandler<string> ClientConnected;
    }

    internal class WebsocketContextManager : IWebsocketContextManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _webSocketClientContexts = new ConcurrentDictionary<string, WebSocket>();

        public bool RegisterWebsocketContext(string clientId, WebSocket context)
        {
            if (!_webSocketClientContexts.ContainsKey(clientId))
            {
                if (_webSocketClientContexts.TryAdd(clientId, context))
                {
                    this.RaiseClientConnected(clientId);
                    ObserveWebsocketContext(clientId, context);
                    return true;
                }
            }

            return false;
        }

        private void ObserveWebsocketContext(string clientId, WebSocket context)
        {
            Task.Run(async () =>
            {
                while (context.State == WebSocketState.Open)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                if (_webSocketClientContexts.TryRemove(clientId, out var innerContext))
                {
                    RaiseClientDisconnected(clientId);
                    innerContext.Dispose();
                }
            });
        }

        public bool TryGetWebsocketContext(string clientId, out WebSocket? websocket)
        {
            websocket = null;

            if (!_webSocketClientContexts.TryGetValue(clientId, out var innerWebSocket)) return false;
            websocket = innerWebSocket;
            return true;
        }

        public IEnumerable<string> ClientIds => _webSocketClientContexts.Keys;

        public event EventHandler<string>? ClientDisconnected;
        public event EventHandler<string>? ClientConnected;

        protected void RaiseClientConnected(string clientId)
        {
            ClientConnected?.Invoke(this, clientId);
        }

        protected void RaiseClientDisconnected(string clientId)
        {
            ClientDisconnected?.Invoke(this, clientId);
        }
    }
}
