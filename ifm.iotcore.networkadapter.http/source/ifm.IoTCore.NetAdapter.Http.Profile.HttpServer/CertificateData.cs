using ifm.Common;
using ifm.Common.Variant;

namespace ifm.IoTCore.NetAdapter.Http.Profile.HttpServer;

public class CertificateData: NotifyPropertyChangedBase
{
    [VariantProperty("certificate")] 
    public string? Certificate { get; set; }

    [VariantProperty("key")]
    public string? Key { get; set; }

    [VariantProperty("password")]
    public string? Password { get; set; }
}