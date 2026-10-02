using Microsoft.Extensions.Logging;

namespace ifm.IoTCore.NetAdapter.Http.Server;

public class MicrosoftExtensionsLoggerProvider : ILoggerProvider
{
    private readonly ifm.Logger.Contracts.ILogger _logger;

    public MicrosoftExtensionsLoggerProvider(ifm.Logger.Contracts.ILogger logger)
    {
        this._logger = logger;
    }

    public void Dispose()
    {
    }

    public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
    {
        return new LoggerWrapper(_logger);
    }
}