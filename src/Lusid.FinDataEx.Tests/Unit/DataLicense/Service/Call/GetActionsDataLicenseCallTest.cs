using System;
using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Util;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Unit.DataLicense.Service.Call
{
    [TestFixture]
    public class GetActionsDataLicenseCallTest
    {

        private GetActionsDataLicenseCall _getActionDataLicenseCall;
        private PerSecurityWS _perSecurityWs;
        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = Mock.Of<PerSecurityWS>();
            _getActionDataLicenseCall = new GetActionsDataLicenseCall(
                _perSecurityWs,
                new List<DataLicenseTypes.CorpActionType>() {DataLicenseTypes.CorpActionType.STOCK_SPLT, DataLicenseTypes.CorpActionType.DVD_CASH, DataLicenseTypes.CorpActionType.DVD_STOCK},
                TimeSpan.FromMilliseconds(10));
        }

        [Test]
        public void Get_OnValidActions_ShouldReturnCorpActions()
        {
            //when
            var testInstruments = CreateCorpActionTestInstrument();
            var responseId = "1603798418-1052073180_ValidActions";

            // setup mock to submit request and get back response id to poll
            submitGetActionsRequestRequest submitGetActionsRequestRequest = null;
            var submitGetActionsRequestResponse = CreateSubmitGetActionsRequestResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                perWsMock.submitGetActionsRequest(It.IsAny<submitGetActionsRequestRequest>()))
                // return a submit request response that contains the response id used to poll for results
                .Returns(submitGetActionsRequestResponse)
                //retrieve request argument to ensure properly constructed
                .Callback<submitGetActionsRequestRequest>(r => submitGetActionsRequestRequest = r);
            
            // setup mock service to return successful data request. 
            retrieveGetActionsResponseRequest retrieveGetDataResponseRequest = null;
            var retrieveGetActionsResponseResponse =
                CreateRetrieveGetActionResponseResponse(responseId);
            Mock.Get(_perSecurityWs).Setup(perWsMock =>
                    perWsMock.retrieveGetActionsResponse(It.IsAny<retrieveGetActionsResponseRequest>()))
                // return the expected data response from bbg
                .Returns(retrieveGetActionsResponseResponse)
                //retrieve get data request argument that contains the response id. ensure it's properly constructed
                .Callback<retrieveGetActionsResponseRequest>(r => retrieveGetDataResponseRequest = r);
            
            //execute test
            var retrieveGetActionsResponse = _getActionDataLicenseCall.Get(testInstruments);
            var instrumentDatas = retrieveGetActionsResponse.instrumentDatas;

            // verify correct submit request was constructed
            Assert.That(submitGetActionsRequestRequest.submitGetActionsRequest.instruments, Is.EqualTo(testInstruments));
            
            // verify correct get data request with response id constructed
            Assert.That(retrieveGetDataResponseRequest.retrieveGetActionsRequest.responseId, Is.EqualTo(responseId));
            
            //verify response as expected
            Assert.That(retrieveGetActionsResponse.responseId, Is.EqualTo(responseId));
            Assert.That(retrieveGetActionsResponse.statusCode.code, Is.EqualTo(DataLicenseService.Success));
            Assert.That(instrumentDatas.Length, Is.EqualTo(1));
            
        }

        private Instruments CreateCorpActionTestInstrument()
        {
            var corporateActionInstrument = new Instrument
            {
                id = "US1729081059",
                yellowkeySpecified = true,
                typeSpecified = true,
                yellowkey = MarketSector.Equity,
                type = InstrumentType.ISIN
            };
            return new Instruments {instrument = new[] {corporateActionInstrument}};
        }

        private submitGetActionsRequestResponse CreateSubmitGetActionsRequestResponse(string responseId)
        {
            var submitGetActionResponse = new SubmitGetActionsResponse() {responseId = responseId};
            var submitGetActionRequestResponse = new submitGetActionsRequestResponse()
            {
                submitGetActionsResponse = submitGetActionResponse
            };
            return submitGetActionRequestResponse;
        }

        private retrieveGetActionsResponseResponse CreateRetrieveGetActionResponseResponse(string responseId)
        {
            var retrieveGetActionsResponse = TestUtils.LoadGetActionsResponseFromFile(responseId);
            var retrieveGetActionsResponseResponse = new retrieveGetActionsResponseResponse
            {
                retrieveGetActionsResponse = retrieveGetActionsResponse
            };
            return retrieveGetActionsResponseResponse;
        }

    }
}