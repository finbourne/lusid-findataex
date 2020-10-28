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
    public class GetActionsDataLicenseCallTests
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
                new List<DataLicenseTypes.CorpActionType>() {DataLicenseTypes.CorpActionType.STOCK_SPLT, DataLicenseTypes.CorpActionType.DVD_CASH, DataLicenseTypes.CorpActionType.DVD_STOCK});
            var testInstruments = CreateCorpActionTestInstrument();
            
            //execute
            var retrieveGetActionsResponse =  _getActionsDataLicenseCall.Get(testInstruments);
            var instrumentDatas = retrieveGetActionsResponse.instrumentDatas;

            //verify
            Assert.That(retrieveGetActionsResponse.statusCode.code, Is.EqualTo(DataLicenseService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(1));
        }

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