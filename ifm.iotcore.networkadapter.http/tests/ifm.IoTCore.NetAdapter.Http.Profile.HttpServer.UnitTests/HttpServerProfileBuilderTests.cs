using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using ifm.IoTCore.Common.Exceptions;
using ifm.IoTCore.ElementManager.Contracts.Elements;
using ifm.IoTCore.NetAdapter.Http.Contracts;
using ifm.Logger;
using ifm.Logger.Contracts;
using NUnit.Framework;

namespace ifm.IoTCore.NetAdapter.Http.Profile.HttpServer.UnitTests
{
    [TestFixture]
    public class HttpServerProfileBuilderTests
    {
        internal const string SectionKeyDefault = "Https";
        internal const string ValueKeyDefault = "HttpServerProfileBuilderOptions";

        [Test]
        public void TestSetPort()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);



            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetPort.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);
            httpServerProfileBuilder.Build();

            void OnHttpServerProfileBuilderOptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                dataStore.Set(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions.PropertyChanged += OnHttpServerProfileBuilderOptionsOnPropertyChanged;

            try
            {
                var portElement =
                    iotCore.ElementManager.GetElementByAddress("id/httpserver/port") as IReadWriteDataElement<int>;
                Assert.That(portElement, Is.Not.Null);
                if (portElement != null)
                {
                    portElement.Value = 8090;
                }

                var dataStore2 = new DataStore.DataStore(dataStoreFilePath);

                var config = dataStore2.Get<HttpServerProfileBuilderOptions>(
                    SectionKeyDefault, ValueKeyDefault);
                Assert.That(config, Is.Not.Null);
                Assert.That(config.Port, Is.EqualTo(8090));
            }
            finally
            {
                httpServerProfileBuilderOptions.PropertyChanged -= OnHttpServerProfileBuilderOptionsOnPropertyChanged;
            }


        }

        [Test]
        public void TestSetSecure()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);

            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetSecure.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            if (httpServerProfileBuilderOptions == null)
            {
                httpServerProfileBuilderOptions = new HttpServerProfileBuilderOptions();
                dataStore.Set<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);
            httpServerProfileBuilder.Build();

            var secureElement = iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/secure");
            Assert.That(secureElement, Is.Not.Null);

            var secureDataElement = (IReadWriteDataElement<int>)secureElement;
            var defaultValue = secureDataElement.Value;

            Assert.Pass();
        }

        [Test]
        public void TestSetCertificatesFailOnMissingServerOid()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);

            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            if (httpServerProfileBuilderOptions == null)
            {
                httpServerProfileBuilderOptions = new HttpServerProfileBuilderOptions();
                dataStore.Set<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);
            httpServerProfileBuilder.Build();

            var certificateKeyPemElement = iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/certificate_key_pem");
            Assert.That(certificateKeyPemElement, Is.Not.Null);

            Assert.That(certificateKeyPemElement, Is.AssignableTo(typeof(IReadWriteDataElement<CertificateData>)));

            var dataElement = (IReadWriteDataElement<CertificateData>)certificateKeyPemElement;

            // Intentionally set a certificate that misses the server oid in the x509enhancedkeyusage extension to trigger the validation failure
            var certificate = X509Certificate2Factory.CreateSelfSignedClientCertificate("myserver");

            var value = new CertificateData()
            {
                Certificate = certificate.ExportCertificatePem(),
                Key = certificate.GetRSAPrivateKey()?.ExportRSAPrivateKeyPem()
            };

            Assert.Throws<IoTCoreException>(() => dataElement.Value = value);

        }

        [Test]
        public void TestSetCertificatesFailOnNoExtensionsAtAll()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);
            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            if (httpServerProfileBuilderOptions == null)
            {
                httpServerProfileBuilderOptions = new HttpServerProfileBuilderOptions();
                dataStore.Set<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);
            httpServerProfileBuilder.Build();

            var certificateKeyPemElement = iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/certificate_key_pem");
            Assert.That(certificateKeyPemElement, Is.Not.Null);

            Assert.That(certificateKeyPemElement, Is.AssignableTo(typeof(IReadWriteDataElement<CertificateData>)));

            var dataElement = (IReadWriteDataElement<CertificateData>)certificateKeyPemElement;

            // Intentionally set a certificate that misses the server oid in the x509enhancedkeyusage extension to trigger the validation failure
            var certificate = X509Certificate2Factory.CreateCertificate("test", null, null, 1);

            var value = new CertificateData()
            {
                Certificate = certificate.ExportCertificatePem(),
                Key = certificate.GetRSAPrivateKey()?.ExportRSAPrivateKeyPem()
            };

            Assert.Throws<IoTCoreException>(() =>
            {

                dataElement.Value = value;
            });

        }

        [Test]
        public void TestSetCertificates()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);

            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            if (httpServerProfileBuilderOptions == null)
            {
                httpServerProfileBuilderOptions = new HttpServerProfileBuilderOptions();
                dataStore.Set<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);
            httpServerProfileBuilder.Build();

            var certificateKeyPemElement = iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/certificate_key_pem");
            Assert.That(certificateKeyPemElement, Is.Not.Null);

            Assert.That(certificateKeyPemElement, Is.AssignableTo(typeof(IReadWriteDataElement<CertificateData>)));

            var dataElement = (IReadWriteDataElement<CertificateData>)certificateKeyPemElement;

            var certificate = X509Certificate2Factory.CreateSelfSignedServerCertificate("myserver");

            var value = new CertificateData()
            {
                Certificate = certificate.ExportCertificatePem(),
                Key = certificate.GetRSAPrivateKey()?.ExportRSAPrivateKeyPem()
            };

            dataElement.Value = value;

            var cert2 = X509Certificate2.CreateFromPem(dataElement.Value.Certificate,
                certificate.GetRSAPrivateKey()?.ExportRSAPrivateKeyPem());


            Assert.That(cert2, Is.Not.Null);
            Assert.That(certificate.Thumbprint, Is.EqualTo(cert2.Thumbprint));

            Assert.Pass();
        }

        [Test]
        public void TestSetValidateClientCertificates()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);
            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetValidateClientCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            void OnHttpServerProfileBuilderOptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                dataStore.Set(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions.PropertyChanged += OnHttpServerProfileBuilderOptionsOnPropertyChanged;

            try
            {
                var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root,
                    httpServerProfileBuilderOptions);
                httpServerProfileBuilder.Build();

                var validateclientcertificateElement =
                    iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/validateclientcertificate");
                Assert.That(validateclientcertificateElement, Is.Not.Null);

                Assert.That(validateclientcertificateElement, Is.AssignableTo(typeof(IReadWriteDataElement<ushort>)));

                var validateClientCertificateDataElement =
                    (IReadWriteDataElement<ushort>)validateclientcertificateElement;

                Assert.That(validateClientCertificateDataElement.Value, Is.EqualTo(0));

                var trustedClientCertificatesElement = iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/trustedclientcertificates");
                Assert.That(trustedClientCertificatesElement, Is.Not.Null);

                Assert.That(trustedClientCertificatesElement, Is.AssignableTo(typeof(IReadWriteDataElement<string[]>)));

                var trustedClientCertificatesDataElement = (IReadWriteDataElement<string[]>)trustedClientCertificatesElement;
                var clientCertificate = X509Certificate2Factory.CreateSelfSignedClientCertificate("test").ExportCertificatePem();

                trustedClientCertificatesDataElement.Value = [clientCertificate];

                validateClientCertificateDataElement.Value = 1;

                var dataStore2 = new DataStore.DataStore(dataStoreFilePath);

                var options = dataStore2.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

                Assert.That(options.ValidateClientCertificates, Is.EqualTo(1));

            }
            finally
            {
                httpServerProfileBuilderOptions.PropertyChanged -= OnHttpServerProfileBuilderOptionsOnPropertyChanged;
            }
        }

        [Test]
        public void TestSetClientCertificatesFailOnInvalidClientCertificate()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);
            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetValidateClientCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            void OnHttpServerProfileBuilderOptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                dataStore.Set(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions.PropertyChanged += OnHttpServerProfileBuilderOptionsOnPropertyChanged;

            try
            {
                var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root,
                    httpServerProfileBuilderOptions);
                httpServerProfileBuilder.Build();

                var validateclientcertificateElement =
                    iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/validateclientcertificate");
                Assert.That(validateclientcertificateElement, Is.Not.Null);

                Assert.That(validateclientcertificateElement, Is.AssignableTo(typeof(IReadWriteDataElement<ushort>)));

                var validateClientCertificateDataElement =
                    (IReadWriteDataElement<ushort>)validateclientcertificateElement;

                Assert.That(validateClientCertificateDataElement.Value, Is.EqualTo(0));

                var trustedClientCertificatesElement = iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/trustedclientcertificates");
                Assert.That(trustedClientCertificatesElement, Is.Not.Null);

                Assert.That(trustedClientCertificatesElement, Is.AssignableTo(typeof(IReadWriteDataElement<string[]>)));

                var trustedClientCertificatesDataElement = (IReadWriteDataElement<string[]>)trustedClientCertificatesElement;
                var clientCertificate = "invalid certificate data";

                Assert.Throws<IoTCoreException>(() =>
                {
                    trustedClientCertificatesDataElement.Value = [clientCertificate];
                });

                Assert.Throws<IoTCoreException>(() =>
                {
                    validateClientCertificateDataElement.Value = 1;

                });


            }
            finally
            {
                httpServerProfileBuilderOptions.PropertyChanged -= OnHttpServerProfileBuilderOptionsOnPropertyChanged;
            }
        }

        [Test]
        public void TestSetValidateClientCertificatesFailOnEmptyCertificates()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);
            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetValidateClientCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;

            void OnHttpServerProfileBuilderOptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                dataStore.Set(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }

            httpServerProfileBuilderOptions.PropertyChanged += OnHttpServerProfileBuilderOptionsOnPropertyChanged;

            try
            {
                var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root,
                    httpServerProfileBuilderOptions);
                httpServerProfileBuilder.Build();

                var validateclientcertificateElement =
                    iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/validateclientcertificate");
                Assert.That(validateclientcertificateElement, Is.Not.Null);

                Assert.That(validateclientcertificateElement, Is.AssignableTo(typeof(IReadWriteDataElement<ushort>)));

                var validateClientCertificateDataElement =
                    (IReadWriteDataElement<ushort>)validateclientcertificateElement;

                Assert.That(validateClientCertificateDataElement.Value, Is.EqualTo(0));

                Assert.Throws<IoTCoreException>(() =>
                {
                    validateClientCertificateDataElement.Value = 1;
                });

                Assert.That(validateClientCertificateDataElement.Value, Is.EqualTo(0));
            }
            finally
            {
                httpServerProfileBuilderOptions.PropertyChanged -= OnHttpServerProfileBuilderOptionsOnPropertyChanged;
            }
        }

        [Test]
        public void TestSetTrustedClientCertificates()
        {
            var logger = new Log4NetLogger(LogLevel.Debug);
            var dataStoreFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "testSetTrustedClientCertificates.json");

            if (File.Exists(dataStoreFilePath))
            {
                File.Delete(dataStoreFilePath);
            }

            var dataStore = new DataStore.DataStore(dataStoreFilePath);

            var iotCore = IoTCoreFactory.Create("id", dataStore, logger);

            var httpServerProfileBuilderOptions = dataStore.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault) ?? new HttpServerProfileBuilderOptions();

            httpServerProfileBuilderOptions.CreateSecureServerElement = true;
            var httpServerProfileBuilder = new HttpServerProfileBuilder(iotCore.ElementManager, iotCore.Root, httpServerProfileBuilderOptions);

            httpServerProfileBuilder.Build();

            void HttpServerProfileBuilderOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
            {
                dataStore.Set<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault, httpServerProfileBuilderOptions);
            }



            httpServerProfileBuilderOptions.PropertyChanged += HttpServerProfileBuilderOptions_PropertyChanged;

            try
            {
                var trustedClientCertificatesElement =
                    iotCore.ElementManager.GetElementByAddress("id/httpserver/securesetup/trustedclientcertificates");

                Assert.That(trustedClientCertificatesElement, Is.Not.Null);
                Assert.That(trustedClientCertificatesElement, Is.AssignableTo(typeof(IReadWriteDataElement<string[]>)));

                var trustedClientCertificatesDataElement =
                    (IReadWriteDataElement<string[]>)trustedClientCertificatesElement;

                var rootCertificate = X509Certificate2Factory.CreateSelfSignedRootCaCertificate("root");
                var intermediateCertificate =
                    X509Certificate2Factory.CreateIntermediateCaCertificate("intermediate", rootCertificate, 0);
                var clientCertificate =
                    X509Certificate2Factory.CreateClientCertificate("client", intermediateCertificate, 0);

                trustedClientCertificatesDataElement.Value = new[]
                {
                    rootCertificate.ExportCertificatePem(),
                    intermediateCertificate.ExportCertificatePem(),
                    clientCertificate.ExportCertificatePem()
                };

                var dataStore2 = new DataStore.DataStore(dataStoreFilePath);

                var options2 = dataStore2.Get<HttpServerProfileBuilderOptions>(SectionKeyDefault, ValueKeyDefault);

                X509Certificate2Collection collection = new X509Certificate2Collection();
                collection.Add(rootCertificate);
                collection.Add(intermediateCertificate);
                collection.Add(clientCertificate);

                foreach (var item in options2.TrustedClientCertificates)
                {
                    var cert = X509Certificate2.CreateFromPem(item);
                    Assert.That(collection.Contains(cert), Is.True);
                }
            }
            finally
            {
                httpServerProfileBuilderOptions.PropertyChanged -= HttpServerProfileBuilderOptions_PropertyChanged;
            }
        }


    }
}
