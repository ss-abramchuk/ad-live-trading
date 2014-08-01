using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using WealthLab;

using RealTimeTrading.ADLiveTrading.Dispatcher;
using RealTimeTrading.ADLiveTrading.Helpers;
using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.RTTManager.Abstract;
using RealTimeTrading.RTTManager.Helpers;

namespace ADLiveTradingUnitTests
{
    [TestClass]
    public class ADProviderTests
    {
        Mock<IRTTSettingsProvider> _settingsProvider;
        Mock<IADSecurityProvider> _securityProvider;

        public ADProviderTests()
        {
            _settingsProvider = new Mock<IRTTSettingsProvider>();
            _settingsProvider.Setup(x => x.GetParameter("Username", string.Empty)).Returns("abramchuk_s");
            _settingsProvider.Setup(x => x.GetParameter("Password", string.Empty)).Returns("ta9dd4");

            _securityProvider = new Mock<IADSecurityProvider>();
        }

        [TestMethod]
        public void GetMarketsTest()
        {
            ADProvider adProvider = new ADProvider(_settingsProvider.Object, _securityProvider.Object);

            IAsyncResult result = adProvider.GetMarkets(true, null);
            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            Assert.IsFalse(string.IsNullOrWhiteSpace(invokeResult.Value), "Список рынков пуст");
            Assert.IsTrue(invokeResult.Value.Contains("FORTS"), "Список рынков не содержит FORTS");
        }
    }
}
