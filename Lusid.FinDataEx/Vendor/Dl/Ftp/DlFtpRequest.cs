namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpRequest : IVendorRequest
    {
        public string FtpUrl { get; }
        public string User { get; }
        public string Password { get; }
        public string RequestFileUrl { get; }
        
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