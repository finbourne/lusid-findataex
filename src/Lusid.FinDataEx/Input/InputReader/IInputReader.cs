using Lusid.FinDataEx.Data;
using Lusid.FinDataEx.Data.CorporateActionRecord;
using Lusid.FinDataEx.Data.DataRecord;
using System;
using System.Collections.Generic;
using static Lusid.FinDataEx.DataLicense.Util.DataLicenseTypes;

namespace Lusid.FinDataEx.Input
{
    /// <summary>
    /// Reader of FinDataInput from an input destination
    /// </summary>
    public interface IInputReader
    {
        public const char CsvDelimiter = ',';

        private static readonly Dictionary<string, CorpActionType> ActionTypeMapping = new Dictionary<string, CorpActionType>
        {
            { "Cash Dividend", CorpActionType.DVD_CASH },
            { "Stock Dividend", CorpActionType.DVD_STOCK },
            { "Stock Split", CorpActionType.STOCK_SPLT }
        };

        /// <summary>
        /// Read financial data retrieved from an
        /// FinDataInputs to an output destination
        /// </summary>
        /// <returns>Financial data to write</returns>
        public DataLicenseOutput Read();

        protected static IRecord ConvertToRecord(Dictionary<string, string> rawRecord, DataLicenseOptions options, string actionTypeKeyString)
        {
            if (options is GetDataOptions)
            {
                return new InstrumentDataRecord(rawRecord);
            }
            else if (options is GetActionsOptions)
            {
                var actionType = ActionTypeMapping[rawRecord[actionTypeKeyString]];
                return actionType switch
                {
                    CorpActionType.DVD_CASH => new CashDividendCorporateActionRecord(rawRecord),
                    _ => throw new NotImplementedException($"No supported Record type for {actionType}"),
                };
            }
            else
            {
                throw new NotImplementedException("No supported request type for the provided options");
            }
        }
    }
}