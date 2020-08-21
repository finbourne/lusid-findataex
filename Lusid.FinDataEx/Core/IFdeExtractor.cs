using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    /// <summary>
    /// Extracts financial data from a supported vendor and processes
    /// it into a standard response.
    /// 
    /// </summary>
    public interface IFdeExtractor
    { 
        /// <summary>
        /// Extract financial data based on parameters provided in a request
        /// </summary>
        /// <param name="request"> details of a given request (e.g. vendor, connection type, etc...)</param>
        /// <returns></returns>
        IVendorResponse Extract(FdeRequest request);
    }
    
}