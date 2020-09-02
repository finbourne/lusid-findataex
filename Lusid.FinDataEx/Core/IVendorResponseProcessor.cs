using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    public interface IVendorResponseProcessor
    {
        ProcessResponseResult ProcessResponse(FdeRequest fdeRequest, IVendorResponse vendorResponse);
    }
}