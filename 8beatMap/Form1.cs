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

        public Notedata.Chart chart = new Notedata.Chart(32 * 48, 120);
        private int TickHeight = 10;
        private int IconWidth = 20;
        private int IconHeight = 10;
        private double CurrentTick = 0;
        private int LastTick = 0;


        private Timer playTimer = new Timer() { Interval = 8 };


        private double[] prevPlayTicks = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        
        private double getAveragedPlayTickTime(double rawtick)
        {
            double avgTickDelta = 0;
            int numentries = 0;
            for (int i = 1; i < prevPlayTicks.Length; i++)
            {
                if (prevPlayTicks[i] >= 0)
                {
                    avgTickDelta += prevPlayTicks[i - 1] - prevPlayTicks[i];
                    numentries++;
                }
            }
            if (numentries > 0)
                avgTickDelta /= numentries;
            else
                avgTickDelta = 0;

            double averagedTick = prevPlayTicks[0] + avgTickDelta;

            if (Math.Abs(rawtick - averagedTick) > 5 | !playTimer.Enabled) // averaged tick is too different to raw tick or playtimer isn't enabled -- reset all to default state
            {
                prevPlayTicks[0] = rawtick;
                for (int i = 1; i < prevPlayTicks.Length; i++)
                {
                    prevPlayTicks[i] = -1;
                }
            }
            else
            {
                for (int i = prevPlayTicks.Length - 1; i > 0; --i)
                {
                    prevPlayTicks[i] = prevPlayTicks[i - 1];
                }
                if (numentries > 0)
                    prevPlayTicks[0] = (averagedTick*2 + rawtick) / 3; // add some portion of rawtick to help avoid drifting
                else
                    prevPlayTicks[0] = rawtick; // if there was no valid average made, it's necessary to just use the raw value provided
            }

            return prevPlayTicks[0];
        }


        GameCloneRenderer_OGL OGLrenderer = new GameCloneRenderer_OGL(853, 480, Skinning.DefaultSkin);

        public bool ShowTypeIdsOnNotes = false;


        Image GetChartImage(double startTick, int tickHeight, int iconWidth, int iconHeight, Color BgCol, bool NoGrid, int Width, int Height)
        {
            Image Bmp = new Bitmap(Width, Height);
            Graphics Grfx = Graphics.FromImage(Bmp);

            int width = Bmp.Width;
            int height = Bmp.Height;

            Font Arial65Font = new Font("Arial", 6.5f);

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
            int halfIconWidth = iconWidth / 2;
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
                    NoteTypes.NoteTypeDef Type = chart.FindVisualNoteType(i, j);

                    if (!chart.Ticks[i].Notes[j].IsSwipeEnd)
                    {
                        Point swipeEndPoint = chart.Ticks[i].Notes[j].SwipeEndPoint;

                        if (swipeEndPoint.X > i)
                            Grfx.DrawLine(new Pen(Color.LightGray, iconWidth / 3), (float)(j + 0.5) * laneWidth, height - (float)(i - startTick + 1) * tickHeight - 2, (float)(swipeEndPoint.Y + 0.5) * laneWidth, height - (float)(swipeEndPoint.X - startTick + 1) * tickHeight - 2);
                            
                    }


                    int iconX = (int)((j + 0.5) * laneWidth - halfIconWidth);
                    int iconY = (int)Math.Ceiling(height - (i - startTick + 1.5) * tickHeight - 2);

                    Color backColor = skin.EditorColours[Type.TypeName][0];
                    Color iconColor = skin.EditorColours[Type.TypeName][1];

                    if (backColor.A > 0)
                        Grfx.FillRectangle(new SolidBrush(backColor), iconX, iconY, iconWidth, iconHeight);

                    if (iconColor.A > 0)
                    {
                        if (Type.IconType == NoteTypes.IconType.LeftArrow)
                            Grfx.FillPolygon(new SolidBrush(iconColor), new Point[] { new Point(iconX + iconWidth - 1, iconY + 0), new Point(iconX + iconWidth - 1, iconY + iconHeight - 1), new Point(iconX + 0, iconY + halfIconHeight) });
                        else if (Type.IconType == NoteTypes.IconType.RightArrow)
                            Grfx.FillPolygon(new SolidBrush(iconColor), new Point[] { new Point(iconX + 0, iconY + 0), new Point(iconX + 0, iconY + iconHeight - 1), new Point(iconX + iconWidth - 1, iconY + halfIconHeight) });
                        else if (Type.IconType == NoteTypes.IconType.UpArrow)
                            Grfx.FillPolygon(new SolidBrush(iconColor), new Point[] { new Point(iconX + halfIconWidth, iconY + 0), new Point(iconX + iconWidth - 1, iconY + iconHeight - 1), new Point(iconX + 0, iconY + iconHeight - 1) });
                        else if (Type.IconType == NoteTypes.IconType.HalfSplit)
                            Grfx.FillPolygon(new SolidBrush(iconColor), new Point[] { new Point(iconX + iconWidth - 1, iconY + 0), new Point(iconX + iconWidth - 1, iconY + iconHeight - 1), new Point(iconX + 0, iconY + iconHeight - 1) });
                    }

                    if (ShowTypeIdsOnNotes)
                    {
                        int typeId = chart.Ticks[i].Notes[j].NoteType.TypeId;
                        if (typeId != 0)
                        {
                            string typeStr = typeId.ToString();
                            Grfx.DrawString(typeStr, Arial65Font, Brushes.White, iconX + halfIconWidth - typeStr.Length * 3.5f, iconY);
                        }
                    }
                }

                if (!NoGrid)
                {
                    if (i % 48 == 0)
                    {
                        Grfx.FillRectangle(Brushes.SlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 3, width, 3);
                        Grfx.DrawString((i / 48 + 1).ToString(), Arial65Font, Brushes.DarkSlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 13);
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

            CurrentTick = getAveragedPlayTickTime(tick);

            TimeSpan ctickTime = chart.ConvertTicksToTime(tick);

            if (Sound.MusicReader != null &&
                    (Sound.MusicReader.CurrentTime < ctickTime - TimeSpan.FromMilliseconds(MusicDelayMs + 10) |
                    Sound.MusicReader.CurrentTime > ctickTime - TimeSpan.FromMilliseconds(MusicDelayMs - 10)))
                try {
                    if (ctickTime < TimeSpan.FromMilliseconds(MusicDelayMs))
                        Sound.MusicReader.CurrentTime = TimeSpan.FromMilliseconds(0);
                    else
                        Sound.MusicReader.CurrentTime = ctickTime - TimeSpan.FromMilliseconds(MusicDelayMs);
                }
                catch
                { }

            ChartScrollBar.Value = (int)(chart.Length * TickHeight - tick * TickHeight);
        }



        int VideoDelayMs = 90;

        public void UpdateChart()
        {
            double tick = CurrentTick;
            if (playTimer.Enabled)
            {
                tick -= chart.ConvertTimeToTicks(TimeSpan.FromMilliseconds(VideoDelayMs));
            }

            pictureBox1.Image.Dispose();
            pictureBox1.Image = GetChartImage(tick, TickHeight, IconWidth, IconHeight, SystemColors.ControlLight, false, pictureBox1.Width, pictureBox1.Height);
        }

        public void UpdateGameCloneChart()
        {
            double tick = CurrentTick;
            if (playTimer.Enabled)
            {
                tick -= chart.ConvertTimeToTicks(TimeSpan.FromMilliseconds(VideoDelayMs));
            }

            OGLrenderer.currentTick = tick;
            OGLrenderer.numTicksVisible = (int)chart.ConvertTimeToTicks(TimeSpan.FromMilliseconds(700));
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


        private void AddNoteTypes() // to dropdown selector
        {
            NoteTypeSelector.Items.Clear();

            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                //NoteTypeSelector.DataSource = Enum.GetValues(typeof(Notedata.UserVisibleNoteType_Nihongo));
                foreach (KeyValuePair<string, int> type in NoteTypes.UserVisibleNoteTypes_Nihongo)
                {
                    NoteTypeSelector.Items.Add(type);
                }
            else
                // NoteTypeSelector.DataSource = Enum.GetValues(typeof(Notedata.UserVisibleNoteType));
                foreach (KeyValuePair<string, int> type in NoteTypes.UserVisibleNoteTypes)
                {
                    NoteTypeSelector.Items.Add(type);
                }

            NoteTypeSelector.SelectedIndex = 0;
        }


        private void LoadSounds()
        {
            try
            {
                Sound.NoteSoundWave = new Sound.CachedSound(skin.SoundPaths["hit"]);
                Sound.NoteSoundWave_Swipe = new Sound.CachedSound(skin.SoundPaths["swipe"]);
                //NoteSoundMixer.AddMixerInput(NoteSoundWave);
                //NoteSoundMixer.AddMixerInput(NoteSoundWave_Swipe);
                //Sound.SetNoteSoundLatency(95);
            }
            catch
            {
                Sound.NoteSoundWave = null;
                Sound.NoteSoundWave_Swipe = null;
            }
        }

        private void OpenPreviewWindow()
        {
            OGLrenderer.Stop();
            OGLrenderer = null;
            OGLrenderer = new GameCloneRenderer_OGL(853, 480, skin);
            OGLrenderer.mainform = this;
        }

        private Skinning.Skin skin = Skinning.DefaultSkin;
        private void SetSkin(string skin)
        {
            this.skin = Skinning.LoadSkin(skin);
            UpdateChart();
            LoadSounds();
            OpenPreviewWindow();
        }


        public Form1()
        {
            InitializeComponent();
            
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            OGLrenderer.mainform = this;

            SetSkin("skin_8bs");

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


        int MusicDelayMs = 10;
        
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
                            NoteTypes.NoteTypeDef note = chart.FindVisualNoteType(i, j);

                            if (Sound.NoteSoundWave != null)
                            {
                                if (note.DetectType == NoteTypes.DetectType.Tap | note.DetectType == NoteTypes.DetectType.Hold | note.DetectType == NoteTypes.DetectType.GbsClock)
                                {
                                    //Sound.PlayNoteSound(Sound.NoteSoundWave);
                                    Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(new Sound.CachedSoundSampleProvider(Sound.NoteSoundWave));
                                    if (MusicDelayMs > 30)
                                        Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs - 30);
                                    else
                                        Sound.NoteSoundTrim.SkipOver = TimeSpan.FromMilliseconds(30 - MusicDelayMs);
                                    Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                }

                                else if ((note.DetectType == NoteTypes.DetectType.SwipeEndPoint & !chart.Ticks[i].Notes[j].IsSwipeEnd) ||
                                         note.DetectType == NoteTypes.DetectType.SwipeDirChange || note.DetectType == NoteTypes.DetectType.Flick || note.DetectType == NoteTypes.DetectType.GbsFlick)
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

                            else if (note.NotNode != true & note.DetectType != NoteTypes.DetectType.SwipeMid)
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

            if (CurrentTick >= chart.Length - 1 || Sound.MusicReader.CurrentTime == Sound.MusicReader.TotalTime)
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

        private void ProcessClick(int Tick, int Lane, MouseButtons MouseButton, NoteTypes.NoteTypeDef NewNote)
        {
            //Console.WriteLine(Lane + ", " + Tick);

            if (Tick == -1 | Tick >= chart.Length)
                return;

            if (Lane < 0 | Lane > 7)
                return;

            if (MouseButton == MouseButtons.Left)
            {
                if (chart.Ticks[Tick].Notes[Lane].NoteType.TypeId != NewNote.TypeId)
                {
                    if (NewNote.TypeId == NoteTypes.NoteTypeDefs.None.TypeId)
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
                if (chart.Ticks[Tick].Notes[Lane].NoteType.TypeId != NoteTypes.NoteTypeDefs.None.TypeId)
                {
                    chart.Ticks[Tick].SetNote(NoteTypes.NoteTypeDefs.None, Lane, ref chart);
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

            ProcessClick(Tick, Lane, e.Button, NoteTypes.NoteTypeDefs.gettypebyid(((KeyValuePair<string, int>)NoteTypeSelector.SelectedItem).Value));
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left | e.Button == MouseButtons.Right)
            {
                int Lane = ConvertXCoordToNote(e.X);
                int Tick = (int)ConvertYCoordToTick(e.Y);

                ProcessClick(Tick, Lane, e.Button, NoteTypes.NoteTypeDefs.gettypebyid(((KeyValuePair<string, int>)NoteTypeSelector.SelectedItem).Value));
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
            if (NoteShiftBox.Value == 0) return;

            chart.ShiftAllNotes((int)NoteShiftBox.Value);

            ResizeScrollbar();
            UpdateChart();

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
            MessageBox.Show(String.Format(DialogResMgr.GetString("NoteCountMessage"), chart.NoteCount));
        }

        private void AutoSimulBtn_Click(object sender, EventArgs e)
        {
            chart.AutoSetSimulNotes();
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
        
        private static bool IsDeSerializable(object obj)
            {
                System.IO.MemoryStream mem = new System.IO.MemoryStream();
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bin = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                try
                {
                    bin.Serialize(mem, obj);
                    mem.Seek(0, System.IO.SeekOrigin.Begin);
                    bin.Deserialize(mem);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Your object cannot be deserialized." +
                                     " The reason is: " + ex.ToString());
                    return false;
                }
            }
    private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char key = e.KeyChar;

                if (ModifierKeys == Keys.Control)
                {
                    if ((key == 3 | key == 'c' | key == 'C')) // not sure why this should be 3.....
                    {
                        int copylen = (int)(48 * CopyLengthBox.Value);
                        Notedata.Tick[] copydata = new Notedata.Tick[copylen];

                        for (int i = 0; i < copylen; i++)
                        {
                            copydata[i] = chart.Ticks[(int)CurrentTick + i];
                        }
                        
                        Clipboard.Clear();
                        Clipboard.SetDataObject(new DataObject(copydata));
                    }
                    else if ((key == 22 | key == 'v' | key == 'V'))
                    {
                        Type datatype = typeof(Notedata.Tick[]);
                        IDataObject dataobject = Clipboard.GetDataObject();
                        if (dataobject.GetDataPresent(datatype))
                        {
                            Notedata.Tick[] pastedata = (Notedata.Tick[])dataobject.GetData(datatype);

                            for (int i = 0; i < pastedata.Length; i++)
                            {
                                chart.Ticks[(int)CurrentTick + i] = pastedata[i];
                            }
                        }

                    }
                }
                else switch (key)
                {
                    case '/': // toggle showing numbers on keys
                        ShowTypeIdsOnNotes = !ShowTypeIdsOnNotes;
                        UpdateChart();
                        break;

                    case 'P': // reopen preview window
                    case 'p':
                        // OpenPreviewWindow();
                        if (skin.SkinName == "skin_8bs")
                        {
                            SetSkin("skin_gbssolid");
                        }
                        else
                        {
                            SetSkin("skin_8bs");
                        }
                        break;
                    
                    case '[':
                        OGLrenderer.clearColor = Color.FromArgb(0, 0, 0, 0);
                        break;
                    case ']':
                        OGLrenderer.clearColor = Color.FromArgb(0, 170, 170, 170);
                        break;
                    case '\\':
                    case '¥': // probably not necessary...
                        OGLrenderer.clearColor = Color.FromArgb(0, 255, 255, 255);
                        break;

                    default:
                        if (Char.IsDigit(key))
                        {
                            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                                NoteTypeSelector.SelectedItem = NoteTypes.UserVisibleNoteTypes_Nihongo.FirstOrDefault(x => x.Value == NoteTypes.NoteShortcutKeys["_" + key]);
                            else
                                NoteTypeSelector.SelectedItem = NoteTypes.UserVisibleNoteTypes.FirstOrDefault(x => x.Value == NoteTypes.NoteShortcutKeys["_" + key]);

                        }
                        else
                        {
                            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                                NoteTypeSelector.SelectedItem = NoteTypes.UserVisibleNoteTypes_Nihongo.FirstOrDefault(x => x.Value == NoteTypes.NoteShortcutKeys[key.ToString().ToUpper()]);
                            else
                                NoteTypeSelector.SelectedItem = NoteTypes.UserVisibleNoteTypes.FirstOrDefault(x => x.Value == NoteTypes.NoteShortcutKeys[key.ToString().ToUpper()]);
                        }
                        break;
                }
            }
            catch { }
        }

        private void PreviewWndBtn_Click(object sender, EventArgs e)
        {
            // OpenPreviewWindow();
            if (skin.SkinName == "skin_8bs")
            {
                SetSkin("skin_gbssolid");
            }
            else
            {
                SetSkin("skin_8bs");
            }
        }
    }
}
