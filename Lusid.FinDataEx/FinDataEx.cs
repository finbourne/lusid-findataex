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
        public void Process(string fdeRequestSource, string fdeJsonRequest)
        {
            // TODO : fdeRequestSource to Enum
            FdeRequest fdeRequest =  CreateFdeRequest(fdeRequestSource, fdeJsonRequest);
            Console.WriteLine($"Processing request from source {fdeRequestSource} with loaded fdeRequest={fdeRequest}");
            IFdeExtractor ifdExtractor = _vendorExtractorBuilder.CreateFdeExtractor(fdeRequest);
            IVendorResponseProcessor vendorResponseProcessor = _fdeResponseProcessorBuilder.CreateFdeResponseProcessor(fdeRequest);
            
            IVendorResponse vendorResponse = ifdExtractor.Extract(fdeRequest);
            ProcessResponseResult processResponseResult = vendorResponseProcessor.ProcessResponse(fdeRequest, vendorResponse);
            
            // TODO decide on how to handle fail/parital fail response. Fail entire scheduled run?
            if (processResponseResult.Status != ProcessResponseResultStatus.Ok)
            {
                Console.WriteLine("Fin data extraction completed with failures...");
            }
            Console.WriteLine(processResponseResult.Message);
        }

        public void Process(string fdeJsonRequest)
        {
            Process("FileSystem", fdeJsonRequest);
        }

        private FdeRequest CreateFdeRequest(string fdeRequestSource, string fdeJsonRequest)
        {
            if (fdeRequestSource.Equals("LusidDrive"))
            {
                return _fdeRequestBuilder.LoadFromLusidDrive(fdeJsonRequest);
                
            }
            return _fdeRequestBuilder.LoadFromFile(fdeJsonRequest);
        }
        
    }
}