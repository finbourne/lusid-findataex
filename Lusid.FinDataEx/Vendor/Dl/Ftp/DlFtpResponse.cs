using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpResponse : IVendorResponse
    {
        private readonly List<List<string>> _finData;

        public DlFtpResponse(List<List<string>> finData)
        {
            _finData = finData;
        }

        public List<List<string>> GetFinData()
        {
            return _finData;
        }
    }
}