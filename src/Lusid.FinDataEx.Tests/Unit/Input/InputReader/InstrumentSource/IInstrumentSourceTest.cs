using Lusid.FinDataEx.DataLicense.Service.Instrument;
using NUnit.Framework;
using PerSecurity_Dotnet;
using System.Collections.Generic;
using System.Linq;

namespace Lusid.FinDataEx.Tests.Unit.Input.InputReader.InstrumentSource
{
    [TestFixture]
    public class IInstrumentSourceTest
    {
        [Test]
        public void SingleInstrument()
        {
            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };
            var instrumentArgs = InstrumentArgs.Create(options);
            var instrumentIds = new List<string> { "instrument1" };

            var output = IInstrumentSource.CreateInstruments(instrumentArgs, instrumentIds);
            var outputInstrument = output.instrument.Single();

            Assert.That(outputInstrument.id, Is.EqualTo(instrumentIds.First()));
            Assert.That(outputInstrument.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument.typeSpecified, Is.True);
            Assert.That(outputInstrument.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void MultipleInstrument()
        {
            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };
            var instrumentArgs = InstrumentArgs.Create(options);
            var instrumentIds = new List<string> { "instrument1", "instrument2" };

            var output = IInstrumentSource.CreateInstruments(instrumentArgs, instrumentIds);
            var outputInstrument1 = output.instrument.First();

            Assert.That(outputInstrument1.id, Is.EqualTo(instrumentIds.First()));
            Assert.That(outputInstrument1.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument1.typeSpecified, Is.True);
            Assert.That(outputInstrument1.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument1.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument2 = output.instrument.Last();

            Assert.That(outputInstrument2.id, Is.EqualTo(instrumentIds.Last()));
            Assert.That(outputInstrument2.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument2.typeSpecified, Is.True);
            Assert.That(outputInstrument2.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument2.yellowkey, Is.EqualTo(MarketSector.Govt));
        }

        [Test]
        public void DuplicateInstrumentsAreIgnored()
        {
            var options = new DataLicenseOptions
            {
                InstrumentIdType = InstrumentType.ISIN,
            };
            var instrumentArgs = InstrumentArgs.Create(options);
            var instrumentIds = new List<string> { "instrument1", "instrument2", "instrument1" };
            var uniqueInstrumentIds = instrumentIds.ToHashSet();

            var output = IInstrumentSource.CreateInstruments(instrumentArgs, instrumentIds);

            Assert.That(output.instrument.Length, Is.EqualTo(2));

            var outputInstrument1 = output.instrument.First();

            Assert.That(outputInstrument1.id, Is.EqualTo(uniqueInstrumentIds.First()));
            Assert.That(outputInstrument1.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument1.typeSpecified, Is.True);
            Assert.That(outputInstrument1.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument1.yellowkey, Is.EqualTo(MarketSector.Govt));

            var outputInstrument2 = output.instrument.Last();

            Assert.That(outputInstrument2.id, Is.EqualTo(uniqueInstrumentIds.Last()));
            Assert.That(outputInstrument2.type, Is.EqualTo(instrumentArgs.InstrumentType));
            Assert.That(outputInstrument2.typeSpecified, Is.True);
            Assert.That(outputInstrument2.yellowkeySpecified, Is.True);
            Assert.That(outputInstrument2.yellowkey, Is.EqualTo(MarketSector.Govt));
        }
    }
}