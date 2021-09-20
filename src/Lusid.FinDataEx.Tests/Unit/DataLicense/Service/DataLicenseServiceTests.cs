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
        public void AdhocGetDataWithNoIdsShouldReturnEmptyData()
        {
            var mockDataLicenseCall = Mock.Of<IDataLicenseCall<PerSecurityResponse>>();
            Mock.Get(mockDataLicenseCall).Verify(mock => mock.Get(It.IsAny<Instruments>()), Times.Never());

            var finDataOutput = _dataLicenseService.Get(_getDataLicenseCall, CreateInstruments(new List<string> { }), DataLicenseTypes.ProgramTypes.Adhoc, true);
            Assert.That(finDataOutput.requestId, Is.Null);
            Assert.That(finDataOutput.statusCode, Is.Null);
        }

        [Test]
        public void ScheduledDataShouldThrowUnsupportedException()
        {
            var bbgIds = CreateInstruments(new List<string>{"BBG000BPHFS9", "BBG000BVPV84"});
            Assert.Throws<NotSupportedException>(() => _dataLicenseService.Get(_getDataLicenseCall, bbgIds, DataLicenseTypes.ProgramTypes.Scheduled, true));
        }

        [Test]
        public void UnsafeRequestsAreNotRun()
        {
            var mockDataLicenseCall = Mock.Of<IDataLicenseCall<PerSecurityResponse>>();
            Mock.Get(mockDataLicenseCall).Verify(mock => mock.Get(It.IsAny<Instruments>()), Times.Never());

            var bbgIds = CreateInstruments(new List<string> { "BBG000BPHFS9", "BBG000BVPV84" });
            var finDataOutput = _dataLicenseService.Get(_getDataLicenseCall, bbgIds, DataLicenseTypes.ProgramTypes.Adhoc, false);
            Assert.That(finDataOutput.requestId, Is.Null);
            Assert.That(finDataOutput.statusCode, Is.Null);
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