using System.IO;
using Lusid.FinDataEx.DataLicense;
using Lusid.FinDataEx.DataLicense.Service;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.DataLicence.Service.Call
{
    [TestFixture]
    public class GetDataBbgCallTests
    {

        private GetDataBbgCall _getDataBbgCall;
        private PerSecurityWS _perSecurityWs;
        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = Mock.Of<PerSecurityWS>();
            _getDataBbgCall = new GetDataBbgCall(_perSecurityWs);
            _getDataBbgCall.PollingInterval = 10;
        }

        [Test]
        public void Get_OnValidInstruments_ShouldReturnPrice()
        {
            //when
            Instruments testInstruments = CreateTestInstruments();
            string responseId = "1602149495-71386027";

            // setup mock to submit request and get back response id to poll
            submitGetDataRequestRequest submitGetDataRequestRequest = null;
            submitGetDataRequestResponse submitGetDataRequestResponse = CreateSubmitGetDataRequestResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                perWsMock.submitGetDataRequest(It.IsAny<submitGetDataRequestRequest>()))
                // return a submit request response that contains the response id used to poll for results
                .Returns(submitGetDataRequestResponse)
                //retrieve request argument to ensure properly constructed
                .Callback<submitGetDataRequestRequest>(r => submitGetDataRequestRequest = r);
            
            // setup mock service to return successful data request. 
            retrieveGetDataResponseRequest retrieveGetDataResponseRequest = null;
            retrieveGetDataResponseResponse retrieveGetDataResponseResponse =
                CreateRetrieveGetDataResponseResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                    perWsMock.retrieveGetDataResponse(It.IsAny<retrieveGetDataResponseRequest>()))
                // return the expected data response from bbg
                .Returns(retrieveGetDataResponseResponse)
                //retrieve get data request argument that contains the response id. ensure it's properly constructed
                .Callback<retrieveGetDataResponseRequest>(r => retrieveGetDataResponseRequest = r);
            
            //execute test
            RetrieveGetDataResponse retrieveGetDataResponse =  _getDataBbgCall.Get(testInstruments);
            InstrumentData[] instrumentDatas = retrieveGetDataResponse.instrumentDatas;
            string[] getDataFields = retrieveGetDataResponse.fields;

            
            // verify correct submit request was constructed
            Assert.That(submitGetDataRequestRequest.submitGetDataRequest.instruments, Is.EqualTo(testInstruments));
            
            // verify correct get data request with response id constructed
            Assert.That(retrieveGetDataResponseRequest.retrieveGetDataRequest.responseId, Is.EqualTo(responseId));
            
            //verify response as expected
            Assert.That(retrieveGetDataResponse.responseId, Is.EqualTo(responseId));
            Assert.That(retrieveGetDataResponse.statusCode.code, Is.EqualTo(DLDataService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(2));
            AssertBbUniqueQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[0], "EQ0010174300001000");
            AssertIsinQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[1], "US0231351067", "EQ0021695200001000");
        }

        internal static void AssertBbUniqueQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(bbUid));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_UNIQUE"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }
        
        internal static void AssertIsinQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string isin, string bbUid)
        {
            Assert.That(instrumentData.instrument.id, Is.EqualTo(isin));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_UNIQUE"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Not.Null);
        }

        internal static Instruments CreateTestInstruments()
        {
            // IBM
            Instrument bbUniqueId1 = new Instrument
            {
                id = "EQ0010174300001000", type = InstrumentType.BB_UNIQUE, typeSpecified = true
            };

            // FB
            Instrument bbUniqueId2 = new Instrument
            {
                id = "US0231351067", type = InstrumentType.ISIN, typeSpecified = true
            };

            return new Instruments {instrument = new Instrument[] {bbUniqueId1, bbUniqueId2}};
        }

        private submitGetDataRequestResponse CreateSubmitGetDataRequestResponse(string responseId)
        {
            SubmitGetDataResponse submitGetDataResponse = new SubmitGetDataResponse {responseId = responseId};
            submitGetDataRequestResponse submitGetDataRequestResponse = new submitGetDataRequestResponse
            {
                submitGetDataResponse = submitGetDataResponse
            };
            return submitGetDataRequestResponse;
        }

        private retrieveGetDataResponseResponse CreateRetrieveGetDataResponseResponse(string responseId)
        {
            RetrieveGetDataResponse retrieveGetDataResponse = LoadResponseFromFile(responseId);
            retrieveGetDataResponseResponse retrieveGetDataResponseResponse = new retrieveGetDataResponseResponse
            {
                retrieveGetDataResponse = retrieveGetDataResponse
            };
            return retrieveGetDataResponseResponse;
        }

        private RetrieveGetDataResponse LoadResponseFromFile(string responseId)
        {
            string responsePath = Path.Combine(new[]{"DataLicense","Service","Call","TestData",$"{responseId}.json"});
            RetrieveGetDataResponse retrieveGetDataResponse =  JsonConvert.DeserializeObject<RetrieveGetDataResponse>(File.ReadAllText(responsePath));
            return retrieveGetDataResponse;
        }
        
    }
}