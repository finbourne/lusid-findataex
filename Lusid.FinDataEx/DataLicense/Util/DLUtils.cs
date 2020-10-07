using System;
using Lusid.FinDataEx.DataLicense.Service;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Util
{
    public class DlUtils
    {

        public static void PrintGetDataResponse(RetrieveGetDataResponse retrieveGetDataResponse)
        {
            // Code taken from BBG DL Samples
            if (retrieveGetDataResponse.statusCode.code == DLDataService.Success)
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
        
    }
}