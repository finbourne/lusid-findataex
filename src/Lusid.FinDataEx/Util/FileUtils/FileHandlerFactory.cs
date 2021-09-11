using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util.FileUtils;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using System;

namespace Lusid.FinDataEx.Util
{
    public class FileHandlerFactory : IFileHandlerFactory
    {
        public IFileHandler Build(FileHandlerType fileHandlerType, ILusidApiFactory driveApiFactory) => fileHandlerType switch
        {
            FileHandlerType.Lusid => new LusidDriveFileHandler(driveApiFactory),
            FileHandlerType.Local => new LocalFileHandler(),
            _ => throw new ArgumentNullException($"No handler for fileHandlerType {fileHandlerType}")
        };
    }
}