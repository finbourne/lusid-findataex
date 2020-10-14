using System;
using System.IO;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Client;
using Lusid.Drive.Sdk.Utilities;

namespace Lusid.FinDataEx.Util
{
    public class LusidDriveUtils
    {
        public static byte[] LoadFileFromLusidDrive(string lusidDriveFileId)
        {
            ILusidApiFactory factory = LusidApiFactoryBuilder.Build("secrets.json");
            IFilesApi filesApi = factory.Api<IFilesApi>();
            try
            {
                Console.WriteLine($"Loading byte[] from lusidDriveFileId={lusidDriveFileId}");
                Stream responseDlFileStream = filesApi.DownloadFile(lusidDriveFileId);
                MemoryStream ms = new MemoryStream();
                responseDlFileStream.CopyTo(ms);
                return ms.ToArray();
            }
            catch (ApiException e)
            {
                Console.WriteLine($"Error in retrieving byte[] for lusidDriveFileId={lusidDriveFileId}");
                Console.WriteLine($"Exception for lusidDriveFileId={lusidDriveFileId} error code={e.ErrorCode}");
                Console.WriteLine($"Exception for lusidDriveFileId={lusidDriveFileId} error content={e.ErrorContent}");
                throw;
            }
            
        }
        
        
    }
}