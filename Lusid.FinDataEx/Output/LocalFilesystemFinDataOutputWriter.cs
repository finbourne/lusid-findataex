using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lusid.FinDataEx.Output.IFinDataOutputWriter;

namespace Lusid.FinDataEx.Output
{
    /// <summary>
    ///  Writes FinDataOutput to local file system.
    /// </summary>
    public class LocalFilesystemFinDataOutputWriter : IFinDataOutputWriter
    {

        private readonly string _outputDir;

        public LocalFilesystemFinDataOutputWriter(string outputDir)
        {
            _outputDir = outputDir;
        }
        
        public WriteResult Write(IEnumerable<FinDataOutput> finDataOutputs)
        {
         
            var responseOkResults = new List<string>();
            var responseFailResults = new List<string>();
            foreach (var finDataOutput in finDataOutputs)
            {
                // prepare fin data as strings to be written to file
                var headers = string.Join(BbgDlDelimiter, finDataOutput.Header);
                var finDataRecords = new List<string>{headers};
                finDataRecords.AddRange(
                finDataOutput.Records.Select(dR =>
                        {
                            List<string> record = new List<string>();
                            foreach (var header in finDataOutput.Header)
                            {
                                dR.TryGetValue(header, out string recordEntry);
                                record.Add(recordEntry);
                            }

                            return string.Join(BbgDlDelimiter, record);
                        }).ToList()
                );
                
                var outputPath = _outputDir + Path.DirectorySeparatorChar + finDataOutput.Id + BbgDlOutputFileFormat;
                try
                {
                    File.WriteAllLines(outputPath, finDataRecords);
                    responseOkResults.Add($"SUCCESS : Completed write of {finDataOutput.Id} to {outputPath}");
                }
                catch (Exception e)
                {
                    responseFailResults.Add($"FAILURE : Did not write {finDataOutput.Id} to {outputPath} due to an exception. Cause of failure: {e}");
                }
            }
            
            // Prepare the result status to return
            var status = (responseFailResults.Any())
                ? WriteResultStatus.Fail
                : WriteResultStatus.Ok;
            var responseResults = new List<string>();
            responseResults.AddRange(responseFailResults);
            responseResults.AddRange(responseOkResults);

            return new WriteResult(status, string.Join(Environment.NewLine, responseResults));
        }

    }
}