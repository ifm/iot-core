using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using ifm.DataStore;
using ifm.IoTCore;
using ifm.IoTCore.MessageConverter.Json;
using ifm.IoTCore.MessageDispatcher.Contracts;
using ifm.IoTCore.NetAdapter.Http.Contracts;
using ifm.IoTCore.NetAdapter.Http.Profile.HttpServer;
using ifm.IoTCore.NetAdapter.Http.Server;
using ifm.Logger;
using ifm.Logger.Contracts;

namespace TestApplication.Websockets
{
    internal class Program
    {
        internal const string SectionKeyDefault = "Https";
        internal const string ValueKeyDefault = "HttpServerProfileBuilderOptions";

        static void Main()
        {
            UseHttps();
            //NoHttps();
        }

        static void UseHttps()
        {
            var logger = new ConsoleLogger();
            var dataStore = new DataStore("c:\\temp\\datastore.json");
            var iotCore = IoTCoreFactory.Create("id0", dataStore, logger);

            var options = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault,
                ValueKeyDefault)
                          ?? new HttpServerProfileBuilderOptions();

            options.Port = 8090;

            var rootCert = X509Certificate2Factory.CreateSelfSignedRootCaCertificate("root");
            var intermediateCert = X509Certificate2Factory.CreateIntermediateCaCertificate("intermediate", rootCert, 0);
            var clientCert = X509Certificate2Factory.CreateClientCertificate("client", intermediateCert, 0);

            options.TrustedClientCertificates = [clientCert.ExportCertificatePem()
                                                 + "\n" + rootCert.ExportCertificatePem()
                                                 + "\n" + intermediateCert.ExportCertificatePem()];

            if (options.CertificateData == null || string.IsNullOrEmpty(options.CertificateData.Certificate))
            {
                var serverCert = X509Certificate2Factory.CreateServerCertificate("127.0.0.1", intermediateCert, 0);
                var privateKeyPem = serverCert.GetRSAPrivateKey()?.ExportRSAPrivateKeyPem();

                options.CertificateData = new CertificateData
                {
                    Certificate = serverCert.ExportCertificatePem() + "\n" + rootCert.ExportCertificatePem() + "\n" + intermediateCert.ExportCertificatePem(),
                    Key = privateKeyPem
                };
            }

            _ = options.CertificateData.Certificate.TryParse(out var certificateCollection);
            var serverCertificate = certificateCollection.ExtractCertificateChainLeaf();
            var serverCertificateWithKey = X509Certificate2.CreateFromPem(serverCertificate?.ExportCertificatePem(), options.CertificateData.Key);

            if (serverCertificate != null)
            {
                certificateCollection?.Remove(serverCertificate);
            }

            certificateCollection?.Add(serverCertificateWithKey);

            var config = dataStore.Get<HttpServerNetAdapterConfiguration>("Server1", "Config") ?? new HttpServerNetAdapterConfiguration();

            config.VisualizerCompactMiddlewareEnabled = true;
            config.VisualizerCompactMiddlewareOptions = new VisualizerCompactMiddlewareOptions()
            {
                FilePath = "C:\\temp\\Tools\\Visualizer\\full.ifm-iot-core-visualizer.html.gz",
                MappedPaths = ["/visualizer-compact"]
            };

            config.VisualizerEnhancedMiddlewareEnabled = true;
            config.VisualizerEnhancedMiddlewareOptions = new VisualizerEnhancedMiddlewareOptions()
            {
                BasePath = "C:\\temp\\Tools\\visualizer-premium",
                MappedPaths = ["/"],
                ClientRoutePrefix = "/config"
            };

            using var httpsServerNetAdapter =
                CreateHttpServerNetAdapter("https",
                    iotCore.MessageDispatcher,
                    options.ListenAddress,
                    options.Port,
                    config,
                    logger,
                    certificateCollection ?? [],
                    null);

            iotCore.ServerNetAdapterManager.RegisterServerNetAdapter(httpsServerNetAdapter);
            httpsServerNetAdapter.Start();

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            var httpServerProfileBuilder =
                new HttpServerProfileBuilder(
                    iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);

            httpServerProfileBuilder.Build();

            using var manualResetEvent = new ManualResetEvent(false);

            iotCore.ElementManager.CreateActionServiceElement(iotCore.Root, "stop", _ =>
            {
                // ReSharper disable once AccessToDisposedClosure
                manualResetEvent.Set();
            });

