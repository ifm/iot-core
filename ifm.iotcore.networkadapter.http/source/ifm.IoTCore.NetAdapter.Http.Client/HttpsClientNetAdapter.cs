namespace ifm.IoTCore.NetAdapter.Http.Client;

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MessageConverter.Contracts;

public class HttpsClientNetAdapter : HttpClientNetAdapter
{
    /// <summary>
    /// Initializes a new instance of <see cref="HttpsClientNetAdapter"/>.
    /// </summary>
    /// <param name="remoteUri">The uri the client will send requests to.</param>
    /// <param name="converter">The message converter to de/serialize messages.</param>
    /// <param name="timeout">The connection timeout.</param>
    /// <param name="keepAlive">If true, the client connection should be kept alive.</param>
    /// <param name="serverCertificateValidation">A callback to validate the server certificate.If null, the default value of the callback will be used (which is dependent of the underlying framework).</param>
    /// <param name="x509CertificateCollection">A collection that contains the client certificates, that this client should present, if the remote server requests client certificates.</param>
    public HttpsClientNetAdapter(Uri remoteUri,
        IMessageConverter converter,
        TimeSpan timeout,
        bool keepAlive,
        Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> serverCertificateValidation,
        X509CertificateCollection x509CertificateCollection) : 
        base(remoteUri, converter, timeout, keepAlive, CreateClientHandler(serverCertificateValidation, x509CertificateCollection))
    {
    }

    private static HttpClientHandler CreateClientHandler(Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> serverCertificateValidation, 
        X509CertificateCollection x509CertificateCollection)
    {
        var httpClientHandler = new HttpClientHandler();
        if (x509CertificateCollection != null)
        {
            foreach (var item in x509CertificateCollection)
            {
                httpClientHandler.ClientCertificates.Add(item);
            }
        }
        httpClientHandler.ServerCertificateCustomValidationCallback = serverCertificateValidation ?? httpClientHandler.ServerCertificateCustomValidationCallback;
        return httpClientHandler;
    }
}