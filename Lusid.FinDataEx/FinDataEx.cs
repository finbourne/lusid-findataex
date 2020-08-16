using System;
using Lusid.FinDataEx.Core;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        private readonly FdeRequestBuilder _fdeRequestBuilder;
        private readonly VendorExtractorBuilder _vendorExtractorBuilder;
        private readonly IFdeResponseProcessor _fdeResponseProcessor;

        public FinDataEx(FdeRequestBuilder fdeRequestBuilder, VendorExtractorBuilder vendorExtractorBuilder, IFdeResponseProcessor fdeResponseProcessor)
        {
            _fdeRequestBuilder = fdeRequestBuilder;
            _vendorExtractorBuilder = vendorExtractorBuilder;
            _fdeResponseProcessor = fdeResponseProcessor;
        }

        public void processJson(String fdeJsonRequest)
        {
            FdeRequest fdeRequest =  _fdeRequestBuilder.LoadFromJson(fdeJsonRequest);
            IFdeExtractor ifdExtractor = _vendorExtractorBuilder.createFDEExtractor(fdeRequest);
            FdeResponse fdeResponse = ifdExtractor.Extract(fdeRequest);
            _fdeResponseProcessor.ProcessResponse(fdeResponse);
        }
        
    }
}