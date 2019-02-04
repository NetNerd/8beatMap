using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _8beatMap
{
    class GameCloneRenderer_OGL
    {
        private GameWindow myWindow = null;

        public double currentTick = 0;
        public int numTicksVisible = 24;

        private float timingAdjust = 0;
        private int hitlineAdjust = 1;

        public Form1 mainform = null;

        private Skinning.Skin skin = Skinning.DefaultSkin;

        static string[] textureNames = { "spr_HoldLocus", "spr_SwipeLocus",
            "spr_TapIcon", "spr_HoldIcon", "spr_SimulIcon",
            "spr_SwipeRightIcon", "spr_SwipeRightIcon_Simul", "spr_SwipeLeftIcon", "spr_SwipeLeftIcon_Simul",
            "spr_gbsFlick", "spr_gbsFlick_Simul", "spr_gbsClock", "spr_gbsClock_Simul",
            "spr_HitEffect",
            "spr_Chara1", "spr_Chara2", "spr_Chara3", "spr_Chara4", "spr_Chara5", "spr_Chara6", "spr_Chara7", "spr_Chara8" };

        static string[] texturePaths = { "nodeimg/locus.png", "nodeimg/locus2.png",
            "nodeimg/node_1.png", "nodeimg/node_2.png", "nodeimg/node_3.png",
            "nodeimg/node_4.png", "nodeimg/node_4_3.png", "nodeimg/node_6.png", "nodeimg/node_6_3.png",
            "nodeimg/gbs/node_7.png", "nodeimg/gbs/node_8.png", "nodeimg/gbs/node_9.png", "nodeimg/gbs/node_10.png",
            "nodeimg/node_effect.png",
            "charaimg/1.png", "charaimg/2.png", "charaimg/3.png", "charaimg/4.png", "charaimg/5.png", "charaimg/6.png", "charaimg/7.png", "charaimg/8.png"};

        static System.Collections.Generic.Dictionary<string, int> textures = new System.Collections.Generic.Dictionary<string, int>();

        public Color clearColor = Color.FromArgb(0, 0, 0, 0);


        Point[] NodeStartLocs_raw = { new Point(223, 77), new Point(320, 100), new Point(419, 114), new Point(519, 119), new Point(617, 119), new Point(717, 114), new Point(816, 100), new Point(923, 77) };
        Point[] NodeStartLocs = { new Point(223, 640 - 77), new Point(320, 640 - 100), new Point(419, 640 - 114), new Point(519, 640 - 119), new Point(617, 640 - 119), new Point(717, 640 - 114), new Point(816, 640 - 100), new Point(923, 640 - 77) };
        Point[] NodeEndLocs = { new Point(75, 156), new Point(213, 120), new Point(354, 98), new Point(497, 88), new Point(639, 88), new Point(782, 98), new Point(923, 120), new Point(1061, 156) };
        int numLanes = 8;


        static System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

        
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
        }

        public GameCloneRenderer_OGL(int wndWidth, int wndHeight, Skinning.Skin newskin)
        {
            skin = newskin;
            NodeStartLocs = (Point[])skin.NodeStartLocs.Clone();
            NodeEndLocs = (Point[])skin.NodeEndLocs.Clone();
            SetupNodeLocs(wndWidth, wndHeight);

            System.Threading.Thread oglThread = new System.Threading.Thread(() =>
            {
                myWindow = new GameWindow(wndWidth, wndHeight, OpenTK.Graphics.GraphicsMode.Default, "8beatMap Preview Window");
                myWindow.VSync = OpenTK.VSyncMode.Adaptive;


                myWindow.Load += (sender, e) =>
                {
                    SetupTextures();

                    GL.ClearColor(clearColor);

                    GL.Enable(EnableCap.Texture2D);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    //GL.Enable(EnableCap.PolygonSmooth);  // sometimes causes diagonal lines through quads
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


                myWindow.TargetRenderFrequency = 60;
                myWindow.Run();
            });

            oglThread.Start();

            while (myWindow == null)
                System.Threading.Thread.Yield();
        }

        public void Stop()
        {
            if (myWindow != null)
            {
                myWindow.Close();
                myWindow = null;
            }
        }



        Matrix4 ProjMatrix;
        
        int iconSize = 352;
        int halfIconSize = 176;
        int holdSize = 130;
        int halfHoldSize = 65;
        int swipeSize = 66;
        int halfSwipeSize = 33;

        int EffectTime = 1000000;
        int EffectFadeTime = 390000;

        int viewHeight = 640;


        void RenderFrame(object sender, EventArgs e)
        {
            if (myWindow == null) return;
            if (myWindow.WindowState == WindowState.Minimized) return;

            if (mainform == null) return;

            mainform.UpdateGameCloneChart();


            double EffectTicks = mainform.chart.ConvertTimeToTicks(new TimeSpan(EffectTime));
            double EffectFadeTicks = mainform.chart.ConvertTimeToTicks(new TimeSpan(EffectFadeTime));


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



            GL.Color4(1f, 1f, 1f, 0.65f); //transparency

            for (int i = (int)currentTick + numTicksVisible + 1; i >= (int)currentTick - 48; i--) // 48 is magic from Notedata.Chart.UpdateSwipeEnd()
            {
                if (i >= mainform.chart.Length) i = mainform.chart.Length - 1;
                if (i < 0) break;

                for (int j = 0; j < numLanes; j++)
                {
                    NoteTypes.NoteTypeDef Type = mainform.chart.FindVisualNoteType(i, j);
                    
                    GL.BindTexture(TextureTarget.Texture2D, textures["spr_SwipeLocus"]);

                    if ((Type.DetectType == NoteTypes.DetectType.SwipeEndPoint | Type.DetectType == NoteTypes.DetectType.SwipeMid | Type.DetectType == NoteTypes.DetectType.SwipeDirChange) && !mainform.chart.Ticks[i].Notes[j].IsSwipeEnd)
                    {
                        Point swipeEndPoint = mainform.chart.Ticks[i].Notes[j].SwipeEndPoint;

                        if (swipeEndPoint.X > i & currentTick-timingAdjust < swipeEndPoint.X & swipeEndPoint.Y < numLanes)
                        {
                            int sTick = i;
                            if (sTick < currentTick) sTick = (int)currentTick;

                            float iDist = (float)(numTicksVisible - i + currentTick-timingAdjust) / numTicksVisible;
                            float eDist = (float)(numTicksVisible - swipeEndPoint.X + currentTick-timingAdjust) / numTicksVisible;
                            int eSize = (int)(halfSwipeSize * eDist);
                            Point iPoint = GetPointAlongLine(NodeStartLocs[j], NodeEndLocs[j], iDist);
                            Point ePoint = GetPointAlongLine(NodeStartLocs[swipeEndPoint.Y], NodeEndLocs[swipeEndPoint.Y], eDist);

                            //float iDirection = 88888888;
                            //if (iPoint.Y - NodeStartLocs[j].Y != 0) { iDirection = (iPoint.X - NodeStartLocs[j].X) / (float)(iPoint.Y - NodeStartLocs[j].Y) / 2f; }
                            float eDirection = 88888888;
                            if (ePoint.Y - NodeStartLocs[swipeEndPoint.Y].Y != 0) { eDirection = (ePoint.X - NodeStartLocs[swipeEndPoint.Y].X) / (float)(ePoint.Y - NodeStartLocs[swipeEndPoint.Y].Y) / 2f; }

                            //float avgDirection = iDirection + eDirection / 2f;

                            //if (iDirection == 0) iDirection = 0.0001f;
                            //if (iDirection > 1) iDirection = 1f;
                            //if (iDirection < -1) iDirection = -1f;
                            //if (eDirection == 0) eDirection = 0.0001f;
                            if (eDirection > 1) eDirection = 1f;
                            if (eDirection < -1) eDirection = -1f;
                            //if (avgDirection == 0) avgDirection = 0.0001f;
                            //if (avgDirection > 1) avgDirection = 1f;
                            //if (avgDirection < -1) avgDirection = -1f;

                            //float absiDirection = iDirection;
                            //if (iDirection < 0) absiDirection = iDirection * -1;
                            float abseDirection = eDirection;
                            if (eDirection < 0) abseDirection = eDirection * -1;
                            //float absDirection = avgDirection;
                            //if (avgDirection < 0) absDirection = avgDirection * -1;


                            float sDist;
                            Point sPoint;
                            if (i >= currentTick-timingAdjust)
                            {
                                sDist = iDist;
                                sPoint = iPoint;
                             }
                            else
                            {
                                sDist = (float)(numTicksVisible - sTick + currentTick-timingAdjust) / numTicksVisible;
                                sPoint = GetPointAlongLine(iPoint, ePoint, ((float)currentTick-timingAdjust - i) / (swipeEndPoint.X - i));
                                //Point sPoint = GetPointAlongLine(ePoint, iPoint, sDist / iDist);
                            }

                            int sSize = (int)(halfSwipeSize * sDist);


                            GL.Begin(PrimitiveType.Quads);

                            GL.TexCoord2(0.1, 0);
                            GL.Vertex2(sPoint.X + sSize*eDirection, sPoint.Y + sSize - sSize*abseDirection);

                            GL.TexCoord2(0.9, 0);
                            GL.Vertex2(ePoint.X + eSize*eDirection, ePoint.Y + eSize - eSize*abseDirection);

                            GL.TexCoord2(0.9, eDist);
                            GL.Vertex2(ePoint.X - eSize*eDirection, ePoint.Y - eSize + eSize*abseDirection);
                            
                            GL.TexCoord2(0.1, sDist);
                            GL.Vertex2(sPoint.X - sSize*eDirection, sPoint.Y - sSize + sSize*abseDirection);

                            GL.End();
                        }
                    }



                    if (i < (int)currentTick-1) continue;

                    GL.BindTexture(TextureTarget.Texture2D, textures["spr_HoldLocus"]);

                    if (Type.DetectType == NoteTypes.DetectType.HoldMid && (i == (int)currentTick-1 | mainform.chart.FindVisualNoteType(i - 1, j).DetectType != NoteTypes.DetectType.HoldMid))
                    {
                        double start = i;
                        if (start < currentTick + 1 - timingAdjust) start = (int)((currentTick + 1 - timingAdjust) * 4) / 4f;
                        int end = i;
                        while (mainform.chart.FindVisualNoteType(end, j).DetectType == NoteTypes.DetectType.HoldMid) end++;
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

                        GL.TexCoord2(0, 0.9);
                        GL.Vertex2(ePoint.X + eSize*angledSizeScale, ePoint.Y - eSize*endYOffset*angledSizeScale);

                        GL.TexCoord2(0, 0.1);
                        GL.Vertex2(sPoint.X + sSize*angledSizeScale, sPoint.Y - sSize*endYOffset*angledSizeScale);

                        GL.TexCoord2(sDist, 0.1);
                        GL.Vertex2(sPoint.X - sSize*angledSizeScale, sPoint.Y + sSize*endYOffset*angledSizeScale);

                        GL.TexCoord2(eDist, 0.9);
                        GL.Vertex2(ePoint.X - eSize*angledSizeScale, ePoint.Y + eSize*endYOffset*angledSizeScale);

                        GL.End();
                    }
                }
            }

            GL.Color4(1f, 1f, 1f, 1f);


            for (int i = (int)currentTick + numTicksVisible; i >= (int)(currentTick - EffectTicks - EffectFadeTicks - 1); i--)
            //for (int i = (int)(startTick - EffectTicks - EffectFadeTicks - 1); i <= (int)startTick + numTicksVisible; i++)
            {
                if (i >= mainform.chart.Length) i = mainform.chart.Length - 1;
                if (i < 0) break;

                for (int j = 0; j < numLanes; j++)
                {
                    NoteTypes.NoteTypeDef Type = mainform.chart.FindVisualNoteType(i, j);

                    if (Type.DetectType == NoteTypes.DetectType.None | Type.DetectType == NoteTypes.DetectType.HoldMid | Type.OGLTextureName == null) continue;

                    if (i >= (int)currentTick+hitlineAdjust)
                    {
                        string NoteTex = Type.OGLTextureName;

                        if (NoteTex == "spr_SwipeLeftIcon")
                        {
                            for (int k = 0; k < numLanes; k++)
                            {
                                if (mainform.chart.Ticks[i].Notes[k].NoteType.IsSimul)
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
                                if (mainform.chart.Ticks[i].Notes[k].NoteType.IsSimul)
                                {
                                    NoteTex = "spr_SwipeRightIcon_Simul";
                                    break;
                                }
                            }
                        }

                        float icnDist = (float)(numTicksVisible - i + currentTick-timingAdjust) / numTicksVisible;
                        if (icnDist < 0) icnDist = 0;
                        Point icnPoint = GetPointAlongLine(NodeStartLocs[j], NodeEndLocs[j], icnDist);
                        int icnSize = (int)(iconSize * icnDist);
                        DrawFilledRect(icnPoint.X - icnSize / 2, icnPoint.Y - icnSize / 2, icnSize, icnSize, NoteTex);

                    }
                    else if (i > (int)(currentTick+hitlineAdjust - EffectTicks - 1))
                    {
                        int effectSize = (int)(((currentTick+hitlineAdjust - i - 1) / EffectTicks + 1) * iconSize);
                        DrawFilledRect(NodeEndLocs[j].X - effectSize / 2, NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize, "spr_HitEffect");
                    }
                    else if (i >= (int)(currentTick+hitlineAdjust - EffectTicks - EffectFadeTicks - 1))
                    {
                        int effectSize = (int)(iconSize * 2.0f);
                        float effectOpacity = 1 - (float)((currentTick+hitlineAdjust - EffectTicks - i - 1) / EffectFadeTicks * 0.8f);

                        GL.Color4(1f, 1f, 1f, effectOpacity);

                        //Grfx.DrawImage(spr_HitEffect, new Rectangle((int)NodeEndLocs[j].X - effectSize / 2, (int)NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize), 0, 0, spr_HitEffect.Width, spr_HitEffect.Height, GraphicsUnit.Pixel, effectTranspAttr);
                        DrawFilledRect(NodeEndLocs[j].X - effectSize / 2, NodeEndLocs[j].Y - effectSize / 2, effectSize, effectSize, "spr_HitEffect");
                    }

                    GL.Color4(1f, 1f, 1f, 1f);
                }
            }


            myWindow.SwapBuffers();
            //GL.Finish();
            System.Threading.Thread.Sleep(12);

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


        int LoadTexture(string path)
        {
            Bitmap bmp;
            try
            { bmp = new Bitmap(path); }
            catch
            {
                bmp = new Bitmap(1, 1);
                //System.Windows.Forms.MessageBox.Show("Error: Missing texture image(s) for preview display.");
                System.Windows.Forms.MessageBox.Show(DialogResMgr.GetString("MissingTextureError"));
                Stop();
            }

            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

            bmp.UnlockBits(bmpData);
            bmp.Dispose();


            GL.BindTexture(TextureTarget.Texture2D, 0);

            return tex;
        }

        void UnloadTexture(int tex)
        {
            GL.DeleteTexture(tex);
        }


        void DrawRect(int x, int y, int width, int height)
        {
            GL.Begin(PrimitiveType.Quads);

            // Top-Left
            GL.TexCoord2(0, 0);
            GL.Vertex2(x, y+height);

            // Top-Right
            GL.TexCoord2(1, 0);
            GL.Vertex2(x+width, y+height);

            // Bottom-Right
            GL.TexCoord2(1, 1);
            GL.Vertex2(x+width, y);

            // Bottom-Left
            GL.TexCoord2(0, 1);
            GL.Vertex2(x, y);

            GL.End();
        }

        void DrawFilledRect(int x, int y, int width, int height, int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            DrawRect(x, y, width, height);
        }
        void DrawFilledRect(int x, int y, int width, int height, string textureName)
        {
            DrawFilledRect(x, y, width, height, textures[textureName]);
        }
    }
}