namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    /// Vendor request for financial data using the DL vendor via the FTP flow.
    /// </summary>
    public class DlFtpRequest : IVendorRequest
    {
        /// <summary> url of DL ftp server/// </summary>
        public string FtpUrl { get; }
        
        /// <summary> user for DL ftp server/// </summary>
        public string User { get; }
        /// <summary> password for DL ftp server/// </summary>
        public string Password { get; }
        /// <summary> the location of the specific DL request file/// </summary>
        public string RequestFileUrl { get; }
        /// <summary> type of financial data requesting from DL/// </summary>
        public DlRequestType DlRequestType { get; }

        public DlFtpRequest(string ftpUrl, string user, string password, string requestFileUrl, DlRequestType dlRequestType)
        {
            FtpUrl = ftpUrl;
            User = user;
            Password = password;
            RequestFileUrl = requestFileUrl;
            DlRequestType = dlRequestType;
        }
    }
}