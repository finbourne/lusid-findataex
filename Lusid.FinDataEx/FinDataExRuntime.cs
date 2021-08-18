using System;
using System.Linq;
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
        /// e.g. Lusid.FinDataEx.dll "FileSystem" "/var/fderequests/fde_req_2020_08_14.json" "/var/outputdata/2020_08_14"
        /// e.g. Lusid.FinDataEx.dll "LusidDrive" "0eb302a8-6435-4676-9ea4-0930a11b80df" "/lusid_drive_folder/2020_08_14" 
        ///
        /// TODO integrate https://github.com/fclp/fluent-command-line-parser for args. Check first that scheduler
        /// TODO supports passing in only strings and not param=string
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Main(string[] args)
        {
            // ensure correct number of arguments passed in
            if (args.Length != 3)
            {
                throw new ArgumentException($"Invalid arguments {string.Join(", ", args)}. Arguments" +
                                            $" must be of format \"Lusid.FinDataEx.dll -t <request_source> -f <request_file_path> -o <output_dir>\"");
            }

            // parse arguments - scheduler jobs for example pass in a specific format.
            FdeRequestSource requestSource = ParseRequestSourceArgument(args[0]);
            string requestPath = ParseArgumentVariable(args[1]);
            string outputDir = ParseArgumentVariable(args[2]);
            Console.WriteLine($"Running FinDataEx for request source={requestSource}, request={requestPath}, output directory={outputDir}.");
            
            // construct request, the extractor for a specific vendor and the processor of vendor responses
            FdeRequestBuilder fdeRequestBuilder = new FdeRequestBuilder();
            VendorExtractorBuilder vendorExtractorBuilder = new VendorExtractorBuilder();
            FdeResponseProcessorBuilder fdeResponseProcessorBuilder = new FdeResponseProcessorBuilder(outputDir);
            
            // create central workflow processor for findataex
            FinDataEx finDataEx = new FinDataEx(fdeRequestBuilder, vendorExtractorBuilder, fdeResponseProcessorBuilder);
            // TODO should we throw exception on failed (or partially failed) extracts
            ProcessResponseResult processResponseResult =  finDataEx.Process(requestSource, requestPath);

            // log response results
            LogProcessResponseResult(processResponseResult);
        }

        private static FdeRequestSource ParseRequestSourceArgument(string argument)
        {
            var requestSourceArgument = ParseArgumentVariable(argument);
            if (!Enum.TryParse<FdeRequestSource>(requestSourceArgument, true, out var fdeRequestSource))
            {
                throw new ArgumentException($"{requestSourceArgument} is not a supported source of FDE requests");
            };
            return fdeRequestSource;
        }

        /// <summary>
        /// LUSID scheduler jobs currently only pass in argument parameters in the form paramName=paramValue. This method
        /// adds support for that format.
        /// </summary>
        private static string ParseArgumentVariable(string argument)
        {
            return argument.Contains(LusidSchedulerArgumentSeparator) ? argument.Split(LusidSchedulerArgumentSeparator).Last() : argument;
        }

        private static void LogProcessResponseResult(ProcessResponseResult processResponseResult)
        {
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
    }
    
    
}