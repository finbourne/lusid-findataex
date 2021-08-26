﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
using Lusid.FinDataEx.DataLicense.Util;
using Lusid.FinDataEx.DataLicense.Vendor;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicense.Service
{
    [TestFixture]
    [Category("Unsafe")]
    public class DataLicenseServiceTests
    {

        private DataLicenseService _dataLicenseService;
        private PerSecurityWS _perSecurityWs;

        // BBG DL Calls
        private GetDataLicenseCall _getDataLicenseCall;

        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            _getDataLicenseCall = new GetDataLicenseCall(_perSecurityWs);
            _dataLicenseService = new DataLicenseService();
        }

        [Test]
        public void Get_OnAdhocGetData_ShouldReturnOutput()
        {
            var bbgIds = CreateInstruments(new List<string>{"BBG000BPHFS9", "BBG000BVPV84"});
            var finDataOutput = _dataLicenseService.Get(_getDataLicenseCall, bbgIds, DataLicenseTypes.ProgramTypes.Adhoc);
            
            CollectionAssert.AreEqual(finDataOutput.Header, new List<string>(){"timeStarted","timeFinished","ID_BB_GLOBAL","PX_LAST"});

            // check data in records (changing prices mean just need to check its populated and not the exact number)
            Assert.That(finDataOutput.Records.Count, Is.EqualTo(2));
            finDataOutput.Records[0].TryGetValue("ID_BB_GLOBAL", out string bbdId1); 
            Assert.That(bbdId1, Is.EqualTo("BBG000BPHFS9"));
            finDataOutput.Records[0].TryGetValue("PX_LAST", out string lastPrice1); 
            Assert.That(lastPrice1, Is.Not.Empty);
            finDataOutput.Records[0].TryGetValue("timeStarted", out string timeStarted); 
            Assert.That(timeStarted, Is.Not.Empty);
            finDataOutput.Records[0].TryGetValue("timeFinished", out string timeFinished); 
            Assert.That(timeFinished, Is.Not.Empty);
            
            finDataOutput.Records[1].TryGetValue("ID_BB_GLOBAL", out string bbdId2); 
            Assert.That(bbdId2, Is.EqualTo("BBG000BVPV84"));
            finDataOutput.Records[1].TryGetValue("PX_LAST", out string lastPrice2); 
            Assert.That(lastPrice2, Is.Not.Empty);
            finDataOutput.Records[1].TryGetValue("timeStarted", out string timeStarted2); 
            Assert.That(timeStarted2, Is.Not.Empty);
            finDataOutput.Records[1].TryGetValue("timeFinished", out string timeFinished2); 
            Assert.That(timeFinished2, Is.Not.Empty);
        }
        
        [Test]
        public void Get_OnAdhocGetDataWithNoIds_ShouldReturnEmptyData()
        {
            var finDataOutput = _dataLicenseService.Get(_getDataLicenseCall, CreateInstruments(new List<string>()), DataLicenseTypes.ProgramTypes.Adhoc);
            Assert.True(finDataOutput.IsEmpty());
        }
        
        [Test]
        public void Get_OnAScheduledGetData_ShouldThrowUnsupportedException()
        {
            var bbgIds = CreateInstruments(new List<string>{"BBG000BPHFS9", "BBG000BVPV84"});
            Assert.Throws<NotSupportedException>(() =>
                _dataLicenseService.Get(_getDataLicenseCall, bbgIds, DataLicenseTypes.ProgramTypes.Scheduled));
        }
        
        private Instruments CreateInstruments(IEnumerable<string> bbgIds)
        {
            var instruments = bbgIds.Select(id => new PerSecurity_Dotnet.Instrument()
            {
                id = id,
                type = InstrumentType.BB_GLOBAL,
                typeSpecified = true
            }).ToArray();
            return new Instruments()
            {
                instrument = instruments
            };
        }

    }
}