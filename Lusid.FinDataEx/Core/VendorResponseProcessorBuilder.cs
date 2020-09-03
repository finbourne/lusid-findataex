using System.IO;
using Lusid.Drive.Sdk.Utilities;

namespace Lusid.FinDataEx.Core
{
    /// <summary>
    ///  Builder for all supported IVendorResponseProcessor implementations that process vendor
    ///  responses and output them to a given directory
    ///
    ///  TODO Support more general output case then just persisting to file (e.g. ftp)  
    /// </summary>
    public class FdeResponseProcessorBuilder
    {
        private const string LptResponseProcessor = "lusidtools";
        private const string LusidDriveResponseProcessor = "lusiddrive";

        
        public string OutputDir { get; }

        public FdeResponseProcessorBuilder(string outputDir)
        {
            this.OutputDir = outputDir;
        }

        /// <summary>
        ///  Create a IVendorResponseProcessor based on the specific request
        /// </summary>
        /// <param name="fdeRequest"> request for vendor data</param>
        /// <returns>a IVendorResponseProcessor based on the specific request</returns>
        /// <exception cref="InvalidDataException"> if the requested vendor processor is not supported</exception>
        public IVendorResponseProcessor CreateFdeResponseProcessor(FdeRequest fdeRequest)
        {
            return fdeRequest.Output switch
            {
                LptResponseProcessor => new LptVendorResponseProcessor(OutputDir),
                LusidDriveResponseProcessor => new LusidDriveVendorResponseProcessor(OutputDir, LusidApiFactoryBuilder.Build("secrets.json")),
                _ => throw new InvalidDataException($"No response processor found for {fdeRequest.Output}")
            };
        }
    }
}