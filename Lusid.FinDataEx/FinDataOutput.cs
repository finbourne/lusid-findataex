using System.Collections.Generic;

namespace Lusid.FinDataEx
{
    public class FinDataOutput
    {
        public string Id { get; }
        public List<string> Header { get; }
        public List<Dictionary<string,string>> Records { get; }

        public FinDataOutput(string id, List<string> header, List<Dictionary<string, string>> records)
        {
            Id = id;
            Header = header;
            Records = records;
        }
    }
}