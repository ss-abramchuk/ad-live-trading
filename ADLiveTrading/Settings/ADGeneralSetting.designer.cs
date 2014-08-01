namespace RealTimeTrading.ADLiveTrading.Settings
{
    partial class ADGeneralSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbWarning = new System.Windows.Forms.GroupBox();
            this.rtbWarningText = new System.Windows.Forms.RichTextBox();
            this.pbWarningImage = new System.Windows.Forms.PictureBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.labelUserName = new System.Windows.Forms.Label();
            this.labelPass = new System.Windows.Forms.Label();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbUserInfo = new System.Windows.Forms.GroupBox();
            this.gbBrokerAdapterSettings = new System.Windows.Forms.GroupBox();
            this.gbSlippageSettings = new System.Windows.Forms.GroupBox();
            this.numFuturesSlippage = new System.Windows.Forms.NumericUpDown();
            this.lblFuturesSlippage = new System.Windows.Forms.Label();
            this.lblStocksSlippage = new System.Windows.Forms.Label();
            this.numStocksSlippage = new System.Windows.Forms.NumericUpDown();
            this.chbSlippageEnable = new System.Windows.Forms.CheckBox();
            this.gbStopOrderTIF = new System.Windows.Forms.GroupBox();
            this.numDays = new System.Windows.Forms.NumericUpDown();
            this.rbTifMonth = new System.Windows.Forms.RadioButton();
            this.rbTifDays = new System.Windows.Forms.RadioButton();
            this.rbTifToday = new System.Windows.Forms.RadioButton();
            this.chbBrokerAdapterEnable = new System.Windows.Forms.CheckBox();
            this.gbDataAdapterSettings = new System.Windows.Forms.GroupBox();
            this.lblGetHistoryTryouts = new System.Windows.Forms.Label();
            this.numGetHistoryTryouts = new System.Windows.Forms.NumericUpDown();
            this.lblGetHistoryTimeout = new System.Windows.Forms.Label();
            this.lblBeginUpdateDate = new System.Windows.Forms.Label();
            this.numGetHistoryTimeout = new System.Windows.Forms.NumericUpDown();
            this.dtpBeginUpdateDate = new System.Windows.Forms.DateTimePicker();
            this.gbWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWarningImage)).BeginInit();
            this.gbUserInfo.SuspendLayout();
            this.gbBrokerAdapterSettings.SuspendLayout();
            this.gbSlippageSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFuturesSlippage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStocksSlippage)).BeginInit();
            this.gbStopOrderTIF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDays)).BeginInit();
            this.gbDataAdapterSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGetHistoryTryouts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetHistoryTimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // gbWarning
            // 
            this.gbWarning.Controls.Add(this.rtbWarningText);
            this.gbWarning.Controls.Add(this.pbWarningImage);
            this.gbWarning.Location = new System.Drawing.Point(6, 11);
            this.gbWarning.Name = "gbWarning";
            this.gbWarning.Size = new System.Drawing.Size(505, 50);
            this.gbWarning.TabIndex = 0;
            this.gbWarning.TabStop = false;
            // 
            // rtbWarningText
            // 
            this.rtbWarningText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbWarningText.Enabled = false;
            this.rtbWarningText.HideSelection = false;
            this.rtbWarningText.Location = new System.Drawing.Point(47, 13);
            this.rtbWarningText.Name = "rtbWarningText";
            this.rtbWarningText.ReadOnly = true;
            this.rtbWarningText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtbWarningText.Size = new System.Drawing.Size(452, 32);
            this.rtbWarningText.TabIndex = 3;
            this.rtbWarningText.Text = "Внимание! Имя пользователя и пароль хранятся в незашифрованном виде. Сохраняйте и" +
    "х только в том случае, если вы уверены, что никто не сможет получить к ним досту" +
    "п.";
            // 
            // pbWarningImage
            // 
            this.pbWarningImage.Location = new System.Drawing.Point(6, 10);
            this.pbWarningImage.Name = "pbWarningImage";
            this.pbWarningImage.Size = new System.Drawing.Size(35, 35);
            this.pbWarningImage.TabIndex = 0;
            this.pbWarningImage.TabStop = false;
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(115, 68);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(167, 20);
            this.tbUserName.TabIndex = 1;
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(3, 71);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(106, 13);
            this.labelUserName.TabIndex = 3;
            this.labelUserName.Text = "Имя пользователя:";
            // 
            // labelPass
            // 
            this.labelPass.AutoSize = true;
            this.labelPass.Location = new System.Drawing.Point(290, 71);
            this.labelPass.Name = "labelPass";
            this.labelPass.Size = new System.Drawing.Size(48, 13);
            this.labelPass.TabIndex = 4;
            this.labelPass.Text = "Пароль:";
            // 
            // tbPass
            // 
            this.tbPass.Location = new System.Drawing.Point(344, 68);
            this.tbPass.Name = "tbPass";
            this.tbPass.Size = new System.Drawing.Size(167, 20);
            this.tbPass.TabIndex = 5;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(356, 354);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(446, 354);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // gbUserInfo
            // 
            this.gbUserInfo.Controls.Add(this.gbWarning);
            this.gbUserInfo.Controls.Add(this.tbUserName);
            this.gbUserInfo.Controls.Add(this.labelUserName);
            this.gbUserInfo.Controls.Add(this.tbPass);
            this.gbUserInfo.Controls.Add(this.labelPass);
            this.gbUserInfo.Location = new System.Drawing.Point(12, 12);
            this.gbUserInfo.Name = "gbUserInfo";
            this.gbUserInfo.Size = new System.Drawing.Size(518, 101);
            this.gbUserInfo.TabIndex = 9;
            this.gbUserInfo.TabStop = false;
            this.gbUserInfo.Text = "Информация о пользователе";
            // 
            // gbBrokerAdapterSettings
            // 
            this.gbBrokerAdapterSettings.Controls.Add(this.gbSlippageSettings);
            this.gbBrokerAdapterSettings.Controls.Add(this.gbStopOrderTIF);
            this.gbBrokerAdapterSettings.Location = new System.Drawing.Point(12, 224);
            this.gbBrokerAdapterSettings.Name = "gbBrokerAdapterSettings";
            this.gbBrokerAdapterSettings.Size = new System.Drawing.Size(518, 124);
            this.gbBrokerAdapterSettings.TabIndex = 10;
            this.gbBrokerAdapterSettings.TabStop = false;
            this.gbBrokerAdapterSettings.Text = "Настройки брокер адаптера";
            // 
            // gbSlippageSettings
            // 
            this.gbSlippageSettings.Controls.Add(this.numFuturesSlippage);
            this.gbSlippageSettings.Controls.Add(this.lblFuturesSlippage);
            this.gbSlippageSettings.Controls.Add(this.lblStocksSlippage);
            this.gbSlippageSettings.Controls.Add(this.numStocksSlippage);
            this.gbSlippageSettings.Controls.Add(this.chbSlippageEnable);
            this.gbSlippageSettings.Location = new System.Drawing.Point(195, 19);
            this.gbSlippageSettings.Name = "gbSlippageSettings";
            this.gbSlippageSettings.Size = new System.Drawing.Size(316, 98);
            this.gbSlippageSettings.TabIndex = 13;
            this.gbSlippageSettings.TabStop = false;
            this.gbSlippageSettings.Text = "Настройки проскальзывания";
            // 
            // numFuturesSlippage
            // 
            this.numFuturesSlippage.Location = new System.Drawing.Point(6, 69);
            this.numFuturesSlippage.Name = "numFuturesSlippage";
            this.numFuturesSlippage.Size = new System.Drawing.Size(71, 20);
            this.numFuturesSlippage.TabIndex = 4;
            // 
            // lblFuturesSlippage
            // 
            this.lblFuturesSlippage.AutoSize = true;
            this.lblFuturesSlippage.Location = new System.Drawing.Point(83, 71);
            this.lblFuturesSlippage.Name = "lblFuturesSlippage";
            this.lblFuturesSlippage.Size = new System.Drawing.Size(214, 13);
            this.lblFuturesSlippage.TabIndex = 5;
            this.lblFuturesSlippage.Text = "Проскальзывание для фьючерсов (тики)";
            // 
            // lblStocksSlippage
            // 
            this.lblStocksSlippage.AutoSize = true;
            this.lblStocksSlippage.Location = new System.Drawing.Point(83, 45);
            this.lblStocksSlippage.Name = "lblStocksSlippage";
            this.lblStocksSlippage.Size = new System.Drawing.Size(172, 13);
            this.lblStocksSlippage.TabIndex = 4;
            this.lblStocksSlippage.Text = "Проскальзывание для акций (%)";
            // 
            // numStocksSlippage
            // 
            this.numStocksSlippage.DecimalPlaces = 2;
            this.numStocksSlippage.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.numStocksSlippage.Location = new System.Drawing.Point(6, 43);
            this.numStocksSlippage.Name = "numStocksSlippage";
            this.numStocksSlippage.Size = new System.Drawing.Size(71, 20);
            this.numStocksSlippage.TabIndex = 3;
            // 
            // chbSlippageEnable
            // 
            this.chbSlippageEnable.AutoSize = true;
            this.chbSlippageEnable.Location = new System.Drawing.Point(6, 20);
            this.chbSlippageEnable.Name = "chbSlippageEnable";
            this.chbSlippageEnable.Size = new System.Drawing.Size(256, 17);
            this.chbSlippageEnable.TabIndex = 2;
            this.chbSlippageEnable.Text = "Включить проскальзывание для стоп заявок";
            this.chbSlippageEnable.UseVisualStyleBackColor = true;
            // 
            // gbStopOrderTIF
            // 
            this.gbStopOrderTIF.Controls.Add(this.numDays);
            this.gbStopOrderTIF.Controls.Add(this.rbTifMonth);
            this.gbStopOrderTIF.Controls.Add(this.rbTifDays);
            this.gbStopOrderTIF.Controls.Add(this.rbTifToday);
            this.gbStopOrderTIF.Location = new System.Drawing.Point(6, 19);
            this.gbStopOrderTIF.Name = "gbStopOrderTIF";
            this.gbStopOrderTIF.Size = new System.Drawing.Size(183, 98);
            this.gbStopOrderTIF.TabIndex = 12;
            this.gbStopOrderTIF.TabStop = false;
            this.gbStopOrderTIF.Text = "Срок действия заявок";
            // 
            // numDays
            // 
            this.numDays.Location = new System.Drawing.Point(102, 68);
            this.numDays.Name = "numDays";
            this.numDays.Size = new System.Drawing.Size(71, 20);
            this.numDays.TabIndex = 6;
            this.numDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rbTifMonth
            // 
            this.rbTifMonth.AutoSize = true;
            this.rbTifMonth.Location = new System.Drawing.Point(9, 45);
            this.rbTifMonth.Name = "rbTifMonth";
            this.rbTifMonth.Size = new System.Drawing.Size(58, 17);
            this.rbTifMonth.TabIndex = 2;
            this.rbTifMonth.Text = "Месяц";
            this.rbTifMonth.UseVisualStyleBackColor = true;
            // 
            // rbTifDays
            // 
            this.rbTifDays.AutoSize = true;
            this.rbTifDays.Location = new System.Drawing.Point(9, 68);
            this.rbTifDays.Name = "rbTifDays";
            this.rbTifDays.Size = new System.Drawing.Size(87, 17);
            this.rbTifDays.TabIndex = 1;
            this.rbTifDays.Text = "Дней всего:";
            this.rbTifDays.UseVisualStyleBackColor = true;
            // 
            // rbTifToday
            // 
            this.rbTifToday.AutoSize = true;
            this.rbTifToday.Location = new System.Drawing.Point(9, 22);
            this.rbTifToday.Name = "rbTifToday";
            this.rbTifToday.Size = new System.Drawing.Size(158, 17);
            this.rbTifToday.TabIndex = 0;
            this.rbTifToday.Text = "Текущая торговая сессия";
            this.rbTifToday.UseVisualStyleBackColor = true;
            // 
            // chbBrokerAdapterEnable
            // 
            this.chbBrokerAdapterEnable.AutoSize = true;
            this.chbBrokerAdapterEnable.Checked = true;
            this.chbBrokerAdapterEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbBrokerAdapterEnable.Location = new System.Drawing.Point(12, 358);
            this.chbBrokerAdapterEnable.Name = "chbBrokerAdapterEnable";
            this.chbBrokerAdapterEnable.Size = new System.Drawing.Size(308, 17);
            this.chbBrokerAdapterEnable.TabIndex = 11;
            this.chbBrokerAdapterEnable.Text = "Включить брокер адаптер (требуется перезапуск WLD)";
            this.chbBrokerAdapterEnable.UseVisualStyleBackColor = true;
            // 
            // gbDataAdapterSettings
            // 
            this.gbDataAdapterSettings.Controls.Add(this.lblGetHistoryTryouts);
            this.gbDataAdapterSettings.Controls.Add(this.numGetHistoryTryouts);
            this.gbDataAdapterSettings.Controls.Add(this.lblGetHistoryTimeout);
            this.gbDataAdapterSettings.Controls.Add(this.lblBeginUpdateDate);
            this.gbDataAdapterSettings.Controls.Add(this.numGetHistoryTimeout);
            this.gbDataAdapterSettings.Controls.Add(this.dtpBeginUpdateDate);
            this.gbDataAdapterSettings.Location = new System.Drawing.Point(12, 119);
            this.gbDataAdapterSettings.Name = "gbDataAdapterSettings";
            this.gbDataAdapterSettings.Size = new System.Drawing.Size(518, 99);
            this.gbDataAdapterSettings.TabIndex = 12;
            this.gbDataAdapterSettings.TabStop = false;
            this.gbDataAdapterSettings.Text = "Настройки адаптера данных";
            // 
            // lblGetHistoryTryouts
            // 
            this.lblGetHistoryTryouts.AutoSize = true;
            this.lblGetHistoryTryouts.Location = new System.Drawing.Point(6, 70);
            this.lblGetHistoryTryouts.Name = "lblGetHistoryTryouts";
            this.lblGetHistoryTryouts.Size = new System.Drawing.Size(275, 13);
            this.lblGetHistoryTryouts.TabIndex = 11;
            this.lblGetHistoryTryouts.Text = "Количество повторных попыток при запросе данных";
            // 
            // numGetHistoryTryouts
            // 
            this.numGetHistoryTryouts.Location = new System.Drawing.Point(350, 68);
            this.numGetHistoryTryouts.Name = "numGetHistoryTryouts";
            this.numGetHistoryTryouts.Size = new System.Drawing.Size(73, 20);
            this.numGetHistoryTryouts.TabIndex = 10;
            this.numGetHistoryTryouts.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblGetHistoryTimeout
            // 
            this.lblGetHistoryTimeout.AutoSize = true;
            this.lblGetHistoryTimeout.Location = new System.Drawing.Point(6, 44);
            this.lblGetHistoryTimeout.Name = "lblGetHistoryTimeout";
            this.lblGetHistoryTimeout.Size = new System.Drawing.Size(322, 13);
            this.lblGetHistoryTimeout.TabIndex = 9;
            this.lblGetHistoryTimeout.Text = "Время ожидания ответа от сервера при запросе данных (сек)";
            // 
            // lblBeginUpdateDate
            // 
            this.lblBeginUpdateDate.AutoSize = true;
            this.lblBeginUpdateDate.Location = new System.Drawing.Point(6, 19);
            this.lblBeginUpdateDate.Name = "lblBeginUpdateDate";
            this.lblBeginUpdateDate.Size = new System.Drawing.Size(263, 13);
            this.lblBeginUpdateDate.TabIndex = 8;
            this.lblBeginUpdateDate.Text = "Запрос исторических данных начиная с этой даты";
            // 
            // numGetHistoryTimeout
            // 
            this.numGetHistoryTimeout.Location = new System.Drawing.Point(350, 42);
            this.numGetHistoryTimeout.Name = "numGetHistoryTimeout";
            this.numGetHistoryTimeout.Size = new System.Drawing.Size(73, 20);
            this.numGetHistoryTimeout.TabIndex = 7;
            this.numGetHistoryTimeout.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // dtpBeginUpdateDate
            // 
            this.dtpBeginUpdateDate.Location = new System.Drawing.Point(350, 16);
            this.dtpBeginUpdateDate.Name = "dtpBeginUpdateDate";
            this.dtpBeginUpdateDate.Size = new System.Drawing.Size(161, 20);
            this.dtpBeginUpdateDate.TabIndex = 0;
            this.dtpBeginUpdateDate.Value = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            // 
            // ADGeneralSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 388);
            this.Controls.Add(this.gbDataAdapterSettings);
            this.Controls.Add(this.chbBrokerAdapterEnable);
            this.Controls.Add(this.gbBrokerAdapterSettings);
            this.Controls.Add(this.gbUserInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ADGeneralSetting";
            this.Text = "Общие настройки";
            this.Load += new System.EventHandler(this.ADPassSetting_Load);
            this.gbWarning.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbWarningImage)).EndInit();
            this.gbUserInfo.ResumeLayout(false);
            this.gbUserInfo.PerformLayout();
            this.gbBrokerAdapterSettings.ResumeLayout(false);
            this.gbSlippageSettings.ResumeLayout(false);
            this.gbSlippageSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFuturesSlippage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStocksSlippage)).EndInit();
            this.gbStopOrderTIF.ResumeLayout(false);
            this.gbStopOrderTIF.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDays)).EndInit();
            this.gbDataAdapterSettings.ResumeLayout(false);
            this.gbDataAdapterSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numGetHistoryTryouts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetHistoryTimeout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbWarning;
        private System.Windows.Forms.PictureBox pbWarningImage;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.RichTextBox rtbWarningText;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label labelPass;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbUserInfo;
        private System.Windows.Forms.GroupBox gbBrokerAdapterSettings;
        private System.Windows.Forms.CheckBox chbBrokerAdapterEnable;
        private System.Windows.Forms.GroupBox gbStopOrderTIF;
        private System.Windows.Forms.GroupBox gbSlippageSettings;
        private System.Windows.Forms.NumericUpDown numFuturesSlippage;
        private System.Windows.Forms.Label lblFuturesSlippage;
        private System.Windows.Forms.Label lblStocksSlippage;
        private System.Windows.Forms.NumericUpDown numStocksSlippage;
        private System.Windows.Forms.CheckBox chbSlippageEnable;
        private System.Windows.Forms.GroupBox gbDataAdapterSettings;
        private System.Windows.Forms.RadioButton rbTifMonth;
        private System.Windows.Forms.RadioButton rbTifDays;
        private System.Windows.Forms.RadioButton rbTifToday;
        private System.Windows.Forms.NumericUpDown numDays;
        private System.Windows.Forms.Label lblGetHistoryTimeout;
        private System.Windows.Forms.Label lblBeginUpdateDate;
        private System.Windows.Forms.NumericUpDown numGetHistoryTimeout;
        private System.Windows.Forms.DateTimePicker dtpBeginUpdateDate;
        private System.Windows.Forms.Label lblGetHistoryTryouts;
        private System.Windows.Forms.NumericUpDown numGetHistoryTryouts;
    }
}