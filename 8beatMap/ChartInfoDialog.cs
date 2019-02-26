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
    public partial class ChartInfoDialog : Form
    {
        private void InitComponentNew()
        {
            this.SuspendLayout();
            this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8.8f);
            //this.Font = new System.Drawing.Font(System.Drawing.SystemFonts.MessageBoxFont.FontFamily, System.Drawing.SystemFonts.MessageBoxFont.SizeInPoints);
            this.AutoScaleMode = AutoScaleMode.None;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public ChartInfoDialog(Skinning.Skin skin, string songname, string author)
        {
            InitComponentNew();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            SongNameBox.Text = songname;
            AuthorBox.Text = author;
        }

        public ChartInfoDialog()
        {
            InitComponentNew();
        }

        public string[] result = {"", ""};

        private void OKBtn_Click(object sender, EventArgs e)
        {
            result = new string[2] { SongNameBox.Text, AuthorBox.Text};
            this.Close();
        }
    }
}
