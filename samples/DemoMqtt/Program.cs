namespace DemoMqtt
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ifmIoTCore;
    using ifmIoTCore.Elements;
    using ifmIoTCore.MessageConverter.Json.Newtonsoft;
    using ifmIoTCore.NetAdapter.Http;
    using ifmIoTCore.NetAdapter.Mqtt;
    using ifmIoTCore.Profiles.Base;
    using ifmIoTCore.Profiles.DeviceManagement;

    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineParameters>(args)
                .WithParsed(o =>
                {
                    Run(o.MqttBrokerUrl, o.HttpUri);
                });
        }

        private static void Run(string mqttBrokerUrlArg, string httpUrlArg)
        {
            var mqttUrl = new Uri(mqttBrokerUrlArg);
            var httpUrl = new Uri(httpUrlArg);

            var file = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

            var logger = new ifmIoTCore.Logging.Log4Net.Logger(file, nameof(Program));
            

            using (var manualResetEvent = new ManualResetEventSlim())
            using (var iotCore = IoTCoreFactory.Create("id", null, logger))
            using (var mqttServerNetAdapter = new MqttServerNetAdapter(iotCore, iotCore.Root, new MessageConverter(), new IPEndPoint(IPAddress.Parse(mqttUrl.Host), mqttUrl.Port)))
            using (var httpServerNetAdapter = new HttpServerNetAdapter(iotCore, httpUrl, new MessageConverter(), logger))
            using (var httpClientNetAdapterFactory = new HttpClientNetAdapterFactory(new MessageConverter()))
            using (var mqttClientNetAdapterFactory = new MqttNetAdapterClientFactory(new MessageConverter()))
            {
                var deviceManagementProfileBuilder = new DeviceManagementProfileBuilder(new ProfileBuilderConfiguration(iotCore, null));
                deviceManagementProfileBuilder.Build();

                iotCore.RegisterClientNetAdapterFactory(mqttClientNetAdapterFactory);
                iotCore.RegisterClientNetAdapterFactory(httpClientNetAdapterFactory);

                mqttServerNetAdapter.RequestReceived += (s, e) =>
                {
                    e.ResponseMessage = iotCore.HandleRequest(e.RequestMessage);
                };

                mqttServerNetAdapter.EventReceived += (s, e) =>
                {
                    iotCore.HandleEvent(e.EventMessage);
                };

                iotCore.RegisterServerNetAdapter(mqttServerNetAdapter);
                iotCore.RegisterServerNetAdapter(httpServerNetAdapter);

                int value = 0;

                var dataElement = new ReadOnlyDataElement<int>("data", element => ++value);
                iotCore.Root.AddChild(dataElement);

                var eventElement = new EventElement("myEvent1");
                iotCore.Root.AddChild(eventElement, true);

                var raiseMyEvent1= new ActionServiceElement("raiseMyEvent1", (element, i) =>
                {
                    Task.Run(() =>
                    {
                        DateTime dateTime = DateTime.Now;
                        while (DateTime.Now - dateTime < TimeSpan.FromSeconds(30))
                        {
                            eventElement.RaiseEvent();
                        }
                    }).ConfigureAwait(false);
                });

                iotCore.Root.AddChild(raiseMyEvent1);

                var stopService = new ActionServiceElement("stop", (element, i) =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    manualResetEvent.Set();
                });

                iotCore.Root.AddChild(stopService);

                try
                {
                    mqttServerNetAdapter.Start();
                }
                catch
                {
                    
                }
                httpServerNetAdapter.Start();

                manualResetEvent.Wait();

                mqttServerNetAdapter.Stop();
                httpServerNetAdapter.Stop();
            }
        }
    }

    internal class CommandLineParameters
    {
        [Option( "mqtt-broker", Required = true, HelpText = "Url of the mqtt broker", Default = "mqtt://192.168.83.247:1883")]
        public string MqttBrokerUrl { get; set; }

        [Option('u', "http-uri", Required = true, HelpText = "Uri to start the http server", Default = "http://127.0.0.1:8001")]
        public string HttpUri { get; set; }
    }
}