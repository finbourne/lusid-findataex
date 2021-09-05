using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Lusid.FinDataEx.Input;
using Lusid.FinDataEx.Output;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        private const int SuExitCode = 0;
        private const int FaBadArgExitCode = 1;
        private const int FaProcessingExitCode = 1;

        private const string secretsJsonFilename = "secrets.json";
        private static readonly Sdk.Utilities.ILusidApiFactory LusidApiFactory = Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);
        private static readonly Drive.Sdk.Utilities.ILusidApiFactory DriveApiFactory = Drive.Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);

        public static int Main(string[] args)
        {
            try
            {
                var parserResult = Parser.Default.ParseArguments<GetDataOptions, GetActionsOptions>(args)
                    .WithParsed<GetDataOptions>(Execute)
                    .WithParsed<GetActionsOptions>(Execute);

                if (parserResult.Tag == ParserResultType.NotParsed)
                {
                    Console.WriteLine("FinDataExt program arguments could not be parsed. Check above logs for details. Exiting FinDataEx.");
                    return FaBadArgExitCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"FinDataEx request processing failed. Exiting FinDataEx. Exception details : {e}");
                return FaProcessingExitCode;
            }
            Console.WriteLine($"FinDataEx run to successful completion with exit code {SuExitCode}");
            return SuExitCode;
        }

        /// <summary>
        /// Execute a dread from an input source and persists the output.
        /// </summary>
        /// <param name="getOptions"></param>
        private static void Execute(DataLicenseOptions getOptions)
        {
            // construct the reader to get BBG data
            var finDataInputReader = CreateFinDataInputReader(getOptions);
            var dataLicenseOutput = finDataInputReader.Read();

            // construct the writer to persist any data retrieved from BBG
            var finDataOutputWriters = CreateFinDataOutputWriter(getOptions);
            foreach (var finDataOutputWriter in finDataOutputWriters)
            {
                var writeResult = finDataOutputWriter.Write(dataLicenseOutput);
                ProcessWriteResult(writeResult);
            }
        }

        /// <summary>
        /// Select an input reader to take BBG data from
        /// </summary>
        /// <param name="getOptions"></param>
        /// <returns></returns>
        private static IInputReader CreateFinDataInputReader(DataLicenseOptions getOptions)
        {
            if (!string.IsNullOrWhiteSpace(getOptions.BBGSource))
            {
                return new DataLicenseReader(getOptions, LusidApiFactory, DriveApiFactory);
            }
            else if (!string.IsNullOrWhiteSpace(getOptions.InstrumentSource))
            {
                return new FileReader(getOptions, DriveApiFactory);
            }
            else
            {
                throw new ArgumentNullException("No input readers");
            }
        }

        /// <summary>
        /// Select and construct an output writer to store the returned data from a DLWS call.
        /// </summary>
        /// <param name="getOptions"></param>
        /// <returns></returns>
        private static List<IOutputWriter> CreateFinDataOutputWriter(DataLicenseOptions getOptions)
        {
            var outputWriters = new List<IOutputWriter>();
            if (!string.IsNullOrWhiteSpace(getOptions.OutputLocal))
            {
                outputWriters.Add(new LocalFilesystemOutputWriter(getOptions));
            }
            if (!string.IsNullOrWhiteSpace(getOptions.OutputDrive))
            {
                outputWriters.Add(new LusidDriveOutputWriter(getOptions, DriveApiFactory));
            }
            if (!string.IsNullOrWhiteSpace(getOptions.OutputLusid))
            {
                outputWriters.Add(new LusidTenantOutputWriter(getOptions, LusidApiFactory));
            }

            if (!outputWriters.Any())
            {
                throw new ArgumentNullException("No output writers provided");
            }

            return outputWriters;
        }

        /// <summary>
        ///  Log results of BBG response write.
        /// </summary>
        /// <param name="writeResult"></param>
        private static void ProcessWriteResult(WriteResult writeResult)
        {
            if (writeResult.Status != WriteResultStatus.Ok)
            {
                Console.Error.WriteLine("FinDataEx request completed with failures...");
                throw new Exception(writeResult.ToString());
            }
            Console.WriteLine($"FinDataEx request completed and output written to {writeResult.FileOutputPath}");
        }
    }
}