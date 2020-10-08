using System.IO;
using Newtonsoft.Json;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Unit
{
    public static class TestUtils
    {
        
        public static RetrieveGetDataResponse LoadResponseFromFile(string responseId)
        {
            string responsePath = Path.Combine(new[]{"Unit","DataLicense","Service","Call","TestData",$"{responseId}.json"});
            RetrieveGetDataResponse retrieveGetDataResponse =  JsonConvert.DeserializeObject<RetrieveGetDataResponse>(File.ReadAllText(responsePath));
            return retrieveGetDataResponse;
        }
        
    }
}