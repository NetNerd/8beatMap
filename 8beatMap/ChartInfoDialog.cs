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
            this.TimesigStartBarColumn.ValueType = typeof(int);
            this.TimesigStartTickColumn.ValueType = typeof(int);
            this.TimesigNumeratorColumn.ValueType = typeof(int);
            this.TimesigDenominatorColumn.ValueType = typeof(int);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public ChartInfoDialog(Skinning.Skin skin, string songname, string author, string timesigs)
        {
            InitComponentNew();
            Skinning.SetBackCol(this, skin.UIColours[UIColours.UIColourDefs.Form_BG.TypeName]);
            Skinning.SetForeCol(this, skin.UIColours[UIColours.UIColourDefs.Form_Text.TypeName]);
            Skinning.SetUIStyle(this, skin.UIStyle);

            TimesigsGrid.DefaultCellStyle.ForeColor = SystemColors.ControlText; // reset these because they inherit incorrectly
            TimesigsGrid.DefaultCellStyle.BackColor = SystemColors.Window;

            SongNameBox.Text = songname;
            AuthorBox.Text = author;

            Notedata.TimeSigChange[] timesigsconverted = Notedata.ReadTimesigChangesFromString(timesigs);
            if (timesigsconverted != null)
            {
                foreach (Notedata.TimeSigChange sig in timesigsconverted)
                {
                    TimesigsGrid.Rows.Add(new object[] { sig.StartBar, sig.StartTick, sig.Numerator, sig.Denominator });
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

        private void TimesigsGrid_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Keys key = e.KeyCode;

                if (ModifierKeys == Keys.Control)
                {
                    if (key == Keys.C)
                    {
                        string copystr = "";

                        int firstrow = 9999;
                        int lastrow = 0;
                        int firstcol = 9999;
                        int lastcol = 0;

                        if (TimesigsGrid.SelectedRows.Count > 0) // fix for wrong behaviour in mono
                        {
                            foreach (DataGridViewRow row in TimesigsGrid.SelectedRows)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    cell.Selected = true;
                                }
                            }
                        }

                        foreach (DataGridViewCell cell in TimesigsGrid.SelectedCells)
                        {
                            if (cell.RowIndex < firstrow) firstrow = cell.RowIndex;
                            if (cell.RowIndex > lastrow) lastrow = cell.RowIndex;
                            if (cell.ColumnIndex < firstcol) firstcol = cell.ColumnIndex;
                            if (cell.ColumnIndex > lastcol) lastcol = cell.ColumnIndex;
                        }
                        
                        if (TimesigsGrid.Rows[lastrow].IsNewRow) lastrow -= 1;

                        for (int i = firstrow; i <= lastrow; i++)
                        {
                            for (int j = firstcol; j <= lastcol; j++)
                            {
                                copystr += TimesigsGrid.Rows[i].Cells[j].Value.ToString() + ",";
                            }
                            copystr = copystr.Remove(copystr.Length - 1) + "; "; // replace last comma with semicolon
                        }

                        if (copystr.Length > 1)
                        {
                            copystr = copystr.Remove(copystr.Length - 2); // remove semicolon from end
                            Clipboard.SetText(copystr);
                        }

                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    else if (key == Keys.V)
                    {
                        string pastestr = Clipboard.GetText();
                        string[] pastelines;
                        bool tsvmode;

                        if (pastestr.Contains(","))
                        {
                            pastelines = pastestr.Split(';');
                            tsvmode = false;
                        }
                        else
                        {
                            pastelines = pastestr.Split('\n');
                            tsvmode = true;
                        }

                        int currrow = TimesigsGrid.CurrentCell.RowIndex;
                        int currcol = TimesigsGrid.CurrentCell.ColumnIndex;


                        if (TimesigsGrid.SelectedRows.Count > 0 && !TimesigsGrid.CurrentCell.Selected) // fix for wrong behaviour in mono
                        {
                            currrow = 999;
                            currcol = 0;

                            foreach (DataGridViewRow row in TimesigsGrid.SelectedRows)
                            {
                                if (row.Index < currrow)
                                {
                                    currrow = row.Index;
                                    TimesigsGrid.CurrentCell = row.Cells[0];
                                }
                            }
                        }


                        if (pastelines.Length + currrow >= TimesigsGrid.Rows.Count)
                            TimesigsGrid.Rows.Add(1 + pastelines.Length + currrow - TimesigsGrid.Rows.Count);

                        for (int i = 0; i < pastelines.Length; i++)
                        {
                            string[] pastelinecells;
                            
                            if (tsvmode) pastelinecells = pastelines[i].Split('\t');
                            else pastelinecells = pastelines[i].Split(',');

                            for (int j = 0; j < pastelinecells.Length && j < TimesigsGrid.Rows[i + currrow].Cells.Count - currcol; j++)
                                TimesigsGrid.Rows[i + currrow].Cells[j + currcol].Value = pastelinecells[j].Trim();
                        }

                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                }
            }
            catch
            { }
        }
    }
}
