using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor
{
    /// <summary>
    ///  External request call to any supported connectors (e.g. FTP, SOAP WebServices, etc...))
    /// </summary>
    public interface IVendorResponse
    {

        Dictionary<string,List<List<string>>> GetFinData();
        
    }
}