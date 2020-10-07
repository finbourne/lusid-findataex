using System.Collections.Generic;

namespace Lusid.FinDataEx
{
    public class FinDataOutput
    {
        private List<string> Header { get; }
        private List<List<string>> Records { get; }

        public FinDataOutput(List<string> header, List<List<string>> records)
        {
            this.Header = header;
            this.Records = records;
        }
    }
}