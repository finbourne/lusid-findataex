using Lusid.FinDataEx.Vendor;

namespace Lusid.FinDataEx.Core
{
    public interface IFdeExtractor
    { 
        IVendorResponse Extract(FdeRequest request);
    }
    
}