using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ifm.IoTCore.NetAdapter.Http.Contracts
{
    public static class X509Certificate2Extensions
    {
        /// <summary>
        /// Determines whether the specified X.509 certificate is self-signed.
        /// </summary>
        /// <remarks>A certificate is considered self-signed if its subject and issuer are identical. This
        /// method does not validate the certificate's signature.</remarks>
        /// <param name="certificate">The X.509 certificate to evaluate. Cannot be null.</param>
        /// <returns>true if the certificate is self-signed; otherwise, false.</returns>
        public static bool IsSelfSigned(this X509Certificate2 certificate)
        {
            return certificate.SubjectName.RawData.SequenceEqual(certificate.IssuerName.RawData);
        }

        public static void SaveAsPfx(this X509Certificate2 certificate, string filePath)
        {
            File.WriteAllBytes(filePath, certificate.Export(X509ContentType.Pfx, (string?)null));
        }

        public static void SaveAsPem(this X509Certificate2 certificate, string basePath, string certificateFileName, string publicKeyFileName, string privateKeyFileName)
        {
            certificate.SavePublicKeyAsPem(Path.Combine(basePath, publicKeyFileName));
            certificate.SavePrivateKeyAsPem(Path.Combine(basePath, privateKeyFileName));
            certificate.SaveCertificateAsPem(Path.Combine(basePath, certificateFileName));
        }

        public static void SaveCertificateAsPem(this X509Certificate2 certificate, string certificateFilePath)
        {
            byte[] certificateBytes = certificate.RawData;
            char[] certificatePem = PemEncoding.Write("CERTIFICATE", certificateBytes);

            File.WriteAllText(certificateFilePath, new string(certificatePem));
        }

        public static void SavePublicKeyAsPem(this X509Certificate2 certificate, string publicKeyFilePath)
        {
            _ = certificate.RawData;
            
            var key = certificate.GetRSAPrivateKey() ?? (AsymmetricAlgorithm?)certificate.GetECDsaPrivateKey();

            if (key == null)
            {
                throw new InvalidOperationException("key may not be null");
            }

            byte[] pubKeyBytes = key.ExportSubjectPublicKeyInfo();
            char[] pubKeyPem = PemEncoding.Write("PUBLIC KEY", pubKeyBytes);
            
            File.WriteAllText(publicKeyFilePath, new string(pubKeyPem));
        }

        public static void SavePrivateKeyAsPem(this X509Certificate2 certificate, string privateKeyFilePath)
        {
            _ = certificate.RawData;
            AsymmetricAlgorithm? key = certificate.GetRSAPrivateKey() ?? (AsymmetricAlgorithm?)certificate.GetECDsaPrivateKey();

            if (key == null)
            {
                throw new InvalidOperationException("key may not be null");
            }

            byte[] privKeyBytes = key.ExportPkcs8PrivateKey();
            
            char[] privKeyPem = PemEncoding.Write("PRIVATE KEY", privKeyBytes);
            File.WriteAllText(privateKeyFilePath, new string(privKeyPem));
        }

        /// <summary>
        /// Certificates created in memory (e.g. using CertificateRequest.CreateSelfSigned) are ephemeral and
        /// do not have their private key material exportable by default. This method creates a new
        /// certificate with the same data but with an exportable private key.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns>A new certificate with an exportable private key.</returns>
        public static X509Certificate2 EphemeralToNonEphemeral(this X509Certificate2 certificate)
        {
            var export = certificate.Export(X509ContentType.Pkcs12);

            var result = new X509Certificate2(export,
                (string?)null,
                //X509KeyStorageFlags.PersistKeySet | 
                X509KeyStorageFlags.Exportable |
                X509KeyStorageFlags.MachineKeySet);

            return result;
        }
    }
}
