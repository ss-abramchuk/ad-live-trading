using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal enum InvokeType
    {
        GetData,
        GetHistory,
        CreateOrder,
        CancelOrder,
        Subscribe,
        Unsubscribe
    }
}
