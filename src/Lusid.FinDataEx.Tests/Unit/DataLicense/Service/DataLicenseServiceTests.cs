using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Util;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service
{
    [TestFixture]
    public class DataLicenseServiceTests
    {
        private DataLicenseService _dataLicenseService;
        private PerSecurityWS _perSecurityWs;
        private GetDataLicenseCall _getDataLicenseCall;

        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = new PerSecurityWSClient(new BasicHttpBinding(), new EndpointAddress("http://example.test/not/real/endpoint"));
            _getDataLicenseCall = new GetDataLicenseCall(_perSecurityWs);
            _dataLicenseService = new DataLicenseService();
        }

        [Test]
        public void Get_OnAdhocGetDataWithNoIds_ShouldReturnEmptyData()
        {
            var finDataOutput = _dataLicenseService.Get(_getDataLicenseCall, CreateInstruments(new List<string>()), DataLicenseTypes.ProgramTypes.Adhoc, true);
            Assert.True(finDataOutput.IsEmpty());
        }

        [Test]
        public void Get_OnAScheduledGetData_ShouldThrowUnsupportedException()
        {
            var bbgIds = CreateInstruments(new List<string>{"BBG000BPHFS9", "BBG000BVPV84"});
            Assert.Throws<NotSupportedException>(() => _dataLicenseService.Get(_getDataLicenseCall, bbgIds, DataLicenseTypes.ProgramTypes.Scheduled, true));
        }

        private Instruments CreateInstruments(IEnumerable<string> bbgIds)
        {
            var instruments = bbgIds.Select(id => new Instrument()
            {
                id = id,
                type = InstrumentType.BB_GLOBAL,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }
    }
}