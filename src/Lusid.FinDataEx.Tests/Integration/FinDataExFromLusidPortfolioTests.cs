using System.IO;
using NUnit.Framework;

namespace Lusid.FinDataEx.Tests.Integration
{
    [TestFixture]
    [Ignore("Integration tests are currently unable to run because of licensing issues")]
    [Category("Unsafe")]
    public class FinDataExFromLusidPortfolioTests : LusidPortfolioTestHelper
    {
        private readonly string _tempOutputDir = $"TempTestDir_{nameof(FinDataExTests)}";

        [SetUp]
        public override void SetUp()
        {
            // base setup to create portfolios for test
            base.SetUp();

            TestUtils.SetupTempTestDirectory(_tempOutputDir);
        }
        
        [TearDown]
        public override void TearDown()
        {
            // base tear down to drop portfolios on test completion
            base.TearDown();

            TestUtils.TearDownTempTestDirectory(_tempOutputDir);
        }

        [Test]
        public void FinDataEx_GetData_OnValidPortfolios_ShouldProduceDataFile()
        {
            var scopePortfolio1 = $"{Scope}|{Portfolio}";
            var scopePortfolio2 = $"{Scope}|{Portfolio2}";
            var scopePortfolioSameHoldingP1 = $"{Scope}|{PortfolioSameHoldingAsP1}";
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";

            var commandArgs = $"getdata -i LusidPortfolioInstrumentSource -a {scopePortfolio1} {scopePortfolio2} {scopePortfolioSameHoldingP1} -f {filepath} -d ID_BB_GLOBAL PX_LAST --unsafe";
            FinDataEx.Main(commandArgs.Split(" "));

            var entries = File.ReadAllLines(filepath);

            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|PX_LAST"));

            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BVPV84"));
            // timestamps and price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[3], Is.Not.Empty);

            var instrumentEntry2 = entries[2].Split("|");
            Assert.That(instrumentEntry2[2], Is.EqualTo("BBG000BPHFS9"));
            // price will change with each call so just check not empty
            Assert.That(instrumentEntry2[0], Is.Not.Empty);
            Assert.That(instrumentEntry2[1], Is.Not.Empty);
            Assert.That(instrumentEntry2[3], Is.Not.Empty);
        }
        
        [Test]
        public void FinDataEx_GetData_OnValidPortfoliosUsingIsin_ShouldProduceDataFile()
        {
            var scopePortfolio1 = $"{Scope}|{Portfolio}";
            var filepath = $"{_tempOutputDir + Path.DirectorySeparatorChar}dl_request_output.csv";

            var commandArgs = $"getdata -i LusidPortfolioInstrumentSource -a {scopePortfolio1} -t ISIN -f {filepath} -d ID_BB_GLOBAL PX_LAST --unsafe";
            FinDataEx.Main(commandArgs.Split(" "));

            var entries = File.ReadAllLines(filepath);

            // querying BBG with ISIN but returning BBG Global Id (Figi)
            Assert.That(entries[0], Is.EqualTo("timeStarted|timeFinished|ID_BB_GLOBAL|PX_LAST"));

            var instrumentEntry1 = entries[1].Split("|");
            Assert.That(instrumentEntry1[2], Is.EqualTo("BBG000BVPV84"));
            // timestamps and price will change with each call so just check not empty
            Assert.That(instrumentEntry1[0], Is.Not.Empty);
            Assert.That(instrumentEntry1[1], Is.Not.Empty);
            Assert.That(instrumentEntry1[3], Is.Not.Empty);
        }
    }
}