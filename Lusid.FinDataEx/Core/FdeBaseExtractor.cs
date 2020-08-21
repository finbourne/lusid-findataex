using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    /// <summary>
    /// Base implementation of a financial data extractor
    /// 
    /// </summary>
    /// <typeparam name="TReq"> a vendor specific request</typeparam>
    /// <typeparam name="TResp">a vendor specific response</typeparam>
    public abstract class FdeBaseExtractor<TReq,TResp> : IFdeExtractor 
        where TReq : IVendorRequest
        where TResp : IVendorResponse 
    {
        /// <summary> client to connect to the vendor and retrieve financial data </summary>
        private IVendorClient<TReq, TResp> VendorClient { get; }

        protected FdeBaseExtractor(IVendorClient<TReq, TResp> vendorClient)
        {
            VendorClient = vendorClient;
        }

        /// <summary>
        /// Extracts financial data from a vendor based on a standard FdeRequest.
        /// A base workflow implementation that builds a vendor specific request that
        /// is submitted to the vendor client.
        /// 
        /// </summary>
        /// <param name="request">a standard FdeRequest</param>
        /// <returns>a vendor specific response</returns>
        public IVendorResponse Extract(FdeRequest request)
        {
            TReq vendorRequest = ToVendorRequest(request);
            TResp vendorResponse =  VendorClient.Submit(vendorRequest);
            return vendorResponse;
        }

        /// <summary>
        ///  Build a specific vendor request based on a standard FdeRequest
        /// </summary>
        /// <param name="request">a standard FdeRequest</param>
        /// <returns>a vendor specific request</returns>
        public abstract TReq ToVendorRequest(FdeRequest request);
        

    }
}