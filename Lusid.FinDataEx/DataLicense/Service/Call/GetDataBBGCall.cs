﻿using System;
using System.Threading;
using Lusid.FinDataEx.DataLicense.Service;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense
{
    public class GetDataBbgCall : IBbgCall<RetrieveGetDataResponse>
    {
        private readonly PerSecurityWS _perSecurityWs;
        private readonly GetDataHeaders _getDataHeaders;
        private readonly string[] _getDataFields;

        public GetDataBbgCall(PerSecurityWS perSecurityWs) : this(perSecurityWs, GetDefaultHeaders(),
            GetDefaultDataFields()) {}
        
        public GetDataBbgCall(PerSecurityWS perSecurityWs, GetDataHeaders dataHeaders, string[] dataFields)
        {
            _perSecurityWs = perSecurityWs;
            _getDataHeaders = dataHeaders;
            _getDataFields = dataFields;
        }

        public RetrieveGetDataResponse Get(Instruments instruments)
        {
            submitGetDataRequestRequest getDataRequest = CreateGetDataRequest(instruments);
            submitGetDataRequestResponse submitGetDataRequest = _perSecurityWs.submitGetDataRequest(getDataRequest);
            SubmitGetDataResponse submitGetDataResponse = submitGetDataRequest.submitGetDataResponse;
            Console.WriteLine($"Submitted GetDataRequest. Response ID to check for {submitGetDataResponse.responseId} with current status {submitGetDataResponse.statusCode}");
            
            // Await for response and retrieve once ready
            retrieveGetDataResponseRequest retrieveGetDataResponseRequest =
                RetrieveGetDataResponseRequest(submitGetDataResponse);
            RetrieveGetDataResponse retrieveGetDataResponse =  GetDataResponseSync(retrieveGetDataResponseRequest);
            return retrieveGetDataResponse;
        }

        private RetrieveGetDataResponse GetDataResponseSync(retrieveGetDataResponseRequest retrieveGetDataResponseRequest)
        {
            //Poll for data availability. As per BBG DL sample recommendation.
            //Beware amending the poll interval due to BBG limitations. Especially in TEST.
            RetrieveGetDataResponse getDataResponse;
            do
            {
                Thread.Sleep(DLDataService.PollInterval);
                retrieveGetDataResponseResponse retrieveGetDataResponse = _perSecurityWs.retrieveGetDataResponse(retrieveGetDataResponseRequest);
                getDataResponse = retrieveGetDataResponse.retrieveGetDataResponse;
            }
            while (getDataResponse.statusCode.code == DLDataService.DataNotAvailable);
            return getDataResponse;
        }

        private submitGetDataRequestRequest CreateGetDataRequest(Instruments instruments)
        {
            SubmitGetDataRequest submitGetDataRequest = new SubmitGetDataRequest
            {
                headers = _getDataHeaders, fields = _getDataFields, instruments = instruments
            };
            submitGetDataRequestRequest submitGetDataRequestRequest = new submitGetDataRequestRequest(submitGetDataRequest);
            return submitGetDataRequestRequest;
        }

        private retrieveGetDataResponseRequest RetrieveGetDataResponseRequest(SubmitGetDataResponse submitGetDataResponse)
        {
            RetrieveGetDataRequest retrieveGetDataRequest = new RetrieveGetDataRequest
            {
                responseId = submitGetDataResponse.responseId
            };
            retrieveGetDataResponseRequest retrieveGetDataResponseRequest = new retrieveGetDataResponseRequest(retrieveGetDataRequest);
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