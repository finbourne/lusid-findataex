using System;
using System.IO;

namespace Lusid.FinDataEx.Tests.Integration
{
    public static class TestUtils
    {
        private const string secretsJsonFilename = "secrets.json";
        public static readonly Sdk.Utilities.ILusidApiFactory LusidApiFactory = Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);
        public static readonly Drive.Sdk.Utilities.ILusidApiFactory DriveApiFactory = Drive.Sdk.Utilities.LusidApiFactoryBuilder.Build(secretsJsonFilename);

        public static void SetupTempTestDirectory(string tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Console.WriteLine($"Test directory {tempOutputDir} exists but it should have been removed during" +
                                  $" test tear down on a previous run. Deleting {tempOutputDir} to continue tests.");
                Directory.Delete(tempOutputDir, true);
            }
            Directory.CreateDirectory(tempOutputDir);
        }

        public static void TearDownTempTestDirectory(string tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Directory.Delete(tempOutputDir, true);
            }
        }
    }
}