using System.Collections.Generic;

namespace Lusid.FinDataEx.Core
{
    public class FdeResponse
    {
        // dataframe?? do we force them. Slightly sloppy it being a string 
        public List<List<string>> FinData { get; }

        public FdeResponse(List<List<string>> finData)
        {
            FinData = finData;
        }
    }
}