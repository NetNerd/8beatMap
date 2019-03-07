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
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // ChartInfoDialog
            // 
            this.AcceptButton = this.OKBtn;
            this.CancelButton = this.CancelBtn;
            resources.ApplyResources(this, "$this");
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
    }
}