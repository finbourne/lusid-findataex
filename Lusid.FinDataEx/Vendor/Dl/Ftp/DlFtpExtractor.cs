using Lusid.FinDataEx.Core;
using static Lusid.FinDataEx.Util.FdeRequestUtils;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpExtractor : FdeBaseExtractor<DlFtpRequest, DlFtpResponse>
    {

        public DlFtpExtractor(IVendorClient<DlFtpRequest, DlFtpResponse> vendorClient) : base(vendorClient)
        {
        }

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