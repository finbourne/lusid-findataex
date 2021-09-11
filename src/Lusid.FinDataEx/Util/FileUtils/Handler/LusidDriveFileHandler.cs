using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lusid.FinDataEx.Util.FileUtils.Handler
{
    public class LusidDriveFileHandler : IFileHandler
    {
        public const string LusidDrivePathSeparator = "/";

        private readonly IFilesApi _filesApi;
        private readonly ISearchApi _searchApi;

        public LusidDriveFileHandler(ILusidApiFactory driveApiFactory)
        {
            _filesApi = driveApiFactory.Api<IFilesApi>();
            _searchApi = driveApiFactory.Api<ISearchApi>();
        }

        public bool Exists(string path) => string.IsNullOrWhiteSpace(ValidatePath(path));

        public string ValidatePath(string path)
        {
            var (directoryName, fileName) = PathToFolderAndFile(path);

            return _searchApi.Search(new SearchBody(directoryName, fileName)).Values.SingleOrDefault()?.Id;
        }

        public List<string> Read(string path, char entrySeparator) => new StreamReader(_filesApi.DownloadFile(path))
            .ReadToEnd()
            .Split(entrySeparator)
            .ToList();

        public string Write(string path, List<string> data, char entrySeparator)
        {
            var (lusidDriveFilename, lusidDriveFolderPath) = PathToFolderAndFile(path);
            var dataString = string.Join(entrySeparator, data);
            var dataBytes = Encoding.UTF8.GetBytes(dataString);

            var storage = _filesApi.CreateFile(lusidDriveFilename, lusidDriveFolderPath, dataBytes.Length, dataBytes);
            return storage.Id;
        }

        public Tuple<string, string> PathToFolderAndFile(string filepath)
        {
            var splitPath = filepath.Split(LusidDrivePathSeparator);

            // File is in root folder
            if (splitPath.Length < 2)
            {
                return Tuple.Create("", filepath);
            }

            // Split into folder path and file name
            return Tuple.Create(
                string.Join(LusidDrivePathSeparator, splitPath.Take(splitPath.Length - 1)),
                splitPath.Last());
        }
    }
}