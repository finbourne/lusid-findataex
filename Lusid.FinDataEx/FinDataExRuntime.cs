using System;
using Lusid.FinDataEx.Core;

namespace Lusid.FinDataEx
{
    class FinDataExRuntime
    {
        static void Main(string[] args)
        {
            Console.WriteLine("running FinDataEx with args {0}", string.Join(", ", args));
            // TODO use framework accepts standard commanline args like -f. Must be a nuget? 
            string requestJson = args[0];
            FdeRequestBuilder fdeRequestBuilder = new FdeRequestBuilder();
            VendorExtractorBuilder vendorExtractorBuilder = new VendorExtractorBuilder();
            IFdeResponseProcessor fdeResponseProcessor = new LptFdeResponseProcessor();
            FinDataEx finDataEx = new FinDataEx(fdeRequestBuilder, vendorExtractorBuilder, fdeResponseProcessor);
            // TODO wrap with proper error handling for meaninful output if it fails
            finDataEx.processJson(requestJson);
        }
    }
}