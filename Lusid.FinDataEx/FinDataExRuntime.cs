﻿using System;
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
            finDataEx.Process(requestJsonFilePath);
        }
    }
}