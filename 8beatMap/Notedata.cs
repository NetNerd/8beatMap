using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace _8beatMap
{
    public static class Notedata
    {
        [Serializable]
        public struct Note
        {
            public NoteTypes.NoteTypeDef NoteType;

            public System.Drawing.Point SwipeEndPoint;

            public bool IsSwipeEnd;
        }

        [Serializable]
        public struct Tick
        {
            private Note[] NoteArray;
            
            public Note[] Notes
            {
                get
                {
                    if (NoteArray == null)
                    {
                        NoteArray = new Note[8];
                        for (int i = 0; i < 8; i++)
                            NoteArray[i].NoteType = NoteTypes.NoteTypeDefs.None;
                    }
                    return NoteArray;
                }
            }


            //using SetNote is preffered over directly setting notes because it handles fixing the swipe cache for you. Either way works though.
            public void SetNote(NoteTypes.NoteTypeDef Note, int Lane, ref Chart chart)
            {
                if (NoteArray == null)
                {
                    NoteArray = new Note[8];
                    for (int i = 0; i < 8; i++)
                        NoteArray[i].NoteType = NoteTypes.NoteTypeDefs.None;
                }
                NoteArray[Lane].NoteType = Note;

                chart.FixSwipes();
            }
        }


        public struct Chart
        {
            private string ChartName;
            public string SongName
            {
                get { return ChartName; }
                set { ChartName = value.Trim(); }
            }

            private string ChartAuthor;
            public string Author
            {
                get { return ChartAuthor; }
                set { ChartAuthor = value.Trim(); }
            }

            public Tick[] Ticks;

            private int ChartLen;
            public int Length
            {
                get { return ChartLen; }
                set
                {
                    if (value > 0)
                    {
                        Array.Resize(ref Ticks, value);
                        ChartLen = value;
                        FixSwipes();
                    }
                }
            }

            public int NoteCount
            {
                get
                {
                    int notes = 0;

                    for (int i = 0; i < Length; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypes.NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.NotNode != true & NoteType.DetectType != NoteTypes.DetectType.SwipeMid)
                                notes++;
                        }
                    }

                    return notes;
                }
            }

            public int AutoDifficultyScore
            {
                get
                {
                    int lastnotetick = 0;

                    for (int i = 0; i < Length; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            if (this.Ticks[i].Notes[j].NoteType.TypeId != NoteTypes.NoteTypeDefs.None.TypeId)
                            {
                                lastnotetick = i;
                            }
                        }
                    }

                    if (lastnotetick == 0)
                        return 0;
                    
                    float metric_notefreq = NoteCount/(float)lastnotetick; // just number of notes/ticks

                    int metric_notedistance = 0; // based on X, Y distance from previous notes
                    
                    int metric_numswipes = 0;
                    int metric_numflicks = 0;
                    int metric_numclocks = 0;

                    int lastnotestick = 0;
                    Note[] lastnotes = this.Ticks[0].Notes;
                    int lastnotescount = 0;

                    for (int i = 0; i < Length; i++) // processing loop
                    {
                        Note[] thisticknotes = this.Ticks[i].Notes;
                        int thisticknotescount = 0;

                        for (int j = 0; j < 8; j++)
                        {
                            if (thisticknotes[j].NoteType.NotNode == false && thisticknotes[j].NoteType.DetectType != NoteTypes.DetectType.SwipeMid)
                            {
                                thisticknotescount++;

                                if (thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.SwipeEndPoint && !thisticknotes[j].IsSwipeEnd)
                                {
                                    metric_numswipes += 1;
                                }
                                else if (thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.Flick || thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.GbsFlick)
                                {
                                    metric_numflicks += 1;
                                }
                                else if (thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.GbsClock)
                                {
                                    metric_numclocks += 1;
                                }
                            }
                        }

                        if (thisticknotescount == 1)
                        {
                            // a distance of two (maybe three) is easy to hit always, larger depends on previous tick also having only one note
                            // we should find the most optimal movement if there's multiple possibilities
                            for (int j = 0; j < 8; j++)
                            {
                                if (thisticknotes[j].NoteType.NotNode || thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.SwipeMid) continue;

                                int bestscore = 999;

                                for (int k = 0; k < 8; k++)
                                {
                                    if (lastnotes[k].NoteType.NotNode || thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.SwipeMid) continue;

                                    float newscore = Math.Abs(j - k); // get positive distance 
                                    if (newscore == 0) newscore = 2f; // override 0 distance to best value
                                    newscore = Math.Abs(newscore - 2f); // normalise to easiest notes
                                    newscore = newscore / ((i - lastnotestick)*0.1f * (i - lastnotestick)*0.1f); // divide by ((i - lastnotestick)*0.1)^2 to not penalize after greater time

                                    if (newscore < bestscore) bestscore = (int)newscore;
                                }

                                metric_notedistance += bestscore;
                            }
                        }
                        else if (thisticknotescount == 2)
                        {
                            // a distance of two (maybe three) is easy to hit always, larger depends on previous tick also having only one note
                            // we should find the shortest distance for each note to reflect that both hands should move
                            for (int j = 0; j < 8; j++)
                            {
                                if (thisticknotes[j].NoteType.NotNode || thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.SwipeMid) continue;
                                
                                int closestlane = 20;

                                for (int k = 0; k < 8; k++)
                                {
                                    if (lastnotes[k].NoteType.NotNode || thisticknotes[j].NoteType.DetectType == NoteTypes.DetectType.SwipeMid) continue;

                                    int dist = Math.Abs(j - k); // get positive distance 
                                    if (dist < Math.Abs(j - closestlane)) closestlane = k;
                                }

                                float newscore = Math.Abs(j - closestlane); // get positive distance 
                                if (newscore == 0) newscore = 2f; // override 0 distance to best value
                                newscore = Math.Abs(newscore - 2f); // normalise to easiest notes
                                newscore = newscore / ((i - lastnotestick) * 0.1f * (i - lastnotestick) * 0.1f); // divide by ((i - lastnotestick)*0.1)^2 to not penalize after greater time
                                
                                metric_notedistance += (int)newscore;
                            }
                        }
                        else if (thisticknotescount == 3)
                        {
                            metric_notedistance += 70; // hardest possible normally should be around 20...  so this seems fair
                        }
                        else if (thisticknotescount >= 4)
                        {
                            metric_notedistance += thisticknotescount*25; // this is a ridiculous case for 8bs...  so it's fair
                        }

                        if (thisticknotescount > 0)
                        {
                            lastnotestick = i;
                            lastnotes = thisticknotes;
                            lastnotescount = thisticknotescount;
                        }
                    }

                    double weighted_out = 0;
                    weighted_out += metric_notefreq * (BPM / 140) * 300; // factor in BPM because higher BPM is harder
                    weighted_out += metric_notedistance * Math.Sqrt(BPM / 140) / 2000;
                    weighted_out += metric_numflicks / 20;
                    weighted_out += metric_numclocks / 20;
                    weighted_out += metric_numswipes / 25;

                    return (int)(weighted_out * 0.28);
                }
            }

            public double BPM;



            private bool[,] UsedSwipes;
            private void UpdateSwipeEnd(int tick, int lane)
            {
                Note Note = Ticks[tick].Notes[lane];
                NoteTypes.NoteTypeDef Type = Note.NoteType;

                // The actual game appears to distinguish between left and right swipes only for note icon graphics.
                //     (proof is in the initial remember chart in res ver 250)
                // IIRC, I think it scans across all lanes and uses the first currently unused target node.
                // This could be changed to match the game by adding a new state to swipeEnds_internal.
                //     (probably should make it a struct to improve readability too)
                // However, this implementation works for all official charts currently (remember was fixed in ver 251), so for now it should be fine like this.
                if (Type.DetectDir == NoteTypes.DetectDir.Right && (Type.DetectType == NoteTypes.DetectType.SwipeEndPoint | Type.DetectType == NoteTypes.DetectType.SwipeMid | Type.DetectType == NoteTypes.DetectType.SwipeDirChange) & !Note.IsSwipeEnd)
                {
                    for (int i = tick + 1; i < tick + 48; i++)
                    {
                        if (i >= Length) break;
                        int j = lane + 1;
                        if (j > 7) break;

                        if (UsedSwipes[i, j])
                            continue;

                        NoteTypes.NoteTypeDef ijType = Ticks[i].Notes[j].NoteType;

                        if (ijType.DetectType == NoteTypes.DetectType.SwipeEndPoint & ijType.DetectDir == NoteTypes.DetectDir.Right)
                            Ticks[i].Notes[j].IsSwipeEnd = true;

                        if ((ijType.DetectDir == NoteTypes.DetectDir.Right & (ijType.DetectType == NoteTypes.DetectType.SwipeEndPoint | ijType.DetectType == NoteTypes.DetectType.SwipeMid)) || (ijType.DetectDir == NoteTypes.DetectDir.Left & ijType.DetectType == NoteTypes.DetectType.SwipeDirChange))
                        {
                            Ticks[tick].Notes[lane].SwipeEndPoint = new System.Drawing.Point(i, j);
                            UsedSwipes[i,j] = true;
                            break;
                        }
                    }
                }

                if (Type.DetectDir == NoteTypes.DetectDir.Left && (Type.DetectType == NoteTypes.DetectType.SwipeEndPoint | Type.DetectType == NoteTypes.DetectType.SwipeMid | Type.DetectType == NoteTypes.DetectType.SwipeDirChange) & !Note.IsSwipeEnd)
                {
                    for (int i = tick + 1; i < tick + 48; i++)
                    {
                        if (i >= Length) break;
                        int j = lane - 1;
                        if (j < 0) break;

                        if (UsedSwipes[i, j])
                            continue;

                        NoteTypes.NoteTypeDef ijType = Ticks[i].Notes[j].NoteType;

                        if (ijType.DetectType == NoteTypes.DetectType.SwipeEndPoint & ijType.DetectDir == NoteTypes.DetectDir.Left)
                            Ticks[i].Notes[j].IsSwipeEnd = true;

                        if ((ijType.DetectDir == NoteTypes.DetectDir.Left & (ijType.DetectType == NoteTypes.DetectType.SwipeEndPoint | ijType.DetectType == NoteTypes.DetectType.SwipeMid)) || (ijType.DetectDir == NoteTypes.DetectDir.Right & ijType.DetectType == NoteTypes.DetectType.SwipeDirChange))
                        {
                            Ticks[tick].Notes[lane].SwipeEndPoint = new System.Drawing.Point(i, j);
                            UsedSwipes[i, j] = true;
                            break;
                        }
                    }
                }
            }


            public void FixSwipes()
            {
                UsedSwipes = new bool[Length,8];
                for (int i = 0; i < Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Ticks[i].Notes[j].IsSwipeEnd = false;
                        Ticks[i].Notes[j].SwipeEndPoint = new System.Drawing.Point();
                    }
                }
                for (int i = 0; i < Length; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        UpdateSwipeEnd(i, j);
                    }
                }
                UsedSwipes = null;
            }


            public TimeSpan ConvertTicksToTime(double ticks)
            {
                TimeSpan a = TimeSpan.FromSeconds((5 * ticks / BPM));
                return TimeSpan.FromSeconds((5 * ticks / BPM));
            }

            public double ConvertTimeToTicks(TimeSpan time)
            {
                return time.TotalSeconds / (double)(5 / BPM);
            }


            public NoteTypes.NoteTypeDef FindVisualNoteType(int tick, int lane)
            {
                if (tick >= Length | tick < 0) return NoteTypes.NoteTypeDefs.None;

                if (Ticks[tick].Notes[lane].NoteType.DetectType == NoteTypes.DetectType.Hold)
                {
                    if (tick == 0 | tick == Length - 1) return Ticks[tick].Notes[lane].NoteType;
                    if ((Ticks[tick - 1].Notes[lane].NoteType.DetectType == NoteTypes.DetectType.Hold |
                        Ticks[tick - 1].Notes[lane].NoteType.DetectType == NoteTypes.DetectType.SwipeEndPoint) &&
                        Ticks[tick + 1].Notes[lane].NoteType.DetectType != NoteTypes.DetectType.None)
                        return NoteTypes.NoteTypeDefs.ExtendHoldMid;
                }
                return Ticks[tick].Notes[lane].NoteType;
            }


            public void AutoSetSimulNotes()
            {
                for (int i = 0; i < Length; i++)
                {
                    int SimulNum_Tap = 0;
                    int SimulNum_Hold = 0;

                    for (int j = 0; j < 8; j++)
                    {
                        // taps get drawn as simulnotes when swipes or flicks are present, but holds don't
                        NoteTypes.NoteTypeDef NoteType = FindVisualNoteType(i, j);
                        if (NoteType.NotNode != true & NoteType.DetectType != NoteTypes.DetectType.SwipeMid)
                            SimulNum_Tap++;

                        if (NoteType.DetectType == NoteTypes.DetectType.Tap | NoteType.DetectType == NoteTypes.DetectType.Hold | NoteType.DetectType == NoteTypes.DetectType.GbsClock)
                            SimulNum_Hold++;

                        if (NoteType.TypeId == NoteTypes.NoteTypeDefs.ExtendHoldMid.TypeId && Ticks[i].Notes[j].NoteType.TypeId != NoteTypes.NoteTypeDefs.Hold.TypeId)
                            Ticks[i].SetNote(NoteTypes.NoteTypeDefs.Hold, j, ref this);
                    }

                    if (SimulNum_Tap > 1)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypes.NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == NoteTypes.DetectType.Tap)
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.SimulTap, j, ref this);
                            }
                            else if (NoteType.DetectType == NoteTypes.DetectType.GbsFlick & NoteType.TypeName != "GbsHoldEndFlick")
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.GbsSimulFlick, j, ref this);
                            }
                            else if (NoteType.DetectType == NoteTypes.DetectType.GbsClock)
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.GbsSimulClock, j, ref this);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypes.NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == NoteTypes.DetectType.Tap)
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.Tap, j, ref this);
                            }
                            else if (NoteType.DetectType == NoteTypes.DetectType.GbsFlick & NoteType.TypeName != "GbsHoldEndFlick")
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.GbsFlick, j, ref this);
                            }
                            else if (NoteType.DetectType == NoteTypes.DetectType.GbsClock)
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.GbsClock, j, ref this);
                            }
                        }
                    }

                    if (SimulNum_Hold > 1)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypes.NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == NoteTypes.DetectType.Hold)
                            {
                                if (i + 1 < Length & Ticks[i + 1].Notes[j].NoteType.DetectType == NoteTypes.DetectType.Hold)
                                    Ticks[i].SetNote(NoteTypes.NoteTypeDefs.SimulHoldStart, j, ref this);
                                else
                                    Ticks[i].SetNote(NoteTypes.NoteTypeDefs.SimulHoldRelease, j, ref this);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypes.NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == NoteTypes.DetectType.Hold)
                            {
                                Ticks[i].SetNote(NoteTypes.NoteTypeDefs.Hold, j, ref this);
                            }
                        }
                    }
                }
            }

            public void ShiftAllNotes(int offset)
            {
                if (offset == 0) return;
                                
                else if (offset > 0)
                {
                    for (int i = Length - 1; i >= offset; i--)
                    {
                        Ticks[i] = Ticks[i - offset];
                    }
                    for (int i = 0; i < offset; i++)
                    {
                        Ticks[i] = new Tick();
                    }
                }
                
                else if (offset < 0)
                {
                    for (int i = 0; i < Length + offset; i++)
                    {
                        Ticks[i] = Ticks[i - offset];
                    }
                    for (int i = Length + offset; i < Length; i++)
                    {
                        Ticks[i] = new Tick();
                    }
                }

                FixSwipes();
            }


            public Chart(int Length, double BPM, string Name="", string Author="")
            {
                ChartName = Name.Trim();
                ChartAuthor = Author.Trim();
                Ticks = new Tick[Length];
                ChartLen = Length;
                this.BPM = BPM;
                UsedSwipes = null;
            }
        }

        #pragma warning disable 649 // disable warnings for unassigned variables -- these are actually used for JSON import
        class JsonTick_Import
        {
            public string BAR;
            public string BEAT;

            public string BUTTON1;
            public string BUTTON2;
            public string BUTTON3;
            public string BUTTON4;
            public string BUTTON5;
            public string BUTTON6;
            public string BUTTON7;
            public string BUTTON8;

            // short versions of button names for non-standard charts
            public string B1;
            public string B2;
            public string B3;
            public string B4;
            public string B5;
            public string B6;
            public string B7;
            public string B8;

            public int[] Buttons
            {
                get
                {
                    if (BUTTON1 != null)
                    {
                        return new int[] { SafeParseInt(BUTTON1), SafeParseInt(BUTTON2), SafeParseInt(BUTTON3), SafeParseInt(BUTTON4),
                            SafeParseInt(BUTTON5), SafeParseInt(BUTTON6), SafeParseInt(BUTTON7), SafeParseInt(BUTTON8) };
                    }
                    else
                    {
                        return new int[] { SafeParseInt(B1), SafeParseInt(B2), SafeParseInt(B3), SafeParseInt(B4),
                            SafeParseInt(B5), SafeParseInt(B6), SafeParseInt(B7), SafeParseInt(B8) };
                    }
                }
            }
        }
