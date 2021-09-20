using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util.FileUtils;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using System;

namespace Lusid.FinDataEx.Util
{
    public class FileHandlerFactory : IFileHandlerFactory
    {
        private readonly ILusidApiFactory _driveApiFactory;

        public FileHandlerFactory(ILusidApiFactory driveApiFactory)
        {
            _driveApiFactory = driveApiFactory;
        }

        public IFileHandler Build(FileHandlerType fileHandlerType) => fileHandlerType switch
        {
            FileHandlerType.Lusid => new LusidDriveFileHandler(_driveApiFactory),
            FileHandlerType.Local => new LocalFileHandler(),
            _ => throw new ArgumentNullException($"No handler for fileHandlerType {fileHandlerType}")
        };
    }
}