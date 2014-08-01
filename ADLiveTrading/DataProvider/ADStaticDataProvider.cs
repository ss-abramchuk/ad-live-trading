using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WealthLab;
using ADLite;
using log4net;
using WealthLab.DataProviders.MarketManagerService;

using WLDSolutions.LiveTradingManager.Abstract;
using WLDSolutions.LiveTradingManager.Helpers;
using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.ADLiveTrading.Dispatcher;
using RealTimeTrading.ADLiveTrading.Helpers;

namespace RealTimeTrading.ADLiveTrading.DataProvider
{
    public sealed class ADStaticDataProvider : StaticDataProvider
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ADStaticDataProvider));

        private IADStaticDataProvider _adStaticProvider;
        private ILTSettingsProvider _adSettingsProvider;
        private ILTSettingsProvider _rttSettingsProvider;

        private BarDataScale _newDSScale;
        private List<SymbolDescription> _newDSSymbols;

        private BarDataStore _dataStore;

        private Object _locker = new object();

        private CancellationTokenSource _cancelTokenSource = null;

        private int _iterations;
        private int _progress;

        public override void Initialize(IDataHost dataHost)
        {
            base.Initialize(dataHost);

            _adStaticProvider = ADDispatcher.Instance.StaticDataProvider;
            _adSettingsProvider = ADDispatcher.Instance.SettingsProvider;
            _rttSettingsProvider = ADDispatcher.Instance.RTTSettingsProvider;

            _dataStore = new BarDataStore(dataHost, this);
        }

        #region Создание и изменение наборов данных

        public override DataSource CreateDataSource()
        {
            DataSource dataSource = new DataSource(this);

            dataSource.BarDataScale = _newDSScale;
            dataSource.DSString = SymbolDescription.SerializeList(_newDSSymbols);

            return dataSource;
        }

        public override string SuggestedDataSourceName
        {
            get { return "Alfa-Direct Dataset"; }
        }

        public override string ModifySymbols(DataSource ds, List<string> symbols)
        {
            List<SymbolDescription> symbolsDescription = SymbolDescription.DeserializeList(ds.DSString);

            // Удаление инструментов
            //
            List<SymbolDescription> removeQuery = (from description in symbolsDescription
                                                  where !symbols.Exists(x => x == description.FullName)
                                                  select description).ToList();

            foreach (SymbolDescription description in removeQuery)
            {
                symbolsDescription.Remove(description);
            }

            // Добавление инструментов
            //
            var addQuery = from symbol in symbols
                        where !symbolsDescription.Exists(x => x.FullName == symbol)
                        select symbol;

            int errorsCount = 0;

            foreach (string symbol in addQuery)
            {
                SymbolDescription description = GetSymbolDescription(symbol);

                if (description != null)
                    symbolsDescription.Add(description);
                else
                    errorsCount++;
            }            

            if (errorsCount > 0)
                MessageBox.Show(string.Format("В формате имени одного или нескольких инструментов была допущена ошибка. Всего ошибок: {0}.", errorsCount),
                    "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return SymbolDescription.SerializeList(symbolsDescription);
        }

        public override void PopulateSymbols(DataSource ds, List<string> symbols)
        {
            if (string.IsNullOrEmpty(ds.Name)) return;

            List<SymbolDescription> symbolsDescription = SymbolDescription.DeserializeList(ds.DSString);

            var query = from description in symbolsDescription
                        select description.FullName;

            symbols.AddRange(query);
        }

        public override System.Windows.Forms.UserControl WizardFirstPage()
        {
            WizardPage wizardPage = new WizardPage(_adStaticProvider);

            return wizardPage;
        }

        public override System.Windows.Forms.UserControl WizardNextPage(System.Windows.Forms.UserControl currentPage)
        {
            WizardPage wizardPage = currentPage as WizardPage;

            if (wizardPage != null)
            {
                _newDSScale = wizardPage.GetDataScale();
                _newDSSymbols = wizardPage.GetSymbolsDescription();
            }

            return null;
        }

        public override System.Windows.Forms.UserControl WizardPreviousPage(System.Windows.Forms.UserControl currentPage)
        {
            return null;
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

        public override bool CanModifySymbols
        {
            get { return true; }
        }

        public override bool CanEditSymbolDataFile
        {
            get { return false; }
        }

        public override bool CanDeleteSymbolDataFile
        {
            get { return true; }
        }

        #endregion

        #region Получение исторических данных и обновление наборов данных

        public override MarketInfo GetMarketInfo(string symbol)
        {
            MarketInfo marketInfo = MarketManager.GetMarketInfo(symbol, "Eastern Standard Time", "ADStaticDataProvider");

            if (marketInfo.Name != null)
            {
                List<MarketTimeZone> timeZones = _rttSettingsProvider.GetObject("MarketTimeZones", typeof(List<MarketTimeZone>)) as List<MarketTimeZone> ?? new List<MarketTimeZone>();

                marketInfo.TimeZoneName = (from timeZone in timeZones
                                        where timeZone.MarketName == marketInfo.Name
                                        select timeZone.TimeZoneName).DefaultIfEmpty("Eastern Standard Time").First();
            }
            else
            {
                marketInfo.Name = "Default Eastern Market";
                marketInfo.TimeZoneName = "Eastern Standard Time";
                marketInfo.OpenTimeNative = new DateTime(1970, 1, 1, 0, 0, 0);
                marketInfo.CloseTimeNative = new DateTime(1970, 1, 1, 23, 59, 59);
            }

            return marketInfo;
        }

        public override Bars RequestData(DataSource ds, string symbol, DateTime startDate, DateTime endDate, int maxBars, bool includePartialBar)
        {
            Bars bars = new Bars(symbol, ds.Scale, ds.BarInterval);            

            List<SymbolDescription> symbolDescriptions = SymbolDescription.DeserializeList(ds.DSString);
            SymbolDescription description = symbolDescriptions.Find(x => x.FullName == symbol);

            if (description != null && OnDemandUpdate)
            {
                DateTime lastUpdate;
                DateTime currentDate = DateTime.Now;

                if (_dataStore.ContainsSymbol(description.FullName, ds.Scale, ds.BarInterval))
                {
                    _dataStore.LoadBarsObject(bars);
                    lastUpdate = _dataStore.SymbolLastUpdated(description.FullName, ds.Scale, ds.BarInterval);
                }
                else
                    lastUpdate = DateTime.Parse(_adSettingsProvider.GetParameter("BeginUpdateDate", new DateTime(2000, 1, 1).ToString()));

                List<DateInterval> intervals = GetDateIntervals(lastUpdate, currentDate, ds.BarDataScale);

                foreach (DateInterval interval in intervals)
                {
                    try
                    {
                        int corrections = 0;
                        bars.AppendWithCorrections(GetHistoryInterval(description, ds.BarDataScale, interval), out corrections);
                    }
                    catch (ADException exception)
                    {
                        MessageBox.Show(exception.ResultMessage, "Внимание! Произошла ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    catch (Exception exception)
                    {
                        logger.Error(exception);
                        MessageBox.Show(exception.Message, "Внимание! Произошла ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }                

                if (!includePartialBar && bars.Count > 0 && bars.Date[bars.Count - 1] > currentDate)
                    bars.Delete(bars.Count - 1);

                if(!base.IsStreamingRequest)
                    lock(_locker)
                        _dataStore.SaveBarsObject(bars);
            }

            _dataStore.LoadBarsObject(bars, startDate, endDate, maxBars);

            bars = MarketManager.ConvertBars(bars, startDate, endDate, maxBars, "ADStaticDataProvider");

            return bars;
        }

        public override void RequestUpdates(List<string> symbols, DateTime startDate, DateTime endDate, BarScale scale, int barInterval, IUpdateRequestCompleted requestCompleted)
        {
            List<Task> tasks = new List<Task>();

            foreach (string symbol in symbols)
            {
                SymbolDescription symbolDescription = GetSymbolDescription(symbol);

                if (symbolDescription != null)
                {
                    UpdateHistoryParams updParams = new UpdateHistoryParams()
                    {
                        SymbolDescription = symbolDescription,
                        DataScale = new BarDataScale(scale, barInterval),
                        DataUpdateMsg = null,
                        UpdateRequestCompleted = requestCompleted
                    };

                    tasks.Add(Task.Factory.StartNew(new Action<object>(GetHistory), updParams));
                }
                else
                {
                    requestCompleted.UpdateError(symbol, new Exception("В формате имени инструмента была допущена ошибка"));
                }
            }

            if (tasks.Count > 0)
                Task.WaitAll(tasks.ToArray());

            requestCompleted.ProcessingCompleted();
        }

        public override void UpdateDataSource(DataSource ds, IDataUpdateMessage dataUpdateMsg)
        {
            _cancelTokenSource = new CancellationTokenSource();

            _iterations = 0;
            _progress = 0;

            dataUpdateMsg.ReportUpdateProgress(0);

            if (string.IsNullOrWhiteSpace(ds.DSString))
            {
                List<SymbolDescription> symbolDescriptions = (from symbol in ds.Symbols
                                                              select GetSymbolDescription(symbol)).ToList();

                ds.DSString = SymbolDescription.SerializeList(symbolDescriptions);
            }

            List<SymbolDescription> updateRequired = (from description in SymbolDescription.DeserializeList(ds.DSString)
                                                     where UpdateRequired(description, ds.BarDataScale)
                                                     select description).ToList();

            dataUpdateMsg.DisplayUpdateMessage(string.Format("Количество инструментов требующих обновления: {0}", updateRequired.Count));

            if (updateRequired.Count > 0)
            {
                dataUpdateMsg.DisplayUpdateMessage("Запуск обновления инструментов:");

                Task[] tasks = new Task[updateRequired.Count];

                for (int i = 0; i < updateRequired.Count; i++)
                {
                    UpdateHistoryParams updParams = new UpdateHistoryParams()
                    {
                        SymbolDescription = updateRequired[i],
                        DataScale = ds.BarDataScale,
                        DataUpdateMsg = dataUpdateMsg,
                        UpdateRequestCompleted = null
                    };

                    tasks[i] = Task.Factory.StartNew(new Action<object>(GetHistory), updParams, _cancelTokenSource.Token);
                }

                try
                {
                    Task.WaitAll(tasks);
                }
                catch (AggregateException exception)
                {
                    exception.Handle((inner) =>
                    {
                        if (inner is OperationCanceledException)
                            return true;
                        else
                        {
                            logger.Error(inner);
                            return false;
                        }
                    });
                }
            }

            dataUpdateMsg.ReportUpdateProgress(100);
        }

        public override void UpdateProvider(IDataUpdateMessage dataUpdateMsg, List<DataSource> dataSources, bool updateNonDSSymbols, bool deleteNonDSSymbols)
        {
            foreach (BarDataScale scale in this._dataStore.GetExistingBarScales())
            {
                if (_cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested)
                {
                    dataUpdateMsg.DisplayUpdateMessage("Обновление провайдера отменено");
                    break;
                }

                dataUpdateMsg.DisplayUpdateMessage("Обновление таймфрейма " + scale.ToString());                

                var visibleSymbols = from dataSource in dataSources
                            where dataSource.BarDataScale == scale
                            select SymbolDescription.DeserializeList(dataSource.DSString);

                List<SymbolDescription> symbols = new List<SymbolDescription>();

                foreach (List<SymbolDescription> visibleSymbol in visibleSymbols)
                {
                    symbols.AddRange(visibleSymbol);
                }

                if (updateNonDSSymbols)
                {
                    var nonDSSymbols = from symbol in _dataStore.GetExistingSymbols(scale.Scale, scale.BarInterval)
                                       where !symbols.Exists(x => x.FullName == symbol)
                                       select GetSymbolDescription(symbol);

                    symbols.AddRange(nonDSSymbols);
                }

                DataSource dsVirtual = new DataSource(this);

                dsVirtual.Name = "VirtualProvider";
                dsVirtual.BarDataScale = scale;

                dsVirtual.DSString = SymbolDescription.SerializeList(symbols);

                UpdateDataSource(dsVirtual, dataUpdateMsg);

                if (deleteNonDSSymbols)
                {
                    dataUpdateMsg.DisplayUpdateMessage("--------------");
                    dataUpdateMsg.DisplayUpdateMessage("Удаление истории инструментов не входящих ни в один набор данных данного таймфрейма:");

                    var nonDSSymbols = from symbol in _dataStore.GetExistingSymbols(scale.Scale, scale.BarInterval)
                                       where !symbols.Exists(x => x.FullName == symbol)
                                       select symbol;

                    foreach (string symbol in nonDSSymbols)
                    {
                        lock(_locker)
                            _dataStore.RemoveFile(symbol, scale.Scale, scale.BarInterval);
                        dataUpdateMsg.DisplayUpdateMessage(string.Format("[DELETED] Инструмент {0} - История удалена", symbol));
                    }

                    if(nonDSSymbols.Count() == 0)
                        dataUpdateMsg.DisplayUpdateMessage(string.Format("[NA] Инструменты для удаления не найдены"));
                }

                dataUpdateMsg.DisplayUpdateMessage("--------------");
            }
        }

        private void GetHistory(object updateHistoryParams)
        {
            UpdateHistoryParams updParams = (UpdateHistoryParams)updateHistoryParams;

            Bars bars = new Bars(updParams.SymbolDescription.FullName, updParams.DataScale.Scale, updParams.DataScale.BarInterval);

            DateTime lastUpdate;
            DateTime currentDate = DateTime.Now;

            if (_dataStore.ContainsSymbol(updParams.SymbolDescription.FullName, updParams.DataScale.Scale, updParams.DataScale.BarInterval))
            {
                if (updParams.UpdateRequestCompleted == null)
                    _dataStore.LoadBarsObject(bars);

                lastUpdate = _dataStore.SymbolLastUpdated(updParams.SymbolDescription.FullName, updParams.DataScale.Scale, updParams.DataScale.BarInterval);
            }
            else
                lastUpdate = DateTime.Parse(_adSettingsProvider.GetParameter("BeginUpdateDate", new DateTime(2000, 1, 1).ToString()));

            List<DateInterval> intervals = GetDateIntervals(lastUpdate, currentDate, updParams.DataScale);

            if (updParams.DataUpdateMsg != null)
            {
                Interlocked.Add(ref _iterations, intervals.Count);
                updParams.DataUpdateMsg.DisplayUpdateMessage(string.Format("[START] Инструмент: {0} - Обновление запущено", updParams.SymbolDescription.FullName));
            }

            foreach (DateInterval interval in intervals)
            {
                if (updParams.UpdateRequestCompleted == null && _cancelTokenSource != null && _cancelTokenSource.Token.IsCancellationRequested)
                {
                    updParams.DataUpdateMsg.DisplayUpdateMessage(string.Format("[CANCELED] Инструмент {0} - Обновление отменено", updParams.SymbolDescription.FullName));

                    throw new OperationCanceledException(_cancelTokenSource.Token);
                }

                try
                {
                    int corrections = 0;
                    bars.AppendWithCorrections(GetHistoryInterval(updParams.SymbolDescription, updParams.DataScale, interval), out corrections);

                    if (bars.Count > 0 && bars.Date[bars.Count - 1] > currentDate)
                        bars.Delete(bars.Count - 1);

                    if (updParams.UpdateRequestCompleted == null)
                        lock(_locker)
                            _dataStore.SaveBarsObject(bars);                    

                    if (updParams.DataUpdateMsg != null)
                    {
                        Interlocked.Add(ref _progress, 1);
                        double result = (_progress * 100) / _iterations;

                        updParams.DataUpdateMsg.ReportUpdateProgress((int)result);
                    }
                }
                catch (ADException exception)
                {
                    if (updParams.UpdateRequestCompleted != null)
                        updParams.UpdateRequestCompleted.UpdateError(updParams.SymbolDescription.FullName, exception);

                    if (updParams.DataUpdateMsg != null)
                        updParams.DataUpdateMsg.DisplayUpdateMessage(string.Format("[ERROR] Инструмент {0} - {1}", updParams.SymbolDescription.FullName, exception.ResultMessage));

                    return;
                }
                catch (Exception exception)
                {
                    logger.Error(exception);

                    if (updParams.UpdateRequestCompleted != null)
                        updParams.UpdateRequestCompleted.UpdateError(updParams.SymbolDescription.FullName, exception);

                    if (updParams.DataUpdateMsg != null)
                        updParams.DataUpdateMsg.DisplayUpdateMessage(string.Format("[ERROR] Инструмент {0} - {1}", updParams.SymbolDescription.FullName, exception.Message));

                    return;
                }
            }

            if (updParams.UpdateRequestCompleted != null)
                updParams.UpdateRequestCompleted.UpdateCompleted(bars);

            if (updParams.DataUpdateMsg != null)
                updParams.DataUpdateMsg.DisplayUpdateMessage(string.Format("[COMPLETE] Инструмент: {0} - Обновление завершено", updParams.SymbolDescription.FullName));
        }

        private Bars GetHistoryInterval(SymbolDescription symbolDescription, BarDataScale dataScale, DateInterval interval)
        {
            Bars bars = new Bars(symbolDescription.FullName, dataScale.Scale, dataScale.BarInterval);

            int tryouts = _adSettingsProvider.GetParameter("GetHistoryTryouts", 3);

            do
            {
                IAsyncResult asyncResult = _adStaticProvider.GetStaticData(symbolDescription.MarketCode, symbolDescription.SymbolCode, dataScale, interval.Start, interval.End, null);

                asyncResult.AsyncWaitHandle.WaitOne();

                InvokeResult result = (InvokeResult)asyncResult.AsyncState;

                switch (result.State)
                {
                    case StateCodes.stcSuccess:
                        tryouts = 0;

                        if (!string.IsNullOrWhiteSpace(result.Value))
                        {
                            string[] historyRaw = result.Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string history in historyRaw)
                            {
                                string[] barRaw = history.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                                DateTime barDate = Convert.ToDateTime(barRaw[0]);
                                double open = Convert.ToDouble(barRaw[1]);
                                double high = Convert.ToDouble(barRaw[2]);
                                double low = Convert.ToDouble(barRaw[3]);
                                double close = Convert.ToDouble(barRaw[4]);
                                double vol = Math.Abs(Convert.ToDouble(barRaw[5]));

                                bars.Add(barDate, open, high, low, close, vol);
                            }
                        }

                        break;

                    case StateCodes.stcTimeout:
                        tryouts--;

                        if (tryouts == 0)
                        {
                            throw new ADException(result.State, result.Message);
                        }

                        break;

                    case StateCodes.stcClientError:
                    case StateCodes.stcCriticalClientError:
                    case StateCodes.stcNotConnected:
                    case StateCodes.stcServerError:
                    case StateCodes.stcWarning:
                            throw new ADException(result.State, result.Message);
                }
            }
            while (tryouts > 0);

            return bars;
        }

        private List<DateInterval> GetDateIntervals(DateTime start, DateTime end, BarDataScale dataScale)
        {
            List<DateInterval> intervals = new List<DateInterval>();

            switch (dataScale.Scale)
            {
                case BarScale.Minute:
                    switch (dataScale.BarInterval)
                    {
                        case (1):
                            {
                                DateTime bufferStart = start.AddDays(-7);
                                DateTime bufferEnd = start;

                                do
                                {
                                    bufferStart = bufferStart.AddDays(7);
                                    bufferEnd = bufferEnd.AddDays(7);

                                    intervals.Add(new DateInterval() { Start = bufferStart, End = bufferEnd });
                                }
                                while (end >= bufferEnd);
                            }
                            break;

                        case (5):
                        case (10):
                            {
                                DateTime bufferStart = start.AddMonths(-1);
                                DateTime bufferEnd = start;

                                do
                                {
                                    bufferStart = bufferStart.AddMonths(1);
                                    bufferEnd = bufferEnd.AddMonths(1);

                                    intervals.Add(new DateInterval() { Start = bufferStart, End = bufferEnd });
                                }
                                while (end >= bufferEnd);
                            }
                            break;

                        case (15):
                            {
                                DateTime bufferStart = start.AddMonths(-3);
                                DateTime bufferEnd = start;

                                do
                                {
                                    bufferStart = bufferStart.AddMonths(3);
                                    bufferEnd = bufferEnd.AddMonths(3);

                                    intervals.Add(new DateInterval() { Start = bufferStart, End = bufferEnd });
                                }
                                while (end >= bufferEnd);
                            }
                            break;

                        case (30):
                            {
                                DateTime bufferStart = start.AddMonths(-6);
                                DateTime bufferEnd = start;

                                do
                                {
                                    bufferStart = bufferStart.AddMonths(6);
                                    bufferEnd = bufferEnd.AddMonths(6);

                                    intervals.Add(new DateInterval() { Start = bufferStart, End = bufferEnd });
                                }
                                while (end >= bufferEnd);
                            }
                            break;

                        case (60):
                            {
                                DateTime bufferStart = start.AddMonths(-12);
                                DateTime bufferEnd = start;

                                do
                                {
                                    bufferStart = bufferStart.AddMonths(12);
                                    bufferEnd = bufferEnd.AddMonths(12);

                                    intervals.Add(new DateInterval() { Start = bufferStart, End = bufferEnd });
                                }
                                while (end >= bufferEnd);
                            }
                            break;
                    }
                    break;

                case BarScale.Daily:
                case BarScale.Weekly:
                case BarScale.Monthly:
                case BarScale.Yearly:
                    intervals.Add(new DateInterval() { Start = start, End = end });
                    break;
            }

            return intervals;
        }

        private bool UpdateRequired(SymbolDescription description, BarDataScale barDataScale)
        {
            if (!_dataStore.ContainsSymbol(description.FullName, barDataScale.Scale, barDataScale.BarInterval))
                return true;

            MarketHours mktHours = new MarketHours();
            mktHours.Market = GetMarketInfo(description.FullName);
            DateTime updateTime = _dataStore.SymbolLastUpdated(description.FullName, barDataScale.Scale, barDataScale.BarInterval);

            if (!barDataScale.IsIntraday)
            {
                if ((DateTime.Now.Date >= updateTime.Date.AddDays(1)) ||
                    (updateTime.Date < mktHours.LastTradingSessionEndedNative.Date))
                    return true;
                else
                    return false;
            }
            else
            {
                if (mktHours.IsMarketOpenNow || (updateTime < mktHours.LastTradingSessionEndedNative))
                    return true;
                else
                    return false;
            }
        }

        public override void CancelUpdate()
        {
            if(_cancelTokenSource != null)
                _cancelTokenSource.Cancel();
        }

        public override bool CanRequestUpdates
        {
            get { return true; }
        }

        public override bool SupportsDataSourceUpdate
        {
            get { return true; }
        }

        public override bool SupportsProviderUpdate
        {
            get { return true; }
        }

        public override bool SupportsDynamicUpdate(BarScale scale)
        {
            return true;
        }

        private bool OnDemandUpdate
        {
            get { return base.DataHost.OnDemandUpdateEnabled; }
        }

        public override bool InternalUseOnly
        {
            get
            {
                if (_adStaticProvider.Connected)
                    return false;
                else
                    return true;
            }
        }

        public override void CheckConnectionWithServer()
        {
            base.CheckConnectionWithServer();
        }

        #endregion

        #region Служебная информация

        public override string Description
        {
            get { return "Импорт исторических данных через терминал Alfa-Direct"; }
        }

        public override string FriendlyName
        {
            get { return "Alfa-Direct Static Data"; }
        }

        public override System.Drawing.Bitmap Glyph
        {
            get { return Properties.Resources.ADLogo16x16; }
        }

        public override string URL
        {
            get { return "http://realtimetrading.ru"; }
        }

        #endregion
    }
}
