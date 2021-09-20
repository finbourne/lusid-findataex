using System;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using PerSecurity_Dotnet;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Vendor;
using Lusid.FinDataEx.Util.FileUtils;
using Lusid.Sdk.Utilities;
using Lusid.FinDataEx.DataLicense.Service.Transform;

namespace Lusid.FinDataEx.Operation
{
    public class DataLicenseRequestExecutor : IOperationExecutor
    {
        private readonly DataLicenseOptions _getOptions;
        private readonly ILusidApiFactory _lusidApiFactory;
        private readonly IFileHandlerFactory _fileHandlerFactory;
        private readonly IDataLicenseService _dataLicenseService;
        private readonly IPerSecurityWsFactory _perSecurityWsFactory;
        private readonly ITransformerFactory _transformerFactory;

        public DataLicenseRequestExecutor(
            DataLicenseOptions getOptions,
            ILusidApiFactory lusidApiFactory,
            IFileHandlerFactory fileHandlerFactory,
            IDataLicenseService dataLicenseService,
            IPerSecurityWsFactory perSecurityWsFactory,
            ITransformerFactory transformerFactory)
        {
            _getOptions = getOptions;
            _lusidApiFactory = lusidApiFactory;
            _fileHandlerFactory = fileHandlerFactory;
            _dataLicenseService = dataLicenseService;
            _perSecurityWsFactory = perSecurityWsFactory;
            _transformerFactory = transformerFactory;
        }

        public DataLicenseOutput Execute()
        {
            var instruments = CreateInstruments(_getOptions);

            return new DataLicenseInputReader(_getOptions, instruments, _dataLicenseService, _perSecurityWsFactory, _transformerFactory).Read();
        }

        private Instruments CreateInstruments(DataLicenseOptions dataOptions)
        {
            var instrumentSource = CreateInstrumentSource(dataOptions);
            var instruments = instrumentSource.Get();

            if (!instruments.instrument.Any())
            {
                throw new ArgumentException("No DL instruments could be created from the instruments or portfolios provided. " +
                                            "No DLWS call will be executed.");
            }

            if (instruments.instrument.Length > dataOptions.MaxInstruments)
            {
                throw new ArgumentException($"Breach maximum instrument limit. Attempted to request " +
                                            $"{instruments.instrument.Length} instruments but only {dataOptions.MaxInstruments} are allowed. " +
                                            $"To increase the limit override the max allowed instruments with the -m argument parameter.");
            }

            return instruments;
        }

        private IInstrumentSource CreateInstrumentSource(DataLicenseOptions dataOptions) => dataOptions.InputSource switch
        {
            InputType.CLI => new CliInstrumentSource(dataOptions),
            InputType.Lusid => new LusidPortfolioInstrumentSource(dataOptions, _lusidApiFactory),
            InputType.Local => new FileInstrumentSource(dataOptions, _fileHandlerFactory.Build(FileHandlerType.Local)),
            InputType.Drive => new FileInstrumentSource(dataOptions, _fileHandlerFactory.Build(FileHandlerType.Lusid)),
            _ => throw new ArgumentOutOfRangeException($"No instrument sources for input type {dataOptions.InputSource}")
        };
    }
}