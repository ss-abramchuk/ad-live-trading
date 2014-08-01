using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WealthLab.DataProviders.MarketManagerService;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class ADMarketManagerInfo : MarketManagerInfo
    {
        public override string ProviderName()
        {
            return "ADStaticDataProvider";
        }
    }
}
