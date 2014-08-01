using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;

using WLDSolutions.LiveTradingManager.Abstract;
using WLDSolutions.LiveTradingManager.Settings;
using WLDSolutions.LiveTradingManager.Dispatcher;

using RealTimeTrading.ADLiveTrading.Abstract;

namespace RealTimeTrading.ADLiveTrading.Dispatcher
{
    internal sealed class ADDispatcher
    {
        #region Свойства диспетчера

        public ILTSettingsProvider RTTSettingsProvider
        {
            get;
            private set;
        }

        public ILTSettingsProvider SettingsProvider
        {
            get;
            private set;
        }

        public IADStaticDataProvider StaticDataProvider
        {
            get;
            private set;
        }

        public IADStreamingProvider StreamingProvider
        {
            get;
            private set;
        }

        public IADAccountProvider AccountProvider
        {
            get;
            private set;
        }

        public IADOrderProvider OrderProvider
        {
            get;
            private set;
        }

        #endregion

        #region Singleton шаблон

        private static readonly ADDispatcher _instance = new ADDispatcher();

        public static ADDispatcher Instance
        {
            get { return _instance; }
        }

        static ADDispatcher()
        {

        }

        private ADDispatcher()
        {
            string dataPath = string.Concat(Application.UserAppDataPath, "\\Data");

            RTTSettingsProvider = LTDispatcher.Instance.LTSettingsProvider;
            SettingsProvider = new LiveTradingSettingsProvider(dataPath, "\\ADLiveTrading.xml");

            ADProvider adProvider = new ADProvider(SettingsProvider);

            StaticDataProvider = adProvider;
            StreamingProvider = adProvider;
            AccountProvider = adProvider;
            OrderProvider = adProvider;            
        }

        #endregion
    }
}
