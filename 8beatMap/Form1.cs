using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;


namespace _8beatMap
{
    public partial class Form1 : Form
    {
        System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

        Notedata.Chart chart = new Notedata.Chart(32 * 48, 120);
        private int TickHeight = 10;
        private int IconWidth = 20;
        private int IconHeight = 10;
        private double CurrentTick = 0;
        private int LastTick = 0;

        private Timer playTimer = new Timer() { Interval = 4 };
        
        WaveOutEvent WaveOut = new WaveOutEvent { DesiredLatency = 100, NumberOfBuffers = 16 };
        MediaFoundationReader MusicFileReader;

        WaveOutEvent NoteSoundWaveOut = new WaveOutEvent { DesiredLatency = 110, NumberOfBuffers = 4 };
        static NAudio.Wave.SampleProviders.SignalGenerator NoteSoundSig = new NAudio.Wave.SampleProviders.SignalGenerator { Frequency = 1000, Gain = 0.5, Type = NAudio.Wave.SampleProviders.SignalGeneratorType.Square };
        NAudio.Wave.SampleProviders.OffsetSampleProvider NoteSoundTrim;
        NAudio.Wave.SampleProviders.MixingSampleProvider NoteSoundMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2)) { ReadFully = true };
        

        private struct NoteDataInfo
        {
            public int Tick;
            public int Lane;
            public Notedata.NoteType Type;

            public NoteDataInfo(int Tick, int Lane, Notedata.NoteType Type)
            {
                this.Tick = Tick;
                this.Lane = Lane;
                this.Type = Type;
            }
        }

        private Notedata.NoteType FindVisualNoteType(int tick, int lane)
        {
            if (tick >= chart.Length) return Notedata.NoteType.None;

            if (chart.Ticks[tick].Notes[lane] == Notedata.NoteType.Hold || chart.Ticks[tick].Notes[lane] == Notedata.NoteType.SimulHoldRelease)
            {
                if (tick == 0 || tick == chart.Length - 1) return chart.Ticks[tick].Notes[lane];
                if ((chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.Hold ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SimulHoldStart ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SimulHoldRelease ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SwipeLeftStartEnd ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SwipeRightStartEnd) &&
                    chart.Ticks[tick + 1].Notes[lane] != Notedata.NoteType.None)
                    return Notedata.NoteType.ExtendHoldMid;
            }
            return chart.Ticks[tick].Notes[lane];
        }

        
        Image GetChartImage(int width, int height, double startTick, int tickHeight, int iconWidth, int iconHeight)
        {
            Image Bmp = pictureBox1.Image;
            Graphics Grfx = Graphics.FromImage(Bmp);

            Grfx.FillRectangle(new SolidBrush(SystemColors.ControlLight), 0, 0, width, height);

            for (int i = 1; i < 8; i++)
            {
                Grfx.FillRectangle(new SolidBrush(Color.LightGray), i*width/8, 0, 1, height);
            }



            float laneWidth = width / 8;
            float halfIconWidth = iconWidth / 2;
            int halfIconHeight = iconHeight / 2;

            for (int i = (int)startTick;i < startTick+height/tickHeight; i++)
            {
               if (i >= chart.Length) break;

               for (int j = 0; j < 8; j++)
                {
                    Color noteCol = Color.LightGray;
                    Color ArrowCol = Color.Transparent;
                    int ArrowDir = 0;

                    Notedata.NoteType Type = FindVisualNoteType(i, j);

                    switch (Type)
                    {
                        case Notedata.NoteType.Tap: noteCol = Color.Blue; break;
                        case Notedata.NoteType.Hold: noteCol = Color.LimeGreen; break;
                        case Notedata.NoteType.SimulTap:
                        case Notedata.NoteType.SimulHoldStart:
                        case Notedata.NoteType.SimulHoldRelease: noteCol = Color.DeepPink; break;
                        case Notedata.NoteType.FlickLeft:
                        case Notedata.NoteType.HoldEndFlickLeft: ArrowCol = Color.FromArgb(0x70, 0, 0x78); ArrowDir = -1; break;
                        case Notedata.NoteType.SwipeLeftStartEnd: ArrowCol = Color.DarkViolet; ArrowDir = -1; break;
                        case Notedata.NoteType.SwipeLeftMid:
                        case Notedata.NoteType.SwipeChangeDirR2L: ArrowCol = Color.Violet; ArrowDir = -1; break;
                        case Notedata.NoteType.FlickRight:
                        case Notedata.NoteType.HoldEndFlickRight: ArrowCol = Color.FromArgb(0xcc, 0x88, 0); ArrowDir = 1; break;
                        case Notedata.NoteType.SwipeRightStartEnd: ArrowCol = Color.DarkOrange; ArrowDir = 1; break;
                        case Notedata.NoteType.SwipeRightMid:
                        case Notedata.NoteType.SwipeChangeDirL2R: ArrowCol = Color.Gold; ArrowDir = 1; break;
                        case Notedata.NoteType.ExtendHoldMid: noteCol = Color.LightGray; break;
                    }


                    if (chart.Ticks[i].Notes[j] != Notedata.NoteType.None)
                    {
                        int iconX = (int)((j + 0.5) * laneWidth - halfIconWidth);
                        int iconY = (int)(height - (float)(i - startTick + 1.5) * tickHeight);

                        Grfx.FillRectangle(new SolidBrush(noteCol), iconX, iconY, iconWidth, iconHeight);
                        if (ArrowDir == -1)
                            Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(iconX + IconWidth - 1, iconY + 0), new Point(iconX + IconWidth - 1, iconY + IconHeight - 1), new Point(iconX + 0, iconY + halfIconHeight) });
                        else if (ArrowDir == 1)
                            Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(iconX + 0, iconY + 0), new Point(iconX + 0, iconY + IconHeight - 1), new Point(iconX + IconWidth - 1, iconY + halfIconHeight) });
                    }
                }

                if (i % 48 == 0)
                {
                    Grfx.FillRectangle(new SolidBrush(Color.SlateGray), 0, height - (float)(i - startTick + 0.5) * tickHeight - 1, width, 3);
                    Grfx.DrawString((i / 48 + 1).ToString(), new System.Drawing.Font("Arial", 6.5f), new SolidBrush(Color.DarkSlateGray), 0, height - (float)(i - startTick + 0.5) * tickHeight - 11);
                }
                else if (i % 12 == 0)
                {
                    Grfx.FillRectangle(new SolidBrush(Color.LightSlateGray), 0, height - (float)(i - startTick + 0.5) * tickHeight, width, 1);
                }
                else if (i % 6 == 0)
                {
                    Grfx.FillRectangle(new SolidBrush(Color.LightGray), 0, height - (float)(i - startTick + 0.5) * tickHeight, width, 1);
                }
            }

            Grfx.Dispose();
            return Bmp;
        }

        private void SetCurrTick(double tick)
        {
            if (tick < 0) tick = 0;
            if (tick >= chart.Length) tick = chart.Length - 1;

            CurrentTick = tick;

            ChartScrollBar.Value = (int)(chart.Length * TickHeight - tick * TickHeight);
        }

        private void UpdateChart()
        {
            pictureBox1.Image = GetChartImage(pictureBox1.Width, pictureBox1.Height, CurrentTick, TickHeight, IconWidth, IconHeight);
        }

        private int ConvertXCoordToNote(int X)
        {
            return ((X - pictureBox1.Location.X) / (pictureBox1.Width/8));
        }

        private double ConvertYCoordToTick(int Y)
        {
            return (pictureBox1.Location.Y + pictureBox1.Height - Y - 1) / TickHeight + CurrentTick;
        }


        private TimeSpan ConvertTicksToTime(double ticks)
        {
            TimeSpan a = TimeSpan.FromSeconds((5 * ticks / chart.BPM));
            return TimeSpan.FromSeconds((5 * ticks / chart.BPM));
        }

        private double ConvertTimeToTicks(TimeSpan time)
        {
            return time.TotalSeconds / (double)(5/BPMbox.Value);
        }


        private void ResizeScrollbar()
        {
            ChartScrollBar.Minimum = 0;
            ChartScrollBar.Maximum = (int)(chart.Length * TickHeight + IconHeight / 2 + 110);
        }


        private void ResizeChart(int NewLen)
        {
            chart.Length = NewLen;
            ResizeScrollbar();
            SetCurrTick(CurrentTick);
            UpdateChart();
        }


        private void StartPlayback()
        {
            playTimer.Enabled = true;
            WaveOut.Play();
        }

        private void StopPlayback()
        {
            playTimer.Enabled = false;
            WaveOut.Pause();
        }


        private void LoadChart(string Path)
        {
            if (Path.Length > 0)
            {
                try {
                    chart = Notedata.ConvertJsonToChart(System.IO.File.ReadAllText(Path));
                }
                catch { MessageBox.Show(DialogResMgr.GetString("ChartLoadError")); }

                if (chart.BPM == 1)
                {
                    MessageBox.Show(DialogResMgr.GetString("ChartLoadNoBPM"));
                    chart.BPM = 120;
                }
                ResizeBox.Value = chart.Length / 48;
                BPMbox.Value = (decimal)chart.BPM;
                ResizeScrollbar();
                SetCurrTick(0);
                UpdateChart();

            }
        }


        private void LoadMusic(string Path)
        {
            if (Path.Length > 0)
            {
                WaveOut.Stop();

                try {
                    MusicFileReader = new MediaFoundationReader(Path);
                }
                catch { MessageBox.Show(DialogResMgr.GetString("MusicLoadError")); return; }
                
                WaveOut.Init(MusicFileReader);
            }
        }


        private void AddNoteTypes()
        {
            NoteTypeSelector.Items.Clear();

            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                //NoteTypeSelector.DataSource = Enum.GetValues(typeof(Notedata.UserVisibleNoteType_Nihongo));
                foreach (Notedata.UserVisibleNoteType_Nihongo type in Enum.GetValues(typeof(Notedata.UserVisibleNoteType_Nihongo)))
                {
                    NoteTypeSelector.Items.Add(type);
                }
            else
                // NoteTypeSelector.DataSource = Enum.GetValues(typeof(Notedata.UserVisibleNoteType));
                foreach (Notedata.UserVisibleNoteType type in Enum.GetValues(typeof(Notedata.UserVisibleNoteType)))
                {
                    NoteTypeSelector.Items.Add(type);
                }

            NoteTypeSelector.SelectedIndex = 0;
        }


        public Form1()
        {
            InitializeComponent();

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            AddNoteTypes();

            ActiveControl = ZoomLbl;

            NoteSoundWaveOut.Init(NoteSoundMixer);
            NoteSoundWaveOut.Play();

            ResizeScrollbar();
            SetCurrTick(0);
            UpdateChart();

            //chart.Ticks[0].SetNote(Notedata.NoteType.Hold, 7) ;
            //for (int i = 0; i < 32; i++)
            //{
            //    chart.Ticks[i * 48].SetNote(Notedata.NoteType.SimulTap, 3);
            //    chart.Ticks[i * 48].SetNote(Notedata.NoteType.SimulTap, 4);

            //    chart.Ticks[i * 48 + 12].SetNote(Notedata.NoteType.SimulTap, 2);
            //    chart.Ticks[i * 48 + 12].SetNote(Notedata.NoteType.SimulTap, 5);

            //    chart.Ticks[i * 48 + 24].SetNote(Notedata.NoteType.FlickRight, 1);
            //    chart.Ticks[i * 48 + 24].SetNote(Notedata.NoteType.FlickLeft, 6);

            //    chart.Ticks[i * 48 + 36].SetNote(Notedata.NoteType.Hold, 0);
            //    chart.Ticks[i * 48 + 36].SetNote(Notedata.NoteType.Tap, 7);

            //    chart.Ticks[i+1].SetNote(Notedata.NoteType.Hold, 7);
            //}
            //chart.Ticks[32].SetNote(Notedata.NoteType.Hold, 7);

            playTimer.Tick += playtimer_Tick;
        }

        private void ChartScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (PauseOnSeek.Checked) StopPlayback();
            SetCurrTick(chart.Length - e.NewValue / TickHeight);
            UpdateChart();
            if (MusicFileReader != null)
                try { MusicFileReader.CurrentTime = ConvertTicksToTime(CurrentTick); } catch { }
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            if (MusicFileReader != null)
                StartPlayback();
            else
            {
                MessageBox.Show(DialogResMgr.GetString("PlaybackNoMusicError"));
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            StopPlayback();
        }


        private void playtimer_Tick(object sender, EventArgs e)
        {
            SetCurrTick(ConvertTimeToTicks(MusicFileReader.CurrentTime));
            UpdateChart();

            if ((int)CurrentTick != LastTick)
            {
                int ltick = LastTick;
                LastTick = (int)CurrentTick;

                if (NoteSoundBox.Checked)
                {
                    for (int i = ltick + 1; i <= CurrentTick; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Notedata.NoteType note = FindVisualNoteType(i, j);

                            if (note != Notedata.NoteType.None && note != Notedata.NoteType.ExtendHoldMid &&
                                note != Notedata.NoteType.SwipeLeftMid && note != Notedata.NoteType.SwipeRightMid)
                            {
                                NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(NoteSoundSig);
                                NoteSoundTrim.Take = TimeSpan.FromMilliseconds(20);
                                NoteSoundMixer.AddMixerInput(NoteSoundTrim);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void BPMbox_ValueChanged(object sender, EventArgs e)
        {
            chart.BPM = (double)BPMbox.Value;
            ResizeScrollbar();
            if (MusicFileReader != null)
            {
                SetCurrTick(ConvertTimeToTicks(MusicFileReader.CurrentTime));
                UpdateChart();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeScrollbar();
            UpdateChart();
        }

        private void ProcessClick(int Tick, int Lane, MouseButtons MouseButton, Notedata.NoteType NewNote)
        {
            Console.WriteLine(Lane + ", " + Tick);

            if (Tick == -1 || Tick >= chart.Length)
                return;

            if (MouseButton == MouseButtons.Left)
            {
                if (FindVisualNoteType(Tick, Lane) != NewNote)
                {
                    chart.Ticks[Tick].Notes[Lane] = NewNote;

                    
                    if (NewNote == Notedata.NoteType.None)
                    {
                        ProcessClick(Tick, Lane, MouseButtons.Right, NewNote);
                        return;
                    }

                    UpdateChart();
                }

            }

            else if (MouseButton == MouseButtons.Right)
            {
                Notedata.NoteType OldNote = chart.Ticks[Tick].Notes[Lane];
                
                chart.Ticks[Tick].Notes[Lane] = Notedata.NoteType.None;
                
                UpdateChart();
            }
        }

        private void Chart_Click(object sender, MouseEventArgs e)
        {
            Control sendCtl = (Control)sender;
            sendCtl.Capture = false;

            int Lane = ConvertXCoordToNote(e.X);
            
            int Tick = (int)ConvertYCoordToTick(e.Y);

            ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
        }

        private void ZoomBox_ValueChanged(object sender, EventArgs e)
        {
            TickHeight = (int)ZoomBox.Value;
            IconHeight = TickHeight;
            ResizeScrollbar();
            UpdateChart();
        }

        private void ResizeBtn_Click(object sender, EventArgs e)
        {
            ResizeChart((int)ResizeBox.Value * 48);
            
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
             LoadChart(openFileDialog1.FileName);
        }

        private void SaveChartBtn_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                System.IO.File.WriteAllText(saveFileDialog1.FileName, Notedata.ConvertChartToJson(chart));
        }

        private void OpenMusicButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
                LoadMusic(openFileDialog2.FileName);
        }

        private void ImgSaveBtn_Click(object sender, EventArgs e)
        {
            /* float scaledivX = LaneWidth / 2; // these are the final pixel dimensions of each note in the image
            float scaledivY = TickHeight / 1;
            Bitmap img = new Bitmap((int)(LaneWidth * 8 * 8/scaledivX + 7), (int)(PanelHeight / scaledivY / 2));
            Graphics grfx = Graphics.FromImage(img);
            Bitmap tmpimg = new Bitmap(LaneWidth * 8, PanelHeight);

            ChartPanel.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, 0, -PanelHeight / scaledivY / 2, LaneWidth * 8/scaledivX, PanelHeight / scaledivY);
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 1 / scaledivX + 1, 0, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);

            ChartPanel2.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 2 / scaledivX + 2, -PanelHeight / scaledivY / 2, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 3 / scaledivX + 3, 0, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);

            ChartPanel3.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 4 / scaledivX + 4, -PanelHeight / scaledivY / 2, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 5 / scaledivX + 5, 0, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);

            ChartPanel4.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 6 / scaledivX + 6, -PanelHeight / scaledivY / 2, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);
            grfx.DrawImage(tmpimg, LaneWidth * 8 * 7 / scaledivX + 7, 0, LaneWidth * 8 / scaledivX, PanelHeight / scaledivY);

            img.Save("imgout.png");

            tmpimg.Dispose();
            grfx.Dispose();
            img.Dispose(); */
        }

        private void NoteShiftBtn_Click(object sender, EventArgs e)
        {
            if (NoteShiftBox.Value > 0)
            {
                List<Notedata.Tick> NewTicks = chart.Ticks.ToList();
                NewTicks.RemoveRange(chart.Length - (int)NoteShiftBox.Value, (int)NoteShiftBox.Value);
                NewTicks.InsertRange(0, new Notedata.Tick[(int)NoteShiftBox.Value]);
                chart.Ticks = NewTicks.ToArray();

                ResizeScrollbar();
                UpdateChart();
            }

            else if (NoteShiftBox.Value < 0)
            {
                List<Notedata.Tick> NewTicks = chart.Ticks.ToList();
                NewTicks.RemoveRange(0, - (int)NoteShiftBox.Value);
                NewTicks.AddRange(new Notedata.Tick[- (int)NoteShiftBox.Value]);
                chart.Ticks = NewTicks.ToArray();

                ResizeScrollbar();
                UpdateChart();
            }

            NoteShiftBox.Value = 0;


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            else
            {
                if (MessageBox.Show(DialogResMgr.GetString("ExitMessage"), DialogResMgr.GetString("ExitCaption"), MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void NoteCountButton_Click(object sender, EventArgs e)
        {
            int NoteCount = 0;

            for (int i = 0; i < chart.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                    if (NoteType != Notedata.NoteType.None && NoteType != Notedata.NoteType.ExtendHoldMid &&
                        NoteType != Notedata.NoteType.SwipeLeftMid && NoteType != Notedata.NoteType.SwipeRightMid)
                        NoteCount++;
                }
            }

            MessageBox.Show(String.Format(DialogResMgr.GetString("NoteCountMessage"), NoteCount));
        }

        private void AutoSimulBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chart.Length; i++)
            {
                int SimulNum_Tap = 0;
                int SimulNum_Hold = 0;

                for (int j = 0; j < 8; j++)
                {
                    // taps get drawn as simulnotes when swipes or flicks are present, but holds don't
                    Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                    if (NoteType != Notedata.NoteType.None && NoteType != Notedata.NoteType.ExtendHoldMid &&
                        NoteType != Notedata.NoteType.SwipeLeftMid && NoteType != Notedata.NoteType.SwipeRightMid)
                        SimulNum_Tap++;

                    if (NoteType == Notedata.NoteType.Tap || NoteType == Notedata.NoteType.SimulTap ||
                        NoteType == Notedata.NoteType.Hold || NoteType == Notedata.NoteType.SimulHoldStart || NoteType == Notedata.NoteType.SimulHoldRelease)
                        SimulNum_Hold++;
                }

                if (SimulNum_Tap > 1)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.Tap)
                        {
                            chart.Ticks[i].Notes[j] = Notedata.NoteType.SimulTap;

                            UpdateChart();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.SimulTap)
                        {
                            chart.Ticks[i].Notes[j] = Notedata.NoteType.Tap;

                            UpdateChart();
                        }
                    }
                }

                if (SimulNum_Hold > 1)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.Hold || NoteType == Notedata.NoteType.SimulHoldStart
                        || NoteType == Notedata.NoteType.SimulHoldRelease)
                        {
                            if (i + 1 < chart.Length && (chart.Ticks[i + 1].Notes[j] == Notedata.NoteType.Hold || chart.Ticks[i + 1].Notes[j] == Notedata.NoteType.SimulHoldRelease))
                                chart.Ticks[i].Notes[j] = Notedata.NoteType.SimulHoldStart;
                            else
                                chart.Ticks[i].Notes[j] = Notedata.NoteType.SimulHoldRelease;

                            UpdateChart();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.SimulHoldStart || NoteType == Notedata.NoteType.SimulHoldRelease)
                        {
                            chart.Ticks[i].Notes[j] = Notedata.NoteType.Hold;

                            UpdateChart();
                        }
                    }
                }
            }
        }

        private void LangChangeBtn_Click(object sender, EventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
            else
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja");


            SuspendLayout();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            resources.ApplyResources(this, "$this");
            foreach (Control Ctrl in Controls)
                resources.ApplyResources(Ctrl, Ctrl.Name);
            ResumeLayout();

            AddNoteTypes();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char key = e.KeyChar;
                if (Char.IsDigit(key))
                {
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType)Enum.Parse(typeof(Notedata.NoteShortcutKeys), "_" + key);
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType_Nihongo)Enum.Parse(typeof(Notedata.NoteShortcutKeys), "_" + key);
                }
                else
                {
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType)Enum.Parse(typeof(Notedata.NoteShortcutKeys), key.ToString().ToUpper());
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType_Nihongo)Enum.Parse(typeof(Notedata.NoteShortcutKeys), key.ToString().ToUpper());
                }
            }
            catch { }
        }
    }
}
