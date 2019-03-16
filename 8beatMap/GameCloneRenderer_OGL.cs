using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _8beatMap
{
    class GameCloneRenderer_OGL
    {
        private GameWindow myWindow = null;
        private System.Diagnostics.Stopwatch FrameStopwatch = new System.Diagnostics.Stopwatch();

        public double currentTick = 0;
        public int numTicksVisible = 24;

        private float timingAdjust = 0;
        private int hitlineAdjust = 1;
        
        private Form1 mainform = null;

        private Skinning.Skin skin = Skinning.DefaultSkin;

        static System.Collections.Generic.Dictionary<string, int> textures = new System.Collections.Generic.Dictionary<string, int>();

        public Color clearColor = Color.FromArgb(0, 0, 0, 0);

        public Point WindowSize
        {
            get
            {
                if (myWindow != null)
                    return new Point(myWindow.Width, myWindow.Height);
                else
                    return new Point(0, 0);
            }
        }
        public Point WindowLocation
        {
            get
            {
                if (myWindow != null)
                    return myWindow.Location;
                else
                    return new Point(-99999, -99999);
            }
        }
        public WindowState WindowState
        {
            get
            {
                if (myWindow != null)
                    return myWindow.WindowState;
                else
                    return WindowState.Normal;
            }
        }


        Point[] NodeStartLocs_raw = { new Point(223, 77), new Point(320, 100), new Point(419, 114), new Point(519, 119), new Point(617, 119), new Point(717, 114), new Point(816, 100), new Point(923, 77) };
        Point[] NodeStartLocs = { new Point(223, 640 - 77), new Point(320, 640 - 100), new Point(419, 640 - 114), new Point(519, 640 - 119), new Point(617, 640 - 119), new Point(717, 640 - 114), new Point(816, 640 - 100), new Point(923, 640 - 77) };
        Point[] NodeEndLocs = { new Point(75, 156), new Point(213, 120), new Point(354, 98), new Point(497, 88), new Point(639, 88), new Point(782, 98), new Point(923, 120), new Point(1061, 156) };
        int numLanes = 8;


        private static System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());
        private static System.Resources.ResourceManager IconResMgr = new System.Resources.ResourceManager("_8beatMap.GameCloneRenderer_OGL", System.Reflection.Assembly.GetEntryAssembly());


        private void SetupNodeLocs(int wndWidth, int wndHeight)
        {
            viewHeight = wndHeight * 1136 / wndWidth;
            NodeStartLocs = (Point[])skin.NodeStartLocs.Clone();
            for (int i = 0; i < NodeStartLocs.Length; i++)
            {
                NodeStartLocs[i].Y = viewHeight - NodeStartLocs[i].Y;
            }
        }

        private void SetupTextures()
        {
            foreach (System.Collections.Generic.KeyValuePair<string, int> tex in textures)
            {
                UnloadTexture(tex.Value);
            }
            textures.Clear();

            foreach (System.Collections.Generic.KeyValuePair<string, string> tex in skin.TexturePaths)
            {
                if (!textures.ContainsKey(tex.Key))
                    textures.Add(tex.Key, LoadTexture(tex.Value));
                else
                    textures[tex.Key] = LoadTexture(tex.Value);
            }

            for (int i = 0; i < skin.ComboFont.PageTexPaths.Length; i++)
            {
                if (skin.ComboFont.PageTexPaths[i] == null) continue;

                string texkey = "combofont_" + i.ToString();
                string texpath = skin.ComboFont.PageTexPaths[i];
                if (!textures.ContainsKey(texkey))
                    textures.Add(texkey, LoadTexture(texpath));
                else
                    textures[texkey] = LoadTexture(texpath);
            }
        }

        public GameCloneRenderer_OGL(int wndWidth, int wndHeight, int wndX, int wndY, WindowState wndState, Form1 mainform, Skinning.Skin skin)
        {
            this.mainform = mainform;
            this.skin = skin;
            NodeStartLocs = (Point[])skin.NodeStartLocs.Clone();
            NodeEndLocs = (Point[])skin.NodeEndLocs.Clone();
            SetupNodeLocs(wndWidth, wndHeight);

            System.Threading.Thread oglThread = new System.Threading.Thread(() =>
            {
                myWindow = new GameWindow(wndWidth, wndHeight, OpenTK.Graphics.GraphicsMode.Default, "8beatMap Preview Window") { Icon = (Icon)IconResMgr.GetObject("Icon") };
                //myWindow = new GameWindow(wndWidth, wndHeight, new OpenTK.Graphics.GraphicsMode(32, 0, 0, 4), "8beatMap Preview Window") { Icon = (Icon)IconResMgr.GetObject("Icon") };
                if ((wndX != -99999 | wndY != -99999) && wndState != WindowState.Maximized)
                {
                    myWindow.X = wndX;
                    myWindow.Y = wndY;
                }
                myWindow.WindowState = wndState;
                myWindow.VSync = OpenTK.VSyncMode.Adaptive;


                myWindow.Load += (sender, e) =>
                {
                    SetupTextures();

                    GL.ClearColor(clearColor);

                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.Blend);
                    //GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                    GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                    //GL.Enable(EnableCap.PolygonSmooth);  // sometimes causes diagonal lines through quads

                    GL.BindTexture(TextureTarget.Texture2D, textures["spr_SwipeLocus"]);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.BindTexture(TextureTarget.Texture2D, textures["spr_HoldLocus"]);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                };

                myWindow.Resize += (sender, e) =>
                {
                    if (myWindow != null)
                    {
                        GL.Viewport(0, 0, myWindow.Width, myWindow.Height);
                        SetupNodeLocs(myWindow.Width, myWindow.Height);
                    }
                };


                myWindow.RenderFrame += new EventHandler<OpenTK.FrameEventArgs>(RenderFrame);


                //myWindow.TargetRenderFrequency = 60;
                myWindow.Run();
            });

            System.Threading.Thread.Sleep(10);

            oglThread.Start();

            while (myWindow == null)
                System.Threading.Thread.Yield();
        }

        public void Stop()
        {
            if (myWindow != null)
            {
                myWindow.Close();
                //myWindow.Dispose(); //disposing seems to cause issues when calling run later.. in a new instance of the class...???  I don't understand this..
                myWindow = null;      //fortunately it isn't a huge memory leak
            }
        }



        Matrix4 ProjMatrix;
        
        int iconSize = 352;
        int halfIconSize = 176;
        //int holdSize = 130;
        int halfHoldSize = 65;
        //int swipeSize = 66;
        int halfSwipeSize = 33;

        int EffectTime = 1000000;
        int EffectFadeTime = 390000;

        int viewHeight = 640;


        void RenderFrame(object sender, EventArgs e)
        {
            if (myWindow == null || !myWindow.Exists) return;
            if (myWindow.WindowState == WindowState.Minimized) return;

            if (mainform == null) return;
            
            FrameStopwatch.Restart();

            mainform.UpdateGameCloneChart();
            
            Notedata.Chart chart = mainform.chart; // doesn't seem necessary for our use case, but if I want to eliminate warnings it should be here

            double EffectTicks = chart.ConvertTimeToTicks(new TimeSpan(EffectTime));
            double EffectFadeTicks = chart.ConvertTimeToTicks(new TimeSpan(EffectFadeTime));


            GL.ClearColor(clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            ProjMatrix = Matrix4.CreateOrthographicOffCenter(0, 1136, 0, viewHeight, 0, 2);
            GL.LoadMatrix(ref ProjMatrix);



            DrawFilledRect(NodeEndLocs[0].X - halfIconSize, NodeEndLocs[0].Y - halfIconSize, iconSize, iconSize, "spr_Chara1");
            DrawFilledRect(NodeEndLocs[1].X - halfIconSize, NodeEndLocs[1].Y - halfIconSize, iconSize, iconSize, "spr_Chara2");
            DrawFilledRect(NodeEndLocs[2].X - halfIconSize, NodeEndLocs[2].Y - halfIconSize, iconSize, iconSize, "spr_Chara3");
            DrawFilledRect(NodeEndLocs[3].X - halfIconSize, NodeEndLocs[3].Y - halfIconSize, iconSize, iconSize, "spr_Chara4");
            DrawFilledRect(NodeEndLocs[4].X - halfIconSize, NodeEndLocs[4].Y - halfIconSize, iconSize, iconSize, "spr_Chara5");
            DrawFilledRect(NodeEndLocs[5].X - halfIconSize, NodeEndLocs[5].Y - halfIconSize, iconSize, iconSize, "spr_Chara6");
            DrawFilledRect(NodeEndLocs[6].X - halfIconSize, NodeEndLocs[6].Y - halfIconSize, iconSize, iconSize, "spr_Chara7");
            DrawFilledRect(NodeEndLocs[7].X - halfIconSize, NodeEndLocs[7].Y - halfIconSize, iconSize, iconSize, "spr_Chara8");



            GL.Color4(0.65f, 0.65f, 0.65f, 0.65f); //transparency

            for (int i = (int)currentTick + numTicksVisible + 1; i >= (int)currentTick - 48; i--) // 48 is magic from Notedata.Chart.UpdateSwipeEnd()
            {
                if (i >= chart.Length) i = chart.Length - 1;
                if (i < 0) break;

                for (int j = 0; j < numLanes; j++)
                {
                    NoteTypes.NoteTypeDef Type = chart.FindVisualNoteType(i, j);
                    
                    GL.BindTexture(TextureTarget.Texture2D, textures["spr_SwipeLocus"]);

                    if ((Type.DetectType == NoteTypes.DetectType.SwipeEndPoint | Type.DetectType == NoteTypes.DetectType.SwipeMid | Type.DetectType == NoteTypes.DetectType.SwipeDirChange) && !chart.Ticks[i].Notes[j].IsSwipeEnd)
                    {
                        Point swipeEndPoint = chart.Ticks[i].Notes[j].SwipeEndPoint;

                        if (swipeEndPoint.X > i & currentTick-timingAdjust < swipeEndPoint.X & swipeEndPoint.Y < numLanes)
                        {
                            int sTick = i;
                            if (sTick < currentTick) sTick = (int)currentTick;

                            float iDist = (float)(numTicksVisible - i + currentTick-timingAdjust) / numTicksVisible;
                            float eDist = (float)(numTicksVisible - swipeEndPoint.X + currentTick-timingAdjust) / numTicksVisible;
                            float eSize = halfSwipeSize * eDist;
                            PointF iPoint = GetPointAlongLineF(NodeStartLocs[j], NodeEndLocs[j], iDist);
                            PointF ePoint = GetPointAlongLineF(NodeStartLocs[swipeEndPoint.Y], NodeEndLocs[swipeEndPoint.Y], eDist);
                            
                            float endYOffset = 88888888;
                            if (ePoint.X - NodeStartLocs[swipeEndPoint.Y].X != 0) { endYOffset = ((float)(ePoint.Y - NodeStartLocs[swipeEndPoint.Y].Y) / (float)(ePoint.X - NodeStartLocs[swipeEndPoint.Y].X)); }
                            float angledSizeScale = 1 / (float)Math.Sqrt((Math.Pow(1, 2) + Math.Pow(endYOffset, 2)));
                            

                            float sDist;
                            PointF sPoint;
                            if (i >= currentTick-timingAdjust)
                            {
                                sDist = iDist;
                                sPoint = iPoint;
                             }
                            else
                            {
                                sDist = (float)(numTicksVisible - sTick + currentTick-timingAdjust) / numTicksVisible;
                                sPoint = GetPointAlongLineF(iPoint, ePoint, ((float)currentTick-timingAdjust - i) / (swipeEndPoint.X - i));
                                //Point sPoint = GetPointAlongLine(ePoint, iPoint, sDist / iDist);
                            }

                            float sSize = halfSwipeSize * sDist;


                            GL.Begin(PrimitiveType.Quads);

                            GL.TexCoord2(0, 0);
                            GL.Vertex2(sPoint.X + sSize*angledSizeScale, sPoint.Y + sSize*endYOffset*angledSizeScale);

                            GL.TexCoord2(32, 0);
                            GL.Vertex2(ePoint.X + eSize*angledSizeScale, ePoint.Y + eSize*endYOffset*angledSizeScale);

                            GL.TexCoord2(32, eDist);
                            GL.Vertex2(ePoint.X - eSize*angledSizeScale, ePoint.Y - eSize*endYOffset*angledSizeScale);
                            
                            GL.TexCoord2(0, sDist);
                            GL.Vertex2(sPoint.X - sSize*angledSizeScale, sPoint.Y - sSize*endYOffset*angledSizeScale);

                            GL.End();
                        }
                    }



                    if (i < (int)currentTick-1) continue;

                    GL.BindTexture(TextureTarget.Texture2D, textures["spr_HoldLocus"]);

                    if (Type.DetectType == NoteTypes.DetectType.HoldMid && (i == (int)currentTick-1 | chart.FindVisualNoteType(i - 1, j).DetectType != NoteTypes.DetectType.HoldMid))
                    {
                        float start = i;
                        if (start < currentTick + 1 - timingAdjust) start = (int)((currentTick + 1 - timingAdjust) * 4) / 4f;
                        int end = i;
                        while (chart.FindVisualNoteType(end, j).DetectType == NoteTypes.DetectType.HoldMid) end++;
                        if (end <= start-1) continue;

                        float sDist = (float)(numTicksVisible - start + 1 + currentTick-timingAdjust) / numTicksVisible;
                        float eDist = (float)(numTicksVisible - end + currentTick-timingAdjust) / numTicksVisible;
                        if (eDist < 0)
                            eDist = 0;
                        float sSize = halfHoldSize * sDist;
                        float eSize = halfHoldSize * eDist;
                        float halfsSize = sSize / 2f;
                        float halfeSize = eSize / 2f;
                        PointF sPoint = GetPointAlongLineF(NodeStartLocs[j], NodeEndLocs[j], sDist);
                        PointF ePoint = GetPointAlongLineF(NodeStartLocs[j], NodeEndLocs[j], eDist);
                        //Grfx.DrawImage(spr_HoldLocus, new PointF[] { new PointF(ePoint.X + eSize, ePoint.Y), new PointF(ePoint.X + eSize - iconSize, ePoint.Y), new PointF(sPoint.X + sSize, sPoint.Y) }, new Rectangle(0, (int)(spr_HoldLocus.Height * eDist), spr_HoldLocus.Width, (int)(spr_HoldLocus.Height * (sDist - eDist)) - 8), GraphicsUnit.Pixel, transpAttr);

                        float endYOffset = 88888888;
                        if (sPoint.Y - NodeStartLocs[j].Y != 0) { endYOffset = ((float)(sPoint.X - NodeStartLocs[j].X) / (float)(sPoint.Y - NodeStartLocs[j].Y)); }
                        float angledSizeScale = 1/(float)Math.Sqrt((Math.Pow(1, 2) + Math.Pow(endYOffset, 2)));

                        GL.Begin(PrimitiveType.Quads);

                        GL.TexCoord2(0, (eDist - sDist) * numTicksVisible * 2);
                        GL.Vertex2(ePoint.X + eSize*angledSizeScale, ePoint.Y - eSize*endYOffset*angledSizeScale);

                        GL.TexCoord2(0, 0);
                        GL.Vertex2(sPoint.X + sSize*angledSizeScale, sPoint.Y - sSize*endYOffset*angledSizeScale);

                        GL.TexCoord2(sDist, 0);
                        GL.Vertex2(sPoint.X - sSize*angledSizeScale, sPoint.Y + sSize*endYOffset*angledSizeScale);

                        GL.TexCoord2(eDist, (eDist - sDist) * numTicksVisible * 2);
                        GL.Vertex2(ePoint.X - eSize*angledSizeScale, ePoint.Y + eSize*endYOffset*angledSizeScale);

                        GL.End();
                    }
                }
            }

            GL.Color4(1f, 1f, 1f, 1f);


            for (int i = (int)currentTick + numTicksVisible; i >= (int)(currentTick - EffectTicks - EffectFadeTicks - 1); i--)
            //for (int i = (int)(startTick - EffectTicks - EffectFadeTicks - 1); i <= (int)startTick + numTicksVisible; i++)
            {
                if (i >= chart.Length) i = chart.Length - 1;
                if (i < 0) break;

                for (int j = 0; j < numLanes; j++)
                {
                    NoteTypes.NoteTypeDef Type = chart.FindVisualNoteType(i, j);

                    if (Type.DetectType == NoteTypes.DetectType.None | Type.DetectType == NoteTypes.DetectType.HoldMid | Type.OGLTextureName == null) continue;

                    if (i >= (int)currentTick+hitlineAdjust)
                    {
                        string NoteTex = Type.OGLTextureName;

                        if (NoteTex == "spr_SwipeLeftIcon")
                        {
                            for (int k = 0; k < numLanes; k++)
                            {
                                if (chart.Ticks[i].Notes[k].NoteType.IsSimul)
                                {
                                    NoteTex = "spr_SwipeLeftIcon_Simul";
                                    break;
                                }
                            }
                        }

                        if (NoteTex == "spr_SwipeRightIcon")
                        {
                            for (int k = 0; k < numLanes; k++)
                            {
                                if (chart.Ticks[i].Notes[k].NoteType.IsSimul)
                                {
                                    NoteTex = "spr_SwipeRightIcon_Simul";
                                    break;
                                }
                            }
                        }

                        float icnDist = (float)(numTicksVisible - i + currentTick-timingAdjust) / numTicksVisible;
                        if (icnDist < 0) icnDist = 0;
                        PointF icnPoint = GetPointAlongLineF(NodeStartLocs[j], NodeEndLocs[j], icnDist);
                        float icnSize = iconSize * icnDist;
                        DrawFilledRect(icnPoint.X - icnSize / 2, icnPoint.Y - icnSize / 2, icnSize, icnSize, NoteTex);

                    }
                    else if (i > (int)(currentTick+hitlineAdjust - EffectTicks - 1))
                    {
                        float effectSize = (float)(((currentTick+hitlineAdjust - i - 1) / EffectTicks + 1) * iconSize);
                        DrawFilledRect(NodeEndLocs[j].X - effectSize / 2, NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize, "spr_HitEffect");
                    }
                    else if (i >= (int)(currentTick+hitlineAdjust - EffectTicks - EffectFadeTicks - 1))
                    {
                        float effectSize = iconSize * 2.0f;
                        float effectOpacity = 1 - (float)((currentTick+hitlineAdjust - EffectTicks - i - 1) / EffectFadeTicks * 0.8f);

                        GL.Color4(effectOpacity, effectOpacity, effectOpacity, effectOpacity);

                        //Grfx.DrawImage(spr_HitEffect, new Rectangle((int)NodeEndLocs[j].X - effectSize / 2, (int)NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize), 0, 0, spr_HitEffect.Width, spr_HitEffect.Height, GraphicsUnit.Pixel, effectTranspAttr);
                        DrawFilledRect(NodeEndLocs[j].X - effectSize / 2, NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize, "spr_HitEffect");
                    }

                    GL.Color4(1f, 1f, 1f, 1f);

                }
            }

            int comboTick = (int)currentTick + hitlineAdjust - 1;
            if (comboTick < 0) comboTick = 0;
            else if (comboTick >= chart.Length) comboTick = chart.Length - 1;

            // disabled until complete and I can add an option to toggle it
            //if (chart.Ticks[comboTick].ComboNumber > 1)
            //{
            //    int textsize = 48;
            //    if (skin.ComboFont.CommonInfo.LineHeight > 0) textsize = skin.ComboFont.CommonInfo.LineHeight;
            //    int numX = 1040;
            //    int numY = viewHeight - 8;
            //    int textX = numX;
            //    int textY = viewHeight - 128;
            //    DrawCharactersAligned(numX - 128, numY - textsize, textsize, skin.ComboFont, chart.Ticks[comboTick].ComboNumber.ToString(), 256, 1, -4);
            //    DrawFilledRect(textX - 128, textY, 256, 64, "spr_ComboText");
            //}
            //DrawCharactersAligned(64, 64, 32, skin.ComboFont, "01189998819991197253", 80);
            //DrawCharactersAligned(64, 64, 32, skin.ComboFont, "88", 160);
            //DrawCharactersAligned(64, 96, 32, skin.ComboFont, "88", 160, 1);
            //DrawCharactersAligned(64, 128, 32, skin.ComboFont, "88", 160, 2);
            //DrawCharactersAligned(64, 96, 32, skin.ComboFont, "This is a test!---!!!@♪", 205, 0, 0);
            //DrawFilledRect(64, 64, 205, 24, "spr_HoldLocus");

            FrameStopwatch.Stop();
            int sleeptime = (int)(1000*1f/DisplayDevice.Default.RefreshRate) - (int)FrameStopwatch.ElapsedMilliseconds - 3;

            //only render at 30fps if not playing and we would otherwise be rendering faster
            if (!mainform.IsPlaying && DisplayDevice.Default.RefreshRate > 30)
            {
                sleeptime = (int)(1000*1f/30) - (int)FrameStopwatch.ElapsedMilliseconds - 3;
            }
            
            if (sleeptime > 0)
            {
                System.Threading.Thread.Sleep(sleeptime);
                //Console.WriteLine("a");
            }
            else if (sleeptime < 0)
            {
                System.Threading.Thread.Sleep(10);
                //Console.WriteLine("b");
            }
            //else Console.WriteLine("c");
            
            try
            {
                myWindow.SwapBuffers();
            }
            catch
            {
                //myWindow.Close();
            }
            //GL.Finish();

            //Console.WriteLine(myWindow.RenderFrequency);
        }

        Point GetPointAlongLine(Point start, Point end, float distance)
        {
            return new Point(start.X + (int)((end.X - start.X) * distance), start.Y + (int)((end.Y - start.Y) * distance));
        }
        PointF GetPointAlongLineF(Point start, Point end, float distance)
        {
            return new PointF(start.X + (end.X - start.X) * distance, start.Y + (end.Y - start.Y) * distance);
        }
        PointF GetPointAlongLineF(PointF start, PointF end, float distance)
        {
            return new PointF(start.X + (end.X - start.X) * distance, start.Y + (end.Y - start.Y) * distance);
        }


        int LoadTexture(string path)
        {
            Bitmap bmp;
            try
            { bmp = new Bitmap(path); }
            catch
            {
                bmp = new Bitmap(1, 1);
                SkinnedMessageBox.Show(skin, DialogResMgr.GetString("MissingTextureError") + "\n(" + path + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Stop();
            }

            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            
            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.Enable(EnableCap.Texture2D); // this is needed because an AMDTI bug apparently (not sure how recently)
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return tex;
        }

        void UnloadTexture(int tex)
        {
            GL.DeleteTexture(tex);
        }


        void DrawRect(float x, float y, float width, float height, RectangleF uv, bool skipBeginAndEnd = false)
        {
            if (!skipBeginAndEnd) GL.Begin(PrimitiveType.Quads);

            // Top-Left
            GL.TexCoord2(uv.Left, uv.Y-uv.Height);
            GL.Vertex2(x, y+height);

            // Top-Right
            GL.TexCoord2(uv.Right, uv.Y - uv.Height);
            GL.Vertex2(x+width, y+height);

            // Bottom-Right
            GL.TexCoord2(uv.Right, uv.Y);
            GL.Vertex2(x+width, y);

            // Bottom-Left
            GL.TexCoord2(uv.Left, uv.Y);
            GL.Vertex2(x, y);

            if (!skipBeginAndEnd) GL.End();
        }

        private static RectangleF defaultUVrect = new RectangleF(0, 1, 1, 1);
        void DrawRect(float x, float y, float width, float height, bool skipBeginAndEnd = false)
        {
            DrawRect(x, y, width, height, defaultUVrect, skipBeginAndEnd);
        }

        void DrawFilledRect(float x, float y, float width, float height, int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            DrawRect(x, y, width, height);
        }
        void DrawFilledRect(float x, float y, float width, float height, string textureName)
        {
            DrawFilledRect(x, y, width, height, textures[textureName]);
        }

        // returns { NumberOfCharacters(that fit into line), WidthInPixels(of characters that fit) }
        int[] DrawCharacters(float x, float y, float height, BMFontReader.BMFont font, string str, int maxwidth = 0, float chrtracking = -2)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { 0, 0 };
            float sizescale = height / font.CommonInfo.LineHeight;

            // avoid constantly reloading if texture page doesn't change
            // start at -1 because invalid
            int lasttexpage = -1;

            float totalwidth = 0;

            GL.Begin(PrimitiveType.Quads); // begin so that we don't need to decide whether to end or not in loop

            for (int i = 0; i < str.Length; i++)
            {
                float newtotalwidth = totalwidth; // don't touch totalwidth until this iteration is done

                bool fontHasChar = false;
                if (font.Characters.ContainsKey(str[i]))
                {
                    newtotalwidth += ((font.Characters[str[i]].XAdvance + chrtracking) * sizescale);
                    fontHasChar = true;
                }
                else newtotalwidth += height * 2 / 3; // advance by some amount anyway, even if no character (could also draw missing character glyph if I want)


                if (maxwidth > 0 && newtotalwidth >= maxwidth) // character doesn't fit
                {
                    totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
                    return new int[] { i, (int)totalwidth }; // when new character doesn't fit return index
                                                             // index is always character number - 1
                }
                else if (fontHasChar) // character does fit
                {
                    BMFontReader.CharacterInfo chrinfo = font.Characters[str[i]];

                    // X, Y is bottom left
                    float texCoordX = (float)chrinfo.TexCoordX / font.CommonInfo.TexScaleWidth;
                    float texCoordWidth = (float)chrinfo.Width / font.CommonInfo.TexScaleWidth;
                    float texCoordHeight = (float)chrinfo.Height / font.CommonInfo.TexScaleHeight;
                    float texCoordY = (float)chrinfo.TexCoordY / font.CommonInfo.TexScaleHeight + texCoordHeight;

                    // X, Y is bottom left
                    float quadX = x + totalwidth + (chrinfo.XOffset * sizescale); // deliberately use width from before advancing for new character
                    float quadWidth = (chrinfo.Width * sizescale);
                    float quadHeight = (chrinfo.Height * sizescale);
                    float quadY = y + ((font.CommonInfo.BaseHeight - chrinfo.YOffset) * sizescale) - quadHeight;

                    if (chrinfo.TexturePage != lasttexpage)
                    {
                        GL.End();
                        lasttexpage = chrinfo.TexturePage;
                        int texture = textures["combofont_" + chrinfo.TexturePage.ToString()];
                        GL.BindTexture(TextureTarget.Texture2D, texture);
                        GL.Begin(PrimitiveType.Quads);
                    }
                    DrawRect(quadX, quadY, quadWidth, quadHeight, new RectangleF(texCoordX, texCoordY, texCoordWidth, texCoordHeight), true);
                }
                

                // adjust next char position for kerning if needed
                // this is after our if case so it can't affect whether previous character should fit or not
                if (font.KernPairs.Count > 0 && i < str.Length - 1)
                {
                    Tuple<char, char> pair = new Tuple<char, char>(str[i], str[i + 1]); ;
                    if (font.KernPairs.ContainsKey(pair)) newtotalwidth += ((font.KernPairs[pair].Amount) * sizescale);
                }


                totalwidth = newtotalwidth;
            }

            GL.End();

            totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
            return new int[] { str.Length, (int)totalwidth }; // only reached if not triggered early
        }

        // returns { NumberOfCharacters(that will fit), WidthInPixels(of characters that fit) }
        int[] GetLineLength(float height, BMFontReader.BMFont font, string str, int maxwidth = 0, float chrtracking = -2)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { 0, 0 };
            float sizescale = height / font.CommonInfo.LineHeight;

            float totalwidth = 0;

            for (int i = 0; i < str.Length; i++)
            {
                float newtotalwidth = totalwidth; // don't touch totalwidth until this iteration is done

                if (font.Characters.ContainsKey(str[i])) newtotalwidth += ((font.Characters[str[i]].XAdvance + chrtracking) * sizescale);
                else newtotalwidth += height * 2 / 3; // advance by some amount anyway, even if no character (could also draw missing character glyph if I want)

                if (maxwidth > 0 && newtotalwidth >= maxwidth) // character doesn't fit
                {
                    totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
                    return new int[] { i, (int)totalwidth }; // when new character doesn't fit return index
                                                        // index is always character number - 1
                }


                // adjust next char position for kerning if needed
                // this is after our if case so it can't affect whether previous character should fit or not
                if (font.KernPairs.Count > 0 && i < str.Length - 1)
                {
                    Tuple<char, char> pair = new Tuple<char, char>(str[i], str[i + 1]); ;
                    if (font.KernPairs.ContainsKey(pair)) newtotalwidth += ((font.KernPairs[pair].Amount) * sizescale);
                }

                totalwidth = newtotalwidth;
            }

            totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
            return new int[] { str.Length, (int)totalwidth }; // only reached if not triggered early
        }

        int DrawCharactersAligned(float x, float y, float height, BMFontReader.BMFont font, string str, int maxwidth = 0, int align = 0, float chrtracking = -2)
        {
            if (font.CommonInfo.LineHeight == 0) return 0;

            if (maxwidth > 0)
            {
                int[] maxchrs = GetLineLength(height, font, str, maxwidth, chrtracking);
                if (maxchrs[0] < str.Length) str = str.Remove(maxchrs[0]);

                if (align == 1)
                {
                    x += (maxwidth - maxchrs[1]) / 2;
                }
                else if (align == 2)
                {
                    x += maxwidth - maxchrs[1];
                }
            }

            DrawCharacters(x, y, height, font, str, 0, chrtracking);

            return str.Length;
        }
    }
}