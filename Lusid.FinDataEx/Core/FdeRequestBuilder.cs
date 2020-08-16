using System.IO;
using Newtonsoft.Json;

namespace Lusid.FinDataEx.Core
{
    public class FdeRequestBuilder
    {
        public FdeRequest LoadFromFile(string requestUrl)
        {
            FdeRequest fdeRequest = JsonConvert.DeserializeObject<FdeRequest>(File.ReadAllText(requestUrl));
            return fdeRequest;
        }
        
        public FdeRequest LoadFromJson(string requestJson)
        {
            //Newton soft load from json in dynamic
            return null;
        }
    }
}