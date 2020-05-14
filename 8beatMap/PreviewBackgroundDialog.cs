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
    public partial class PreviewBackgroundDialog : Form
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

        public PreviewBackgroundDialog(Skinning.Skin skin, string bgpath)
        {
            InitComponentNew();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            result = bgpath;

            ImagePathBox.Text = bgpath;
        }

        public PreviewBackgroundDialog()
        {
            InitComponentNew();
        }

        public string result;

        private void OKBtn_Click(object sender, EventArgs e)
        {
            result = ImagePathBox.Text;
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
