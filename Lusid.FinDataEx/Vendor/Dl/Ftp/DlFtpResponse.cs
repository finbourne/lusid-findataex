using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    public class DlFtpResponse : IVendorResponse
    {
        private readonly Dictionary<string,List<List<string>>> _finData;

        public DlFtpResponse(Dictionary<string,List<List<string>>> finData)
        {
            _finData = finData;
        }

        public Dictionary<string,List<List<string>>> GetFinData()
        {
            return _finData;
        }
    }
}