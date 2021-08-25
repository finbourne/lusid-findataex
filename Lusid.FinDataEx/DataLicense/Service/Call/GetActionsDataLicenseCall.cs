using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Call
{
    /// <summary>
    /// GetActions BBG DWLS call.
    ///
    /// GetActions calls request corporate actions data for a given set of instruments.
    /// 
    /// </summary>
    public class GetActionsDataLicenseCall : IDataLicenseCall<RetrieveGetActionsResponse>
    {
        /* DO NOT change _PollingInterval except for testing with Mocks. BBG DL will throttle
         or worse if poll interval against actual servers*/
        private readonly TimeSpan _pollingInterval;
        
        private readonly PerSecurityWS _perSecurityWs;
        private readonly List<DataLicenseTypes.CorpActionType> _corporateActions;

        public GetActionsDataLicenseCall(PerSecurityWS perSecurityWs, List<DataLicenseTypes.CorpActionType> corporateActions, TimeSpan? pollingInterval = null)
        {
            _corporateActions = corporateActions;
            _perSecurityWs = perSecurityWs;
            _pollingInterval = pollingInterval ?? DataLicenseUtils.DefaultPollingInterval;
        }

        /// <summary>
        /// Execute a GetActions call for a given set of instruments.
        /// 
        /// </summary>
        /// <param name="instruments">Instruments to retrieve corporate action data against.</param>
        /// <returns>Response from BBG DLWS that should contain corporate actions data and relevant status codes.</returns>
        public RetrieveGetActionsResponse Get(Instruments instruments)
        {
            var getActionRequest = CreateGetActionsRequest(instruments);
            var submitGetActionsRequest = _perSecurityWs.submitGetActionsRequest(getActionRequest);
            var submitGetActionsResponse = submitGetActionsRequest.submitGetActionsResponse;
            Console.WriteLine($"Submitted GetActionRequest. Response ID to check for {submitGetActionsResponse.responseId}");

            // Await for response and retrieve once ready
            var getActionsRespReq = RetrieveGetActionsResponseRequest(submitGetActionsResponse);
            var retrieveGetActionsResponse = GetActionsResponseSync(getActionsRespReq);

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
            return DataLicenseUtils
                .GetBBGRetryPolicy<RetrieveGetActionsResponse>(_pollingInterval)
                .Execute(() => _perSecurityWs.retrieveGetActionsResponse(getActionsRespReq).retrieveGetActionsResponse);
        }
        
        private submitGetActionsRequestRequest CreateGetActionsRequest(Instruments instruments)
        {
            var submitGetActionsRequest = new SubmitGetActionsRequest
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
                actions_date = ActionsDate.both,
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