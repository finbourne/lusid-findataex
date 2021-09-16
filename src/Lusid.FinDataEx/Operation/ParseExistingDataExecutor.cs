using Lusid.FinDataEx.Input;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils;
using System;

namespace Lusid.FinDataEx.Operation
{
    public class ParseExistingDataExecutor : IOperationExecutor
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly IFileHandlerFactory _fileHandlerFactory;

        public ParseExistingDataExecutor(DataLicenseOptions getOptions, IFileHandlerFactory fileHandlerFactory)
        {
            _getOptions = getOptions;
            _fileHandlerFactory = fileHandlerFactory;
        }

        public DataLicenseOutput Execute()
        {
            return CreateFinDataInputReader(_getOptions).Read();
        }

        private IInputReader CreateFinDataInputReader(DataLicenseOptions getOptions) => getOptions.InputSource switch
        {
            InputType.Drive => new FileInputReader(getOptions, _fileHandlerFactory.Build(FileHandlerType.Lusid)),
            InputType.Local => new FileInputReader(getOptions, _fileHandlerFactory.Build(FileHandlerType.Local)),
            _ => throw new ArgumentNullException($"No input readers for input type {getOptions.InputSource}")
        };
    }
}