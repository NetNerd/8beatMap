namespace _8beatMap
{
    partial class ChartInfoDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartInfoDialog));
            this.SongNameLbl = new System.Windows.Forms.Label();
            this.SongNameBox = new System.Windows.Forms.TextBox();
            this.AuthorBox = new System.Windows.Forms.TextBox();
            this.AuthorLbl = new System.Windows.Forms.Label();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.TimesigsBox = new System.Windows.Forms.TextBox();
            this.TimesigsLbl = new System.Windows.Forms.Label();
            this.TimesigsGrid = new System.Windows.Forms.DataGridView();
            this.TimesigStartBarColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimesigStartTickColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimesigNumeratorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimesigDenominatorColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.TimesigsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // SongNameLbl
            // 
            resources.ApplyResources(this.SongNameLbl, "SongNameLbl");
            this.SongNameLbl.Name = "SongNameLbl";
            // 
            // SongNameBox
            // 
            resources.ApplyResources(this.SongNameBox, "SongNameBox");
            this.SongNameBox.Name = "SongNameBox";
            // 
            // AuthorBox
            // 
            resources.ApplyResources(this.AuthorBox, "AuthorBox");
            this.AuthorBox.Name = "AuthorBox";
            // 
            // AuthorLbl
            // 
            resources.ApplyResources(this.AuthorLbl, "AuthorLbl");
            this.AuthorLbl.Name = "AuthorLbl";
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // TimesigsBox
            // 
            resources.ApplyResources(this.TimesigsBox, "TimesigsBox");
            this.TimesigsBox.Name = "TimesigsBox";
            this.TimesigsBox.ReadOnly = true;
            // 
            // TimesigsLbl
            // 
            resources.ApplyResources(this.TimesigsLbl, "TimesigsLbl");
            this.TimesigsLbl.Name = "TimesigsLbl";
            // 
            // TimesigsGrid
            // 
            this.TimesigsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TimesigsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TimesigStartBarColumn,
            this.TimesigStartTickColumn,
            this.TimesigNumeratorColumn,
            this.TimesigDenominatorColumn});
            this.TimesigsGrid.EnableHeadersVisualStyles = false;
            resources.ApplyResources(this.TimesigsGrid, "TimesigsGrid");
            this.TimesigsGrid.Name = "TimesigsGrid";
            this.TimesigsGrid.RowHeadersVisible = false;
            this.TimesigsGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.TimesigsGrid_DataError);
            // 
            // TimesigStartBarColumn
            // 
            resources.ApplyResources(this.TimesigStartBarColumn, "TimesigStartBarColumn");
            this.TimesigStartBarColumn.Name = "TimesigStartBarColumn";
            this.TimesigStartBarColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TimesigStartTickColumn
            // 
            resources.ApplyResources(this.TimesigStartTickColumn, "TimesigStartTickColumn");
            this.TimesigStartTickColumn.Name = "TimesigStartTickColumn";
            this.TimesigStartTickColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TimesigNumeratorColumn
            // 
            resources.ApplyResources(this.TimesigNumeratorColumn, "TimesigNumeratorColumn");
            this.TimesigNumeratorColumn.Name = "TimesigNumeratorColumn";
            this.TimesigNumeratorColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TimesigDenominatorColumn
            // 
            resources.ApplyResources(this.TimesigDenominatorColumn, "TimesigDenominatorColumn");
            this.TimesigDenominatorColumn.Name = "TimesigDenominatorColumn";
            this.TimesigDenominatorColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ChartInfoDialog
            // 
            this.AcceptButton = this.OKBtn;
            this.CancelButton = this.CancelBtn;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.TimesigsGrid);
            this.Controls.Add(this.TimesigsBox);
            this.Controls.Add(this.TimesigsLbl);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.AuthorBox);
            this.Controls.Add(this.AuthorLbl);
            this.Controls.Add(this.SongNameBox);
            this.Controls.Add(this.SongNameLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChartInfoDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.TimesigsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SongNameLbl;
        private System.Windows.Forms.TextBox SongNameBox;
        private System.Windows.Forms.TextBox AuthorBox;
        private System.Windows.Forms.Label AuthorLbl;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.TextBox TimesigsBox;
        private System.Windows.Forms.Label TimesigsLbl;
        private System.Windows.Forms.DataGridView TimesigsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimesigStartBarColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimesigStartTickColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimesigNumeratorColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimesigDenominatorColumn;
    }
}