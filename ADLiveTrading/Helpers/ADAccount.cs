using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WLDSolutions.LiveTradingManager.Abstract;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class ADAccount : LTAccount
    {
        public string Treaty
        {
            get;
            set;
        }

        public string Account
        {
            get;
            set;
        }

        public string Section
        {
            get;
            set;
        }
    }
}
