using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WealthLab;
using WealthLab.DataProviders.MarketManagerService;
using log4net;

using WLDSolutions.LiveTradingManager.Abstract;
using WLDSolutions.LiveTradingManager.Helpers;
using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.ADLiveTrading.Dispatcher;
using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.DataProvider
{
    public sealed class ADStreamingDataProvider : StreamingDataProvider
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ADStaticDataProvider));

        private ILTSettingsProvider _rttSettingsProvider;
        private IADStreamingProvider _adStreamingProvider;
        private ADStaticDataProvider _adStaticProvider;

        private MarketInfo _marketInfo;

        private BarDataStore _dataStore;

        private List<string> _subscribedSymbols;

        public override void Initialize(IDataHost dataHost)
        {
            base.Initialize(dataHost);             

            _rttSettingsProvider = ADDispatcher.Instance.RTTSettingsProvider;
            _adStreamingProvider = ADDispatcher.Instance.StreamingProvider;

            _adStreamingProvider.NewQuote += NewQuote;

            _adStaticProvider = new ADStaticDataProvider();

            _dataStore = new BarDataStore(dataHost, _adStaticProvider);

            _subscribedSymbols = new List<string>();
        }

        #region Включение/выключение получения данных в реальном времени

        public override MarketInfo GetMarketInfo(string symbol)
        {
            _marketInfo = MarketManager.GetMarketInfo(symbol, "Eastern Standard Time", "ADStaticDataProvider");

            if (_marketInfo.Name != null)
            {
                List<MarketTimeZone> timeZones = _rttSettingsProvider.GetObject("MarketTimeZones", typeof(List<MarketTimeZone>)) as List<MarketTimeZone> ?? new List<MarketTimeZone>();

                _marketInfo.TimeZoneName = (from timeZone in timeZones
                                            where timeZone.MarketName == _marketInfo.Name
                                            select timeZone.TimeZoneName).DefaultIfEmpty("Eastern Standard Time").First();
            }
            else
            {
                _marketInfo.Name = "Default Eastern Market";
                _marketInfo.TimeZoneName = "Eastern Standard Time";
                _marketInfo.OpenTimeNative = new DateTime(1970, 1, 1, 0, 0, 0);
                _marketInfo.CloseTimeNative = new DateTime(1970, 1, 1, 23, 59, 59);
            }

            return _marketInfo;
        }

        protected override void Subscribe(string symbol)
        {
            // TODO реализовать получение информации о цене открытия и цене предыдущего закрытия при подписке на котировки
            if (string.IsNullOrWhiteSpace(symbol) || !IsConnected)
                return;

            if (_dataStore.GetExistingSymbols().Exists( x => x == symbol) && !_subscribedSymbols.Exists(x => x == symbol))
            {
                _subscribedSymbols.Add(symbol);
                
                List<SymbolDescription> symbolDescriptions = (from subscribedSymbol in _subscribedSymbols
                                                              let symbolDescription = GetSymbolDescription(subscribedSymbol)
                                                              where symbolDescription != null
                                                              select symbolDescription).ToList();

                Task.Factory.StartNew(new Action<object>(_adStreamingProvider.StreamDataSubscribe), symbolDescriptions);
            }
        }

        protected override void UnSubscribe(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol) || !IsConnected)
                return;

            bool unsubscribed = _subscribedSymbols.Remove(symbol);

            if (unsubscribed)
            {
                if (_subscribedSymbols.Count > 0)
                {
                    List<SymbolDescription> symbolDescriptions = (from subscribedSymbol in _subscribedSymbols
                                                                  let symbolDescription = GetSymbolDescription(subscribedSymbol)
                                                                  where symbolDescription != null
                                                                  select symbolDescription).ToList();

                    Task.Factory.StartNew(new Action<object>(_adStreamingProvider.StreamDataSubscribe), symbolDescriptions);
                }
                else
                    Task.Factory.StartNew(new Action(_adStreamingProvider.StreamDataUnsubscribe));
            }
        }

        private void NewQuote(string data)
        {
            string[] quotesRaw = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string quoteRaw in quotesRaw)
            {
                try
                {
                    string[] quoteRawData = quoteRaw.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    Quote quote = new Quote();

                    quote.Symbol = string.Format("{0}.{1}", quoteRawData[0], quoteRawData[1]);

                    //DateTime quoteDate = Convert.ToDateTime(quoteRawData[2]);
                    //TimeSpan quoteTime = TimeSpan.Parse(quoteRawData[3]);

                    //quote.TimeStamp = new DateTime(quoteDate.Year, quoteDate.Month, quoteDate.Day, quoteTime.Hours, quoteTime.Minutes, quoteTime.Seconds);

                    //quote.Price = Convert.ToDouble(quoteRawData[4]);
                    //quote.Open = Convert.ToDouble(quoteRawData[5]);
                    //quote.PreviousClose = Convert.ToDouble(quoteRawData[6]);
                    //quote.Size = Convert.ToDouble(quoteRawData[7]);

                    quote.TimeStamp = Convert.ToDateTime(quoteRawData[2]);
                    quote.Price = Convert.ToDouble(quoteRawData[3]);
                    quote.Size = Convert.ToDouble(quoteRawData[4]);

                    UpdateQuote(quote);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }

        public override bool IsConnected
        {
            get { return _adStreamingProvider.Connected; }
        }

        public override StaticDataProvider GetStaticProvider()
        {
            return _adStaticProvider;
        }

        private SymbolDescription GetSymbolDescription(string symbol)
        {
            try
            {
                SymbolDescription description = new SymbolDescription();

                description.MarketCode = symbol.Substring(0, symbol.IndexOf("."));
                description.SymbolCode = symbol.Substring(symbol.IndexOf(".") + 1, symbol.Length - symbol.IndexOf(".") - 1);

                return description;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        #endregion

        #region Служебная информация

        public override string Description
        {
            get { return "Импорт данных в реальном времени через терминал Alfa-Direct"; }
        }

        public override string FriendlyName
        {
            get { return "Alfa-Direct Streaming Data"; }
        }

        public override string URL
        {
            get { return "http://realtimetrading.ru"; }
        }

        public override System.Drawing.Bitmap Glyph
        {
            get { return Properties.Resources.ADLogo16x16; }
        }

        #endregion
    }
}
