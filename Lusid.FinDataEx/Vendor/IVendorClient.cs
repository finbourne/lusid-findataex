using Lusid.FinDataEx.Vendor.Bbg;

namespace Lusid.FinDataEx.Vendor
{
    /// <summary>
    ///  Connects to BBG DL and returns financial data synchronoulsy
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