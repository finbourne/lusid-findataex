using System;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx
{
    /// <summary>
    /// Standardised container for responses returned from BBG DLWS calls.
    /// 
    /// </summary>
    public class DataLicenseOutput
    {
        /// <summary>Id of the specific BBG DL request for data</summary>
        public string Id { get; }
        
        /// <summary>Headers for requested data from BBG DL</summary>
        public IEnumerable<string> Header { get; }
        
        /// <summary>Financial data for each of the instruments requested from BBG DL</summary>
        public IList<Dictionary<string,string>> Records { get; }

        public DataLicenseOutput(string id, IEnumerable<string> header, IList<Dictionary<string, string>> records)
        {
            Id = id;
            Header = header;
            Records = records;
        }

        public static DataLicenseOutput Empty(string id)
        {
            return new DataLicenseOutput(id, new List<string>(), new List<Dictionary<string, string>>());
        }
        
        public static DataLicenseOutput Empty()
        {
            return new DataLicenseOutput("-", new List<string>(), new List<Dictionary<string, string>>());
        }

        public bool IsEmpty()
        {
            return !Header.Any() && !Records.Any();
        }

        protected bool Equals(DataLicenseOutput other)
        {
            return Id == other.Id && Equals(Header, other.Header) && Equals(Records, other.Records);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataLicenseOutput) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Header, Records);
        }
    }
}