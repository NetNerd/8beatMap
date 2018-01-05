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
        public enum NoteType
        {
            None = 0,
            Tap = 1,
            Hold = 2,
            SimulTap = 3,
            SwipeRightStartEnd = 4,
            SwipeRightMid = 5,
            SwipeLeftStartEnd = 6,
            SwipeLeftMid = 7,
            SimulHoldRelease = 8,
            SimulHoldStart = 9,
            HoldEndFlickRight = 10,
            HoldEndFlickLeft = 11,
            FlickRight = 12,
            FlickLeft = 13,
            SwipeChangeDirR2L = 14,
            SwipeChangeDirL2R = 15,

            ExtendHoldMid = 20
        }

        public enum UserVisibleNoteType
        {
            Tap = NoteType.Tap,
            SimulTap = NoteType.SimulTap,

            Hold = NoteType.Hold,
            SimulHoldStart = NoteType.SimulHoldStart,
            SimulHoldRelease = NoteType.SimulHoldRelease,

            SwipeLeftStartEnd = NoteType.SwipeLeftStartEnd,
            SwipeLeftMid = NoteType.SwipeLeftMid,

            SwipeRightStartEnd = NoteType.SwipeRightStartEnd,
            SwipeRightMid = NoteType.SwipeRightMid,

            SwipeChangeDirL2R = NoteType.SwipeChangeDirL2R,
            SwipeChangeDirR2L = NoteType.SwipeChangeDirR2L,

            FlickLeft = NoteType.FlickLeft,
            HoldEndFlickLeft = NoteType.HoldEndFlickLeft,

            FlickRight = NoteType.FlickRight,
            HoldEndFlickRight = NoteType.HoldEndFlickRight
        }

        public enum UserVisibleNoteType_Nihongo
        {
            押す = NoteType.Tap,
            同時に押す = NoteType.SimulTap,

            長い押す = NoteType.Hold,
            同時に長い押すの最初 = NoteType.SimulHoldStart,
            同時に長い押すの最後 = NoteType.SimulHoldRelease,

            左へスワイプの端 = NoteType.SwipeLeftStartEnd,
            左へスワイプの真ん中で = NoteType.SwipeLeftMid,

            右へスワイプの端 = NoteType.SwipeRightStartEnd,
            右へスワイプの真ん中で = NoteType.SwipeRightMid,

            左へスワイプから右に変わる = NoteType.SwipeChangeDirL2R,
            右へスワイプから左に変わる = NoteType.SwipeChangeDirR2L,

            左へフリック = NoteType.FlickLeft,
            長い押すの最後での左へフリック = NoteType.HoldEndFlickLeft,

            右へフリック = NoteType.FlickRight,
            長い押すの最後での右へフリック = NoteType.HoldEndFlickRight
        }

        public enum NoteShortcutKeys
        {
            _1 = NoteType.Tap,
            Q = NoteType.SimulTap,

            _2 = NoteType.Hold,
            W = NoteType.SimulHoldStart,
            S = NoteType.SimulHoldRelease,

            _3 = NoteType.FlickLeft,
            E = NoteType.HoldEndFlickLeft,
            _4 = NoteType.FlickRight,
            R = NoteType.HoldEndFlickRight,

            _5 = NoteType.SwipeLeftStartEnd,
            T = NoteType.SwipeLeftMid,
            G = NoteType.SwipeChangeDirR2L,
            _6 = NoteType.SwipeRightStartEnd,
            Y = NoteType.SwipeRightMid,
            H = NoteType.SwipeChangeDirL2R
        }


        public struct Note
        {
            public NoteTypeDef NoteType;

            public System.Drawing.Point SwipeEndPoint;

            public bool IsSwipeEnd;
        }


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
                    }
                    return NoteArray;
                }
            }           


            //using SetNote is preffered over directly setting notes because it handles fixing the swipe cache for you. Either way works though.
            public void SetNote(NoteTypeDef Note, int Lane, ref Chart chart)
            {
                if (NoteArray == null)
                {
                    NoteArray = new Note[8];
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
                            NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.NotNode != true & NoteType.DetectType != DetectType.SwipeMid)
                                notes++;
                        }
                    }

                    return notes;
                }
            }

            public double BPM;

            


            private void UpdateSwipeEnd(int tick, int lane)
            {
                Note Note = Ticks[tick].Notes[lane];
                NoteTypeDef Type = Note.NoteType;

                // The actual game appears to distinguish between left and right swipes only for note icon graphics.
                //     (proof is in the initial remember chart in res ver 250)
                // IIRC, I think it scans across all lanes and uses the first currently unused target node.
                // This could be changed to match the game by adding a new state to swipeEnds_internal.
                //     (probably should make it a struct to improve readability too)
                // However, this implementation works for all official charts currently (remember was fixed in ver 251), so for now it should be fine like this.
                if (Type.DetectDir == DetectDir.Right && (Type.DetectType == DetectType.SwipeEndPoint | Type.DetectType == DetectType.SwipeMid | Type.DetectType == DetectType.SwipeDirChange) & !Note.IsSwipeEnd)
                {
                    for (int i = tick + 1; i < tick + 48; i++)
                    {
                        if (i >= Length) break;
                        int j = lane + 1;
                        if (j > 7) break;

                        NoteTypeDef ijType = Ticks[i].Notes[j].NoteType;

                        if (ijType.DetectType == DetectType.SwipeEndPoint & ijType.DetectDir == DetectDir.Right)
                            Ticks[i].Notes[j].IsSwipeEnd = true;

                        if ((ijType.DetectDir == DetectDir.Right & (ijType.DetectType == DetectType.SwipeEndPoint | ijType.DetectType == DetectType.SwipeMid)) || (ijType.DetectDir == DetectDir.Left & ijType.DetectType == DetectType.SwipeDirChange))
                        {
                            Ticks[tick].Notes[lane].SwipeEndPoint = new System.Drawing.Point(i, j);
                            break;
                        }
                    }
                }

                if (Type.DetectDir == DetectDir.Left && (Type.DetectType == DetectType.SwipeEndPoint | Type.DetectType == DetectType.SwipeMid | Type.DetectType == DetectType.SwipeDirChange) & !Note.IsSwipeEnd)
                {
                    for (int i = tick + 1; i < tick + 48; i++)
                    {
                        if (i >= Length) break;
                        int j = lane - 1;
                        if (j < 0) break;

                        NoteTypeDef ijType = Ticks[i].Notes[j].NoteType;

                        if (ijType.DetectType == DetectType.SwipeEndPoint & ijType.DetectDir == DetectDir.Left)
                            Ticks[i].Notes[j].IsSwipeEnd = true;

                        if ((ijType.DetectDir == DetectDir.Left & (ijType.DetectType == DetectType.SwipeEndPoint | ijType.DetectType == DetectType.SwipeMid)) || (ijType.DetectDir == DetectDir.Right & ijType.DetectType == DetectType.SwipeDirChange))
                        {
                            Ticks[tick].Notes[lane].SwipeEndPoint = new System.Drawing.Point(i, j);
                            break;
                        }
                    }
                }
            }


            public void FixSwipes()
            {
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


            public NoteTypeDef FindVisualNoteType(int tick, int lane)
            {
                if (tick >= Length) return NoteTypes.None;

                if (Ticks[tick].Notes[lane].NoteType.DetectType == DetectType.Hold)
                {
                    if (tick == 0 | tick == Length - 1) return Ticks[tick].Notes[lane].NoteType;
                    if ((Ticks[tick - 1].Notes[lane].NoteType.DetectType == DetectType.Hold |
                        Ticks[tick - 1].Notes[lane].NoteType.DetectType == DetectType.SwipeEndPoint) &&
                        Ticks[tick + 1].Notes[lane].NoteType.DetectType != DetectType.None)
                        return NoteTypes.ExtendHoldMid;
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
                        NoteTypeDef NoteType = FindVisualNoteType(i, j);
                        if (NoteType.NotNode != true & NoteType.DetectType != DetectType.SwipeMid)
                            SimulNum_Tap++;

                        if (NoteType.DetectType == DetectType.Tap | NoteType.DetectType == DetectType.Hold)
                            SimulNum_Hold++;
                    }

                    if (SimulNum_Tap > 1)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == DetectType.Tap)
                            {
                                Ticks[i].SetNote(NoteTypes.SimulTap, j, ref this);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == DetectType.Tap)
                            {
                                Ticks[i].SetNote(NoteTypes.Tap, j, ref this);
                            }
                        }
                    }

                    if (SimulNum_Hold > 1)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == DetectType.Hold)
                            {
                                if (i + 1 < Length & Ticks[i + 1].Notes[j].NoteType.DetectType == DetectType.Hold)
                                    Ticks[i].SetNote(NoteTypes.SimulHoldStart, j, ref this);
                                else
                                    Ticks[i].SetNote(NoteTypes.SimulHoldRelease, j, ref this);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            NoteTypeDef NoteType = FindVisualNoteType(i, j);
                            if (NoteType.DetectType == DetectType.Hold)
                            {
                                Ticks[i].SetNote(NoteTypes.Hold, j, ref this);
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
                {
                    if (tickObj[i].Buttons[j] != 0)
                    {
                        //chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)tickObj[i].Buttons[j], j, ref chart);
                        chart.Ticks[tickObjTickNumber(tickObj, i)].Notes[j].NoteType = NoteTypes.gettypebyid(tickObj[i].Buttons[j]);
                    }
                }

                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON1), 0);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON2), 1);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON3), 2);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON4), 3);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON5), 4);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON6), 5);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON7), 6);
                //chart.Ticks[chartTick].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON8), 7);
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
                NewTick.BUTTON1 = chart.Ticks[i].Notes[0].NoteType.NoteId;
                NewTick.BUTTON2 = chart.Ticks[i].Notes[1].NoteType.NoteId;
                NewTick.BUTTON3 = chart.Ticks[i].Notes[2].NoteType.NoteId;
                NewTick.BUTTON4 = chart.Ticks[i].Notes[3].NoteType.NoteId;
                NewTick.BUTTON5 = chart.Ticks[i].Notes[4].NoteType.NoteId;
                NewTick.BUTTON6 = chart.Ticks[i].Notes[5].NoteType.NoteId;
                NewTick.BUTTON7 = chart.Ticks[i].Notes[6].NoteType.NoteId;
                NewTick.BUTTON8 = chart.Ticks[i].Notes[7].NoteType.NoteId;

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
                    if (chart.Ticks[i].Notes[j].NoteType.NoteId > 0) TickHasNote = true;
                }

                if (TickHasNote)
                {
                    JsonTick_Export NewTick = new JsonTick_Export();
                    NewTick.BAR = i / 48;
                    NewTick.BEAT = i % 48;
                    NewTick.BUTTON1 = chart.Ticks[i].Notes[0].NoteType.NoteId;
                    NewTick.BUTTON2 = chart.Ticks[i].Notes[1].NoteType.NoteId;
                    NewTick.BUTTON3 = chart.Ticks[i].Notes[2].NoteType.NoteId;
                    NewTick.BUTTON4 = chart.Ticks[i].Notes[3].NoteType.NoteId;
                    NewTick.BUTTON5 = chart.Ticks[i].Notes[4].NoteType.NoteId;
                    NewTick.BUTTON6 = chart.Ticks[i].Notes[5].NoteType.NoteId;
                    NewTick.BUTTON7 = chart.Ticks[i].Notes[6].NoteType.NoteId;
                    NewTick.BUTTON8 = chart.Ticks[i].Notes[7].NoteType.NoteId;

                    tickObj.Add(NewTick);
                }
            }

            return JsonConvert.SerializeObject(tickObj).Replace("null", "\"\"").Replace(":0", ":\"\"").Replace("R\":\"\"", "R\":0").Replace("T\":\"\"", "T\":0");
        }
    }
}
