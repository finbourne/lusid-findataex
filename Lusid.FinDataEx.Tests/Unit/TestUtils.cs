using System;
using System.IO;
using Newtonsoft.Json;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Unit
{
    public static class TestUtils
    {
        public static void SetupTempTestDirectory(String tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Console.WriteLine($"Test directory {tempOutputDir} exists but it should have been removed during" +
                                  $" test tear down on a previous run. Deleting {tempOutputDir} to continue tests.");
                Directory.Delete(tempOutputDir, true);
            } 
            Directory.CreateDirectory(tempOutputDir);
        }

        public static void TearDownTempTestDirectory(String tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Directory.Delete(tempOutputDir, true);
            }
        }
        
        public static RetrieveGetDataResponse LoadResponseFromFile(string responseId)
        {
            var responsePath = Path.Combine(new[]{"Unit","DataLicense","Service","Call","TestData",$"{responseId}.json"});
            var retrieveGetDataResponse =  JsonConvert.DeserializeObject<RetrieveGetDataResponse>(File.ReadAllText(responsePath));
            return retrieveGetDataResponse;
        }
        
    }
}