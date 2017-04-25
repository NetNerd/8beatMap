using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.Drawing.Imaging;

namespace _8beatMap
{
    public partial class Form1 : Form
    {
        System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

        Notedata.Chart chart = new Notedata.Chart(32 * 48, 120);
        private int TickHeight = 10;
        private int IconWidth = 20;
        private int IconHeight = 10;
        private double CurrentTick = 0;
        private int LastTick = 0;


        private Timer playTimer = new Timer() { Interval = 4 };

        

        private struct NoteDataInfo
        {
            public int Tick;
            public int Lane;
            public Notedata.NoteType Type;

            public NoteDataInfo(int Tick, int Lane, Notedata.NoteType Type)
            {
                this.Tick = Tick;
                this.Lane = Lane;
                this.Type = Type;
            }
        }

        private Notedata.NoteType FindVisualNoteType(int tick, int lane)
        {
            if (tick >= chart.Length) return Notedata.NoteType.None;

            if (chart.Ticks[tick].Notes[lane] == Notedata.NoteType.Hold || chart.Ticks[tick].Notes[lane] == Notedata.NoteType.SimulHoldRelease)
            {
                if (tick == 0 || tick == chart.Length - 1) return chart.Ticks[tick].Notes[lane];
                if ((chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.Hold ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SimulHoldStart ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SimulHoldRelease ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SwipeLeftStartEnd ||
                    chart.Ticks[tick - 1].Notes[lane] == Notedata.NoteType.SwipeRightStartEnd) &&
                    chart.Ticks[tick + 1].Notes[lane] != Notedata.NoteType.None)
                    return Notedata.NoteType.ExtendHoldMid;
            }
            return chart.Ticks[tick].Notes[lane];
        }


        


        Image GetChartImage(double startTick, int tickHeight, int iconWidth, int iconHeight, Color BgCol, bool NoGrid, int Width, int Height)
        {
            Image Bmp = new Bitmap(Width, Height);
            Graphics Grfx = Graphics.FromImage(Bmp);

            int width = Bmp.Width;
            int height = Bmp.Height;

            Grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
            Grfx.Clear(BgCol);
            Grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            if (!NoGrid)
            {
                for (int i = 1; i < 8; i++)
                {
                    Grfx.FillRectangle(new SolidBrush(Color.LightGray), i * width / 8, 0, 1, height);
                }
            }
            


            float laneWidth = width / 8;
            float halfIconWidth = iconWidth / 2;
            int halfIconHeight = iconHeight / 2;

            for (int i = (int)startTick - 24; i < startTick + height / tickHeight; i++)
            {
                if (i >= chart.Length) break;
                if (i < 0) i = 0;

                if (i % 48 == 0)
                {
                    Grfx.FillRectangle(Brushes.SlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 2, width, 1);
                }

                for (int j = 0; j < 8; j++)
                {
                    Color noteCol = Color.FromArgb(0xc0, 0xc0, 0xc0);
                    Color ArrowCol = Color.Transparent;
                    int ArrowDir = 0;

                    Notedata.NoteType Type = FindVisualNoteType(i, j);

                    if (chart.swipeEnds[i * 8 + j] == 0)
                    {
                        Point swipeEndPoint = chart.swipeEndpointNodes[i * 8 + j];

                        if (swipeEndPoint.X > i)
                            Grfx.DrawLine(new Pen(Color.LightGray, iconWidth / 3), (float)(j + 0.5) * laneWidth, height - (float)(i - startTick + 1) * tickHeight - 2, (float)(swipeEndPoint.Y + 0.5) * laneWidth, height - (float)(swipeEndPoint.X - startTick + 1) * tickHeight - 2);
                            
                    }

                    switch (Type)
                    {
                        case Notedata.NoteType.Tap: noteCol = Color.Blue; break;
                        case Notedata.NoteType.Hold: noteCol = Color.LimeGreen; break;
                        case Notedata.NoteType.SimulTap:
                        case Notedata.NoteType.SimulHoldStart:
                        case Notedata.NoteType.SimulHoldRelease: noteCol = Color.DeepPink; break;
                        case Notedata.NoteType.FlickLeft: ArrowCol = Color.FromArgb(0x70, 0, 0x78); ArrowDir = -1; break;
                        case Notedata.NoteType.HoldEndFlickLeft: ArrowCol = Color.FromArgb(0x70, 0, 0x78); noteCol = Color.LightGray; ArrowDir = -1; break;
                        case Notedata.NoteType.SwipeLeftStartEnd: ArrowCol = Color.DarkViolet; ArrowDir = -1; break;
                        case Notedata.NoteType.SwipeLeftMid:
                        case Notedata.NoteType.SwipeChangeDirR2L: ArrowCol = Color.Violet; ArrowDir = -1; break;
                        case Notedata.NoteType.FlickRight: ArrowCol = Color.FromArgb(0xcc, 0x88, 0); ArrowDir = 1; break;
                        case Notedata.NoteType.HoldEndFlickRight: ArrowCol = Color.FromArgb(0xcc, 0x88, 0); noteCol = Color.LightGray; ArrowDir = 1; break;
                        case Notedata.NoteType.SwipeRightStartEnd: ArrowCol = Color.DarkOrange; ArrowDir = 1; break;
                        case Notedata.NoteType.SwipeRightMid:
                        case Notedata.NoteType.SwipeChangeDirL2R: ArrowCol = Color.Gold; ArrowDir = 1; break;
                        case Notedata.NoteType.ExtendHoldMid: noteCol = Color.LightGray; break;
                    }


                    if (chart.Ticks[i].Notes[j] != Notedata.NoteType.None)
                    {
                        int iconX = (int)((j + 0.5) * laneWidth - halfIconWidth);
                        int iconY = (int)Math.Ceiling(height - (i - startTick + 1.5) * tickHeight - 2);

                        Grfx.FillRectangle(new SolidBrush(noteCol), iconX, iconY, iconWidth, iconHeight);
                        if (ArrowDir == -1)
                            Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(iconX + iconWidth - 1, iconY + 0), new Point(iconX + iconWidth - 1, iconY + iconHeight - 1), new Point(iconX + 0, iconY + halfIconHeight) });
                        else if (ArrowDir == 1)
                            Grfx.FillPolygon(new SolidBrush(ArrowCol), new Point[] { new Point(iconX + 0, iconY + 0), new Point(iconX + 0, iconY + iconHeight - 1), new Point(iconX + iconWidth - 1, iconY + halfIconHeight) });
                    }
                }

