using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static class NoteTypes
    {
        public enum RenderMode
        {
            None,
            Icon,
            HoldLocus
        }

        public enum IconType
        {
            None,
            LeftArrow,
            RightArrow,
            UpArrow,
            HalfSplit
        }

        public enum DetectType
        {
            None,
            Tap,
            Hold,
            HoldMid,
            SwipeEndPoint,
            SwipeMid,
            SwipeDirChange,
            Flick,
            GbsFlick
        }

        public enum DetectDir
        {
            None,
            Left,
            Right
        }

        public struct NoteTypeDef
        {
            public int TypeId;

            public RenderMode RenderMode;

            public string OGLTextureName;

            public Color BackColor;
            public Color IconColor;

            public IconType IconType;

            public DetectType DetectType;
            public DetectDir DetectDir;

            public bool NotNode;

            public bool IsSimul;
        }

        public static class NoteTypeDefs
        {

            public static NoteTypeDef None = new NoteTypeDef() { TypeId = 0, RenderMode = RenderMode.None, OGLTextureName = null, BackColor = Color.Transparent, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.None, DetectDir = DetectDir.None, NotNode = true, IsSimul = false };

            public static NoteTypeDef Tap = new NoteTypeDef() { TypeId = 1, RenderMode = RenderMode.Icon, OGLTextureName = "spr_TapIcon", BackColor = Color.Blue, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef Hold = new NoteTypeDef() { TypeId = 2, RenderMode = RenderMode.Icon, OGLTextureName = "spr_HoldIcon", BackColor = Color.LimeGreen, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef SimulTap = new NoteTypeDef() { TypeId = 3, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef SimulHoldStart = new NoteTypeDef() { TypeId = 9, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef SimulHoldRelease = new NoteTypeDef() { TypeId = 8, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };

            public static NoteTypeDef FlickLeft = new NoteTypeDef() { TypeId = 13, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef HoldEndFlickLeft = new NoteTypeDef() { TypeId = 11, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeLeftStartEnd = new NoteTypeDef() { TypeId = 6, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkViolet, IconType = IconType.LeftArrow, DetectType = DetectType.SwipeEndPoint, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeLeftMid = new NoteTypeDef() { TypeId = 7, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow, DetectType = DetectType.SwipeMid, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeChangeDirR2L = new NoteTypeDef() { TypeId = 14, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow, DetectType = DetectType.SwipeDirChange, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };

            public static NoteTypeDef FlickRight = new NoteTypeDef() { TypeId = 12, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef HoldEndFlickRight = new NoteTypeDef() { TypeId = 10, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeRightStartEnd = new NoteTypeDef() { TypeId = 4, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkOrange, IconType = IconType.RightArrow, DetectType = DetectType.SwipeEndPoint, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeRightMid = new NoteTypeDef() { TypeId = 5, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow, DetectType = DetectType.SwipeMid, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeChangeDirL2R = new NoteTypeDef() { TypeId = 15, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow, DetectType = DetectType.SwipeDirChange, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };

            public static NoteTypeDef ExtendHoldMid = new NoteTypeDef() { TypeId = 88, RenderMode = RenderMode.HoldLocus, OGLTextureName = null, BackColor = Color.LightGray, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.HoldMid, DetectDir = DetectDir.None, NotNode = true, IsSimul = false };

            public static NoteTypeDef GbsFlick = new NoteTypeDef() { TypeId = 20, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsFlick", BackColor = Color.Gold, IconColor = Color.LightYellow, IconType = IconType.UpArrow, DetectType = DetectType.GbsFlick, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef GbsHoldEndFlick = new NoteTypeDef() { TypeId = 21, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsFlick", BackColor = Color.LightGray, IconColor = Color.Gold, IconType = IconType.UpArrow, DetectType = DetectType.GbsFlick, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef GbsSimulFlick = new NoteTypeDef() { TypeId = 30, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsFlick_Simul", BackColor = Color.Goldenrod, IconColor = Color.LightYellow, IconType = IconType.UpArrow, DetectType = DetectType.GbsFlick, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef GbsClock = new NoteTypeDef() { TypeId = 40, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsClock", BackColor = Color.DeepPink, IconColor = Color.Gold, IconType = IconType.HalfSplit, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };


            static Dictionary<int, NoteTypeDef> idToTypeDict = new Dictionary<int, NoteTypeDef> { { 0, None }, { 1, Tap }, { 2, Hold }, { 3, SimulTap }, { 9, SimulHoldStart }, { 8, SimulHoldRelease },
                                                                                            { 13, FlickLeft }, { 11, HoldEndFlickLeft }, { 6, SwipeLeftStartEnd }, { 7, SwipeLeftMid },
                                                                                            { 14, SwipeChangeDirR2L }, { 12, FlickRight }, { 10, HoldEndFlickRight }, { 4, SwipeRightStartEnd },
                                                                                            { 5, SwipeRightMid }, { 15, SwipeChangeDirL2R }, { 88, ExtendHoldMid },
                                                                                            { 20, GbsFlick }, { 21, GbsHoldEndFlick }, { 30, GbsSimulFlick }, { 40, GbsClock } };


            public static NoteTypeDef gettypebyid(int id)
            {
                try { return idToTypeDict[id]; }
                catch { return None; }
            }
        }


        public static Dictionary<string, int> UserVisibleNoteTypes = new Dictionary<string, int>
        {
            { "Tap", NoteTypeDefs.Tap.TypeId },
            { "SimulTap", NoteTypeDefs.SimulTap.TypeId },

            { "Hold", NoteTypeDefs.Hold.TypeId },
            { "SimulHoldStart", NoteTypeDefs.SimulHoldStart.TypeId },
            { "SimulHoldRelease", NoteTypeDefs.SimulHoldRelease.TypeId },

            { "SwipeLeftStartEnd", NoteTypeDefs.SwipeLeftStartEnd.TypeId },
            { "SwipeLeftMid", NoteTypeDefs.SwipeLeftMid.TypeId },

            { "SwipeRightStartEnd", NoteTypeDefs.SwipeRightStartEnd.TypeId },
            { "SwipeRightMid", NoteTypeDefs.SwipeRightMid.TypeId },

            { "SwipeChangeDirL2R", NoteTypeDefs.SwipeChangeDirL2R.TypeId },
            { "SwipeChangeDirR2L", NoteTypeDefs.SwipeChangeDirR2L.TypeId },

            { "FlickLeft", NoteTypeDefs.FlickLeft.TypeId },
            { "HoldEndFlickLeft", NoteTypeDefs.HoldEndFlickLeft.TypeId },

            { "FlickRight", NoteTypeDefs.FlickRight.TypeId },
            { "HoldEndFlickRight", NoteTypeDefs.HoldEndFlickRight.TypeId }
        };

        public static Dictionary<string, int> UserVisibleNoteTypes_Nihongo = new Dictionary<string, int>
        {
            { "押す", NoteTypeDefs.Tap.TypeId },
            { "同時に押す", NoteTypeDefs.SimulTap.TypeId },

            { "長い押す", NoteTypeDefs.Hold.TypeId },
            { "同時に長い押すの最初", NoteTypeDefs.SimulHoldStart.TypeId },
            { "同時に長い押すの最後", NoteTypeDefs.SimulHoldRelease.TypeId },

            { "左へスワイプの端", NoteTypeDefs.SwipeLeftStartEnd.TypeId },
            { "左へスワイプの真ん中で", NoteTypeDefs.SwipeLeftMid.TypeId },

            { "右へスワイプの端", NoteTypeDefs.SwipeRightStartEnd.TypeId },
            { "右へスワイプの真ん中で", NoteTypeDefs.SwipeRightMid.TypeId },

            { "左へスワイプから右に変わる", NoteTypeDefs.SwipeChangeDirL2R.TypeId },
            { "右へスワイプから左に変わる", NoteTypeDefs.SwipeChangeDirR2L.TypeId },

            { "左へフリック", NoteTypeDefs.FlickLeft.TypeId },
            { "長い押すの最後での左へフリック", NoteTypeDefs.HoldEndFlickLeft.TypeId },

            { "右へフリック", NoteTypeDefs.FlickRight.TypeId },
            { "長い押すの最後での右へフリック", NoteTypeDefs.HoldEndFlickRight.TypeId }
        };

        public static Dictionary<string, int> NoteShortcutKeys = new Dictionary<string, int>
        {
            { "_1", NoteTypeDefs.Tap.TypeId },
            { "Q", NoteTypeDefs.SimulTap.TypeId },

            { "_2", NoteTypeDefs.Hold.TypeId },
            { "W", NoteTypeDefs.SimulHoldStart.TypeId },
            { "S", NoteTypeDefs.SimulHoldRelease.TypeId },

            { "_3", NoteTypeDefs.FlickLeft.TypeId },
            { "E", NoteTypeDefs.HoldEndFlickLeft.TypeId },
            { "_4", NoteTypeDefs.FlickRight.TypeId },
            { "R", NoteTypeDefs.HoldEndFlickRight.TypeId },

            { "_5", NoteTypeDefs.SwipeLeftStartEnd.TypeId },
            { "T", NoteTypeDefs.SwipeLeftMid.TypeId },
            { "G", NoteTypeDefs.SwipeChangeDirR2L.TypeId },

            { "_6", NoteTypeDefs.SwipeRightStartEnd.TypeId },
            { "Y", NoteTypeDefs.SwipeRightMid.TypeId },
            { "H", NoteTypeDefs.SwipeChangeDirL2R.TypeId }
        };
    }
}
