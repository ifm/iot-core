namespace ifm.IoTCore.NetAdapter.Http.Server
{
    public class HttpServerNetAdapterConfiguration
    {
        public bool VisualizerCompactMiddlewareEnabled { get; set; }
        public bool VisualizerEnhancedMiddlewareEnabled { get; set; }
        public bool AllowedServicesMiddlewareEnabled { get; set; }
        public bool UseConsoleLifeTime { get; set; } = true;

        public string[] WebsocketAllowedOrigins { get; set; } = ["*"];
        public int WebsocketKeepAliveIntervalSeconds { get; set; } = 120;

        public VisualizerCompactMiddlewareOptions? VisualizerCompactMiddlewareOptions { get; set; } = new();
        public VisualizerEnhancedMiddlewareOptions? VisualizerEnhancedMiddlewareOptions { get; set; } = new();
        public AllowedServicesMiddlewareOptions? AllowedServicesMiddlewareOptions { get; set; } = new();
    }
}
