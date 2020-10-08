using System.Collections.Generic;

namespace Lusid.FinDataEx
{
    public class FinDataOutput
    {
        public List<string> Header { get; }
        public List<Dictionary<string,string>> Records { get; }

        public FinDataOutput(List<string> header, List<Dictionary<string,string>> records)
        {
            this.Header = header;
            this.Records = records;
        }
    }
}