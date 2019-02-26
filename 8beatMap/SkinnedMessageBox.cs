using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _8beatMap
{
    public partial class SkinnedMessageBox : Form
    {
        private void InitComponentNew()
        {
            InitializeComponent();
            this.SuspendLayout();
            this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8.8f);
            //this.Font = new System.Drawing.Font(System.Drawing.SystemFonts.MessageBoxFont.FontFamily, System.Drawing.SystemFonts.MessageBoxFont.SizeInPoints);
            this.AutoScaleMode = AutoScaleMode.None;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public SkinnedMessageBox(Skinning.Skin skin, string message, string caption="", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            InitComponentNew();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            this.Text = caption;
            MessageLbl.Text = message;

            if (buttons == MessageBoxButtons.AbortRetryIgnore || buttons == MessageBoxButtons.YesNoCancel)
            {
                throw new ArgumentException("Only one or two buttons are supported");
            }

            if (buttons == MessageBoxButtons.OKCancel || buttons == MessageBoxButtons.YesNo || buttons == MessageBoxButtons.RetryCancel)
            {
                OKbtn.Left = OKbtn.Left - (this.ClientSize.Width - OKbtn.Left);
                CancelBtn.Top = OKbtn.Top;
                CancelBtn.Enabled = true;
            }
            if (buttons == MessageBoxButtons.YesNo)
            {
                OKbtn.DialogResult = DialogResult.Yes;
                CancelBtn.DialogResult = DialogResult.No;
            }
            else if (buttons == MessageBoxButtons.RetryCancel)
            {
                OKbtn.DialogResult = DialogResult.Retry;
            }
        }

        public SkinnedMessageBox()
        {
            InitComponentNew();
        }

        private void SkinnedMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult == DialogResult.None || this.DialogResult == DialogResult.Cancel)
                this.DialogResult = CancelBtn.DialogResult;
        }
    }

    public static class SkinnedMessageBoxMaker
    {
        public static DialogResult ShowMessageBox(Skinning.Skin skin, string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            SkinnedMessageBox mb = new SkinnedMessageBox(skin, message, caption, buttons);
            DialogResult res = mb.ShowDialog();
            mb.Dispose();
            return res;
        }
    }
}