            var eventElement = iotCore.ElementManager.CreateEventElement(iotCore.Root, "event");
            _ = Task.Run(() =>
            {

                while (!manualResetEvent.WaitOne(1000))
                {
                    eventElement.Raise();
                    Task.Delay(1000).Wait();
                }
            });

            httpsServerNetAdapter.ClientDisconnected += (s, e) =>
            {
                logger.Info("Client disconnected " + e.SenderId);
            };

            manualResetEvent.WaitOne();
        }

        static void NoHttps()
        {
            var logger = new ConsoleLogger();
            var dataStore = new DataStore("c:\\temp\\datastore.json");
            var iotCore = IoTCoreFactory.Create("id0", dataStore, logger);

            var options = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault,
                ValueKeyDefault)
                          ?? new HttpServerProfileBuilderOptions();

            options.Port = 8090;

            var config = dataStore.Get<HttpServerNetAdapterConfiguration>("Server1", "Config") ?? new HttpServerNetAdapterConfiguration();

            config.VisualizerCompactMiddlewareEnabled = true;
            config.VisualizerCompactMiddlewareOptions = new VisualizerCompactMiddlewareOptions()
            {
                FilePath = "C:\\temp\\Tools\\Visualizer\\full.ifm-iot-core-visualizer.html.gz",
                MappedPaths = ["/visualizer-compact"]
            };

            config.VisualizerEnhancedMiddlewareEnabled = true;
            config.VisualizerEnhancedMiddlewareOptions = new VisualizerEnhancedMiddlewareOptions()
            {
                BasePath = "C:\\temp\\Tools\\visualizer-premium",
                MappedPaths = ["/"],
                ClientRoutePrefix = "/config"
            };

            using var httpsServerNetAdapter =
                CreateHttpServerNetAdapter("http",
                    iotCore.MessageDispatcher,
                    options.ListenAddress,
                    options.Port,
                    config,
                    logger,
                    [],
                    null);

            iotCore.ServerNetAdapterManager.RegisterServerNetAdapter(httpsServerNetAdapter);
            httpsServerNetAdapter.Start();

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            var httpServerProfileBuilder =
                new HttpServerProfileBuilder(
                    iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);

            httpServerProfileBuilder.Build();

            using var manualResetEvent = new ManualResetEvent(false);

            iotCore.ElementManager.CreateActionServiceElement(iotCore.Root, "stop", _ =>
            {
                // ReSharper disable once AccessToDisposedClosure
                manualResetEvent.Set();
            });

            var eventElement = iotCore.ElementManager.CreateEventElement(iotCore.Root, "event");
            _ = Task.Run(() =>
            {

                while (!manualResetEvent.WaitOne(1000))
                {
                    eventElement.Raise();
                    Task.Delay(1000).Wait();
                }
            });

            httpsServerNetAdapter.ClientDisconnected += (s, e) =>
            {
                logger.Info("Client disconnected " + e.SenderId);
            };

            manualResetEvent.WaitOne();
        }

        

        private static HttpServerNetAdapter CreateHttpServerNetAdapter(string scheme, IMessageDispatcher messageDispatcher, 
            string listenAddress, 
            int port, 
            HttpServerNetAdapterConfiguration httpServerNetAdapterConfiguration, 
            ILogger logger, 
            X509Certificate2Collection certificateCollection,
            Func<X509Certificate2, X509Chain?, SslPolicyErrors, bool>? clientCertificateValidationFunction)
        {
            var httpServerNetAdapter = new HttpServerNetAdapter(messageDispatcher,
            new UriBuilder(scheme, listenAddress, port).Uri,
                new MessageConverter(),
                httpServerNetAdapterConfiguration,
                logger,
                certificateCollection,
                clientCertificateValidationFunction);

            return httpServerNetAdapter;
        }

        private static Func<X509Certificate2, X509Chain?, SslPolicyErrors, bool>? BuildValidationFunc(
            HttpServerProfileBuilderOptions options)
        {
            if (options.ValidateClientCertificates == 1)
            {
                return (certificate, chain, sslPolicyErrors) =>
                {
                    if (chain == null) return false;

                    chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
                    // Custom root trust
                    chain.ChainPolicy.CustomTrustStore.AddRange(options.TrustedClientCertificates.Select(x => X509Certificate2.CreateFromPem(x.ToCharArray())).ToArray());

                    // Trusted client certificates
                    chain.ChainPolicy.ExtraStore.AddRange(options.TrustedClientCertificates.Select(x => X509Certificate2.CreateFromPem(x.ToCharArray())).ToArray());

                    return chain.Build(certificate);
                };
            }

            return null;
        }
    }
}
