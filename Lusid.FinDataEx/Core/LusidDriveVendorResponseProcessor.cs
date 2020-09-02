using System;
using System.Collections.Generic;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    public class LusidDriveVendorResponseProcessor : IVendorResponseProcessor
    {

        private readonly string _luisdDriveOutputFolder;
        private readonly IFilesApi _filesApi;

        public LusidDriveVendorResponseProcessor(string luisdDriveOutputFolder, ILusidApiFactory factory)
        {
            _luisdDriveOutputFolder = luisdDriveOutputFolder;
            _filesApi = factory.Api<IFilesApi>();
        }

        public ProcessResponseResult ProcessResponse(FdeRequest fdeRequest, IVendorResponse vendorResponse)
        {
            Dictionary<string, List<List<string>>> finData = vendorResponse.GetFinData();
            Dictionary<string, object> processResponseProperties = new Dictionary<string, object>();
            foreach (var finDataEntrySet in finData)
            {
                try
                {
                    List<string> entries = finDataEntrySet.Value.ConvertAll(
                        e => string.Join("|", e));
                    string toWrite = string.Join("\n", entries);
                    byte[] data = Encoding.ASCII.GetBytes(toWrite);
                    
                    string outputFilename = fdeRequest.Uid + "_" + finDataEntrySet.Key + ".csv";
                    
                    //Upload a file
                    var upload = _filesApi.CreateFile(outputFilename, _luisdDriveOutputFolder, data.Length, data);

                    processResponseProperties.Add(finDataEntrySet.Key, 
                        LusidDriveUploadResults.CreateSuccessUploadResults(fdeRequest.Uid, finDataEntrySet.Key, _luisdDriveOutputFolder, outputFilename, upload.Id, upload.Size));
                    
                }
                catch (Exception e)
                {
                    processResponseProperties.Add(finDataEntrySet.Key, 
                        LusidDriveUploadResults.CreateFailedUploadResults(fdeRequest.Uid, finDataEntrySet.Key));

                }
            }

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
        
    }
    
    
}