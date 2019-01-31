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
        [Serializable]
        public enum RenderMode
        {
            None,
            Icon,
            HoldLocus
        }

        [Serializable]
        public enum IconType
        {
            None,
            LeftArrow,
            RightArrow,
            UpArrow,
            HalfSplit
        }

        [Serializable]
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
            GbsFlick,
            GbsClock
        }

        [Serializable]
        public enum DetectDir
        {
            None,
            Left,
            Right
        }

        [Serializable]
        public struct NoteTypeDef
        {
            public string TypeName;
            public int TypeId;

            public RenderMode RenderMode;

            public string OGLTextureName;

            public IconType IconType;

            public DetectType DetectType;
            public DetectDir DetectDir;

            public bool NotNode;

            public bool IsSimul;
        }

        public static class NoteTypeDefs
        {

            public static NoteTypeDef None = new NoteTypeDef() { TypeName = "None", TypeId = 0, RenderMode = RenderMode.None, OGLTextureName = null, IconType = IconType.None, DetectType = DetectType.None, DetectDir = DetectDir.None, NotNode = true, IsSimul = false };

            public static NoteTypeDef Tap = new NoteTypeDef() { TypeName = "Tap", TypeId = 1, RenderMode = RenderMode.Icon, OGLTextureName = "spr_TapIcon", IconType = IconType.None, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef Hold = new NoteTypeDef() { TypeName = "Hold", TypeId = 2, RenderMode = RenderMode.Icon, OGLTextureName = "spr_HoldIcon", IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef SimulTap = new NoteTypeDef() { TypeName = "SimulTap", TypeId = 3, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", IconType = IconType.None, DetectType = DetectType.Tap, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef SimulHoldStart = new NoteTypeDef() { TypeName = "SimulHoldStart", TypeId = 9, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef SimulHoldRelease = new NoteTypeDef() { TypeName = "SimulHoldRelease", TypeId = 8, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", IconType = IconType.None, DetectType = DetectType.Hold, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };

            public static NoteTypeDef FlickLeft = new NoteTypeDef() { TypeName = "FlickLeft", TypeId = 13, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", IconType = IconType.LeftArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef HoldEndFlickLeft = new NoteTypeDef() { TypeName = "HoldEndFlickLeft", TypeId = 11, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", IconType = IconType.LeftArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeLeftStartEnd = new NoteTypeDef() { TypeName = "SwipeLeftStartEnd", TypeId = 6, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", IconType = IconType.LeftArrow, DetectType = DetectType.SwipeEndPoint, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeLeftMid = new NoteTypeDef() { TypeName = "SwipeLeftMid", TypeId = 7, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", IconType = IconType.LeftArrow, DetectType = DetectType.SwipeMid, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeChangeDirR2L = new NoteTypeDef() { TypeName = "SwipeChangeDirR2L", TypeId = 14, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", IconType = IconType.LeftArrow, DetectType = DetectType.SwipeDirChange, DetectDir = DetectDir.Left, NotNode = false, IsSimul = false };

            public static NoteTypeDef FlickRight = new NoteTypeDef() { TypeName = "FlickRight", TypeId = 12, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", IconType = IconType.RightArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef HoldEndFlickRight = new NoteTypeDef() { TypeName = "HoldEndFlickRight", TypeId = 10, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", IconType = IconType.RightArrow, DetectType = DetectType.Flick, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeRightStartEnd = new NoteTypeDef() { TypeName = "SwipeRightStartEnd", TypeId = 4, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", IconType = IconType.RightArrow, DetectType = DetectType.SwipeEndPoint, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeRightMid = new NoteTypeDef() { TypeName = "SwipeRightMid", TypeId = 5, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", IconType = IconType.RightArrow, DetectType = DetectType.SwipeMid, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };
            public static NoteTypeDef SwipeChangeDirL2R = new NoteTypeDef() { TypeName = "SwipeChangeDirL2R", TypeId = 15, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", IconType = IconType.RightArrow, DetectType = DetectType.SwipeDirChange, DetectDir = DetectDir.Right, NotNode = false, IsSimul = false };

            public static NoteTypeDef ExtendHoldMid = new NoteTypeDef() { TypeName = "ExtendHoldMid", TypeId = 88, RenderMode = RenderMode.HoldLocus, OGLTextureName = null, IconType = IconType.None, DetectType = DetectType.HoldMid, DetectDir = DetectDir.None, NotNode = true, IsSimul = false };

            public static NoteTypeDef GbsFlick = new NoteTypeDef() { TypeName = "GbsFlick", TypeId = 20, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsFlick", IconType = IconType.UpArrow, DetectType = DetectType.GbsFlick, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef GbsHoldEndFlick = new NoteTypeDef() { TypeName = "GbsHoldEndFlick", TypeId = 21, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsFlick", IconType = IconType.UpArrow, DetectType = DetectType.GbsFlick, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef GbsSimulFlick = new NoteTypeDef() { TypeName = "GbsSimulFlick", TypeId = 30, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsFlick_Simul", IconType = IconType.UpArrow, DetectType = DetectType.GbsFlick, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };
            public static NoteTypeDef GbsClock = new NoteTypeDef() { TypeName = "GbsClock", TypeId = 40, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsClock", IconType = IconType.HalfSplit, DetectType = DetectType.GbsClock, DetectDir = DetectDir.None, NotNode = false, IsSimul = false };
            public static NoteTypeDef GbsSimulClock = new NoteTypeDef() { TypeName = "GbsSimulClock", TypeId = 41, RenderMode = RenderMode.Icon, OGLTextureName = "spr_gbsClock_Simul", IconType = IconType.HalfSplit, DetectType = DetectType.GbsClock, DetectDir = DetectDir.None, NotNode = false, IsSimul = true };


            static Dictionary<int, NoteTypeDef> idToTypeDict = new Dictionary<int, NoteTypeDef> { { 0, None }, { 1, Tap }, { 2, Hold }, { 3, SimulTap }, { 9, SimulHoldStart }, { 8, SimulHoldRelease },
                                                                                            { 13, FlickLeft }, { 11, HoldEndFlickLeft }, { 6, SwipeLeftStartEnd }, { 7, SwipeLeftMid },
                                                                                            { 14, SwipeChangeDirR2L }, { 12, FlickRight }, { 10, HoldEndFlickRight }, { 4, SwipeRightStartEnd },
                                                                                            { 5, SwipeRightMid }, { 15, SwipeChangeDirL2R }, { 88, ExtendHoldMid },
                                                                                            { 20, GbsFlick }, { 21, GbsHoldEndFlick }, { 30, GbsSimulFlick }, { 40, GbsClock }, { 41, GbsSimulClock } };


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
            { "HoldEndFlickRight", NoteTypeDefs.HoldEndFlickRight.TypeId },

            { "GbsFlick", NoteTypeDefs.GbsFlick.TypeId },
            { "GbsSimulFlick", NoteTypeDefs.GbsSimulFlick.TypeId },
            { "GbsClock", NoteTypeDefs.GbsClock.TypeId },
            { "GbsSimulClock", NoteTypeDefs.GbsSimulClock.TypeId }
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
            { "H", NoteTypeDefs.SwipeChangeDirL2R.TypeId },

            { "_7", NoteTypeDefs.GbsFlick.TypeId },
            { "U", NoteTypeDefs.GbsSimulFlick.TypeId },

            { "_8", NoteTypeDefs.GbsClock.TypeId },
            { "I", NoteTypeDefs.GbsSimulClock.TypeId }
        };
    }
}
