using System.IO;
using Newtonsoft.Json;

namespace Lusid.FinDataEx.Core
{
    public class FdeRequestBuilder
    {
        /// <summary>
        ///  Construct a FdeRequest from a json file
        /// </summary>
        /// <param name="requestPath"> location of the json file FdeRequest</param>
        /// <returns></returns>
        public FdeRequest LoadFromFile(string requestPath)
        {
            FdeRequest fdeRequest = JsonConvert.DeserializeObject<FdeRequest>(File.ReadAllText(requestPath));
            return fdeRequest;
        }
    }
}