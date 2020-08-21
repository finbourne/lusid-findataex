using System;
using System.IO;

namespace Lusid.FinDataEx.Tests.Vendor.Util
{
    public static class TestUtils
    {

        public static void SetupTempTestDirectory(String tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Console.WriteLine($"Test directory {tempOutputDir} exists but it should have been removed during" +
                                  $" test tear down on a previous run. Deleting {tempOutputDir} to continue tests.");
                Directory.Delete(tempOutputDir, true);
            } 
            Directory.CreateDirectory(tempOutputDir);
        }

        public static void TearDownTempTestDirectory(String tempOutputDir)
        {
            if (Directory.Exists(tempOutputDir))
            {
                Directory.Delete(tempOutputDir, true);
            }
        }
        
    }
}