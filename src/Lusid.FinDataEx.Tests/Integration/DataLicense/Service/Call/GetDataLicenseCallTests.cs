using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Vendor;
using NUnit.Framework;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.Tests.Unit.DataLicense.Service.Call.GetDataLicenseCallTest;

namespace Lusid.FinDataEx.Tests.Integration.DataLicense.Service.Call
{
    [TestFixture]
    [Ignore("Integration tests are currently unable to run because of licensing issues")]
    [Category("Unsafe")]
    public class GetDataLicenseCallTests
    {
        private GetDataLicenseCall _getDataLicenseCall;

        [SetUp]
        public void SetUp()
        {
            var perSecurityWs = new PerSecurityWsFactory().CreateDefault(PerSecurityWsFactory.BbgDlAddress);
            _getDataLicenseCall = new GetDataLicenseCall(perSecurityWs);
        }

        [Test]
        public void Get_OnValidInstruments_ShouldReturnPrice()
        {
            var testInstruments = CreateTestInstruments();

            var retrieveGetDataResponse = _getDataLicenseCall.Get(testInstruments);
            var instrumentDatas = retrieveGetDataResponse.instrumentDatas;
            var getDataFields = retrieveGetDataResponse.fields;

            Assert.That(retrieveGetDataResponse.statusCode.code, Is.EqualTo(DataLicenseService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(2));
            AssertBbUniqueQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[0], "BBG000BPHFS9");
            AssertIsinQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[1], "US0231351067", "BBG000BVPV84");
        }

        private void AssertBbUniqueQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(bbUid));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_GLOBAL"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }

        private void AssertIsinQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string isin, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(isin));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_GLOBAL"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }
    }
}