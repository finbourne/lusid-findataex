using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Core;
using static Lusid.FinDataEx.Util.FdeRequestUtils;
using Lusid.FinDataEx.Vendor.Bbg.Ftp;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpExtractor : FdeBaseExtractor<DlFtpRequest, DlFtpResponse>
    {

        public DlFtpExtractor(IVendorClient<DlFtpRequest, DlFtpResponse> vendorClient) : base(vendorClient)
        {
        }

        protected override DlFtpRequest ToVendorRequest(FdeRequest request)
        {
            return new DlFtpRequest(
                GetConnectorConfigParameter(request.ConnectorConfig, ConUrl),
                GetConnectorConfigParameter(request.ConnectorConfig, ConUser),
                GetConnectorConfigParameter(request.ConnectorConfig, ConPass),
                GetRequestBodyParameter(request.RequestBody, ReqSourceData)
                );
        }

    }
}