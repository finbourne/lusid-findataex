using System.Diagnostics;

namespace Lusid.FinDataEx.Core
{
    public interface IFdeExtractor
    { 
        FdeResponse Extract(FdeRequest request);
    }
    
}