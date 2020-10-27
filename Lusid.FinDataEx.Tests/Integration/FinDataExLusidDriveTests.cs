﻿using System;
using System.IO;
using System.Linq;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Output;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Integration
{
    public class FinDataExLusidDriveTests
    {
        private string _lusidOutputDirPath;
        private string _lusidOutputDirName;
        private ILusidApiFactory _factory;
        private IFoldersApi _foldersApi;
        private IFilesApi _filesApi;
        private string outputDirId;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _lusidOutputDirName = ("Test_Dir_FDE_" + Guid.NewGuid()).Substring(0,49);
            _lusidOutputDirPath = "/" + _lusidOutputDirName;
            _factory = LusidApiFactoryBuilder.Build("secrets.json");
            _foldersApi = _factory.Api<IFoldersApi>();
            _filesApi = _factory.Api<IFilesApi>();
        }
        
        [SetUp]
        public void SetUp()
        {
            // setup temp test folder in LUSID drive for each run
            outputDirId = _foldersApi.GetRootFolder(filter: $"Name eq '{_lusidOutputDirName}'").Values.SingleOrDefault()?.Id;
            var createFolder = new CreateFolder("/", _lusidOutputDirName);
            outputDirId ??= _foldersApi.CreateFolder(createFolder).Id;
        }
        
        [TearDown]
        public void TearDown()
        {
            // remove folders in drive at end of each test.
            // note if debugging ensure to clean lusid drive if terminate tests early
            _foldersApi.DeleteFolder(outputDirId);
        }

        [Test]
        public void FinDataEx_GetData_OnValidBbgId_ShouldProduceDataFile()
        {
            var commandArgs = $"GetData -i BBG000BPHFS9 BBG000BVPV84 -o {_lusidOutputDirPath} -f Lusid -d ID_BB_GLOBAL PX_LAST";
            FinDataEx.Main(commandArgs.Split(" "));

            //verify
            var priceEntries = GetFileAsStringsFromFolderInDrive(outputDirId);
            
            // check headers
            Assert.That(priceEntries[0], Is.EqualTo("ID_BB_GLOBAL|PX_LAST"));

            // check instrument 1 entry
            var instrumentEntry1 = priceEntries[1].Split("|");
            Assert.That(instrumentEntry1[0], Is.EqualTo("BBG000BPHFS9"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            
            // check instrument 2 entry
            var instrumentEntry2 = priceEntries[2].Split("|");
            Assert.That(instrumentEntry2[0], Is.EqualTo("BBG000BVPV84"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
        }

        private string[] GetFileAsStringsFromFolderInDrive(string lusdiDriveFolderId)
        {
            var contents = _foldersApi.GetFolderContents(lusdiDriveFolderId);
            // ensure only one file in folder otherwise test folder is contaminated and test corrupt
            Assert.That(contents.Values.Count, Is.EqualTo(1));
            
            var expectedPricesStorageObject = contents.Values[0];
            StringAssert.EndsWith("_GetData.csv", expectedPricesStorageObject.Name);
            
            return new StreamReader(_filesApi.DownloadFile(expectedPricesStorageObject.Id))
                .ReadToEnd()
                .Split(LusidDriveOutputWriter.OutputFileEntrySeparator);
        }
        
        
    }
}