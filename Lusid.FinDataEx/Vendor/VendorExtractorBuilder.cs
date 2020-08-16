using System.IO;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Vendor;
using static Lusid.FinDataEx.Util.DynamicUtils;

namespace Lusid.FinDataEx.Core
{
    public class VendorExtractorBuilder
    {
        
        public IFdeExtractor createFDEExtractor(FdeRequest fdeRequest)
        {
            switch (fdeRequest.Vendor)
            {
                case Vendors.BBG:
                    return createBbgFDExtractor(fdeRequest);
                    break;
                default:
                    throw new InvalidDataException($"Vendor {fdeRequest.Vendor} is not currently supported.");
            }
        }

        private IFdeExtractor createBbgFDExtractor(FdeRequest fdeRequest)
        {
            dynamic config = fdeRequest.ConnectorConfig;
            if (HasProperty(config, "ConnectorType") && config.ConnectorType == "ftp")
            {
                return null;
            }

            return null;
        }
    }
}