using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using WealthLab;
using log4net;
using ADLite;

using WLDSolutions.LiveTradingManager.Abstract;
using WLDSolutions.LiveTradingManager.Helpers;
using RealTimeTrading.ADLiveTrading.Dispatcher;
using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.BrokerProvider
{
    public class ADBrokerProvider : LTBrokerProvider
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ADBrokerProvider));

        private ILTSettingsProvider _settingsProvider;
        private IADAccountProvider _accountProvider;
        private IADOrderProvider _orderProvider;

        private List<ADAccount> _accounts;

        private System.Threading.Timer _updateTimer;

        private object _orderLocker = new object();

        public override List<Order> Orders
        {
            get;
            set;
        }

        public ADBrokerProvider()
        {
            Orders = new List<Order>();
        }

        public override void Initialize()
        {
            _settingsProvider = ADDispatcher.Instance.SettingsProvider;
            _accountProvider = ADDispatcher.Instance.AccountProvider;
            _orderProvider = ADDispatcher.Instance.OrderProvider;

            _accounts = GetADAccounts();

            for (int i = 0; i < _accounts.Count; i++)
            {
                _accounts[i].Positions = GetADPositions(_accounts[i]);
            }

            _orderProvider.UpdateOrder += UpdateOrder;
            _orderProvider.UpdateTrades += UpdateTrades;
            _orderProvider.OrderConfirmed += OrderConfirmed;

            IAsyncResult resultOrderUpdates = _orderProvider.SubscribeOrderUpdates(null);
            resultOrderUpdates.AsyncWaitHandle.WaitOne();

            IAsyncResult resultTradesUpdates = _orderProvider.SubscribeTradesUpdates(null);
            resultTradesUpdates.AsyncWaitHandle.WaitOne();

            int updateInterval = 5;

            updateInterval = _settingsProvider.GetParameter("UpdateInterval", 5);

            _updateTimer = new System.Threading.Timer(new TimerCallback(DelayedUpdates), null, updateInterval * 1000, updateInterval * 1000);
        }        

        #region Получение информации об аккаунтах и открытых позициях

        public override event Action<LTAccount> AccountUpdate;

        private List<ADAccount> GetADAccounts()
        {
            IAsyncResult result = _accountProvider.GetAccounts(null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            List<ADAccount> accounts = new List<ADAccount>();

            if (invokeResult.State == StateCodes.stcSuccess)
            {
                if (string.IsNullOrWhiteSpace(invokeResult.Value))
                {
                    _logger.Debug("Аккаунты не найдены в таблицах терминала");
                    return accounts;
                }

                string[] accountsRaw = invokeResult.Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string accountRaw in accountsRaw)
                {
                    try
                    {
                        string[] accountInfo = accountRaw.Split(new string[] { "|" }, StringSplitOptions.None);

                        ADAccount account = new ADAccount();

                        account.ProviderName = "ADLiveTrading";

                        account.Treaty = accountInfo[7];
                        account.Account = accountInfo[0];
                        account.Section = accountInfo[4];
                        account.AccountNumber = string.Format("{0}-{1}", accountInfo[0], accountInfo[4]);

                        account.IsPaperAccount = false;
                        account.AccountValue = double.Parse(accountInfo[1]);
                        account.AccountValueTimeStamp = DateTime.Now;
                        account.AvailableCash = double.Parse(accountInfo[2]);
                        account.BuyingPower = double.Parse(accountInfo[2]);

                        accounts.Add(account);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
            else
            {
                _logger.Error(invokeResult.Message);
            }

            return accounts;
        }

        private List<AccountPosition> GetADPositions(ADAccount account)
        {
            IAsyncResult result = _accountProvider.GetPositions(account, null);

            result.AsyncWaitHandle.WaitOne();

            InvokeResult invokeResult = (InvokeResult)result.AsyncState;

            List<AccountPosition> positions = new List<AccountPosition>();

            if (invokeResult.State == StateCodes.stcSuccess)
            {
                if (string.IsNullOrWhiteSpace(invokeResult.Value))
                {
                    _logger.Debug(string.Format("Аккаунт: {0}. Не найдено открытых позиций.", account.AccountNumber));
                    return positions;
                }

                string[] positionsRaw = invokeResult.Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string positionRaw in positionsRaw)
                {
                    try
                    {
                        string[] positionInfo = positionRaw.Split(new string[] { "|" }, StringSplitOptions.None);                                               

                        if (Convert.ToDouble(positionInfo[5]) != 0 && !positionInfo[1].Contains("money"))
                        {
                            AccountPosition position = new AccountPosition();

                            position.Account = account;

                            position.Symbol = string.Format("{0}.{1}", positionInfo[0], positionInfo[1]);
                            position.Quantity = Math.Abs(Convert.ToDouble(positionInfo[5]));
                            position.EntryPrice = Convert.ToDouble(positionInfo[3]);
                            position.LastPrice = Convert.ToDouble(positionInfo[4]);

                            position.PositionType = Convert.ToDouble(positionInfo[5]) > 0 ? PositionType.Long : PositionType.Short;

                            positions.Add(position);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
            else
            {
                _logger.Error(invokeResult.Message);
            }

            return positions;
        }

        public override List<LTAccount> GetAccounts()
        {
            List<LTAccount> result = new List<LTAccount>();
            result.AddRange(_accounts);

            return result;
        }

        public override void UpdateAccounts()
        {
            List<ADAccount> updAccounts = GetADAccounts();

            foreach (ADAccount updAccount in updAccounts)
            {
                ADAccount account = _accounts.Find(x => x.AccountNumber == updAccount.AccountNumber);

                if (account != null)
                {
                    account.AccountValue = updAccount.AccountValue;
                    account.AccountValueTimeStamp = DateTime.Now;
                    account.AvailableCash = updAccount.AvailableCash;
                    account.BuyingPower = updAccount.BuyingPower;

                    account.Positions = GetADPositions(account);

                    if (AccountUpdate != null)
                        AccountUpdate(account);
                }
            }
        }

        private void DelayedUpdates(object state)
        {
            if(_accountProvider.Connected)
                UpdateAccounts();
        }

        public override List<string> AccountTradeTypes(string account)
        {
            return new List<string>() { "Default" };
        }

        #endregion

        #region Выставление заявок и обновление их статусов

        public override event Action<OrderUpdateInfo> OrderUpdate;

        public override void PlaceOrder(Order order)
        {
            ADAccount account = _accounts.Find(x => x.AccountNumber == order.Account);

            if (account != null)
            {
                Orders.Add(order);

                IAsyncResult result = _orderProvider.SendOrder(account, order, null);

                result.AsyncWaitHandle.WaitOne();

                InvokeResult invokeResult = (InvokeResult)result.AsyncState;
                order.BrokerTag = invokeResult.Value;

                OrderUpdateInfo updateInfo = new OrderUpdateInfo()
                {
                    OrderID = order.OrderID,
                    OrderStatus = invokeResult.State == StateCodes.stcSuccess ? OrderStatus.Submitted : OrderStatus.Error,
                    TimeStamp = DateTime.Now,
                    FillPrice = 0,
                    FillQty = 0,
                    Code = 0,
                    Message = invokeResult.Message
                };

                if (OrderUpdate != null)
                    OrderUpdate(updateInfo);
            }

            Thread.Sleep(550);
        }

        public override void CancelOrder(Order order)
        {
            ADAccount account = _accounts.Find(x => x.AccountNumber == order.Account);

            if (account != null)
            {
                IAsyncResult result = _orderProvider.CancelOrder(account, order, null);

                result.AsyncWaitHandle.WaitOne();

                InvokeResult invokeResult = (InvokeResult)result.AsyncState;

                OrderUpdateInfo updateInfo = new OrderUpdateInfo()
                {
                    OrderID = order.OrderID,
                    OrderStatus = invokeResult.State == StateCodes.stcSuccess ? OrderStatus.CancelPending : OrderStatus.Error,
                    TimeStamp = DateTime.Now,
                    FillPrice = 0,
                    FillQty = 0,
                    Code = 0,
                    Message = invokeResult.Message
                };

                if (OrderUpdate != null)
                    OrderUpdate(updateInfo);
            }
        }

        public override void OrderStatusUpdate(Order order)
        {
            IAsyncResult resultOrder = _orderProvider.UpdateOrderStatus(order, null);

            resultOrder.AsyncWaitHandle.WaitOne();

            IAsyncResult resultTrades = _orderProvider.UpdateTradesStatus(order, null);

            resultTrades.AsyncWaitHandle.WaitOne();            

            string[] orderInfo;
            try
            {
                InvokeResult invokeResultOrder = (InvokeResult)resultOrder.AsyncState;

                if(string.IsNullOrWhiteSpace(invokeResultOrder.Value))
                    throw new Exception(string.Format("Ордер в таблицах терминала не найден: {0}", order.OrderID));

                if (invokeResultOrder.State == StateCodes.stcSuccess)
                    orderInfo = invokeResultOrder.Value.Split(new string[] { "|" }, StringSplitOptions.None);
                else
                    throw new ADException(invokeResultOrder.State, invokeResultOrder.Message);
            }
            catch (ADException ex)
            {
                orderInfo = null;
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                orderInfo = null;
                _logger.Debug(ex);
            }

            string[] tradesInfo;
            try
            {
                InvokeResult invokeResultTrades = (InvokeResult)resultTrades.AsyncState;

                if (string.IsNullOrWhiteSpace(invokeResultTrades.Value))
                    throw new Exception(string.Format("История сделок в таблицах терминала не найдена: {0}", order.OrderID));

                if (invokeResultTrades.State == StateCodes.stcSuccess)
                    tradesInfo = invokeResultTrades.Value.Split(new string[] { "|" }, StringSplitOptions.None);
                else
                    throw new ADException(invokeResultTrades.State, invokeResultTrades.Message);
            }
            catch (ADException ex)
            {
                tradesInfo = null;
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                tradesInfo = null;
                _logger.Debug(ex);
            }

            try
            {
                lock (_orderLocker)
                {
                    OrderUpdateInfo updateInfo = new OrderUpdateInfo();

                    double qty = tradesInfo != null ? Math.Abs(Convert.ToDouble(tradesInfo[4])) : order.FillQty;
                    double price = tradesInfo != null ? Convert.ToDouble(tradesInfo[2]) : order.FillPrice;

                    OrderStatus status = orderInfo != null ? GetOrderStatus(orderInfo[1]) : order.Status;

                    updateInfo.OrderID = order.OrderID;
                    updateInfo.OrderStatus = qty > 0 && status == OrderStatus.Active ? OrderStatus.PartialFilled : status;
                    updateInfo.TimeStamp = orderInfo != null ? Convert.ToDateTime(orderInfo[4]) : DateTime.Now;
                    updateInfo.FillPrice = price;
                    updateInfo.FillQty = qty;
                    updateInfo.Code = 0;
                    updateInfo.Message = orderInfo != null ? string.Format("Ордер {0}. {1}", orderInfo[0], GetMessage(orderInfo[1])).TrimEnd(' ') :
                        updateInfo.OrderStatus == OrderStatus.Unknown ? GetMessage(null) : string.Empty;

                    if (OrderUpdate != null && order.Status != OrderStatus.Canceled && order.Status != OrderStatus.Error && order.Status != OrderStatus.Filled)
                        OrderUpdate(updateInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void UpdateOrder(string ordersRaw)
        {
            string[] orders = ordersRaw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string orderRaw in orders)
            {
                string[] orderInfo = orderRaw.Split(new string[] { "|" }, StringSplitOptions.None);

                string orderID;
                try
                {
                    orderID = orderInfo[5].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                catch
                {
                    orderID = null;
                }

                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    Order order = Orders.Find(x => x.OrderID == orderID);

                    if (order != null)
                    {
                        try
                        {
                            lock (_orderLocker)
                            {
                                OrderUpdateInfo updateInfo = new OrderUpdateInfo();

                                OrderStatus status = orderInfo != null ? GetOrderStatus(orderInfo[1]) : order.Status;

                                updateInfo.OrderID = order.OrderID;
                                updateInfo.OrderStatus = orderInfo != null ? GetOrderStatus(orderInfo[1]) : order.Status;
                                updateInfo.TimeStamp = orderInfo != null ? Convert.ToDateTime(orderInfo[4]) : DateTime.Now;
                                updateInfo.FillPrice = order.FillPrice;
                                updateInfo.FillQty = order.FillQty;
                                updateInfo.Code = 0;
                                updateInfo.Message = orderInfo != null ? string.Format("Ордер {0}. {1}", orderInfo[0], GetMessage(orderInfo[1])).TrimEnd(' ') :
                                    updateInfo.OrderStatus == OrderStatus.Unknown ? GetMessage(null) : string.Empty;

                                if (order.Status == OrderStatus.Filled && order.FillQty != order.Shares)
                                    order.Status = OrderStatus.PartialFilled;

                                if (OrderUpdate != null && order.Status != OrderStatus.Canceled && order.Status != OrderStatus.Error && order.Status != OrderStatus.Filled)
                                    OrderUpdate(updateInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }
            }
        }

        private void UpdateTrades(string tradesRaw)
        {
            string[] trades = tradesRaw.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string tradeRaw in trades)
            {
                string[] tradeInfo = tradeRaw.Split(new string[] { "|" }, StringSplitOptions.None);

                string orderID;
                try
                {
                    orderID = tradeInfo[6].Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                catch
                {
                    orderID = null;
                }

                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    Order order = Orders.Find(x => x.OrderID == orderID);

                    if (order != null)
                    {
                        try
                        {
                            lock (_orderLocker)
                            {
                                OrderUpdateInfo updateInfo = new OrderUpdateInfo();

                                double qty = Math.Abs(Convert.ToDouble(tradeInfo[4]));
                                double price = Convert.ToDouble(tradeInfo[2]);

                                updateInfo.OrderID = order.OrderID;
                                updateInfo.OrderStatus = order.Status;
                                updateInfo.TimeStamp = order.TimeStamp;
                                updateInfo.FillPrice = price;
                                updateInfo.FillQty = qty;
                                updateInfo.Code = 0;
                                updateInfo.Message = string.Empty;

                                if (order.Status == OrderStatus.Filled && order.FillQty != order.Shares)
                                    order.Status = OrderStatus.PartialFilled;

                                if (OrderUpdate != null && order.Status != OrderStatus.Canceled && order.Status != OrderStatus.Error && order.Status != OrderStatus.Filled)
                                    OrderUpdate(updateInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                        }
                    }
                }
            }
        }

        private void OrderConfirmed(int ID, int OrderNum, string Message, eCommandResult Status)
        {
            Order order = Orders.Find(x => 
            {
                try
                {
                    return x.BrokerTag.ToString() == ID.ToString();
                }
                catch
                {
                    return false;
                }
            });

            if (order != null)
            {
                lock (_orderLocker)
                {
                    order.BrokerTag = -1;

                    OrderUpdateInfo updateInfo = new OrderUpdateInfo();

                    updateInfo.OrderID = order.OrderID;
                    updateInfo.OrderStatus = Status == eCommandResult.crSuccess ? OrderStatus.Active : OrderStatus.Error;
                    updateInfo.TimeStamp = order.TimeStamp;
                    updateInfo.FillPrice = order.FillPrice;
                    updateInfo.FillQty = order.FillQty;
                    updateInfo.Code = 0;
                    updateInfo.Message = Message;

                    if (OrderUpdate != null && order.Status != OrderStatus.Canceled && order.Status != OrderStatus.Error && order.Status != OrderStatus.Filled)
                        OrderUpdate(updateInfo);
                }
            }
        }

        private OrderStatus GetOrderStatus(string status)
        {
            switch (status)
            {
                case "D":
                case "E":
                    return OrderStatus.CancelPending;

                case "F":
                    return OrderStatus.Error;

                case "M":
                    return OrderStatus.Filled;

                case "Z":
                    return OrderStatus.Submitted;

                case "G":
                case "N":
                case "O":
                case "Q":
                    return OrderStatus.Active;

                case "W":
                    return OrderStatus.Canceled;

                default:
                    return OrderStatus.Unknown;
            }
        }

        private string GetMessage(string status)
        {
            switch (status)
            {
                case "D":
                case "E":
                    return "Заявка помечена к удалению.";

                case "F":
                    return string.Empty;

                case "M":
                    return "Заявка исполнена.";

                case "Z":
                    return "Ожидание.";

                case "G":
                case "N":
                case "O":
                case "Q":
                    return "Заявка активна.";

                case "W":
                    return "Заявка отменена.";

                default:
                    return "Статус заявки не известен. Возможно она была отменена, либо истек срок её действия.";
            }
        }

        public override bool AllowOrderType(OrderType orderType)
        {
            if (orderType == OrderType.AtClose)
                return false;
            else
                return true;
        }

        public override List<string> TifsAllowed()
        {
            return new List<string>() { "Default" };
        }

        #endregion

        #region Служебная информация

        public override string ProviderName
        {
            get { return "ADLiveTrading"; }
        }

        public override bool Enable
        {
            get { return _settingsProvider.GetParameter("BrokerProviderActive", true); }
        }

        #endregion
    }
}
