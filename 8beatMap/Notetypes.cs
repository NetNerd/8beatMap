using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
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

    public struct NoteType
    {
        public int NoteId;

        public RenderMode RenderMode;

        public string OGLTextureName;

        public Color BackColor;
        public Color IconColor;

        public IconType IconType;
    }


    public static class NoteTypes
    {
        public static NoteType None = new NoteType() { NoteId = 0, RenderMode = RenderMode.None, OGLTextureName = null, BackColor = Color.Transparent, IconColor = Color.Transparent, IconType = IconType.None };

        public static NoteType Tap = new NoteType() { NoteId = 1, RenderMode = RenderMode.Icon, OGLTextureName = "spr_TapIcon",  BackColor = Color.Blue, IconColor = Color.Transparent, IconType = IconType.None };
        public static NoteType Hold = new NoteType() { NoteId = 2, RenderMode = RenderMode.Icon, OGLTextureName = "spr_HoldIcon", BackColor = Color.LimeGreen, IconColor = Color.Transparent, IconType = IconType.None };
        public static NoteType SimulTap = new NoteType() { NoteId = 3, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None };
        public static NoteType SimulHoldStart = new NoteType() { NoteId = 9, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None };
        public static NoteType SimulHoldRelease = new NoteType() { NoteId = 8, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.Transparent, IconType = IconType.None };

        public static NoteType FlickLeft = new NoteType() { NoteId = 13, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow };
        public static NoteType HoldEndFlickLeft = new NoteType() { NoteId = 11, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow };
        public static NoteType SwipeLeftStartEnd = new NoteType() { NoteId = 6, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkViolet, IconType = IconType.LeftArrow };
        public static NoteType SwipeLeftMid = new NoteType() { NoteId = 7, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow };
        public static NoteType SwipeChangeDirR2L = new NoteType() { NoteId = 14, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow };

        public static NoteType FlickRight = new NoteType() { NoteId = 12, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow };
        public static NoteType HoldEndFlickRight = new NoteType() { NoteId = 10, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow };
        public static NoteType SwipeRightStartEnd = new NoteType() { NoteId = 4, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkOrange, IconType = IconType.RightArrow };
        public static NoteType SwipeRightMid = new NoteType() { NoteId = 7, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow };
        public static NoteType SwipeChangeDirL2R = new NoteType() { NoteId = 15, RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow };

        public static NoteType ExtendHoldMid = new NoteType() { NoteId = 20, RenderMode = RenderMode.HoldLocus, OGLTextureName = null, BackColor = Color.LightGray, IconColor = Color.Transparent, IconType = IconType.None };


        static Dictionary<int, NoteType> idToTypeDict = new Dictionary<int, NoteType> { { 0, None }, { 1, Tap }, { 2, Hold }, { 3, SimulTap }, { 9, SimulHoldStart }, { 8, SimulHoldRelease },
                                                                                        { 13, FlickLeft }, { 11, HoldEndFlickLeft }, { 6, SwipeLeftStartEnd }, { 7, SwipeLeftMid },
                                                                                        { 14, SwipeChangeDirR2L }, { 12, FlickRight }, { 10, HoldEndFlickRight }, { 4, SwipeRightStartEnd },
                                                                                        { 7, SwipeRightMid }, { 15, SwipeChangeDirL2R }, { 20, ExtendHoldMid } };


        public static NoteType gettypebyid(int id)
        {
            try { return idToTypeDict[id]; }
            catch { return None; }
        }
    }
}
