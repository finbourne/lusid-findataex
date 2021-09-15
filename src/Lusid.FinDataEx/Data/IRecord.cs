using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Data
{
    public interface IRecord
    {
        public Dictionary<string, string> RawData { get; }

        public List<string> Headers => RawData.Keys.ToList();
    }
}