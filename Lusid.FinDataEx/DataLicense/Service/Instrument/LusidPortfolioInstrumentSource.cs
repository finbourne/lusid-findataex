﻿using System;
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
        private readonly ILusidApiFactory _lusidApiFactory;
        private readonly InstrumentType _instrumentType;
        private readonly string _instrumentTypeLusidPropertyKey;
        private readonly ISet<Tuple<string, string>> _scopesAndPortfolios;
        private readonly DateTimeOffset _effectiveAt;

        public LusidPortfolioInstrumentSource(ILusidApiFactory lusidApiFactory, InstrumentType instrumentType, ISet<Tuple<string, string>> scopesAndPortfolios, DateTimeOffset effectiveAt)
        {
            _lusidApiFactory = lusidApiFactory;
            _instrumentType = instrumentType;
            _instrumentTypeLusidPropertyKey = GetLusidInstrumentIdPropertyAddress(instrumentType);
            _scopesAndPortfolios = scopesAndPortfolios;
            _effectiveAt = effectiveAt;
        }

        #nullable enable
        public Instruments? Get()
        {
            var holdingsInstrumentIds = GetHoldingsInstrumentIds(_lusidApiFactory, _scopesAndPortfolios, _effectiveAt);
            return holdingsInstrumentIds.Any() ? ToBbgDlInstruments(holdingsInstrumentIds) : null;
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
            var instrumentIds = new HashSet<string>();
            foreach (var (scope, portfolio) in scopesAndPortfolios)
            {
                try
                {
                    var holdings = transactionPortfoliosApi.GetHoldings(scope, portfolio, effectiveAt,
                        propertyKeys: new List<string>() {_instrumentTypeLusidPropertyKey});
                    var validPortfolioInstrumentIds = holdings.Values
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

        private static string GetLusidInstrumentIdPropertyAddress(InstrumentType instrumentType)
        {
            return instrumentType switch
            {
                InstrumentType.BB_GLOBAL => "Instrument/default/Figi",
                InstrumentType.ISIN => "Instrument/default/Isin",
                InstrumentType.CUSIP => "Instrument/default/Cusip",
                _ => throw new ArgumentException(
                    $"Only Figi, Isin and Cusips are currently supported. {instrumentType} not yet supported.")
            };
        }
    }
}