using System;
using System.Collections.Generic;
using System.Windows.Forms;

using WealthLab;
using ADLite;
using log4net;

using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading
{
    public partial class WizardPage : UserControl
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(WizardPage));

        private IADStaticDataProvider _staticProvider;

        private List<string> _dataScales;
        private List<int> _dataIntervals;

        private List<WizardMarketDescription> _markets;
        private List<WizardSymbolDescription> _srcSymbols;
        private List<WizardSymbolDescription> _symbols;

        public WizardPage(IADStaticDataProvider staticProvider)
        {
            _staticProvider = staticProvider;

            InitializeComponent();

            _dataScales = new List<string>() { "Минутный", "Дневной", "Недельный", "Месячный", "Годовой" };
            _dataIntervals = new List<int>() { 1, 5, 10, 15, 30, 60 };
            _markets = new List<WizardMarketDescription>();
            _srcSymbols = new List<WizardSymbolDescription>();
            _symbols = new List<WizardSymbolDescription>();

            cbDataScale.DataSource = _dataScales;
            cbDataInterval.DataSource = _dataIntervals;

            bsMarkets.DataSource = _markets;
            cbMarket.DataSource = bsMarkets;
            cbMarket.DisplayMember = "FullName";

            bsSRCSymbols.DataSource = _srcSymbols;
            lbSRCSymbols.DataSource = bsSRCSymbols;
            lbSRCSymbols.DisplayMember = "FullSRCName";

            bsSymbols.DataSource = _symbols;
            lbSymbols.DataSource = bsSymbols;
            lbSymbols.DisplayMember = "FullName";

            _markets.AddRange(GetMarkets());
            bsMarkets.ResetBindings(false);
        }

        #region Обработка событий добавления/удаления инструментов

        private void btnAdd_Click(object sender, EventArgs e)
        {
            foreach (object symbol in lbSRCSymbols.SelectedItems)
            {
                _symbols.Add((WizardSymbolDescription)symbol);
                _srcSymbols.Remove((WizardSymbolDescription)symbol);
            }

            bsSRCSymbols.ResetBindings(false);
            bsSymbols.ResetBindings(false);
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            _symbols.AddRange(_srcSymbols);
            _srcSymbols.Clear();

            bsSRCSymbols.ResetBindings(false);
            bsSymbols.ResetBindings(false);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            WizardMarketDescription market = (WizardMarketDescription)cbMarket.SelectedItem;

            foreach (object obj in lbSymbols.SelectedItems)
            {

                WizardSymbolDescription symbolDescription = (WizardSymbolDescription)obj;

                if (symbolDescription.Market.MarketCode == market.MarketCode)
                    _srcSymbols.Add(symbolDescription);

                _symbols.Remove(symbolDescription);
            }

            _srcSymbols.Sort((x, y) => x.FullSRCName.CompareTo(y.FullSRCName));

            bsSRCSymbols.ResetBindings(false);
            bsSymbols.ResetBindings(false);
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            _symbols.Clear();
            bsSymbols.ResetBindings(false);

            ReloadSRCSymbols();
        }

        #endregion

        #region Обработка изменения фильтров

        private void cbMarket_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadSRCSymbols();
        }

        private void chbAllowTrade_CheckedChanged(object sender, EventArgs e)
        {
            _markets.Clear();
            _markets.AddRange(GetMarkets());
            bsMarkets.ResetBindings(false);
        }

        private void chbAllowShort_CheckedChanged(object sender, EventArgs e)
        {
            ReloadSRCSymbols();
        }

        private void chbAllowPawn_CheckedChanged(object sender, EventArgs e)
        {
            ReloadSRCSymbols();
        }

        private void cbDataScale_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbDataScale.SelectedIndex == 0)
                cbDataInterval.Enabled = true;
            else
                cbDataInterval.Enabled = false;
        }

        private void ReloadSRCSymbols()
        {
            _srcSymbols.Clear();
            _srcSymbols.AddRange(GetSymbols());
            _srcSymbols.Sort((x, y) => x.FullSRCName.CompareTo(y.FullSRCName));
            bsSRCSymbols.ResetBindings(false);
        }

        #endregion

        #region Запросы информации        

        private List<WizardMarketDescription> GetMarkets()
        {
            List<WizardMarketDescription> markets = new List<WizardMarketDescription>();

            IAsyncResult asyncResult = _staticProvider.GetMarkets(chbAllowTrade.Checked, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            try
            {
                InvokeResult result = (InvokeResult)asyncResult.AsyncState;

                if (result.State == StateCodes.stcSuccess)
                {
                    if (string.IsNullOrWhiteSpace(result.Value))
                        return markets;

                    string[] marketsRaw = result.Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string marketRaw in marketsRaw)
                    {
                        string[] market = marketRaw.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                        markets.Add(new WizardMarketDescription() { MarketCode = market[0], MarketName = market[1] });
                    }
                }
                else
                {
                    throw new Exception(string.Format("State: {0}; Message: {1}", result.State.ToString(), result.Message));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return markets;
        }

        private List<WizardSymbolDescription> GetSymbols()
        {
            List<WizardSymbolDescription> symbols = new List<WizardSymbolDescription>();

            WizardMarketDescription marketDescription = (WizardMarketDescription)cbMarket.SelectedItem;

            IAsyncResult asyncResult = _staticProvider.GetSymbols(marketDescription.MarketCode, chbAllowShort.Checked, chbAllowPawn.Checked, null);

            asyncResult.AsyncWaitHandle.WaitOne();

            try
            {
                InvokeResult result = (InvokeResult)asyncResult.AsyncState;

                if (result.State == StateCodes.stcSuccess)
                {
                    if (string.IsNullOrWhiteSpace(result.Value))
                        return symbols;

                    string[] symbolsRaw = result.Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach(string symbolRaw in symbolsRaw)
                    {
                        string[] symbol = symbolRaw.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                        WizardSymbolDescription symbolDescription = new WizardSymbolDescription() { Market = marketDescription, SymbolCode = symbol[0], SymbolName = symbol[1] };

                        if (!_symbols.Exists(x => x.FullName == symbolDescription.FullName))
                            symbols.Add(symbolDescription);
                    }
                }
                else
                {
                    throw new Exception(string.Format("State: {0}; Message: {1}", result.State.ToString(), result.Message));
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }

            return symbols;
        }

        #endregion

        #region Возвращение данных для создания нового набора данных

        public BarDataScale GetDataScale()
        {
            BarDataScale dataScale;

            switch (cbDataScale.SelectedIndex)
            {
                case 0:
                    dataScale = new BarDataScale(BarScale.Minute, (int)cbDataInterval.SelectedItem);
                    break;

                case 1:
                    dataScale = new BarDataScale(BarScale.Daily, 0);
                    break;

                case 2:
                    dataScale = new BarDataScale(BarScale.Weekly, 0);
                    break;

                case 3:
                    dataScale = new BarDataScale(BarScale.Monthly, 0);
                    break;

                case 4:
                    dataScale = new BarDataScale(BarScale.Yearly, 0);
                    break;

                default:
                    dataScale = new BarDataScale();
                    break;
            }

            return dataScale;
        }

        public List<SymbolDescription> GetSymbolsDescription()
        {
            List<SymbolDescription> symbols = new List<SymbolDescription>();

            foreach (WizardSymbolDescription symbol in _symbols)
            {
                symbols.Add(new SymbolDescription() { MarketCode = symbol.Market.MarketCode, SymbolCode = symbol.SymbolCode });
            }

            return symbols;
        }

        #endregion
    }
}
