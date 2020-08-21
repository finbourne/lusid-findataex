namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpRequest : IVendorRequest
    {
        public string BbgFtpUrl { get; }
        public string BbqUser { get; }
        public string BbqPass { get; }
        public string RequestFileUrl { get; }
        
        public DlRequestType DlRequestType { get; }

        public DlFtpRequest(string bbgFtpUrl, string bbqUser, string bbqPass, string requestFileUrl, DlRequestType dlRequestType)
        {
            BbgFtpUrl = bbgFtpUrl;
            BbqUser = bbqUser;
            BbqPass = bbqPass;
            RequestFileUrl = requestFileUrl;
            DlRequestType = dlRequestType;
        }
    }
}