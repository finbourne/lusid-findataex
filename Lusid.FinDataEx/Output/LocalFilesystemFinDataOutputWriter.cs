using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Lusid.FinDataEx.Output.IFinDataOutputWriter;

namespace Lusid.FinDataEx.Output
{
    public class LocalFilesystemFinDataOutputWriter : IFinDataOutputWriter
    {

        private readonly string _outputDir;

        public LocalFilesystemFinDataOutputWriter(string outputDir)
        {
            _outputDir = outputDir;
        }
        
        public WriteResult Write(List<FinDataOutput> finDataOutputs)
        {
         
            List<string> responseOkResults = new List<string>();
            List<string> responseFailResults = new List<string>();
            foreach (var finDataOutput in finDataOutputs)
            {
                // prepare fin data as strings to be written to file
                string headers = string.Join(BbgDlDelimitter, finDataOutput.Header);
                List<string> finDataRecords = new List<string>{headers};
                finDataRecords.AddRange(
                finDataOutput.Records.Select(dR =>
                        {
                            List<string> record = new List<string>();
                            foreach (var header in finDataOutput.Header)
                            {
                                dR.TryGetValue(header, out string recordEntry);
                                record.Add(recordEntry);
                            }

                            return string.Join(BbgDlDelimitter, record);
                        }).ToList()
                );
                
                string outputPath = _outputDir + Path.DirectorySeparatorChar + finDataOutput.Id + BbgDlOutputFileFormat;
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
            WriteResultStatus status = (responseFailResults.Any())
                ? WriteResultStatus.Fail
                : WriteResultStatus.Ok;
            List<string> responseResults = new List<string>();
            responseResults.AddRange(responseFailResults);
            responseResults.AddRange(responseOkResults);

            return new WriteResult(status, string.Join(Environment.NewLine, responseResults));
        }

    }
}