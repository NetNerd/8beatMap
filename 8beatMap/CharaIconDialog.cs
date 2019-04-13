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
    public partial class CharaIconDialog : Form
    {
        private void InitComponentNew()
        {
            InitializeComponent();
            this.SuspendLayout();
            try
            {
                this.Icon = Application.OpenForms[0].Icon;
            }
            catch { }
            this.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 8.8f);
            //this.Font = new System.Drawing.Font(System.Drawing.SystemFonts.MessageBoxFont.FontFamily, System.Drawing.SystemFonts.MessageBoxFont.SizeInPoints);
            this.AutoScaleMode = AutoScaleMode.None;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public CharaIconDialog(Skinning.Skin skin, CharaIcons.CharaIconInfo icondef)
        {
            InitComponentNew();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            result = icondef;

            ImagePathBox.Text = icondef.ImagePath;
            TypeBox.SelectedIndex = icondef.Type;
            RarityBox.SelectedIndex = icondef.Rarity;
            SizeBox.Value = icondef.IconSize;
        }

        public CharaIconDialog()
        {
            InitComponentNew();
        }

        public CharaIcons.CharaIconInfo result;

        private void OKBtn_Click(object sender, EventArgs e)
        {
            result.ImagePath = ImagePathBox.Text;
            result.Type = TypeBox.SelectedIndex;
            result.Rarity = RarityBox.SelectedIndex;
            result.IconSize = (int)SizeBox.Value;

            if (result.IconSize <= 0)
                result.IconSize = 127;

            this.Close();
        }

        private void ImagePathButton_Click(object sender, EventArgs e)
        {
            if (ImagePathBox.Text != null && ImagePathBox.Text.Length > 0)
            {
                openFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(ImagePathBox.Text);
                openFileDialog1.FileName = System.IO.Path.GetFileName(ImagePathBox.Text);
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                ImagePathBox.Text = openFileDialog1.FileName;
        }
    }
}
