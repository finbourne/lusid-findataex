using System;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using PerSecurity_Dotnet;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Vendor;
using Lusid.FinDataEx.Util.FileUtils.Handler;

namespace Lusid.FinDataEx.Operation
{
    public class DataLicenseRequestExecutor : IOperationExecutor
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly Sdk.Utilities.ILusidApiFactory _lusidApiFactory;
        private readonly Drive.Sdk.Utilities.ILusidApiFactory _driveApiFactory;

        public DataLicenseRequestExecutor(DataLicenseOptions getOptions, Sdk.Utilities.ILusidApiFactory lusidApiFactory, Drive.Sdk.Utilities.ILusidApiFactory driveApiFactory)
        {
            _getOptions = getOptions;
            _lusidApiFactory = lusidApiFactory;
            _driveApiFactory = driveApiFactory;
        }

        public DataLicenseOutput Execute()
        {
            throw new NotImplementedException("Due to licensing issues, this operation can not run at this time. " +
                                              "Please use another operation type");

            // construct instruments in DL format to be passed to DLWS
            var instruments = CreateInstruments(_getOptions);
            if (!instruments.instrument.Any())
            {
                Console.WriteLine("No instruments were constructed from your selected source and arguments. " +
                                  "No DLWS call will be executed");
                return DataLicenseOutput.Empty();
            }

            return new DataLicenseInputReader(_getOptions, instruments, new DataLicenseService(), new PerSecurityWsFactory()).Read();
        }

        private Instruments CreateInstruments(DataLicenseOptions dataOptions)
        {
            var instrumentSource = CreateInstrumentSource(dataOptions);
            var instruments = instrumentSource.Get();
            if (instruments is { } dlInstruments)
            {
                // check instruments in request does not exceed the allowed limit
                if (dlInstruments.instrument.Length > dataOptions.MaxInstruments)
                {
                    throw new ArgumentException($"Breach maximum instrument limit. Attempted to request" +
                                                $" {dlInstruments.instrument.Length} instruments but only {dataOptions.MaxInstruments} are allowed. " +
                                                $"To increase the limit override the max allowed instruments with the -m argument parameter.");
                }
                return dlInstruments;
            }

            throw new ArgumentException($"No DL instruments could be created from the instruments or " +
                                        $"portfolios provided. Check portfolios provided have existing holdings or" +
                                        $" the instruments arguments are legal . Inputs={dataOptions}");
        }

        private IInstrumentSource CreateInstrumentSource(DataLicenseOptions dataOptions)
        {
            var instrumentArgs = InstrumentArgs.Create(dataOptions);
            return dataOptions.InputSource switch
            {
                InputType.CLI => CliInstrumentSource.Create(instrumentArgs, dataOptions.InstrumentSourceArguments),
                InputType.Lusid => LusidPortfolioInstrumentSource.Create(_lusidApiFactory, instrumentArgs, dataOptions.InstrumentSourceArguments),
                InputType.Local => FileInstrumentSource.Create(new LocalFileHandler(), instrumentArgs, dataOptions.InstrumentSourceArguments),
                InputType.Drive => FileInstrumentSource.Create(new LusidDriveFileHandler(_driveApiFactory), instrumentArgs, dataOptions.InstrumentSourceArguments),
                _ => throw new ArgumentOutOfRangeException($"No instrument sources for input type {dataOptions.InputSource}")
            };
        }
    }
}