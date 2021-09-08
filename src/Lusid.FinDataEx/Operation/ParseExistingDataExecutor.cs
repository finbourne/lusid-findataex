using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Input;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileHandler;
using System;

namespace Lusid.FinDataEx.Operation
{
    public class ParseExistingDataExecutor : IOperationExecutor
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly ILusidApiFactory _driveApiFactory;

        public ParseExistingDataExecutor(DataLicenseOptions getOptions, ILusidApiFactory driveApiFactory)
        {
            _getOptions = getOptions;
            _driveApiFactory = driveApiFactory;
        }

        public DataLicenseOutput Execute()
        {
            return CreateFinDataInputReader(_getOptions).Read();
        }

        private IInputReader CreateFinDataInputReader(DataLicenseOptions getOptions) => getOptions.InputSource switch
        {
            InputType.Drive => new FileInputReader(getOptions, new LusidDriveFileHandler(_driveApiFactory)),
            InputType.Local => new FileInputReader(getOptions, new LocalFileHandler()),
            _ => throw new ArgumentNullException($"No input readers for input type {getOptions.InputSource}")
        };
    }
}