using System;
using System.Collections.Generic;
using System.IO;
using Lusid.Drive.Sdk.Api;
using Lusid.Drive.Sdk.Model;
using Lusid.Drive.Sdk.Utilities;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using Moq;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicense.Instrument
{
    /// <summary>
    /// Note this requires test data available in the domain instance being tested against.
    ///
    /// TestData can be copied from the sources used for the CsvInstrumentSourceTest
    /// 
    /// </summary>
    [TestFixture]
    [Category("Unsafe")]
    public class DriveCsvInstrumentSourceTest
    {
        private Mock<ILusidApiFactory> mockApiFactory;
        private Mock<IFilesApi> mockFilesApi = new Mock<IFilesApi>();
        private Mock<ISearchApi> mockSearchApi = new Mock<ISearchApi>();

        const string filepath = "/findataex-tests/testdata/";

        [SetUp]
        public void Setup()
        {
            mockApiFactory = new Mock<ILusidApiFactory>();
            mockFilesApi = new Mock<IFilesApi>();
            mockSearchApi = new Mock<ISearchApi>();

            mockApiFactory.Setup(factory => factory.Api<IFilesApi>()).Returns(mockFilesApi.Object);
            mockApiFactory.Setup(factory => factory.Api<ISearchApi>()).Returns(mockSearchApi.Object);
        }

        [Test]
        public void Get_OnSingleColumnCsv_ShouldReturnDlInstruments()
        {
            //when
            const string filename = "single_col_instruments.csv";
            ConfigureDriveApi(filename);

            var instrumentSourceArgs = new List<string> { Path.Combine(filepath, filename) };
            var instrumentSource = DriveCsvInstrumentSource.Create(mockApiFactory.Object, InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);

            //execute
            var instruments = instrumentSource.Get();
            Assert.That(instruments.instrument.Length, Is.EqualTo(6));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000NEWTRA"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[1].id, Is.EqualTo("BBG000NEWTRB"));
            Assert.That(instruments.instrument[1].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[5].id, Is.EqualTo("BBG000NEWTRF"));
            Assert.That(instruments.instrument[5].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }
        
        [Test]
        public void Get_OnMultiColumnCsv_ShouldReturnDlInstrumentsFromSelectedColumn()
        {
            //when
            const string filename = "multi_col_instruments.csv";
            ConfigureDriveApi(filename);

            var instrumentSourceArgs = new List<string> { Path.Combine(filepath, filename), ",", "1" };
            var instrumentSource = DriveCsvInstrumentSource.Create(mockApiFactory.Object, InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);

            //execute
            var instruments = instrumentSource.Get();
            Assert.That(instruments.instrument.Length, Is.EqualTo(6));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000NEWTRA"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[1].id, Is.EqualTo("BBG000NEWTRB"));
            Assert.That(instruments.instrument[1].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[5].id, Is.EqualTo("BBG000NEWTRF"));
            Assert.That(instruments.instrument[5].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }

        [Test]
        public void Get_OnMultiColumnCsvPipeDelimited_ShouldReturnDlInstrumentsFromSelectedColumn()
        {
            //when
            const string filename = "multi_col_instruments_pipe_delim.csv";
            ConfigureDriveApi(filename);

            var instrumentSourceArgs = new List<string> { Path.Combine(filepath, filename), "|", "1" };
            var instrumentSource = DriveCsvInstrumentSource.Create(mockApiFactory.Object, InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);

            //execute
            var instruments = instrumentSource.Get();
            Assert.That(instruments.instrument.Length, Is.EqualTo(6));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000NEWTRA"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[1].id, Is.EqualTo("BBG000NEWTRB"));
            Assert.That(instruments.instrument[1].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[5].id, Is.EqualTo("BBG000NEWTRF"));
            Assert.That(instruments.instrument[5].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }

        [Test]
        public void Get_EmptyCsv_ShouldReturnNoInstruments()
        {
            //when
            const string filename = "empty_instruments.csv";
            ConfigureDriveApi(filename);

            var instrumentSourceArgs = new List<string>{ Path.Combine(filepath, filename) };
            var instrumentSource = DriveCsvInstrumentSource.Create(mockApiFactory.Object, InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);
            
            //execute
            var instruments = instrumentSource.Get();
            Assert.That(instruments.instrument, Is.Empty);
        }

        private void ConfigureDriveApi(string filename)
        {
            mockSearchApi.Setup(searchApi => searchApi.Search(It.IsAny<SearchBody>(), It.IsAny<string>(), null, null, null))
                    .Returns(new PagedResourceListOfStorageObject(values: new List<StorageObject> { new StorageObject("1", "", "", "", DateTimeOffset.Now, "", DateTimeOffset.Now, "", 0, "", "", null) }));

            mockFilesApi.Setup(filesApi => filesApi.DownloadFile("1"))
                .Returns(File.OpenRead(Path.Combine("Integration","DataLicense","Instrument","TestData", filename)));
        }
    }
}