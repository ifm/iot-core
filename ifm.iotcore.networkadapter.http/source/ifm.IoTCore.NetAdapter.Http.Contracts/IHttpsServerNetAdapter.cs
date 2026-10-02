namespace ifm.IoTCore.NetAdapter.Http.Contracts;

using System.Security.Cryptography.X509Certificates;
using NetAdapter.Contracts.Server;

public interface IHttpsServerNetAdapter : IServerNetAdapter
{
    string? CertificateThumbPrint { get; }
    X509Certificate2Collection? CertificateChain { get; set; }
}