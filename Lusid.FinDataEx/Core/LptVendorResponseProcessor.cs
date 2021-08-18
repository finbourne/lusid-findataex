using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public ProcessResponseResult ProcessResponse(FdeRequest fdeRequest, IVendorResponse vendorResponse)
        {
            Dictionary<string, List<List<string>>> finData = vendorResponse.GetFinData();

            List<string> responseOkResults = new List<string>();
            List<string> responseFailResults = new List<string>();
            foreach (var finDataEntrySet in finData)
            {
                List<string> toWrite = finDataEntrySet.Value.ConvertAll(
                    e => string.Join("|", e));
                string outputPath = _outputDir + Path.DirectorySeparatorChar + fdeRequest.Uid + "_" +
                                    finDataEntrySet.Key + ".csv";
                try
                {
                    File.WriteAllLines(
                        outputPath,
                        toWrite);
                    responseOkResults.Add($"Processed vendor response for fde request={fdeRequest.Uid} and key={finDataEntrySet.Key} and saved to {outputPath}");
                }
                catch (Exception e)
                {
                    responseFailResults.Add($"Failed to process vendor response for fde request={fdeRequest.Uid} and key={finDataEntrySet.Key}. Cause of failure: {e}");
                }
            }

            ProcessResponseResultStatus status = (responseFailResults.Any())
                ? ProcessResponseResultStatus.Fail
                : ProcessResponseResultStatus.Ok;
            List<string> responseResults = new List<string>
            {
                $"Preparing csv outputs from vendor data responses for fde request={fdeRequest.Uid}"
            };
            responseResults.AddRange(responseFailResults);
            responseResults.AddRange(responseOkResults);

            return new ProcessResponseResult(status, string.Join(Environment.NewLine, responseResults));
        }
    }
}