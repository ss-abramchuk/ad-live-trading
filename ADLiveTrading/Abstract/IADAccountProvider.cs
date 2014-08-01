using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WealthLab;

using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.Abstract
{
    internal interface IADAccountProvider : IADConnectionProvider
    {
        IAsyncResult GetAccounts(AsyncCallback callBack);
        IAsyncResult GetPositions(ADAccount account, AsyncCallback callBack);
    }
}
