using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class HistoryQuery
    {
        public string Market
        {
            get;
            set;
        }

        public string Symbol
        {
            get;
            set;
        }

        public int Period
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }
    }
}
