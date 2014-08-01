using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WealthLab;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class OrderQuery
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

        public SymbolDescription SymbolDescription
        {
            get;
            set;
        }

        public OrderType OrderType
        {
            get;
            set;
        }

        public TradeType TradeType
        {
            get;
            set;
        }

        public double Qty
        {
            get;
            set;
        }

        public double Price
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }
    }
}
