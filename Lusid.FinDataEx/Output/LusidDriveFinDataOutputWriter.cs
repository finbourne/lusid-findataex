using System;
using System.Collections.Generic;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;

namespace Lusid.FinDataEx.Output
{
    public class LusidDriveFinDataOutputWriter : LocalFilesystemFinDataOutputWriter
    {
        private const string OutputFileEntrySeparator = "\n";

        private readonly IFilesApi _filesApi;

        public LusidDriveFinDataOutputWriter(string outputDir, ILusidApiFactory factory) : base(outputDir)
        {
            _filesApi = factory.Api<IFilesApi>();
        }

        protected override string WriteToFile(string outputFilename, List<string> finDataRecords)
        {
            Console.WriteLine($"Attempting to write to LUSID drive filename={outputFilename}, folder={OutputDir}.");
            // convert records to byte array for upload into LUSID drive
            var finDataEntriesStr = string.Join(OutputFileEntrySeparator, finDataRecords);
            var finDataEntriesBytes = Encoding.UTF8.GetBytes(finDataEntriesStr);
            
            // upload to drive
            var upload = _filesApi.CreateFile(outputFilename, OutputDir, finDataEntriesBytes.Length, finDataEntriesBytes);
            Console.WriteLine($"Completed write to LUSID drive for filename={outputFilename}, " +
                              $"folder={OutputDir}. Output file id={upload.Id} with size={upload.Size}");
            
            // return file id which will be required to reference file going forward
            return upload.Id;
        }
    }
}