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
            List<List<string>> finData = vendorResponse.GetFinData();
            List<string> toWrite = finData.ConvertAll(
                e => string.Join("|", e));
            File.WriteAllLines(
                _outputDir + Path.DirectorySeparatorChar + fdeRequest.Uid + ".csv",
                toWrite);
        }
    }
}