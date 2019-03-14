using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static class BMFontReader
    {
        private static Dictionary<string, string> getTagParams(string line)
        {
            Dictionary<string, string> outdict = new Dictionary<string, string>();
            string[] rawparams = line.Split(' ');
            
            outdict.Add("tagtype", rawparams[0]);

            // fix any mistakes in splitting
            for (int i = 1; i < rawparams.Length; i++)
            {
                if (!rawparams[i].Contains("="))
                {
                    rawparams[i] = rawparams[i - 1] + " " + rawparams[i];
                    rawparams[i - 1] = null;
                }
            }

            for (int i = 1; i < rawparams.Length; i++)
            {
                if (rawparams[i] == null) continue;

                string[] paramsplit = rawparams[i].Split("=".ToCharArray(), 2);
                if (outdict.ContainsKey(paramsplit[0])) outdict[paramsplit[0]] = paramsplit[1];
                else outdict.Add(paramsplit[0], paramsplit[1]); 
            }

            return outdict;
        }

        private static System.Windows.Forms.Padding ParsePadding(string str)
        {
            string[] strsplit = str.Split(',');
            return new System.Windows.Forms.Padding(int.Parse(strsplit[0]), int.Parse(strsplit[1]), int.Parse(strsplit[2]), int.Parse(strsplit[3]));
        }

        private static System.Drawing.Point ParsePoint(string str)
        {
            string[] strsplit = str.Split(',');
            return new System.Drawing.Point(int.Parse(strsplit[0]), int.Parse(strsplit[1]));
        }

        private static string RemoveQuotes(string str)
        {
            if (str.First() == '"' && str.Last() == '"')
            {
                str = str.Remove(0, 1);
                str = str.Remove(str.Length - 1, 1);
            }
            return str;
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
            public char Char
            {
                get
                {
                    return (char)Id;
                }
                set
                {
                    Id = (int)value;
                }
            }
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
            public int CharOneId;
            public char CharOne
            {
                get
                {
                    return (char)CharOneId;
                }
                set
                {
                    CharOneId = (int)value;
                }
            }
            public int CharTwoId;
            public char CharTwo
            {
                get
                {
                    return (char)CharTwoId;
                }
                set
                {
                    CharTwoId = (int)value;
                }
            }
            public int Amount;
        }

        public class BMFont
        {
            public FontGenInfo GenInfo;
            public FontCommonInfo CommonInfo;
            public Dictionary<char, CharacterInfo> Characters = new Dictionary<char, CharacterInfo>();
            public Dictionary<char, KerningInfo> KernPairs = new Dictionary<char, KerningInfo>();
            public string[] PageTexPaths = { };

            public BMFont(string path)
            {
                string baseDir = new System.IO.FileInfo(path).Directory.FullName;

                string infoStr = System.IO.File.ReadAllText(path);

                string[] infoLines = infoStr.Split("\n".ToCharArray());

                for (int i = 0; i < infoLines.Length; i++)
                {
                    Dictionary<string, string> tag = getTagParams(infoLines[i]);

                    switch(tag["tagtype"])
                    {
                        case "info":
                            {
                                if (tag.ContainsKey("face"))
                                    GenInfo.FontFace = RemoveQuotes(tag["face"]);
                                if (tag.ContainsKey("size"))
                                    GenInfo.Size = int.Parse(tag["size"]);
                                if (tag.ContainsKey("bold"))
                                    GenInfo.Bold = int.Parse(tag["bold"]) == 1 ? true : false;
                                if (tag.ContainsKey("italic"))
                                    GenInfo.Italic = int.Parse(tag["italic"]) == 1 ? true : false;
                                if (tag.ContainsKey("charset"))
                                    GenInfo.Charset = RemoveQuotes(tag["charset"]);
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
                                if (PageTexPaths.Length <= texId) Array.Resize(ref PageTexPaths, texId+1);
                                PageTexPaths[texId] = baseDir + "/" + RemoveQuotes(tag["file"]);
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

                                if (Characters.ContainsKey(thisChr.Char)) Characters[thisChr.Char] = thisChr;
                                else Characters.Add(thisChr.Char, thisChr);
                                break;
                            }

                        case "kerning":
                            {
                                KerningInfo thisKrn = new KerningInfo();
                                thisKrn.CharOneId = int.Parse(tag["first"]);
                                thisKrn.CharTwoId = int.Parse(tag["second"]);
                                thisKrn.Amount = int.Parse(tag["amount"]);

                                if (KernPairs.ContainsKey(thisKrn.CharOne)) KernPairs[thisKrn.CharOne] = thisKrn;
                                else KernPairs.Add(thisKrn.CharOne, thisKrn);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
        }
    }
}
