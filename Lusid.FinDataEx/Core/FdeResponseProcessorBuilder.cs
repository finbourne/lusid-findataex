using System.IO;

namespace Lusid.FinDataEx.Core
{
    public class FdeResponseProcessorBuilder
    {
        private const string LPT_RESPONSE_PROCESSOR = "lusidtools";
        
        public string outputDir { get; }

        public FdeResponseProcessorBuilder(string outputDir)
        {
            this.outputDir = outputDir;
        }

        public IVendorResponseProcessor CreateFdeResponseProcessor(FdeRequest fdeRequest)
        {
            switch (fdeRequest.Output)
            {
                case LPT_RESPONSE_PROCESSOR:
                    return new LptVendorResponseProcessor(outputDir);
                    break;
            }

            throw new InvalidDataException($"No response processor found for {fdeRequest.Output}");
        }
    }
}