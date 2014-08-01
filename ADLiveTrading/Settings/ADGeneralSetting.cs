using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WealthLab;

using WLDSolutions.LiveTradingManager.Abstract;

namespace RealTimeTrading.ADLiveTrading.Settings
{
    public partial class ADGeneralSetting : Form
    {
        private ILTSettingsProvider _settingsProvider;

        public ADGeneralSetting(ILTSettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;

            InitializeComponent();

            pbWarningImage.Image = SystemIcons.Warning.ToBitmap();
        }

        private void ADPassSetting_Load(object sender, EventArgs e)
        {
            tbUserName.Text = _settingsProvider.GetParameter("Username", string.Empty);
            tbPass.Text = _settingsProvider.GetParameter("Password", string.Empty);

            chbBrokerAdapterEnable.Checked = _settingsProvider.GetParameter("BrokerProviderActive", false);

            dtpBeginUpdateDate.Value = Convert.ToDateTime(_settingsProvider.GetParameter("BeginUpdateDate", new DateTime(2000, 1, 1).ToString()));
            numGetHistoryTimeout.Value = _settingsProvider.GetParameter("GetHistoryTimeout", 10);
            numGetHistoryTryouts.Value = _settingsProvider.GetParameter("GetHistoryTryouts", 3);
            
            string tif = _settingsProvider.GetParameter("TIF", "Month");

            switch (tif)
            {
                case "Today":
                    rbTifToday.Checked = true;
                    break;

                case "Days":
                    rbTifDays.Checked = true;
                    break;

                default:
                    rbTifMonth.Checked = true;
                    break;
            }

            numDays.Value = _settingsProvider.GetParameter("TIFDays", 1);

            chbSlippageEnable.Checked = _settingsProvider.GetParameter("EnableSlippage", false);
            numStocksSlippage.Value = (decimal)_settingsProvider.GetParameter("SlippageUnits", 0.0);
            numFuturesSlippage.Value = _settingsProvider.GetParameter("SlippageTicks", 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _settingsProvider.SetParameter("Username", tbUserName.Text);
            _settingsProvider.SetParameter("Password", tbPass.Text);

            _settingsProvider.SetParameter("BrokerProviderActive", chbBrokerAdapterEnable.Checked);

            _settingsProvider.SetParameter("BeginUpdateDate", dtpBeginUpdateDate.Value.ToString());
            _settingsProvider.SetParameter("GetHistoryTimeout", Convert.ToInt32(numGetHistoryTimeout.Value));
            _settingsProvider.SetParameter("GetHistoryTryouts", Convert.ToInt32(numGetHistoryTryouts.Value));

            if(rbTifToday.Checked)
                _settingsProvider.SetParameter("TIF", "Today");
            else if (rbTifMonth.Checked)
                _settingsProvider.SetParameter("TIF", "Month");
            else
                _settingsProvider.SetParameter("TIF", "Days");

            _settingsProvider.SetParameter("TIFDays", Convert.ToInt32(numDays.Value));

            _settingsProvider.SetParameter("EnableSlippage", chbSlippageEnable.Checked);
            _settingsProvider.SetParameter("SlippageUnits", Convert.ToDouble(numStocksSlippage.Value));
            _settingsProvider.SetParameter("SlippageTicks", Convert.ToInt32(numFuturesSlippage.Value));

            this.Close();
        }
    }
}
