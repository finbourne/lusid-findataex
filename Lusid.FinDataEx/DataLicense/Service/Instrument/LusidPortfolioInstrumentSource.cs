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
        private readonly InstrumentType _instrumentType;
        private readonly string _instrumentTypeLusidPropertyKey;
        private readonly ISet<Tuple<string, string>> _scopesAndPortfolios;
        private readonly DateTimeOffset _effectiveAt;

        public LusidPortfolioInstrumentSource(InstrumentType instrumentType, ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            _instrumentType = instrumentType;
            _instrumentTypeLusidPropertyKey = getLusidInstrumentIdPropertyAddress(instrumentType);
            _scopesAndPortfolios = scopesAndPortfolios;
            _effectiveAt = effectiveAt;
        }

        #nullable enable
        public Instruments? Get()
        {
            var lusidApiFactory = LusidApiFactoryBuilder.Build("secrets_api.json");
            var portfolioIntrumentIds = GetHoldingsInstrumentIds(lusidApiFactory, _scopesAndPortfolios, _effectiveAt);
            return portfolioIntrumentIds.Any() ? ToBbgDlInstruments(portfolioIntrumentIds) : null;
        }

        private Instruments ToBbgDlInstruments(ISet<string> ids)
        {
            var instruments = ids.Select(figi => new PerSecurity_Dotnet.Instrument()
            {
                id = figi,
                type = _instrumentType,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }

        private ISet<string> GetHoldingsInstrumentIds(ILusidApiFactory lusidApiFactory, ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            var transactionPortfoliosApi = lusidApiFactory.Api<TransactionPortfoliosApi>();
            ISet<string> instrumentIds = new HashSet<string>();
            foreach (Tuple<string, string> scopeAndPortfolio in scopesAndPortfolios)
            {
                string scope = scopeAndPortfolio.Item1;
                string portfolio = scopeAndPortfolio.Item2;
                try
                {
                    var holdings = transactionPortfoliosApi.GetHoldings(scope, portfolio, effectiveAt,
                        propertyKeys: new List<string>() {_instrumentTypeLusidPropertyKey});
                    ISet<string> validPortfolioInstrumentIds = holdings.Values
                        .Where(h => h.Properties.ContainsKey(_instrumentTypeLusidPropertyKey))
                        .Select(h => h.Properties[_instrumentTypeLusidPropertyKey].Value.LabelValue)
                        .ToHashSet();

                    Console.WriteLine($"Retrieving ids for instruments of positions for scope={scope}, " +
                                      $"portfolio={portfolio}, effectiveAt={effectiveAt}. InstrumentId={_instrumentType}, instrument Ids={validPortfolioInstrumentIds}");

                    instrumentIds.UnionWith(validPortfolioInstrumentIds);
                }
                catch (ApiException e)
                {
                    Console.WriteLine(
                        $"Could not retrieve holdings for scope={scope} and portfolio={portfolio} due to exception. " +
                        $"Ignoring and moving to next portfolio. Details: {e}");
                }
            }

            return instrumentIds;
        }

        private string getLusidInstrumentIdPropertyAddress(InstrumentType instrumentType)
        {
            switch (instrumentType)
            {
                case InstrumentType.BB_GLOBAL:
                    return "Instrument/default/Figi";
                case InstrumentType.ISIN:
                    return "Instrument/default/Isin";
                case InstrumentType.CUSIP:
                    return "Instrument/default/Cusip";
                default:
                    throw new ArgumentException(
                        $"Only Figi, Isin and Cusips are currently supported. {instrumentType} not yet supported.");
            }
        }
    }
}