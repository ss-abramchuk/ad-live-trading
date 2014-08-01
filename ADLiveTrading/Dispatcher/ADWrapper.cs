using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using System.Management;
using Microsoft.Win32;

using ADLite;
using log4net;

using WLDSolutions.LiveTradingManager.Abstract;
using RealTimeTrading.ADLiveTrading.Helpers;
using RealTimeTrading.ADLiveTrading.Abstract;

namespace RealTimeTrading.ADLiveTrading.Dispatcher
{
    internal sealed class ADWrapper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ADWrapper));

        private ILTSettingsProvider _settingsProvider;

        private AlfaDirect _adProvider;        

        private AutoResetEvent _invokeTrigger = new AutoResetEvent(false);

        private ConcurrentQueue<InvokeDescription> _operationsQueue;

        public event Action<string> NewQuote;
        public event Action<string> UpdateOrder;
        public event Action<string> UpdateTrades;
        public event Action<int, int, string, eCommandResult> SendOrderConfirmed;

        public bool Connected
        {
            get;
            private set;
        }

        public ADWrapper(ILTSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider; 

            _operationsQueue = new ConcurrentQueue<InvokeDescription>();

            Task.Factory.StartNew(InvokeProcess, TaskCreationOptions.LongRunning);
        }

        private void Initialize()
        {
            try
            {
                _adProvider = new AlfaDirectClass();

                Connect();

                _adProvider.OnConnectionChanged += OnConnectionChanged;
                _adProvider.OnTableChanged += OnTableChanged;
                _adProvider.OrderConfirmed += OrderConfirmed;
            }
            catch (Exception ex)
            {
                _adProvider = null;
                _logger.Error(ex);
            }
        }        

        private void Connect()
        {
            string userName = _settingsProvider.GetParameter("Username", string.Empty);
            string password = _settingsProvider.GetParameter("Password", string.Empty);

            try
            {
                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                {
                    _adProvider.UserName = userName;
                    _adProvider.Password = password;

                    _adProvider.Connected = true;
                }

                Connected = _adProvider.Connected;
            }
            catch (Exception ex)
            {
                Connected = false;
                _logger.Error(ex);
            }
        }

        private void InvokeProcess()
        {
            Initialize();

            while (true)
            {
                _invokeTrigger.WaitOne();

                while (_operationsQueue.Count > 0)
                {
                    InvokeDescription description;
                    _operationsQueue.TryDequeue(out description);

                    InvokeResult result = (InvokeResult)description.AsyncResult.AsyncState;
                    result.Type = description.Type;

                    switch (description.Type)
                    {
                        case InvokeType.GetData:
                            DataQuery dataQuery = (DataQuery)description.Query;
                            result.Value = GetDBData(dataQuery.Table, dataQuery.Fields, dataQuery.Conditions);
                            break;

                        case InvokeType.GetHistory:
                            HistoryQuery historyQuery = (HistoryQuery)description.Query;
                            result.Value = GetHistoryData(historyQuery.Market, historyQuery.Symbol, historyQuery.Period, historyQuery.StartDate, historyQuery.EndDate);
                            break;

                        case InvokeType.Subscribe:
                            SubscribeQuery subscribeQuery = (SubscribeQuery)description.Query;
                            Subscribe(subscribeQuery.Table, subscribeQuery.Fields, subscribeQuery.Filter, subscribeQuery.FilterConditions, subscribeQuery.SubscribeConditions);
                            break;

                        case InvokeType.Unsubscribe:
                            SubscribeQuery unsubscribeQuery = (SubscribeQuery)description.Query;
                            Unsubscribe(unsubscribeQuery.Table, unsubscribeQuery.Filter);
                            break;

                        case InvokeType.CreateOrder:
                            OrderQuery createOrderQuery = (OrderQuery)description.Query;
                            result.Value = CreateOrder(createOrderQuery);
                            break;

                        case InvokeType.CancelOrder:
                            OrderQuery cancelOrderQuery = (OrderQuery)description.Query;
                            CancelOrder(cancelOrderQuery);
                            break;
                    }

                    try
                    {
                        result.State = _adProvider.LastResult;
                        result.Message = _adProvider.LastResultMsg;
                    }
                    catch (Exception ex)
                    {
                        result.State = StateCodes.stcCriticalClientError;
                        result.Message = ex.ToString();
                    }

                    AsyncResult asyncResult = (AsyncResult)description.AsyncResult;
                    asyncResult.IsCompleted = true;

                    AutoResetEvent resultTrigger = (AutoResetEvent)asyncResult.AsyncWaitHandle;
                    resultTrigger.Set();

                    if (description.CallBack != null)
                        description.CallBack(description.AsyncResult);
                }
            }
        }

        public void Invoke(InvokeDescription description)
        {
            _operationsQueue.Enqueue(description);

            _invokeTrigger.Set();
        }

        private string GetDBData(string table, string fields, string condition)
        {
            string result;

            try
            {
                result = _adProvider.GetLocalDBData(table, fields, condition);
            }
            catch (Exception ex)
            {
                result = string.Empty;
                _logger.Error(ex);
            }

            return result;
        }

        private string GetHistoryData(string market, string symbol, int period, DateTime startDate, DateTime endDate)
        {
            string result;

            try
            {
                int timeout = _settingsProvider.GetParameter("GetHistoryTimeout", 10);

                result = (string)_adProvider.GetArchiveFinInfo(market, symbol, period, startDate, endDate, 3, timeout);
            }
            catch (Exception ex)
            {
                result = string.Empty;
                _logger.Error(ex);
            }

            return result;
        }

        private void Subscribe(string table, string fields, string filter, string filterConditions, string subscribeConditions)
        {
            try
            {
                if (!string.IsNullOrEmpty(filter))
                    _adProvider.set_GlobalFilter(filter, filterConditions);

                string result = string.Empty;
                _adProvider.SubscribeTable(table, fields, subscribeConditions, eSubsctibeOptions.UpdatesOnly, out result);
            }
            catch(Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Unsubscribe(string table, string filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(filter))
                    _adProvider.set_GlobalFilter(filter, string.Empty);

                string result = string.Empty;
                _adProvider.UnSubscribeTable(table, "", out result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private string CreateOrder(OrderQuery orderQuery)
        {
            int timeout = _settingsProvider.GetParameter("SendOrderTimeout", -1);

            string tradeType = orderQuery.TradeType == WealthLab.TradeType.Buy || orderQuery.TradeType == WealthLab.TradeType.Cover ? "B" : "S";

            DateTime tif = GetTIF(DateTime.Now);

            int result = 0;

            switch (orderQuery.OrderType)
            {
                case WealthLab.OrderType.Market:
                    {
                        string priceRaw = GetDBData("fin_info", "last_price", string.Format("place_code = {0} and p_code = {1}", orderQuery.SymbolDescription.MarketCode,
                            orderQuery.SymbolDescription.SymbolCode));

                        if (!string.IsNullOrWhiteSpace(priceRaw))
                            priceRaw = priceRaw.TrimEnd('|', '\r', '\n');
                        else
                            _logger.Debug("[CreateOrder] priceRaw == empty");

                        double price = 0;
                        double.TryParse(priceRaw, out price);

                        double marketPrice = tradeType == "B" ? marketPrice = price + price * 0.005 : marketPrice = price - price * 0.005;

                        try
                        {
                            result = _adProvider.CreateLimitOrder(orderQuery.Account, orderQuery.SymbolDescription.MarketCode, orderQuery.SymbolDescription.SymbolCode,
                                DateTime.Now.AddDays(30), orderQuery.Comment, "RUR", tradeType, (int)orderQuery.Qty, marketPrice, null, null,
                                null, null, null, null, null, null, null, null, null, null, null, null, null, null, timeout);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                    break;

                case WealthLab.OrderType.Limit:
                    {                        
                        try
                        {
                            result = _adProvider.CreateLimitOrder(orderQuery.Account, orderQuery.SymbolDescription.MarketCode, orderQuery.SymbolDescription.SymbolCode,
                                DateTime.Now.AddDays(30), orderQuery.Comment, "RUR", tradeType, (int)orderQuery.Qty, orderQuery.Price, null, null,
                                null, null, null, null, null, null, null, null, null, null, null, null, null, null, timeout);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                    break;

                case WealthLab.OrderType.Stop:
                    {
                        double slippage = 0;

                        if (_settingsProvider.GetParameter("EnableSlippage", false))
                        {
                            if (orderQuery.SymbolDescription.MarketCode == "FORTS")
                            {
                                slippage = _settingsProvider.GetParameter("SlippageTicks", 1);

                                string tickRaw = GetDBData("fin_info", "price_step", string.Format("place_code = {0} and p_code = {1}", orderQuery.SymbolDescription.MarketCode,
                                    orderQuery.SymbolDescription.SymbolCode));

                                if (!string.IsNullOrWhiteSpace(tickRaw))
                                    tickRaw = tickRaw.TrimEnd('|', '\r', '\n');
                                else
                                    _logger.Debug("[CreateOrder] tickRaw == empty");

                                double tick = 0;
                                double.TryParse(tickRaw, out tick);

                                slippage *= tick;
                            }
                            else
                            {
                                slippage = _settingsProvider.GetParameter("SlippageUnits", 0.0);

                                slippage = orderQuery.Price * slippage / 100;
                            }
                        }

                        try
                        {
                            result = _adProvider.CreateStopOrder(orderQuery.Account, orderQuery.SymbolDescription.MarketCode, orderQuery.SymbolDescription.SymbolCode,
                                DateTime.Now.AddDays(30), orderQuery.Comment, "RUR", tradeType, (int)orderQuery.Qty, orderQuery.Price, slippage, null, timeout);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                        
                    }
                    break;
            }

            return result.ToString();
        }

        private void CancelOrder(OrderQuery orderQuery)
        {
            int timeout = _settingsProvider.GetParameter("SendOrderTimeout", -1);

            string whereCondition = string.Format("comments like {0}",orderQuery.Comment);
            string orderInfo = GetDBData("orders", "ord_no", whereCondition) ?? "-1";

            _adProvider.DropOrder(Convert.ToInt32(orderInfo.TrimEnd('|', '\r', '\n')), null, null, null, null, null, timeout);
        }

        private DateTime GetTIF(DateTime dateTime)
        {
            string tif = _settingsProvider.GetParameter("TIF", "Month");

            switch (tif)
            {
                case "Today":
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 50, 00);

                case "Days":
                    int days = _settingsProvider.GetParameter("TIFDays", 1);
                    return dateTime.AddDays(days);

                default:
                    return dateTime.AddMonths(1);
            }
        }

        private void OrderConfirmed(int ID, int OrderNum, string Message, eCommandResult Status)
        {
            if (SendOrderConfirmed != null)
                SendOrderConfirmed(ID, OrderNum, Message, Status);
        }

        private void OnTableChanged(string TableName, string TableParams, object Data, object FieldTypes)
        {
            switch (TableName)
            {
                case "all_trades":
                    Task.Factory.StartNew(x =>
                    {
                        string data;

                        try
                        {
                            data = (string)x;
                        }
                        catch
                        {
                            data = null;
                        }

                        if (NewQuote != null && !string.IsNullOrWhiteSpace(data))
                            NewQuote(data);
                    }, Data);
                    break;

                case "fin_info":
                    Task.Factory.StartNew(x =>
                    {
                        string data;

                        try
                        {
                            data = (string)x;
                        }
                        catch
                        {
                            data = null;
                        }

                        if (NewQuote != null && !string.IsNullOrWhiteSpace(data))
                            NewQuote(data);
                    }, Data);
                    break;

                case "orders":
                    Task.Factory.StartNew(x =>
                    {
                        string data;

                        try
                        {
                            data = (string)x;
                        }
                        catch
                        {
                            data = null;
                        }

                        if (UpdateOrder != null && !string.IsNullOrWhiteSpace(data))
                            UpdateOrder(data);
                    }, Data);
                    break;

                case "trades":
                    Task.Factory.StartNew(x =>
                    {
                        string data;

                        try
                        {
                            data = (string)x;
                        }
                        catch
                        {
                            data = null;
                        }

                        if (UpdateTrades != null && !string.IsNullOrWhiteSpace(data))
                            UpdateTrades(data);
                    }, Data);
                    break;
            }
        }

        private void OnConnectionChanged(eConnectionState State)
        {
            if (State == eConnectionState.Connected)
                Connected = true;
            else
                Connected = false;
        }
    }
}
