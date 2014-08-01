using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class SubscribeQuery
    {
        public string Table
        {
            get;
            set;
        }

        public string Fields
        {
            get;
            set;
        }

        public string Filter
        {
            get;
            set;
        }

        public string FilterConditions
        {
            get;
            set;
        }

        public string SubscribeConditions
        {
            get;
            set;
        }
    }
}
