using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace _8beatMap
{
    class Notedata
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

            ExtendHoldMid = 20
        }

        public struct Note
        {
            public NoteType Type;

            private int LaneNum;
            public int Lane
            {
                get { return LaneNum; }
                set
                {
                    if (0 < value && value < 9) LaneNum = value;
                    else throw new ArgumentOutOfRangeException();
                }
            }


            public Note(NoteType NoteType, int Lane)
            {
                Type = NoteType;
                LaneNum = Lane;
            }
        }


        public struct Tick
        {
            private NoteType[] NoteArray;

            public NoteType[] Notes
            {
                get
                {
                    if (NoteArray == null)
                    {
                        NoteArray = new NoteType[8];
                    }
                    return NoteArray;
                }

                set
                {
                    if (NoteArray == null)
                    {
                        NoteArray = new NoteType[8];
                    }
                    NoteArray = value;
                }
            }



            public void SetNote(NoteType Note, int Lane)
            {
                if (NoteArray == null)
                {
                    NoteArray = new NoteType[8];
                }
                NoteArray[Lane] = Note;
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
                    }
                }
            }

            public double BPM;


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

            string b1;
            string b2;
            string b3;
            string b4;
            string b5;
            string b6;
            string b7;
            string b8;

            public string BUTTON1 { get { if (b1 == "") return "0"; else return b1; } set { b1 = value.Trim(); } }
            public string BUTTON2 { get { if (b2 == "") return "0"; else return b2; } set { b2 = value.Trim(); } }
            public string BUTTON3 { get { if (b3 == "") return "0"; else return b3; } set { b3 = value.Trim(); } }
            public string BUTTON4 { get { if (b4 == "") return "0"; else return b4; } set { b4 = value.Trim(); } }
            public string BUTTON5 { get { if (b5 == "") return "0"; else return b5; } set { b5 = value.Trim(); } }
            public string BUTTON6 { get { if (b6 == "") return "0"; else return b6; } set { b6 = value.Trim(); } }
            public string BUTTON7 { get { if (b7 == "") return "0"; else return b7; } set { b7 = value.Trim(); } }
            public string BUTTON8 { get { if (b8 == "") return "0"; else return b8; } set { b8 = value.Trim(); } }
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

        public static Chart ConvertJsonToChart(string json)
        {
            var tickObj = JsonConvert.DeserializeObject<JsonTick_Import[]>(json);

            Chart chart = new Chart(tickObjTickNumber(tickObj, tickObj.Length - 1) + 1, 1);

            try { chart.BPM = int.Parse(tickObj[0].BAR) + double.Parse(tickObj[0].BEAT)/100; } catch { };

            for (int i = 1; i < tickObj.Length; i++)
            {
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON1), 0);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON2), 1);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON3), 2);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON4), 3);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON5), 4);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON6), 5);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON7), 6);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)int.Parse(tickObj[i].BUTTON8), 7);
            }

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
                NewTick.BUTTON1 = chart.Ticks[i].Notes[0].GetHashCode();
                NewTick.BUTTON2 = chart.Ticks[i].Notes[1].GetHashCode();
                NewTick.BUTTON3 = chart.Ticks[i].Notes[2].GetHashCode();
                NewTick.BUTTON4 = chart.Ticks[i].Notes[3].GetHashCode();
                NewTick.BUTTON5 = chart.Ticks[i].Notes[4].GetHashCode();
                NewTick.BUTTON6 = chart.Ticks[i].Notes[5].GetHashCode();
                NewTick.BUTTON7 = chart.Ticks[i].Notes[6].GetHashCode();
                NewTick.BUTTON8 = chart.Ticks[i].Notes[7].GetHashCode();

                tickObj.Add(NewTick);
            }

            return JsonConvert.SerializeObject(tickObj).Replace("null", "\"\"").Replace(":0", ":\"\"").Replace("R\":\"\"", "R\":0").Replace("T\":\"\"", "T\":0");
        }
    }
}
