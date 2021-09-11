using System;
using System.Collections.Generic;
using Lusid.FinDataEx.Output;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Moq;
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
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>
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
            var fakeData = new DataLicenseOutput(id, headers, records);

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
        public void EmptyInputShouldBeWrittenWithHeaderOnly()
        {
            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var path = "path/to/file";
            var data = new List<string>
            {
                "h1|h2|h3"
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

            Assert.That(writeResult.Status, Is.EqualTo(WriteResultStatus.NotRun));
        }

        [Test]
        public void NonExistingOutputDirShouldReturnFail()
        {
            var id = "unique id";
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

            var path = "path/to/file";
            var data = new List<string>
            {
                "h1|h2|h3"
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
            var headers = new List<string> { "h1", "h2", "h3" };
            var records = new List<Dictionary<string, string>>();
            var fakeData = new DataLicenseOutput(id, headers, records);

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
            throw new System.NotImplementedException();
        }

        public List<string> Read(string path, char entrySeparator)
        {
            throw new System.NotImplementedException();
        }

        public string ValidatePath(string path)
        {
            throw new System.NotImplementedException();
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