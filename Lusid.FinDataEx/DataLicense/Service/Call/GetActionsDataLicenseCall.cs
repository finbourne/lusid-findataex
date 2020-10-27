using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lusid.FinDataEx.DataLicense.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Call
{
    public class GetActionsBbgCall : IDataLicenseCall<RetrieveGetActionsResponse>
    {
        /* DO NOT change _PollingInterval except for testing with Mocks. BBG DL will throttle
         or worse if poll interval against actual servers*/
        private readonly int _pollingInterval;
        
        private readonly PerSecurityWS _perSecurityWs;
        private readonly List<DataLicenseTypes.CorpActionTypes> _corporateActions;

        public GetActionsBbgCall(PerSecurityWS perSecurityWs, List<DataLicenseTypes.CorpActionTypes> corporateActions, int pollingInterval=DataLicenseUtils.DefaultPollingInterval)
        {
            _corporateActions = corporateActions;
            _perSecurityWs = perSecurityWs;
            _pollingInterval = pollingInterval;
        }

        public RetrieveGetActionsResponse Get(Instruments instruments)
        {
            var getActionRequest = CreateGetActionsRequest(instruments);
            var submitGetActionsRequest = _perSecurityWs.submitGetActionsRequest(getActionRequest);
            var submitGetActionsResponse = submitGetActionsRequest.submitGetActionsResponse;
            Console.WriteLine($"Submitted GetActionRequest. Response ID to check for {submitGetActionsResponse.responseId} with current status {submitGetActionsResponse.statusCode}");

            // Await for response and retrieve once ready
            var getActionsRespReq = RetrieveGetActionsResponseRequest(submitGetActionsResponse);
            var retrieveGetActionsResponse =  GetActionsResponseSync(getActionsRespReq);

            //log output
            Console.WriteLine($"GetActions response for id={retrieveGetActionsResponse.responseId} and response-level status code={retrieveGetActionsResponse.statusCode.code}({retrieveGetActionsResponse.statusCode.description})");
            DataLicenseUtils.PrintGetActionsResponse(retrieveGetActionsResponse);
            DataLicenseUtils.PrintJsonResponse(retrieveGetActionsResponse);
            return retrieveGetActionsResponse;
        }
        
        public DataLicenseTypes.DataTypes GetDataType()
        {
            return DataLicenseTypes.DataTypes.GetActions;
        }
        
        private RetrieveGetActionsResponse GetActionsResponseSync(retrieveGetActionsResponseRequest getActionsRespReq)
        {
            //Poll for data availability. As per BBG DL sample recommendation.
            //Beware amending the poll interval due to BBG limitations. Especially in TEST.
            RetrieveGetActionsResponse getActionsResponse;
            do
            {
                Thread.Sleep(_pollingInterval);
                var retrieveGetActionsResponse = _perSecurityWs.retrieveGetActionsResponse(getActionsRespReq);
                getActionsResponse = retrieveGetActionsResponse.retrieveGetActionsResponse;
            }
            while (getActionsResponse.statusCode.code == DataLicenseService.DataNotAvailable);
            return getActionsResponse;
        }
        
        private submitGetActionsRequestRequest CreateGetActionsRequest(Instruments instruments)
        {
            SubmitGetActionsRequest submitGetActionsRequest = new SubmitGetActionsRequest
            {
                headers = GetDefaultHeaders(), instruments = instruments
            };

            return new submitGetActionsRequestRequest(submitGetActionsRequest);
        }

        private retrieveGetActionsResponseRequest RetrieveGetActionsResponseRequest(
            SubmitGetActionsResponse submitGetActionsResponse)
        {
            var retrieveGetActionsRequest = new RetrieveGetActionsRequest {responseId = submitGetActionsResponse.responseId};
            return new retrieveGetActionsResponseRequest(retrieveGetActionsRequest);
        }

        private GetActionsHeaders GetDefaultHeaders()
        {
            return new GetActionsHeaders
            {
                actions_date = ActionsDate.entry,
                actions_dateSpecified = true,
                actions = GetCorporateActionsForRequest()
            };
        }

        private string[] GetCorporateActionsForRequest()
        {
            return _corporateActions.Select(a => a.ToString()).ToArray();
        }
    }

}