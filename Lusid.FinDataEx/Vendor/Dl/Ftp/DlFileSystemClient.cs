using System;
using System.IO;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    ///
    /// Simple DL client that loads a pre-existing DL response from file.
    ///
    /// Additionally supports loading from lusid drive.
    ///
    /// NOT intended for production use only for testing purposes
    /// 
    /// </summary>
    public class DlFileSystemClient : IVendorClient<DlFtpRequest, DlFtpResponse>
    {

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
            if (dlFtpRequest.FtpUrl.StartsWith("lusiddrive://"))
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
            ILusidApiFactory factory = LusidApiFactoryBuilder.Build("secrets.json");
            IFilesApi filesApi = factory.Api<IFilesApi>();
            Stream responseDlFileStream = filesApi.DownloadFile(responseLusidFileId);
            byte[] responseDlFileData = new byte[responseDlFileStream.Length];
            responseDlFileStream.Read(responseDlFileData);
            string dlResponse = Encoding.UTF8.GetString(responseDlFileData);
            return dlResponse.Split("\r\n");
        }

        
    }
}