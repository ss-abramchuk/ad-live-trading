using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using ADLite;
using WealthLab;
using log4net;

using WLDSolutions.LiveTradingManager.Abstract;
using WLDSolutions.LiveTradingManager.Helpers;
using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.Dispatcher
{
    internal sealed class ADProvider : IADStaticDataProvider, IADStreamingProvider, IADAccountProvider, IADOrderProvider
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ADProvider));

        private ILTSettingsProvider _settingsProvider;

        private ADWrapper _adWrapper;

        public event Action<string> NewQuote;
        public event Action<string> UpdateOrder;
        public event Action<string> UpdateTrades;
        public event Action<int, int, string, eCommandResult> OrderConfirmed;

        public ADProvider(ILTSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            _adWrapper = new ADWrapper(_settingsProvider);
            _adWrapper.NewQuote += OnNewQuote;
            _adWrapper.UpdateOrder += UpdateOrderStatus;
            _adWrapper.UpdateTrades += UpdateTradesStatus;
            _adWrapper.SendOrderConfirmed += SendOrderConfirmed;
        }       

        #region Реализация IADStaticDataProvider

        public IAsyncResult GetMarkets(bool allowTrade, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new DataQuery()
            {
                Table = "trade_places",
                Fields = "place_code, place_name",
                Conditions = allowTrade ? "allow_trade = \"Y\"" : string.Empty
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult GetSymbols(string market, bool allowShort, bool allowPawn, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new DataQuery()
            {
                Table = "papers",
                Fields = "p_code, ANSI_name",
                Conditions = string.Concat("place_code = ", market, allowShort ? " and allow_short = \"Y\"" : " and allow_short = \"N\"", allowPawn ? " and allow_pawn = \"Y\"" : " and allow_pawn = \"N\"")
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult GetStaticData(string market, string symbol, BarDataScale dataScale, DateTime startDate, DateTime endDate, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetHistory;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            int period = GetPeriod(dataScale);

            description.Query = new HistoryQuery()
            {
                Market = market,
                Symbol = symbol,
                Period = period,
                StartDate = startDate,
                EndDate = endDate
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }        

        private int GetPeriod(BarDataScale dataScale)
        {
            int result = 0;

            switch (dataScale.Scale)
            {
                case BarScale.Minute:
                    switch (dataScale.BarInterval)
                    {
                        case (1):
                            result = 0;
                            break;

                        case (5):
                            result = 1;
                            break;

                        case (10):
                            result = 2;
                            break;

                        case (15):
                            result = 3;
                            break;

                        case (30):
                            result = 4;
                            break;

                        case (60):
                            result = 5;
                            break;
                    }
                    break;

                case BarScale.Daily:
                    result = 6;
                    break;

                case BarScale.Weekly:
                    result = 7;
                    break;

                case (BarScale.Monthly):
                    result = 8;
                    break;

                case (BarScale.Yearly):
                    result = 9;
                    break;
            }

            return result;
        }

        #endregion

        #region Реализация IADStreamingProvider

        private void OnNewQuote(string quote)
        {
            if (NewQuote != null)
                NewQuote(quote);
        }

        public void StreamDataSubscribe(object symbols)
        {
            string papersNumbers = GetNumbers((List<SymbolDescription>)symbols);

            SubscribeQuery query = GetStreamDataSubscribeQuery(papersNumbers);

            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.Subscribe;
            description.AsyncResult = asyncResult;
            description.CallBack = null;

            description.Query = query;

            _adWrapper.Invoke(description);
        }

        private string GetNumbers(List<SymbolDescription> symbols)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = null;

            string conditions = string.Empty;

            foreach (SymbolDescription symbolDescription in symbols)
            {
                conditions += string.Format("(place_code = {0} and p_code = {1}){2}", symbolDescription.MarketCode, symbolDescription.SymbolCode,
                    symbols.IndexOf(symbolDescription) != symbols.Count - 1 ? " or " : string.Empty);
            }

            description.Query = new DataQuery()
            {
                Table = "papers",
                Fields = "paper_no",
                Conditions = conditions
            };

            _adWrapper.Invoke(description);

            asyncResult.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)asyncResult.AsyncState;

            return invokeResult.Value;
        }

        private SubscribeQuery GetStreamDataSubscribeQuery(string numbersRaw)
        {
            try
            {
                //SubscribeQuery query = new SubscribeQuery()
                //    {
                //        Table = "fin_info",
                //        Fields = "place_code, p_code, last_update_date, last_update_time, last_price, open_price, close_price, last_qty, lot_size",
                //        Filter = "FI"
                //    };

                SubscribeQuery query = new SubscribeQuery()
                {
                    Table = "all_trades",
                    Fields = "place_code, p_code, ts_time, price, qty",
                    Filter = "AT"
                };

                numbersRaw = numbersRaw.Replace("\r\n", string.Empty);

                query.FilterConditions = numbersRaw.TrimEnd('|');
                query.SubscribeConditions = string.Concat("paper_no = ", numbersRaw.Replace("|", ",").TrimEnd(','));

                return query;
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                return new SubscribeQuery();
            }
        }

        public void StreamDataUnsubscribe()
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.Unsubscribe;
            description.AsyncResult = asyncResult;
            description.CallBack = null;

            SubscribeQuery query = new SubscribeQuery()
            {
                Table = "fin_info",
                Filter = "FI"
            };

            description.Query = query;

            _adWrapper.Invoke(description);
        }

        #endregion        

        #region Реализация IADAccountProvider

        public IAsyncResult GetAccounts(AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new DataQuery()
            {
                Table = "PORTF_SECTION_BAL",
                Fields = "acc_code, forward_bal, money, margin, section_code, deposit, var_margin, treaty",
                Conditions = string.Empty
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult GetPositions(ADAccount account, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new DataQuery()
            {
                Table = "BALANCE",
                Fields = "place_code, p_code, real_rest, balance_price, assessed_price, forword_rest, income_rest, i_last_update",
                Conditions = account.Section != "ФОРТС" ? string.Format("acc_code = {0} and board_code <> FUTURES", account.Account) :
                    string.Format("acc_code = {0} and board_code = FUTURES", account.Account)
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        #endregion

        #region Реализация IADOrderProvider        

        public IAsyncResult SendOrder(ADAccount account, Order order, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.CreateOrder;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new OrderQuery()
            {
                Account = account.Account,
                SymbolDescription = GetSymbolDescription(order.Symbol) ?? new SymbolDescription() { MarketCode = null, SymbolCode = null },
                OrderType = order.OrderType,
                TradeType = order.AlertType,
                Qty = order.Shares,
                Price = order.Price,
                Comment = string.Format("ADLT/{0}/", order.OrderID)
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult CancelOrder(ADAccount account, Order order, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.CancelOrder;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new OrderQuery()
            {
                Treaty = account.Treaty,
                Account = account.Account,
                SymbolDescription = GetSymbolDescription(order.Symbol) ?? new SymbolDescription() { MarketCode = null, SymbolCode = null },
                OrderType = order.OrderType,
                TradeType = order.AlertType,
                Qty = order.Shares,
                Price = order.Price,
                Comment = string.Format("ADLT/{0}/", order.OrderID)
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult SubscribeOrderUpdates(AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.Subscribe;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new SubscribeQuery()
            {
                Table = "orders",
                Fields = "ord_no, status, qty, rest, ts_time, comments",
                Filter = null
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult SubscribeTradesUpdates(AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.Subscribe;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new SubscribeQuery()
            {
                Table = "trades",
                Fields = "trd_no, ord_no, price, qty, paper_rest, ts_time, comments",
                Filter = null
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult UpdateOrderStatus(Order order, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new DataQuery()
            {
                Table = "orders",
                Fields = "ord_no, status, qty, rest, ts_time",
                Conditions = string.Format("comments like ADLT/{0}/", order.OrderID)
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        public IAsyncResult UpdateTradesStatus(Order order, AsyncCallback callBack)
        {
            AsyncResult asyncResult = new AsyncResult();
            asyncResult.AsyncWaitHandle = new AutoResetEvent(false);
            asyncResult.IsCompleted = false;
            asyncResult.AsyncState = new InvokeResult();

            InvokeDescription description = new InvokeDescription();
            description.Type = InvokeType.GetData;
            description.AsyncResult = asyncResult;
            description.CallBack = callBack;

            description.Query = new DataQuery()
            {
                Table = "trades",
                Fields = "trd_no, ord_no, price, qty, paper_rest, ts_time",
                Conditions = string.Format("comments like ADLT/{0}/", order.OrderID)
            };

            _adWrapper.Invoke(description);

            return asyncResult;
        }

        private void SendOrderConfirmed(int ID, int OrderNum, string Message, eCommandResult Status)
        {
            if (OrderConfirmed != null)
                OrderConfirmed(ID, OrderNum, Message, Status);
        } 

        private void UpdateOrderStatus(string orderRaw)
        {
            if (UpdateOrder != null)
                UpdateOrder(orderRaw);
        }

        private void UpdateTradesStatus(string tradesRaw)
        {
            if (UpdateTrades != null)
                UpdateTrades(tradesRaw);
        }
        
        public SymbolDescription GetSymbolDescription(string symbol)
        {
            try
            {
                SymbolDescription description = new SymbolDescription();

                description.MarketCode = symbol.Substring(0, symbol.IndexOf("."));
                description.SymbolCode = symbol.Substring(symbol.IndexOf(".") + 1, symbol.Length - symbol.IndexOf(".") - 1);

                return description;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion

        #region Реализация IADConnectionProvider

        public bool Connected
        {
            get { return _adWrapper.Connected; }
        }

        #endregion
    }
}
