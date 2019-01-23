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
                if (tick >= Length) return NoteTypes.NoteTypeDefs.None;

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


            public Chart(int Length, double BPM)
            {
                Ticks = new Tick[Length];
                ChartLen = Length;
                this.BPM = BPM;
                UsedSwipes = null;
            }
        }


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

            public int[] Buttons
            {
                get
                {
                    return new int[] { SafeParseInt(BUTTON1), SafeParseInt(BUTTON2), SafeParseInt(BUTTON3), SafeParseInt(BUTTON4),
                        SafeParseInt(BUTTON5), SafeParseInt(BUTTON6), SafeParseInt(BUTTON7), SafeParseInt(BUTTON8) };
                }
            }
        }

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

            try { chart.BPM = int.Parse(tickObj[0].BAR) + int.Parse(tickObj[0].BEAT)/100f; } catch { };

            for (int i = 1; i < tickObj.Length; i++)
            {
                //int chartTick = tickObjTickNumber(tickObj, i);
                
                for (int j = 0; j < 8; j++)
                    chart.Ticks[tickObjTickNumber(tickObj, i)].Notes[j].NoteType = NoteTypes.NoteTypeDefs.gettypebyid(tickObj[i].Buttons[j]);
            }

            chart.FixSwipes();
            return chart;
        }

        public static String ConvertChartToJson(Chart chart)
        {
            var tickObj = new List<JsonTick_Export>();

            tickObj.Add(new JsonTick_Export { BAR = (int)chart.BPM, BEAT = (int)(chart.BPM % 1 / 100) });

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

            return JsonConvert.SerializeObject(tickObj).Replace("null", "\"\"").Replace(":0", ":\"\"").Replace("R\":\"\"", "R\":0").Replace("T\":\"\"", "T\":0");
        }

        public static String ConvertChartToJson_Small(Chart chart)
        {
            var tickObj = new List<JsonTick_Export>();

            tickObj.Add(new JsonTick_Export { BAR = (int)chart.BPM, BEAT = (int)(chart.BPM % 1 / 100) });

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

            return JsonConvert.SerializeObject(tickObj).Replace("null", "\"\"").Replace(":0", ":\"\"").Replace("R\":\"\"", "R\":0").Replace("T\":\"\"", "T\":0");
        }
    }
}
