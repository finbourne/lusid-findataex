using Lusid.FinDataEx.Vendor.Bbg.Ftp;

namespace Lusid.FinDataEx.Vendor.Bbg
{
    /// <summary>
    ///
    /// A client that extracts a preexisting response for fin data from BBG.
    ///
    /// Not intended for production use.
    /// 
    /// </summary>
    public class BbgDlFileSystemClient : IVendorClient<BbgFtpRequest, BbgFtpResponse>
    {
        /// <summary>
        ///  Extracts a preexisting response with the same name and locations as the URL except
        /// with a .out extension.
        /// </summary>
        /// <param name="submitGetDataRequest"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BbgFtpResponse Submit(BbgFtpRequest submitGetDataRequest)
        {
            string requestFileUrl = submitGetDataRequest.RequestFileUrl;
            string responseFileUrl = requestFileUrl.Replace(".req", ".out");
            return new BbgFtpResponse(responseFileUrl);
        }
    }
}