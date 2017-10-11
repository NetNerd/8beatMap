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
using System.Drawing.Imaging;

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


        private Timer playTimer = new Timer() { Interval = 3 };


        GameCloneRenderer_OGL OGLrenderer = new GameCloneRenderer_OGL(853, 480);
        

        Image GetChartImage(double startTick, int tickHeight, int iconWidth, int iconHeight, Color BgCol, bool NoGrid, int Width, int Height)
        {
            Image Bmp = new Bitmap(Width, Height);
            Graphics Grfx = Graphics.FromImage(Bmp);

            int width = Bmp.Width;
            int height = Bmp.Height;

            Grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            Grfx.Clear(BgCol);
            Grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            if (!NoGrid)
            {
                for (int i = 1; i < 8; i++)
                {
                    Grfx.FillRectangle(new SolidBrush(Color.LightGray), i * width / 8, 0, 1, height);
                }
            }
            


            float laneWidth = width / 8;
            float halfIconWidth = iconWidth / 2;
            int halfIconHeight = iconHeight / 2;

            for (int i = (int)startTick - 24; i < startTick + height / tickHeight; i++)
            {
                if (i >= chart.Length) break;
                if (i < 0) i = 0;

                if (i % 48 == 0)
                {
                    Grfx.FillRectangle(Brushes.SlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 2, width, 1);
                }

                for (int j = 0; j < 8; j++)
                {
                    Color noteCol = Color.FromArgb(0xc0, 0xc0, 0xc0);
                    Color ArrowCol = Color.Transparent;
                    int ArrowDir = 0;

                    Notedata.NoteType Type = chart.FindVisualNoteType(i, j);

                    if (chart.swipeEnds[i * 8 + j] == 0)
                    {
                        Point swipeEndPoint = chart.swipeEndpointNodes[i * 8 + j];

                        if (swipeEndPoint.X > i)
                            Grfx.DrawLine(new Pen(Color.LightGray, iconWidth / 3), (float)(j + 0.5) * laneWidth, height - (float)(i - startTick + 1) * tickHeight - 2, (float)(swipeEndPoint.Y + 0.5) * laneWidth, height - (float)(swipeEndPoint.X - startTick + 1) * tickHeight - 2);
                            
                    }

                    switch (Type)
                    {
                        case Notedata.NoteType.Tap: noteCol = Color.Blue; break;
                        case Notedata.NoteType.Hold: noteCol = Color.LimeGreen; break;
                        case Notedata.NoteType.SimulTap:
                        case Notedata.NoteType.SimulHoldStart:
                        case Notedata.NoteType.SimulHoldRelease: noteCol = Color.DeepPink; break;
                        case Notedata.NoteType.FlickLeft: ArrowCol = Color.FromArgb(0x70, 0, 0x78); ArrowDir = -1; break;
                        case Notedata.NoteType.HoldEndFlickLeft: ArrowCol = Color.FromArgb(0x70, 0, 0x78); noteCol = Color.LightGray; ArrowDir = -1; break;
                        case Notedata.NoteType.SwipeLeftStartEnd: ArrowCol = Color.DarkViolet; ArrowDir = -1; break;
                        case Notedata.NoteType.SwipeLeftMid:
                        case Notedata.NoteType.SwipeChangeDirR2L: ArrowCol = Color.Violet; ArrowDir = -1; break;
                        case Notedata.NoteType.FlickRight: ArrowCol = Color.FromArgb(0xcc, 0x88, 0); ArrowDir = 1; break;
                        case Notedata.NoteType.HoldEndFlickRight: ArrowCol = Color.FromArgb(0xcc, 0x88, 0); noteCol = Color.LightGray; ArrowDir = 1; break;
                        case Notedata.NoteType.SwipeRightStartEnd: ArrowCol = Color.DarkOrange; ArrowDir = 1; break;
                        case Notedata.NoteType.SwipeRightMid:
                        case Notedata.NoteType.SwipeChangeDirL2R: ArrowCol = Color.Gold; ArrowDir = 1; break;
                        case Notedata.NoteType.ExtendHoldMid: noteCol = Color.LightGray; break;
                    }


                    if (chart.Ticks[i].Notes[j] != Notedata.NoteType.None)
                    {
                        int iconX = (int)((j + 0.5) * laneWidth - halfIconWidth);
                        int iconY = (int)Math.Ceiling(height - (i - startTick + 1.5) * tickHeight - 2);

                        Grfx.FillRectangle(new SolidBrush(noteCol), iconX, iconY, iconWidth, iconHeight);
                        if (ArrowDir == -1)
                            Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(iconX + iconWidth - 1, iconY + 0), new Point(iconX + iconWidth - 1, iconY + iconHeight - 1), new Point(iconX + 0, iconY + halfIconHeight) });
                        else if (ArrowDir == 1)
                            Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(iconX + 0, iconY + 0), new Point(iconX + 0, iconY + iconHeight - 1), new Point(iconX + iconWidth - 1, iconY + halfIconHeight) });
                    }
                }

                if (!NoGrid)
                {
                    if (i % 48 == 0)
                    {
                        Grfx.FillRectangle(Brushes.SlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 3, width, 3);
                        Grfx.DrawString((i / 48 + 1).ToString(), new System.Drawing.Font("Arial", 6.5f), Brushes.DarkSlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 13);
                    }
                    else if (i % 12 == 0)
                    {
                        Grfx.FillRectangle(Brushes.LightSlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 2, width, 1);
                    }
                    else if (i % 6 == 0)
                    {
                        Grfx.FillRectangle(Brushes.LightGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 2, width, 1);
                    }
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

            if (Sound.MusicReader != null &&
                    (Sound.MusicReader.CurrentTime < chart.ConvertTicksToTime(CurrentTick) - TimeSpan.FromMilliseconds(MusicDelayMs - 3) |
                    Sound.MusicReader.CurrentTime > chart.ConvertTicksToTime(CurrentTick) - TimeSpan.FromMilliseconds(MusicDelayMs + 3)))
                try { Sound.MusicReader.CurrentTime = chart.ConvertTicksToTime(CurrentTick) - TimeSpan.FromMilliseconds(MusicDelayMs); } catch { }

            ChartScrollBar.Value = (int)(chart.Length * TickHeight - tick * TickHeight);
        }



        int VideoDelayMs = 50;

        private void UpdateChart()
        {
            double tick = CurrentTick;
            if (playTimer.Enabled)
            {
                tick -= chart.ConvertTimeToTicks(TimeSpan.FromMilliseconds(VideoDelayMs));
            }
            pictureBox1.Image.Dispose();
            pictureBox1.Image = GetChartImage(tick, TickHeight, IconWidth, IconHeight, SystemColors.ControlLight, false, pictureBox1.Width, pictureBox1.Height);

            OGLrenderer.currentTick = tick;
            OGLrenderer.numTicksVisible = (int)chart.ConvertTimeToTicks(TimeSpan.FromMilliseconds(700));
            OGLrenderer.chart = chart;
        }


        private int ConvertXCoordToNote(int X)
        {
            return ((X - pictureBox1.Location.X) / (pictureBox1.Width/8));
        }

        private double ConvertYCoordToTick(int Y)
        {
            return (pictureBox1.Location.Y + pictureBox1.Height - Y - 2 + CurrentTick%1 - TickHeight/2) / TickHeight + CurrentTick;
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
            Sound.PlayMusic();
        }

        private void StopPlayback()
        {
            playTimer.Enabled = false;
            Sound.StopMusic();
            UpdateChart();
        }


        private void LoadChart(string Path)
        {
            StopPlayback();

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

            
            try
            {
                Sound.NoteSoundWave = new Sound.CachedSound("notesnd/hit.wav");
                Sound.NoteSoundWave_Swipe = new Sound.CachedSound("notesnd/swipe.wav");
                //NoteSoundMixer.AddMixerInput(NoteSoundWave);
                //NoteSoundMixer.AddMixerInput(NoteSoundWave_Swipe);
                //Sound.SetNoteSoundLatency(95);
            }
            catch
            {
                Sound.NoteSoundWave = null;
                Sound.NoteSoundWave_Swipe = null;
            }


            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            AddNoteTypes();

            ActiveControl = ZoomLbl;

            Sound.InitWaveOut();

            ResizeScrollbar();
            SetCurrTick(0);
            UpdateChart();

            playTimer.Tick += playtimer_Tick;
        }


        private void ChartScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (PauseOnSeek.Checked) StopPlayback();
            SetCurrTick(chart.Length - e.NewValue / TickHeight);
            UpdateChart();
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            if (Sound.MusicReader != null)
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


        int MusicDelayMs = 20;

        double lastPlayTickTime = 0;
        private void playtimer_Tick(object sender, EventArgs e)
        {
            SetCurrTick(chart.ConvertTimeToTicks(Sound.MusicReader.CurrentTime + TimeSpan.FromMilliseconds(MusicDelayMs)));
            if (lastPlayTickTime < chart.ConvertTicksToTime(CurrentTick).TotalMilliseconds - 5 | lastPlayTickTime > chart.ConvertTicksToTime(CurrentTick).TotalMilliseconds)
            {
                lastPlayTickTime = chart.ConvertTicksToTime(CurrentTick).TotalMilliseconds;
                UpdateChart(); //(update graphics)
            }

            if ((int)CurrentTick != LastTick)
            {
                int ltick = LastTick;
                LastTick = (int)CurrentTick;

                if ((LastTick - ltick) > 5) //replace the last tick recorded with current tick if time difference is too large
                    ltick = LastTick;

                if (NoteSoundBox.Checked)
                {
                    for (int i = ltick + 1; i <= CurrentTick; i++) //process for all ticks since the last one
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Notedata.NoteType note = chart.FindVisualNoteType(i, j);

                            if (Sound.NoteSoundWave != null)
                            {
                                if (note == Notedata.NoteType.Tap || note == Notedata.NoteType.SimulTap || note == Notedata.NoteType.Hold
                                    || note == Notedata.NoteType.SimulHoldStart || note == Notedata.NoteType.SimulHoldRelease)
                                {
                                    //Sound.PlayNoteSound(Sound.NoteSoundWave);
                                    Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(new Sound.CachedSoundSampleProvider(Sound.NoteSoundWave));
                                    if (MusicDelayMs > 30)
                                        Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs - 30);
                                    else
                                        Sound.NoteSoundTrim.SkipOver = TimeSpan.FromMilliseconds(30 - MusicDelayMs);
                                    Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                }

                                else if ((
                                    (note == Notedata.NoteType.SwipeLeftStartEnd | note == Notedata.NoteType.SwipeRightStartEnd) && chart.swipeEnds[i * 8 + j] == 0)
                                    || note == Notedata.NoteType.SwipeChangeDirR2L | note == Notedata.NoteType.SwipeChangeDirL2R
                                    || note == Notedata.NoteType.FlickLeft | note == Notedata.NoteType.HoldEndFlickLeft
                                    || note == Notedata.NoteType.FlickRight | note == Notedata.NoteType.HoldEndFlickRight)
                                {
                                    //Sound.PlayNoteSound(Sound.NoteSoundWave_Swipe);
                                    Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(new Sound.CachedSoundSampleProvider(Sound.NoteSoundWave_Swipe));
                                    if (MusicDelayMs > 30)
                                        Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs - 30);
                                    else
                                        Sound.NoteSoundTrim.SkipOver = TimeSpan.FromMilliseconds(30 - MusicDelayMs);
                                    Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                }
                            }

                            else if (note != Notedata.NoteType.None && note != Notedata.NoteType.ExtendHoldMid &&
                                note != Notedata.NoteType.SwipeLeftMid && note != Notedata.NoteType.SwipeRightMid)
                            {
                                Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(Sound.NoteSoundSig);
                                Sound.NoteSoundTrim.Take = TimeSpan.FromMilliseconds(20);
                                Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs + 5);
                                Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                return;
                            }
                        }
                    }
                }
            }

            if (CurrentTick == chart.Length - 1 || Sound.MusicReader.CurrentTime == Sound.MusicReader.TotalTime)
            {
                StopPlayback();
            }
        }

        private void BPMbox_ValueChanged(object sender, EventArgs e)
        {
            chart.BPM = (double)BPMbox.Value;
            ResizeScrollbar();
            if (Sound.MusicReader != null)
            {
                SetCurrTick(chart.ConvertTimeToTicks(Sound.MusicReader.CurrentTime + TimeSpan.FromMilliseconds(MusicDelayMs)));
                UpdateChart();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Height != Height)
            {
                if (ClientSize.Height == 0)
                    pictureBox1.Height = 1;
                else
                    pictureBox1.Height = ClientSize.Height;
                Image bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = bmp;
                UpdateChart();
            }
        }

        private void ProcessClick(int Tick, int Lane, MouseButtons MouseButton, Notedata.NoteType NewNote)
        {
            //Console.WriteLine(Lane + ", " + Tick);

            if (Tick == -1 | Tick >= chart.Length)
                return;

            if (Lane < 0 | Lane > 7)
                return;

            if (MouseButton == MouseButtons.Left)
            {
                if (chart.Ticks[Tick].Notes[Lane] != NewNote)
                {
                    if (NewNote == Notedata.NoteType.None)
                    {
                        ProcessClick(Tick, Lane, MouseButtons.Right, NewNote);
                        return;
                    }

                    chart.Ticks[Tick].SetNote(NewNote, Lane, ref chart);
                    UpdateChart();
                }

            }

            else if (MouseButton == MouseButtons.Right)
            {
                if (chart.Ticks[Tick].Notes[Lane] != Notedata.NoteType.None)
                {
                    chart.Ticks[Tick].SetNote(Notedata.NoteType.None, Lane, ref chart);
                    UpdateChart();
                }
            }
        }

        private void Chart_Click(object sender, MouseEventArgs e)
        {
            Control sendCtl = (Control)sender;
            sendCtl.Capture = false;

            pictureBox1.Focus();

            int Lane = ConvertXCoordToNote(e.X);
            int Tick = (int)ConvertYCoordToTick(e.Y);

            ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left | e.Button == MouseButtons.Right)
            {
                int Lane = ConvertXCoordToNote(e.X);
                int Tick = (int)ConvertYCoordToTick(e.Y);

                ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
            }
        }

        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            SetCurrTick(CurrentTick + e.Delta / 15);
            UpdateChart();
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
            UpdateChart();
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
             LoadChart(openFileDialog1.FileName);
        }

        private void SaveChartBtn_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                System.IO.File.WriteAllText(saveFileDialog1.FileName, Notedata.ConvertChartToJson_Small(chart));
        }

        private void OpenMusicButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
                Sound.LoadMusic(openFileDialog2.FileName);
        }

        private void ImgSaveBtn_Click(object sender, EventArgs e)
        {
            int TicksPerCol = 48 * 8; //8 bars
            int TickHeight = 1;
            int ColWidth = 16;
            int NoteHeight = 1;
            int NoteWidth = 2;

            int NumCols = (chart.Ticks.Length - 1) / TicksPerCol + 1;

            Bitmap img = new Bitmap(NumCols * ColWidth + NumCols - 1, TicksPerCol * NoteHeight);
            Graphics grfx = Graphics.FromImage(img);

            grfx.Clear(SystemColors.ControlDark);

            for (int i = 0; i < NumCols; i++)
            {
                grfx.DrawImage(GetChartImage(i * TicksPerCol, TickHeight, NoteWidth, NoteHeight, SystemColors.ControlLight, true, ColWidth, TicksPerCol * NoteHeight + 1), i + i * ColWidth, 0);
            }

            img.Save("imgout.png");
            
            grfx.Dispose();
            img.Dispose();
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
                chart.FixSwipes();
                UpdateChart();
            }

            else if (NoteShiftBox.Value < 0)
            {
                List<Notedata.Tick> NewTicks = chart.Ticks.ToList();
                NewTicks.RemoveRange(0, - (int)NoteShiftBox.Value);
                NewTicks.AddRange(new Notedata.Tick[- (int)NoteShiftBox.Value]);
                chart.Ticks = NewTicks.ToArray();

                ResizeScrollbar();
                chart.FixSwipes();
                UpdateChart();
            }

            NoteShiftBox.Value = 0;


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
            {
                OGLrenderer.Stop();
                return;
            }

            if (MessageBox.Show(DialogResMgr.GetString("ExitMessage"), DialogResMgr.GetString("ExitCaption"), MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
            else
                OGLrenderer.Stop();
        }

        private void NoteCountButton_Click(object sender, EventArgs e)
        {
            int NoteCount = 0;

            for (int i = 0; i < chart.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Notedata.NoteType NoteType = chart.FindVisualNoteType(i, j);
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
                    Notedata.NoteType NoteType = chart.FindVisualNoteType(i, j);
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
                        Notedata.NoteType NoteType = chart.FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.Tap)
                        {
                            chart.Ticks[i].SetNote(Notedata.NoteType.SimulTap, j, ref chart);

                            //UpdateChart();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = chart.FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.SimulTap)
                        {
                            chart.Ticks[i].SetNote(Notedata.NoteType.Tap, j, ref chart);

                            //UpdateChart();
                        }
                    }
                }

                if (SimulNum_Hold > 1)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = chart.FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.Hold || NoteType == Notedata.NoteType.SimulHoldStart
                        || NoteType == Notedata.NoteType.SimulHoldRelease)
                        {
                            if (i + 1 < chart.Length && (chart.Ticks[i + 1].Notes[j] == Notedata.NoteType.Hold || chart.Ticks[i + 1].Notes[j] == Notedata.NoteType.SimulHoldRelease))
                                chart.Ticks[i].SetNote(Notedata.NoteType.SimulHoldStart, j, ref chart);
                            else
                                chart.Ticks[i].SetNote(Notedata.NoteType.SimulHoldRelease, j, ref chart);

                            //UpdateChart();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = chart.FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.SimulHoldStart || NoteType == Notedata.NoteType.SimulHoldRelease)
                        {
                            chart.Ticks[i].SetNote(Notedata.NoteType.Hold, j, ref chart);

                            //UpdateChart();
                        }
                    }
                }
            }

            UpdateChart();
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
                    if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                        NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType_Nihongo)Enum.Parse(typeof(Notedata.NoteShortcutKeys), "_" + key);
                    else
                        NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType)Enum.Parse(typeof(Notedata.NoteShortcutKeys), "_" + key);

                }
                else
                {
                    if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                        NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType_Nihongo)Enum.Parse(typeof(Notedata.NoteShortcutKeys), key.ToString().ToUpper());
                    else
                        NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType)Enum.Parse(typeof(Notedata.NoteShortcutKeys), key.ToString().ToUpper());
                }
            }
            catch { }
        }
    }
}
