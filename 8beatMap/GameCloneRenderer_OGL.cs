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

        public bool showcombo = true;

        private System.Collections.Generic.Dictionary<string, int> textures = new System.Collections.Generic.Dictionary<string, int>();

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

        private void UnloadAllTextures()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0); // make sure no textures are loaded before deleting
            foreach (System.Collections.Generic.KeyValuePair<string, int> tex in textures)
            {
                UnloadTexture(tex.Value);
            }
            textures.Clear();
        }

        private void LoadFontTextures(BMFontReader.BMFont font)
        {
            for (int i = 0; i < font.PageTexPaths.Length; i++)
            {
                if (font.PageTexPaths[i] == null) continue;

                string texkey = font.PageTexPaths[i];
                string texpath = font.PageTexPaths[i];

                if (font.CanLoad8Bit)
                {
                    if (!textures.ContainsKey(texkey))
                        textures.Add(texkey, LoadTexture8BitGrayscale(texpath));
                    else
                        textures[texkey] = LoadTexture8BitGrayscale(texpath);
                }
                else
                {
                    if (!textures.ContainsKey(texkey))
                        textures.Add(texkey, LoadTexture(texpath));
                    else
                        textures[texkey] = LoadTexture(texpath);
                }

                if (font.CommonInfo.Packed)
                {
                    int[] channelTextures = LoadTextureToSplitChannels(texpath);

                    if (!textures.ContainsKey(texkey + "A"))
                        textures.Add(texkey + "A", channelTextures[0]);
                    else
                        textures[texkey + "A"] = channelTextures[0];

                    if (!textures.ContainsKey(texkey + "R"))
                        textures.Add(texkey + "R", channelTextures[1]);
                    else
                        textures[texkey + "R"] = channelTextures[1];

                    if (!textures.ContainsKey(texkey + "G"))
                        textures.Add(texkey + "G", channelTextures[2]);
                    else
                        textures[texkey + "G"] = channelTextures[2];

                    if (!textures.ContainsKey(texkey + "B"))
                        textures.Add(texkey + "B", channelTextures[3]);
                    else
                        textures[texkey + "B"] = channelTextures[3];
                }
            }
        }

        private void SetupTextures()
        {
            UnloadAllTextures();

            foreach (System.Collections.Generic.KeyValuePair<string, string> tex in skin.TexturePaths)
            {
                if (!textures.ContainsKey(tex.Key))
                    textures.Add(tex.Key, LoadTexture(tex.Value));
                else
                    textures[tex.Key] = LoadTexture(tex.Value);
            }

            LoadFontTextures(skin.ComboTextInfo.Font);
        }

        public GameCloneRenderer_OGL(int wndWidth, int wndHeight, int wndX, int wndY, WindowState wndState, Form1 mainform, Skinning.Skin skin, bool showcombo)
        {
            this.mainform = mainform;
            this.skin = skin;
            this.showcombo = showcombo;
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

                myWindow.Closed += (sender, e) =>
                {
                    UnloadAllTextures();
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


            GL.Color4(1f, 1f, 1f, 1f);

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
            int comboNumber = 0;

            if (comboTick >= chart.Length) comboNumber = chart.Ticks[chart.Length - 1].ComboNumber; // if outside of chart, get last valid tick
            else if (comboTick >= 0) comboNumber = chart.Ticks[comboTick].ComboNumber; // if inside chart, get current tick
            // otherwise comboNumber is still 0


            if (showcombo && comboNumber >= skin.ComboTextInfo.StartNumber)
            {
                int textsize = skin.ComboTextInfo.TextSize;
                if (textsize == 0 && skin.ComboTextInfo.Font.CommonInfo.LineHeight > 0) textsize = skin.ComboTextInfo.Font.CommonInfo.LineHeight;
                int numX = skin.ComboTextInfo.Locs[0].X;
                int numY = viewHeight - skin.ComboTextInfo.Locs[0].Y;
                int textX = skin.ComboTextInfo.Locs[1].X;
                int textY = viewHeight - skin.ComboTextInfo.Locs[1].Y;
                DrawCharactersAligned(numX - 128, numY, textsize, skin.ComboTextInfo.Font, comboNumber.ToString(), 256, 1, skin.ComboTextInfo.CharacterTracking);
                DrawFilledRect(textX - 128, textY, 256, 64, "spr_ComboText");
            }

            //DrawString(64, 64, 32, skin.ComboTextInfo.Font, "01189998819991197253", 80, 1);
            //DrawCharactersAligned(64, 64, 32, skin.ComboTextInfo.Font, "88", 160);
            //DrawCharactersAligned(64, 96, 32, skin.ComboTextInfo.Font, "88", 160, 1);
            //DrawCharactersAligned(64, 128, 32, skin.ComboTextInfo.Font, "88", 160, 2);
            //DrawCharactersAligned(64, 96, 32, skin.ComboTextInfo.Font, "This is a test!---!!!@♪", 205, 0, 0);
            //DrawString(32, 600-32, 32, skin.ComboTextInfo.Font, "Lor\nem ip\nsum dolor s❗it amet, consectetur adipiscing elit. Vestibulum consequ\nat sem at purus pretium, vitae mollis sapien max\nimus. Duis rutrum elit vel odio iaculis dictum. Fusce viverra nisi eget dictum facilisis. Maecenas eleifend eu lorem ut convallis. Donec sed ullamc\norper dui. Vivamus hendrerit magna vitae nisl porttitor, ac accumsan urna volutpat. Pellentesque nec nulla ultricies, suscipit arcu a, eleifend dui. Suspe\nndisse potenti. Mauris felis arcu, sollicitudin eu finibus ut, interdum id ante.", 500, 12, 0, 0, 1);
            //DrawString(600, 600-32, 32, skin.ComboTextInfo.Font, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consequat sem at purus pretium, vitae mollis sapien maximus. Duis rutrum elit vel odio iaculis dictum. Fusce viverra nisi eget dictum facilisis. Maecenas eleifend eu lorem ut convallis. Donec sed ullamcorper dui. Vivamus hendrerit magna vitae nisl porttitor, ac accumsan urna volutpat. Pellentesque nec nulla ultricies, suscipit arcu a, eleifend dui. Suspendisse potenti. Mauris felis arcu, sollicitudin eu finibus ut, interdum id ante.", 500, 12, 0, 0, 1, true);
            //DrawString(32, 600-32, 32, skin.ComboTextInfo.Font, "日本語テストだ。\n多分機能する…\nチェック\nEverybody\nDance Dance Dance 纏ったもの脱ぎ捨てて\nDance Dance Dance 自分の殻つきやぶって(hey!)", 500, 12, 0, 0, 1, true);
            //DrawFilledRect(580, 202, 10, 390, "spr_HoldLocus");
            //GL.Color4(0f, 1f, 0, 1f);
            //DrawCharactersAligned(640, 16, 32, skin.ComboTextInfo.Font, "😃☺😃☺😃⁉⬆", 0, 0, 0);
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

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            

            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            
            GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return tex;
        }


        int LoadTexture8BitGrayscale(string path)
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


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] dataBytes = new byte[bmp.Width * bmp.Height * 4];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, dataBytes, 0, bmp.Width * bmp.Height * 4);

            byte[] monodata = new byte[bmp.Width * bmp.Height];
            for (int i = 0; i < bmp.Height * bmp.Width * 4; i += 4)
            {
                monodata[i / 4] = dataBytes[i]; // just sample blue because it's easiest
            }


            int tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, tex);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


            System.Drawing.Imaging.BitmapData bmpDataJustChannel = new System.Drawing.Imaging.BitmapData()
            { PixelFormat = System.Drawing.Imaging.PixelFormat.DontCare, Width = bmp.Width, Height = bmp.Height };

            bmpDataJustChannel.Scan0 = System.Runtime.InteropServices.Marshal.AllocHGlobal(monodata.Length);
            System.Runtime.InteropServices.Marshal.Copy(monodata, 0, bmpDataJustChannel.Scan0, monodata.Length);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, bmp.Width, bmp.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, bmpDataJustChannel.Scan0);

            bmpDataJustChannel.Scan0 = IntPtr.Zero;
            System.Runtime.InteropServices.Marshal.FreeHGlobal(bmpDataJustChannel.Scan0);

            GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);


            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return tex;
        }


        // loads in order ARGB
        int[] LoadTextureToSplitChannels(string path)
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


            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] dataBytes = new byte[bmp.Width * bmp.Height * 4];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, dataBytes, 0, bmp.Width * bmp.Height * 4);

            byte[][] planes = new byte[4][]
            { new byte[bmp.Width * bmp.Height], new byte[bmp.Width * bmp.Height], new byte[bmp.Width * bmp.Height], new byte[bmp.Width * bmp.Height] };
            for (int i = 0; i < bmp.Height * bmp.Width * 4; i += 4)
            {
                planes[0][i / 4] = dataBytes[i];
                planes[1][i / 4] = dataBytes[i + 1];
                planes[2][i / 4] = dataBytes[i + 2];
                planes[3][i / 4] = dataBytes[i + 3];
            }


            int[] outtextures = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int tex = GL.GenTexture();
                outtextures[3-i] = tex;

                GL.BindTexture(TextureTarget.Texture2D, tex);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);


                System.Drawing.Imaging.BitmapData bmpDataJustChannel = new System.Drawing.Imaging.BitmapData()
                { PixelFormat = System.Drawing.Imaging.PixelFormat.DontCare, Width = bmp.Width, Height = bmp.Height };

                bmpDataJustChannel.Scan0 = System.Runtime.InteropServices.Marshal.AllocHGlobal(planes[i].Length);
                System.Runtime.InteropServices.Marshal.Copy(planes[i], 0, bmpDataJustChannel.Scan0, planes[i].Length);
                
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, bmp.Width, bmp.Height, 0, PixelFormat.Alpha, PixelType.UnsignedByte, bmpDataJustChannel.Scan0);

                bmpDataJustChannel.Scan0 = IntPtr.Zero;
                System.Runtime.InteropServices.Marshal.FreeHGlobal(bmpDataJustChannel.Scan0);

                GL.Enable(EnableCap.Texture2D); // this is needed because an ATI bug apparently (not sure how recently)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }


            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return outtextures;
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

        void DrawMissingCharGlyph(float top, float bottom, float left, float right, float linewidth, bool skipBeginAndEnd = false)
        {
            float halflinewidth = linewidth / 1f;

            float midpointX = left + (right - left) / 2f;
            float midpointY = bottom + (top - bottom) / 2f;

            if (!skipBeginAndEnd) GL.Begin(PrimitiveType.Quads);
            // this is a little messy because of only using quads... but easy

            // Top Line
            GL.Vertex2(left, top); // Top-Left
            GL.Vertex2(right, top); // Top-Right
            GL.Vertex2(right, top - linewidth); // Bottom-Right
            GL.Vertex2(left, top - linewidth); // Bottom-Left

            // Bottom Line
            GL.Vertex2(left, bottom + linewidth); // Top-Left
            GL.Vertex2(right, bottom + linewidth); // Top-Right
            GL.Vertex2(right, bottom); // Bottom-Right
            GL.Vertex2(left, bottom); // Bottom-Left

            // Left Line
            GL.Vertex2(left, top - linewidth); // Top-Left
            GL.Vertex2(left + linewidth, top - linewidth); // Top-Right
            GL.Vertex2(left + linewidth, bottom + linewidth); // Bottom-Right
            GL.Vertex2(left, bottom + linewidth); // Bottom-Left

            // Right Line
            GL.Vertex2(right - linewidth, top - linewidth); // Top-Left
            GL.Vertex2(right, top - linewidth); // Top-Right
            GL.Vertex2(right, bottom + linewidth); // Bottom-Right
            GL.Vertex2(right - linewidth, bottom + linewidth); // Bottom-Left

            // Top-Left Quadrant Upper
            GL.Vertex2(left + linewidth, top - linewidth); // Outer Corner
            GL.Vertex2(left + linewidth + halflinewidth, top - linewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX, midpointY + halflinewidth); // Inner Corner (offset)
            GL.Vertex2(midpointX - halflinewidth, midpointY + halflinewidth); // Inner Corner
                                                                              // Top-Left Quadrant Lower
            GL.Vertex2(left + linewidth, top - linewidth); // Outer Corner
            GL.Vertex2(left + linewidth, top - linewidth - halflinewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX - halflinewidth, midpointY); // Inner Corner (offset)
            GL.Vertex2(midpointX - halflinewidth, midpointY + halflinewidth); // Inner Corner

            // Top-Right Quadrant Upper
            GL.Vertex2(right - linewidth, top - linewidth); // Outer Corner
            GL.Vertex2(right - linewidth - halflinewidth, top - linewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX, midpointY + halflinewidth); // Inner Corner (offset)
            GL.Vertex2(midpointX + halflinewidth, midpointY + halflinewidth); // Inner Corner
                                                                              // Top-Right Quadrant Lower
            GL.Vertex2(right - linewidth, top - linewidth); // Outer Corner
            GL.Vertex2(right - linewidth, top - linewidth - halflinewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX + halflinewidth, midpointY); // Inner Corner (offset)
            GL.Vertex2(midpointX + halflinewidth, midpointY + halflinewidth); // Inner Corner

            // Bottom-Left Quadrant Upper
            GL.Vertex2(left + linewidth, bottom + linewidth); // Outer Corner
            GL.Vertex2(left + linewidth + halflinewidth, bottom + linewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX, midpointY - halflinewidth); // Inner Corner (offset)
            GL.Vertex2(midpointX - halflinewidth, midpointY - halflinewidth); // Inner Corner
                                                                              // Bottom-Left Quadrant Lower
            GL.Vertex2(left + linewidth, bottom + linewidth); // Outer Corner
            GL.Vertex2(left + linewidth, bottom + linewidth + halflinewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX - halflinewidth, midpointY); // Inner Corner (offset)
            GL.Vertex2(midpointX - halflinewidth, midpointY - halflinewidth); // Inner Corner

            // Bottom-Right Quadrant Upper
            GL.Vertex2(right - linewidth, bottom + linewidth); // Outer Corner
            GL.Vertex2(right - linewidth - halflinewidth, bottom + linewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX, midpointY - halflinewidth); // Inner Corner (offset)
            GL.Vertex2(midpointX + halflinewidth, midpointY - halflinewidth); // Inner Corner
                                                                              // Bottom-Right Quadrant Lower
            GL.Vertex2(right - linewidth, bottom + linewidth); // Outer Corner
            GL.Vertex2(right - linewidth, bottom + linewidth + halflinewidth); // Outer Corner (offset)
            GL.Vertex2(midpointX + halflinewidth, midpointY); // Inner Corner (offset)
            GL.Vertex2(midpointX + halflinewidth, midpointY - halflinewidth); // Inner Corner

            // Inner Square
            GL.Vertex2(midpointX - halflinewidth, midpointY + halflinewidth); // Top-Left
            GL.Vertex2(midpointX + halflinewidth, midpointY + halflinewidth); // Top-Right
            GL.Vertex2(midpointX + halflinewidth, midpointY - halflinewidth); // Bottom-Right
            GL.Vertex2(midpointX - halflinewidth, midpointY - halflinewidth); // Bottom-Left

            if (!skipBeginAndEnd) GL.End();
        }

        // returns { NumberOfCharacters(that fit into line), WidthInPixels(of characters that fit) }
        int[] DrawCharacters(float x, float y, float height, BMFontReader.BMFont font, string str, int maxwidth = 0, float chrtracking = -2, bool breakOnWhitespaceNearEnd = true)
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

                int utf32Char = char.ConvertToUtf32(str, i);

                bool fontHasChar = false;
                bool replacedSurrogateWithMissingGlyph = false;
                if (font.Characters.ContainsKey(utf32Char))
                {
                    newtotalwidth += ((font.Characters[utf32Char].XAdvance + chrtracking) * sizescale);
                    fontHasChar = true;
                }
                else if(font.Characters.ContainsKey(-1)) // check for missing character glyph in font
                {
                    if (utf32Char > 0xffff) replacedSurrogateWithMissingGlyph = true; // this is needed to track position in the string properly
                                                                                      // -- we should advance by one if the next character isn't a standalone int
                    utf32Char = -1;
                    newtotalwidth += ((font.Characters[utf32Char].XAdvance + chrtracking) * sizescale);
                    fontHasChar = true;
                }
                else
                {
                    newtotalwidth += height * 1 / 2; // advance by some amount anyway, even if no character (could also draw missing character glyph if I want

                    GL.End();

                    GL.BindTexture(TextureTarget.Texture2D, 0); // clear texture
                    lasttexpage = -1;

                    float bottom = y;
                    float top = bottom + (font.CommonInfo.BaseHeight * sizescale * 0.8f);
                    float left = x + totalwidth;
                    float right = left + (height * 1 / 2) - 2;
                    float linewidth = 1.5f;

                    GL.Begin(PrimitiveType.Quads);

                    DrawMissingCharGlyph(top, bottom, left, right, linewidth, true);
                }


                if (maxwidth > 0 && (newtotalwidth >= maxwidth || (breakOnWhitespaceNearEnd && char.IsWhiteSpace(str, i) && newtotalwidth + height*2 >= maxwidth))) // character doesn't fit (or we can start a new line soon)
                {
                    totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
                    return new int[] { i, (int)totalwidth }; // when new character doesn't fit return index
                                                             // index is always character number - 1
                }
                else if (fontHasChar) // character does fit
                {
                    BMFontReader.CharacterInfo chrinfo = font.Characters[utf32Char];

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

                    if (chrinfo.TexturePage != lasttexpage || font.CommonInfo.Packed) // just always do this if packed...
                    {
                        GL.End();
                        lasttexpage = chrinfo.TexturePage;
                        int texture = 0;
                        if (font.CommonInfo.Packed)
                        {
                            // heyy... I can treat this as unpremultiplied!   rgb = old*(1-alpha)+alpha, a = old*(1-alpha)+alpha
                            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                            if (chrinfo.Channels == BMFontReader.CharacterChannels.Red) texture = textures[font.PageTexPaths[chrinfo.TexturePage] + "R"];
                            else if (chrinfo.Channels == BMFontReader.CharacterChannels.Green) texture = textures[font.PageTexPaths[chrinfo.TexturePage] + "G"];
                            else if (chrinfo.Channels == BMFontReader.CharacterChannels.Blue) texture = textures[font.PageTexPaths[chrinfo.TexturePage] + "B"];
                            else if (chrinfo.Channels == BMFontReader.CharacterChannels.Alpha) texture = textures[font.PageTexPaths[chrinfo.TexturePage] + "A"];
                            else
                            {
                                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                                texture = textures[font.PageTexPaths[chrinfo.TexturePage]];
                            }
                        }
                        else if (font.CanLoad8Bit)
                        {
                            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
                            texture = textures[font.PageTexPaths[chrinfo.TexturePage]];
                        }
                        else
                        {
                            texture = textures[font.PageTexPaths[chrinfo.TexturePage]];
                        }
                        GL.BindTexture(TextureTarget.Texture2D, texture);
                        GL.Begin(PrimitiveType.Quads);
                    }
                    DrawRect(quadX, quadY, quadWidth, quadHeight, new RectangleF(texCoordX, texCoordY, texCoordWidth, texCoordHeight), true);
                }
                
                if (replacedSurrogateWithMissingGlyph || utf32Char > 0xffff) i += 1; // if greater than 0xffff it was a pair. add 1 now to get right next character for kerning
                                                                                     // don't add it earlier because that would affect return value (relies on previous character being at i-1)

                // adjust next char position for kerning if needed
                // this is after our if case so it can't affect whether previous character should fit or not
                if (font.KernPairs.Count > 0 && i < str.Length - 1)
                {
                    Tuple<int, int> pair = new Tuple<int, int>(utf32Char, char.ConvertToUtf32(str, i+1)); ;
                    if (font.KernPairs.ContainsKey(pair)) newtotalwidth += ((font.KernPairs[pair].Amount) * sizescale);
                }


                totalwidth = newtotalwidth;
            }

            GL.End();
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha); // restore to original state

            totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
            return new int[] { str.Length, (int)totalwidth }; // only reached if not triggered early
        }

        // returns { NumberOfCharacters(that will fit), WidthInPixels(of characters that fit) }
        int[] GetLineLength(float height, BMFontReader.BMFont font, string str, int maxwidth = 0, float chrtracking = -2, bool breakOnWhitespaceNearEnd = true)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { 0, 0 };
            float sizescale = height / font.CommonInfo.LineHeight;

            float totalwidth = 0;

            for (int i = 0; i < str.Length; i++)
            {
                float newtotalwidth = totalwidth; // don't touch totalwidth until this iteration is done

                int utf32Char = char.ConvertToUtf32(str, i);

                if (font.Characters.ContainsKey(utf32Char)) newtotalwidth += ((font.Characters[utf32Char].XAdvance + chrtracking) * sizescale);
                else newtotalwidth += height * 1 / 2; // advance by some amount anyway, even if no character (could also draw missing character glyph if I want)

                if (maxwidth > 0 && (newtotalwidth >= maxwidth || (breakOnWhitespaceNearEnd && char.IsWhiteSpace(str, i) && newtotalwidth + height*2 >= maxwidth))) // character doesn't fit (or we can start a new line soon)
                {
                    totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
                    return new int[] { i, (int)totalwidth }; // when new character doesn't fit return index
                                                        // index is always character number - 1
                }

                if (utf32Char > 0xffff) i += 1; // if greater than 0xffff it was a pair. add 1 now to get right next character for kerning
                                                // don't add it earlier because that would affect return value (relies on previous character being at i-1)

                // adjust next char position for kerning if needed
                // this is after our if case so it can't affect whether previous character should fit or not
                if (font.KernPairs.Count > 0 && i < str.Length - 1)
                {
                    Tuple<int, int> pair = new Tuple<int, int>(utf32Char, char.ConvertToUtf32(str, i + 1)); ;
                    if (font.KernPairs.ContainsKey(pair)) newtotalwidth += ((font.KernPairs[pair].Amount) * sizescale);
                }

                totalwidth = newtotalwidth;
            }

            totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
            return new int[] { str.Length, (int)totalwidth }; // only reached if not triggered early
        }

        // returns { NumberOfCharacters(that fit), Horizontal position after last character(for adding hyphen) }
        int[] DrawCharactersAligned(float x, float y, float height, BMFontReader.BMFont font, string str, int maxwidth = 0, int align = 0, float chrtracking = -2, bool breakOnWhitespaceNearEnd = true)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { 0, (int)x };

            int rightpoint = 0;

            if (maxwidth > 0)
            {
                int[] maxchrs = GetLineLength(height, font, str, maxwidth, chrtracking, breakOnWhitespaceNearEnd);
                if (maxchrs[0] < str.Length) str = str.Remove(maxchrs[0]);

                if (align == 1)
                {
                    x += (maxwidth - maxchrs[1]) / 2;
                }
                else if (align == 2)
                {
                    x += maxwidth - maxchrs[1];
                }

                rightpoint = (int)x + maxchrs[1];
            }

            DrawCharacters(x, y, height, font, str, 0, chrtracking, breakOnWhitespaceNearEnd);

            return new int[] { str.Length, rightpoint };
        }

        // returns { NumberOfCharactersLeft(that weren't rendered), NumberOfLines(that were rendereed) }
        // punctuation added to the end of a line may extend past max width
        // smartFlow: starts new line early if there's a space close to the end, inserts hyphens when a word is broken, inserts ellipsis at end of string if not all fits in given space
        int[] DrawString(float x, float y, float height, BMFontReader.BMFont font, string str, int maxwidth = 0, int maxlines = 0, int align = 0, float chrtracking = -2, int linespacing = 1, bool smartFlow = true)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { str.Length, 0 };

            int totallines = 0;

            while (str.Contains("\n"))
            {
                string[] newlinesplit = str.Split("\n".ToCharArray(), 2); // get portion before newline to render
                int[] res = DrawString(x, y - totallines*(height+linespacing), height, font, newlinesplit[0], maxwidth, maxlines - totallines, align, chrtracking, linespacing, smartFlow);
                totallines += res[1]; // advance height
                if (res[0] > 0 || (maxlines > 0 && totallines >= maxlines)) // already can't render more...
                {
                    return new int[] { res[0] + 1 + newlinesplit[1].Length, totallines }; // +1 is for the newline character we removed
                }
                str = newlinesplit[1]; // remove already drawn content from string
            }

            int[] maxchrs = DrawCharactersAligned(x, y - totallines*(height+linespacing), height, font, str, maxwidth, align, chrtracking, smartFlow);
            while (maxchrs[0] <= str.Length)
            {
                if (maxchrs[0] == str.Length)
                {
                    str = "";
                    break;
                }

                str = str.Remove(0, maxchrs[0]);

                if (char.IsWhiteSpace(str, 0)) // remove whitespace from start of line
                {
                    if (char.IsHighSurrogate(str[0])) str = str.Remove(0, 2);
                    else str = str.Remove(0, 1);
                }
                else if (char.IsPunctuation(str, 0)) // draw punctuation attached to last word
                {
                    DrawCharacters(maxchrs[1], y - totallines*(height+linespacing), height, font, char.ConvertFromUtf32(char.ConvertToUtf32(str, 0)), 0, chrtracking);
                    if (char.IsHighSurrogate(str[0])) str = str.Remove(0, 2);
                    else str = str.Remove(0, 1);

                    if (char.IsWhiteSpace(str, 0)) // remove whitespace from start of line
                    {
                        if (char.IsHighSurrogate(str[0])) str = str.Remove(0, 2);
                        else str = str.Remove(0, 1);
                    }
                }
                else // broke mid-word
                {
                    if (maxlines > 0 && totallines + 1 >= maxlines) // if going to stop draw ellipsis instead
                    {
                        // disable because moved
                        //DrawCharacters(maxchrs[1], y - totallines*(height+linespacing), height, font, "...", 0, chrtracking);
                    }
                    else if (smartFlow)
                    {
                        DrawCharacters(maxchrs[1], y - totallines*(height+linespacing), height, font, "-", 0, chrtracking);
                    }
                }

                if (maxlines > 0 && totallines + 1 >= maxlines) // +1 because 1 will be added after
                {
                    // draw ellipsis when breaking early
                    if (smartFlow)
                    {
                        if (font.Characters.ContainsKey('…')) DrawCharacters(maxchrs[1], y - totallines * (height + linespacing), height, font, "…", 0, chrtracking);
                        else DrawCharacters(maxchrs[1], y - totallines * (height + linespacing), height, font, "...", 0, chrtracking - 1);
                    }
                    break;
                }
                totallines += 1;
                maxchrs = DrawCharactersAligned(x, y - totallines*(height+linespacing), height, font, str, maxwidth, align, chrtracking, smartFlow);
            }

            return new int[] { str.Length, totallines + 1 };
        }
    }
}