using System;
using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.Vendor.Dl;

namespace Lusid.FinDataEx.Util
{
    public static class FdeRequestUtils
    {
        
        public const string ConUrl = "url";
        public const string ConUser = "user";
        public const string ConPass = "password";
        public const string ConType = "type";
        public const string ReqSourceData = "sourceData";
        public const string ReqRequestType = "requestType";

        public static string GetConnectorConfigParameter(Dictionary<string, object> connectorConfig, string parameterType)
        {
            if (connectorConfig.TryGetValue(parameterType, out object configParameter))
            {
                return (string) configParameter;
            }
            throw new InvalidDataException($"Dl request connector configuration has no \"{parameterType}\" defined");
        }
        
        public static T GetRequestBodyParameter<T>(Dictionary<string, object> requestBody, string parameterType)
        {
            if (requestBody.TryGetValue(parameterType, out object requestBodyParameter))
            {
                if (parameterType == ReqRequestType)
                {
                    return (T) Enum.Parse(typeof(DlRequestType), (string) requestBodyParameter);
                }
                return (T) requestBodyParameter;
            }
            throw new InvalidDataException($"Dl request request body has no \"{parameterType}\" defined");
        }
    }
}