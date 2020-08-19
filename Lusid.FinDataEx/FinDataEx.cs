using System;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        private readonly FdeRequestBuilder _fdeRequestBuilder;
        private readonly VendorExtractorBuilder _vendorExtractorBuilder;
        private readonly FdeResponseProcessorBuilder _fdeResponseProcessorBuilder;

        public FinDataEx(FdeRequestBuilder fdeRequestBuilder, VendorExtractorBuilder vendorExtractorBuilder, FdeResponseProcessorBuilder fdeResponseProcessorBuilder)
        {
            _fdeRequestBuilder = fdeRequestBuilder;
            _vendorExtractorBuilder = vendorExtractorBuilder;
            _fdeResponseProcessorBuilder = fdeResponseProcessorBuilder;
        }

        public void process(String fdeJsonRequest)
        {
            FdeRequest fdeRequest =  _fdeRequestBuilder.LoadFromFile(fdeJsonRequest);
            IFdeExtractor ifdExtractor = _vendorExtractorBuilder.CreateFdeExtractor(fdeRequest);
            IVendorResponseProcessor vendorResponseProcessor = _fdeResponseProcessorBuilder.CreateFdeResponseProcessor(fdeRequest);
            
            IVendorResponse vendorResponse = ifdExtractor.Extract(fdeRequest);
            vendorResponseProcessor.ProcessResponse(fdeRequest, vendorResponse);
        }
        
    }
}