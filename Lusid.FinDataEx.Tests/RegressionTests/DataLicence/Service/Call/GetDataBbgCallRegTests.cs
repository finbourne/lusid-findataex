using Lusid.FinDataEx.DataLicense;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Vendor;
using NUnit.Framework;
using PerSecurity_Dotnet;
using static Lusid.FinDataEx.Tests.DataLicence.Service.Call.GetDataBbgCallTests;

namespace Lusid.FinDataEx.Tests.DataLicence.Service.Call
{
    [TestFixture]
    public class GetDataBbgCallRegTests
    {

        private GetDataBbgCall _getDataBbgCall;

        [SetUp]
        public void SetUp()
        {
            string clientCertFilePath = "C:\\finbourne\\repos-1\\DLWSDotnetSample\\DLWSCert.p12";
            string clientCertPassword = "TXTNEDQZ";
            PerSecurityWS perSecurityWs = new PerSecurityWSFactory().CreateDefault(PerSecurityWSFactory.BbgDlAddress, clientCertFilePath, clientCertPassword);
            //PerSecurityWS perSecurityWs = new PerSecurityWSFactory().CreateDefault(PerSecurityWSFactory.BbgDlAddress);
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
        
    }
}