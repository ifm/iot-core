using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ifm.IoTCore.NetAdapter.Http.Contracts
{
    public static class X509Certificate2Factory
    {
        public static X509Certificate2 CreateCertificate(string commonName,
            IEnumerable<X509Extension>? extensions,
            X509Certificate2? issuerCertificate = null,
            uint serialNumber = 0)
        {
            var rsaKeySize = 2048;
            var years = 5;
            var hashAlgorithm = HashAlgorithmName.SHA256;

            using (var rsa = RSA.Create(rsaKeySize))
            {
                var request = new CertificateRequest($"cn={commonName}", rsa, hashAlgorithm, RSASignaturePadding.Pkcs1);

                if (extensions != null)
                {
                    foreach (var extension in extensions)
                    {
                        request.CertificateExtensions.Add(extension);
                    }
                }

                X509Certificate2 result;

                if (issuerCertificate == null)
                {
                    result =
                        request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(years));
                }
                else
                {
                    result = request.Create(issuerCertificate,
                        DateTimeOffset.Now.AddDays(-1),
                        issuerCertificate.NotAfter.Subtract(TimeSpan.FromDays(2)),
                        Enumerable.Reverse(BitConverter.GetBytes(serialNumber)).ToArray());
                }

                if (!result.HasPrivateKey)
                {
                    result = result.CopyWithPrivateKey(rsa);
                }

                // On windows the certificates created here will be ephemeral keys. These wont work further down the line when an SSL Channel is opened:
                // See: https://github.com/dotnet/runtime/issues/23749
                // That means, if we are on windows, then make the ephemeral key a machine key, by saving it into a X509Store.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return result.EphemeralToNonEphemeral();
                }
                else
                {
                    return result;
                }
            }
        }

        public static X509Certificate2 CreateSelfSignedRootCaCertificate(string commonName)
        {
            return CreateCertificate(commonName, new List<X509Extension>
            {
                new X509BasicConstraintsExtension(true,
                    false,
                    0,
                    true),

                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.KeyCertSign |
                    X509KeyUsageFlags.DigitalSignature,
                    false)
            });
        }

        public static X509Certificate2 CreateSelfSignedServerCertificate(string commonName)
        {
            return CreateCertificate(commonName, new List<X509Extension>
            {
                new X509BasicConstraintsExtension(false,
                    false,
                    0,
                    false),

                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.KeyCertSign |
                    X509KeyUsageFlags.DigitalSignature,
                    false),

                new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false)
            });
        }

        public static X509Certificate2 CreateSelfSignedClientCertificate(string commonName)
        {
            return CreateCertificate(commonName, new List<X509Extension>
            {
                new X509BasicConstraintsExtension(true,
                    false,
                    0,
                    true),

                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.KeyCertSign |
                    X509KeyUsageFlags.DigitalSignature,
                    false),

                new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, false)
            });
        }



        public static X509Certificate2 CreateIntermediateCaCertificate(string commonName, X509Certificate2 rootCaCertificate, uint serialNumber)
        {
            return CreateCertificate(commonName, new List<X509Extension>
            {
                new X509BasicConstraintsExtension(true,
                    true,
                    0,
                    true),

                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment
                    | X509KeyUsageFlags.KeyEncipherment
                    | X509KeyUsageFlags.KeyCertSign
                    | X509KeyUsageFlags.DigitalSignature
                    , false)

            }, rootCaCertificate, serialNumber);
        }

        public static X509Certificate2 CreateServerCertificate(string commonName, X509Certificate2 intermediateCaCertificate, uint serialNumber)
        {
            return CreateCertificate(commonName, new List<X509Extension>
            {
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.KeyCertSign |
                    X509KeyUsageFlags.DigitalSignature, false),
                new X509EnhancedKeyUsageExtension(
                    new OidCollection {new Oid("1.3.6.1.5.5.7.3.1")}, false)
            }, intermediateCaCertificate, serialNumber);
        }

        public static X509Certificate2 CreateClientCertificate(string commonName, X509Certificate2 intermediateCaCertificate, uint serialNumber)
        {

            return CreateCertificate(commonName, new List<X509Extension>
            {
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.DataEncipherment |
                    X509KeyUsageFlags.KeyEncipherment |
                    X509KeyUsageFlags.KeyCertSign |
                    X509KeyUsageFlags.DigitalSignature,
                    false),
                new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") }, false)
            }, intermediateCaCertificate, serialNumber);
        }
    }
}
