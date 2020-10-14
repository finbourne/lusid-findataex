using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Lusid.FinDataEx.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Vendor
{
    /// <summary>
    /// Factory for creating BBG DL web service proxy class. Default setups should
    /// work fine for most use cases but if specific settings are needed (e.g. increased timeouts
    /// due to large calls) than new factory creation methods should be added here.
    /// </summary>
    public class PerSecurityWsFactory
    {
        
        public const string BbgDlAddress = "https://dlws.bloomberg.com/dlps";
        public const string BbgDlCertEnvVar = "BBG_DL_CERT";
        public const string BbgDlPassEnvVar = "BBG_DL_PASS";

        /// <summary>
        ///  Creates a default BBG DLWS client using default bbg ws address 
        ///  (https://dlws.bloomberg.com/dlps). Retrieves certificates and
        /// password details from environment variables BBG_DL_CERT and BBG_DL_PASS
        /// </summary>
        /// <returns></returns>
        public PerSecurityWS CreateDefault()
        {
            var certificate = CreateCertificateFromEnvVar();
            return Create(BbgDlAddress, certificate);
        }
        
        /// <summary>
        ///  Creates a default BBG DLWS client against specified address and
        ///  authentication and retrieves certificates and
        /// password details from environment variables BBG_DL_CERT and BBG_DL_PASS
        /// </summary>
        /// <param name="bbgDlAddress"></param>
        /// <returns></returns>
        public PerSecurityWS CreateDefault(string bbgDlAddress)
        {
            var certificate = CreateCertificateFromEnvVar();
            return Create(bbgDlAddress, certificate);
        }
        
        /// <summary>
        ///  Creates a default BBG DLWS client.
        /// </summary>
        /// <param name="bbgDlAddress"></param>
        /// <param name="bbgDlCert"></param>
        /// <param name="bbgDlPass"></param>
        /// <returns></returns>
        public PerSecurityWS CreateDefault(string bbgDlAddress, string bbgDlCert, string bbgDlPass)
        {
            var certificate =  new X509Certificate2(bbgDlCert, bbgDlPass);
            return Create(bbgDlAddress, certificate);
        }

        private PerSecurityWS Create(string bbgDlAddress, X509Certificate2 certificate)
        {
            var binding = CreateDefaultBinding();
            var endpointAddress = new EndpointAddress(bbgDlAddress);
            var perSecurityWsClient = new PerSecurityWSClient(binding, endpointAddress);
            perSecurityWsClient.ClientCredentials.ClientCertificate.Certificate = certificate;
            return perSecurityWsClient;
        }

        private Binding CreateDefaultBinding()
        {
            var basicHttpBinding = new BasicHttpBinding
            {
                Name = "PerSecurityWSBinding",
                AllowCookies = false,
                BypassProxyOnLocal = false,
                MaxBufferPoolSize = 524288,
                MaxBufferSize = 104857600,
                MaxReceivedMessageSize = 104857600,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true,
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport,
                    Transport =
                    {
                        ClientCredentialType = HttpClientCredentialType.Certificate,
                        ProxyCredentialType = HttpProxyCredentialType.None
                    },
                    Message = {ClientCredentialType = BasicHttpMessageCredentialType.Certificate}
                }
            };
            // Need to review docs on using the timeouts
            /*basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(1);
            basicHttpBinding.OpenTimeout = TimeSpan.FromMinutes(1);
            basicHttpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            basicHttpBinding.SendTimeout = TimeSpan.FromMinutes(1);*/
            return basicHttpBinding;
        }
        
        private X509Certificate2 CreateCertificateFromEnvVar()
        {
            var clientCertFilePath = Environment.GetEnvironmentVariable(BbgDlCertEnvVar);
            if (clientCertFilePath == null)
            {
                throw new ArgumentNullException($"Cannot connect to BBG DLWS without a client " +
                                                $"certificate. Ensure you've set environment variable {BbgDlCertEnvVar}");
            }
            var clientCertPassword = Environment.GetEnvironmentVariable(BbgDlPassEnvVar);
            if (clientCertPassword == null)
            {
                throw new ArgumentNullException($"Cannot connect to BBG DLWS without a client " +
                                                $"password. Ensure you've set environment variable {BbgDlPassEnvVar}");
            }
            Console.WriteLine($"Retrieving BBG DL certificate {clientCertFilePath}");
            return new X509Certificate2(clientCertFilePath, clientCertPassword);
        }
        
    }
}