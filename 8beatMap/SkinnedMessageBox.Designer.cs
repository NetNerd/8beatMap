namespace _8beatMap
{
    partial class SkinnedMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkinnedMessageBox));
            this.MessageLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MessageLbl
            // 
            resources.ApplyResources(this.MessageLbl, "MessageLbl");
            this.MessageLbl.Name = "MessageLbl";
            // 
            // SkinnedMessageBox
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.MessageLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SkinnedMessageBox";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SkinnedMessageBox_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label MessageLbl;
    }
}