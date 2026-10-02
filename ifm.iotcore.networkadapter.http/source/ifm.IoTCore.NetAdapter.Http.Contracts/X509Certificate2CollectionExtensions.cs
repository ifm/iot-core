using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ifm.IoTCore.NetAdapter.Http.Contracts
{
    public static class X509Certificate2CollectionExtensions
    {
        public static X509Certificate2? ExtractCertificateChainLeaf(this X509Certificate2Collection? collection)
        {
            if (collection == null)
            {
                return null;
            }

            if (collection.Count == 0)
            {
                throw new InvalidOperationException("Certificate chain is empty. Please provide a certificate chain.");
            }

            if (collection.Count == 1)
            {
                return collection.Single();
            }

            if (collection.Count > 1)
            {
                var certificates = collection.ToList();
                var currentCertificate = collection.FirstOrDefault(x => x.Issuer == x.Subject);
                
                if (currentCertificate == null)
                {
                    currentCertificate = certificates.FirstOrDefault(x => certificates.All(y => x.Issuer != y.Subject ));
                }
                else
                {
                    certificates.Remove(currentCertificate);
                    currentCertificate = certificates.FirstOrDefault(x => certificates.All(y => x.Issuer != y.Subject));
                }
                
                while (certificates.Count > 1)
                {
                    if (currentCertificate != null)
                    {
                        var next = certificates.Single(x => x.Issuer == currentCertificate.Subject);
                        certificates.Remove(currentCertificate);
                        currentCertificate = next;
                    }
                }

                return certificates.Single();
            }

            throw new InvalidDataException("Could not extract certificate from certificate chain.");
        }
    }
}
