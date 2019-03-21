using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static partial class BMFontReader
    {
        private static Dictionary<string, string> getTextTagParams(string line)
        {
            Dictionary<string, string> outdict = new Dictionary<string, string>();
            string[] rawparams = line.Trim().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            
            if(rawparams.Length > 0) outdict.Add("tagtype", rawparams[0]);

            // fix any mistakes in splitting
            for (int i = 1; i < rawparams.Length; i++)
            {
                string tmpstr = rawparams[i - 1].Replace("\\\"", ""); // ignore escaped quotes
                if ((tmpstr.Count(x =>  x == '"') % 2) == 1) // if there's an odd number of quotes in previous string (currently in a quote)
                {
                    rawparams[i] = rawparams[i - 1] + " " + rawparams[i]; // make both strings into one with a space separating them
                    rawparams[i - 1] = null;
                }
            }

            for (int i = 1; i < rawparams.Length; i++)
            {
                if (rawparams[i] == null || !rawparams[i].Contains("=")) continue;

                string[] paramsplit = rawparams[i].Split("=".ToCharArray(), 2);
                if (outdict.ContainsKey(paramsplit[0])) outdict[paramsplit[0]] = EscapeString(RemoveQuotes(paramsplit[1]));
                else outdict.Add(paramsplit[0], EscapeString(RemoveQuotes(paramsplit[1]))); 
            }

            return outdict;
        }

        private static System.Windows.Forms.Padding ParsePadding(string str)
        {
            string[] strsplit = str.Split(',');
            return new System.Windows.Forms.Padding(int.Parse(strsplit[3]), int.Parse(strsplit[0]), int.Parse(strsplit[1]), int.Parse(strsplit[2]));
        }

        private static System.Drawing.Point ParsePoint(string str)
        {
            string[] strsplit = str.Split(',');
            return new System.Drawing.Point(int.Parse(strsplit[0]), int.Parse(strsplit[1]));
        }

        private static string EscapeString(string str)
        {
            return str.Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("&quot;", "\""); // unescape quotes and slashes -- replacing escaped quotes before slashes will avoid making quotes after slashes in the original input remove slashes from intended output
        }

        private static string RemoveQuotes(string str)
        {
            str = str.Trim();
            if (str.First() == '"' && str.Last() == '"')
            {
                str = str.Remove(0, 1);
                str = str.Remove(str.Length - 1, 1);
            }
            return str;
        }

        private static bool IsImageOpaqueGrayscale(string path)
        {
            System.Drawing.Bitmap bmp;
            try
            {
                bmp = new System.Drawing.Bitmap(path);
            }
            catch (Exception e)
            {
                throw e;
            }

            bool foundDifference = false;

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytesPerPixel = bmpData.Stride / bmp.Width;
            if (bytesPerPixel < 4)
            {
                bmp.UnlockBits(bmpData);
                bmp.Dispose();
                return false;
            }
            
            unsafe
            {
                byte* inputData = (byte*)bmpData.Scan0;

                for (int i = 0; i < bmp.Height * bmpData.Stride; i += 4)
                {
                    if ((inputData[i] != inputData[i + 1]) // B != G
                        | (inputData[i] != inputData[i + 2]) // | B != R
                        | (inputData[i + 3] != 255)) // | A != 255
                    {
                        foundDifference = true;
                        break;
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            bmp.Dispose();

            return !foundDifference;
        }

        public struct FontGenInfo
        {
            public string FontFace;
            public int Size;
            public bool Bold;
            public bool Italic;
            public string Charset;
            public bool IsUnicode;
            public int StretchHeight;
            public bool Smoothing;
            public int SupersamplingLevel;
            public System.Windows.Forms.Padding CharPadding;
            public System.Drawing.Point CharSpacing;
            public int OutlineWidth;
        }

        public enum ChannelMode
        {
            Glyph = 0,
            Outline = 1,
            GlyphAndOutline = 2,
            Zero = 3,
            One = 4
        }

        public struct FontCommonInfo
        {
            public int LineHeight;
            public int BaseHeight;
            public int TexScaleWidth;
            public int TexScaleHeight;
            public int PageCount;
            public bool Packed;
            public ChannelMode AlphaChannelMode;
            public ChannelMode RedChannelMode;
            public ChannelMode GreenChannelMode;
            public ChannelMode BlueChannelMode;
        }

        public enum CharacterChannels
        {
            Blue = 1,
            Green = 2,
            Red = 4,
            Alpha = 8
        }

        public struct CharacterInfo
        {
            public int Id;
            public int TexCoordX;
            public int TexCoordY;
            public int Width;
            public int Height;
            public int XOffset;
            public int YOffset;
            public int XAdvance;
            public int TexturePage;
            public CharacterChannels Channels;
        }

        public struct KerningInfo
        {
            public int CharOne;
            public int CharTwo;
            public int Amount;
        }

        private static string ReadFile(string path)
        {
            return System.IO.File.ReadAllText(path);
        }
        private static string ReadFile_EmptyStringIfException(string path)
        {
            try
            {
                return ReadFile(path);
            }
            catch
            {
                Skinning.ShowUnskinnedErrorMessage("can't load file \"" + path + "\".");
                return "";
            }
        }

        public partial class BMFont
        {
            public FontGenInfo GenInfo;
            public FontCommonInfo CommonInfo;
            public Dictionary<int, CharacterInfo> Characters = new Dictionary<int, CharacterInfo>();
            public Dictionary<Tuple<int, int>, KerningInfo> KernPairs = new Dictionary<Tuple<int, int>, KerningInfo>();
            public string[] PageTexPaths = { };
            public bool CanLoad8Bit;
            public bool AreAllGylphsSingleChannel;
            private string baseDir;
            
            private void ApplyTag(Dictionary<string, string> tag)
            {
                switch (tag["tagtype"])
                {
                    case "info":
                        {
                            if (tag.ContainsKey("face"))
                                GenInfo.FontFace = tag["face"];
                            if (tag.ContainsKey("size"))
                                GenInfo.Size = int.Parse(tag["size"]);
                            if (tag.ContainsKey("bold"))
                                GenInfo.Bold = int.Parse(tag["bold"]) == 1 ? true : false;
                            if (tag.ContainsKey("italic"))
                                GenInfo.Italic = int.Parse(tag["italic"]) == 1 ? true : false;
                            if (tag.ContainsKey("charset"))
                                GenInfo.Charset = tag["charset"];
                            if (tag.ContainsKey("unicode"))
                                GenInfo.IsUnicode = int.Parse(tag["unicode"]) == 1 ? true : false;
                            if (tag.ContainsKey("stretchH"))
                                GenInfo.StretchHeight = int.Parse(tag["stretchH"]);
                            if (tag.ContainsKey("smooth"))
                                GenInfo.Smoothing = int.Parse(tag["smooth"]) == 1 ? true : false;
                            if (tag.ContainsKey("aa"))
                                GenInfo.SupersamplingLevel = int.Parse(tag["aa"]);
                            if (tag.ContainsKey("padding"))
                                GenInfo.CharPadding = ParsePadding(tag["padding"]);
                            if (tag.ContainsKey("spacing"))
                                GenInfo.CharSpacing = ParsePoint(tag["spacing"]);
                            if (tag.ContainsKey("outline"))
                                GenInfo.OutlineWidth = int.Parse(tag["outline"]);
                            break;
                        }
                    case "common":
                        {
                            if (tag.ContainsKey("lineHeight"))
                                CommonInfo.LineHeight = int.Parse(tag["lineHeight"]);
                            if (tag.ContainsKey("base"))
                                CommonInfo.BaseHeight = int.Parse(tag["base"]);
                            if (tag.ContainsKey("scaleW"))
                                CommonInfo.TexScaleWidth = int.Parse(tag["scaleW"]);
                            if (tag.ContainsKey("scaleH"))
                                CommonInfo.TexScaleHeight = int.Parse(tag["scaleH"]);
                            if (tag.ContainsKey("pages"))
                                CommonInfo.PageCount = int.Parse(tag["pages"]);
                            if (tag.ContainsKey("packed"))
                                CommonInfo.Packed = int.Parse(tag["packed"]) == 1 ? true : false;
                            if (tag.ContainsKey("alphaChnl"))
                                CommonInfo.AlphaChannelMode = (ChannelMode)int.Parse(tag["alphaChnl"]);
                            if (tag.ContainsKey("redChnl"))
                                CommonInfo.RedChannelMode = (ChannelMode)int.Parse(tag["redChnl"]);
                            if (tag.ContainsKey("greenChnl"))
                                CommonInfo.GreenChannelMode = (ChannelMode)int.Parse(tag["greenChnl"]);
                            if (tag.ContainsKey("blueChnl"))
                                CommonInfo.BlueChannelMode = (ChannelMode)int.Parse(tag["blueChnl"]);
                            break;
                        }
                    case "page":
                        {
                            int texId = int.Parse(tag["id"]);
                            if (PageTexPaths.Length <= texId) Array.Resize(ref PageTexPaths, texId + 1);
                            PageTexPaths[texId] = baseDir + "/" + tag["file"];
                            break;
                        }
                    case "char":
                        {
                            CharacterInfo thisChr = new CharacterInfo();
                            thisChr.Id = int.Parse(tag["id"]);
                            thisChr.TexCoordX = int.Parse(tag["x"]);
                            thisChr.TexCoordY = int.Parse(tag["y"]);
                            thisChr.Width = int.Parse(tag["width"]);
                            thisChr.Height = int.Parse(tag["height"]);
                            thisChr.TexturePage = int.Parse(tag["page"]);

                            if (tag.ContainsKey("xoffset"))
                                thisChr.XOffset = int.Parse(tag["xoffset"]);
                            if (tag.ContainsKey("yoffset"))
                                thisChr.YOffset = int.Parse(tag["yoffset"]);
                            if (tag.ContainsKey("xadvance"))
                                thisChr.XAdvance = int.Parse(tag["xadvance"]);
                            if (tag.ContainsKey("chnl"))
                                thisChr.Channels = (CharacterChannels)int.Parse(tag["chnl"]);

                            if (Characters.ContainsKey(thisChr.Id)) Characters[thisChr.Id] = thisChr;
                            else Characters.Add(thisChr.Id, thisChr);
                            break;
                        }

                    case "kerning":
                        {
                            KerningInfo thisKrn = new KerningInfo();
                            thisKrn.CharOne = int.Parse(tag["first"]);
                            thisKrn.CharTwo = int.Parse(tag["second"]);
                            thisKrn.Amount = int.Parse(tag["amount"]);

                            Tuple<int, int> pairkey = new Tuple<int, int>(thisKrn.CharOne, thisKrn.CharTwo);
                            if (KernPairs.ContainsKey(pairkey)) KernPairs[pairkey] = thisKrn;
                            else KernPairs.Add(pairkey, thisKrn);
                            break;
                        }
                    default:
                        break;
                }
            }

            private void LoadFromTextDef(string infoStr)
            {
                string[] infoLines = infoStr.Split("\n".ToCharArray());

                for (int i = 0; i < infoLines.Length; i++)
                {
                    string line = infoLines[i].Trim();
                    if (line.StartsWith("<") && line.EndsWith("/>"))
                    {
                        line = line.Remove(0, 1);
                        line = line.Remove(line.Length - 2, 2);
                    };
                    Dictionary<string, string> tag = getTextTagParams(line);
                    if (!tag.ContainsKey("tagtype")) continue;
                    ApplyTag(tag);
                }
            }


            public BMFont(string path, bool do8BitCheck = true)
            {
                try
                {
                    baseDir = new System.IO.FileInfo(path).Directory.FullName;
                }
                catch
                {
                    return;
                }


                try
                {
                    LoadFromBinaryDef(new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read));
                }
                catch (NotABinaryBMFontException)
                {
                    string infoStr = ReadFile_EmptyStringIfException(path);
                    LoadFromTextDef(infoStr);
                }


                if (do8BitCheck)
                {
                    if (PageTexPaths.Length > 0)
                    {
                        try
                        {
                            CanLoad8Bit = IsImageOpaqueGrayscale(PageTexPaths[0]);
                        }
                        catch
                        { }
                    }
                }
                else
                {
                    CanLoad8Bit = false; // default is already false, but being explicit doesn't hurt;
                }


                bool foundMultichannelGlyph = false;

                foreach (CharacterInfo charinfo in Characters.Values)
                {
                    if (!Enum.IsDefined(typeof(CharacterChannels), charinfo.Channels))
                    {
                        foundMultichannelGlyph = true;
                        break;
                    }
                }
                AreAllGylphsSingleChannel = !foundMultichannelGlyph;
            }
        }
    }
}
