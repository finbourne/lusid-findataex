using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicense.Instrument
{
    [TestFixture]
    public class CsvInstrumentSourceTest
    {
        
        [Test]
        public void Get_OnSingleColumnCsv_ShouldReturnDlInstruments()
        {
            //when
            var filepath = Path.Combine(new[]{"Integration","DataLicense","Instrument","TestData",
                "single_col_instruments.csv"});
            IEnumerable<string> instrumentSourceArgs = new[]{filepath};
            var instrumentSource = CsvInstrumentSource.Create(InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);
            
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
            var filepath = Path.Combine(new[]{"Integration","DataLicense","Instrument","TestData",
                "multi_col_instruments.csv"});
            IEnumerable<string> instrumentSourceArgs = new[]{filepath, ",", "1"};
            var instrumentSource = CsvInstrumentSource.Create(InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);
            
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
            var filepath = Path.Combine(new[]{"Integration","DataLicense","Instrument","TestData",
                "multi_col_instruments_pipe_delim.csv"});
            IEnumerable<string> instrumentSourceArgs = new[]{filepath, "|", "1"};
            var instrumentSource = CsvInstrumentSource.Create(InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);
            
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
        public void Get_OnAutoGenPatternInFilePath_ShouldReturnDlInstruments()
        {
            //{TEST} should end up looking up single_col_instruments_TestAutoGenPattern.csv is AutoGen working.
            var filepath = Path.Combine(new[]{"Integration","DataLicense","Instrument","TestData",
                "single_col_instruments_{TEST}.csv"});
            IEnumerable<string> instrumentSourceArgs = new[]{filepath};
            var instrumentSource = CsvInstrumentSource.Create(InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);
            
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
            var filepath = Path.Combine(new[]{"Integration","DataLicense","Instrument","TestData",
                "empty_instruments.csv"});
            IEnumerable<string> instrumentSourceArgs = new[]{filepath};
            var instrumentSource = CsvInstrumentSource.Create(InstrumentArgs.Create(InstrumentType.BB_GLOBAL), instrumentSourceArgs);
            
            //execute
            var instruments = instrumentSource.Get();
            Assert.That(instruments.instrument, Is.Empty);
        }
        
    }
}