namespace ifmIoTCore.Logging.Log4Net
{
    using System;
    using System.IO;
    using System.Reflection;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;
    using Utilities;

    public class Logger : Utilities.ILogger
    {
        private readonly ILog _logger;

        public Logger(LogLevel logLevel)
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
                ConversionPattern = "%date %-5level %message%newline"
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

        public Logger(string configFileName, string loggerName)
        {
            var repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(repository, new FileInfo(configFileName));
            _logger = LogManager.GetLogger(repository.Name, loggerName);
        }

        public Logger(string loggerName)
        {
            XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(loggerName);
        }

        public void Info(string message)
        {
            _logger?.Info(message);
        }

        public void Warning(string message)
        {
            _logger?.Warn(message);
        }

        public void Error(string message)
        {
            _logger?.Error(message);
        }

        public void Debug(string message)
        {
            _logger?.Debug(message);
        }
    }
}
