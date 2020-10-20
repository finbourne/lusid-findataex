using System;
using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service.Instrument;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicence.Instrument
{
    [TestFixture]
    public class LusidPortfolioInstrumentSourceTests : BaseLusidPortfolioTests
    {

        [Test]
        public void Get_OnPortfoliosWithAndWithoutHoldings_ShouldDlInstruments()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(Scope, Portfolio),
                new Tuple<string, string>(Scope, Portfolio2),
                new Tuple<string, string>(Scope, PortfolioNoHoldings),
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, EffectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.That(instruments.instrument.Length, Is.EqualTo(2));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
            Assert.That(instruments.instrument[1].id, Is.EqualTo("BBG000BPHFS9"));
            Assert.That(instruments.instrument[1].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }
        
        [Test]
        public void Get_OnPortfoliosWithOneThatDoesNotExist_ShouldReturnDlInstrumentsForExistingPortfolio()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(Scope, Portfolio),
                new Tuple<string, string>(Scope, "portfolio_does_not_exist"),
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, EffectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.That(instruments.instrument.Length, Is.EqualTo(1));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }

        [Test]
        public void Get_OnPortfoliosWithSameInstrumentHoldings_ShouldReturnOnlyOneOfDlInstruments()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string, string>>()
            {
                new Tuple<string, string>(Scope, Portfolio),
                new Tuple<string, string>(Scope, PortfolioSameHoldingAsP1),
            };

            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, EffectiveAt);
            var instruments = instrumentSource.Get();

            Assert.That(instruments.instrument.Length, Is.EqualTo(1));
            Assert.That(instruments.instrument[0].id, Is.EqualTo("BBG000BVPV84"));
            Assert.That(instruments.instrument[0].type, Is.EqualTo(InstrumentType.BB_GLOBAL));
        }

        [Test]
        public void Get_OnPortfoliosWithoutHoldings_ShouldReturnNull()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(Scope, PortfolioNoHoldings)
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, EffectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.IsNull(instruments);
        }
        
        [Test]
        public void Get_OnPortfoliosDoNotExist_ShouldReturnNull()
        {
            var scopesAndPortfolios = new HashSet<Tuple<string,string>>()
            {
                new Tuple<string, string>(Scope, "portfolio_does_not_exist"),
                new Tuple<string, string>(Scope, "neither_does_this_portfolio")
            };
            
            var instrumentSource = new LusidPortfolioInstrumentSource(scopesAndPortfolios, EffectiveAt);
            var instruments = instrumentSource.Get();
            
            Assert.IsNull(instruments);
        }
        
        

    }
}