using System.Collections.Generic;

namespace Lusid.FinDataEx.Output
{
    public interface IFinDataOutputWriter
    {

        public const char BbgDlDelimitter = '|';
        public const string BbgDlOutputFileFormat = ".csv";

        WriteResult Write(List<FinDataOutput> finDataOutputs);

    }
}