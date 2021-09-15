using System;
using System.Collections.Generic;
using Lusid.FinDataEx.Data;
using Lusid.FinDataEx.Data.DataRecord;
using Lusid.FinDataEx.Output;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Unit.Output
{
    [TestFixture]
    public class FileOutputWriterTests
    {
        [Test]
        public void ValidDataIsWrittenToFile()
        {
            var id = "unique id";
            var records = new List<IRecord>
            {
                new InstrumentDataRecord(
                    new Dictionary<string, string>
                    {
                        ["h1"] = "entry1Record1",
                        ["h2"] = "entry2Record1",
                        ["h3"] = "entry3Record1",
                    }
                ),
                new InstrumentDataRecord(
                    new Dictionary<string, string>
                    {
                        ["h1"] = "entry1Record2",
                        ["h2"] = "entry2Record2",
                        ["h3"] = "entry3Record2",
                    }
                )
            };
            var fakeData = new DataLicenseOutput(id, records);

            var path = "path/to/file";
            var data = new List<string>
            {
                "h1|h2|h3",
                "entry1Record1|entry2Record1|entry3Record1",
                "entry1Record2|entry2Record2|entry3Record2"
            };
            var fakeFileHandler = new AssertFileHandler(path, data);

            var fakeOptions = new DataLicenseOptions
            {
                OutputPath = path
            };

            var writeResult = new FileOutputWriter(fakeOptions, fakeFileHandler).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.EqualTo(path));
        }

        [Test]
        public void EmptyInputShouldDoNothingButReturnOk()
        {
            var id = "unique id";
            var records = new List<IRecord>();
            var fakeData = new DataLicenseOutput(id, records);

            var path = "path/to/file";
            var data = new List<string>();
            var fakeFileHandler = new AssertFileHandler(path, data);

            var fakeOptions = new DataLicenseOptions
            {
                OutputPath = path
            };

            var writeResult = new FileOutputWriter(fakeOptions, fakeFileHandler).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.EqualTo(""));
        }

        [Test]
        public void NoInputShouldDoNothingButReturnOk()
        {
            var fakeData = DataLicenseOutput.Empty("Request returned no results");

            var path = "path/to/file";
            var data = new List<string>();
            var fakeFileHandler = new AssertFileHandler(path, data);

            var fakeOptions = new DataLicenseOptions
            {
                OutputPath = path
            };

            var writeResult = new FileOutputWriter(fakeOptions, fakeFileHandler).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
            Assert.That(writeResult.FileOutputPath, Is.EqualTo(""));
        }

        [Test]
        public void NonExistingOutputDirShouldReturnFail()
        {
            var id = "unique id";
            var records = new List<IRecord>
            {
                new InstrumentDataRecord(new Dictionary<string, string>
                    {
                        { "h1", "v1" },
                        { "h2", "v2" },
                        { "h3", "v3" },
                    }
                )
            };
            var fakeData = new DataLicenseOutput(id, records);

            var path = "path/to/file";
            var data = new List<string>
            {
                "h1|h2|h3",
                "v1|v2|v3"
            };
            var fakeFileHandler = new AssertFileHandler("", data);

            var fakeOptions = new DataLicenseOptions
            {
                OutputPath = path
            };

            var writeResult = new FileOutputWriter(fakeOptions, fakeFileHandler).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Fail));
        }

        [Test]
        public void FilenamesRespectAutoPatterns()
        {
            var id = "unique id";
            var records = new List<IRecord>();
            var fakeData = new DataLicenseOutput(id, records);

            var path = "path/to/{TEST}";
            var data = new List<string>
            {
                "h1|h2|h3"
            };
            var fakeFileHandler = new AssertFileHandler("path/to/TestAutoGenPattern", data);

            var fakeOptions = new DataLicenseOptions
            {
                OutputPath = path
            };

            var writeResult = new FileOutputWriter(fakeOptions, fakeFileHandler).Write(fakeData);

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.Ok));
        }
    }

    public class AssertFileHandler : IFileHandler
    {
        private readonly string _expectedPath;
        private readonly List<string> _expectedData;

        public AssertFileHandler(string expectedPath, List<string> expectedData)
        {
            _expectedPath = expectedPath;
            _expectedData = expectedData;
        }

        public bool Exists(string path)
        {
            throw new NotImplementedException();
        }

        public List<string> Read(string path, char entrySeparator)
        {
            throw new NotImplementedException();
        }

        public string ValidatePath(string path)
        {
            throw new NotImplementedException();
        }

        public string Write(string path, List<string> data, char entrySeparator)
        {
            if (!path.Equals(_expectedPath))
            {
                throw new Exception("Path has failed test assertion");
            }

            Assert.That(path, Is.EqualTo(_expectedPath));

            Assert.That(data.Count, Is.EqualTo(_expectedData.Count));

            for (var i = 0; i < data.Count; i++)
            {
                Assert.That(data[i], Is.EqualTo(_expectedData[i]));
            }

            return path;
        }
    }
}