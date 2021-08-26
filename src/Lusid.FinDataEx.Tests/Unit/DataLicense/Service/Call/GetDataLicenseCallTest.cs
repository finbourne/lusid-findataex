using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;
using System;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service.Call
{
    [TestFixture]
    public class GetDataLicenseCallTest
    {

        private GetDataLicenseCall _getDataLicenseCall;
        private PerSecurityWS _perSecurityWs;
        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = Mock.Of<PerSecurityWS>();
            _getDataLicenseCall = new GetDataLicenseCall(_perSecurityWs, pollingInterval: TimeSpan.FromMilliseconds(10));
        }

        [Test]
        public void Get_OnValidInstruments_ShouldReturnPrice()
        {
            //when
            var testInstruments = CreateTestInstruments();
            var responseId = "1602149495-71386027_ValidInstruments";

            // setup mock to submit request and get back response id to poll
            submitGetDataRequestRequest submitGetDataRequestRequest = null;
            var submitGetDataRequestResponse = CreateSubmitGetDataRequestResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                perWsMock.submitGetDataRequest(It.IsAny<submitGetDataRequestRequest>()))
                // return a submit request response that contains the response id used to poll for results
                .Returns(submitGetDataRequestResponse)
                //retrieve request argument to ensure properly constructed
                .Callback<submitGetDataRequestRequest>(r => submitGetDataRequestRequest = r);
            
            // setup mock service to return successful data request. 
            retrieveGetDataResponseRequest retrieveGetDataResponseRequest = null;
            var retrieveGetDataResponseResponse =
                CreateRetrieveGetDataResponseResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                    perWsMock.retrieveGetDataResponse(It.IsAny<retrieveGetDataResponseRequest>()))
                // return the expected data response from bbg
                .Returns(retrieveGetDataResponseResponse)
                //retrieve get data request argument that contains the response id. ensure it's properly constructed
                .Callback<retrieveGetDataResponseRequest>(r => retrieveGetDataResponseRequest = r);
            
            //execute test
            var retrieveGetDataResponse = _getDataLicenseCall.Get(testInstruments);
            var instrumentDatas = retrieveGetDataResponse.instrumentDatas;
            var getDataFields = retrieveGetDataResponse.fields;

            
            // verify correct submit request was constructed
            Assert.That(submitGetDataRequestRequest.submitGetDataRequest.instruments, Is.EqualTo(testInstruments));
            
            // verify correct get data request with response id constructed
            Assert.That(retrieveGetDataResponseRequest.retrieveGetDataRequest.responseId, Is.EqualTo(responseId));
            
            //verify response as expected
            Assert.That(retrieveGetDataResponse.responseId, Is.EqualTo(responseId));
            Assert.That(retrieveGetDataResponse.statusCode.code, Is.EqualTo(DataLicenseService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(2));
            AssertBbUniqueQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[0], "BBG000BPHFS9", "209.830000");
            AssertIsinQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[1], "US0231351067", "BBG000BVPV84", "3195.690000");
        }
        
        [Test]
        public void Get_OnOneBadInstrumentId_ShouldReturnNothingForBadInstrumentOnly()
        {
            // A bad instrument should still return as a success for the general BBG DL call but the
            // instrument data should be empty with instrument specific status code as error.
            
            //when
            var testInstruments = CreateTestInstrumentsWithBadInstrument();
            var responseId = "1602161569-1051504982_OneBadInstrument";

            // setup mock to submit request and get back response id to poll
            submitGetDataRequestRequest submitGetDataRequestRequest = null;
            var submitGetDataRequestResponse = CreateSubmitGetDataRequestResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                perWsMock.submitGetDataRequest(It.IsAny<submitGetDataRequestRequest>()))
                // return a submit request response that contains the response id used to poll for results
                .Returns(submitGetDataRequestResponse)
                //retrieve request argument to ensure properly constructed
                .Callback<submitGetDataRequestRequest>(r => submitGetDataRequestRequest = r);
            
            // setup mock service to return successful data request. 
            retrieveGetDataResponseRequest retrieveGetDataResponseRequest = null;
            var retrieveGetDataResponseResponse =
                CreateRetrieveGetDataResponseResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                    perWsMock.retrieveGetDataResponse(It.IsAny<retrieveGetDataResponseRequest>()))
                // return the expected data response from bbg
                .Returns(retrieveGetDataResponseResponse)
                //retrieve get data request argument that contains the response id. ensure it's properly constructed
                .Callback<retrieveGetDataResponseRequest>(r => retrieveGetDataResponseRequest = r);
            
            //execute test
            var retrieveGetDataResponse = _getDataLicenseCall.Get(testInstruments);
            var instrumentDatas = retrieveGetDataResponse.instrumentDatas;
            var getDataFields = retrieveGetDataResponse.fields;

            
            // verify correct submit request was constructed
            Assert.That(submitGetDataRequestRequest.submitGetDataRequest.instruments, Is.EqualTo(testInstruments));
            
            // verify correct get data request with response id constructed
            Assert.That(retrieveGetDataResponseRequest.retrieveGetDataRequest.responseId, Is.EqualTo(responseId));
            
            //verify response as expected
            Assert.That(retrieveGetDataResponse.responseId, Is.EqualTo(responseId));
            Assert.That(retrieveGetDataResponse.statusCode.code, Is.EqualTo(DataLicenseService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(2));
            AssertBadInstrumentIsNotPopulated(getDataFields, instrumentDatas[0], "TestBadUniqueId");
            AssertIsinQueriedInstrumentIsPopulated(getDataFields, instrumentDatas[1], "US0231351067", "BBG000BVPV84", "3195.690000");
        }

        private void AssertBbUniqueQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string bbUid, string lastPrice)
        {
            Assert.That(instrumentData.code, Is.EqualTo(DataLicenseService.InstrumentSuccessCode));
            Assert.That(instrumentData.instrument.id, Is.EqualTo(bbUid));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_GLOBAL"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.EqualTo(lastPrice));
        }
        
        private void AssertIsinQueriedInstrumentIsPopulated(string[] getDataFields, InstrumentData instrumentData, string isin, string bbUid, string lastPrice)
        {
            Assert.That(instrumentData.code, Is.EqualTo(DataLicenseService.InstrumentSuccessCode));
            Assert.That(instrumentData.instrument.id, Is.EqualTo(isin));
            Assert.That(instrumentData.instrument.yellowkey, Is.EqualTo(MarketSector.Govt));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_GLOBAL"));
            Assert.That(instrumentData.data[0].value, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.EqualTo(lastPrice));
        }
        
        private void AssertBadInstrumentIsNotPopulated(string[] getDataFields, InstrumentData instrumentData, string bbUid)
        {
            Assert.That(instrumentData.code, Is.Not.EqualTo(DataLicenseService.InstrumentSuccessCode));
            Assert.That(instrumentData.instrument.id, Is.EqualTo(bbUid));
            
            Assert.That(getDataFields[0], Is.EqualTo("ID_BB_GLOBAL"));
            Assert.That(instrumentData.data[0].value, Is.Empty);
            
            Assert.That(getDataFields[1], Is.EqualTo("PX_LAST"));
            Assert.That(instrumentData.data[1].value, Is.Empty);
        }

        internal static Instruments CreateTestInstruments()
        {
            // MSFT
            var bbUniqueId1 = new Instrument
            {
                id = "BBG000BPHFS9", type = InstrumentType.BB_GLOBAL, typeSpecified = true
            };

            //AMZ
            var bbUniqueId2 = new Instrument
            {
                id = "US0231351067", type = InstrumentType.ISIN, typeSpecified = true
            };

            return new Instruments {instrument = new Instrument[] {bbUniqueId1, bbUniqueId2}};
        }

        private Instruments CreateTestInstrumentsWithBadInstrument()
        {
            var bbUniqueId1 = new Instrument
            {
                id = "TestBadUniqueId", type = InstrumentType.BB_GLOBAL, typeSpecified = true
            };

            var bbUniqueId2 = new Instrument
            {
                id = "US0231351067", type = InstrumentType.ISIN, typeSpecified = true
            };

            return new Instruments {instrument = new Instrument[] {bbUniqueId1, bbUniqueId2}};
        }

        private submitGetDataRequestResponse CreateSubmitGetDataRequestResponse(string responseId)
        {
            var submitGetDataResponse = new SubmitGetDataResponse {responseId = responseId};
            var submitGetDataRequestResponse = new submitGetDataRequestResponse
            {
                submitGetDataResponse = submitGetDataResponse
            };
            return submitGetDataRequestResponse;
        }

        private retrieveGetDataResponseResponse CreateRetrieveGetDataResponseResponse(string responseId)
        {
            var retrieveGetDataResponse = TestUtils.LoadGetDataResponseFromFile(responseId);
            var retrieveGetDataResponseResponse = new retrieveGetDataResponseResponse
            {
                retrieveGetDataResponse = retrieveGetDataResponse
            };
            return retrieveGetDataResponseResponse;
        }

    }
}