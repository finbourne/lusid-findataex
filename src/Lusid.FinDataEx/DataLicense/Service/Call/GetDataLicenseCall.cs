using System;
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
    public class GetDataLicenseCall : IDataLicenseCall<RetrieveGetDataResponse>
    {
        /* DO NOT change _PollingInterval except for testing with Mocks. BBG DL will throttle
         or worse if poll interval against actual servers*/
        private readonly TimeSpan _pollingInterval;
        
        private readonly PerSecurityWS _perSecurityWs;
        private readonly GetDataHeaders _getDataHeaders;
        private readonly string[] _getDataFields;

        public GetDataLicenseCall(PerSecurityWS perSecurityWs, string[] dataFields = null, TimeSpan? pollingInterval = null)
        {
            _perSecurityWs = perSecurityWs;
            _getDataHeaders = GetDefaultHeaders();
            _getDataFields = dataFields ?? GetDefaultDataFields();
            _pollingInterval = pollingInterval ?? DataLicenseUtils.DefaultPollingInterval;
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
            Console.WriteLine($"Submitted GetDataRequest. Response ID to check for {submitGetDataResponse.responseId}");

            // Await for response and retrieve once ready
            var retrieveGetDataResponseRequest =
                RetrieveGetDataResponseRequest(submitGetDataResponse);
            var retrieveGetDataResponse = GetDataResponseSync(retrieveGetDataResponseRequest);

            // log output
            Console.WriteLine($"GetData response for id={retrieveGetDataResponse.responseId} and response-level status code={retrieveGetDataResponse.statusCode.code}({retrieveGetDataResponse.statusCode.description})");
            DataLicenseUtils.PrintGetDataResponse(retrieveGetDataResponse);
            DataLicenseUtils.PrintJsonResponse(retrieveGetDataResponse);
            return retrieveGetDataResponse;
        }

        public DataLicenseTypes.DataTypes GetDataType()
        {
            return DataLicenseTypes.DataTypes.GetData;
        }

        private RetrieveGetDataResponse GetDataResponseSync(retrieveGetDataResponseRequest retrieveGetDataResponseRequest)
        {
            return DataLicenseUtils
                .GetBBGRetryPolicy<RetrieveGetDataResponse>(_pollingInterval)
                .Execute(() => _perSecurityWs.retrieveGetDataResponse(retrieveGetDataResponseRequest).retrieveGetDataResponse);
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

        /// <summary>
        ///  Return default headers for a GetData Request.
        ///
        /// For details on header options see https://eap.bloomberg.com/docs/data-license/
        /// </summary>
        /// <returns></returns>
        private static GetDataHeaders GetDefaultHeaders()
        {
            return new GetDataHeaders
            {
                secmaster = true,
                secmasterSpecified = true,
                closingvalues = true,
                closingvaluesSpecified = true,
                derived = false,
                derivedSpecified = false
            };
        }

        private static string[] GetDefaultDataFields()
        {
            return new[]
            {
                "ID_BB_GLOBAL", "PX_LAST"
            };
        }
        
    }
}