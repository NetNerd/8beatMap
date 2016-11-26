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
        private int LaneWidth = 36;
        private int IconWidth = 20;
        private int IconHeight = 10;
        private int PanelHeight = 23040; // max is 2304 ticks per panel (total 9216) or 48 bars (total 192)
                                         // 192 bars is close to 6:30 at 120bpm, so it should be enough for most stuff
        private double CurrentTick = 0;
        private int LastTick = 0;

        private Timer playTimer = new Timer() { Interval = 4 };
        
        WaveOutEvent WaveOut = new WaveOutEvent { DesiredLatency = 100, NumberOfBuffers = 16 };
        MediaFoundationReader MusicFileReader;

        WaveOutEvent NoteSoundWaveOut = new WaveOutEvent { DesiredLatency = 110, NumberOfBuffers = 4 };
        static NAudio.Wave.SampleProviders.SignalGenerator NoteSoundSig = new NAudio.Wave.SampleProviders.SignalGenerator { Frequency = 1000, Gain = 0.5, Type = NAudio.Wave.SampleProviders.SignalGeneratorType.Square };
        NAudio.Wave.SampleProviders.OffsetSampleProvider NoteSoundTrim;
        NAudio.Wave.SampleProviders.MixingSampleProvider NoteSoundMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2)) { ReadFully = true };
        

        private PictureBox MakeNoteBox(int Tick, int Lane, Notedata.NoteType Type)
        {
            Color noteCol = Color.LightGray;
            Color ArrowCol = Color.Transparent;
            int ArrowDir = 0;

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

            Image Bmp = new Bitmap(17, 17);
            Graphics Grfx = Graphics.FromImage(Bmp);

            if (ArrowDir == -1)
                Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(16, 0), new Point(16, 16), new Point(0, 8) });
            else if (ArrowDir == 1)
                Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(0, 0), new Point(0, 16), new Point(16, 8) });


            int Top = PanelHeight - Tick * TickHeight - IconHeight;
            PictureBox NoteBox = new PictureBox { Left = Lane * LaneWidth + (LaneWidth - IconWidth) / 2, Top = Top, Width = IconWidth, Height = IconHeight, BackColor = noteCol, Image = Bmp, SizeMode = PictureBoxSizeMode.StretchImage };

            if (Type == Notedata.NoteType.HoldEndFlickLeft || Type == Notedata.NoteType.HoldEndFlickRight || Type == Notedata.NoteType.SimulHoldRelease
                || Type == Notedata.NoteType.SwipeChangeDirL2R || Type == Notedata.NoteType.SwipeChangeDirR2L)
                NoteBox.BorderStyle = BorderStyle.Fixed3D;
            else
                NoteBox.BorderStyle = BorderStyle.None;

            
            NoteBox.MouseDown += new System.Windows.Forms.MouseEventHandler(NoteBox_Click);
            
            NoteBox.MouseEnter += new EventHandler(NoteBox_MouseEnter);
            NoteBox.MouseLeave += new EventHandler(NoteBox_MouseLeave);

            return NoteBox;
        }

        private void AddSingleNoteIcon(int Tick, int Lane, Notedata.NoteType Type)
        {
            PictureBox NoteBox = MakeNoteBox(Tick, Lane, Type);

            chart.Ticks[Tick].NoteIcons[Lane] = NoteBox;
            
            if (NoteBox.Top >= 0)
                ChartPanel.Controls.Add(NoteBox);
            else if (NoteBox.Top >= 0 - PanelHeight)
            {
                NoteBox.Top += PanelHeight;
                ChartPanel2.Controls.Add(NoteBox);
            }
            else if (NoteBox.Top >= 0 - PanelHeight * 2)
            {
                NoteBox.Top += PanelHeight * 2;
                ChartPanel3.Controls.Add(NoteBox);
            }
            else
            {
                NoteBox.Top += PanelHeight * 3;
                ChartPanel4.Controls.Add(NoteBox);
            }
        }

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

        private void AddNoteIcons(NoteDataInfo[] Data)
        {
            List<PictureBox> icons = new List<PictureBox>();
            List<PictureBox> icons2 = new List<PictureBox>();
            List<PictureBox> icons3 = new List<PictureBox>();
            List<PictureBox> icons4 = new List<PictureBox>();

            foreach (NoteDataInfo notedata in Data)
            {
                PictureBox NoteBox = MakeNoteBox(notedata.Tick, notedata.Lane, notedata.Type);

                chart.Ticks[notedata.Tick].NoteIcons[notedata.Lane] = NoteBox;

                if (NoteBox.Top >= 0)
                    icons.Add(NoteBox);
                else if (NoteBox.Top >= 0 - PanelHeight)
                {
                    NoteBox.Top += PanelHeight;
                    icons2.Add(NoteBox);
                }
                else if (NoteBox.Top >= 0 - PanelHeight * 2)
                {
                    NoteBox.Top += PanelHeight * 2;
                    icons3.Add(NoteBox);
                }
                else
                {
                    NoteBox.Top += PanelHeight * 3;
                    icons4.Add(NoteBox);
                }
            }

            ChartPanel.Controls.AddRange(icons.ToArray());
            ChartPanel2.Controls.AddRange(icons2.ToArray());
            ChartPanel3.Controls.AddRange(icons3.ToArray());
            ChartPanel4.Controls.AddRange(icons4.ToArray());
        }

        private Notedata.NoteType FindVisualNoteType(int tick, int lane)
        {
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

        private void RedrawAllNoteIcons()
        {
            ChartPanel.Top = 0;
            ChartPanel2.Top = 0;
            ChartPanel3.Top = 0;
            ChartPanel4.Top = 0;

            ChartPanel.Controls.Clear();
            ChartPanel2.Controls.Clear();
            ChartPanel3.Controls.Clear();
            ChartPanel4.Controls.Clear();
            // ChartPanel.Controls.Add(playHead);


            List<PictureBox> grid = new List<PictureBox>();
            List<PictureBox> grid2 = new List<PictureBox>();
            List<PictureBox> grid3 = new List<PictureBox>();
            List<PictureBox> grid4 = new List<PictureBox>();

            for (int i = 0; i < chart.Length / 48; i++)
            {
                int Top = PanelHeight - i * 48 * TickHeight - 1;
                PictureBox GridLine = new PictureBox { Left = 0, Top = Top, Width = LaneWidth * 8, Height = 3, BackColor = Color.SlateGray };

                if (Top >= 0)
                    grid.Add(GridLine);
                else if (Top >= 0 - PanelHeight)
                {
                    GridLine.Top += PanelHeight;
                    grid2.Add(GridLine);
                }
                else if (Top >= 0 - PanelHeight * 2)
                {
                    GridLine.Top += PanelHeight * 2;
                    grid3.Add(GridLine);
                }
                else
                {
                    GridLine.Top += PanelHeight * 3;
                    grid4.Add(GridLine);
                }
            }


            for (int i = 0; i < chart.Length / 12; i++)
            {
                int Top = PanelHeight - i * 12 * TickHeight;
                PictureBox GridLine = new PictureBox { Left = 0, Top = Top, Width = LaneWidth * 8, Height = 1, BackColor = Color.LightSlateGray };

                if (Top >= 0)
                    grid.Add(GridLine);
                else if (Top >= 0 - PanelHeight)
                {
                    GridLine.Top += PanelHeight;
                    grid2.Add(GridLine);
                }
                else if (Top >= 0 - PanelHeight * 2)
                {
                    GridLine.Top += PanelHeight * 2;
                    grid3.Add(GridLine);
                }
                else
                {
                    GridLine.Top += PanelHeight * 3;
                    grid4.Add(GridLine);
                }
            }

            for (int i = 0; i < chart.Length / 6; i++)
            {
                int Top = PanelHeight - i * 6 * TickHeight;
                PictureBox GridLine = new PictureBox { Left = 0, Top = Top, Width = LaneWidth * 8, Height = 1, BackColor = Color.LightGray };

                if (Top >= 0)
                    grid.Add(GridLine);
                else if (Top >= 0 - PanelHeight)
                {
                    GridLine.Top += PanelHeight;
                    grid2.Add(GridLine);
                }
                else if (Top >= 0 - PanelHeight * 2)
                {
                    GridLine.Top += PanelHeight * 2;
                    grid3.Add(GridLine);
                }
                else
                {
                    GridLine.Top += PanelHeight * 3;
                    grid4.Add(GridLine);
                }
            }


            for (int i = 1; i < 8; i++)
            {
                PictureBox GridLine1 = new PictureBox { Left = i * LaneWidth, Top = 0, Width = 1, Height = PanelHeight, BackColor = Color.LightGray };
                PictureBox GridLine2 = new PictureBox { Left = i * LaneWidth, Top = 0, Width = 1, Height = PanelHeight, BackColor = Color.LightGray };
                PictureBox GridLine3 = new PictureBox { Left = i * LaneWidth, Top = 0, Width = 1, Height = PanelHeight, BackColor = Color.LightGray };
                PictureBox GridLine4 = new PictureBox { Left = i * LaneWidth, Top = 0, Width = 1, Height = PanelHeight, BackColor = Color.LightGray };

                grid.Add(GridLine1);
                grid2.Add(GridLine2);
                grid3.Add(GridLine3);
                grid4.Add(GridLine4);
            }

            ChartPanel.Controls.AddRange(grid.ToArray());
            ChartPanel2.Controls.AddRange(grid2.ToArray());
            ChartPanel3.Controls.AddRange(grid3.ToArray());
            ChartPanel4.Controls.AddRange(grid4.ToArray());



            List<NoteDataInfo> Notes = new List<NoteDataInfo>();

            for (int i = 0; i < chart.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (chart.Ticks[i].Notes[j] != Notedata.NoteType.None)
                    {
                        Notes.Add(new NoteDataInfo(i, j, FindVisualNoteType(i, j)));
                    }
                }
            }

            AddNoteIcons(Notes.ToArray());

            PositionPanel(CurrentTick);
        }

        private int ConvertXCoordToNote(int X)
        {
            return (X / LaneWidth);
        }

        private double ConvertYCoordToTick(int Y)
        {
            // return (-Y - IconHeight / 2) / TickHeight + chart.Length;
            return (PanelHeight - Y - 1) / TickHeight;
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
            ChartScrollBar.Maximum = (int)(1.015 * chart.Length * TickHeight + IconHeight / 2);
        }

        private void PositionPanel(double tick)
        {
            if (tick < 0) tick = 0;
            if (tick >= chart.Length) tick = chart.Length - 1;

            CurrentTick = tick;
            // playHead.Left = 0;
            // playHead.Top = chart.Length * TickHeight - tick * TickHeight + 2;
            
            ChartScrollBar.Value = (int)(chart.Length * TickHeight - tick * TickHeight);

            int p1Top = this.ClientSize.Height - PanelHeight + (int)(tick * TickHeight) - IconHeight / 2 - 2;
            ChartPanel.Top = p1Top;
            ChartPanel2.Top = p1Top - PanelHeight;
            ChartPanel3.Top = p1Top - PanelHeight * 2;
            ChartPanel4.Top = p1Top - PanelHeight * 3;
        }


        private void ResizeChart(int NewLen)
        {
            chart.Length = NewLen;
            ResizeScrollbar();
            RedrawAllNoteIcons();
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
                CurrentTick = 0;
                BPMbox.Value = (decimal)chart.BPM;
                ResizeScrollbar();
                RedrawAllNoteIcons();

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

            AddNoteTypes();

            ActiveControl = ZoomLbl;

            NoteSoundWaveOut.Init(NoteSoundMixer);
            NoteSoundWaveOut.Play();

            ResizeScrollbar();
            RedrawAllNoteIcons();

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
            // PositionPlayhead(( - e.NewValue - IconHeight/2) / TickHeight + chart.Length);
            PositionPanel(chart.Length - e.NewValue / TickHeight);
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
            PositionPanel(ConvertTimeToTicks(MusicFileReader.CurrentTime));

            if (CurrentTick != LastTick)
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
                PositionPanel(ConvertTimeToTicks(MusicFileReader.CurrentTime));
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeScrollbar();
            PositionPanel(CurrentTick);
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

                    PictureBox Icn = chart.Ticks[Tick].NoteIcons[Lane];

                    
                    if (NewNote == Notedata.NoteType.None)
                    {
                        ProcessClick(Tick, Lane, MouseButtons.Right, NewNote);
                        return;
                    }


                    try { Icn.Parent.Controls.Remove(Icn); } catch { }
                    AddSingleNoteIcon(Tick, Lane, FindVisualNoteType(Tick, Lane));

                    //if ((NewNote == Notedata.NoteType.Hold || NewNote == Notedata.NoteType.SimulHoldRelease ||
                    //    NewNote == Notedata.NoteType.HoldEndFlickLeft || NewNote == Notedata.NoteType.HoldEndFlickRight)
                    //    && Tick > 0)
                    //{
                    //    Notedata.NoteType NewNote2 = chart.Ticks[Tick-1].Notes[Lane];
                    //    if (NewNote2 == Notedata.NoteType.Hold || NewNote2 == Notedata.NoteType.SimulHoldRelease)
                    //        ProcessClick(Tick - 1, Lane, MouseButtons.Left, NewNote2, true);
                    //}

                    if (NewNote == Notedata.NoteType.Hold || NewNote == Notedata.NoteType.SimulHoldRelease ||
                    NewNote == Notedata.NoteType.HoldEndFlickLeft || NewNote == Notedata.NoteType.HoldEndFlickRight)
                    {
                        int i = Tick - 1;
                        if (i < 0)
                            i = 0;
                        while (chart.Ticks[i].Notes[Lane] == Notedata.NoteType.Hold || chart.Ticks[i].Notes[Lane] == Notedata.NoteType.SimulHoldRelease)
                        {
                            i--;
                            if (i < 0)
                                break;
                        }

                        i++;

                        while (chart.Ticks[i].Notes[Lane] == Notedata.NoteType.Hold || chart.Ticks[i].Notes[Lane] == Notedata.NoteType.SimulHoldRelease)
                        {
                            PictureBox Icn2 = chart.Ticks[i].NoteIcons[Lane];
                            try { Icn2.Parent.Controls.Remove(Icn2); } catch { }
                            AddSingleNoteIcon(i, Lane, FindVisualNoteType(i, Lane));

                            i++;
                            if (i >= chart.Length)
                                break;
                        }
                    }
                }

            }

            else if (MouseButton == MouseButtons.Right)
            {
                Notedata.NoteType OldNote = chart.Ticks[Tick].Notes[Lane];
                
                chart.Ticks[Tick].Notes[Lane] = Notedata.NoteType.None;
                
                if (OldNote == Notedata.NoteType.Hold || OldNote == Notedata.NoteType.SimulHoldRelease ||
                    OldNote == Notedata.NoteType.SimulHoldStart ||
                    OldNote == Notedata.NoteType.HoldEndFlickLeft || OldNote == Notedata.NoteType.HoldEndFlickRight)
                {
                    if (Tick > 0)
                    {
                        Notedata.NoteType OldNote2 = chart.Ticks[Tick - 1].Notes[Lane];
                        if (OldNote2 == Notedata.NoteType.Hold || OldNote2 == Notedata.NoteType.SimulHoldRelease)
                        {
                            PictureBox Icn2 = chart.Ticks[Tick - 1].NoteIcons[Lane];
                            try { Icn2.Parent.Controls.Remove(Icn2); } catch { }
                            AddSingleNoteIcon(Tick - 1, Lane, FindVisualNoteType(Tick - 1, Lane));
                        }
                        }

                    if (Tick < chart.Length - 1)
                    {
                        Notedata.NoteType OldNote2 = chart.Ticks[Tick + 1].Notes[Lane];
                        if (OldNote2 == Notedata.NoteType.Hold || OldNote2 == Notedata.NoteType.SimulHoldRelease)
                        {
                            PictureBox Icn2 = chart.Ticks[Tick + 1].NoteIcons[Lane];
                            try { Icn2.Parent.Controls.Remove(Icn2); } catch { }
                            AddSingleNoteIcon(Tick + 1, Lane, FindVisualNoteType(Tick + 1, Lane));

                        }
                    }
                }
                
                PictureBox Icn = chart.Ticks[Tick].NoteIcons[Lane];

                try { Icn.Parent.Controls.Remove(Icn); } catch { }
            }
        }

        private void NoteBox_Click(object sender, MouseEventArgs e)
        {
            Control sendCtl = (Control)sender;
            sendCtl.Capture = false;

            int Lane = ConvertXCoordToNote(ChartPanel.PointToClient(MousePosition).X);

            int Tick = -1;
            if (sendCtl.Parent == ChartPanel)
                Tick = (int)ConvertYCoordToTick(ChartPanel.PointToClient(MousePosition).Y);
            else if (sendCtl.Parent == ChartPanel2)
                Tick = (int)ConvertYCoordToTick(ChartPanel2.PointToClient(MousePosition).Y) + PanelHeight / TickHeight;
            else if (sendCtl.Parent == ChartPanel3)
                Tick = (int)ConvertYCoordToTick(ChartPanel3.PointToClient(MousePosition).Y) + 2 * PanelHeight / TickHeight;
            else if (sendCtl.Parent == ChartPanel4)
                Tick = (int)ConvertYCoordToTick(ChartPanel4.PointToClient(MousePosition).Y) + 3 * PanelHeight / TickHeight;

            ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
        }

        private void NoteBox_MouseLeave(object sender, EventArgs e)
        {
            if (MouseButtons != MouseButtons.None)
            {
                Control sendCtl = (Control)sender;
                sendCtl.Capture = false;

                int Lane = ConvertXCoordToNote(ChartPanel.PointToClient(MousePosition).X);

                int YOffset;

                if (sendCtl.PointToClient(MousePosition).Y < TickHeight / 2)
                    YOffset = -TickHeight / 2;
                else
                    YOffset = TickHeight / 2;

                int Tick = -1;
                if (sendCtl.Parent == ChartPanel)
                    Tick = (int)ConvertYCoordToTick(ChartPanel.PointToClient(MousePosition).Y + YOffset);
                else if (sendCtl.Parent == ChartPanel2)
                    Tick = (int)ConvertYCoordToTick(ChartPanel2.PointToClient(MousePosition).Y + YOffset) + PanelHeight / TickHeight;
                else if (sendCtl.Parent == ChartPanel3)
                    Tick = (int)ConvertYCoordToTick(ChartPanel3.PointToClient(MousePosition).Y + YOffset) + 2 * PanelHeight / TickHeight;
                else if (sendCtl.Parent == ChartPanel4)
                    Tick = (int)ConvertYCoordToTick(ChartPanel4.PointToClient(MousePosition).Y + YOffset) + 3 * PanelHeight / TickHeight;

                ProcessClick(Tick, Lane, MouseButtons, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
            }
        }

        private void NoteBox_MouseEnter(object sender, EventArgs e)
        {
            if (MouseButtons != MouseButtons.None)
                NoteBox_Click(sender, new MouseEventArgs(MouseButtons, 1, 0, 0, 0));
        }

        private void ChartPanel_Click(object sender, MouseEventArgs e)
        {
            Control sendCtl = (Control)sender;
            sendCtl.Capture = false;

            int Lane = ConvertXCoordToNote(ChartPanel.PointToClient(MousePosition).X);

            int Tick = -1;
            if (sender == ChartPanel)
                Tick = (int)ConvertYCoordToTick(ChartPanel.PointToClient(MousePosition).Y);
            else if (sender == ChartPanel2)
                Tick = (int)ConvertYCoordToTick(ChartPanel2.PointToClient(MousePosition).Y) + PanelHeight / TickHeight;
            else if (sender == ChartPanel3)
                Tick = (int)ConvertYCoordToTick(ChartPanel3.PointToClient(MousePosition).Y) + 2 * PanelHeight / TickHeight;
            else if (sender == ChartPanel4)
                Tick = (int)ConvertYCoordToTick(ChartPanel4.PointToClient(MousePosition).Y) + 3 * PanelHeight / TickHeight;

            ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
        }


        private void ChangePanelHeight(int NewHeight)
        {
            ChartPanel.Height = NewHeight;
            ChartPanel2.Height = NewHeight;
            ChartPanel3.Height = NewHeight;
            ChartPanel4.Height = NewHeight;
            PanelHeight = NewHeight;
        }

        private void ZoomBtn_Click(object sender, EventArgs e)
        {
            TickHeight = (int)ZoomBox.Value;
            IconHeight = TickHeight;
            ResizeScrollbar();
            ChangePanelHeight(2304 * TickHeight);
            RedrawAllNoteIcons();
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
            float scaledivX = LaneWidth / 2; // these are the final pixel dimensions of each note in the image
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
                RedrawAllNoteIcons();
            }

            else if (NoteShiftBox.Value < 0)
            {
                List<Notedata.Tick> NewTicks = chart.Ticks.ToList();
                NewTicks.RemoveRange(0, - (int)NoteShiftBox.Value);
                NewTicks.AddRange(new Notedata.Tick[- (int)NoteShiftBox.Value]);
                chart.Ticks = NewTicks.ToArray();

                ResizeScrollbar();
                RedrawAllNoteIcons();
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
                            
                            PictureBox Icn = chart.Ticks[i].NoteIcons[j];
                            try { Icn.Parent.Controls.Remove(Icn); } catch { }
                            AddSingleNoteIcon(i, j, FindVisualNoteType(i, j));
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

                            PictureBox Icn = chart.Ticks[i].NoteIcons[j];
                            try { Icn.Parent.Controls.Remove(Icn); } catch { }
                            AddSingleNoteIcon(i, j, FindVisualNoteType(i, j));
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

                            PictureBox Icn = chart.Ticks[i].NoteIcons[j];
                            try { Icn.Parent.Controls.Remove(Icn); } catch { }
                            AddSingleNoteIcon(i, j, FindVisualNoteType(i, j));
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

                            PictureBox Icn = chart.Ticks[i].NoteIcons[j];
                            try { Icn.Parent.Controls.Remove(Icn); } catch { }
                            AddSingleNoteIcon(i, j, FindVisualNoteType(i, j));
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
            foreach (Control Ctrl in Controls)
                resources.ApplyResources(Ctrl, Ctrl.Name);
            resources.ApplyResources(this, "$this");
            ResumeLayout();

            AddNoteTypes();

            PositionPanel(CurrentTick);
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
