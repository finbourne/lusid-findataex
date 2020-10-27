using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.Output;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Unit.TestUtils;

namespace Lusid.FinDataEx.Tests.Integration.Output
{
    [TestFixture]
    public class LocalFilesystemOutputWriterTests
    {
        
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExTests)}";
        private LocalFilesystemOutputWriter _outputWriter;
        [SetUp]
        public void SetUp()
        {
            SetupTempTestDirectory(_tempOutputDir);
            _outputWriter = new LocalFilesystemOutputWriter(_tempOutputDir);
        }
        
        [TearDown]
        public void TearDown()
        {
            TearDownTempTestDirectory(_tempOutputDir);
        }

        [Test]
        public void Write_OnValidFinData_ShouldWriteToOutputDir()
        {
            var finDataOutputs = new List<DataLicenseOutput>
            {
                CreateFinDataEntry("id_1_GetData"),
                CreateFinDataEntry("id_2_GetData")
            };

            var writeResult =  _outputWriter.Write(finDataOutputs);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            CollectionAssert.AreEqual(writeResult.FilesWritten, new List<string>(){"TempTestDir_FinDataExTests\\id_1_GetData.csv", "TempTestDir_FinDataExTests\\id_2_GetData.csv"});
            Assert.That(writeResult.FilesWritten[0], Does.Exist);
            Assert.That(writeResult.FilesWritten[1], Does.Exist);
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(writeResult.FilesWritten[0]);
            // check headers
            Assert.That(entries[0], Is.EqualTo("h1|h2|h3"));
            Assert.That(entries[1], Is.EqualTo("entry1Record1|entry2Record1|entry3Record1"));
            Assert.That(entries[2], Is.EqualTo("entry1Record2|entry2Record2|entry3Record2"));
        }
        
        [Test]
        public void Write_OnEmptyInput_ShouldWriteFileWithHeaderOnly()
        {
            var finDataOutputs = new List<DataLicenseOutput>();
            var headers = new List<string>{"h1","h2","h3"};
            var records = new List<Dictionary<string, string>>();
            finDataOutputs.Add(new DataLicenseOutput("id_GetData", headers, records));

            var writeResult =  _outputWriter.Write(finDataOutputs);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            
            // ensure file is properly populated
            var entries = File.ReadAllLines(_tempOutputDir + Path.DirectorySeparatorChar + "id_GetData.csv");
            // contains headers only
            Assert.That(entries.Length, Is.EqualTo(1));
            // check headers
            Assert.That(entries[0], Is.EqualTo("h1|h2|h3"));

        }
        
        [Test]
        public void Write_OnNoFinData_ShouldDoNothingButReturnOk()
        {
            var finDataOutputs = new List<DataLicenseOutput>();

            var writeResult =  _outputWriter.Write(finDataOutputs);
            
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.False(Directory.EnumerateFileSystemEntries(_tempOutputDir).Any());
        }
        
        [Test]
        public void Write_OnNonExistingOutputDir_ShouldReturnFail()
        {
            var nonExistingPath = Path.Combine(new[]{"this","Should","Not","Exist123"});
            _outputWriter = new LocalFilesystemOutputWriter(nonExistingPath);
            
            List<DataLicenseOutput> finDataOutputs = new List<DataLicenseOutput>
            {
                CreateFinDataEntry("id_1_GetData"),
                CreateFinDataEntry("id_2_GetData")
            };
            var writeResult =  _outputWriter.Write(finDataOutputs);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Fail));
        }

        private DataLicenseOutput CreateFinDataEntry(string id)
        {
            var headers = new List<string>{"h1","h2","h3"};
            var records = new List<Dictionary<string,string>>
            {
                new Dictionary<string, string>
                {
                    ["h1"] = "entry1Record1",
                    ["h2"] = "entry2Record1",
                    ["h3"] = "entry3Record1",
                },
                new Dictionary<string, string>
                {
                    ["h1"] = "entry1Record2",
                    ["h2"] = "entry2Record2",
                    ["h3"] = "entry3Record2",
                }
            };
            return new DataLicenseOutput(id, headers, records);
        }            
        
    }
}