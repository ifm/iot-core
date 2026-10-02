using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        public static bool TryParse(this string value, out X509Certificate2Collection? collection)
        {
            try
            {
                collection = Parse(value);
                return true;
            }
            catch
            {
                collection = default;
                return false;
            }
        }

        private static X509Certificate2Collection Parse(string value)
        {
            X509Certificate2Collection result = new X509Certificate2Collection();

            var currentString = value.Trim('\r', '\n');

            while (currentString.Length > 0)
            {
                var fields = PemEncoding.Find(currentString.ToCharArray());
                var length = fields.Location.End.Value - fields.Location.Start.Value;

                var currentCertificate = currentString.Substring(fields.Location.Start.Value, length);

                result.Add(X509Certificate2.CreateFromPem(currentCertificate));

                currentString = currentString.Substring(fields.Location.End.Value).Trim('\r', '\n');
            }

            return result;
        }
    }
}
