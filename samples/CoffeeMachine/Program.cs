using System;
using System.Net;
using CoffeeMachine.Model;
using CommandLine;
using ifmIoTCore;
using ifmIoTCore.Converter.Json;
using ifmIoTCore.Elements.EventArguments;
using ifmIoTCore.Exceptions;
using ifmIoTCore.Logging.Log4Net;
using ifmIoTCore.Messages;
using ifmIoTCore.NetAdapter.Http;
using ifmIoTCore.NetAdapter.Mqtt;
using ifmIoTCore.Profiles.DeviceManagement;
using ifmIoTCore.Profiles.IoTCoreManagement;
using ifmIoTCore.Resources;
using ifmIoTCore.Utilities;

namespace CoffeeMachine
{
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
                var ioTCore = IoTCoreFactory.Create(id, _logger);

                _logger.Info("Register tree_changed event handler");
                ioTCore.TreeChanged += IoTCore_TreeChangedEvent;

                _logger.Info("Create some elements");
                CreateElements(ioTCore);

                _logger.Info("Register http client factory");
                ioTCore.RegisterClientNetAdapterFactory(new HttpClientNetAdapterFactory(new ifmIoTCore.Converter.Json.JsonConverter()));

                _logger.Info("Register mqtt client factory");
                ioTCore.RegisterClientNetAdapterFactory(new MqttNetAdapterClientFactory(new ifmIoTCore.Converter.Json.JsonConverter()));

                var mqttServer = new MqttServerNetAdapter(ioTCore, ioTCore.Root, new JsonConverter(),
                    IPEndPoint.Parse("127.0.0.1:1883"));
                ioTCore.RegisterServerNetAdapter(mqttServer);

                mqttServer.Start();

                _logger.Info("Adding profile 'iotcore_management'");
                var elementProfileBuilder = new IoTCoreManagementProfileBuilder(ioTCore);
                elementProfileBuilder.Build();

                _logger.Info("Adding profile 'device_management'");
                var deviceProfileBuilder = new DeviceManagementProfileBuilder(ioTCore);
                deviceProfileBuilder.Build();

                _logger.Info($"Starting http server on {httpUri}");
                var httpServer = new HttpServerNetAdapter(ioTCore, new Uri(httpUri), new ifmIoTCore.Converter.Json.JsonConverter(), _logger);
                ioTCore.RegisterServerNetAdapter(httpServer);
                httpServer.Start();
                
                _logger.Info("Application running");

                using (var manualResetEventSlim = new System.Threading.ManualResetEventSlim())
                {
                    ioTCore.CreateActionServiceElement(ioTCore.Root, "stop", (sender, cid) => 
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        manualResetEventSlim.Set();
                    });

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

        private static readonly Model.CoffeeMachine _coffeMachine = new Model.CoffeeMachine();

        private static void CreateElements(IIoTCore ioTCore)
        {
            var coffeeMachine = ioTCore.CreateStructureElement(ioTCore.Root, "coffemachine");

            ioTCore.CreateServiceElement<bool, Capuccino>(coffeeMachine, "create_capuccino",
                (element, addSugar, cid) =>
                {
                    return _coffeMachine.CreateCapuccino(addSugar);
                });

            ioTCore.CreateGetterServiceElement<LatteMachiatto>(coffeeMachine, "create_latte_macchiato",
                (element, cid) =>
                {
                    return _coffeMachine.CreateLatteMacchiato();
                });

            ioTCore.CreateServiceElement<SpecialLatteMacciatoParameters, LatteMachiatto>(coffeeMachine,
                "create_special_latte_macchiato", (element, parameters, cid) =>
                {
                    if (parameters == null)
                    {
                        throw new ServiceException(ResponseCodes.BadRequest, string.Format(Resource1.ServiceDataEmpty, "create_special_latte_macchiato"));
                    }

                    return _coffeMachine.CreateSpecialLatteMachiatto(parameters.AddSugar, parameters.Amount);
                });

            ioTCore.CreateDataElement<uint>(coffeeMachine, "watertank_level",
                element =>
                {
                    return _coffeMachine.WaterTank.Level;
                }, createSetDataServiceElement: false);
            
            ioTCore.CreateActionServiceElement(coffeeMachine, "fill_watertank", (element, cid) => { _coffeMachine.WaterTank.FillWaterTank(); });

            var eventElement = ioTCore.CreateEventElement(coffeeMachine, "waterlevel_warning");

            _coffeMachine.WaterTank.LevelChanged += (s, e) =>
            {
                var waterTank = (WaterTank)s;
                if (waterTank.Level < 30)
                {
                    eventElement.RaiseEvent();
                }
            };
            


            ioTCore.RaiseTreeChanged();
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
