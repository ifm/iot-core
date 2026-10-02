using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ifm.IoTCore.Common;
using ifm.IoTCore.Common.Exceptions;
using ifm.IoTCore.ElementManager.Contracts;
using ifm.IoTCore.ElementManager.Contracts.Elements;
using ifm.IoTCore.ElementManager.Contracts.Elements.Formats;
using ifm.IoTCore.ElementManager.Contracts.Elements.Valuations;
using ifm.IoTCore.NetAdapter.Http.Contracts;

namespace ifm.IoTCore.NetAdapter.Http.Profile.HttpServer
{
    public class HttpServerProfileBuilder
    {
        private readonly IElementManager _elementManager;
        private readonly IBaseElement _targetElement;
        private readonly HttpServerProfileBuilderOptions _httpServerProfileBuilderOptions;

        public HttpServerProfileBuilder(IElementManager elementManager,
            IBaseElement targetElement,
            HttpServerProfileBuilderOptions httpServerProfileBuilderOptions)
        {
            _elementManager = elementManager;
            _targetElement = targetElement;
            _httpServerProfileBuilderOptions = httpServerProfileBuilderOptions ?? throw new ArgumentNullException(nameof(httpServerProfileBuilderOptions));
        }

        public void Build()
        {
            var httpServerElement = _elementManager.CreateStructureElement(_targetElement, "httpserver", profiles: ["httpserver"]);

            _ = _elementManager.CreateDataElement(httpServerElement, "port",
                (_, serviceInfo) => _httpServerProfileBuilderOptions.Port,
                (_, value, serviceInfo) =>
                {
                    if (value < 1 || value > 65535)
                    {
                        throw new IoTCoreException(ResponseCodes.DataInvalid, "Port number must be between 1 and 65535.");
                    }

                    if (_httpServerProfileBuilderOptions.Port == value) return;

                    var options = _httpServerProfileBuilderOptions;
                    TcpListener? listener = null;
                    try
                    {
                        if (IPAddress.TryParse(options.ListenAddress, out var ipAddress))
                        {
                            listener = new TcpListener(ipAddress, value);
                            listener.Start();
                        }
                        else
                        {
                            throw new Exception("Invalid listen IP address");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new IoTCoreException(ResponseCodes.DataInvalid, e.Message);
                    }
                    finally
                    {
                        listener?.Dispose();
                    }

                    options.Port = value;

                    serviceInfo ??= new ServiceInfo();
                    serviceInfo.ResponseCode = ResponseCodes.SuccessReboot;

                }, createDataChangedEventElement: false, profiles: ["parameter"]);

            if (_httpServerProfileBuilderOptions.CreateAllowedServicesElement)
            {
                _elementManager.CreateDataElement(httpServerElement, "allowed_services",
                    (_, serviceInfo) => _httpServerProfileBuilderOptions.AllowedServicesMode,
                    (_, i, serviceInfo) =>
                    {
                        if (!Enum.IsDefined(typeof(AllowedServicesMode), i))
                        {
                            throw new ArgumentOutOfRangeException(
                                $"The value '{i}' is not a valid value for enumeration {typeof(AllowedServicesMode)}");
                        }

                        _httpServerProfileBuilderOptions.AllowedServicesMode = i;

                        serviceInfo ??= new ServiceInfo();
                        serviceInfo.ResponseCode = ResponseCodes.SuccessReboot;
                    },
                    true,
                    format: new IntegerEnumFormat(new IntegerEnumValuation(new Dictionary<string, string>
                    {
                        {"0", "All services allowed"},
                        {"1", "Only getidentity service allowed"}
                    })));
            }

            if (_httpServerProfileBuilderOptions.CreateSecureServerElement)
            {
                var secureSetupElement = _elementManager.CreateStructureElement(httpServerElement, "securesetup", profiles: ["secureserver"]);

                _ = _elementManager.CreateDataElement(secureSetupElement, "secure",
                    _ => _httpServerProfileBuilderOptions.Secure,
                    (_, value) =>
                    {
                        _httpServerProfileBuilderOptions.Secure = value;
                    },
                    format: new IntegerEnumFormat(new IntegerEnumValuation(new Dictionary<string, string>
                    {
                    { "0", "disabled" },
                    { "1", "enabled" }
                    })));

                _ = _elementManager.CreateDataElement<CertificateData>(secureSetupElement, "certificate_key_pem",
                    (_, serviceInfo) => new CertificateData()
                    {
                        Certificate = _httpServerProfileBuilderOptions.CertificateData?.Certificate
                    },
                    (_, value, serviceInfo) =>
                    {
                        if (value.Certificate == null)
                        {
                            throw new InvalidOperationException("Cannot set certificate without value for certificate.");
                        }

                        if (value.Key == null)
                        {
                            throw new InvalidOperationException("Cannot set certificate without value for key.");
                        }

                        if (value.Certificate.TryParse(out var collection))
                        {
                            var serverCertificateWithoutKey = collection.ExtractCertificateChainLeaf();

                            if (serverCertificateWithoutKey == null)
                            {
                                throw new InvalidOperationException("Could not determine the server certificate in the certificate input string.");
                            }

                            var serverCertifcateWithoutKeyPem = serverCertificateWithoutKey.ExportCertificatePem();

                            X509Certificate2 serverCertificate;

                            if (value.Password == null)
                            {
                                serverCertificate = X509Certificate2.CreateFromPem(serverCertifcateWithoutKeyPem, value.Key.ToCharArray());
                            }
                            else
                            {
                                serverCertificate = X509Certificate2.CreateFromEncryptedPem(serverCertifcateWithoutKeyPem, value.Key.ToCharArray(), value.Password.ToCharArray());
                            }

                            var extensions = serverCertificate.Extensions;

                            var containsServerOid = from e in extensions.OfType<X509EnhancedKeyUsageExtension>()
                                                    let key = e.EnhancedKeyUsages.OfType<Oid>()
                                                    from oid in key
                                                    where oid.Value == "1.3.6.1.5.5.7.3.1"
                                                    select oid;

                            if (!containsServerOid.Any())
                            {
                                throw new IoTCoreException(ResponseCodes.DataInvalid,
                                    "No certifcate has the X509EnhancedKeyUsageExtension with the Oid for Tls Server: '1.3.6.1.5.5.7.3.1'. Please add an X509EnhancedKeyUsageExtension to the server certificate with the Oid '1.3.6.1.5.5.7.3.1'. ");
                            }

                            _httpServerProfileBuilderOptions.CertificateData = new CertificateData()
                            {
                                Certificate = value.Certificate,
                                Key = value.Key,
                                Password = value.Password
                            };

                            serviceInfo ??= new ServiceInfo();
                            serviceInfo.ResponseCode = ResponseCodes.SuccessReboot;

                            foreach (var item in collection ?? Enumerable.Empty<X509Certificate2>())
                            {
                                item.Dispose();
                            }

                            collection?.Clear();
                        }
                        else
                        {
                            throw new IoTCoreException(ResponseCodes.DataInvalid);
                        }
                    },
                    format: new ObjectFormat(new ObjectValuation(new List<ObjectValuation.Field>
                    {
                    new ObjectValuation.Field("certificate", new StringFormat(), optional:true),
                    new ObjectValuation.Field("key", new StringFormat(), optional:true),
                    new ObjectValuation.Field("password", new StringFormat(), optional:true),

                    })));

                _ = _elementManager.CreateDataElement<ushort>(secureSetupElement,
                "validateclientcertificate",
                (_, _) => Convert.ToUInt16(_httpServerProfileBuilderOptions.ValidateClientCertificates),
                (_, value, serviceInfo) =>
                {
                    if (value < 0 || value > 1)
                    {
                        throw new IoTCoreException(ResponseCodes.DataInvalid);
                    }

                    if (value == 1)
                    {
                        if (_httpServerProfileBuilderOptions.TrustedClientCertificates == null ||
                            _httpServerProfileBuilderOptions.TrustedClientCertificates.Length == 0)
                        {
                            throw new IoTCoreException(ResponseCodes.FailedDependency, "Cannot require client certificates when no trusted client certificates are configured.");
                        }
                        else
                        {
                            if (!TryLoadCertificates(_httpServerProfileBuilderOptions.TrustedClientCertificates))
                            {
                                throw new IoTCoreException(ResponseCodes.DataInvalid, "One or more of the provided certificates are invalid.");
                            }
                        }
                    }

                    _httpServerProfileBuilderOptions.ValidateClientCertificates = value;

                    serviceInfo ??= new ServiceInfo();
                    serviceInfo.ResponseCode = ResponseCodes.SuccessReboot;
                },
                format: new IntegerEnumFormat(new IntegerEnumValuation(new Dictionary<string, string>
                {
                    {"0", "none"},
                    {"1", "require"}
                })));

                var trustedClientCertificatesElement = _elementManager.CreateDataElement<string[]?>(secureSetupElement,
                    "trustedclientcertificates",
                    (_, _) => _httpServerProfileBuilderOptions.TrustedClientCertificates,
                    (_, value, serviceInfo) =>
                    {
                        if (value != null && value.Length > 0)
                        {
                            if (!TryLoadCertificates(value))
                            {
                                throw new IoTCoreException(ResponseCodes.DataInvalid, "One or more of the provided certificates are invalid.");
                            }
                        }

                        _httpServerProfileBuilderOptions.TrustedClientCertificates = value ?? [];

                        serviceInfo ??= new ServiceInfo();
                        serviceInfo.ResponseCode = ResponseCodes.SuccessReboot;
                    },
                    format: new ArrayFormat(new ArrayValuation(new StringFormat())));
            }
        }

        private bool TryLoadCertificates(string[]? certificates)
        {
            if (certificates == null || certificates.Length == 0)
            {
                return false;
            }

            var exceptions = new List<Exception>();

            var list = new List<X509Certificate2>();

            foreach (var cert in certificates)
            {
                try
                {
                    var certificateInstance = X509Certificate2.CreateFromPem(cert);
                    list.Add(certificateInstance);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            foreach (var item in list.ToArray())
            {
                try
                {
                    item.Dispose();
                }
                finally
                {
                    list.Remove(item);
                }
            }

            if (exceptions.Any())
            {
                return false;
            }

            return true;
        }
    }
}
