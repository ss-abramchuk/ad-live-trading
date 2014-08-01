using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    internal class WizardMarketDescription
    {
        public string MarketCode
        {
            get;
            set;
        }

        public string MarketName
        {
            get;
            set;
        }

        [System.Reflection.ObfuscationAttribute(Feature = "renaming")]
        public string FullName
        {
            get { return string.Format("{0} ({1})", MarketName, MarketCode); }
        }
    }
}
