using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.IO;

using RealTimeTrading.ADLiveTrading.Abstract;
using RealTimeTrading.ADLiveTrading.Dispatcher;
using RealTimeTrading.ADLiveTrading.Settings;
using WLDSolutions.LiveTradingManager.Abstract;
using WLDSolutions.LiveTradingManager.Helpers;

namespace RealTimeTrading.ADLiveTrading.Settings
{
    public partial class ADSettingsPanel : LTSettingsPanel
    {
        private string _activationKey;
        private string _fullActivationKey;

        public ADSettingsPanel() 
            : base()
        {
            InitializeComponent();

            cbSettings.SelectedIndex = 0;
        }

        private Form GetParentForm()
        {
            Form result = null;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == "MainForm")
                    result = form;
            }

            return result;
        }
        
        private void btnSettings_Click(object sender, EventArgs e)
        {
            switch (cbSettings.SelectedIndex)
            {
                case (0):
                    ADGeneralSetting adGeneralSettings = new ADGeneralSetting(ADDispatcher.Instance.SettingsProvider);

                    adGeneralSettings.MdiParent = GetParentForm();

                    adGeneralSettings.Show();
                    adGeneralSettings.Activate();

                    break;
            }
        }
    }
}
