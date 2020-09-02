using System;
using System.IO;
using System.Linq;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;
using Newtonsoft.Json;

namespace Lusid.FinDataEx.Core
{
    public class FdeRequestBuilder
    {
        
        /// <summary>
        ///  Construct an FdeRequest from a json file
        /// </summary>
        /// <param name="requestPath"> location of the json file FdeRequest</param>
        /// <returns></returns>
        public FdeRequest LoadFromFile(string requestPath)
        {
            FdeRequest fdeRequest = JsonConvert.DeserializeObject<FdeRequest>(File.ReadAllText(requestPath));
            return fdeRequest;
        }

        /// <summary>
        ///  Construct an FdeRequest from a json file that exists on LUSID drive
        /// </summary>
        /// <param name="lusidDriveFileId">lusid drive file id</param>
        /// <returns></returns>
        public FdeRequest LoadFromLusidDrive(string lusidDriveFileId)
        {
            ILusidApiFactory factory = LusidApiFactoryBuilder.Build("secrets.json");
            IFilesApi filesApi = factory.Api<IFilesApi>();
            Stream responseDlFileStream = filesApi.DownloadFile(lusidDriveFileId);
            using (StreamReader sr = new StreamReader(responseDlFileStream))
            {
                FdeRequest fdeRequest = JsonConvert.DeserializeObject<FdeRequest>(sr.ReadToEnd());
                return fdeRequest;
            }
        }
    }
}