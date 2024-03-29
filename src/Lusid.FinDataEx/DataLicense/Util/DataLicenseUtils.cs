﻿using System;
using System.Text.Json;
using Lusid.FinDataEx.DataLicense.Service;
using PerSecurity_Dotnet;
using Polly;
using Polly.Wrap;

namespace Lusid.FinDataEx.DataLicense.Util
{
    public static class DataLicenseUtils
    {
        public static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(30);
        public static readonly int DefaultPollingAttempts = 2;

        /// <summary>
        /// Poll for data availability. As per BBG DL sample recommendation.
        /// Beware amending the poll interval due to BBG limitations. Especially in TEST.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pollingInterval"></param>
        /// <returns></returns>
        //
        public static PolicyWrap<T> GetBBGRetryPolicy<T>(TimeSpan? pollingInterval = null) where T : PerSecurityResponse
        {
            var fallback = Policy
                .HandleResult<T>(response => response.statusCode.code == DataLicenseService.DataNotAvailable)
                .Fallback(() => throw new TimeoutException($"Failed to poll for BBG response - Status code was {DataLicenseService.DataNotAvailable}"));

            var waitAndRetry = Policy
                .HandleResult<T>(response => response.statusCode.code == DataLicenseService.DataNotAvailable)
                .WaitAndRetry(DefaultPollingAttempts, _ => pollingInterval ?? DefaultPollingInterval);

            return Policy.Wrap(fallback, waitAndRetry);
        }

        /// <summary>
        /// Print the BBG DL response as JSON to allow the response to be reconstructed
        /// in a test environment for debugging purposes
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <typeparam name="T"></typeparam>
        public static void PrintJsonResponse<T>(T response)
        {
            var bbgResponseAsJson = JsonSerializer.Serialize(response);
            Console.WriteLine("Response as Json for debugging:");
            Console.WriteLine(bbgResponseAsJson);
        }

        /// <summary>
        /// Utility method to print out response of a BBG DLWS GetData call.
        ///
        /// NOTE : This code is copied straight from c# BBG samples and so remains untouched.
        /// </summary>
        public static void PrintGetDataResponse(RetrieveGetDataResponse retrieveGetDataResponse)
        {
            // Code taken from BBG DL Samples
            if (retrieveGetDataResponse.statusCode.code == DataLicenseService.Success)
            {
                // Displaying the RetrieveGetDataResponse
                for (int i = 0; i < retrieveGetDataResponse.instrumentDatas.Length; i++)
                {
                    Console.WriteLine("Data for :" + retrieveGetDataResponse.instrumentDatas[i].instrument.id +
                                      "  " + retrieveGetDataResponse.instrumentDatas[i].instrument.yellowkey);
                    for (int j = 0; j < retrieveGetDataResponse.instrumentDatas[i].data.Length; j++)
                    {
                        if (retrieveGetDataResponse.instrumentDatas[i].data[j].isArray == true)
                        {
                            // In case this is a bulk field request
                            for (int k = 0; k < retrieveGetDataResponse.instrumentDatas[i].data[j].bulkarray.Length; k++)
                            {
                                Console.WriteLine("-------------------------");
                                for (int l = 0; l < retrieveGetDataResponse.instrumentDatas[i].data[j].
                                    bulkarray[k].data.Length; l++)
                                {
                                    Console.WriteLine(retrieveGetDataResponse.
                                        instrumentDatas[i].data[j].bulkarray[k].data[l].value);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("	" + retrieveGetDataResponse.fields[j] + " : " + retrieveGetDataResponse.
                                instrumentDatas[i].data[j].value);
                        }
                    }
                }
            }
            else 
                Console.WriteLine($"Error in the submitted request with status code {retrieveGetDataResponse.statusCode.code}");
        }

        /// <summary>
        /// Utility method to print out response of a BBG DLWS GetActions call.
        ///
        /// NOTE : This code is copied straight from c# BBG samples and so remains untouched.
        /// </summary>
        public static void PrintGetActionsResponse(RetrieveGetActionsResponse retrieveGetActionsResponse)
        {
            if (retrieveGetActionsResponse.statusCode.code == DataLicenseService.Success)
            {
                Console.WriteLine("Retrieve get quotes request successful.  Response ID:" + retrieveGetActionsResponse.responseId);
                for (int i = 0; i < retrieveGetActionsResponse.instrumentDatas.Length; i++)
                {
                    Console.WriteLine("Data for :"
                                      + retrieveGetActionsResponse.instrumentDatas[i].instrument.id + " "
                                      + retrieveGetActionsResponse.instrumentDatas[i].instrument.yellowkey
                    );
                    Console.WriteLine(", Company id = " + retrieveGetActionsResponse.instrumentDatas[i].standardFields.companyId.ToString());
                    Console.WriteLine(", Security id = " + retrieveGetActionsResponse.instrumentDatas[i].standardFields.securityId.ToString());
                    if (retrieveGetActionsResponse.instrumentDatas[i].data != null)
                    {
                        for (int j = 0; j < retrieveGetActionsResponse.instrumentDatas[i].data.Length; j++)
                        {
                            Console.WriteLine(": field = "
                                              + retrieveGetActionsResponse.instrumentDatas[i].data[j].field
                                              + ", value = "
                                              + retrieveGetActionsResponse.instrumentDatas[i].data[j].value);
                        }
                    }

                }
            }
            else if (retrieveGetActionsResponse.statusCode.code == DataLicenseService.RequestError)
                Console.WriteLine("Error in submitted request");
        }

    }
}