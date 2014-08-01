using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADLite;
using WealthLab;

using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.Abstract
{
    internal interface IADOrderProvider : IADConnectionProvider
    {
        event Action<string> UpdateOrder;
        event Action<string> UpdateTrades;
        event Action<int, int, string, eCommandResult> OrderConfirmed;

        IAsyncResult SendOrder(ADAccount account, Order order, AsyncCallback callBack);
        IAsyncResult CancelOrder(ADAccount account, Order order, AsyncCallback callBack);
        IAsyncResult UpdateOrderStatus(Order order, AsyncCallback callBack);
        IAsyncResult UpdateTradesStatus(Order order, AsyncCallback callBack);

        IAsyncResult SubscribeOrderUpdates(AsyncCallback callBack);
        IAsyncResult SubscribeTradesUpdates(AsyncCallback callBack);
    }
}
