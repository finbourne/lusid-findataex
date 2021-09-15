using Lusid.FinDataEx.Data;
using Lusid.FinDataEx.Data.CorporateActionRecord;
using Lusid.FinDataEx.Data.DataRecord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx
{
    /// <summary>
    /// Standardised container for responses returned from BBG DLWS calls.
    /// </summary>
    public class DataLicenseOutput
    {
        /// <summary>Id of the specific BBG DL request for data</summary>
        public string Id { get; }

        /// <summary>Headers for requested data from BBG DL</summary>
        //public IEnumerable<string> Header { get; }

        /// <summary>Financial data for each of the instruments requested from BBG DL</summary>
        public IList<IDataRecord> DataRecords { get; }

        /// <summary>Corporate Actions for each of the instruments requested from BBG DL</summary>
        public IList<ICorporateActionRecord> CorporateActionRecords { get; }

        public DataLicenseOutput(string id, IList<IRecord> records)
        {
            Id = id;
            //Header = header;
            DataRecords = records.Where(r => r is IDataRecord).Cast<IDataRecord>().ToList();
            CorporateActionRecords = records.Where(r => r is ICorporateActionRecord).Cast<ICorporateActionRecord>().ToList();
        }

        public static DataLicenseOutput Empty(string id)
        {
            return new DataLicenseOutput(id, new List<IRecord>());
        }

        public static DataLicenseOutput Empty()
        {
            return Empty("-");
        }

        public bool IsEmpty()
        {
            return //!Header.Any()
                true
                && !DataRecords.Any()
                && !CorporateActionRecords.Any();
        }

        private bool Equals(DataLicenseOutput other)
        {
            return Equals(Id, other.Id)
                //&& Equals(Header, other.Header)
                && Equals(DataRecords, other.DataRecords)
                && Equals(CorporateActionRecords, other.CorporateActionRecords);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataLicenseOutput) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, /*Header,*/ DataRecords, CorporateActionRecords);
        }
    }
}