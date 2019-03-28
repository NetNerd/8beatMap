using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace _8beatMap
{
    public partial class ChartInfoDialog : Form
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

        public ChartInfoDialog(Skinning.Skin skin, string songname, string author, string timesigs)
        {
            InitComponentNew();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            SongNameBox.Text = songname;
            AuthorBox.Text = author;

            Notedata.TimeSigChange[] timesigsconverted = Notedata.ReadTimesigChangesFromString(timesigs);
            if (timesigsconverted != null)
            {
                foreach (Notedata.TimeSigChange sig in timesigsconverted)
                {
                    TimesigsGrid.Rows.Add(new object[] { sig.StartBar, sig.StartTick, sig.Numerator, sig.Denominator });
                    //DataGridViewRow row = new DataGridViewRow();
                    //row.Cells.Add(new DataGridViewTextBoxCell() { ValueType = typeof(int), Value = sig.StartBar });
                    //row.Cells.Add(new DataGridViewTextBoxCell() { ValueType = typeof(int), Value = sig.StartTick });
                    //row.Cells.Add(new DataGridViewTextBoxCell() { ValueType = typeof(int), Value = sig.Numerator });
                    //row.Cells.Add(new DataGridViewTextBoxCell() { ValueType = typeof(int), Value = sig.Denominator });

                    //TimesigsGrid.Rows.Add(row);
                }
            }
        }

        public ChartInfoDialog()
        {
            InitComponentNew();
        }

        public string[] result = { "", "", "nochange" };

        private void OKBtn_Click(object sender, EventArgs e)
        {
            result = new string[3] { SongNameBox.Text, AuthorBox.Text, "nochange" };
            
            try
            {
                List<Notedata.TimeSigChange> timesigs = new List<Notedata.TimeSigChange>();
                foreach (DataGridViewRow row in TimesigsGrid.Rows) //DataGridViewRowCollection
                {
                    try
                    {
                        Notedata.TimeSigChange sig = new Notedata.TimeSigChange { StartBar = int.Parse(row.Cells[0].Value.ToString()), StartTick = int.Parse(row.Cells[1].Value.ToString()), Numerator = int.Parse(row.Cells[2].Value.ToString()), Denominator = int.Parse(row.Cells[3].Value.ToString()) };
                        timesigs.Add(sig);
                    }
                    catch
                    { }

                }
                result[2] = Notedata.MakeTimesigChangesString(timesigs.ToArray());
            }
            catch
            {
                result[2] = "nochange"; // don't change if not valid
            }

            this.Close();
        }

        private void TimesigsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = false; // suppress the error message and use last value
        }
    }
}
