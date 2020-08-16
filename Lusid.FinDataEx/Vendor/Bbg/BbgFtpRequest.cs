namespace Lusid.FinDataEx.Vendor.Bbg.Ftp
{
    public class BbgFtpRequest : IVendorRequest
    {
        public string BbgFtpUrl { get; }
        public string BbqUser { get; }
        public string BbqPass { get; }
        public string RequestFileUrl { get; }

        public BbgFtpRequest(string bbgFtpUrl, string bbqUser, string bbqPass, string requestFileUrl)
        {
            BbgFtpUrl = bbgFtpUrl;
            BbqUser = bbqUser;
            BbqPass = bbqPass;
            RequestFileUrl = requestFileUrl;
        }
    }
}