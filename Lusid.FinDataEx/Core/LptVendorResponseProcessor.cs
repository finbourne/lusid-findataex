using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    public class LptVendorResponseProcessor : IVendorResponseProcessor
    {

        private readonly string _outputDir;

        public LptVendorResponseProcessor(string outputDir)
        {
            this._outputDir = outputDir;
        }

        public void ProcessResponse(FdeRequest fdeRequest, IVendorResponse vendorResponse)
        {
            Dictionary<string, List<List<string>>> finData = vendorResponse.GetFinData();

            foreach (var finDataEntrySet in finData)
            {
                List<string> toWrite = finDataEntrySet.Value.ConvertAll(
                    e => string.Join("|", e));
                File.WriteAllLines(
                    _outputDir + Path.DirectorySeparatorChar + fdeRequest.Uid + "_" + finDataEntrySet.Key + ".csv",
                    toWrite);
            }
            
        }
    }
}