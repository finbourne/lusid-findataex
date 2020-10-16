using System;
using System.Collections.Generic;
using Lusid.FinDataEx.DataLicense.Service;
using Lusid.FinDataEx.DataLicense.Service.Call;
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

        // BBG DL Calls
        private GetDataBbgCall _getDataBbgCall;

        [SetUp]
        public void SetUp()
        {
            _perSecurityWs = new PerSecurityWsFactory().CreateDefault();
            _getDataBbgCall = new GetDataBbgCall(_perSecurityWs);
            _dlDataService = new DlDataService();
        }

        [Test]
        public void Get_OnAdhocGetData_ShouldReturnOutput()
        {
            var bbgIds = new List<string>{"BBG000BPHFS9", "BBG000BVPV84"};
            var finDataOutputs = _dlDataService.Get(_getDataBbgCall, bbgIds, DlTypes.ProgramTypes.Adhoc);
            
            Assert.That(finDataOutputs.Count, Is.EqualTo(1));
            CollectionAssert.AreEqual(finDataOutputs[0].Header, new List<string>(){"ID_BB_GLOBAL","PX_LAST"});

            // check data in records (changing prices mean just need to check its populated and not the exact number)
            Assert.That(finDataOutputs[0].Records.Count, Is.EqualTo(2));
            finDataOutputs[0].Records[0].TryGetValue("ID_BB_GLOBAL", out string bbdId1); 
            Assert.That(bbdId1, Is.EqualTo("BBG000BPHFS9"));
            finDataOutputs[0].Records[0].TryGetValue("PX_LAST", out string lastPrice1); 
            Assert.That(lastPrice1, Is.Not.Empty);
            
            finDataOutputs[0].Records[1].TryGetValue("ID_BB_GLOBAL", out string bbdId2); 
            Assert.That(bbdId2, Is.EqualTo("BBG000BVPV84"));
            finDataOutputs[0].Records[1].TryGetValue("PX_LAST", out string lastPrice2); 
            Assert.That(lastPrice2, Is.Not.Empty);
        }
        
        [Test]
        public void Get_OnAdhocGetDataWithNoIds_ShouldReturnEmptyData()
        {
            var finDataOutputs = _dlDataService.Get(_getDataBbgCall, new List<string>(), DlTypes.ProgramTypes.Adhoc);
            CollectionAssert.IsEmpty(finDataOutputs);
        }
        
        [Test]
        public void Get_OnAScheduledGetData_ShouldThrowUnsupportedException()
        {
            var bbgIds = new List<string>{"BBG000BPHFS9", "BBG000BVPV84"};
            Assert.Throws<NotSupportedException>(() =>
                _dlDataService.Get(_getDataBbgCall, bbgIds, DlTypes.ProgramTypes.Scheduled));
        }

    }
}