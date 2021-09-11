using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.Util.FileUtils.Handler;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lusid.FinDataEx.Tests.Unit.Util.FileUtils.Handler
{
    [TestFixture]
    public class LusidDriveFileHandlerTests
    {
        private ILusidApiFactory _mockDriveApiFactory;
        private IFilesApi _mockFilesApi;
        private ISearchApi _mockSearchApi;

        [SetUp]
        public void SetUp()
        {
            _mockDriveApiFactory = Mock.Of<ILusidApiFactory>();
            _mockFilesApi = Mock.Of<IFilesApi>();
            _mockSearchApi = Mock.Of<ISearchApi>();

            Mock.Get(_mockDriveApiFactory)
                .Setup(mock => mock.Api<IFilesApi>())
                .Returns(_mockFilesApi);

            Mock.Get(_mockDriveApiFactory)
                .Setup(mock => mock.Api<ISearchApi>())
                .Returns(_mockSearchApi);
        }

        [Test]
        public void SplitsPathCorrectly()
        {
            var driveFileHandler = new LusidDriveFileHandler(_mockDriveApiFactory);

            var folderAndFile = driveFileHandler.PathToFolderAndFile("path/to/file");

            Assert.That(folderAndFile.Item1, Is.EqualTo("path/to"));
            Assert.That(folderAndFile.Item2, Is.EqualTo("file"));
        }

        [Test]
        public void ValidationSearchesCorrectly()
        {
            Mock.Get(_mockSearchApi)
                .Setup(mock => mock.Search(It.Is<SearchBody>(s => s.WithPath.Equals("path/to") && s.Name.Equals("file")), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<int?>(), It.IsAny<string>()))
                .Returns(new PagedResourceListOfStorageObject(null, null, new List<StorageObject> { new StorageObject("success", "", "", "", DateTimeOffset.Now, "", DateTimeOffset.Now, "") }));

            var driveFileHandler = new LusidDriveFileHandler(_mockDriveApiFactory);

            var id = driveFileHandler.ValidatePath("path/to/file");

            Assert.That(id, Is.EqualTo("success"));
        }

        [Test]
        public void ReadProducesValidOutput()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write("value11,value12,value13");
            writer.Flush();
            stream.Position = 0;

            Mock.Get(_mockFilesApi)
                .Setup(mock => mock.DownloadFile(It.IsAny<string>()))
                .Returns(stream);

            var driveFileHandler = new LusidDriveFileHandler(_mockDriveApiFactory);

            var data = driveFileHandler.Read("path/to/file", ',');

            Assert.That(data[0], Is.EqualTo("value11"));
            Assert.That(data[1], Is.EqualTo("value12"));
            Assert.That(data[2], Is.EqualTo("value13"));
        }
    }
}
