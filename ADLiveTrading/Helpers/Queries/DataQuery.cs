using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class DataQuery
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

        public string Conditions
        {
            get;
            set;
        }
    }
}
