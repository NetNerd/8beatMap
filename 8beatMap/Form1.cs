﻿using System;
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
    public partial class Form1 : Form
    {

        Notedata.Chart chart = new Notedata.Chart(32 * 48, 120);
        private int TickHeight = 10;
        private int LaneWidth = 36;
        private int IconWidth = 20;
        private int IconHeight = 10;
        private int PanelHeight = 23040; // max is 2304 ticks per panel (total 9216) or 48 bars (total 192)
                                         // 192 bars is close to 6:30 at 120bpm, so it should be enough for most stuff
        private double CurrentTick = 0;

        private Timer playTimer = new Timer() { Interval = 10 };

        // PictureBox playHead = new PictureBox { Width = 288, Height = 4, BackColor = Color.DarkSlateGray };
        
        NAudio.Wave.WaveOutEvent WaveOut = new NAudio.Wave.WaveOutEvent();
        NAudio.Wave.MediaFoundationReader MusicFileReader;
        

        private void AddSingleNoteIcon(int Tick, int Lane, Notedata.NoteType Type)
        {
            Color noteCol = Color.Transparent;

            switch (Type)
            {
                case Notedata.NoteType.Tap: noteCol = Color.Blue; break;
                case Notedata.NoteType.Hold: noteCol = Color.LimeGreen; break;
                case Notedata.NoteType.SimulTap:
                case Notedata.NoteType.SimulHoldStart:
                case Notedata.NoteType.SimulHoldRelease: noteCol = Color.DeepPink; ; break;
                case Notedata.NoteType.FlickLeft:
                case Notedata.NoteType.HoldEndFlickLeft: noteCol = Color.FromArgb(0x80, 0, 0xa0); break;
                case Notedata.NoteType.SwipeLeftStartEnd: noteCol = Color.DarkViolet; break;
                case Notedata.NoteType.SwipeLeftMid: noteCol = Color.Violet; break;
                case Notedata.NoteType.FlickRight:
                case Notedata.NoteType.HoldEndFlickRight: noteCol = Color.FromArgb(0xcc, 0x88, 0); break;
                case Notedata.NoteType.SwipeRightStartEnd: noteCol = Color.DarkOrange; break;
                case Notedata.NoteType.SwipeRightMid: noteCol = Color.Gold; break;
                case Notedata.NoteType.ExtendHoldMid: noteCol = Color.LightGray; break;
            }
            int Top = PanelHeight - Tick * TickHeight - IconHeight;
            PictureBox NoteBox = new PictureBox { Left = Lane * LaneWidth + (LaneWidth - IconWidth) / 2, Top = Top, Width = IconWidth, Height = IconHeight, BackColor = noteCol };

            if (Type == Notedata.NoteType.HoldEndFlickLeft || Type == Notedata.NoteType.HoldEndFlickRight)
                NoteBox.BorderStyle = BorderStyle.Fixed3D;

            NoteBox.Click += new System.EventHandler(NoteBox_Click);


            if (Top >= 0)
                ChartPanel.Controls.Add(NoteBox);
            else if (Top >= 0 - PanelHeight)
            {
                NoteBox.Top += PanelHeight;
                ChartPanel2.Controls.Add(NoteBox);
            }
            else if (Top >= 0 - PanelHeight * 2)
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
                Color noteCol = Color.Transparent;

                switch (notedata.Type)
                {
                    case Notedata.NoteType.Tap: noteCol = Color.Blue; break;
                    case Notedata.NoteType.Hold: noteCol = Color.LimeGreen; break;
                    case Notedata.NoteType.SimulTap:
                    case Notedata.NoteType.SimulHoldStart:
                    case Notedata.NoteType.SimulHoldRelease: noteCol = Color.DeepPink; ; break;
                    case Notedata.NoteType.FlickLeft:
                    case Notedata.NoteType.HoldEndFlickLeft: noteCol = Color.FromArgb(0x80, 0, 0xa0); break;
                    case Notedata.NoteType.SwipeLeftStartEnd: noteCol = Color.DarkViolet; break;
                    case Notedata.NoteType.SwipeLeftMid: noteCol = Color.Violet; break;
                    case Notedata.NoteType.FlickRight:
                    case Notedata.NoteType.HoldEndFlickRight: noteCol = Color.FromArgb(0xcc, 0x88, 0); break;
                    case Notedata.NoteType.SwipeRightStartEnd: noteCol = Color.DarkOrange; break;
                    case Notedata.NoteType.SwipeRightMid: noteCol = Color.Gold; break;
                    case Notedata.NoteType.ExtendHoldMid: noteCol = Color.LightGray; break;
                }
                int Top = PanelHeight - notedata.Tick * TickHeight - IconHeight;
                PictureBox NoteBox = new PictureBox { Left = notedata.Lane * LaneWidth + (LaneWidth - IconWidth) / 2, Top = Top, Width = IconWidth, Height = IconHeight, BackColor = noteCol };

                if (notedata.Type == Notedata.NoteType.HoldEndFlickLeft || notedata.Type == Notedata.NoteType.HoldEndFlickRight)
                    NoteBox.BorderStyle = BorderStyle.Fixed3D;

                NoteBox.Click += new System.EventHandler(NoteBox_Click);

                if (Top >= 0)
                    icons.Add(NoteBox);
                else if (Top >= 0 - PanelHeight)
                {
                    NoteBox.Top += PanelHeight;
                    icons2.Add(NoteBox);
                }
                else if (Top >= 0 - PanelHeight * 2)
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

        private Notedata.NoteType FindHoldType(int tick, int note)
        {
            if (tick == 0) return Notedata.NoteType.Hold;
            if ((chart.Ticks[tick - 1].Notes[note] == Notedata.NoteType.Hold ||
                chart.Ticks[tick - 1].Notes[note] == Notedata.NoteType.SimulHoldStart ||
                chart.Ticks[tick - 1].Notes[note] == Notedata.NoteType.SimulHoldRelease ||
                chart.Ticks[tick - 1].Notes[note] == Notedata.NoteType.SwipeLeftStartEnd ||
                chart.Ticks[tick - 1].Notes[note] == Notedata.NoteType.SwipeRightStartEnd) &&
                chart.Ticks[tick + 1].Notes[note] != Notedata.NoteType.None) return Notedata.NoteType.ExtendHoldMid;
            else return chart.Ticks[tick].Notes[note];
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
                        if (chart.Ticks[i].Notes[j] == Notedata.NoteType.Hold || chart.Ticks[i].Notes[j] == Notedata.NoteType.SimulHoldRelease)
                        {
                            Notes.Add(new NoteDataInfo(Array.IndexOf(chart.Ticks, chart.Ticks[i]), j, FindHoldType(i, j)));
                        }
                        else Notes.Add(new NoteDataInfo(Array.IndexOf(chart.Ticks, chart.Ticks[i]), j, chart.Ticks[i].Notes[j]));
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

        private int ConvertTimeToTicks(TimeSpan time)
        {
            return (int)(time.TotalSeconds / (double)(5/BPMbox.Value));
        }


        private void ResizeScrollbar()
        {
            ChartScrollBar.Minimum = 0;
            ChartScrollBar.Maximum = (int)(1.015 * chart.Length * TickHeight + IconHeight / 2);
        }

        private void PositionPanel(double tick)
        {
            if (tick < 0) tick = 0;
            if (tick > chart.Length) tick = chart.Length;

            CurrentTick = tick;
            // playHead.Left = 0;
            // playHead.Top = chart.Length * TickHeight - tick * TickHeight + 2;
            
            ChartScrollBar.Value = (int)(chart.Length * TickHeight - tick * TickHeight);

            int p1Top = this.ClientSize.Height - PanelHeight + (int)tick * TickHeight - IconHeight / 2 - 2;
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
                catch { MessageBox.Show("Unable to load chart."); }

                if (chart.BPM == 1)
                {
                    MessageBox.Show("Unable to detect BPM (charts from the game don't contain this info). \n Set it in the BPM box for notes to move at the right speed.");
                    chart.BPM = 120;
                }
                ResizeBox.Value = chart.Length / 48;
                CurrentTick = 0;
                ResizeScrollbar();
                RedrawAllNoteIcons();

                BPMbox.Value = (decimal)chart.BPM;
            }
        }


        private void LoadMusic(string Path)
        {
            if (Path.Length > 0)
            {
                WaveOut.Stop();

                try {
                    MusicFileReader = new NAudio.Wave.MediaFoundationReader(Path);
                }
                catch { MessageBox.Show("Unable to load music file."); }

                WaveOut.DesiredLatency = 100;
                    WaveOut.NumberOfBuffers = 8;
                    WaveOut.Init(MusicFileReader);
            }
        }


        public Form1()
        {
            InitializeComponent();

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
            MusicFileReader.CurrentTime = ConvertTicksToTime(CurrentTick);
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            if (MusicFileReader != null)
                StartPlayback();
            else
            {
                MessageBox.Show("A music file must be loaded for playback to work.");
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            StopPlayback();
        }


        private void playtimer_Tick(object sender, EventArgs e)
        {
            // PositionPlayhead(CurrentTick + 1);
            PositionPanel(ConvertTimeToTicks(MusicFileReader.CurrentTime));
        }

        private void BPMbox_ValueChanged(object sender, EventArgs e)
        {
            // playTimer.Interval = (int)ConvertTicksToTime(1).TotalMilliseconds;
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

        private void NoteBox_Click(object sender, EventArgs e)
        {
            int Lane = ConvertXCoordToNote(ChartPanel.PointToClient(MousePosition).X);

            Control sendCtl = (Control)sender;

            int Tick;
            if (sendCtl.Parent == ChartPanel)
                Tick = (int)ConvertYCoordToTick(ChartPanel.PointToClient(MousePosition).Y);
            else if (sendCtl.Parent == ChartPanel2)
                Tick = (int)ConvertYCoordToTick(ChartPanel2.PointToClient(MousePosition).Y) + PanelHeight / TickHeight;
            else if(sendCtl.Parent == ChartPanel3)
                Tick = (int)ConvertYCoordToTick(ChartPanel3.PointToClient(MousePosition).Y) + 2 * PanelHeight / TickHeight;
            else
                Tick = (int)ConvertYCoordToTick(ChartPanel4.PointToClient(MousePosition).Y) + 3 * PanelHeight / TickHeight;

            Console.WriteLine(Lane + ", " + Tick);
        }

        private void ChartPanel_Click(object sender, EventArgs e)
        {
            int Lane = ConvertXCoordToNote(ChartPanel.PointToClient(MousePosition).X);

            int Tick;
            if (sender == ChartPanel)
                Tick = (int)ConvertYCoordToTick(ChartPanel.PointToClient(MousePosition).Y);
            else if (sender == ChartPanel2)
                Tick = (int)ConvertYCoordToTick(ChartPanel2.PointToClient(MousePosition).Y) + PanelHeight / TickHeight;
            else if (sender == ChartPanel3)
                Tick = (int)ConvertYCoordToTick(ChartPanel3.PointToClient(MousePosition).Y) + 2 * PanelHeight / TickHeight;
            else
                Tick = (int)ConvertYCoordToTick(ChartPanel4.PointToClient(MousePosition).Y) + 3 * PanelHeight / TickHeight;

            Console.WriteLine(Lane + ", " + Tick);
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
            ChangePanelHeight(2304 * TickHeight);
            RedrawAllNoteIcons();
        }

        private void ResizeBtn_Click(object sender, EventArgs e)
        {
            ResizeChart((int)ResizeBox.Value * 48);
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            LoadChart(openFileDialog1.FileName);
        }

        private void SaveChartBtn_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            System.IO.File.WriteAllText(saveFileDialog1.FileName, Notedata.ConvertChartToJson(chart));
        }

        private void OpenMusicButton_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            LoadMusic(openFileDialog2.FileName);
        }

        private void ImgSaveBtn_Click(object sender, EventArgs e)
        {
            float scaledivX = LaneWidth / 2;
            float scaledivY = TickHeight / 1;
            Bitmap img = new Bitmap((int)(LaneWidth * 8/scaledivX), (int)(chart.Length * TickHeight / scaledivY));
            Graphics grfx = Graphics.FromImage(img);
            Bitmap tmpimg = new Bitmap(LaneWidth * 8, PanelHeight);

            ChartPanel.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, 0, img.Height - PanelHeight / scaledivY, LaneWidth * 8/scaledivX, PanelHeight / scaledivY);

            ChartPanel2.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, 0, img.Height - PanelHeight * 2/scaledivY, LaneWidth * 8/scaledivX, PanelHeight / scaledivY);

            ChartPanel3.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, 0, img.Height - PanelHeight * 3/scaledivY, LaneWidth * 8/scaledivX, PanelHeight / scaledivY);

            ChartPanel4.DrawToBitmap(tmpimg, new Rectangle(0, 0, LaneWidth * 8, PanelHeight));
            grfx.DrawImage(tmpimg, 0, img.Height - PanelHeight * 4/scaledivY, LaneWidth * 8/scaledivX, PanelHeight / scaledivY);

            img.Save("imgout.png");

            tmpimg.Dispose();
            grfx.Dispose();
            img.Dispose();
        }
    }
}
