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

                //set
                //{
                //    if (NoteArray == null)
                //    {
                //        NoteArray = new NoteType[8];
                //    }
                //    NoteArray = value;
                //}
            }


            private System.Windows.Forms.PictureBox[] NoteIconArray;

            public System.Windows.Forms.PictureBox[] NoteIcons
            {
                get
                {
                    if (NoteIconArray == null)
                    {
                        NoteIconArray = new System.Windows.Forms.PictureBox[8];
                    }
                    return NoteIconArray;
                }

                set
                {
                    if (NoteIconArray == null)
                    {
                        NoteIconArray = new System.Windows.Forms.PictureBox[8];
                    }
                    NoteIconArray = value;
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

            public string BUTTON1;
            public string BUTTON2;
            public string BUTTON3;
            public string BUTTON4;
            public string BUTTON5;
            public string BUTTON6;
            public string BUTTON7;
            public string BUTTON8;
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

            Chart chart = new Chart(tickObjTickNumber(tickObj, tickObj.Length - 1) + 1, 1);

            try { chart.BPM = int.Parse(tickObj[0].BAR) + double.Parse(tickObj[0].BEAT)/100; } catch { };

            for (int i = 1; i < tickObj.Length; i++)
            {
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON1), 0);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON2), 1);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON3), 2);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON4), 3);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON5), 4);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON6), 5);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON7), 6);
                chart.Ticks[tickObjTickNumber(tickObj, i)].SetNote((NoteType)SafeParseInt(tickObj[i].BUTTON8), 7);
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
