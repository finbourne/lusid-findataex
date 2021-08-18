using System;
using System.IO;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    ///
    /// Simple DL client that loads a pre-existing DL response from file in local file system
    /// or from lusid drive.
    /// 
    /// NOT intended for production use only for testing purposes
    /// 
    /// </summary>
    public class DlFileSystemClient : IVendorClient<DlFtpRequest, DlFtpResponse>
    {

        // Used for passing in location of mock response files when testing with LUSID drive
        private const string LusidDriveFabricatedUrlPrefix = "lusiddrive://";
        
        private readonly DlFtpResponseBuilder _dlFtpResponseBuilder;

        public DlFileSystemClient(DlFtpResponseBuilder dlFtpResponseBuilder)
        {
            _dlFtpResponseBuilder = dlFtpResponseBuilder;
        }

        /// <summary>
        /// Extracts a preexisting response with the same name and locations as the URL except
        /// with a .out extension.
        /// </summary>
        /// <param name="submitGetDataRequest"></param>
        /// <returns></returns>
        public DlFtpResponse Submit(DlFtpRequest submitGetDataRequest)
        {
            string[] dlResponseEntries = LoadDlResponseEntries(submitGetDataRequest);
            return _dlFtpResponseBuilder.CreateFromFile(submitGetDataRequest.DlRequestType, dlResponseEntries);
        }

        private string[] LoadDlResponseEntries(DlFtpRequest dlFtpRequest)
        {
            Console.WriteLine($"Processing DL responses for DLFtpRequest from url={dlFtpRequest.FtpUrl}");
            if (dlFtpRequest.FtpUrl.StartsWith(LusidDriveFabricatedUrlPrefix))
            {
                return LoadDlRequestEntriesFromLusidDrive(dlFtpRequest.FtpUrl.Split("//")[1]);
            }
            else
            {
                return LoadDlRequestEntriesFromFile(dlFtpRequest.RequestFilePath);
            }
        }
        
        private string[] LoadDlRequestEntriesFromFile(string requestFilePath)
        {
            string responseFilePath = requestFilePath.Replace(".req", ".out.txt");
            return File.ReadAllLines(responseFilePath);
        }
        
        private string[] LoadDlRequestEntriesFromLusidDrive(string responseLusidFileId)
        {
            var factory = LusidApiFactoryBuilder.Build("secrets.json");
            var filesApi = factory.Api<IFilesApi>();
            var responseDlFileStream = filesApi.DownloadFile(responseLusidFileId);
            var responseDlFileData = new byte[responseDlFileStream.Length];
            responseDlFileStream.Read(responseDlFileData);
            var dlResponse = Encoding.UTF8.GetString(responseDlFileData);
            return dlResponse.Split("\r\n");
        }

        
    }
}