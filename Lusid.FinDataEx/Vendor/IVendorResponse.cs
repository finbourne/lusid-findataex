using System.Collections.Generic;

namespace Lusid.FinDataEx.Vendor
{
    /// <summary>
    ///  External request call to any BBG DL supported connectors (e.g. FTP, SOAP WebServices, etc...))
    /// </summary>
    public interface IVendorResponse
    {

        List<List<string>> GetFinData();
        
        //TODO look at adding output formats e.g. DataFrame

    }
}