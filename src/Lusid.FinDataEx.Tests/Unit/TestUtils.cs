using System;
using System.IO;
using Newtonsoft.Json;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Unit
{
    public static class TestUtils
    {
        private const string secretsJsonFilename = "secrets.json";
        public static readonly Sdk.Utilities.ILusidApiFactory LusidApiFactory = Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);
        public static readonly Drive.Sdk.Utilities.ILusidApiFactory DriveApiFactory = Drive.Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);

        public static void SetupTempTestDirectory(string tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Console.WriteLine($"Test directory {tempOutputDir} exists but it should have been removed during" +
                                  $" test tear down on a previous run. Deleting {tempOutputDir} to continue tests.");
                Directory.Delete(tempOutputDir, true);
            } 
            Directory.CreateDirectory(tempOutputDir);
        }

        public static void TearDownTempTestDirectory(string tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Directory.Delete(tempOutputDir, true);
            }
        }
        
        public static RetrieveGetDataResponse LoadGetDataResponseFromFile(string responseId)
        {
            var responsePath = Path.Combine(new[]{"Unit","DataLicense","Service","Call","TestData",$"{responseId}.json"});
            var retrieveGetDataResponse = JsonConvert.DeserializeObject<RetrieveGetDataResponse>(File.ReadAllText(responsePath));
            return retrieveGetDataResponse;
        }
        
        public static RetrieveGetActionsResponse LoadGetActionsResponseFromFile(string responseId)
        {
            var responsePath = Path.Combine(new[]{"Unit","DataLicense","Service","Call","TestData",$"{responseId}.json"});
            var retrieveGetActionsResponse = JsonConvert.DeserializeObject<RetrieveGetActionsResponse>(File.ReadAllText(responsePath));
            return retrieveGetActionsResponse;
        }
        
    }
}