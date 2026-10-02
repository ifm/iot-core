namespace ifm.Logger;

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Contracts;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

/// <summary>
/// Represents a logger implementation that uses log4net for logging. This class provides methods to log messages at different log levels (Debug, Info, Warning, Error) and can be configured with a specified log level or a log4net configuration file.
/// </summary>
public class Log4NetLogger : Contracts.ILogger
{
    private readonly ILog _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetLogger"/> class with the specified log level. If the log level is set to LogLevel.Off, no logging will be performed. The logger is configured to log messages to both the console and a rolling file appender, with the specified log level determining which messages are logged.
    /// </summary>
    /// <param name="logLevel">The log level to use for logging.</param>
    public Log4NetLogger(LogLevel logLevel)
    {
        if (logLevel == LogLevel.Off)
        {
            return;
        }

        var hierarchy = (Hierarchy)LogManager.GetRepository(Assembly.GetExecutingAssembly());
        if (hierarchy.Configured)
        {
            _logger = LogManager.GetLogger(hierarchy.Name, "");
            return;
        }

        var patternLayout = new PatternLayout
        {
            ConversionPattern = "%date [%thread] %-5level %message%newline"
        };
        patternLayout.ActivateOptions();

        var consoleAppender = new ConsoleAppender
        {
            Name = "ConsoleAppender",
            Layout = patternLayout
        };

        var rollingFileAppender = new RollingFileAppender
        {
            Name = "RollingFileAppender",
            File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ifm", "iotcore", "logs", "iotcore.log"),
            AppendToFile = true,
            RollingStyle = RollingFileAppender.RollingMode.Size,
            MaxSizeRollBackups = 10,
            MaximumFileSize = "10MB",
            StaticLogFileName = true,
            Layout = patternLayout
        };
        rollingFileAppender.ActivateOptions();

        hierarchy.Root.AddAppender(consoleAppender);
        hierarchy.Root.AddAppender(rollingFileAppender);
        switch (logLevel)
        {
            case LogLevel.Debug:
                hierarchy.Root.Level = Level.Debug;
                break;
            case LogLevel.Info:
                hierarchy.Root.Level = Level.Info;
                break;
            case LogLevel.Warning:
                hierarchy.Root.Level = Level.Warn;
                break;
            case LogLevel.Error:
                hierarchy.Root.Level = Level.Error;
                break;
            default:
                hierarchy.Root.Level = Level.Warn;
                break;
        }
        hierarchy.Configured = true;
        _logger = LogManager.GetLogger(hierarchy.Name, "");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetLogger"/> class with the specified log4net configuration file and logger name. The logger is configured based on the provided configuration file, allowing for custom logging settings and appenders to be defined.
    /// </summary>
    /// <param name="configFileName">The path to the log4net configuration file.</param>
    /// <param name="loggerName">The name of the logger.</param>
    public Log4NetLogger(string configFileName, string loggerName)
    {
        var repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
        XmlConfigurator.Configure(repository, new FileInfo(configFileName));
        _logger = LogManager.GetLogger(repository.Name, loggerName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Log4NetLogger"/> class with the specified logger name. The logger is configured based on the default log4net configuration.
    /// </summary>
    /// <param name="loggerName">The name of the logger.</param>
    public Log4NetLogger(string loggerName)
    {
        XmlConfigurator.Configure();
        _logger = LogManager.GetLogger(loggerName);
    }

    /// <summary>
    /// Logs a debug message to the current logger. The message is only logged if the log level is set to LogLevel.Debug.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    public void Debug(string message)
    {
        _logger.Debug(PatchMessage(message));
    }

    /// <summary>
    /// Logs an information message to the current logger. The message is only logged if the log level is set to LogLevel.Info or lower.
    /// </summary>
    /// <param name="message">The information message to log.</param>
    public void Info(string message)
    {
        _logger.Info(PatchMessage(message));
    }

    /// <summary>
    /// Logs a warning message to the current logger. The message is only logged if the log level is set to LogLevel.Warning or lower.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public void Warning(string message)
    {
        _logger.Warn(PatchMessage(message));
    }

    /// <summary>
    /// Logs an error message to the current logger. The message is only logged if the log level is set to LogLevel.Error or lower.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public void Error(string message)
    {
        _logger.Error(PatchMessage(message));
    }

    /// <summary>
    /// Logs an exception as error message to the current logger. The message is only logged if the log level is set to LogLevel.Error or lower.
    /// </summary>
    /// <param name="ex">The exception to log.</param>
    public void Error(Exception ex)
    {
        var sb = new StringBuilder(PatchMessage(ex.Message));
        if (ex is AggregateException ex1)
        {
            foreach (var item in ex1.InnerExceptions)
            {
                sb.AppendLine($"  {PatchMessage(item.Message)}");
            }
        }
        else
        {
            if (ex.InnerException != null)
            {
                sb.AppendLine($"  {PatchMessage(ex.InnerException.Message)}");
            }
        }
        _logger.Error(sb.ToString());
    }

    private static string PatchMessage(string message)
    {
        return message.Replace("\\", "/")/*.Replace("\"", "\\\"")*/;
    }
}