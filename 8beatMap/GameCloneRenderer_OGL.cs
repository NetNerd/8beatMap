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

        private CharaIcons.CharaIconInfo[] charaicons = new CharaIcons.CharaIconInfo[8];

        private string bgPath;
        private Size bgSize;

        private OpenTkBMFontRenderer combofontrenderer = null;

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
                OpenTkTextureLoadFuncs.UnloadTexture(tex.Value);
            }
            textures.Clear();
        }

        private void SetupTextures()
        {
            UnloadAllTextures();

            foreach (System.Collections.Generic.KeyValuePair<string, string> tex in skin.TexturePaths)
            {
                try
                {
                    if (!textures.ContainsKey(tex.Key))
                        textures.Add(tex.Key, OpenTkTextureLoadFuncs.LoadTexture(tex.Value));
                    //else
                    //{
                    //    OpenTkTextureLoadFuncs.UnloadTexture(textures[tex.Key]);
                    //    textures[tex.Key] = OpenTkTextureLoadFuncs.LoadTexture(tex.Value);
                    //}
                }
                catch
                {
                    SkinnedMessageBox.Show(skin, DialogResMgr.GetString("MissingTextureError") + "\n(" + tex.Value + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Stop();
                }
            }


            // create an empty texture for using in otherwise unloaded chara icon textures
            // this avoids needing to check for their existence in the rendering loop
            int emptytex = OpenTkTextureLoadFuncs.LoadTransparentRGBATexture();

            for (int i = 0; i < charaicons.Length; i++)
            {
                string iconkey = "spr_Charaicon" + i.ToString();
                string icontex = charaicons[i].ImagePath;

                string typestr = (charaicons[i].Type + 1).ToString();
                string raritystr = (charaicons[i].Rarity + 1).ToString();

                string frontkey = "sprCharafront" + typestr + raritystr;
                string fronttex = skin.RootDir + "/charaimg/icon" + typestr + "_rare" + raritystr + "_front.png";

                string backkey = "sprCharaback" + typestr + raritystr;
                string backtex = skin.RootDir + "/charaimg/icon" + typestr + "_rare" + raritystr + "_bg.png";
                

                if (icontex != null && icontex.Length > 0)
                {
                    try
                    {
                        if (!textures.ContainsKey(iconkey))
                            textures.Add(iconkey, OpenTkTextureLoadFuncs.LoadTexture(icontex));
                    }
                    catch
                    {
                        SkinnedMessageBox.Show(skin, DialogResMgr.GetString("MissingTextureError") + "\n(" + icontex + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        Stop();
                    }
                }
                else
                {
                    // load an empty texture if no icon is specified
                    if (!textures.ContainsKey(iconkey))
                        textures.Add(iconkey, emptytex);
                }


                try
                {
                    if (!textures.ContainsKey(frontkey))
                        textures.Add(frontkey, OpenTkTextureLoadFuncs.LoadTexture(fronttex));
                }
                catch
                {
                    SkinnedMessageBox.Show(skin, DialogResMgr.GetString("MissingTextureError") + "\n(" + fronttex + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Stop();
                }

                try
                {
                    if (!textures.ContainsKey(backkey))
                        textures.Add(backkey, OpenTkTextureLoadFuncs.LoadTexture(backtex));
                }
                catch
                {
                    SkinnedMessageBox.Show(skin, DialogResMgr.GetString("MissingTextureError") + "\n(" + backtex + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Stop();
                }
            }


            // load empty textures for all unused front and back textures
            // not necessary, so commented (can be restored later if needed)
            //for (int i = 1; i < 4; i++)
            //{
            //    string typestr = i.ToString();

            //    for (int j = 1; j < 4; j++)
            //    {
            //        string raritystr = j.ToString();

            //        string frontkey = "sprCharafront" + typestr + raritystr;
            //        string backkey = "sprCharaback" + typestr + raritystr;

            //        if (!textures.ContainsKey(frontkey))
            //            textures.Add(frontkey, emptytex);

            //        if (!textures.ContainsKey(backkey))
            //            textures.Add(backkey, emptytex);
            //    }
            //}


            string bgkey = "spr_Bg";

            if (bgPath != null && bgPath.Length > 0)
            {
                try
                {
                    if (!textures.ContainsKey(bgkey))
                    {
                        OpenTkTextureLoadFuncs.TextureWithSizeInfo tex = OpenTkTextureLoadFuncs.LoadTextureWithSizeInfo(bgPath);
                        textures.Add(bgkey, tex.TexId);
                        bgSize = tex.Size;
                    }
                }
                catch
                {
                    SkinnedMessageBox.Show(skin, DialogResMgr.GetString("MissingTextureError") + "\n(" + bgPath + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    Stop();
                }
            }
            else
            {
                // load an empty texture if no icon is specified
                if (!textures.ContainsKey(bgkey))
                {
                    textures.Add(bgkey, emptytex);
                    bgSize = new Size(0, 0);
                }
            }

        }

        public GameCloneRenderer_OGL(int wndWidth, int wndHeight, int wndX, int wndY, WindowState wndState, Form1 mainform, Skinning.Skin skin, CharaIcons.CharaIconInfo[] charaicons, bool showcombo, string bgPath)
        {
            this.mainform = mainform;
            this.skin = skin;
            this.charaicons = (CharaIcons.CharaIconInfo[])charaicons.Clone();
            this.showcombo = showcombo;
            this.bgPath = bgPath;
            numLanes = skin.NumLanes;
            NodeStartLocs = (Point[])skin.NodeStartLocs.Clone();
            NodeEndLocs = (Point[])skin.NodeEndLocs.Clone();
            SetupNodeLocs(wndWidth, wndHeight);

            System.Threading.Thread oglThread = new System.Threading.Thread(() =>
            {
                // This is roughly coded to around OpenGL v3.0 -- it's the minimum needed to get GenerateMipmap, but some functions (MatrixMode, Color4, TexCoord2, Vertex2, Begin, End) are deprecated in this version or other 3.x versions.
                // v3 was a little easier to get started on for me (someone with very little graphics experience), and it doesn't seem to have major downsides. The compatibility profile allows for all this and is well supported on desktop.
                // I'm not sure if requesting v3.0 this here actually does anything useful -- it doesn't seem to make a difference at all on nvidia drivers
                myWindow = new GameWindow(wndWidth, wndHeight, OpenTK.Graphics.GraphicsMode.Default, "8beatMap Preview Window", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, OpenTK.Graphics.GraphicsContextFlags.Default) { Icon = (Icon)IconResMgr.GetObject("Icon") };
                // The line below this is just an alternate version that enables antialiasing. It looks better, but less similar to the original game... so... I'll leave it disabled
                //myWindow = new GameWindow(wndWidth, wndHeight, new OpenTK.Graphics.GraphicsMode(32, 0, 0, 4), "8beatMap Preview Window", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, OpenTK.Graphics.GraphicsContextFlags.Default) { Icon = (Icon)IconResMgr.GetObject("Icon") };

                if ((wndX != -99999 | wndY != -99999) && wndState != WindowState.Maximized)
                {
                    myWindow.X = wndX;
                    myWindow.Y = wndY;
                }
                myWindow.WindowState = wndState;
                myWindow.VSync = OpenTK.VSyncMode.Adaptive;


                myWindow.Load += (sender, e) =>
                {
                    //Console.WriteLine(GL.GetString(StringName.Version));
                    SetupTextures();

                    try
                    {
                        combofontrenderer = new OpenTkBMFontRenderer(skin, skin.ComboTextInfo.Font);
                    }
                    catch
                    {
                        Stop();
                    }

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
                    combofontrenderer.Dispose();
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


            DrawFilledRect(0, 0, 1136, viewHeight, "spr_Bg");


            for (int i = 0; i < charaicons.Length; i++)
            {
                string key = "spr_Charaicon" + i.ToString();

                string typestr = (charaicons[i].Type + 1).ToString();
                string raritystr = (charaicons[i].Rarity + 1).ToString();

                string frontkey = "sprCharafront" + typestr + raritystr;
                string backkey = "sprCharaback" + typestr + raritystr;

                int charaiconsize = charaicons[i].IconSize;
                int halfcharaiconsize = charaiconsize / 2;


                DrawFilledRect(NodeEndLocs[i].X - halfIconSize, NodeEndLocs[i].Y - halfIconSize, iconSize, iconSize, backkey);
                DrawFilledRect(NodeEndLocs[i].X - halfcharaiconsize, NodeEndLocs[i].Y - halfcharaiconsize, charaiconsize, charaiconsize, key);
                DrawFilledRect(NodeEndLocs[i].X - halfIconSize, NodeEndLocs[i].Y - halfIconSize, iconSize, iconSize, frontkey);
            }

            //DrawFilledRect(NodeEndLocs[0].X - halfIconSize, NodeEndLocs[0].Y - halfIconSize, iconSize, iconSize, "spr_Chara1");
            //DrawFilledRect(NodeEndLocs[1].X - halfIconSize, NodeEndLocs[1].Y - halfIconSize, iconSize, iconSize, "spr_Chara2");
            //DrawFilledRect(NodeEndLocs[2].X - halfIconSize, NodeEndLocs[2].Y - halfIconSize, iconSize, iconSize, "spr_Chara3");
            //DrawFilledRect(NodeEndLocs[3].X - halfIconSize, NodeEndLocs[3].Y - halfIconSize, iconSize, iconSize, "spr_Chara4");
            //DrawFilledRect(NodeEndLocs[4].X - halfIconSize, NodeEndLocs[4].Y - halfIconSize, iconSize, iconSize, "spr_Chara5");
            //DrawFilledRect(NodeEndLocs[5].X - halfIconSize, NodeEndLocs[5].Y - halfIconSize, iconSize, iconSize, "spr_Chara6");
            //DrawFilledRect(NodeEndLocs[6].X - halfIconSize, NodeEndLocs[6].Y - halfIconSize, iconSize, iconSize, "spr_Chara7");
            //DrawFilledRect(NodeEndLocs[7].X - halfIconSize, NodeEndLocs[7].Y - halfIconSize, iconSize, iconSize, "spr_Chara8");
            


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
                        if (Type.DetectType == NoteTypes.DetectType.GbsClock)
                        {
                            // in actual game enters view at about right timing (a little ahead), slows down to be ~ 1/8 slow, then speeds up to reach normal timing
                            // icnDist is up to ~1.0, one cycle is about 6.3(2*pi) so want ~3.2 total for good effect (returns to original position for tap)
                            // subtract a little (~0.05) from icnDist for this calculation so it'll start ahead of real point as intended. use ~3.2/0.95 instead of ~3.2 because of this
                            // because allowing the icon to drift back fully by midpoint looks weird (can disappear off screen even), multiply the amount of effect by icnDist
                            // original game has the icon behind by ~1/8 note at icnDist=0.5, so use 12 as fixed multiplication... but a little less (9) looks better here...
                            // subtract to invert direction (should decrease to look delayed)
                            icnDist -= (float)Math.Sin(((double)icnDist - 0.05) * 3.36) * 9*icnDist / numTicksVisible;
                        }
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
                if (textsize == 0 && combofontrenderer.Font.CommonInfo.LineHeight > 0) textsize = combofontrenderer.Font.CommonInfo.LineHeight;
                int numX = skin.ComboTextInfo.Locs[0].X;
                int numY = viewHeight - skin.ComboTextInfo.Locs[0].Y;
                int textX = skin.ComboTextInfo.Locs[1].X;
                int textY = viewHeight - skin.ComboTextInfo.Locs[1].Y;
                combofontrenderer.DrawCharactersAligned(numX - 128, numY, textsize, comboNumber.ToString(), 256, 1, skin.ComboTextInfo.CharacterTracking);
                DrawFilledRect(textX - 128, textY, 256, 64, "spr_ComboText");
            }

            //combofontrenderer.DrawString(64, 64, 32, "01189998819991197253", 80, 1);
            //combofontrenderer.DrawCharactersAligned(64, 64, 32, "88", 160);
            //combofontrenderer.DrawCharactersAligned(64, 96, 32, "88", 160, 1);
            //combofontrenderer.DrawCharactersAligned(64, 128, 32, "88", 160, 2);
            //combofontrenderer.DrawCharactersAligned(64, 96, 32, "This is a test!---!!!@♪", 205, 0, 0);
            //DrawFilledRect(64, 64, 205, 24, "spr_HoldLocus");
            //combofontrenderer.DrawString(32, 600-32, 32, "Lor\nem ip\nsum dolor s❗it amet, consectetur adipiscing elit. Vestibulum consequ\nat sem at purus pretium, vitae mollis sapien max\nimus. Duis rutrum elit vel odio iaculis dictum. Fusce viverra nisi eget dictum facilisis. Maecenas eleifend eu lorem ut convallis. Donec sed ullamc\norper dui. Vivamus hendrerit magna vitae nisl porttitor, ac accumsan urna volutpat. Pellentesque nec nulla ultricies, suscipit arcu a, eleifend dui. Suspe\nndisse potenti. Mauris felis arcu, sollicitudin eu finibus ut, interdum id ante.", 500, 12, 0, 0, 1);
            //combofontrenderer.DrawString(600, 600-32, 32, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum consequat sem at purus pretium, vitae mollis sapien maximus. Duis rutrum elit vel odio iaculis dictum. Fusce viverra nisi eget dictum facilisis. Maecenas eleifend eu lorem ut convallis. Donec sed ullamcorper dui. Vivamus hendrerit magna vitae nisl porttitor, ac accumsan urna volutpat. Pellentesque nec nulla ultricies, suscipit arcu a, eleifend dui. Suspendisse potenti. Mauris felis arcu, sollicitudin eu finibus ut, interdum id ante.", 500, 12, 0, 0, 1, true);
            //combofontrenderer.DrawString(32, 600-32, 32, "日本語テストだ。\n多分機能する…\nチェック\nEverybody\nDance Dance Dance 纏ったもの脱ぎ捨てて\nDance Dance Dance 自分の殻つきやぶって(hey!)", 500, 12, 0, 0, -4, true);
            //DrawFilledRect(580, 202, 10, 390, "spr_HoldLocus");
            //combofontrenderer.DrawString(32, 600 - 32, 32, "Test test testtest                 ", 200, 2, 0, 0, -4, true);
            //DrawFilledRect(32, 520, 200, 10, "spr_HoldLocus");
            //GL.Color4(0f, 1f, 0, 1f);
            //combofontrenderer.DrawCharactersAligned(640, 16, 32, "😃☺😃☺😃⁉⬆", 0, 0, 0);

            FrameStopwatch.Stop();

            int refreshMs = (int)(1000 * 1f / DisplayDevice.Default.RefreshRate);

            //only render at 30fps if not playing and we would otherwise be rendering faster
            if (!mainform.IsPlaying && DisplayDevice.Default.RefreshRate > 30)
            {
                refreshMs = (int)(1000 * 1f / 30);
            }


            int sleeptime = refreshMs - (int)FrameStopwatch.ElapsedMilliseconds - 4;

            
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
            //Console.WriteLine(myWindow.RenderPeriod);
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
            DrawRect(x, y, width, height, defaultUVrect);
        }
        void DrawFilledRect(float x, float y, float width, float height, string textureName)
        {
            DrawFilledRect(x, y, width, height, textures[textureName]);
        }
    }
}