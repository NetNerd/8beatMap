namespace _8beatMap
{
    partial class CharaIconDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CharaIconDialog));
            this.CancelBtn = new System.Windows.Forms.Button();
            this.OKBtn = new System.Windows.Forms.Button();
            this.ImagePathBox = new System.Windows.Forms.TextBox();
            this.ImagePathLbl = new System.Windows.Forms.Label();
            this.ImagePathButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TypeLbl = new System.Windows.Forms.Label();
            this.TypeBox = new System.Windows.Forms.ComboBox();
            this.RarityBox = new System.Windows.Forms.ComboBox();
            this.RarityLbl = new System.Windows.Forms.Label();
            this.SizeLbl = new System.Windows.Forms.Label();
            this.SizeBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.SizeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // ImagePathBox
            // 
            resources.ApplyResources(this.ImagePathBox, "ImagePathBox");
            this.ImagePathBox.Name = "ImagePathBox";
            // 
            // ImagePathLbl
            // 
            resources.ApplyResources(this.ImagePathLbl, "ImagePathLbl");
            this.ImagePathLbl.Name = "ImagePathLbl";
            // 
            // ImagePathButton
            // 
            this.ImagePathButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.ImagePathButton, "ImagePathButton");
            this.ImagePathButton.Name = "ImagePathButton";
            this.ImagePathButton.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // TypeLbl
            // 
            resources.ApplyResources(this.TypeLbl, "TypeLbl");
            this.TypeLbl.Name = "TypeLbl";
            // 
            // TypeBox
            // 
            this.TypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeBox.FormattingEnabled = true;
            this.TypeBox.Items.AddRange(new object[] {
            resources.GetString("TypeBox.Items"),
            resources.GetString("TypeBox.Items1"),
            resources.GetString("TypeBox.Items2")});
            resources.ApplyResources(this.TypeBox, "TypeBox");
            this.TypeBox.Name = "TypeBox";
            // 
            // RarityBox
            // 
            this.RarityBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RarityBox.FormattingEnabled = true;
            this.RarityBox.Items.AddRange(new object[] {
            resources.GetString("RarityBox.Items"),
            resources.GetString("RarityBox.Items1"),
            resources.GetString("RarityBox.Items2"),
            resources.GetString("RarityBox.Items3")});
            resources.ApplyResources(this.RarityBox, "RarityBox");
            this.RarityBox.Name = "RarityBox";
            // 
            // RarityLbl
            // 
            resources.ApplyResources(this.RarityLbl, "RarityLbl");
            this.RarityLbl.Name = "RarityLbl";
            // 
            // SizeLbl
            // 
            resources.ApplyResources(this.SizeLbl, "SizeLbl");
            this.SizeLbl.Name = "SizeLbl";
            // 
            // SizeBox
            // 
            resources.ApplyResources(this.SizeBox, "SizeBox");
            this.SizeBox.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.SizeBox.Name = "SizeBox";
            // 
            // CharaIconDialog
            // 
            this.AcceptButton = this.OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.Controls.Add(this.SizeBox);
            this.Controls.Add(this.SizeLbl);
            this.Controls.Add(this.RarityBox);
            this.Controls.Add(this.RarityLbl);
            this.Controls.Add(this.TypeBox);
            this.Controls.Add(this.TypeLbl);
            this.Controls.Add(this.ImagePathButton);
            this.Controls.Add(this.ImagePathBox);
            this.Controls.Add(this.ImagePathLbl);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CharaIconDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.SizeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.TextBox ImagePathBox;
        private System.Windows.Forms.Label ImagePathLbl;
        private System.Windows.Forms.Button ImagePathButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label TypeLbl;
        private System.Windows.Forms.ComboBox TypeBox;
        private System.Windows.Forms.ComboBox RarityBox;
        private System.Windows.Forms.Label RarityLbl;
        private System.Windows.Forms.Label SizeLbl;
        private System.Windows.Forms.NumericUpDown SizeBox;
    }
}