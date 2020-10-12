using System;
using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Util;
using Lusid.FinDataEx.DataLicense.Vendor;
using NUnit.Framework;
using PerSecurity_Dotnet;

namespace Lusid.FinDataEx.Tests.Integration.DataLicence.Service
{
    public class DlDataServiceTests
    {

        private DlDataService _dlDataService;
        private PerSecurityWS _perSecurityWs;

        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            _dlDataService = new DlDataService(_perSecurityWs);
        }

        [Test]
        public void Get_OnAdhocGetData_ShouldReturnOutput()
        {
            var bbgIds = new List<string>{"EQ0010174300001000", "EQ0021695200001000"};
            var finDataOutputs = _dlDataService.Get(bbgIds, DlTypes.ProgramTypes.Adhoc, DlTypes.DataTypes.GetData);
            
            Assert.That(finDataOutputs.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(finDataOutputs[0].Header, new List<string>(){"ID_BB_UNIQUE","PX_LAST"});

            // check data in records (changing prices mean just need to check its populated and not the exact number)
            Assert.That(finDataOutputs[0].Records.Count, Is.EqualTo(2));
            finDataOutputs[0].Records[0].TryGetValue("ID_BB_UNIQUE", out string bbdId1); 
            Assert.That(bbdId1, Is.EqualTo("EQ0010174300001000"));
            finDataOutputs[0].Records[0].TryGetValue("PX_LAST", out string lastPrice1); 
            Assert.That(lastPrice1, Is.Not.Empty);
            
            finDataOutputs[0].Records[1].TryGetValue("ID_BB_UNIQUE", out string bbdId2); 
            Assert.That(bbdId2, Is.EqualTo("EQ0021695200001000"));
            finDataOutputs[0].Records[1].TryGetValue("PX_LAST", out string lastPrice2); 
            Assert.That(lastPrice2, Is.Not.Empty);
        }
        
        [Test]
        public void Get_OnAdhocGetDataWithNoIds_ShouldReturnEmptyData()
        {
            var finDataOutputs = _dlDataService.Get(new List<string>{}, DlTypes.ProgramTypes.Adhoc, DlTypes.DataTypes.GetData);
            CollectionAssert.IsEmpty(finDataOutputs);
        }
        
        [Test]
        public void Get_OnAdhocGetAction_ShouldThrowUnsupportedException()
        {
            var bbgIds = new List<string>{"EQ0010174300001000", "EQ0021695200001000"};
            Assert.Throws<NotSupportedException>(() =>
                _dlDataService.Get(bbgIds, DlTypes.ProgramTypes.Adhoc, DlTypes.DataTypes.GetActions));
        }
        
        [Test]
        public void Get_OnAScheduledGetData_ShouldThrowUnsupportedException()
        {
            var bbgIds = new List<string>{"EQ0010174300001000", "EQ0021695200001000"};
            Assert.Throws<NotSupportedException>(() =>
                _dlDataService.Get(bbgIds, DlTypes.ProgramTypes.Scheduled, DlTypes.DataTypes.GetData));
        }

    }
}