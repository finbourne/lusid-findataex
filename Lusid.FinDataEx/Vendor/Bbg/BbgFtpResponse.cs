namespace Lusid.FinDataEx.Vendor.Bbg.Ftp
{
    public class BbgFtpResponse : IVendorResponse
    {
        public string ResponseFileUrl { get; }

        public BbgFtpResponse(string responseFileUrl)
        {
            ResponseFileUrl = responseFileUrl;
        }
    }
}