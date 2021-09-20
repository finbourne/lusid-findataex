using System;
using CommandLine;
using Lusid.FinDataEx.Operation;
using Lusid.FinDataEx.Output;
using Lusid.FinDataEx.Util;
using Lusid.FinDataEx.Util.FileUtils;

namespace Lusid.FinDataEx
{
    public class FinDataEx
    {
        private const int SuccessExitCode = 0;
        private const int BadArgExitCode = 1;
        private const int ProcessingExitCode = 1;

        private const string secretsJsonFilename = "secrets.json";

        private static readonly Sdk.Utilities.ILusidApiFactory LusidApiFactory;
        private static readonly Drive.Sdk.Utilities.ILusidApiFactory DriveApiFactory;
        private static readonly FileHandlerFactory FileHandlerFactory;

        static FinDataEx()
        {
            LusidApiFactory = Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);
            DriveApiFactory = Drive.Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);
            FileHandlerFactory = new FileHandlerFactory(DriveApiFactory);
        }

        public static int Main(string[] args)
        {
            try
            {
                var parserResult = Parser.Default
                    .ParseArguments<GetDataOptions, GetActionsOptions>(args)
                    .WithParsed<GetDataOptions>(Execute)
                    .WithParsed<GetActionsOptions>(Execute);

                if (parserResult.Tag == ParserResultType.NotParsed)
                {
                    Console.WriteLine("FinDataExt program arguments could not be parsed. Check above logs for details. Exiting FinDataEx.");
                    return BadArgExitCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"FinDataEx request processing failed. Exiting FinDataEx. Exception details : {e}");
                return ProcessingExitCode;
            }

            Console.WriteLine($"FinDataEx run to successful completion with exit code {SuccessExitCode}");
            return SuccessExitCode;
        }

        /// <summary>
        /// Execute a read from an input source and persists the output.
        /// </summary>
        /// <param name="getOptions"></param>
        private static void Execute(DataLicenseOptions getOptions)
        {
            // construct the reader to get BBG data
            var finDataExecutor = CreateFinDataOperationExecutor(getOptions);
            var dataLicenseOutput = finDataExecutor.Execute();

            // construct the writer to persist any data retrieved from BBG
            var finDataOutputWriter = CreateFinDataOutputWriter(getOptions);
            var writeResult = finDataOutputWriter.Write(dataLicenseOutput);

            ProcessWriteResult(writeResult);
        }

        /// <summary>
        /// Select an input reader to take BBG data from
        /// </summary>
        /// <param name="getOptions"></param>
        /// <returns></returns>
        private static IOperationExecutor CreateFinDataOperationExecutor(DataLicenseOptions getOptions) => getOptions.OperationType switch
        {
            OperationType.ParseExisting => new ParseExistingDataExecutor(getOptions, FileHandlerFactory),
            OperationType.BloombergRequest => throw new NotImplementedException("Due to licensing issues, this operation can not run at this time. " +
                                                                                "Please use another operation type"),
            _ => throw new ArgumentNullException($"No input readers for operation type {getOptions.OperationType}")
        };

        /// <summary>
        /// Select and construct an output writer to store the returned data from a DLWS call.
        /// </summary>
        /// <param name="getOptions"></param>
        /// <returns></returns>
        private static IOutputWriter CreateFinDataOutputWriter(DataLicenseOptions getOptions) => getOptions.OutputTarget switch
        {
            OutputType.Local => new FileOutputWriter(getOptions, FileHandlerFactory.Build(FileHandlerType.Local)),
            OutputType.Drive => new FileOutputWriter(getOptions, FileHandlerFactory.Build(FileHandlerType.Lusid)),
            OutputType.Lusid => new LusidTenantOutputWriter(getOptions, LusidApiFactory),
            _ => throw new ArgumentNullException($"No output writer for operation type {getOptions.OutputTarget}")
        };

        /// <summary>
        /// Log results of BBG response write.
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