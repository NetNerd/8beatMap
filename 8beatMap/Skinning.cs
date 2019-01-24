using System.Collections.Generic;
using System.Drawing;

namespace _8beatMap
{
    public static class Skinning
    {
        public struct Skin
        {
            public string SkinName;
            public string RootDir;

            public Dictionary<string, string> TexturePaths;

            public Dictionary<string, Color[]> EditorColours;

            public Point[] NodeStartLocs;
            public Point[] NodeEndLocs;
            public int NumLanes;

            public Dictionary<string, string> SoundPaths;
        }

        private static string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        private static Dictionary<string, Color[]> LoadColours(string defs)
        {
            System.ComponentModel.TypeConverter colorConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Color));
            // (Color)colorConverter.ConvertFromString("#00000000")

            Dictionary<string, Color[]> noteColours_Default = new Dictionary<string, Color[]>
            {
                { NoteTypes.NoteTypeDefs.None.TypeName, new Color[] { Color.Transparent, Color.Transparent } },

                { NoteTypes.NoteTypeDefs.Tap.TypeName, new Color[] { Color.Blue, Color.Transparent } },
                { NoteTypes.NoteTypeDefs.Hold.TypeName, new Color[] { Color.LimeGreen, Color.Transparent } },
                { NoteTypes.NoteTypeDefs.SimulTap.TypeName, new Color[] { Color.DeepPink, Color.Transparent } },
                { NoteTypes.NoteTypeDefs.SimulHoldStart.TypeName, new Color[] { Color.DeepPink, Color.Transparent } },
                { NoteTypes.NoteTypeDefs.SimulHoldRelease.TypeName, new Color[] { Color.DeepPink, Color.Transparent } },

                { NoteTypes.NoteTypeDefs.FlickLeft.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.FromArgb(0x70, 0, 0x78) } },
                { NoteTypes.NoteTypeDefs.HoldEndFlickLeft.TypeName, new Color[] { Color.LightGray, Color.FromArgb(0x70, 0, 0x78) } },
                { NoteTypes.NoteTypeDefs.SwipeLeftStartEnd.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.DarkViolet } },
                { NoteTypes.NoteTypeDefs.SwipeLeftMid.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.Violet } },
                { NoteTypes.NoteTypeDefs.SwipeChangeDirR2L.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.Violet } },

                { NoteTypes.NoteTypeDefs.FlickRight.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.FromArgb(0xcc, 0x88, 0) } },
                { NoteTypes.NoteTypeDefs.HoldEndFlickRight.TypeName, new Color[] { Color.LightGray, Color.FromArgb(0xcc, 0x88, 0) } },
                { NoteTypes.NoteTypeDefs.SwipeRightStartEnd.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.DarkOrange } },
                { NoteTypes.NoteTypeDefs.SwipeRightMid.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.Gold } },
                { NoteTypes.NoteTypeDefs.SwipeChangeDirL2R.TypeName, new Color[] { Color.FromArgb(0xc0, 0xc0, 0xc0), Color.Gold } },

                { NoteTypes.NoteTypeDefs.ExtendHoldMid.TypeName, new Color[] { Color.LightGray, Color.Transparent } },

                { NoteTypes.NoteTypeDefs.GbsFlick.TypeName, new Color[] { Color.Gold, Color.LightYellow } },
                { NoteTypes.NoteTypeDefs.GbsHoldEndFlick.TypeName, new Color[] { Color.LightGray, Color.Gold } },
                { NoteTypes.NoteTypeDefs.GbsSimulFlick.TypeName, new Color[] { Color.Goldenrod, Color.LightYellow } },
                { NoteTypes.NoteTypeDefs.GbsClock.TypeName, new Color[] { Color.Blue, Color.Gold } },
                { NoteTypes.NoteTypeDefs.GbsSimulClock.TypeName, new Color[] { Color.DeepPink, Color.Gold } }
            };
            Dictionary<string, Color[]> noteColours = noteColours_Default;

            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length; i++)
            {
                string cleanDef = defslines[i].Replace(" ", "");
                string type = cleanDef.Split(":".ToCharArray())[0];
                string[] vals = cleanDef.Split(":".ToCharArray())[1].Split(",".ToCharArray());

                noteColours[type] = new Color[] { (Color)colorConverter.ConvertFromString(vals[0]), (Color)colorConverter.ConvertFromString(vals[1]) };
            }

            return noteColours;
        }

        private static Dictionary<string, string> LoadTexturePaths(string rootdir)
        {
            Dictionary<string, string> TexturePaths_Default = new Dictionary<string, string>
            {
                {"spr_HoldLocus", rootdir + "/nodeimg/locus.png"},
                {"spr_SwipeLocus", rootdir + "/nodeimg/locus2.png"},

                {"spr_TapIcon", rootdir + "/nodeimg/node_1.png"},
                {"spr_HoldIcon", rootdir + "/nodeimg/node_2.png"},
                {"spr_SimulIcon", rootdir + "/nodeimg/node_3.png"},

                {"spr_SwipeRightIcon", rootdir + "/nodeimg/node_4.png"},
                {"spr_SwipeRightIcon_Simul", rootdir + "/nodeimg/node_4_3.png"},
                {"spr_SwipeLeftIcon", rootdir + "/nodeimg/node_6.png"},
                {"spr_SwipeLeftIcon_Simul", rootdir + "/nodeimg/node_6_3.png"},

                {"spr_gbsFlick", rootdir + "/nodeimg/gbs/node_7.png"},
                {"spr_gbsFlick_Simul", rootdir + "/nodeimg/gbs/node_8.png"},
                {"spr_gbsClock", rootdir + "/nodeimg/gbs/node_9.png"},
                {"spr_gbsClock_Simul", rootdir + "/nodeimg/gbs/node_10.png"},

                {"spr_HitEffect", rootdir + "/nodeimg/node_effect.png"},

                {"spr_Chara1", rootdir + "/charaimg/1.png"},
                {"spr_Chara2", rootdir + "/charaimg/2.png"},
                {"spr_Chara3", rootdir + "/charaimg/3.png"},
                {"spr_Chara4", rootdir + "/charaimg/4.png"},
                {"spr_Chara5", rootdir + "/charaimg/5.png"},
                {"spr_Chara6", rootdir + "/charaimg/6.png"},
                {"spr_Chara7", rootdir + "/charaimg/7.png"},
                {"spr_Chara8", rootdir + "/charaimg/8.png"},
            };

            return TexturePaths_Default;
        }
        private static Dictionary<string, string> LoadSoundPaths(string rootdir)
        {
            Dictionary<string, string> SoundPaths_Default = new Dictionary<string, string>
            {
                {"hit", rootdir + "/notesnd/hit.wav"},
                {"swipe", rootdir + "/notesnd/swipe.wav"},
            };

            return SoundPaths_Default;
        }

        private static int maxlanes = 8;

        private static Point[] LoadNodeStartLocs(string defs)
        {
            Point[] NodeStartLocs = new Point[maxlanes];

            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length; i++)
            {
                if (i >= maxlanes) break;

                string[] pointstrs = defslines[i].Split(" ".ToCharArray());
                string[] startstrs = pointstrs[0].Split(",".ToCharArray());
                Point start = new Point(int.Parse(startstrs[0]), int.Parse(startstrs[1]));
                
                NodeStartLocs[i] = start;
            }

            return NodeStartLocs;
        }
        private static Point[] LoadNodeEndLocs(string defs)
        {
            Point[] NodeEndLocs = new Point[maxlanes];

            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length; i++)
            {
                if (i >= maxlanes) break;

                string[] pointstrs = defslines[i].Split(" ".ToCharArray());
                string[] endstrs = pointstrs[1].Split(",".ToCharArray());
                Point end = new Point(int.Parse(endstrs[0]), int.Parse(endstrs[1]));
                
                NodeEndLocs[i] = end;
            }
            return NodeEndLocs;
        }
        private static int LoadNumLanes(string defs)
        {
            int NumLanes = 0;
            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length; i++)
            {
                string[] pointstrs = defslines[i].Split(" ".ToCharArray());
                string[] startstrs = pointstrs[0].Split(",".ToCharArray());
                Point start = new Point(int.Parse(startstrs[0]), int.Parse(startstrs[1]));

                if (start.X > -1) NumLanes++;
            }
            if (NumLanes > 8) return 8;
            else return NumLanes;
        }

        public static Skin LoadSkin(string rootdir)
        {
            string skinname = new System.IO.DirectoryInfo(rootdir).Name;

            string buttonsfile = ReadFile(rootdir + "/buttons.txt");
            Skin output = new Skin
            {
                SkinName = skinname,
                RootDir = rootdir,
                TexturePaths = LoadTexturePaths(rootdir),
                EditorColours = LoadColours(ReadFile(rootdir + "/colours.txt")),
                NodeStartLocs = LoadNodeStartLocs(buttonsfile),
                NodeEndLocs = LoadNodeEndLocs(buttonsfile),
                NumLanes = LoadNumLanes(buttonsfile),
                SoundPaths = LoadSoundPaths(rootdir)
            };
            return output;
        }

        public static Skin DefaultSkin = LoadSkin("skins/8bs");
    }
}
