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
    public static class SkinnedMessageBox
    {
        private class SkinnedMessageBoxForm : Form
        {
            System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());
            private Button btnToFocus = null;

            private void InitComponentNew()
            {
                this.SuspendLayout();

                this.AutoSize = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                this.Width = 350;
                this.Height = 140;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.ShowInTaskbar = false;
                this.FormClosed += new FormClosedEventHandler(SkinnedMessageBoxForm_FormClosed);
                this.Shown += new EventHandler(SkinnedMessageBoxForm_Shown);

                this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8.8f);
                //this.Font = new System.Drawing.Font(System.Drawing.SystemFonts.MessageBoxFont.FontFamily, System.Drawing.SystemFonts.MessageBoxFont.SizeInPoints);
                this.AutoScaleMode = AutoScaleMode.None;

                this.ResumeLayout(false);
                this.PerformLayout();
            }

            public SkinnedMessageBoxForm(Skinning.Skin skin, Form owner, string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1)
            {
                InitComponentNew();

                if (owner != null)
                {
                    this.Owner = owner;
                }

                this.Text = caption;

                Padding lblMargin = new Padding(14, 0, 14, 45);
                Point lblLocation = new Point(14, 14);
                Label MessageLbl = new Label() { Text = message, AutoSize = true, Margin = lblMargin, Location = lblLocation };
                Controls.Add(MessageLbl);

                int btnWidth = 75;
                int btnHeight = 23;
                int btnPadding = 12;

                Button[] buttonrefs = { null, null, null };

                if (buttons == MessageBoxButtons.OK)
                {
                    Button OKBtn = new Button { Text = DialogResMgr.GetString("BtnOK"), DialogResult = DialogResult.OK, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    OKBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(OKBtn);
                    buttonrefs[0] = OKBtn;
                }
                else if (buttons == MessageBoxButtons.OKCancel)
                {
                    Button OKBtn = new Button { Text = DialogResMgr.GetString("BtnOK"), DialogResult = DialogResult.OK, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    OKBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(OKBtn);
                    buttonrefs[0] = OKBtn;
                    Button CancelBtn = new Button { Text = DialogResMgr.GetString("BtnCancel"), DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    CancelBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(CancelBtn);
                    this.CancelButton = CancelBtn;
                    buttonrefs[1] = CancelBtn;
                }
                else if (buttons == MessageBoxButtons.YesNo)
                {
                    Button YesBtn = new Button { Text = DialogResMgr.GetString("BtnYes"), DialogResult = DialogResult.Yes, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    YesBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(YesBtn);
                    buttonrefs[0] = YesBtn;
                    Button NoBtn = new Button { Text = DialogResMgr.GetString("BtnNo"), DialogResult = DialogResult.No, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    NoBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(NoBtn);
                    this.CancelButton = NoBtn;
                    buttonrefs[1] = NoBtn;
                }
                else if (buttons == MessageBoxButtons.RetryCancel)
                {
                    Button RetryBtn = new Button { Text = DialogResMgr.GetString("BtnRetry"), DialogResult = DialogResult.Retry, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    RetryBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(RetryBtn);
                    buttonrefs[0] = RetryBtn;
                    Button CancelBtn = new Button { Text = DialogResMgr.GetString("BtnCancel"), DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    CancelBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(CancelBtn);
                    this.CancelButton = CancelBtn;
                    buttonrefs[1] = CancelBtn;
                }
                else if (buttons == MessageBoxButtons.YesNoCancel)
                {
                    Button YesBtn = new Button { Text = DialogResMgr.GetString("BtnYes"), DialogResult = DialogResult.Yes, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    YesBtn.Location = new Point(ClientSize.Width - btnWidth * 3 - btnPadding * 3, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(YesBtn);
                    buttonrefs[0] = YesBtn;
                    Button NoBtn = new Button { Text = DialogResMgr.GetString("BtnNo"), DialogResult = DialogResult.No, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    NoBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(NoBtn);
                    buttonrefs[1] = NoBtn;
                    Button CancelBtn = new Button { Text = DialogResMgr.GetString("BtnCancel"), DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    CancelBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(CancelBtn);
                    this.CancelButton = CancelBtn;
                    buttonrefs[2] = CancelBtn;
                }
                else if (buttons == MessageBoxButtons.AbortRetryIgnore)
                {
                    Button AbortBtn = new Button { Text = DialogResMgr.GetString("BtnAbort"), DialogResult = DialogResult.Abort, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    AbortBtn.Location = new Point(ClientSize.Width - btnWidth * 3 - btnPadding * 3, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(AbortBtn);
                    buttonrefs[0] = AbortBtn;
                    Button RetryBtn = new Button { Text = DialogResMgr.GetString("BtnRetry"), DialogResult = DialogResult.Retry, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    RetryBtn.Location = new Point(ClientSize.Width - btnWidth * 2 - btnPadding * 2, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(RetryBtn);
                    buttonrefs[2] = RetryBtn;
                    Button IgnoreBtn = new Button { Text = DialogResMgr.GetString("BtnIgnore"), DialogResult = DialogResult.Ignore, Anchor = AnchorStyles.Bottom | AnchorStyles.Right, Width = btnWidth, Height = btnHeight };
                    IgnoreBtn.Location = new Point(ClientSize.Width - btnWidth - btnPadding, ClientSize.Height - btnHeight - btnPadding);
                    Controls.Add(IgnoreBtn);
                    this.CancelButton = IgnoreBtn;
                    buttonrefs[3] = IgnoreBtn;
                }


                if (icon != MessageBoxIcon.None)
                {
                    int iconsize = 32;
                    int iconpadding = 12;
                    int textpadding = -8;

                    PictureBox iconPB = new PictureBox() { Width = iconsize, Height = iconsize, Top = 10, Left = 10, AccessibleName = "Icon" };

                    // there are many duplicated values:
                    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.messageboxicon?view=netframework-4.7.2
                    if (icon == MessageBoxIcon.Error) // also Hand and Stop
                    {
                        Icon erricon = new Icon(SystemIcons.Error, iconsize, iconsize);
                        iconPB.Image = erricon.ToBitmap();
                        iconPB.AccessibleDescription = "Error";
                        //this.Icon = SystemIcons.Error;
                    }
                    else if (icon == MessageBoxIcon.Question)
                    {
                        Icon qicon = new Icon(SystemIcons.Question, iconsize, iconsize);
                        iconPB.Image = qicon.ToBitmap();
                        iconPB.AccessibleDescription = "Question";
                        //this.Icon = SystemIcons.Question;
                    }
                    else if (icon == MessageBoxIcon.Warning) // also Exclamation
                    {
                        Icon warnicon = new Icon(SystemIcons.Warning, iconsize, iconsize);
                        iconPB.Image = warnicon.ToBitmap();
                        iconPB.AccessibleDescription = "Warning";
                        //this.Icon = SystemIcons.Warning;
                        textpadding -= 2; // triangle shape looks worse without this
                    }
                    else if (icon == MessageBoxIcon.Information) // also Asterisk
                    {
                        Icon infoicon = new Icon(SystemIcons.Information, iconsize, iconsize);
                        iconPB.Image = infoicon.ToBitmap();
                        iconPB.AccessibleDescription = "Information";
                        //this.Icon = SystemIcons.Information;
                    }

                    MessageLbl.Left += (iconsize + iconpadding + textpadding);
                    MessageLbl.Width -= (iconsize + iconpadding + textpadding);
                    Controls.Add(iconPB);
                }

                if (defaultbutton == MessageBoxDefaultButton.Button1)
                {
                    this.AcceptButton = buttonrefs[0];
                    btnToFocus = buttonrefs[0]; // can't focus until after form is shown
                }
                else if (defaultbutton == MessageBoxDefaultButton.Button2)
                {
                    if (buttonrefs[1] == null) throw new ArgumentOutOfRangeException("defaultbutton", "defaultbutton cannot be set to a button number that does not exist");
                    this.AcceptButton = buttonrefs[1];
                    btnToFocus = buttonrefs[1]; // can't focus until after form is shown
                }
                else if (defaultbutton == MessageBoxDefaultButton.Button3)
                {
                    if (buttonrefs[2] == null) throw new ArgumentOutOfRangeException("defaultbutton", "defaultbutton cannot be set to a button number that does not exist");
                    this.AcceptButton = buttonrefs[2];
                    btnToFocus = buttonrefs[2]; // can't focus until after form is shown
                }


                Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
                Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
                Skinning.SetUIStyle(this, skin.UIStyle);
            }

            private void SkinnedMessageBoxForm_Shown(object sender, EventArgs e)
            {
                if (btnToFocus != null)
                    btnToFocus.Focus();
            }

            private void SkinnedMessageBoxForm_FormClosed(object sender, FormClosedEventArgs e)
            {
                if (CancelButton != null && (this.DialogResult == DialogResult.None || this.DialogResult == DialogResult.Cancel))
                    this.DialogResult = CancelButton.DialogResult;
            }
        }

        public static Skinning.Skin defaultskin = Skinning.DefaultSkin;

        // no skin, no owner
        public static DialogResult Show(string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1)
        {
            return Show(defaultskin, null, message, caption, buttons, icon, defaultbutton);
        }

        // no skin, has owner
        public static DialogResult Show(Form owner, string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1)
        {
            return Show(defaultskin, owner, message, caption, buttons, icon, defaultbutton);
        }

        // has skin, no owner
        public static DialogResult Show(Skinning.Skin skin, string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1)
        {
            return Show(skin, null, message, caption, buttons, icon, defaultbutton);
        }

        // has skin, has owner
        public static DialogResult Show(Skinning.Skin skin, Form owner, string message, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None, MessageBoxDefaultButton defaultbutton = MessageBoxDefaultButton.Button1)
        {
            SkinnedMessageBoxForm mb = new SkinnedMessageBoxForm(skin, owner, message, caption, buttons, icon, defaultbutton);
            DialogResult res = mb.ShowDialog();
            mb.Dispose();
            return res;
        }
    }
}
