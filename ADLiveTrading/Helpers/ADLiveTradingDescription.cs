using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WLDSolutions.LiveTradingManager.Abstract;
using RealTimeTrading.ADLiveTrading.Settings;

namespace RealTimeTrading.ADLiveTrading.Helpers
{
    public sealed class ADLiveTradingDescription : LTProductDescription
    {
        public override string ProductName
        {
            get { return "ADLiveTrading"; }
        }

        public override string Version
        {
            get { return "3.0.0.0"; }
        }

        public override LTSettingsPanel SettingsPanel
        {
            get { return new ADSettingsPanel(); }
        }

        public override bool NeedActivation
        {
            get { return false; }
        }
    }
}
