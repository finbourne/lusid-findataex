using System;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx
{
    /// <summary>
    ///  Wrapper class for the FinDataEx workflow
    /// </summary>
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

        /// <summary>
        /// Processes an FdeRequest provided by the caller. Loads the provided request and retrieve the financial data
        /// using the vendor and connector flow (e.g. DL via FTP) included in the request. The financial data is then processed
        /// based on the behaviour defined in the request (e.g. lusidtools which outputs to .csv)
        /// 
        /// </summary>
        /// <param name="fdeJsonRequest"></param>
        public void Process(String fdeJsonRequest)
        {
            FdeRequest fdeRequest =  _fdeRequestBuilder.LoadFromFile(fdeJsonRequest);
            IFdeExtractor ifdExtractor = _vendorExtractorBuilder.CreateFdeExtractor(fdeRequest);
            IVendorResponseProcessor vendorResponseProcessor = _fdeResponseProcessorBuilder.CreateFdeResponseProcessor(fdeRequest);
            
            IVendorResponse vendorResponse = ifdExtractor.Extract(fdeRequest);
            ProcessResponseResult processResponseResult = vendorResponseProcessor.ProcessResponse(fdeRequest, vendorResponse);
            
            // TODO decide on how to handle fail/parital fail respsonse. Fail entire scheduled run?
            if (processResponseResult.Status != ProcessResponseResultStatus.Ok)
            {
                Console.WriteLine("Fin data extraction completed with failures...");
            }
            Console.WriteLine(processResponseResult.Message);
        }
        
    }
}