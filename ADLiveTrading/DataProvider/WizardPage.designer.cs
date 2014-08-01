namespace RealTimeTrading.ADLiveTrading
{
    partial class WizardPage
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelMarket = new System.Windows.Forms.Label();
            this.labelPeriod = new System.Windows.Forms.Label();
            this.cbMarket = new System.Windows.Forms.ComboBox();
            this.cbDataScale = new System.Windows.Forms.ComboBox();
            this.chbAllowTrade = new System.Windows.Forms.CheckBox();
            this.chbAllowShort = new System.Windows.Forms.CheckBox();
            this.chbAllowPawn = new System.Windows.Forms.CheckBox();
            this.lbSRCSymbols = new System.Windows.Forms.ListBox();
            this.lbSymbols = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnAddAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.groupBoxFilters = new System.Windows.Forms.GroupBox();
            this.cbDataInterval = new System.Windows.Forms.ComboBox();
            this.groupBoxStocks = new System.Windows.Forms.GroupBox();
            this.bsMarkets = new System.Windows.Forms.BindingSource(this.components);
            this.bsSRCSymbols = new System.Windows.Forms.BindingSource(this.components);
            this.bsSymbols = new System.Windows.Forms.BindingSource(this.components);
            this.groupBoxFilters.SuspendLayout();
            this.groupBoxStocks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsMarkets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsSRCSymbols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsSymbols)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMarket
            // 
            this.labelMarket.Location = new System.Drawing.Point(8, 22);
            this.labelMarket.Name = "labelMarket";
            this.labelMarket.Size = new System.Drawing.Size(43, 13);
            this.labelMarket.TabIndex = 0;
            this.labelMarket.Text = "Рынок:";
            // 
            // labelPeriod
            // 
            this.labelPeriod.Location = new System.Drawing.Point(309, 22);
            this.labelPeriod.Name = "labelPeriod";
            this.labelPeriod.Size = new System.Drawing.Size(48, 13);
            this.labelPeriod.TabIndex = 1;
            this.labelPeriod.Text = "Период:";
            // 
            // cbMarket
            // 
            this.cbMarket.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMarket.Location = new System.Drawing.Point(57, 19);
            this.cbMarket.Name = "cbMarket";
            this.cbMarket.Size = new System.Drawing.Size(246, 21);
            this.cbMarket.TabIndex = 0;
            this.cbMarket.SelectedIndexChanged += new System.EventHandler(this.cbMarket_SelectedIndexChanged);
            // 
            // cbDataScale
            // 
            this.cbDataScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataScale.Location = new System.Drawing.Point(363, 19);
            this.cbDataScale.Name = "cbDataScale";
            this.cbDataScale.Size = new System.Drawing.Size(105, 21);
            this.cbDataScale.TabIndex = 1;
            this.cbDataScale.SelectedValueChanged += new System.EventHandler(this.cbDataScale_SelectedValueChanged);
            // 
            // chbAllowTrade
            // 
            this.chbAllowTrade.Checked = true;
            this.chbAllowTrade.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAllowTrade.Location = new System.Drawing.Point(11, 48);
            this.chbAllowTrade.Name = "chbAllowTrade";
            this.chbAllowTrade.Size = new System.Drawing.Size(111, 20);
            this.chbAllowTrade.TabIndex = 2;
            this.chbAllowTrade.Text = "Доступ к торгам";
            this.chbAllowTrade.CheckedChanged += new System.EventHandler(this.chbAllowTrade_CheckedChanged);
            // 
            // chbAllowShort
            // 
            this.chbAllowShort.Checked = true;
            this.chbAllowShort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAllowShort.Location = new System.Drawing.Point(128, 48);
            this.chbAllowShort.Name = "chbAllowShort";
            this.chbAllowShort.Size = new System.Drawing.Size(119, 20);
            this.chbAllowShort.TabIndex = 3;
            this.chbAllowShort.Text = "Короткие позиции";
            this.chbAllowShort.CheckedChanged += new System.EventHandler(this.chbAllowShort_CheckedChanged);
            // 
            // chbAllowPawn
            // 
            this.chbAllowPawn.Checked = true;
            this.chbAllowPawn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbAllowPawn.Location = new System.Drawing.Point(248, 48);
            this.chbAllowPawn.Name = "chbAllowPawn";
            this.chbAllowPawn.Size = new System.Drawing.Size(134, 20);
            this.chbAllowPawn.TabIndex = 4;
            this.chbAllowPawn.Text = "Возможность залога";
            this.chbAllowPawn.CheckedChanged += new System.EventHandler(this.chbAllowPawn_CheckedChanged);
            // 
            // lbSymbols
            // 
            this.lbSRCSymbols.HorizontalScrollbar = true;
            this.lbSRCSymbols.Location = new System.Drawing.Point(10, 16);
            this.lbSRCSymbols.Name = "lbSymbols";
            this.lbSRCSymbols.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSRCSymbols.Size = new System.Drawing.Size(230, 251);
            this.lbSRCSymbols.TabIndex = 5;
            // 
            // lbAddedSymbols
            // 
            this.lbSymbols.HorizontalScrollbar = true;
            this.lbSymbols.Location = new System.Drawing.Point(303, 16);
            this.lbSymbols.Name = "lbAddedSymbols";
            this.lbSymbols.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSymbols.Size = new System.Drawing.Size(230, 251);
            this.lbSymbols.TabIndex = 10;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(247, 48);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(49, 37);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = ">";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnAddAll
            // 
            this.btnAddAll.Location = new System.Drawing.Point(247, 91);
            this.btnAddAll.Name = "btnAddAll";
            this.btnAddAll.Size = new System.Drawing.Size(49, 37);
            this.btnAddAll.TabIndex = 7;
            this.btnAddAll.Text = ">>";
            this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(247, 158);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(49, 37);
            this.btnRemove.TabIndex = 8;
            this.btnRemove.Text = "<";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(247, 201);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(49, 37);
            this.btnRemoveAll.TabIndex = 9;
            this.btnRemoveAll.Text = "<<";
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // groupBoxFilters
            // 
            this.groupBoxFilters.Controls.Add(this.cbDataInterval);
            this.groupBoxFilters.Controls.Add(this.chbAllowTrade);
            this.groupBoxFilters.Controls.Add(this.labelMarket);
            this.groupBoxFilters.Controls.Add(this.labelPeriod);
            this.groupBoxFilters.Controls.Add(this.chbAllowShort);
            this.groupBoxFilters.Controls.Add(this.cbMarket);
            this.groupBoxFilters.Controls.Add(this.chbAllowPawn);
            this.groupBoxFilters.Controls.Add(this.cbDataScale);
            this.groupBoxFilters.Location = new System.Drawing.Point(3, 3);
            this.groupBoxFilters.Name = "groupBoxFilters";
            this.groupBoxFilters.Size = new System.Drawing.Size(544, 75);
            this.groupBoxFilters.TabIndex = 11;
            this.groupBoxFilters.TabStop = false;
            this.groupBoxFilters.Text = "Фильтры";
            // 
            // cbDataInterval
            // 
            this.cbDataInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataInterval.Location = new System.Drawing.Point(474, 19);
            this.cbDataInterval.Name = "cbDataInterval";
            this.cbDataInterval.Size = new System.Drawing.Size(58, 21);
            this.cbDataInterval.TabIndex = 5;
            // 
            // groupBoxStocks
            // 
            this.groupBoxStocks.Controls.Add(this.lbSRCSymbols);
            this.groupBoxStocks.Controls.Add(this.btnAdd);
            this.groupBoxStocks.Controls.Add(this.lbSymbols);
            this.groupBoxStocks.Controls.Add(this.btnRemoveAll);
            this.groupBoxStocks.Controls.Add(this.btnRemove);
            this.groupBoxStocks.Controls.Add(this.btnAddAll);
            this.groupBoxStocks.Location = new System.Drawing.Point(4, 84);
            this.groupBoxStocks.Name = "groupBoxStocks";
            this.groupBoxStocks.Size = new System.Drawing.Size(543, 273);
            this.groupBoxStocks.TabIndex = 12;
            this.groupBoxStocks.TabStop = false;
            this.groupBoxStocks.Text = "Инструменты";
            // 
            // WizardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxFilters);
            this.Controls.Add(this.groupBoxStocks);
            this.Name = "WizardPage";
            this.Size = new System.Drawing.Size(550, 360);
            this.groupBoxFilters.ResumeLayout(false);
            this.groupBoxStocks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsMarkets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsSRCSymbols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsSymbols)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMarket;
        private System.Windows.Forms.Label labelPeriod;
        private System.Windows.Forms.ComboBox cbMarket;
        private System.Windows.Forms.ComboBox cbDataScale;
        private System.Windows.Forms.CheckBox chbAllowTrade;
        private System.Windows.Forms.CheckBox chbAllowShort;
        private System.Windows.Forms.CheckBox chbAllowPawn;
        private System.Windows.Forms.ListBox lbSRCSymbols;
        private System.Windows.Forms.ListBox lbSymbols;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnAddAll;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.GroupBox groupBoxFilters;
        private System.Windows.Forms.GroupBox groupBoxStocks;
        private System.Windows.Forms.BindingSource bsMarkets;
        private System.Windows.Forms.BindingSource bsSRCSymbols;
        private System.Windows.Forms.BindingSource bsSymbols;
        private System.Windows.Forms.ComboBox cbDataInterval;
    }
}