#pragma warning restore 649

        class JsonTick_Export
        {
            public int BAR;
            public int BEAT;

            public int BUTTON1;
            public int BUTTON2;
            public int BUTTON3;
            public int BUTTON4;
            public int BUTTON5;
            public int BUTTON6;
            public int BUTTON7;
            public int BUTTON8;
        }

        private static int tickObjTickNumber(JsonTick_Import[] tickObj, int Index)
        {
            return int.Parse(tickObj[Index].BAR) * 48 + int.Parse(tickObj[Index].BEAT);
        }

        private static int SafeParseInt(string str)
        {
            int i;
            int.TryParse(str, out i);
            return i;
        }

        public static Chart ConvertJsonToChart(string json)
        {
            var tickObj = JsonConvert.DeserializeObject<JsonTick_Import[]>(json);

            Chart chart = new Chart((int)Math.Ceiling((tickObjTickNumber(tickObj, tickObj.Length - 1)+1) / 48f) * 48, 1);   // forces length to a full bar

            bool gotBPM = false;
            try
            {
                double bpm = int.Parse(tickObj[0].BAR) + int.Parse(tickObj[0].BEAT) / 100f;
                if (bpm > 30)
                {
                    chart.BPM = bpm;
                    gotBPM = true;
                }
            }
            catch { };

            if (gotBPM && tickObj[0].BUTTON1 != null)
            {
                if(tickObj[0].BUTTON1.Length > 0) chart.SongName = tickObj[0].BUTTON1;
                if (tickObj[0].BUTTON2.Length > 0) chart.Author = tickObj[0].BUTTON2;
            }
            else if (gotBPM && tickObj[0].B1 != null)
            {
                if (tickObj[0].B1.Length > 0) chart.SongName = tickObj[0].B1;
                if (tickObj[0].B2.Length > 0) chart.Author = tickObj[0].B2;
            }

            if (!gotBPM) // there may be data in tick 0
            {
                try
                {
                    for (int j = 0; j < 8; j++)
                        chart.Ticks[tickObjTickNumber(tickObj, 0)].Notes[j].NoteType = NoteTypes.NoteTypeDefs.gettypebyid(tickObj[0].Buttons[j]);
                }
                catch { }
            }

            for (int i = 1; i < tickObj.Length; i++)
            {
                //int chartTick = tickObjTickNumber(tickObj, i);
                
                for (int j = 0; j < 8; j++)
                    chart.Ticks[tickObjTickNumber(tickObj, i)].Notes[j].NoteType = NoteTypes.NoteTypeDefs.gettypebyid(tickObj[i].Buttons[j]);
            }

            chart.FixSwipes();
            return chart;
        }


        private static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static String ConvertChartToJson(Chart chart)
        {
            var tickObj = new List<JsonTick_Export>();

            tickObj.Add(new JsonTick_Export { BAR = (int)chart.BPM, BEAT = (int)(chart.BPM % 1 * 100) });

            for (int i = 0; i < chart.Length; i++)
            {
                JsonTick_Export NewTick = new JsonTick_Export();
                NewTick.BAR = i / 48;
                NewTick.BEAT = i % 48;
                NewTick.BUTTON1 = chart.Ticks[i].Notes[0].NoteType.TypeId;
                NewTick.BUTTON2 = chart.Ticks[i].Notes[1].NoteType.TypeId;
                NewTick.BUTTON3 = chart.Ticks[i].Notes[2].NoteType.TypeId;
                NewTick.BUTTON4 = chart.Ticks[i].Notes[3].NoteType.TypeId;
                NewTick.BUTTON5 = chart.Ticks[i].Notes[4].NoteType.TypeId;
                NewTick.BUTTON6 = chart.Ticks[i].Notes[5].NoteType.TypeId;
                NewTick.BUTTON7 = chart.Ticks[i].Notes[6].NoteType.TypeId;
                NewTick.BUTTON8 = chart.Ticks[i].Notes[7].NoteType.TypeId;

                tickObj.Add(NewTick);
            }

            return JsonConvert.SerializeObject(tickObj).Replace("null", "\"\"").Replace(":0", ":\"\"")
                .Replace("R\":\"\"", "R\":0").Replace("T\":\"\"", "T\":0")
                .ReplaceFirst("\"BUTTON1\":\"\"", "\"BUTTON1\":\"" + chart.SongName + "\"").ReplaceFirst("\"BUTTON2\":\"\"", "\"BUTTON2\":\"" + chart.Author + "\"");
        }

        public static String ConvertChartToJson_Small(Chart chart)
        {
            var tickObj = new List<JsonTick_Export>();

            tickObj.Add(new JsonTick_Export { BAR = (int)chart.BPM, BEAT = (int)(chart.BPM % 1 * 100) });

            for (int i = 0; i < chart.Length; i++)
            {
                bool TickHasNote = false;
                for (int j = 0; j < 8; j++)
                {
                    if (chart.Ticks[i].Notes[j].NoteType.TypeId > 0) TickHasNote = true;
                }

                if (TickHasNote)
                {
                    JsonTick_Export NewTick = new JsonTick_Export();
                    NewTick.BAR = i / 48;
                    NewTick.BEAT = i % 48;
                    NewTick.BUTTON1 = chart.Ticks[i].Notes[0].NoteType.TypeId;
                    NewTick.BUTTON2 = chart.Ticks[i].Notes[1].NoteType.TypeId;
                    NewTick.BUTTON3 = chart.Ticks[i].Notes[2].NoteType.TypeId;
                    NewTick.BUTTON4 = chart.Ticks[i].Notes[3].NoteType.TypeId;
                    NewTick.BUTTON5 = chart.Ticks[i].Notes[4].NoteType.TypeId;
                    NewTick.BUTTON6 = chart.Ticks[i].Notes[5].NoteType.TypeId;
                    NewTick.BUTTON7 = chart.Ticks[i].Notes[6].NoteType.TypeId;
                    NewTick.BUTTON8 = chart.Ticks[i].Notes[7].NoteType.TypeId;

                    tickObj.Add(NewTick);
                }
            }

            return JsonConvert.SerializeObject(tickObj).Replace("null", "\"\"").Replace(":0", ":\"\"")
                .Replace("R\":\"\"", "R\":0").Replace("T\":\"\"", "T\":0")
                .ReplaceFirst("\"BUTTON1\":\"\"", "\"BUTTON1\":\"" + chart.SongName + "\"").ReplaceFirst("\"BUTTON2\":\"\"", "\"BUTTON2\":\"" + chart.Author + "\"");
        }
    }
}
