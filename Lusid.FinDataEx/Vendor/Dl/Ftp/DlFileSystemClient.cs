using System;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    ///
    /// A client that extracts a preexisting response for fin data from BBG.
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
        ///  Extracts a preexisting response with the same name and locations as the URL except
        /// with a .out extension.
        /// </summary>
        /// <param name="submitGetDataRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DlFtpResponse Submit(DlFtpRequest submitGetDataRequest)
        {
            string requestFileUrl = submitGetDataRequest.RequestFileUrl;
            string responseFileUrl = requestFileUrl.Replace(".req", ".out.txt");
            return _dlFtpResponseBuilder.CreateFromFile(submitGetDataRequest.DlRequestType, responseFileUrl);
        }

        
    }
}