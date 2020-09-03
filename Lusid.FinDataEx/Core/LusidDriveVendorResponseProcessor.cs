using System;
using System.Collections.Generic;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    /// <summary>
    ///
    /// Response processor that persists IVendorResponses to LUSID drive as csv files.
    ///
    /// TODO Test version enforces "|" delimitted files with .csv extension. Output configuration
    /// TODO to be added to the FdeRequest under Output arguments.
    /// </summary>
    public class LusidDriveVendorResponseProcessor : IVendorResponseProcessor
    {

        private const char OutputFileDelimiter = '|';
        private const string OutputFileEntrySeparator = "\n";

        private readonly string _lusidDriveOutputFolder;
        private readonly IFilesApi _filesApi;

        public LusidDriveVendorResponseProcessor(string lusidDriveOutputFolder, ILusidApiFactory factory)
        {
            _lusidDriveOutputFolder = lusidDriveOutputFolder;
            _filesApi = factory.Api<IFilesApi>();
        }

        
        public ProcessResponseResult ProcessResponse(FdeRequest fdeRequest, IVendorResponse vendorResponse)
        {
            Dictionary<string, List<List<string>>> finData = vendorResponse.GetFinData();
            Dictionary<string, object> processResponseProperties = new Dictionary<string, object>();
            
            // write each set of data in the vendor response (e.g set fo corp action 1, another for corp action 2)
            // to lusid drive
            foreach (var finDataEntrySet in finData)
            {
                try
                {
                    // convert to byte array representation of all entries in a structured format
                    // TODO these configs to move into FdeRequest to allow for customisability based on request
                    List<string> finDataEntries = finDataEntrySet.Value.ConvertAll(
                        e => string.Join(OutputFileDelimiter, e));
                    string finDataEntriesStr = string.Join(OutputFileEntrySeparator, finDataEntries);
                    byte[] finDataEntriesBytes = Encoding.ASCII.GetBytes(finDataEntriesStr);
                    
                    // create output file name based on request and the specific set of this response (e.g. corpAction1)
                    string outputFilename = fdeRequest.Uid + "_" + finDataEntrySet.Key + ".csv";
                    
                    // upload and write data to LUSID drive in given output folder.
                    Console.WriteLine($"Attempting to write to LUSID drive filename={outputFilename}, folder={_lusidDriveOutputFolder}.");
                    var upload = _filesApi.CreateFile(outputFilename, _lusidDriveOutputFolder, finDataEntriesBytes.Length, finDataEntriesBytes);

                    // record as successful upload to lusid drive 
                    processResponseProperties.Add(finDataEntrySet.Key, 
                        LusidDriveUploadResults.CreateSuccessUploadResults(fdeRequest.Uid, finDataEntrySet.Key, _lusidDriveOutputFolder, outputFilename, upload.Id, upload.Size));
                    
                }
                catch (Exception e)
                {
                    // record as failed upload to lusid drive 
                    processResponseProperties.Add(finDataEntrySet.Key, 
                        LusidDriveUploadResults.CreateFailedUploadResults(fdeRequest.Uid, finDataEntrySet.Key));

                }
            }

            // collect and process the results of the upload to return to the caller
            ProcessResponseResultStatus status = getOverallStatus(processResponseProperties);
            string message = getResponseProcessMessage(fdeRequest, processResponseProperties);
            return new ProcessResponseResult(status, message, processResponseProperties);

        }
        
        private ProcessResponseResultStatus getOverallStatus(Dictionary<string, object> processResponseProperties)
        {
            ProcessResponseResultStatus status = ProcessResponseResultStatus.Ok;
            foreach (LusidDriveUploadResults driveUploadResponseProperties in processResponseProperties.Values)
            {
                if (driveUploadResponseProperties.LuisdDriveUploadStatus.Equals(ProcessResponseResultStatus.Fail))
                {
                    return ProcessResponseResultStatus.Fail;
                }
            }
            return status;
        }

        private string getResponseProcessMessage(
            FdeRequest fdeRequest,
            Dictionary<string, object> processResponseProperties)
        {
            List<string> msg = new List<string>
            {
                $"Preparing csv from vendor data responses for fde request={fdeRequest.Uid}"
            };
            foreach (LusidDriveUploadResults driveUploadResponseProperties in processResponseProperties.Values)
            {
                msg.Add(driveUploadResponseProperties.ToString());
            }
            return string.Join(Environment.NewLine, msg);
        }
    }

    /// <summary>
    ///
    /// Contains details of the upload request to LUSID drive.
    /// 
    /// </summary>
    public class LusidDriveUploadResults
    {
        public readonly string FdeRequestId;
        public readonly string FinDataKey;
        public readonly ProcessResponseResultStatus LuisdDriveUploadStatus;
        public readonly string LusidDriveFolder;
        public readonly string LusidDriveFileName;
        public readonly string LusidDriveFileId;
        public readonly Nullable<int> LusidDriveFileSize;

        private LusidDriveUploadResults(string fdeRequestId, string finDataKey, ProcessResponseResultStatus luisdDriveUploadStatus, string lusidDriveFolder, string lusidDriveFileName, string lusidDriveFileId, Nullable<int> lusidDriveFileSize)
        {
            FdeRequestId = fdeRequestId;
            FinDataKey = finDataKey;
            LuisdDriveUploadStatus = luisdDriveUploadStatus;
            LusidDriveFolder = lusidDriveFolder;
            LusidDriveFileName = lusidDriveFileName;
            LusidDriveFileId = lusidDriveFileId;
            LusidDriveFileSize = lusidDriveFileSize;
        }

        public static LusidDriveUploadResults CreateSuccessUploadResults(string fdeRequestId, string finDataKey,
            string lusidDriveFolder, string lusidDriveFileName,
            string lusidDriveFileId, Nullable<int> lusidDriveFileSize)
        {
            return new LusidDriveUploadResults(fdeRequestId, finDataKey,
                ProcessResponseResultStatus.Ok, lusidDriveFolder, lusidDriveFileName,
                lusidDriveFileId, lusidDriveFileSize);
        }
        
        public static LusidDriveUploadResults CreateFailedUploadResults(string fdeRequestId, string finDataKey)
        {
            return new LusidDriveUploadResults(fdeRequestId, finDataKey,
                ProcessResponseResultStatus.Fail, null, null,
                null, null);
        }

        public override string ToString()
        {
            return $"{nameof(FdeRequestId)}: {FdeRequestId}, {nameof(FinDataKey)}: {FinDataKey}, {nameof(LuisdDriveUploadStatus)}: {LuisdDriveUploadStatus}, {nameof(LusidDriveFolder)}: {LusidDriveFolder}, {nameof(LusidDriveFileName)}: {LusidDriveFileName}, {nameof(LusidDriveFileId)}: {LusidDriveFileId}, {nameof(LusidDriveFileSize)}: {LusidDriveFileSize}";
        }
    }
    
    
}