using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class WizardSymbolDescription
    {
        public WizardMarketDescription Market
        {
            get;
            set;
        }

        public string SymbolCode
        {
            get;
            set;
        }

        public string SymbolName
        {
            get;
            set;
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        public string FullSRCName
        {
            get { return string.Format("{0} ({1})", SymbolCode, SymbolName); }
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        public string FullName
        {
            get { return string.Format("{0}.{1}", Market.MarketCode, SymbolCode); }
        }
    }
}