                if (!NoGrid)
                {
                    if (i % 48 == 0)
                    {
                        Grfx.FillRectangle(Brushes.SlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 3, width, 3);
                        Grfx.DrawString((i / 48 + 1).ToString(), new System.Drawing.Font("Arial", 6.5f), Brushes.DarkSlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 13);
                    }
                    else if (i % 12 == 0)
                    {
                        Grfx.FillRectangle(Brushes.LightSlateGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 2, width, 1);
                    }
                    else if (i % 6 == 0)
                    {
                        Grfx.FillRectangle(Brushes.LightGray, 0, height - (float)(i - startTick + 0.5) * tickHeight - 2, width, 1);
                    }
                }
            }

            Grfx.Dispose();
            return Bmp;
        }

        Point GetPointAlongLine(Point start, Point end, float distance)
        {
            return new Point(start.X + (int)((end.X - start.X) * distance), start.Y + (int)((end.Y - start.Y) * distance));
        }
        PointF GetPointAlongLineF(Point start, Point end, float distance)
        {
            return new PointF(start.X + (end.X - start.X) * distance, start.Y + (end.Y - start.Y) * distance);
        }

        Bitmap spr_HoldLocus;
        Bitmap spr_SwipeLocus;
        Bitmap spr_TapIcon;
        Bitmap spr_HoldIcon;
        Bitmap spr_SimulIcon;
        Bitmap spr_SwipeRightIcon;
        Bitmap spr_SwipeRightIcon_Simul;
        Bitmap spr_SwipeLeftIcon;
        Bitmap spr_SwipeLeftIcon_Simul;
        Bitmap spr_HitEffect;
        Bitmap spr_Chara1;
        Bitmap spr_Chara2;
        Bitmap spr_Chara3;
        Bitmap spr_Chara4;
        Bitmap spr_Chara5;
        Bitmap spr_Chara6;
        Bitmap spr_Chara7;
        Bitmap spr_Chara8;

        Bitmap PArgbConverter(Image ImgIn)
        {
            Bitmap bmp = new Bitmap(ImgIn.Width, ImgIn.Height, PixelFormat.Format32bppPArgb);
            Graphics grfx = Graphics.FromImage(bmp);

            grfx.DrawImageUnscaled(ImgIn, 0, 0);

            grfx.Dispose();

            return bmp;
        }

        Bitmap PArgbConverter_Clip(Image ImgIn, Point[] cnrs, int scaleX, int scaleY)
        {
            Bitmap bmp = new Bitmap(ImgIn.Width * scaleX, ImgIn.Height * scaleY, PixelFormat.Format32bppPArgb);
            Graphics grfx = Graphics.FromImage(bmp);

            grfx.SetClip(new System.Drawing.Drawing2D.GraphicsPath(cnrs, new byte[] { (byte)System.Drawing.Drawing2D.PathPointType.Start, (byte)System.Drawing.Drawing2D.PathPointType.Line, (byte)System.Drawing.Drawing2D.PathPointType.Line, (byte)(System.Drawing.Drawing2D.PathPointType.Line|System.Drawing.Drawing2D.PathPointType.CloseSubpath) }));

            grfx.DrawImage(ImgIn, 0, 0, ImgIn.Width * scaleX, ImgIn.Height * scaleY);

            grfx.Dispose();

            return bmp;
        }

        void DrawGameClone(Graphics Grfx, double startTick, int numTicksVisible, int width, int height, int SpeedupMode)
        {
            float scalefactor = (float)width / 1136;

            Point[] NodeStartLocs = { new Point((int)(223 * scalefactor), (int)(77 * scalefactor)), new Point((int)(320 * scalefactor), (int)(100 * scalefactor)), new Point((int)(419 * scalefactor), (int)(114 * scalefactor)), new Point((int)(519 * scalefactor), (int)(119 * scalefactor)), new Point((int)(617 * scalefactor), (int)(119 * scalefactor)), new Point((int)(717 * scalefactor), (int)(114 * scalefactor)), new Point((int)(816 * scalefactor), (int)(100 * scalefactor)), new Point((int)(923 * scalefactor), (int)(77 * scalefactor)) };
            Point[] NodeEndLocs = { new Point((int)(75 * scalefactor), (int)(height - 156 * scalefactor)), new Point((int)(213 * scalefactor), (int)(height - 120 * scalefactor)), new Point((int)(354 * scalefactor), (int)(height - 98 * scalefactor)), new Point((int)(497 * scalefactor), (int)(height - 88 * scalefactor)), new Point((int)(639 * scalefactor), (int)(height - 88 * scalefactor)), new Point((int)(782 * scalefactor), (int)(height - 98 * scalefactor)), new Point((int)(923 * scalefactor), (int)(height - 120 * scalefactor)), new Point((int)(1061 * scalefactor), (int)(height - 156 * scalefactor)) };

            int iconSize = (int)(128 * scalefactor);

            Grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            Grfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            if (SpeedupMode > 0)
            {
                Grfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                Grfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                Grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            }
            else
            {
                Grfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                Grfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                Grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            }

            //Grfx.Clear(Color.Transparent);


            ColorMatrix transpMatrix = new ColorMatrix();
            transpMatrix.Matrix00 = 0.7f;
            transpMatrix.Matrix11 = 0.7f;
            transpMatrix.Matrix22 = 0.7f;
            transpMatrix.Matrix33 = 0.7f;
            ImageAttributes transpAttr = new ImageAttributes();
            transpAttr.SetColorMatrix(transpMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);



            int EffectTime = 1000000;
            int EffectFadeTime = 390000;
            double EffectTicks = ConvertTimeToTicks(new TimeSpan(EffectTime));
            double EffectFadeTicks = ConvertTimeToTicks(new TimeSpan(EffectFadeTime));

            ColorMatrix effectTranspMatrix = new ColorMatrix();
            ImageAttributes effectTranspAttr = new ImageAttributes();

            if (SpeedupMode < 2)
            {
                Grfx.DrawImage(spr_Chara1, NodeEndLocs[0].X - iconSize / 2, NodeEndLocs[0].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara2, NodeEndLocs[1].X - iconSize / 2, NodeEndLocs[1].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara3, NodeEndLocs[2].X - iconSize / 2, NodeEndLocs[2].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara4, NodeEndLocs[3].X - iconSize / 2, NodeEndLocs[3].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara5, NodeEndLocs[4].X - iconSize / 2, NodeEndLocs[4].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara6, NodeEndLocs[5].X - iconSize / 2, NodeEndLocs[5].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara7, NodeEndLocs[6].X - iconSize / 2, NodeEndLocs[6].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawImage(spr_Chara8, NodeEndLocs[7].X - iconSize / 2, NodeEndLocs[7].Y - iconSize / 2, iconSize, iconSize);
            }
            else
            {
                Pen Outline = new Pen(Color.Gray, 7*scalefactor);
                Grfx.DrawEllipse(Outline, NodeEndLocs[0].X - iconSize / 2, NodeEndLocs[0].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[1].X - iconSize / 2, NodeEndLocs[1].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[2].X - iconSize / 2, NodeEndLocs[2].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[3].X - iconSize / 2, NodeEndLocs[3].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[4].X - iconSize / 2, NodeEndLocs[4].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[5].X - iconSize / 2, NodeEndLocs[5].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[6].X - iconSize / 2, NodeEndLocs[6].Y - iconSize / 2, iconSize, iconSize);
                Grfx.DrawEllipse(Outline, NodeEndLocs[7].X - iconSize / 2, NodeEndLocs[7].Y - iconSize / 2, iconSize, iconSize);
            }


            for (int i = (int)startTick + numTicksVisible + 1; i >= (int)startTick; i--)
            //for (int i = (int)startTick; i < startTick + numTicksVisible + 24; i++)
            {
                if (i >= chart.Length) i = chart.Length - 1;
                if (i < 0) break;

                for (int j = 0; j < 8; j++)
                {
                    Notedata.NoteType Type = FindVisualNoteType(i, j);


                    if ((Type == Notedata.NoteType.SwipeRightStartEnd | Type == Notedata.NoteType.SwipeRightMid | Type == Notedata.NoteType.SwipeChangeDirL2R
                        | Type == Notedata.NoteType.SwipeLeftStartEnd | Type == Notedata.NoteType.SwipeLeftMid | Type == Notedata.NoteType.SwipeChangeDirR2L) && (chart.swipeEnds[i * 8 + j] == 0))
                    {
                        Point swipeEndPoint = chart.swipeEndpointNodes[i * 8 + j];

                        if (swipeEndPoint.X > i)
                        {
                            float iDist = (float)(numTicksVisible - i + startTick) / numTicksVisible;
                            float kDist = (float)(numTicksVisible - swipeEndPoint.X + startTick) / numTicksVisible;
                            int iSize = (int)(iconSize / 4 * iDist);
                            int kSize = (int)(iconSize / 4 * kDist);
                            Point iPoint = GetPointAlongLine(NodeStartLocs[j], NodeEndLocs[j], iDist);
                            Point kPoint = GetPointAlongLine(NodeStartLocs[swipeEndPoint.Y], NodeEndLocs[swipeEndPoint.Y], kDist);
                            Grfx.DrawImage(spr_SwipeLocus, new Point[] { new Point(iPoint.X, iPoint.Y - iSize), new Point(kPoint.X, kPoint.Y - kSize), new Point(iPoint.X, iPoint.Y - iSize + iconSize / 2) }, new Rectangle((int)(spr_SwipeLocus.Width - spr_SwipeLocus.Width * iDist), 0, (int)(spr_SwipeLocus.Width * (iDist - kDist)), spr_SwipeLocus.Height), GraphicsUnit.Pixel, transpAttr);
                        }
                    }


                    if (i > chart.Length) i = chart.Length;
                    if (i < 0) break;

                    if (Type == Notedata.NoteType.ExtendHoldMid && (i == (int)startTick | FindVisualNoteType(i - 1, j) != Notedata.NoteType.ExtendHoldMid))
                    {
                        int start = i;
                        if (start <= startTick) start = (int)startTick + 1;
                        int end = i;
                        while (FindVisualNoteType(end, j) == Notedata.NoteType.ExtendHoldMid) end++;
                        if (end <= start) continue;

                        float sDist = (float)(numTicksVisible - start + 1 + startTick) / numTicksVisible;
                        float eDist = (float)(numTicksVisible - end + startTick) / numTicksVisible;
                        float sSize = iconSize / 2 * sDist;
                        float eSize = iconSize / 2 * eDist;
                        PointF sPoint = GetPointAlongLineF(NodeStartLocs[j], NodeEndLocs[j], sDist);
                        PointF ePoint = GetPointAlongLineF(NodeStartLocs[j], NodeEndLocs[j], eDist);
                        Grfx.DrawImage(spr_HoldLocus, new PointF[] { new PointF(ePoint.X + eSize, ePoint.Y), new PointF(ePoint.X + eSize - iconSize, ePoint.Y), new PointF(sPoint.X + sSize, sPoint.Y) }, new Rectangle(0, (int)(spr_HoldLocus.Height * eDist), spr_HoldLocus.Width, (int)(spr_HoldLocus.Height * (sDist - eDist)) - 8), GraphicsUnit.Pixel, transpAttr);
                    }

                }
            }


            for (int i = (int)startTick + numTicksVisible; i >= (int)(startTick - EffectTicks - EffectFadeTicks - 1); i--)
            //for (int i = (int)(startTick - EffectTicks - EffectFadeTicks - 1); i <= (int)startTick + numTicksVisible; i++)
            {
                if (i >= chart.Length) i = chart.Length - 1;
                if (i < 0) break;

                for (int j = 0; j < 8; j++)
                {
                    Notedata.NoteType Type = FindVisualNoteType(i, j);

                    if (Type == Notedata.NoteType.None | Type == Notedata.NoteType.ExtendHoldMid) continue;

                    if (i >= (int)startTick)
                    {
                        Image NoteImg;

                        switch (Type)
                        {
                            case Notedata.NoteType.Tap: NoteImg = spr_TapIcon; break;
                            case Notedata.NoteType.Hold: NoteImg = spr_HoldIcon; break;
                            case Notedata.NoteType.SimulTap:
                            case Notedata.NoteType.SimulHoldStart:
                            case Notedata.NoteType.SimulHoldRelease: NoteImg = spr_SimulIcon; break;
                            case Notedata.NoteType.FlickLeft:
                            case Notedata.NoteType.HoldEndFlickLeft:
                            case Notedata.NoteType.SwipeLeftStartEnd:
                            case Notedata.NoteType.SwipeLeftMid:
                            case Notedata.NoteType.SwipeChangeDirR2L:
                                NoteImg = spr_SwipeLeftIcon;
                                for (int k = 0; k < 8; k++)
                                {
                                    if (chart.Ticks[i].Notes[k] == Notedata.NoteType.SimulTap | chart.Ticks[i].Notes[k] == Notedata.NoteType.SimulHoldStart | chart.Ticks[i].Notes[k] == Notedata.NoteType.SimulHoldRelease)
                                    {
                                        NoteImg = spr_SwipeLeftIcon_Simul;
                                        break;
                                    }
                                }
                                break;
                            case Notedata.NoteType.FlickRight:
                            case Notedata.NoteType.HoldEndFlickRight:
                            case Notedata.NoteType.SwipeRightStartEnd:
                            case Notedata.NoteType.SwipeRightMid:
                            case Notedata.NoteType.SwipeChangeDirL2R:
                                NoteImg = spr_SwipeRightIcon;
                                for (int k = 0; k < 8; k++)
                                {
                                    if (chart.Ticks[i].Notes[k] == Notedata.NoteType.SimulTap | chart.Ticks[i].Notes[k] == Notedata.NoteType.SimulHoldStart | chart.Ticks[i].Notes[k] == Notedata.NoteType.SimulHoldRelease)
                                    {
                                        NoteImg = spr_SwipeRightIcon_Simul;
                                        break;
                                    }
                                }
                                break;
                            default: NoteImg = new Bitmap(1, 1); break;
                        }

                        float icnDist = (float)(numTicksVisible - i + startTick) / numTicksVisible;
                        Point icnPoint = GetPointAlongLine(NodeStartLocs[j], NodeEndLocs[j], icnDist);
                        int icnSize = (int)(iconSize * 1.375f * icnDist);
                        Grfx.DrawImage(NoteImg, icnPoint.X - icnSize / 2, icnPoint.Y - icnSize / 2, icnSize, icnSize);

                    }
                    else if (i > (int)(startTick - EffectTicks - 1))
                    {
                        int effectSize = (int)(((startTick - i - 1) / EffectTicks + 1) * iconSize * 1.375f);
                        Grfx.DrawImage(spr_HitEffect, NodeEndLocs[j].X - effectSize / 2, NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize);
                    }
                    else if (i >= (int)(startTick - EffectTicks - EffectFadeTicks - 1) & SpeedupMode < 1)
                    {
                        int effectSize = (int)(iconSize * 2.75);
                        float effectOpacity = 1 - (float)((startTick - EffectTicks - i - 1) / EffectFadeTicks * 0.8f);

                        transpMatrix.Matrix00 = effectOpacity;
                        transpMatrix.Matrix11 = effectOpacity;
                        transpMatrix.Matrix22 = effectOpacity;
                        effectTranspMatrix.Matrix33 = effectOpacity;
                        effectTranspAttr.SetColorMatrix(effectTranspMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        Grfx.DrawImage(spr_HitEffect, new Rectangle((int)NodeEndLocs[j].X - effectSize / 2, (int)NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize), 0, 0, spr_HitEffect.Width, spr_HitEffect.Height, GraphicsUnit.Pixel, effectTranspAttr);
                    }
                }
            }
            
            transpAttr.Dispose();
        }

        Image GetGameCloneImage(double startTick, int numTicksVisible, int width, int height, int SpeedupMode)
        {
            Image Bmp = new Bitmap (width, height, PixelFormat.Format32bppPArgb);
            Graphics Grfx = Graphics.FromImage(Bmp);

            DrawGameClone(Grfx, startTick, numTicksVisible, width, height, SpeedupMode);
            
            Grfx.Dispose();

            return Bmp;
        }

        private void SetCurrTick(double tick)
        {
            if (tick < 0) tick = 0;
            if (tick >= chart.Length) tick = chart.Length - 1;

            CurrentTick = tick;

            if (Sound.MusicReader != null &&
                    (Sound.MusicReader.CurrentTime < ConvertTicksToTime(CurrentTick) - TimeSpan.FromMilliseconds(MusicDelayMs - 3) |
                    Sound.MusicReader.CurrentTime > ConvertTicksToTime(CurrentTick) - TimeSpan.FromMilliseconds(MusicDelayMs + 3)))
                try { Sound.MusicReader.CurrentTime = ConvertTicksToTime(CurrentTick) - TimeSpan.FromMilliseconds(MusicDelayMs); } catch { }

            ChartScrollBar.Value = (int)(chart.Length * TickHeight - tick * TickHeight);
        }


        int VideoDelayMs = 40;

        private void UpdateChart()
        {
            double tick = CurrentTick;
            if (playTimer.Enabled)
            {
                tick -= ConvertTimeToTicks(TimeSpan.FromMilliseconds(VideoDelayMs));
            }
            pictureBox1.Image.Dispose();
            pictureBox1.Image = GetChartImage(tick, TickHeight, IconWidth, IconHeight, SystemColors.ControlLight, false, pictureBox1.Width, pictureBox1.Height);
            if (Form2.Visible)
            {
                GameClone.Image.Dispose();
                GameClone.Image = GetGameCloneImage(tick, (int)ConvertTimeToTicks(TimeSpan.FromMilliseconds(700)), GameClone.Width, GameClone.Height, 2);
                //GameClone.BackColor = Color.Salmon;
            }
        }

        private int ConvertXCoordToNote(int X)
        {
            return ((X - pictureBox1.Location.X) / (pictureBox1.Width/8));
        }

        private double ConvertYCoordToTick(int Y)
        {
            return (pictureBox1.Location.Y + pictureBox1.Height - Y - 2 + CurrentTick%1 - TickHeight/2) / TickHeight + CurrentTick;
        }


        private TimeSpan ConvertTicksToTime(double ticks)
        {
            TimeSpan a = TimeSpan.FromSeconds((5 * ticks / chart.BPM));
            return TimeSpan.FromSeconds((5 * ticks / chart.BPM));
        }

        private double ConvertTimeToTicks(TimeSpan time)
        {
            return time.TotalSeconds / (double)(5/BPMbox.Value);
        }


        private void ResizeScrollbar()
        {
            ChartScrollBar.Minimum = 0;
            ChartScrollBar.Maximum = (int)(chart.Length * TickHeight + IconHeight / 2 + 110);
        }


        private void ResizeChart(int NewLen)
        {
            chart.Length = NewLen;
            ResizeScrollbar();
            SetCurrTick(CurrentTick);
            UpdateChart();
        }


        private void StartPlayback()
        {
            playTimer.Enabled = true;
            Sound.PlayMusic();
        }

        private void StopPlayback()
        {
            playTimer.Enabled = false;
            Sound.StopMusic();
            UpdateChart();
        }


        private void LoadChart(string Path)
        {
            StopPlayback();

            if (Path.Length > 0)
            {
                try {
                    chart = Notedata.ConvertJsonToChart(System.IO.File.ReadAllText(Path));
                }
                catch { MessageBox.Show(DialogResMgr.GetString("ChartLoadError")); }

                if (chart.BPM == 1)
                {
                    MessageBox.Show(DialogResMgr.GetString("ChartLoadNoBPM"));
                    chart.BPM = 120;
                }
                ResizeBox.Value = chart.Length / 48;
                BPMbox.Value = (decimal)chart.BPM;
                ResizeScrollbar();
                SetCurrTick(0);
                UpdateChart();

            }
        }


        private void AddNoteTypes()
        {
            NoteTypeSelector.Items.Clear();

            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                //NoteTypeSelector.DataSource = Enum.GetValues(typeof(Notedata.UserVisibleNoteType_Nihongo));
                foreach (Notedata.UserVisibleNoteType_Nihongo type in Enum.GetValues(typeof(Notedata.UserVisibleNoteType_Nihongo)))
                {
                    NoteTypeSelector.Items.Add(type);
                }
            else
                // NoteTypeSelector.DataSource = Enum.GetValues(typeof(Notedata.UserVisibleNoteType));
                foreach (Notedata.UserVisibleNoteType type in Enum.GetValues(typeof(Notedata.UserVisibleNoteType)))
                {
                    NoteTypeSelector.Items.Add(type);
                }

            NoteTypeSelector.SelectedIndex = 0;
        }

        PictureBox GameClone = new PictureBox{ Image = new Bitmap(853, 480), Size = new Size(853, 480), Location = new Point(0, 0), BackColor = Color.Black };
        Form Form2 = new Form() { ClientSize = new Size(853, 480) };
        public Form1()
        {
            InitializeComponent();



            Form2.Controls.Add(GameClone);
            Form2.Resize += new EventHandler(delegate (object sender, EventArgs e)
                            {
                                if (Form2.ClientSize.Height == 0 | Form2.ClientSize.Width == 0)
                                    GameClone.Size = new Size(1, 1);
                                else
                                    GameClone.Size = Form2.ClientSize;
                                GameClone.Image.Dispose();
                                GameClone.Image = new Bitmap(GameClone.Width, GameClone.Height);
                                UpdateChart();
                            });
            Form2.Show();

            
            try
            {
                spr_HoldLocus = (Bitmap)Image.FromFile("nodeimg/locus.png");
                spr_HoldLocus = PArgbConverter_Clip(spr_HoldLocus, new Point[] { new Point(0, 0), new Point(0, 0), new Point(spr_HoldLocus.Width*2, spr_HoldLocus.Height*32), new Point(0, spr_HoldLocus.Height*32) }, 2, 32);
                spr_SwipeLocus = (Bitmap)Image.FromFile("nodeimg/locus2.png");
                spr_SwipeLocus = PArgbConverter_Clip(spr_SwipeLocus, new Point[] { new Point(0, 0), new Point(spr_SwipeLocus.Width * 64, 0), new Point(spr_SwipeLocus.Width*64, 0), new Point(0, spr_SwipeLocus.Height*2) }, 64, 2);
                spr_TapIcon = PArgbConverter(Image.FromFile("nodeimg/node_1.png"));
                spr_HoldIcon = PArgbConverter(Image.FromFile("nodeimg/node_2.png"));
                spr_SimulIcon = PArgbConverter(Image.FromFile("nodeimg/node_3.png"));
                spr_SwipeRightIcon = PArgbConverter(Image.FromFile("nodeimg/node_4.png"));
                spr_SwipeRightIcon_Simul = PArgbConverter(Image.FromFile("nodeimg/node_4_3.png"));
                spr_SwipeLeftIcon = PArgbConverter(Image.FromFile("nodeimg/node_6.png"));
                spr_SwipeLeftIcon_Simul = PArgbConverter(Image.FromFile("nodeimg/node_6_3.png"));
                spr_HitEffect = PArgbConverter(Image.FromFile("nodeimg/node_effect.png"));
                spr_Chara1 = PArgbConverter(Image.FromFile("charaimg/1.png"));
                spr_Chara2 = PArgbConverter(Image.FromFile("charaimg/2.png"));
                spr_Chara3 = PArgbConverter(Image.FromFile("charaimg/3.png"));
                spr_Chara4 = PArgbConverter(Image.FromFile("charaimg/4.png"));
                spr_Chara5 = PArgbConverter(Image.FromFile("charaimg/5.png"));
                spr_Chara6 = PArgbConverter(Image.FromFile("charaimg/6.png"));
                spr_Chara7 = PArgbConverter(Image.FromFile("charaimg/7.png"));
                spr_Chara8 = PArgbConverter(Image.FromFile("charaimg/8.png"));
            }
            catch
            {
                spr_HoldLocus = new Bitmap(1, 1);
                spr_SwipeLocus = new Bitmap(1, 1);
                spr_TapIcon = new Bitmap(1, 1);
                spr_HoldIcon = new Bitmap(1, 1);
                spr_SimulIcon = new Bitmap(1, 1);
                spr_SwipeRightIcon = new Bitmap(1, 1);
                spr_SwipeRightIcon_Simul = new Bitmap(1, 1);
                spr_SwipeLeftIcon = new Bitmap(1, 1);
                spr_SwipeLeftIcon_Simul = new Bitmap(1, 1);
                spr_HitEffect = new Bitmap(1, 1);
                spr_Chara1 = new Bitmap(1, 1);
                spr_Chara2 = new Bitmap(1, 1);
                spr_Chara3 = new Bitmap(1, 1);
                spr_Chara4 = new Bitmap(1, 1);
                spr_Chara5 = new Bitmap(1, 1);
                spr_Chara6 = new Bitmap(1, 1);
                spr_Chara7 = new Bitmap(1, 1);
                spr_Chara8 = new Bitmap(1, 1);
            }

            try
            {
                Sound.NoteSoundWave = new Sound.CachedSound("notesnd/hit.wav");
                Sound.NoteSoundWave_Swipe = new Sound.CachedSound("notesnd/swipe.wav");
                //NoteSoundMixer.AddMixerInput(NoteSoundWave);
                //NoteSoundMixer.AddMixerInput(NoteSoundWave_Swipe);
                //Sound.SetNoteSoundLatency(95);
            }
            catch
            {
                Sound.NoteSoundWave = null;
                Sound.NoteSoundWave_Swipe = null;
            }


            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            AddNoteTypes();

            ActiveControl = ZoomLbl;

            Sound.InitWaveOut();

            ResizeScrollbar();
            SetCurrTick(0);
            UpdateChart();

            playTimer.Tick += playtimer_Tick;
        }

        private void ChartScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (PauseOnSeek.Checked) StopPlayback();
            SetCurrTick(chart.Length - e.NewValue / TickHeight);
            UpdateChart();
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            if (Sound.MusicReader != null)
                StartPlayback();
            else
            {
                MessageBox.Show(DialogResMgr.GetString("PlaybackNoMusicError"));
            }
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            StopPlayback();
        }

        int MusicDelayMs = 20;

        double lastPlayTickTime = 0;
        private void playtimer_Tick(object sender, EventArgs e)
        {
            SetCurrTick(ConvertTimeToTicks(Sound.MusicReader.CurrentTime + TimeSpan.FromMilliseconds(MusicDelayMs)));
            if (lastPlayTickTime < ConvertTicksToTime(CurrentTick).TotalMilliseconds - 12 | lastPlayTickTime > ConvertTicksToTime(CurrentTick).TotalMilliseconds)
            {
                lastPlayTickTime = ConvertTicksToTime(CurrentTick).TotalMilliseconds;
                UpdateChart();
            }

            if ((int)CurrentTick != LastTick)
            {
                int ltick = LastTick;
                LastTick = (int)CurrentTick;

                if (NoteSoundBox.Checked)
                {
                    for (int i = ltick + 1; i <= CurrentTick; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            Notedata.NoteType note = FindVisualNoteType(i, j);

                            if (Sound.NoteSoundWave != null)
                            {
                                if (note == Notedata.NoteType.Tap || note == Notedata.NoteType.SimulTap || note == Notedata.NoteType.Hold
                                    || note == Notedata.NoteType.SimulHoldStart || note == Notedata.NoteType.SimulHoldRelease)
                                {
                                    //Sound.PlayNoteSound(Sound.NoteSoundWave);
                                    Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(new Sound.CachedSoundSampleProvider(Sound.NoteSoundWave));
                                    if (MusicDelayMs > 30)
                                        Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs - 30);
                                    else
                                        Sound.NoteSoundTrim.SkipOver = TimeSpan.FromMilliseconds(30 - MusicDelayMs);
                                    Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                }
                                else if (((note == Notedata.NoteType.SwipeLeftStartEnd | note == Notedata.NoteType.SwipeRightStartEnd)
                                    && chart.swipeEnds[i * 8 + j] == 0)
                                    || note == Notedata.NoteType.FlickLeft | note == Notedata.NoteType.HoldEndFlickLeft
                                    || note == Notedata.NoteType.FlickRight | note == Notedata.NoteType.HoldEndFlickRight)
                                {
                                    //Sound.PlayNoteSound(Sound.NoteSoundWave_Swipe);
                                    Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(new Sound.CachedSoundSampleProvider(Sound.NoteSoundWave_Swipe));
                                    if (MusicDelayMs > 30)
                                        Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs - 30);
                                    else
                                        Sound.NoteSoundTrim.SkipOver = TimeSpan.FromMilliseconds(30 - MusicDelayMs);
                                    Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                }
                            }

                            else
                            {
                                if (note != Notedata.NoteType.None && note != Notedata.NoteType.ExtendHoldMid &&
                                    note != Notedata.NoteType.SwipeLeftMid && note != Notedata.NoteType.SwipeRightMid)
                                {
                                    Sound.NoteSoundTrim = new NAudio.Wave.SampleProviders.OffsetSampleProvider(Sound.NoteSoundSig);
                                    Sound.NoteSoundTrim.Take = TimeSpan.FromMilliseconds(20);
                                    Sound.NoteSoundTrim.DelayBy = TimeSpan.FromMilliseconds(MusicDelayMs + 5);
                                    Sound.PlayNoteSound(Sound.NoteSoundTrim);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (CurrentTick == chart.Length - 1 || Sound.MusicReader.CurrentTime == Sound.MusicReader.TotalTime)
            {
                StopPlayback();
            }
        }

        private void BPMbox_ValueChanged(object sender, EventArgs e)
        {
            chart.BPM = (double)BPMbox.Value;
            ResizeScrollbar();
            if (Sound.MusicReader != null)
            {
                SetCurrTick(ConvertTimeToTicks(Sound.MusicReader.CurrentTime + TimeSpan.FromMilliseconds(MusicDelayMs)));
                UpdateChart();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Height != Height)
            {
                if (ClientSize.Height == 0)
                    pictureBox1.Height = 1;
                else
                    pictureBox1.Height = ClientSize.Height;
                Image bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                pictureBox1.Image = bmp;
                UpdateChart();
            }
        }

        private void ProcessClick(int Tick, int Lane, MouseButtons MouseButton, Notedata.NoteType NewNote)
        {
            Console.WriteLine(Lane + ", " + Tick);

            if (Tick == -1 | Tick >= chart.Length)
                return;

            if (Lane < 0 | Lane > 7)
                return;

            if (MouseButton == MouseButtons.Left)
            {
                if (chart.Ticks[Tick].Notes[Lane] != NewNote)
                {
                    if (NewNote == Notedata.NoteType.None)
                    {
                        ProcessClick(Tick, Lane, MouseButtons.Right, NewNote);
                        return;
                    }

                    chart.Ticks[Tick].SetNote(NewNote, Lane, ref chart);
                    UpdateChart();
                }

            }

            else if (MouseButton == MouseButtons.Right)
            {
                if (chart.Ticks[Tick].Notes[Lane] != Notedata.NoteType.None)
                {
                    chart.Ticks[Tick].SetNote(Notedata.NoteType.None, Lane, ref chart);
                    UpdateChart();
                }
            }
        }

        private void Chart_Click(object sender, MouseEventArgs e)
        {
            Control sendCtl = (Control)sender;
            sendCtl.Capture = false;

            pictureBox1.Focus();

            int Lane = ConvertXCoordToNote(e.X);
            int Tick = (int)ConvertYCoordToTick(e.Y);

            ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left | e.Button == MouseButtons.Right)
            {
                int Lane = ConvertXCoordToNote(e.X);
                int Tick = (int)ConvertYCoordToTick(e.Y);

                ProcessClick(Tick, Lane, e.Button, (Notedata.NoteType)NoteTypeSelector.SelectedItem);
            }
        }

        private void Chart_MouseWheel(object sender, MouseEventArgs e)
        {
            SetCurrTick(CurrentTick + e.Delta / 15);
            UpdateChart();
        }


        private void ZoomBox_ValueChanged(object sender, EventArgs e)
        {
            TickHeight = (int)ZoomBox.Value;
            IconHeight = TickHeight;
            ResizeScrollbar();
            UpdateChart();
        }

        private void ResizeBtn_Click(object sender, EventArgs e)
        {
            ResizeChart((int)ResizeBox.Value * 48);
            UpdateChart();
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
             LoadChart(openFileDialog1.FileName);
        }

        private void SaveChartBtn_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                System.IO.File.WriteAllText(saveFileDialog1.FileName, Notedata.ConvertChartToJson(chart));
        }

        private void OpenMusicButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
                Sound.LoadMusic(openFileDialog2.FileName);
        }

        private void ImgSaveBtn_Click(object sender, EventArgs e)
        {
            int TicksPerCol = 48 * 8; //8 bars
            int TickHeight = 1;
            int ColWidth = 16;
            int NoteHeight = 1;
            int NoteWidth = 2;

            int NumCols = (chart.Ticks.Length - 1) / TicksPerCol + 1;

            Bitmap img = new Bitmap(NumCols * ColWidth + NumCols - 1, TicksPerCol * NoteHeight);
            Graphics grfx = Graphics.FromImage(img);

            grfx.Clear(SystemColors.ControlDark);

            for (int i = 0; i < NumCols; i++)
            {
                grfx.DrawImage(GetChartImage(i * TicksPerCol, TickHeight, NoteWidth, NoteHeight, SystemColors.ControlLight, true, ColWidth, TicksPerCol * NoteHeight + 1), i + i * ColWidth, 0);
            }

            img.Save("imgout.png");
            
            grfx.Dispose();
            img.Dispose();
        }

        private void NoteShiftBtn_Click(object sender, EventArgs e)
        {
            if (NoteShiftBox.Value > 0)
            {
                List<Notedata.Tick> NewTicks = chart.Ticks.ToList();
                NewTicks.RemoveRange(chart.Length - (int)NoteShiftBox.Value, (int)NoteShiftBox.Value);
                NewTicks.InsertRange(0, new Notedata.Tick[(int)NoteShiftBox.Value]);
                chart.Ticks = NewTicks.ToArray();

                ResizeScrollbar();
                chart.FixSwipes();
                UpdateChart();
            }

            else if (NoteShiftBox.Value < 0)
            {
                List<Notedata.Tick> NewTicks = chart.Ticks.ToList();
                NewTicks.RemoveRange(0, - (int)NoteShiftBox.Value);
                NewTicks.AddRange(new Notedata.Tick[- (int)NoteShiftBox.Value]);
                chart.Ticks = NewTicks.ToArray();

                ResizeScrollbar();
                chart.FixSwipes();
                UpdateChart();
            }

            NoteShiftBox.Value = 0;


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            if (MessageBox.Show(DialogResMgr.GetString("ExitMessage"), DialogResMgr.GetString("ExitCaption"), MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
        }

        private void NoteCountButton_Click(object sender, EventArgs e)
        {
            int NoteCount = 0;

            for (int i = 0; i < chart.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                    if (NoteType != Notedata.NoteType.None && NoteType != Notedata.NoteType.ExtendHoldMid &&
                        NoteType != Notedata.NoteType.SwipeLeftMid && NoteType != Notedata.NoteType.SwipeRightMid)
                        NoteCount++;
                }
            }

            MessageBox.Show(String.Format(DialogResMgr.GetString("NoteCountMessage"), NoteCount));
        }

        private void AutoSimulBtn_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chart.Length; i++)
            {
                int SimulNum_Tap = 0;
                int SimulNum_Hold = 0;

                for (int j = 0; j < 8; j++)
                {
                    // taps get drawn as simulnotes when swipes or flicks are present, but holds don't
                    Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                    if (NoteType != Notedata.NoteType.None && NoteType != Notedata.NoteType.ExtendHoldMid &&
                        NoteType != Notedata.NoteType.SwipeLeftMid && NoteType != Notedata.NoteType.SwipeRightMid)
                        SimulNum_Tap++;

                    if (NoteType == Notedata.NoteType.Tap || NoteType == Notedata.NoteType.SimulTap ||
                        NoteType == Notedata.NoteType.Hold || NoteType == Notedata.NoteType.SimulHoldStart || NoteType == Notedata.NoteType.SimulHoldRelease)
                        SimulNum_Hold++;
                }

                if (SimulNum_Tap > 1)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.Tap)
                        {
                            chart.Ticks[i].SetNote(Notedata.NoteType.SimulTap, j, ref chart);

                            UpdateChart();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.SimulTap)
                        {
                            chart.Ticks[i].SetNote(Notedata.NoteType.Tap, j, ref chart);

                            UpdateChart();
                        }
                    }
                }

                if (SimulNum_Hold > 1)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.Hold || NoteType == Notedata.NoteType.SimulHoldStart
                        || NoteType == Notedata.NoteType.SimulHoldRelease)
                        {
                            if (i + 1 < chart.Length && (chart.Ticks[i + 1].Notes[j] == Notedata.NoteType.Hold || chart.Ticks[i + 1].Notes[j] == Notedata.NoteType.SimulHoldRelease))
                                chart.Ticks[i].SetNote(Notedata.NoteType.SimulHoldStart, j, ref chart);
                            else
                                chart.Ticks[i].SetNote(Notedata.NoteType.SimulHoldRelease, j, ref chart);

                            UpdateChart();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Notedata.NoteType NoteType = FindVisualNoteType(i, j);
                        if (NoteType == Notedata.NoteType.SimulHoldStart || NoteType == Notedata.NoteType.SimulHoldRelease)
                        {
                            chart.Ticks[i].SetNote(Notedata.NoteType.Hold, j, ref chart);

                            UpdateChart();
                        }
                    }
                }
            }
        }

        private void LangChangeBtn_Click(object sender, EventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ja")
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
            else
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ja");


            SuspendLayout();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            resources.ApplyResources(this, "$this");
            foreach (Control Ctrl in Controls)
                resources.ApplyResources(Ctrl, Ctrl.Name);
            ResumeLayout();

            AddNoteTypes();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                char key = e.KeyChar;
                if (Char.IsDigit(key))
                {
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType)Enum.Parse(typeof(Notedata.NoteShortcutKeys), "_" + key);
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType_Nihongo)Enum.Parse(typeof(Notedata.NoteShortcutKeys), "_" + key);
                }
                else
                {
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType)Enum.Parse(typeof(Notedata.NoteShortcutKeys), key.ToString().ToUpper());
                    NoteTypeSelector.SelectedItem = (Notedata.UserVisibleNoteType_Nihongo)Enum.Parse(typeof(Notedata.NoteShortcutKeys), key.ToString().ToUpper());
                }
            }
            catch { }
        }
    }
}
