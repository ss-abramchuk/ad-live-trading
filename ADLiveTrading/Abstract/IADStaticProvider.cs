using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WealthLab;

namespace RealTimeTrading.ADLiveTrading.Abstract
{
    public interface IADStaticDataProvider : IADConnectionProvider
    {
        IAsyncResult GetStaticData(string market, string symbol, BarDataScale dataScale, DateTime startDate, DateTime endDate, AsyncCallback callBack);

        IAsyncResult GetMarkets(bool allowTrade, AsyncCallback callBack);

        IAsyncResult GetSymbols(string market, bool allowShort, bool allowPawn, AsyncCallback callBack);
    }
}
