using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor
{
    /// <summary>
    ///  Response containing the financial data from a specific vendor
    /// </summary>
    public interface IVendorResponse
    {

        Dictionary<string,List<List<string>>> GetFinData();
        
    }
}