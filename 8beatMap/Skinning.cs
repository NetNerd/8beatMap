using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _8beatMap
{
    public static class Skinning
    {
        private static System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

        public struct Skin
        {
            public string SkinName;
            public string RootDir;

            public Dictionary<string, string> TexturePaths;

            public Dictionary<string, Color[]> EditorNoteColours;
            public Dictionary<string, Color> UIColours;
            public System.Windows.Forms.FlatStyle UIStyle;

            public Point[] NodeStartLocs;
            public Point[] NodeEndLocs;
            public int NumLanes;

            public Dictionary<string, string> SoundPaths;

            public ComboTextInfo ComboTextInfo;
        }

        public static Skin ShowUnskinnedErrorMessage(string extrainfo)
        {
            Skin failskin = new Skin
            {
                SkinName = "fallback skin",
                UIColours = LoadUIColours(""),
                UIStyle = LoadUIStyle("")
            };
            SkinnedMessageBox.Show(failskin, string.Format(DialogResMgr.GetString("SkinLoadError"), extrainfo), "", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return failskin; // can use this to keep compiler happy where we must return something
        }

        private static string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
        private static string ReadFile_EmptyStringIfException(string path)
        {
            try
            {
                return ReadFile(path);
            }
            catch
            {
                ShowUnskinnedErrorMessage("can't load file \"" + path + "\".");
                return "";
            }
        }

        private static Dictionary<string, Color[]> LoadNoteColours(string defs)
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
                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;
                string cleanDef = defslines[i].Replace(" ", "");
                if (!cleanDef.Contains(":")) continue;
                string type = cleanDef.Split(":".ToCharArray())[0];
                string[] vals = cleanDef.Split(":".ToCharArray())[1].Split(",".ToCharArray());

                noteColours[type] = new Color[] { (Color)colorConverter.ConvertFromString(vals[0]), (Color)colorConverter.ConvertFromString(vals[1]) };
            }

            return noteColours;
        }

        private static Dictionary<string, Color> LoadUIColours(string defs)
        {
            System.ComponentModel.TypeConverter colorConverter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Color));
            // (Color)colorConverter.ConvertFromString("#00000000")

            Dictionary<string, Color> uiColours_Default = new Dictionary<string, Color>
            {
                { UIColours.UIColourDefs.Chart_BG.TypeName, SystemColors.ControlLight },
                { UIColours.UIColourDefs.Chart_BG_Lane1.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane2.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane3.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane4.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane5.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane6.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane7.TypeName, Color.Transparent },
                { UIColours.UIColourDefs.Chart_BG_Lane8.TypeName, Color.Transparent },

                { UIColours.UIColourDefs.Chart_LaneLine.TypeName, Color.LightGray },
                { UIColours.UIColourDefs.Chart_BarLine.TypeName, Color.SlateGray },
                { UIColours.UIColourDefs.Chart_BarText.TypeName, Color.DarkSlateGray },
                { UIColours.UIColourDefs.Chart_QuarterLine.TypeName, Color.LightSlateGray },
                { UIColours.UIColourDefs.Chart_EigthLine.TypeName, Color.LightGray },

                { UIColours.UIColourDefs.Chart_Playhead.TypeName, Color.DarkSlateGray },

                { UIColours.UIColourDefs.Form_BG.TypeName, Form1.DefaultBackColor },
                { UIColours.UIColourDefs.Form_Text.TypeName, Form1.DefaultForeColor }
            };
            Dictionary<string, Color> uiColours = uiColours_Default;

            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length; i++)
            {
                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;
                string cleanDef = defslines[i].Replace(" ", "");
                if (!cleanDef.Contains(":")) continue;
                string type = cleanDef.Split(":".ToCharArray())[0];
                string val = cleanDef.Split(":".ToCharArray())[1];

                uiColours[type] = (Color)colorConverter.ConvertFromString(val);
            }

            return uiColours;
        }

        private static System.Windows.Forms.FlatStyle LoadUIStyle(string def)
        {
            def = def.Trim();

            if (def == "Flat") return System.Windows.Forms.FlatStyle.Flat;
            else if (def == "Popup") return System.Windows.Forms.FlatStyle.Popup;
            else if (def == "Standard") return System.Windows.Forms.FlatStyle.Standard;
            else return System.Windows.Forms.FlatStyle.System;
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

                {"spr_ComboText", rootdir + "/font/txt_combo.png"},
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
            int i2 = 0;
            for (int i = 0; i < defslines.Length; i++)
            {
                if (i >= maxlanes) break;

                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;

                if (!defslines[i].Contains(" ")) continue;
                string[] pointstrs = defslines[i].Split(" ".ToCharArray());
                if (!pointstrs[0].Contains(",")) continue;
                string[] startstrs = pointstrs[0].Split(",".ToCharArray());
                Point start = new Point(int.Parse(startstrs[0]), int.Parse(startstrs[1]));
                
                NodeStartLocs[i2] = start;
                i2++;
            }

            return NodeStartLocs;
        }
        private static Point[] LoadNodeEndLocs(string defs)
        {
            Point[] NodeEndLocs = new Point[maxlanes];

            string[] defslines = defs.Split("\n".ToCharArray());
            int i2 = 0;
            for (int i = 0; i < defslines.Length; i++)
            {
                if (i >= maxlanes) break;

                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;
                
                if (!defslines[i].Contains(" ")) continue;
                string[] pointstrs = defslines[i].Split(" ".ToCharArray());
                if (!pointstrs[0].Contains(",")) continue;
                string[] endstrs = pointstrs[1].Split(",".ToCharArray());
                Point end = new Point(int.Parse(endstrs[0]), int.Parse(endstrs[1]));
                
                NodeEndLocs[i2] = end;
                i2++;
            }
            return NodeEndLocs;
        }
        private static int LoadNumLanes(string defs)
        {
            int NumLanes = 0;
            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length; i++)
            {
                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;

                if (!defslines[i].Contains(" ")) continue;
                string[] pointstrs = defslines[i].Split(" ".ToCharArray());
                if (!pointstrs[0].Contains(",")) continue;
                string[] startstrs = pointstrs[0].Split(",".ToCharArray());
                Point start = new Point(int.Parse(startstrs[0]), int.Parse(startstrs[1]));

                if (start.X > -1) NumLanes++;
            }
            if (NumLanes > 8) return 8;
            else return NumLanes;
        }

        public struct ComboTextInfo
        {
            public Point[] Locs;
            public int TextSize;
            public BMFontReader.BMFont Font;
        }

        private static ComboTextInfo LoadComboTextInfo(string defs, string fontsdir)
        {
            ComboTextInfo outinfo = new ComboTextInfo();

            outinfo.Font = new BMFontReader.BMFont(fontsdir + "/font_combo.fnt");

            outinfo.Locs = new Point[2];

            string[] defslines = defs.Split("\n".ToCharArray());
            int i2 = 0;
            for (int i = 0; i < defslines.Length; i++)
            {
                if (i >= maxlanes) break;

                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;

                if (defslines[i].Contains(","))
                {
                    string[] pointstrs = defslines[1].Split(",".ToCharArray());
                    Point loc = new Point(int.Parse(pointstrs[0]), int.Parse(pointstrs[1]));

                    outinfo.Locs[i2] = loc;
                    i2++;
                }
                else
                {
                    try
                    {
                        outinfo.TextSize = int.Parse(defslines[i]);
                    }
                    catch
                    { }
                }
            }
            return outinfo;
        }

        public static Skin LoadSkin(string rootdir)
        {
            try
            {
                string skinname = "";
                try
                {
                    System.IO.DirectoryInfo dirinfo = new System.IO.DirectoryInfo(rootdir);
                    skinname = dirinfo.Name;
                    if (!dirinfo.Exists) throw new System.Exception();
                }
                catch
                {
                    ShowUnskinnedErrorMessage("skin \"" + skinname + "\" does not exist.");
                    skinname = "?";
                }

                string buttonsfile = ReadFile_EmptyStringIfException(rootdir + "/buttons.txt");
                Skin output = new Skin
                {
                    SkinName = skinname,
                    RootDir = rootdir,
                    TexturePaths = LoadTexturePaths(rootdir),
                    EditorNoteColours = LoadNoteColours(ReadFile_EmptyStringIfException(rootdir + "/notecolours.txt")),
                    UIColours = LoadUIColours(ReadFile_EmptyStringIfException(rootdir + "/uicolours.txt")),
                    UIStyle = LoadUIStyle(ReadFile_EmptyStringIfException(rootdir + "/uistyle.txt")),
                    NodeStartLocs = LoadNodeStartLocs(buttonsfile),
                    NodeEndLocs = LoadNodeEndLocs(buttonsfile),
                    NumLanes = LoadNumLanes(buttonsfile),
                    SoundPaths = LoadSoundPaths(rootdir),
                    ComboTextInfo = LoadComboTextInfo(ReadFile_EmptyStringIfException(rootdir + "/font/pos_combo.txt"), rootdir + "/font")
                };

                return output;
            }
            catch
            {
                Skin failskin = ShowUnskinnedErrorMessage("fatal error");
                System.Environment.Exit(0);
                return failskin; // just for compiler to be satisfied
            }
        }

        public static Skin DefaultSkin = LoadSkin("skins/8bs");



        public static void SetBackCol(Control elem, Color colour)
        {
            if ((elem.HasChildren || elem.GetType() == typeof(Button) || elem.GetType() == typeof(ComboBox)) && elem.GetType() != typeof(NumericUpDown))
            {
                elem.BackColor = colour;
                foreach (Control control in elem.Controls)
                {
                    SetBackCol(control, colour);
                }
            }
        }

        public static void SetForeCol(Control elem, Color colour)
        {
            if (elem.GetType() != typeof(NumericUpDown) && elem.GetType() != typeof(TextBox))
            {
                elem.ForeColor = colour;
            }
            if (elem.HasChildren && elem.GetType() != typeof(NumericUpDown))
            {
                foreach (Control control in elem.Controls)
                {
                    SetForeCol(control, colour);
                }
            }
        }

        public static void SetUIStyle(Control elem, FlatStyle style)
        {
            if (elem.GetType() == typeof(Button))
            {
                ((Button)elem).FlatStyle = style;
            }
            else if (elem.GetType() == typeof(ComboBox))
            {
                ((ComboBox)elem).FlatStyle = style;
            }
            else if (elem.GetType() == typeof(CheckBox))
            {
                ((CheckBox)elem).FlatStyle = style;
            }

            if (elem.HasChildren && elem.GetType() != typeof(NumericUpDown))
            {
                foreach (Control control in elem.Controls)
                {
                    SetUIStyle(control, style);
                }
            }
        }
    }
}
