using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lusid.FinDataEx.DataLicense.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Call
{
    public class GetActionsBbgCall : IBbgCall<RetrieveGetActionsResponse>
    {
        /// <summary>Polling interval for BBG requests. Should only change from
        /// default for mock testing purposes</summary>
        public int GetActionsBbgPollingInterval { get; set; } = DlDataService.PollInterval;
        
        private readonly PerSecurityWS _perSecurityWs;
        private readonly List<DlTypes.CorpActionTypes> _corporateActions;

        public GetActionsBbgCall(PerSecurityWS perSecurityWs, List<DlTypes.CorpActionTypes> corporateActions)
        {
            _corporateActions = corporateActions;
            _perSecurityWs = perSecurityWs;
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
            DlUtils.PrintGetActionsResponse(retrieveGetActionsResponse);
            DlUtils.PrintJsonResponse(retrieveGetActionsResponse);
            return retrieveGetActionsResponse;
        }
        
        private RetrieveGetActionsResponse GetActionsResponseSync(retrieveGetActionsResponseRequest getActionsRespReq)
        {
            //Poll for data availability. As per BBG DL sample recommendation.
            //Beware amending the poll interval due to BBG limitations. Especially in TEST.
            RetrieveGetActionsResponse getActionsResponse;
            do
            {
                Thread.Sleep(GetActionsBbgPollingInterval);
                var retrieveGetActionsResponse = _perSecurityWs.retrieveGetActionsResponse(getActionsRespReq);
                getActionsResponse = retrieveGetActionsResponse.retrieveGetActionsResponse;
            }
            while (getActionsResponse.statusCode.code == DlDataService.DataNotAvailable);
            return getActionsResponse;
        }
        
        private submitGetActionsRequestRequest CreateGetActionsRequest(Instruments instruments)
        {
            SubmitGetActionsRequest submitGetActionsRequest = new SubmitGetActionsRequest
            {
                headers = CreateDefaultHeaders(), instruments = instruments
            };

            return new submitGetActionsRequestRequest(submitGetActionsRequest);
        }

        private retrieveGetActionsResponseRequest RetrieveGetActionsResponseRequest(
            SubmitGetActionsResponse submitGetActionsResponse)
        {
            var retrieveGetActionsRequest = new RetrieveGetActionsRequest {responseId = submitGetActionsResponse.responseId};
            return new retrieveGetActionsResponseRequest(retrieveGetActionsRequest);
        }

        private GetActionsHeaders CreateDefaultHeaders()
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
        
        private GetActionsHeaders GetDefaultHeaders()
        {
            return new GetActionsHeaders
            {
                actions_date = ActionsDate.entry,
                actions_dateSpecified = true,
                actions = GetCorporateActionsForRequest()
            };
        }
    }

}