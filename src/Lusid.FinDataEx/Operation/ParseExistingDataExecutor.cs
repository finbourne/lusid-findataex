using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Input;
using Lusid.FinDataEx.Util;
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

        /// <summary>
        /// Select an input reader to take BBG data from
        /// </summary>
        /// <param name="getOptions"></param>
        /// <returns></returns>
        private IInputReader CreateFinDataInputReader(DataLicenseOptions getOptions) => getOptions.InputSource switch
        {
            InputType.Drive => new LusidDriveInputReader(getOptions, _driveApiFactory),
            InputType.Local => new LocalFilesystemInputReader(getOptions),
            _ => throw new ArgumentNullException($"No input readers for input type {getOptions.InputSource}")
        };
    }
}