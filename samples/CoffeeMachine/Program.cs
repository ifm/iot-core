namespace DemoApp1
{
    using System;
    using System.IO;
    using System.Net;
    using CommandLine;
    using CoffeeMachine.Model;
    using ifmIoTCore;
    using ifmIoTCore.Common.Variant;
    using ifmIoTCore.Elements.EventArguments;
    using ifmIoTCore.Exceptions;
    using ifmIoTCore.Logger;
    using ifmIoTCore.Logging.Log4Net;
    using ifmIoTCore.Messages;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.NetAdapter.Mqtt;
    using ifmIoTCore.Profiles.DeviceManagement;
    using ifmIoTCore.Profiles.IoTCoreManagement;
    using ifmIoTCore.Resources;
    using ifmIoTCore.Elements;
    using ifmIoTCore.Profiles.Base;
    using ifmIoTCore.Utilities;

    internal class Program
    {
        private static ILogger _logger;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineParameters>(args)
                .WithParsed(o =>
                {
                    Run(o.Id, o.HttpUri, o.OpcUri, o.LogConfigFile);
                });
        }

        private static void Run(string id, string httpUri, string opcUri, string logConfigFile)
        {
            var logLevel = GetLogLevel();
            _logger = logConfigFile == null ? new Logger(logLevel) : new Logger(logConfigFile, nameof(Program));

            try
            {
                _logger.Info("Application starting up");

                _logger.Info($"Create IoTCore '{id}'");
                var ioTCore = IoTCoreFactory.Create(id, null, _logger);

                _logger.Info("Register tree_changed event handler");
                ioTCore.Root.TreeChanged += IoTCore_TreeChangedEvent;

                _logger.Info("Create some elements");
                CreateElements(ioTCore);

                var messageConverter = new ifmIoTCore.MessageConverter.Json.Newtonsoft.MessageConverter();

                _logger.Info("Register http client factory");
                ioTCore.RegisterClientNetAdapterFactory(new HttpClientNetAdapterFactory(messageConverter));

                _logger.Info("Register mqtt client factory");
                ioTCore.RegisterClientNetAdapterFactory(new MqttNetAdapterClientFactory(messageConverter));

                var mqttServer = new MqttServerNetAdapter(ioTCore, ioTCore.Root, messageConverter,
                    IPEndPoint.Parse("127.0.0.1:1883"));
                ioTCore.RegisterServerNetAdapter(mqttServer);

                mqttServer.Start();

                _logger.Info("Adding profile 'iotcore_management'");
                var elementProfileBuilder = new IoTCoreManagementProfileBuilder(new ProfileBuilderConfiguration(ioTCore, ioTCore.Root.Address));
                elementProfileBuilder.Build();

                _logger.Info("Adding profile 'device_management'");
                var deviceProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(ioTCore, ioTCore.Root.Address));
                deviceProfileBuilder.Build();

                _logger.Info($"Starting http server on {httpUri}");
                var httpServer = new HttpServerNetAdapter(ioTCore, new Uri(httpUri), messageConverter, _logger);
                ioTCore.RegisterServerNetAdapter(httpServer);
                httpServer.Start();
                
                _logger.Info("Application running");

                using (var manualResetEventSlim = new System.Threading.ManualResetEventSlim())
                {
                    var a = new ActionServiceElement("stop",
                        (sender, cid) => 
                        {
                            // ReSharper disable once AccessToDisposedClosure
                            // manualResetEventSlim.Set();
                        });

                    ioTCore.Root.AddChild(a);

                    manualResetEventSlim.Wait();
                }

                _logger.Info("Application shutting down");

                httpServer.Stop();

                _logger.Info("Application stopped");
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
            }
        }

        private static string GetOrCreateIoddDirectory()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ifm", "iodds");
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                if ((dirInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("IODD directory is readonly");
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static LogLevel GetLogLevel()
        {
            var logLevelSetting = System.Configuration.ConfigurationManager.AppSettings.Get("logLevel");
            return Enum.TryParse(logLevelSetting, out LogLevel logLevel) ? logLevel : LogLevel.Info;
        }

        private static void IoTCore_TreeChangedEvent(object sender, TreeChangedEventArgs e)
        {
            var ioTCore = (IIoTCore)sender;
            ioTCore.Logger.Info($"Tree changed: Parent={e.ParentElement} Child={e.ChildElement} Action={e.Action}");
        }

        private static readonly CoffeeMachine _coffeMachine = new CoffeeMachine();

        private static void CreateElements(IIoTCore ioTCore)
        {
            var coffeeMachine = new StructureElement("coffemachine");
            ioTCore.Root.AddChild(coffeeMachine);

            var createCappucinoService = new ServiceElement(
                "create_capuccino",
                (element, data, cid) =>
                {
                    var addSugar = (bool)data.AsVariantValue();
                    var response = _coffeMachine.CreateCapuccino(addSugar);
                    return Helpers.VariantFromObject(response);
                });

            coffeeMachine.AddChild(createCappucinoService);

            var createLatteService = new GetterServiceElement("create_latte_macchiato",
                (element, cid) => 
                    Helpers.VariantFromObject(_coffeMachine.CreateLatteMacchiato()));

            coffeeMachine.AddChild(createLatteService);

            var createSpecialLatteService = new ServiceElement(
                "create_special_latte_macchiato", 
                (element, data, cid) =>
                {
                    if (data == null)
                    {
                        throw new IoTCoreException(ResponseCodes.BadRequest, string.Format(Resource1.ServiceDataEmpty, "create_special_latte_macchiato"));
                    }

                    var parameters = Helpers.VariantToObject<SpecialLatteMacciatoParameters>(data);
                    var response = _coffeMachine.CreateSpecialLatteMachiatto(parameters.AddSugar, parameters.Amount);
                    return Helpers.VariantFromObject(response);
                });

            coffeeMachine.AddChild(createSpecialLatteService);

            var level = new ReadOnlyDataElement<uint>("watertank_level",
                element =>
                {
                    return _coffeMachine.WaterTank.Level;
                });

            coffeeMachine.AddChild(level);
            
            var fillService = new ActionServiceElement(
                "fill_watertank", 
                (element, cid) => { _coffeMachine.WaterTank.FillWaterTank(); });

            coffeeMachine.AddChild(fillService);

            var eventElement = new EventElement("waterlevel_warning");
            coffeeMachine.AddChild(eventElement);

            _coffeMachine.WaterTank.LevelChanged += (s, e) =>
            {
                var waterTank = (WaterTank)s;
                if (waterTank.Level < 30)
                {
                    eventElement.RaiseEvent();
                }
            };
            
            ioTCore.Root.RaiseTreeChanged();
        }
    }

    internal class CommandLineParameters
    {
        [Option('i', "id", Required = true, HelpText = "The id of the iotcore", Default = "id0")]
        public string Id { get; set; }

        [Option('u', "http-uri", Required = true, HelpText = "Uri to start the http server", Default = "http://127.0.0.1:8001")]
        public string HttpUri { get; set; }

        [Option('o', "opc-uri", Required = false, HelpText = "Uri to start the opc server", Default = "opc.tcp://127.0.0.1:62546")]
        public string OpcUri { get; set; }

        [Option('l', "log-config", Required = false, HelpText = "Optional logger configuration file")]
        public string LogConfigFile { get; set; }
    }
}
