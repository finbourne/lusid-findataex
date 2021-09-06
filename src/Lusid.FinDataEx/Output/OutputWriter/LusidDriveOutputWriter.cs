﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util;

namespace Lusid.FinDataEx.Output
{
    public class LusidDriveOutputWriter : LocalFilesystemOutputWriter
    {
        public const string OutputFileEntrySeparator = "\n";

        private readonly IFilesApi _filesApi;

        public LusidDriveOutputWriter(DataLicenseOptions getOptions, ILusidApiFactory factory) : base(getOptions)
        {
            _filesApi = factory.Api<IFilesApi>();
        }

        protected override string WriteToFile(string modifiedFilepath, IEnumerable<string> finDataRecords)
        {
            // convert records to byte array for upload into LUSID drive
            var finDataEntriesStr = string.Join(OutputFileEntrySeparator, finDataRecords);
            var finDataEntriesBytes = Encoding.UTF8.GetBytes(finDataEntriesStr);

            // upload to drive
            var lusidDriveFilename = modifiedFilepath.Split(LusidDriveUtils.LusidDrivePathSeparator).Last();
            var lusidDriveFolderPath =
                string.Join(LusidDriveUtils.LusidDrivePathSeparator, modifiedFilepath.Split(LusidDriveUtils.LusidDrivePathSeparator).SkipLast(1));
            Console.WriteLine($"Attempting to write to LUSID drive filename={lusidDriveFilename} in folder={lusidDriveFolderPath}.");
            var upload = _filesApi.CreateFile(lusidDriveFilename, lusidDriveFolderPath, finDataEntriesBytes.Length, finDataEntriesBytes);
            Console.WriteLine($"Completed write to LUSID drive for filename={lusidDriveFilename}, " +
                              $"folder={lusidDriveFolderPath}. Output file id={upload.Id} with size={upload.Size}");

            // return file id which will be required to reference file going forward
            return upload.Id;
        }
    }
}