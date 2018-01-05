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
        public RenderMode RenderMode;

        public string OGLTextureName;

        public Color BackColor;
        public Color IconColor;

        public IconType IconType;
    }


    public static class NoteTypes
    {
        public static NoteType Tap = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_TapIcon",  BackColor = Color.Blue, IconColor = Color.White, IconType = IconType.None };
        public static NoteType Hold = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_HoldIcon", BackColor = Color.LimeGreen, IconColor = Color.White, IconType = IconType.None };
        public static NoteType SimulTap = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SimulIcon", BackColor = Color.DeepPink, IconColor = Color.White, IconType = IconType.None };
        public static NoteType SimulHoldStart = SimulTap;
        public static NoteType SimulHoldRelease = SimulHoldStart;

        public static NoteType FlickLeft = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow };
        public static NoteType HoldEndFlickLeft = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0x70, 0, 0x78), IconType = IconType.LeftArrow };
        public static NoteType SwipeLeftStartEnd = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkViolet, IconType = IconType.LeftArrow };
        public static NoteType SwipeLeftMid = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeLeftIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Violet, IconType = IconType.LeftArrow };
        public static NoteType SwipeChangeDirR2L = SwipeLeftMid;

        public static NoteType FlickRight = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow };
        public static NoteType HoldEndFlickRight = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.LightGray, IconColor = Color.FromArgb(0xcc, 0x88, 0), IconType = IconType.RightArrow };
        public static NoteType SwipeRightStartEnd = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.DarkOrange, IconType = IconType.RightArrow };
        public static NoteType SwipeRightMid = new NoteType() { RenderMode = RenderMode.Icon, OGLTextureName = "spr_SwipeRightIcon", BackColor = Color.FromArgb(0xc0, 0xc0, 0xc0), IconColor = Color.Gold, IconType = IconType.RightArrow };
        public static NoteType SwipeChangeDirL2R = SwipeRightMid;

        public static NoteType ExtendHoldMid = new NoteType() { RenderMode = RenderMode.HoldLocus, OGLTextureName = null, BackColor = Color.LightGray, IconColor = Color.White, IconType = IconType.None };
    }
}
