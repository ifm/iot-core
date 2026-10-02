using System;
using ifm.Common;
using ifm.IoTCore.NetAdapter.Http.Contracts;

namespace ifm.IoTCore.NetAdapter.Http.Profile.HttpServer
{
    public class HttpServerProfileBuilderOptions : NotifyPropertyChangedBase
    {
        private string _listenAddress = "0.0.0.0";
        private int _port = 80;
        private bool _createAllowedServicesElement;
        private AllowedServicesMode _allowedServicesMode = AllowedServicesMode.All;
        private int _secure = 0;
        private CertificateData? _certificateData = new CertificateData();
        private int _validateClientCertificates;
        private string[] _trustedClientCertificates = Array.Empty<string>();
        private bool _createSecureServerElement;
        
        public bool CreateSecureServerElement
        {
            get => _createSecureServerElement;
            set
            {
                if (value == _createSecureServerElement) return;
                _createSecureServerElement = value;
                RaisePropertyChanged();
            }
        }

        public string ListenAddress
        {
            get => _listenAddress;
            set
            {
                if (value == _listenAddress) return;
                _listenAddress = value;
                RaisePropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (value == _port) return;
                _port = value;
                RaisePropertyChanged();
            }
        }

        public bool CreateAllowedServicesElement
        {
            get => _createAllowedServicesElement;
            set
            {
                if (value == _createAllowedServicesElement) return;
                _createAllowedServicesElement = value;
                RaisePropertyChanged();
            }
        }

        public AllowedServicesMode AllowedServicesMode
        {
            get => _allowedServicesMode;
            set
            {
                if (value == _allowedServicesMode) return;
                _allowedServicesMode = value;
                RaisePropertyChanged();
            }
        }

        public int Secure
        {
            get => _secure;
            set
            {
                if (value == _secure) return;
                _secure = value;
                RaisePropertyChanged();
            }
        }

        public CertificateData? CertificateData
        {
            get => _certificateData;
            set
            {
                if (Equals(value, _certificateData)) return;
                _certificateData = value;
                RaisePropertyChanged();
            }
        }

        public int ValidateClientCertificates
        {
            get => _validateClientCertificates;
            set
            {
                if (value == _validateClientCertificates) return;
                _validateClientCertificates = value;
                RaisePropertyChanged();
            }
        }

        public string[] TrustedClientCertificates
        {
            get => _trustedClientCertificates;
            set
            {
                if (Equals(value, _trustedClientCertificates)) return;
                _trustedClientCertificates = value;
                RaisePropertyChanged();
            }
        }
    }
}
