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
        public ChartInfoDialog(Skinning.Skin skin, string songname, string author)
        {
            InitializeComponent();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            SongNameBox.Text = songname;
            AuthorBox.Text = author;
        }

        public ChartInfoDialog()
        {
            InitializeComponent();
        }

        public string[] result = {"", ""};

        private void OKBtn_Click(object sender, EventArgs e)
        {
            result = new string[2] { SongNameBox.Text, AuthorBox.Text};
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
