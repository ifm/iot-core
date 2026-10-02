using System;

namespace ifm.IoTCore.NetAdapter.Http.Server;

public class LoggerWrapper : Microsoft.Extensions.Logging.ILogger
{
    private readonly ifm.Logger.Contracts.ILogger _logger;

    public LoggerWrapper(ifm.Logger.Contracts.ILogger logger)
    {
        _logger = logger;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        switch (logLevel)
        {
            case Microsoft.Extensions.Logging.LogLevel.Error:
            case Microsoft.Extensions.Logging.LogLevel.Critical:
                var message = $"EventId: '{eventId.Id}', EventId.Name: '{eventId.Name}', State: '{state}'";
                if (exception != null)
                {
                    message += $", Exception: '{exception.Message ?? "no message"}', Stacktrace: '{exception.StackTrace ?? "no stacktrace"}'";
                }
                _logger.Error(message);
                break;
        }
    }
}