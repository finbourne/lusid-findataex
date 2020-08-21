using Lusid.FinDataEx.Core;
using static Lusid.FinDataEx.Util.FdeRequestUtils;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    /// Extractor implementation for the DL vendor.
    /// 
    /// </summary>
    public class DlFtpExtractor : FdeBaseExtractor<DlFtpRequest, DlFtpResponse>
    {

        public DlFtpExtractor(IVendorClient<DlFtpRequest, DlFtpResponse> vendorClient) : base(vendorClient)
        {
        }

        /// <summary>
        ///  Constructs a request to extract financial data from DL via the FTP flow.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>a request for DL</returns>
        public override DlFtpRequest ToVendorRequest(FdeRequest request)
        {
            return new DlFtpRequest(
                GetConnectorConfigParameter(request.ConnectorConfig, ConUrl),
                GetConnectorConfigParameter(request.ConnectorConfig, ConUser),
                GetConnectorConfigParameter(request.ConnectorConfig, ConPass),
                GetRequestBodyParameter<string>(request.RequestBody, ReqSourceData),
            GetRequestBodyParameter<DlRequestType>(request.RequestBody, ReqRequestType)
                );
        }

    }
}