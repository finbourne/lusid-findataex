using System.Collections.Generic;
using System.IO;

namespace Lusid.FinDataEx.Util
{
    public static class FdeRequestUtils
    {
        
        public const string ConUrl = "url";
        public const string ConUser = "user";
        public const string ConPass = "password";
        public const string ConType = "type";
        public const string ReqSourceData = "sourceData";
        
        public static string GetConnectorConfigParameter(Dictionary<string, object> connectorConfig, string parameterType)
        {
            if (connectorConfig.TryGetValue(parameterType, out object configParameter))
            {
                return (string) configParameter;
            }
            throw new InvalidDataException($"Dl request connector configuration has no \"{parameterType}\" defined");
        }
        
        public static string GetRequestBodyParameter(Dictionary<string, object> requestBody, string parameterType)
        {
            if (requestBody.TryGetValue(parameterType, out object requestBodyParameter))
            {
                return (string) requestBodyParameter;
            }
            throw new InvalidDataException($"Dl request request body has no \"{parameterType}\" defined");
        }
    }
}