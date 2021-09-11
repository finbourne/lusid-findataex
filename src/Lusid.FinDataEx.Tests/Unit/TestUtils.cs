using System.IO;
using Newtonsoft.Json;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Unit
{
    public static class TestUtils
    {
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