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
            RightArrow
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
            Flick
        }

        public enum DetectDir
        {
            None,
            Left,
            Right
        }

        public struct NoteTypeDef
        {
            public int NoteId;

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

            public static NoteTypeDef None = new NoteTypeDef() { NoteId = 0, RenderMode = RenderMode.None, OGLTextureName = null, BackColor = Color.Transparent, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.None, DetectDir = DetectDir.None, NotNode = true, IsSimul = false };

            public static NoteTypeDef Tap = new NoteTypeDef() { NoteId = 1, RenderMode = RenderMode.Icon, OGLTextureName = "spr_TapIcon", BackColor = Color.Blue, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef Hold = new NoteTypeDef() { NoteId = 2, RenderMode = RenderMode.Icon, OGLTextureName = "spr_HoldIcon", BackColor = Color.LimeGreen, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef SimulTap = new NoteTypeDef() { NoteId = 3, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef SimulHoldStart = new NoteTypeDef() { NoteId = 9, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef SimulHoldRelease = new NoteTypeDef() { NoteId = 8, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };

            public static NoteTypeDef FlickLeft = new NoteTypeDef() { NoteId = 13, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef HoldEndFlickLeft = new NoteTypeDef() { NoteId = 11, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeLeftStartEnd = new NoteTypeDef() { NoteId = 6, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkViolet, IconType = IconType.LeftArrow, DetectType = DetectType.SwipeEndPoint, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeLeftMid = new NoteTypeDef() { NoteId = 7, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow, DetectType = DetectType.SwipeMid, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeChangeDirR2L = new NoteTypeDef() { NoteId = 14, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow, DetectType = DetectType.SwipeDirChange, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };

            public static NoteTypeDef FlickRight = new NoteTypeDef() { NoteId = 12, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef HoldEndFlickRight = new NoteTypeDef() { NoteId = 10, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeRightStartEnd = new NoteTypeDef() { NoteId = 4, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkOrange, IconType = IconType.RightArrow, DetectType = DetectType.SwipeEndPoint, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeRightMid = new NoteTypeDef() { NoteId = 5, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow, DetectType = DetectType.SwipeMid, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeChangeDirL2R = new NoteTypeDef() { NoteId = 15, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow, DetectType = DetectType.SwipeDirChange, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };

            public static NoteTypeDef ExtendHoldMid = new NoteTypeDef() { NoteId = 20, RenderMode = RenderMode.HoldLocus, OGLTextureName = null, BackColor = Color.LightGray, IconColor = Color.Transparent, IconType = IconType.None, DetectType = DetectType.HoldMid, DetectDir = DetectDir.None, NotNode = true, IsSimul = false };


            static Dictionary<int, NoteTypeDef> idToTypeDict = new Dictionary<int, NoteTypeDef> { { 0, None }, { 1, Tap }, { 2, Hold }, { 3, SimulTap }, { 9, SimulHoldStart }, { 8, SimulHoldRelease },
                                                                                            { 13, FlickLeft }, { 11, HoldEndFlickLeft }, { 6, SwipeLeftStartEnd }, { 7, SwipeLeftMid },
                                                                                            { 14, SwipeChangeDirR2L }, { 12, FlickRight }, { 10, HoldEndFlickRight }, { 4, SwipeRightStartEnd },
                                                                                            { 5, SwipeRightMid }, { 15, SwipeChangeDirL2R }, { 20, ExtendHoldMid } };


            public static NoteTypeDef gettypebyid(int id)
            {
                try { return idToTypeDict[id]; }
                catch { return None; }
            }
        }


        public static Dictionary<string, int> UserVisibleNoteTypes = new Dictionary<string, int>
        {
            { "Tap", NoteTypeDefs.Tap.NoteId },
            { "SimulTap", NoteTypeDefs.SimulTap.NoteId },

            { "Hold", NoteTypeDefs.Hold.NoteId },
            { "SimulHoldStart", NoteTypeDefs.SimulHoldStart.NoteId },
            { "SimulHoldRelease", NoteTypeDefs.SimulHoldRelease.NoteId },

            { "SwipeLeftStartEnd", NoteTypeDefs.SwipeLeftStartEnd.NoteId },
            { "SwipeLeftMid", NoteTypeDefs.SwipeLeftMid.NoteId },

            { "SwipeRightStartEnd", NoteTypeDefs.SwipeRightStartEnd.NoteId },
            { "SwipeRightMid", NoteTypeDefs.SwipeRightMid.NoteId },

            { "SwipeChangeDirL2R", NoteTypeDefs.SwipeChangeDirL2R.NoteId },
            { "SwipeChangeDirR2L", NoteTypeDefs.SwipeChangeDirR2L.NoteId },

            { "FlickLeft", NoteTypeDefs.FlickLeft.NoteId },
            { "HoldEndFlickLeft", NoteTypeDefs.HoldEndFlickLeft.NoteId },

            { "FlickRight", NoteTypeDefs.FlickRight.NoteId },
            { "HoldEndFlickRight", NoteTypeDefs.HoldEndFlickRight.NoteId }
        };

        public static Dictionary<string, int> UserVisibleNoteTypes_Nihongo = new Dictionary<string, int>
        {
            { "押す", NoteTypeDefs.Tap.NoteId },
            { "同時に押す", NoteTypeDefs.SimulTap.NoteId },

            { "長い押す", NoteTypeDefs.Hold.NoteId },
            { "同時に長い押すの最初", NoteTypeDefs.SimulHoldStart.NoteId },
            { "同時に長い押すの最後", NoteTypeDefs.SimulHoldRelease.NoteId },

            { "左へスワイプの端", NoteTypeDefs.SwipeLeftStartEnd.NoteId },
            { "左へスワイプの真ん中で", NoteTypeDefs.SwipeLeftMid.NoteId },

            { "右へスワイプの端", NoteTypeDefs.SwipeRightStartEnd.NoteId },
            { "右へスワイプの真ん中で", NoteTypeDefs.SwipeRightMid.NoteId },

            { "左へスワイプから右に変わる", NoteTypeDefs.SwipeChangeDirL2R.NoteId },
            { "右へスワイプから左に変わる", NoteTypeDefs.SwipeChangeDirR2L.NoteId },

            { "左へフリック", NoteTypeDefs.FlickLeft.NoteId },
            { "長い押すの最後での左へフリック", NoteTypeDefs.HoldEndFlickLeft.NoteId },

            { "右へフリック", NoteTypeDefs.FlickRight.NoteId },
            { "長い押すの最後での右へフリック", NoteTypeDefs.HoldEndFlickRight.NoteId }
        };

        public static Dictionary<string, int> NoteShortcutKeys = new Dictionary<string, int>
        {
            { "_1", NoteTypeDefs.Tap.NoteId },
            { "Q", NoteTypeDefs.SimulTap.NoteId },

            { "_2", NoteTypeDefs.Hold.NoteId },
            { "W", NoteTypeDefs.SimulHoldStart.NoteId },
            { "S", NoteTypeDefs.SimulHoldRelease.NoteId },

            { "_3", NoteTypeDefs.FlickLeft.NoteId },
            { "E", NoteTypeDefs.HoldEndFlickLeft.NoteId },
            { "_4", NoteTypeDefs.FlickRight.NoteId },
            { "R", NoteTypeDefs.HoldEndFlickRight.NoteId },

            { "_5", NoteTypeDefs.SwipeLeftStartEnd.NoteId },
            { "T", NoteTypeDefs.SwipeLeftMid.NoteId },
            { "G", NoteTypeDefs.SwipeChangeDirR2L.NoteId },

            { "_6", NoteTypeDefs.SwipeRightStartEnd.NoteId },
            { "Y", NoteTypeDefs.SwipeRightMid.NoteId },
            { "H", NoteTypeDefs.SwipeChangeDirL2R.NoteId }
        };
    }
}
