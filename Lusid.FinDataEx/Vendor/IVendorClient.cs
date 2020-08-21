using Lusid.FinDataEx.Vendor.Dl;

namespace Lusid.FinDataEx.Vendor
{
    /// <summary>
    ///  Client to submit requests for FinData to.
    ///
    ///  Impls to start FTP/SoapWebService/Mock
    /// </summary>
    public interface IVendorClient<T,V> 
        where T : IVendorRequest
        where V : IVendorResponse 
    {
        /// <summary>
        ///
        /// Submit a BBG DL request to retrieve financial data from BBG
        /// 
        /// </summary>
        /// <param name="submitGetDataRequest"></param>
        /// <returns></returns>
        V Submit(T submitGetDataRequest);
    }
}