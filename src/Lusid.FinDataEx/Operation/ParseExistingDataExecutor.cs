using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Input;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils;
using System;

namespace Lusid.FinDataEx.Operation
{
    public class ParseExistingDataExecutor : IOperationExecutor
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly ILusidApiFactory _driveApiFactory;
        private readonly IFileHandlerFactory _fileHandlerFactory;

        public ParseExistingDataExecutor(DataLicenseOptions getOptions, ILusidApiFactory driveApiFactory, IFileHandlerFactory fileHandlerFactory)
        {
            _getOptions = getOptions;
            _driveApiFactory = driveApiFactory;
            _fileHandlerFactory = fileHandlerFactory;
        }

        public DataLicenseOutput Execute()
        {
            return CreateFinDataInputReader(_getOptions).Read();
        }

        private IInputReader CreateFinDataInputReader(DataLicenseOptions getOptions) => getOptions.InputSource switch
        {
            InputType.Drive => new FileInputReader(getOptions, _fileHandlerFactory.Build(FileHandlerType.Lusid, _driveApiFactory)),
            InputType.Local => new FileInputReader(getOptions, _fileHandlerFactory.Build(FileHandlerType.Local, _driveApiFactory)),
            _ => throw new ArgumentNullException($"No input readers for input type {getOptions.InputSource}")
        };
    }
}