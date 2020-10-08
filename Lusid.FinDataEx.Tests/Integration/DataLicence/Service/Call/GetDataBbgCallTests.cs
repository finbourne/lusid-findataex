using System;
using Lusid.FinDataEx.DataLicense;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Vendor;
using NUnit.Framework;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.Tests.DataLicence.Service.Call.GetDataBbgCallTest;

namespace Lusid.FinDataEx.Tests.DataLicence.Service.Call
{
    [TestFixture]
    public class GetDataBbgCallTests
    {

        private GetDataBbgCall _getDataBbgCall;

        [SetUp]
        public void SetUp()
        {
            PerSecurityWS perSecurityWs = new PerSecurityWSFactory().CreateDefault(PerSecurityWSFactory.BbgDlAddress);
            _getDataBbgCall = new GetDataBbgCall(perSecurityWs);
        }

        [Test]
        public void Get_OnValidInstruments_ShouldReturnPrice()
        {
            //when
            Instruments testInstruments = CreateTestInstruments();
            
            //execute
            RetrieveGetDataResponse retrieveGetDataResponse =  _getDataBbgCall.Get(testInstruments);
            InstrumentData[] instrumentDatas = retrieveGetDataResponse.instrumentDatas;
            string[] getDataFields = retrieveGetDataResponse.fields;

            //verify
            Assert.That(retrieveGetDataResponse.statusCode.code, Is.EqualTo(DLDataService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(2));
            AssertBbUniqueQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[0], "EQ0010174300001000");
            AssertIsinQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[1], "US0231351067", "EQ0021695200001000");
        }
        
        private void AssertBbUniqueQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(bbUid));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_UNIQUE"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }
        
        private void AssertIsinQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string isin, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(isin));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_UNIQUE"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }
        
    }
}