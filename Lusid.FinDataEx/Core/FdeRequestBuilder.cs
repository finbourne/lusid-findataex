using System.IO;
using Newtonsoft.Json;

namespace Lusid.FinDataEx.Core
{
    public class FdeRequestBuilder
    {
        /// <summary>
        ///  Construct a FdeRequest from a json file
        /// </summary>
        /// <param name="requestUrl"> location of the json file FdeRequest</param>
        /// <returns></returns>
        public FdeRequest LoadFromFile(string requestUrl)
        {
            FdeRequest fdeRequest = JsonConvert.DeserializeObject<FdeRequest>(File.ReadAllText(requestUrl));
            return fdeRequest;
        }
    }
}