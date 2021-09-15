using System.Collections.Generic;

namespace Lusid.FinDataEx.Data.DataRecord
{
    public class InstrumentDataRecord : IDataRecord
    {
        private readonly Dictionary<string, string> _rawData;

        public Dictionary<string, string> RawData { get => _rawData; }

        public InstrumentDataRecord(Dictionary<string, string> input)
        {
            _rawData = input;
        }
    }
}