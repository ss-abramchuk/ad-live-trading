using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Threading;

using Moq;
using ADLite;
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
    public class ADOrderProviderTests
    {
        Mock<IRTTSettingsProvider> _settingsProvider;
        Mock<IADSecurityProvider> _securityProvider;

        public ADOrderProviderTests()
        {
            _settingsProvider = new Mock<IRTTSettingsProvider>();
            _settingsProvider.Setup(x => x.GetParameter("Username", string.Empty)).Returns("abramchuk_s");
            _settingsProvider.Setup(x => x.GetParameter("Password", string.Empty)).Returns("ta9dd4");
            _settingsProvider.Setup(x => x.GetParameter("SendOrderTimeout", It.IsAny<int>())).Returns(-1);            

            _securityProvider = new Mock<IADSecurityProvider>();
        }

        [TestMethod]
        public void SendMarketOrderTest()
        {
            ADProvider adProvider = new ADProvider(_settingsProvider.Object, null);

            ADAccount account = new ADAccount()
            {
                Account = "135258-000",
                Section = "Основной"
            };

            Order order = new Order()
            {
                Symbol = "MICEX_SHR.SBER3",
                OrderType = OrderType.Market,
                AlertType = TradeType.Buy,
                Price = 95,
                Shares = 200,
                OrderID = Guid.NewGuid().ToString()
            };

            IAsyncResult result = adProvider.SendOrder(account, order, null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;
            
            Assert.IsTrue(invokeResult.State == StateCodes.stcSuccess, string.Format("State: {0}; Message: {1}", invokeResult.State, invokeResult.Message));
            Assert.Inconclusive(string.Format("State: {0}; Message: {1}", invokeResult.State, invokeResult.Message));
        }

        [TestMethod]
        public void SentStopOrder()
        {
            _settingsProvider.Setup(x => x.GetParameter("EnableSlippage", It.IsAny<bool>())).Returns(true);
            _settingsProvider.Setup(x => x.GetParameter("SlippageUnits", It.IsAny<double>())).Returns(1);

            ADProvider adProvider = new ADProvider(_settingsProvider.Object, null);

            ADAccount account = new ADAccount()
            {
                Account = "135258-000",
                Section = "Основной"
            };

            Order order = new Order()
            {
                Symbol = "MICEX_SHR.SBER3",
                OrderType = OrderType.Stop,
                AlertType = TradeType.Sell,
                Price = 90,
                Shares = 10,
                OrderID = Guid.NewGuid().ToString()
            };

            IAsyncResult result = adProvider.SendOrder(account, order, null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            Assert.IsTrue(invokeResult.State == StateCodes.stcSuccess, string.Format("State: {0}; Message: {1}", invokeResult.State, invokeResult.Message));
            Assert.Inconclusive(string.Format("State: {0}; Message: {1}", invokeResult.State, invokeResult.Message));
        }

        [TestMethod]
        public void CancelOrderTest()
        {
            ADProvider adProvider = new ADProvider(_settingsProvider.Object, null);

            ADAccount account = new ADAccount()
            {
                Account = "135258-000",
                Section = "Основной"
            };

            Order order = new Order()
            {
                Symbol = "MICEX_SHR.SBER3",
                OrderType = OrderType.Stop,
                AlertType = TradeType.Sell,
                Price = 90,
                Shares = 10,
                OrderID = Guid.NewGuid().ToString()
            };

            IAsyncResult sendResult = adProvider.SendOrder(account, order, null);

            sendResult.AsyncWaitHandle.WaitOne();

            Thread.Sleep(5000);

            IAsyncResult result = adProvider.CancelOrder(account, order, null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            Assert.IsTrue(invokeResult.State == StateCodes.stcSuccess, string.Format("State: {0}; Message: {1}", invokeResult.State, invokeResult.Message));
            Assert.Inconclusive(string.Format("State: {0}; Message: {1}", invokeResult.State, invokeResult.Message));
        }
    }
}
