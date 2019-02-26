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
        System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

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


            int btnWidth = 75;
            int btnPadding = 12;

            if (buttons == MessageBoxButtons.OK)
            {
                Button OKBtn = new Button { Text = DialogResMgr.GetString("BtnOK"), DialogResult = DialogResult.OK, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                OKBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - OKBtn.Height - btnPadding);
                Controls.Add(OKBtn);
                this.AcceptButton = OKBtn;
            }
            else if (buttons == MessageBoxButtons.OKCancel)
            {
                Button OKBtn = new Button { Text = DialogResMgr.GetString("BtnOK"), DialogResult = DialogResult.OK, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                OKBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - OKBtn.Height - btnPadding);
                Controls.Add(OKBtn);
                this.AcceptButton = OKBtn;
                Button CancelBtn = new Button { Text = DialogResMgr.GetString("BtnCancel"), DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                CancelBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - CancelBtn.Height - btnPadding);
                Controls.Add(CancelBtn);
                this.CancelButton = CancelBtn;
            }
            else if (buttons == MessageBoxButtons.YesNo)
            {
                Button YesBtn = new Button { Text = DialogResMgr.GetString("BtnYes"), DialogResult = DialogResult.Yes, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                YesBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - YesBtn.Height - btnPadding);
                Controls.Add(YesBtn);
                this.AcceptButton = YesBtn;
                Button NoBtn = new Button { Text = DialogResMgr.GetString("BtnNo"), DialogResult = DialogResult.No, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                NoBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - NoBtn.Height - btnPadding);
                Controls.Add(NoBtn);
                this.CancelButton = NoBtn;
            }
            else if (buttons == MessageBoxButtons.RetryCancel)
            {
                Button RetryBtn = new Button { Text = DialogResMgr.GetString("BtnRetry"), DialogResult = DialogResult.Retry, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                RetryBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - RetryBtn.Height - btnPadding);
                Controls.Add(RetryBtn);
                this.AcceptButton = RetryBtn;
                Button CancelBtn = new Button { Text = DialogResMgr.GetString("BtnCancel"), DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth };
                CancelBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - CancelBtn.Height - btnPadding);
                Controls.Add(CancelBtn);
                this.CancelButton = CancelBtn;
            }
        }

        public SkinnedMessageBox()
        {
            InitComponentNew();
        }

        private void SkinnedMessageBox_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (CancelButton != null && (this.DialogResult == DialogResult.None || this.DialogResult == DialogResult.Cancel))
                this.DialogResult = CancelButton.DialogResult;
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
