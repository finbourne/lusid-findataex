using System;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx
{
    /// <summary>
    /// Central class for the FinDataEx workflow. 
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
        /// Processes an FdeRequest provided by the caller. Loads the provided request and retrieves the financial data
        /// using the vendor and connector flow (e.g. DL via FTP) included in the request. The financial data is then processed
        /// based on the behaviour defined in the request (e.g. lusidtools which outputs to .csv)
        /// 
        /// </summary>
        /// <param name="fdeRequestSource"></param>
        /// <param name="fdeRequestPath"></param>
        public void Process(FdeRequestSource fdeRequestSource, string fdeRequestPath)
        {
            // load and construct request from source
            var fdeRequest =  CreateFdeRequest(fdeRequestSource, fdeRequestPath);
            Console.WriteLine($"Processing request from source {fdeRequestSource} with content={fdeRequest}");
            
            // setup the vendor extractor that has been selected in the request
            IFdeExtractor ifdExtractor = _vendorExtractorBuilder.CreateFdeExtractor(fdeRequest);
            // setup the vendor response processor that has been selected in the request
            IVendorResponseProcessor vendorResponseProcessor = _fdeResponseProcessorBuilder.CreateFdeResponseProcessor(fdeRequest);
            
            // run the extract against an external vendor
            IVendorResponse vendorResponse = ifdExtractor.Extract(fdeRequest);
            
            // process the vendor specific extract response and display errors if any exist
            ProcessResponseResult processResponseResult = vendorResponseProcessor.ProcessResponse(fdeRequest, vendorResponse);
            // TODO how to handle partial failures in requests
            if (processResponseResult.Status != ProcessResponseResultStatus.Ok)
            {
                Console.Error.WriteLine("FinDataEx request completed with failures. See details below: ");
                Console.Error.WriteLine(processResponseResult);
            }
            else
            {
                Console.WriteLine(processResponseResult.Message);
            }
        }

        /// <summary>
        ///  Default behaviour attempts to load requests from the local file system
        /// </summary>
        /// <param name="fdeRequestPath"></param>
        public void Process(string fdeRequestPath)
        {
            Process(FdeRequestSource.FileSystem, fdeRequestPath);
        }

        /// <summary>
        /// Load and construct the fde request from the source and path/id provided.
        /// 
        /// </summary>
        /// <param name="fdeRequestSource"></param>
        /// <param name="fdeRequestPath"></param>
        /// <returns></returns>
        private FdeRequest CreateFdeRequest(FdeRequestSource fdeRequestSource, string fdeRequestPath)
        {
            if (fdeRequestSource.Equals(FdeRequestSource.LusidDrive))
            {
                return _fdeRequestBuilder.LoadFromLusidDrive(fdeRequestPath);
                
            }
            return _fdeRequestBuilder.LoadFromFile(fdeRequestPath);
        }
        
    }
}