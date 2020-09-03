using System;
using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor.Dl.Ftp;
using static Lusid.FinDataEx.Util.FdeRequestUtils;

namespace Lusid.FinDataEx.Vendor
{
    /// <summary>
    /// Builder for all currently supported vendors for financial data extraction.
    /// 
    /// </summary>
    public class VendorExtractorBuilder
    {

        public const string FtpConnector = "ftp";
        
        /// <summary>
        /// Create an extractor based on the parameters supplied by an FdeRequest
        /// 
        /// </summary>
        /// <param name="fdeRequest"> request for vendor data</param>
        /// <returns> an extractor for a specific vendor and connection flow (e.g. DL FTP)</returns>
        /// <exception cref="InvalidDataException"></exception>
        public IFdeExtractor CreateFdeExtractor(FdeRequest fdeRequest)
        {
            switch (fdeRequest.Vendor)
            {
                case Vendors.DL:
                    return CreateDlFdeExtractor(fdeRequest.ConnectorConfig);
                default:
                    throw new InvalidDataException($"Vendor {fdeRequest.Vendor} is not currently supported.");
            }
        }

        private IFdeExtractor CreateDlFdeExtractor(Dictionary<string,object> connectorConfig)
        {
            string connectorType = GetConnectorConfigParameter(connectorConfig, ConType);
            switch (connectorType)
            {
                case FtpConnector:
                    IVendorClient<DlFtpRequest, DlFtpResponse> dlClient =
                        new DlFileSystemClient(new DlFtpResponseBuilder());
                    DlFtpExtractor dlFtpExtractor = new DlFtpExtractor(dlClient);
                    Console.WriteLine("Using Dl ftp file based extractor");
                    return dlFtpExtractor;
                default:
                    throw new InvalidDataException(
                        $"No connector could be configured for connector type {connectorType}");
            }
        }
    }
}