using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace _8beatMap
{
    class OpenTkBMFontRenderer : IDisposable
    {
        public Skinning.Skin ErrorSkin = Skinning.DefaultSkin;
        private BMFontReader.BMFont font;
        private System.Resources.ResourceManager DialogResMgr = new System.Resources.ResourceManager("_8beatMap.Dialogs", System.Reflection.Assembly.GetEntryAssembly());

        private System.Collections.Generic.Dictionary<string, int> textures = new System.Collections.Generic.Dictionary<string, int>();
        
        public BMFontReader.BMFont Font
        {
            get { return font; }
        }

        public OpenTkBMFontRenderer(Skinning.Skin ErrorSkin, BMFontReader.BMFont font)
        {
            this.font = font;
            this.ErrorSkin = ErrorSkin;

            LoadFontTextures();
        }

        public void Dispose()
        {
            UnloadAllTextures();
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

        private void LoadFontTextures()
        {
            for (int i = 0; i < font.PageTexPaths.Length; i++)
            {
                if (font.PageTexPaths[i] == null) continue;

                string texkey = font.PageTexPaths[i];
                string texpath = font.PageTexPaths[i];

                try
                {
                    if (font.CanLoad8Bit)
                    {
                        if (!textures.ContainsKey(texkey))
                            textures.Add(texkey, OpenTkTextureLoadFuncs.LoadTexture8BitGrayscale(texpath));
                        else
                            textures[texkey] = OpenTkTextureLoadFuncs.LoadTexture8BitGrayscale(texpath);
                    }
                    else
                    {
                        if (!textures.ContainsKey(texkey))
                            textures.Add(texkey, OpenTkTextureLoadFuncs.LoadTexture(texpath));
                        else
                            textures[texkey] = OpenTkTextureLoadFuncs.LoadTexture(texpath);
                    }

                    if (font.CommonInfo.Packed)
                    {
                        int[] channelTextures = OpenTkTextureLoadFuncs.LoadTextureToSplitChannels(texpath);

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
                catch (Exception e)
                {
                    SkinnedMessageBox.Show(ErrorSkin, DialogResMgr.GetString("MissingTextureError") + "\n(" + texpath + ")", "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    throw e;
                }
            }
        }


        private void DrawRect(float x, float y, float width, float height, RectangleF uv, bool skipBeginAndEnd = false)
        {
            if (!skipBeginAndEnd) GL.Begin(PrimitiveType.Quads);

            // Top-Left
            GL.TexCoord2(uv.Left, uv.Y - uv.Height);
            GL.Vertex2(x, y + height);

            // Top-Right
            GL.TexCoord2(uv.Right, uv.Y - uv.Height);
            GL.Vertex2(x + width, y + height);

            // Bottom-Right
            GL.TexCoord2(uv.Right, uv.Y);
            GL.Vertex2(x + width, y);

            // Bottom-Left
            GL.TexCoord2(uv.Left, uv.Y);
            GL.Vertex2(x, y);

            if (!skipBeginAndEnd) GL.End();
        }

        private static RectangleF defaultUVrect = new RectangleF(0, 1, 1, 1);
        private void DrawRect(float x, float y, float width, float height, bool skipBeginAndEnd = false)
        {
            DrawRect(x, y, width, height, defaultUVrect, skipBeginAndEnd);
        }

        private void DrawFilledRect(float x, float y, float width, float height, int texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            DrawRect(x, y, width, height, defaultUVrect);
        }
        private void DrawFilledRect(float x, float y, float width, float height, string textureName)
        {
            DrawFilledRect(x, y, width, height, textures[textureName]);
        }

        private void DrawMissingCharGlyph(float top, float bottom, float left, float right, float linewidth, bool skipBeginAndEnd = false)
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
        public int[] DrawCharacters(float x, float y, float height, string str, int maxwidth = 0, float chrtracking = -2, bool breakOnWhitespaceNearEnd = true)
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
                else if (font.Characters.ContainsKey(-1)) // check for missing character glyph in font
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


                if (maxwidth > 0 && (newtotalwidth >= maxwidth || (breakOnWhitespaceNearEnd && char.IsWhiteSpace(str, i) && newtotalwidth + height * 2 >= maxwidth))) // character doesn't fit (or we can start a new line soon)
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
                    Tuple<int, int> pair = new Tuple<int, int>(utf32Char, char.ConvertToUtf32(str, i + 1)); ;
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
        public int[] GetLineLength(float height, string str, int maxwidth = 0, float chrtracking = -2, bool breakOnWhitespaceNearEnd = true)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { 0, 0 };
            float sizescale = height / font.CommonInfo.LineHeight;

            float totalwidth = 0;

            for (int i = 0; i < str.Length; i++)
            {
                float newtotalwidth = totalwidth; // don't touch totalwidth until this iteration is done

                int utf32Char = char.ConvertToUtf32(str, i);
                
                bool replacedSurrogateWithMissingGlyph = false;
                if (font.Characters.ContainsKey(utf32Char))
                {
                    newtotalwidth += ((font.Characters[utf32Char].XAdvance + chrtracking) * sizescale);
                }
                else if (font.Characters.ContainsKey(-1)) // check for missing character glyph in font
                {
                    if (utf32Char > 0xffff) replacedSurrogateWithMissingGlyph = true; // this is needed to track position in the string properly
                                                                                      // -- we should advance by one if the next character isn't a standalone int
                    utf32Char = -1;
                    newtotalwidth += ((font.Characters[utf32Char].XAdvance + chrtracking) * sizescale);
                }
                else
                {
                    newtotalwidth += height * 1 / 2; // advance by some amount anyway, even if no character (could also draw missing character glyph if I want
                }

                if (maxwidth > 0 && (newtotalwidth >= maxwidth || (breakOnWhitespaceNearEnd && char.IsWhiteSpace(str, i) && newtotalwidth + height * 2 >= maxwidth))) // character doesn't fit (or we can start a new line soon)
                {
                    totalwidth -= chrtracking * sizescale; // because we should use the true cursor position at end, not the adjusted one for next character
                    return new int[] { i, (int)totalwidth }; // when new character doesn't fit return index
                                                             // index is always character number - 1
                }

                if (replacedSurrogateWithMissingGlyph || utf32Char > 0xffff) i += 1; // if greater than 0xffff it was a pair. add 1 now to get right next character for kerning
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
        public int[] DrawCharactersAligned(float x, float y, float height, string str, int maxwidth = 0, int align = 0, float chrtracking = -2, bool breakOnWhitespaceNearEnd = true)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { 0, (int)x };

            int rightpoint = 0;

            if (maxwidth > 0)
            {
                int[] maxchrs = GetLineLength(height, str, maxwidth, chrtracking, breakOnWhitespaceNearEnd);
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

            DrawCharacters(x, y, height, str, 0, chrtracking, breakOnWhitespaceNearEnd);

            return new int[] { str.Length, rightpoint };
        }

        // returns { NumberOfCharactersLeft(that weren't rendered), NumberOfLines(that were rendereed) }
        // punctuation added to the end of a line may extend past max width
        // smartFlow: starts new line early if there's a space close to the end, inserts hyphens when a word is broken, inserts ellipsis at end of string if not all fits in given space
        public int[] DrawString(float x, float y, float height, string str, int maxwidth = 0, int maxlines = 0, int align = 0, float chrtracking = -2, int linespacing = 1, bool smartFlow = true)
        {
            if (font.CommonInfo.LineHeight == 0) return new int[] { str.Length, 0 };

            int totallines = 0;

            while (str.Contains("\n"))
            {
                string[] newlinesplit = str.Split("\n".ToCharArray(), 2); // get portion before newline to render
                int[] res = DrawString(x, y - totallines * (height + linespacing), height, newlinesplit[0], maxwidth, maxlines - totallines, align, chrtracking, linespacing, smartFlow);
                totallines += res[1]; // advance height
                if (res[0] > 0 || (maxlines > 0 && totallines >= maxlines)) // already can't render more...
                {
                    return new int[] { res[0] + 1 + newlinesplit[1].Length, totallines }; // +1 is for the newline character we removed
                }
                str = newlinesplit[1]; // remove already drawn content from string
            }

            int[] maxchrs = DrawCharactersAligned(x, y - totallines * (height + linespacing), height, str, maxwidth, align, chrtracking, smartFlow);
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
                    while (str.Length > 0 && char.IsWhiteSpace(str, 0))
                    {
                        if (char.IsHighSurrogate(str[0])) str = str.Remove(0, 2);
                        else str = str.Remove(0, 1);
                    }
                }
                else if (char.IsPunctuation(str, 0)) // draw punctuation attached to last word
                {
                    int repsleft = 3;
                    while (repsleft > 0 && str.Length > 0 && char.IsPunctuation(str, 0))
                    {
                        repsleft--;
                        int[] newchrs = DrawCharacters(maxchrs[1], y - totallines * (height + linespacing), height, char.ConvertFromUtf32(char.ConvertToUtf32(str, 0)), 0, chrtracking);
                        maxchrs[1] += newchrs[1];
                        if (char.IsHighSurrogate(str[0])) str = str.Remove(0, 2);
                        else str = str.Remove(0, 1);
                    }

                    if (str.Length > 0 && char.IsWhiteSpace(str, 0)) // remove whitespace from start of line
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
                        DrawCharacters(maxchrs[1], y - totallines * (height + linespacing), height, "-", 0, chrtracking);
                    }
                }

                if (maxlines > 0 && totallines + 1 >= maxlines) // +1 because 1 will be added after
                {
                    // draw ellipsis when breaking early
                    if (smartFlow)
                    {
                        if (font.Characters.ContainsKey('…')) DrawCharacters(maxchrs[1], y - totallines * (height + linespacing), height, "…", 0, chrtracking);
                        else DrawCharacters(maxchrs[1], y - totallines * (height + linespacing), height, "...", 0, chrtracking - 1);
                    }
                    break;
                }
                totallines += 1;
                maxchrs = DrawCharactersAligned(x, y - totallines * (height + linespacing), height, str, maxwidth, align, chrtracking, smartFlow);
            }

            return new int[] { str.Length, totallines + 1 };
        }
    }
}
