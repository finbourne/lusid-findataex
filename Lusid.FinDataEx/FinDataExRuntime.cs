using System;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx
{
    public class FinDataExRuntime
    {
        // TODO use framework accepts standard commanline args like -f. Must be a nuget? 
        // e.g. lusidfindataex -f "/var/fderequests/fde_req_2020_08_14.json" -o "/var/outputdata/2020_08_14" 
        // e.g. lusidfindataex -j "{req : ""...}" -o "/var/outputdata/2020_08_14" 
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException($"Invalid arguments {string.Join(", ", args)}. Arguments" +
                                            $" must be of format \"lusidfindataex -f <request_file_path> -o <outputdir>\"");
            }
            
            string requestJsonFilePath = args[0];
            string outputDir = args[1];
            Console.WriteLine($"running FinDataEx for request={requestJsonFilePath}, output directory={outputDir}.");
            
            FdeRequestBuilder fdeRequestBuilder = new FdeRequestBuilder();
            VendorExtractorBuilder vendorExtractorBuilder = new VendorExtractorBuilder();
            FdeResponseProcessorBuilder fdeResponseProcessorBuilder = new FdeResponseProcessorBuilder(outputDir);
            FinDataEx finDataEx = new FinDataEx(fdeRequestBuilder, vendorExtractorBuilder, fdeResponseProcessorBuilder);
            // TODO wrap with proper error handling for meaninful output if it fails
            finDataEx.process(requestJsonFilePath);
        }
    }
}