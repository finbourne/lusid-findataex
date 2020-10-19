using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.Sdk.Api;
using Lusid.Sdk.Client;
using Lusid.Sdk.Utilities;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.DataLicense.Service.Instrument
{
    public class LusidPortfolioInstrumentSource : IInstrumentSource
    {
        private const string FigiInstrumentProperty = "Instrument/default/Figi";
        private readonly ISet<Tuple<string, string>> _scopesAndPortfolios;
        private readonly DateTimeOffset _effectiveAt;

        public LusidPortfolioInstrumentSource(ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            this._scopesAndPortfolios = scopesAndPortfolios;
            this._effectiveAt = effectiveAt;
        }

        #nullable enable
        public Instruments? Get()
        {
            var lusidApiFactory = LusidApiFactoryBuilder.Build("secrets_api.json");
            var portfolioIntrumentFigis = GetHoldingsInstrumentFigis(lusidApiFactory, _scopesAndPortfolios, _effectiveAt);
            return portfolioIntrumentFigis.Any() ? ToBbgDlInstruments(portfolioIntrumentFigis) : null;
        }

        private Instruments ToBbgDlInstruments(ISet<string> figis)
        {
            var instruments = figis.Select(figi => new PerSecurity_Dotnet.Instrument()
            {
                id = figi,
                type = InstrumentType.BB_GLOBAL,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }

        private ISet<string> GetHoldingsInstrumentFigis(ILusidApiFactory lusidApiFactory, ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            var transactionPortfoliosApi = lusidApiFactory.Api<TransactionPortfoliosApi>();
            ISet<string> figis = new HashSet<string>();
            foreach (Tuple<string, string> scopeAndPortfolio in scopesAndPortfolios)
            {
                string scope = scopeAndPortfolio.Item1;
                string portfolio = scopeAndPortfolio.Item2;
                try
                {
                    var holdings = transactionPortfoliosApi.GetHoldings(scope, portfolio, effectiveAt,
                        propertyKeys: new List<string>() {FigiInstrumentProperty});
                    ISet<string> portfolioFigis = holdings.Values
                        .Where(h => h.Properties.ContainsKey(FigiInstrumentProperty))
                        .Select(h => h.Properties[FigiInstrumentProperty].Value.LabelValue)
                        .ToHashSet();

                    Console.WriteLine($"Retrieving figis for instruments of positions for scope={scope}, " +
                                      $"portfolio={portfolio}, effectiveAt={effectiveAt}. Figis={portfolioFigis}");

                    figis.UnionWith(portfolioFigis);
                }
                catch (ApiException e)
                {
                    Console.WriteLine(
                        $"Could not retrieve holdings for scope={scope} and portfolio={portfolio} due to exception. " +
                        $"Ignoring and moving to next portfolio. Details: {e}");
                }
            }

            return figis;
        }
    }
}