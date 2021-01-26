using System.Collections.Generic;
using System.IO;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicense.Instrument
{
    /// <summary>
    /// Note this requires test data available in the domain instance being tested against.
    ///
    /// TestData can be copied from the sources used for the InstrumentFromCsvSourceTest
    /// 
    /// </summary>
    [TestFixture]
    public class InstrumentFromDriveCsvSourceTest
    {
        
        [Test]
        public void Get_OnSingleColumnCsv_ShouldReturnDlInstruments()
        {
            //when
            const string filepath = "/findataex-tests/testdata/single_col_instruments.csv";
            IEnumerable<string> instrumentSourceArgs = new[]{filepath};
            var instrumentSource = InstrumentFromDriveCsvSource.Create(InstrumentType.BB_GLOBAL, instrumentSourceArgs);
            
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
            const string filepath = "/findataex-tests/testdata/multi_col_instruments.csv";
            IEnumerable<string> instrumentSourceArgs = new[]{filepath, ",", "1"};
            var instrumentSource = InstrumentFromDriveCsvSource.Create(InstrumentType.BB_GLOBAL, instrumentSourceArgs);
            
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
            const string filepath = "/findataex-tests/testdata/multi_col_instruments_pipe_delim.csv";
            IEnumerable<string> instrumentSourceArgs = new[]{filepath, "|", "1"};
            var instrumentSource = InstrumentFromDriveCsvSource.Create(InstrumentType.BB_GLOBAL, instrumentSourceArgs);
            
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
            const string filepath = "/findataex-tests/testdata/empty_instruments.csv";
            IEnumerable<string> instrumentSourceArgs = new[]{filepath};
            var instrumentSource = InstrumentFromDriveCsvSource.Create(InstrumentType.BB_GLOBAL, instrumentSourceArgs);
            
            //execute
            var instruments = instrumentSource.Get();
            Assert.That(instruments.instrument, Is.Empty);
        }
        
    }
}