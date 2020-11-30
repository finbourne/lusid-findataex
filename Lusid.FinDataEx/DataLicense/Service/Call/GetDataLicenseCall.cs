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
    public class GetDataLicenseCall : IDataLicenseCall<RetrieveGetDataResponse>
    {
        /* DO NOT change _PollingInterval except for testing with Mocks. BBG DL will throttle
         or worse if poll interval against actual servers*/
        private readonly int _pollingInterval;
        
        private readonly PerSecurityWS _perSecurityWs;
        private readonly GetDataHeaders _getDataHeaders;
        private readonly string[] _getDataFields;

        public GetDataLicenseCall(PerSecurityWS perSecurityWs, int pollingInterval=DataLicenseUtils.DefaultPollingInterval) : this(perSecurityWs, GetDefaultHeaders(),
            GetDefaultDataFields())
        {
            _pollingInterval = pollingInterval;
        }

        public GetDataLicenseCall(PerSecurityWS perSecurityWs, string[] dataFields,
            int pollingInterval = DataLicenseUtils.DefaultPollingInterval) : this(perSecurityWs, GetDefaultHeaders(),
            dataFields)
        {
            _pollingInterval = pollingInterval;
        }

        private GetDataLicenseCall(PerSecurityWS perSecurityWs, GetDataHeaders dataHeaders, string[] dataFields)
        {
            _perSecurityWs = perSecurityWs;
            _getDataHeaders = dataHeaders;
            _getDataFields = dataFields;
            _pollingInterval = DataLicenseUtils.DefaultPollingInterval;
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
            //Poll for data availability. As per BBG DL sample recommendation.
            //Beware amending the poll interval due to BBG limitations. Especially in TEST.
            RetrieveGetDataResponse getDataResponse;
            do
            {
                Thread.Sleep(_pollingInterval);
                var retrieveGetDataResponse = _perSecurityWs.retrieveGetDataResponse(retrieveGetDataResponseRequest);
                getDataResponse = retrieveGetDataResponse.retrieveGetDataResponse;
            }
            while (getDataResponse.statusCode.code == DataLicenseService.DataNotAvailable);
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