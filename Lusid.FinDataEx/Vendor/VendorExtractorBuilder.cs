using System;
using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor.Dl.Ftp;
using static Lusid.FinDataEx.Util.FdeRequestUtils;

namespace Lusid.FinDataEx.Vendor
{
    public class VendorExtractorBuilder
    {
        
        public IFdeExtractor CreateFdeExtractor(FdeRequest fdeRequest)
        {
            switch (fdeRequest.Vendor)
            {
                case Vendors.DL:
                    return CreateDlFdeExtractor(fdeRequest.ConnectorConfig);
                    break;
                default:
                    throw new InvalidDataException($"Vendor {fdeRequest.Vendor} is not currently supported.");
            }
        }

        private IFdeExtractor CreateDlFdeExtractor(Dictionary<string,object> connectorConfig)
        {

            string connectorType = GetConnectorConfigParameter(connectorConfig, ConType);
            //if (HasProperty(config, "type") && config.type == "ftp")
            if (connectorType == "ftp")
            {
                IVendorClient<DlFtpRequest, DlFtpResponse> dlClient = new DlFileSystemClient(new DlFtpResponseBuilder());
                DlFtpExtractor dlFtpExtractor = new DlFtpExtractor(dlClient);
                Console.WriteLine("Using Dl ftp file based extractor");
                return dlFtpExtractor;
            }
            else
            {
                throw new InvalidDataException($"No connector could be configured for connector type {connectorType}");
            }
            
        }
    }
}