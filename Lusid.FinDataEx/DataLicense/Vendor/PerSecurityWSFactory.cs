using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Vendor
{
    public class PerSecurityWSFactory
    {
        
        public const string BbgDlAddress = "https://dlws.bloomberg.com/dlps";
        public const string BbgDlCertEnvVar = "BBG_DL_CERT";
        public const string BbgDlPassEnvVar = "BBG_DL_PASS";

        public PerSecurityWS CreateDefault(string bbgDlAddress)
        {
            X509Certificate2 certificate = CreateCertificateFromEnvVar();
            return Create(bbgDlAddress, certificate);
        }
        
        public PerSecurityWS CreateDefault(string bbgDlAddress, string bbgDlCert, string bbgDlPass)
        {
            X509Certificate2 certificate =  new X509Certificate2(bbgDlCert, bbgDlPass);
            return Create(bbgDlAddress, certificate);
        }

        private PerSecurityWS Create(string bbgDlAddress, X509Certificate2 certificate)
        {
            Binding binding = CreateDefaultBinding();
            EndpointAddress endpointAddress = new EndpointAddress(bbgDlAddress);
            PerSecurityWSClient perSecurityWsClient = new PerSecurityWSClient(binding, endpointAddress);
            perSecurityWsClient.ClientCredentials.ClientCertificate.Certificate = certificate;
            return perSecurityWsClient;
        }

        private Binding CreateDefaultBinding()
        {
            BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
            basicHttpBinding.Name = "PerSecurityWSBinding";
            /*basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(1);
            basicHttpBinding.OpenTimeout = TimeSpan.FromMinutes(1);
            basicHttpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            basicHttpBinding.SendTimeout = TimeSpan.FromMinutes(1);*/
            basicHttpBinding.AllowCookies = false;
            basicHttpBinding.BypassProxyOnLocal = false;
            basicHttpBinding.MaxBufferPoolSize = 524288;
            basicHttpBinding.MaxBufferSize = 104857600;
            basicHttpBinding.MaxReceivedMessageSize = 104857600;
            basicHttpBinding.TextEncoding = System.Text.Encoding.UTF8;
            basicHttpBinding.TransferMode = TransferMode.Buffered;
            basicHttpBinding.UseDefaultWebProxy = true;
            basicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
            basicHttpBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            basicHttpBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
            return basicHttpBinding;
        }
        
        private X509Certificate2 CreateCertificateFromEnvVar()
        {
            string clientCertFilePath = Environment.GetEnvironmentVariable(BbgDlCertEnvVar);
            if (clientCertFilePath == null)
            {
                throw new ArgumentNullException($"Cannot connect to BBG DLWS without a client " +
                                                $"certificate. Ensure you've set environment variable {BbgDlCertEnvVar}");
            }
            string clientCertPassword = Environment.GetEnvironmentVariable(BbgDlPassEnvVar);
            if (clientCertPassword == null)
            {
                throw new ArgumentNullException($"Cannot connect to BBG DLWS without a client " +
                                                $"password. Ensure you've set environment variable {BbgDlPassEnvVar}");
            }
            return new X509Certificate2(clientCertFilePath, clientCertPassword);
        }
    }
}