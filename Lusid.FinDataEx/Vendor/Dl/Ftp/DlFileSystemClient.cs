using System;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    ///
    /// Simple DL client that loads a pre-existing DL response from file.
    ///
    /// Not intended for production use.
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
            string requestFilePath = submitGetDataRequest.RequestFilePath;
            string responseFilePath = requestFilePath.Replace(".req", ".out.txt");
            return _dlFtpResponseBuilder.CreateFromFile(submitGetDataRequest.DlRequestType, responseFilePath);
        }

        
    }
}