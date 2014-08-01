using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WealthLab;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    public class UpdateHistoryParams
    {
        public SymbolDescription SymbolDescription
        {
            get;
            set;
        }

        public BarDataScale DataScale
        {
            get;
            set;
        }

        public IDataUpdateMessage DataUpdateMsg
        {
            get;
            set;
        }

        public IUpdateRequestCompleted UpdateRequestCompleted
        {
            get;
            set;
        }
    }
}
