using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lusid.FinDataEx.Output;
using NUnit.Framework;
using static Lusid.FinDataEx.Tests.Unit.TestUtils;

namespace Lusid.FinDataEx.Tests.Integration.Output
{
    [TestFixture]
    public class LocalFilesystemFinDataOutputWriterTests
    {
        
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExTests)}";
        private LocalFilesystemFinDataOutputWriter _outputWriter;
        [SetUp]
        public void SetUp()
        {
            SetupTempTestDirectory(_tempOutputDir);
            _outputWriter = new LocalFilesystemFinDataOutputWriter(_tempOutputDir);
        }
        
        [TearDown]
        public void TearDown()
        {
            TearDownTempTestDirectory(_tempOutputDir);
        }

        [Test]
        public void Write_OnValidFinData_ShouldWriteToOutputDir()
        {
            List<FinDataOutput> finDataOutputs = new List<FinDataOutput>
            {
                CreateFinDataEntry("id_1.GetData"),
                CreateFinDataEntry("id_2.GetData")
            };

            WriteResult writeResult =  _outputWriter.Write(finDataOutputs);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "id_1.GetData.csv", Does.Exist);
            Assert.That(_tempOutputDir + Path.DirectorySeparatorChar + "id_2.GetData.csv", Does.Exist);
            
            // ensure file is properly populated
            string[] entries = File.ReadAllLines(_tempOutputDir + Path.DirectorySeparatorChar + "id_1.GetData.csv");
            // check headers
            Assert.That(entries[0], Is.EqualTo("h1|h2|h3"));
            Assert.That(entries[1], Is.EqualTo("entry1Record1|entry2Record1|entry3Record1"));
            Assert.That(entries[2], Is.EqualTo("entry1Record2|entry2Record2|entry3Record2"));
        }
        
        [Test]
        public void Write_OnEmptyInput_ShouldWriteFileWithHeaderOnly()
        {
            List<FinDataOutput> finDataOutputs = new List<FinDataOutput>();
            List<string> headers = new List<string>{"h1","h2","h3"};
            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            finDataOutputs.Add(new FinDataOutput("id.GetData", headers, records));

            WriteResult writeResult =  _outputWriter.Write(finDataOutputs);
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            
            // ensure file is properly populated
            string[] entries = File.ReadAllLines(_tempOutputDir + Path.DirectorySeparatorChar + "id.GetData.csv");
            // contains headers only
            Assert.That(entries.Length, Is.EqualTo(1));
            // check headers
            Assert.That(entries[0], Is.EqualTo("h1|h2|h3"));

        }
        
        [Test]
        public void Write_OnNoFinData_ShouldDoNothingButReturnOk()
        {
            List<FinDataOutput> finDataOutputs = new List<FinDataOutput>();

            WriteResult writeResult =  _outputWriter.Write(finDataOutputs);
            
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.False(Directory.EnumerateFileSystemEntries(_tempOutputDir).Any());
        }
        
        [Test]
        public void Write_OnNonExistingOutputDir_ShouldReturnFail()
        {
            var nonExistingPath = Path.Combine(new[]{"this","Should","Not","Exist123"});
            _outputWriter = new LocalFilesystemFinDataOutputWriter(nonExistingPath);
            
            List<FinDataOutput> finDataOutputs = new List<FinDataOutput>
            {
                CreateFinDataEntry("id_1.GetData"),
                CreateFinDataEntry("id_2.GetData")
            };
            WriteResult writeResult =  _outputWriter.Write(finDataOutputs);
            
            //verify
            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Fail));
        }

        private FinDataOutput CreateFinDataEntry(string id)
        {
            List<string> headers = new List<string>{"h1","h2","h3"};
            List<Dictionary<string,string>> records = new List<Dictionary<string,string>>
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
            return new FinDataOutput(id, headers, records);
        }            
        
    }
}