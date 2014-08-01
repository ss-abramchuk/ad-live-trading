using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

using Moq;
using WealthLab;

using RealTimeTrading.ADLiveTrading.Dispatcher;
using RealTimeTrading.ADLiveTrading.Helpers;
using RealTimeTrading.ADLiveTrading.BrokerProvider;
using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.RTTManager.Abstract;
using RealTimeTrading.RTTManager.Helpers;

namespace ADLiveTradingUnitTests
{
    [TestClass]
    public class ADBrokerProviderTests
    {
        Mock<IRTTSettingsProvider> _settingsProvider;
        Mock<IADSecurityProvider> _securityProvider;

        public ADBrokerProviderTests()
        {
            _settingsProvider = new Mock<IRTTSettingsProvider>();
            _settingsProvider.Setup(x => x.GetParameter("Username", string.Empty)).Returns("abramchuk_s");
            _settingsProvider.Setup(x => x.GetParameter("Password", string.Empty)).Returns("ta9dd4");

            _securityProvider = new Mock<IADSecurityProvider>();
        }

        [TestMethod]
        public void GetADAccountsTest()
        {
            ADBrokerProvider adBrokerProvider = new ADBrokerProvider();
            Type adBrokerProviderType = adBrokerProvider.GetType();

            ADProvider adProvider = new ADProvider(_settingsProvider.Object, _securityProvider.Object);

            FieldInfo adAccountProvider = adBrokerProviderType.GetField("_accountProvider", BindingFlags.Instance | BindingFlags.NonPublic);
            adAccountProvider.SetValue(adBrokerProvider, adProvider);

            MethodInfo adProviderGetADAccounts = adBrokerProviderType.GetMethod("GetADAccounts", BindingFlags.Instance | BindingFlags.NonPublic);

            List<ADAccount> accounts = (List<ADAccount>)adProviderGetADAccounts.Invoke(adBrokerProvider, null);

            Assert.IsNotNull(accounts, "Список accounts == null");
            Assert.IsTrue(accounts.Count > 0, "Количество элементов в списке accounts == 0");
            Assert.IsTrue(accounts.Exists(x => x.Treaty == "135258"), "В списке account не содержится счет с номером \"135258\"");
            Assert.IsTrue(accounts.Exists(x => x.Account == "135258-000"), "В списке account не содержится портфель с номером \"135258-000\"");
        }

        [TestMethod]
        public void GetADPositions()
        {
            ADBrokerProvider adBrokerProvider = new ADBrokerProvider();
            Type adBrokerProviderType = adBrokerProvider.GetType();

            ADProvider adProvider = new ADProvider(_settingsProvider.Object, _securityProvider.Object);

            FieldInfo adAccountProvider = adBrokerProviderType.GetField("_accountProvider", BindingFlags.Instance | BindingFlags.NonPublic);
            adAccountProvider.SetValue(adBrokerProvider, adProvider);

            MethodInfo adProviderGetADAccounts = adBrokerProviderType.GetMethod("GetADPositions", BindingFlags.Instance | BindingFlags.NonPublic);

            Object[] args = new object[1];
            args[0] = new ADAccount()
            {
                Account = "135258-000",
            };

            List<AccountPosition> positions = (List<AccountPosition>)adProviderGetADAccounts.Invoke(adBrokerProvider, args);

            Assert.IsNotNull(positions, "Список positions == null");
            Assert.IsTrue(positions.Count > 0, "Количество элементов в списке positions == 0");
            Assert.IsTrue(positions.Exists(x => x.Symbol == "MICEX_SHR.SBER3"), "В списке positions не содержится позиция с тикером \"MICEX_SHR.SBER3\"");
        }
    }
}
