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
    public class ADAccountsProviderTests
    {
        Mock<IRTTSettingsProvider> _settingsProvider;
        Mock<IADSecurityProvider> _securityProvider;

        public ADAccountsProviderTests()
        {
            _settingsProvider = new Mock<IRTTSettingsProvider>();
            _settingsProvider.Setup(x => x.GetParameter("Username", string.Empty)).Returns("abramchuk_s");
            _settingsProvider.Setup(x => x.GetParameter("Password", string.Empty)).Returns("ta9dd4");

            _securityProvider = new Mock<IADSecurityProvider>();
        }

        [TestMethod]
        public void GetAccountsTest()
        {
            ADProvider adProvider = new ADProvider(_settingsProvider.Object, null);

            IAsyncResult result = adProvider.GetAccounts(null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            Assert.IsTrue(invokeResult.Value.Contains("Основной"), "Строка не содержит раздела рынка: Основной");
            Assert.IsTrue(invokeResult.Value.Contains("135258-000"), "Строка не содержит номера счета: 135258-000");
        }

        [TestMethod]
        public void GetPositionsTest()
        {
            ADProvider adProvider = new ADProvider(_settingsProvider.Object, null);

            ADAccount account = new ADAccount()
            {
                Account = "135258-000",
                Section = "Основной"
            };

            IAsyncResult result = adProvider.GetPositions(account, null);

            result.AsyncWaitHandle.WaitOne();            

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            Assert.IsTrue(invokeResult.Value.Contains("SBER3"), "Строка не содержит тикер: SBER3");
        }

        [TestMethod]
        public void ConvertToDateTimeTest()
        {
            ADProvider adProvider = new ADProvider(_settingsProvider.Object, null);

            ADAccount account = new ADAccount()
            {
                Account = "135258-000",
                Section = "Основной"
            };

            IAsyncResult result = adProvider.GetPositions(account, null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            string[] positionsRaw = invokeResult.Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string positionRaw in positionsRaw)
            {
                string[] positionInfo = positionRaw.Split(new string[] { "|" }, StringSplitOptions.None);

                long count = Convert.ToInt64(positionInfo[7]);

                TimeSpan ticks = TimeSpan.FromSeconds(count);

                DateTime startDate = new DateTime(1999, 1, 1, 0, 0, 0);

                DateTime updDate = new DateTime(1999, 1, 1, 0, 0, 0) + ticks;
            }

            string source = null;

            string[] test = source.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Inconclusive();
        }
    }
}
