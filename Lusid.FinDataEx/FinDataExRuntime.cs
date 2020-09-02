using System;
using Lusid.FinDataEx.Core;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx
{
    /// <summary>
    /// Runtime main class called to trigger a financial data extract
    /// 
    /// </summary>
    public class FinDataExRuntime
    {

        private const char LusidSchedulerArgumentSeparator = '=';
        
        /// <summary>
        /// Entry method to run a financial data extract.
        ///
        /// Expects as arguments a path to a valid FdeRequest json file and the output directory to persist
        /// any responses. 
        /// e.g. lusidfindataex -f "/var/fderequests/fde_req_2020_08_14.json" -o "/var/outputdata/2020_08_14" 
        ///
        /// TODO support e.g. lusidfindataex -j "{req : ""...}" -o "/var/outputdata/2020_08_14" which takes in
        /// TODO all request parameters and constructs vendor requests file. 
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException($"Invalid arguments {string.Join(", ", args)}. Arguments" +
                                            $" must be of format \"lusidfindataex -t <request_source> -f <request_file_path> -o <output_dir>\"");
            }

            string requestSource = ParseArgumentVariable(args[0]);
            string requestPath = ParseArgumentVariable(args[1]);
            string outputDir = ParseArgumentVariable(args[2]);
            Console.WriteLine($"running FinDataEx for request source={requestSource}, request={requestPath}, output directory={outputDir}.");
            
            FdeRequestBuilder fdeRequestBuilder = new FdeRequestBuilder();
            VendorExtractorBuilder vendorExtractorBuilder = new VendorExtractorBuilder();
            FdeResponseProcessorBuilder fdeResponseProcessorBuilder = new FdeResponseProcessorBuilder(outputDir);
            FinDataEx finDataEx = new FinDataEx(fdeRequestBuilder, vendorExtractorBuilder, fdeResponseProcessorBuilder);
            finDataEx.Process(requestSource, requestPath);
        }
        
        private static string ParseArgumentVariable(string argument){
            if(argument.Contains(LusidSchedulerArgumentSeparator))
            {
                return argument.Split(LusidSchedulerArgumentSeparator)[1];
            }
            return argument;
        }
    }
    
    
}