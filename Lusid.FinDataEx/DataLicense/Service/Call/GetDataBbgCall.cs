using System;
using System.Threading;
using Lusid.FinDataEx.DataLicense.Util;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Call
{
    /// <summary>
    ///  GetData BBG DLWS call.
    ///
    ///  GetData calls are the typical BBG DL call that request a set of fields of data for
    ///  a given set of instruments
    /// 
    /// </summary>
    public class GetDataBbgCall : IBbgCall<RetrieveGetDataResponse>
    {
        /* DO NOT change PollInterval except for testing with Mocks. BBG DL will throttle
         or worse if poll interval against actual servers*/
        public int PollingInterval { get; set; } = DlDataService.PollInterval;
        
        private readonly PerSecurityWS _perSecurityWs;
        private readonly GetDataHeaders _getDataHeaders;
        private readonly string[] _getDataFields;

        public GetDataBbgCall(PerSecurityWS perSecurityWs) : this(perSecurityWs, GetDefaultHeaders(),
            GetDefaultDataFields()) {}
        
        public GetDataBbgCall(PerSecurityWS perSecurityWs, string[] dataFields) : this(perSecurityWs, GetDefaultHeaders(),
            dataFields) {}

        private GetDataBbgCall(PerSecurityWS perSecurityWs, GetDataHeaders dataHeaders, string[] dataFields)
        {
            _perSecurityWs = perSecurityWs;
            _getDataHeaders = dataHeaders;
            _getDataFields = dataFields;
        }

        /// <summary>
        /// Execute a GetData call for a given set of instruments.
        /// 
        /// </summary>
        /// <param name="instruments">Instruments to retrieve data against.</param>
        /// <returns>Response from BBG DLWS that should contain the requested data and relevant status codes.</returns>
        public RetrieveGetDataResponse Get(Instruments instruments)
        {
            var getDataRequest = CreateGetDataRequest(instruments);
            var submitGetDataRequest = _perSecurityWs.submitGetDataRequest(getDataRequest);
            var submitGetDataResponse = submitGetDataRequest.submitGetDataResponse;
            Console.WriteLine($"Submitted GetDataRequest. Response ID to check for {submitGetDataResponse.responseId} with current status {submitGetDataResponse.statusCode}");
            
            // Await for response and retrieve once ready
            var retrieveGetDataResponseRequest =
                RetrieveGetDataResponseRequest(submitGetDataResponse);
            var retrieveGetDataResponse =  GetDataResponseSync(retrieveGetDataResponseRequest);


            // log output
            Console.WriteLine($"GetData response for id={retrieveGetDataResponse.responseId} and response-level status code={retrieveGetDataResponse.statusCode.code}({retrieveGetDataResponse.statusCode.description})");
            DlUtils.PrintGetDataResponse(retrieveGetDataResponse);
            DlUtils.PrintJsonResponse(retrieveGetDataResponse);
            return retrieveGetDataResponse;
        }

        public DlTypes.DataTypes GetDlDataType()
        {
            return DlTypes.DataTypes.GetData;
        }

        private RetrieveGetDataResponse GetDataResponseSync(retrieveGetDataResponseRequest retrieveGetDataResponseRequest)
        {
            //Poll for data availability. As per BBG DL sample recommendation.
            //Beware amending the poll interval due to BBG limitations. Especially in TEST.
            RetrieveGetDataResponse getDataResponse;
            do
            {
                Thread.Sleep(PollingInterval);
                var retrieveGetDataResponse = _perSecurityWs.retrieveGetDataResponse(retrieveGetDataResponseRequest);
                getDataResponse = retrieveGetDataResponse.retrieveGetDataResponse;
            }
            while (getDataResponse.statusCode.code == DlDataService.DataNotAvailable);
            return getDataResponse;
        }

        private submitGetDataRequestRequest CreateGetDataRequest(Instruments instruments)
        {
            var submitGetDataRequest = new SubmitGetDataRequest
            {
                headers = _getDataHeaders, fields = _getDataFields, instruments = instruments
            };
            var submitGetDataRequestRequest = new submitGetDataRequestRequest(submitGetDataRequest);
            return submitGetDataRequestRequest;
        }

        private retrieveGetDataResponseRequest RetrieveGetDataResponseRequest(SubmitGetDataResponse submitGetDataResponse)
        {
            var retrieveGetDataRequest = new RetrieveGetDataRequest
            {
                responseId = submitGetDataResponse.responseId
            };
            var retrieveGetDataResponseRequest = new retrieveGetDataResponseRequest(retrieveGetDataRequest);
            return retrieveGetDataResponseRequest;
        }

        private static GetDataHeaders GetDefaultHeaders()
        {
            return new GetDataHeaders
            {
                secmaster = true,
                secmasterSpecified = true,
                closingvalues = true,
                closingvaluesSpecified = true,
                derived = true,
                derivedSpecified = true
            };
        }

        private static string[] GetDefaultDataFields()
        {
            return new[]
            {
                "ID_BB_UNIQUE", "PX_LAST"
            };
        }
        
    }
}