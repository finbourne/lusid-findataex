using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor.Dl.Ftp
{
    /// <summary>
    /// Response for financial data request from DL via the FTP flow.
    ///
    /// Contains data retrieved from the FTP DL response file and parsed
    /// into a supported IVendorResponse format.
    /// 
    /// </summary>
    public class DlFtpResponse : IVendorResponse
    {
        /// <summary> financial data returned from DL </summary>
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