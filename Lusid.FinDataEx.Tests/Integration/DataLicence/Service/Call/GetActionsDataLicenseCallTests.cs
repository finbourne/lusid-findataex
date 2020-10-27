using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Util;
using Lusid.FinDataEx.DataLicense.Vendor;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicence.Service.Call
{
    [TestFixture]
    public class GetActionsBbgCallTests
    {

        private PerSecurityWS _perSecurityWs;
        private GetActionsDataLicenseCall _getActionsDataLicenseCall;

        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = new PerSecurityWsFactory().CreateDefault(PerSecurityWsFactory.BbgDlAddress);
        }

        [Test]
        public void Get_OnValidInstrumentsAndMultipleCorpActions_ShouldReturnCorpActions()
        {
            //when
            _getActionsDataLicenseCall = new GetActionsDataLicenseCall(_perSecurityWs, 
                new List<DataLicenseTypes.CorpActionTypes>() {DataLicenseTypes.CorpActionTypes.DVD_CASH});
            var testInstruments = CreateCorpActionTestInstrument();
            
            //execute
            var retrieveGetActionsResponse =  _getActionsDataLicenseCall.Get(testInstruments);
            var instrumentDatas = retrieveGetActionsResponse.instrumentDatas;

            //verify
            Assert.That(retrieveGetActionsResponse.statusCode.code, Is.EqualTo(DataLicenseService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(1));
            //AssertBbUniqueQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[0], "EQ0010174300001000");
        }
        
        /*private void AssertBbUniqueQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(bbUid));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_UNIQUE"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }*/

        private Instruments CreateCorpActionTestInstrument()
        {
            var corporateActionInstrument = new PerSecurity_Dotnet.Instrument
            {
                id = "COP US",
                yellowkeySpecified = true,
                typeSpecified = true,
                yellowkey = MarketSector.Equity,
                type = InstrumentType.TICKER
            };
            return new Instruments {instrument = new[] {corporateActionInstrument}};
        }

    }
}