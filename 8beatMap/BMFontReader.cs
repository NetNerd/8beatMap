using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static class BMFontReader
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
                if (outdict.ContainsKey(paramsplit[0])) outdict[paramsplit[0]] = paramsplit[1];
                else outdict.Add(paramsplit[0], paramsplit[1]); 
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

        private static bool IsBinaryHeaderGood(byte[] header)
        {
            return (header.Length >=4 && header[0] == 66 && header[1] == 77 && header[2] == 70 && header[3] == 3);
        }

        private static int ReadBinaryInt(byte[] data)
        {
            int output = 0;
            for (int i = 0; i < data.Length; i++)
            {
                output += data[i] << 8 * i;
            }
            return output;
        }
        private static Delegate ReadBinaryIntDelegate = new Func<byte[], int>(ReadBinaryInt);

        private static string ReadBinaryString(byte[] data)
        {
            data = data.TakeWhile((x) => x != 0).ToArray();
            string output = Encoding.UTF8.GetString(data);
            return output;
        }
        private static Delegate ReadBinaryStringDelegate = new Func<byte[], string>(ReadBinaryString);

        private static bool[] ReadBinaryFlags(byte[] data)
        {
            bool[] output = new bool[data.Length*8];
            for (int i = 0; i < data.Length; i++)
            {
                output[i] =     (data[i] & 0x80) > 0;
                output[i + 1] = (data[i] & 0x40) > 0;
                output[i + 2] = (data[i] & 0x20) > 0;
                output[i + 3] = (data[i] & 0x10) > 0;
                output[i + 4] = (data[i] & 0x08) > 0;
                output[i + 5] = (data[i] & 0x04) > 0;
                output[i + 6] = (data[i] & 0x02) > 0;
                output[i + 7] = (data[i] & 0x01) > 0;
            }
            return output;
        }
        private static Delegate ReadBinaryFlagsDelegate = new Func<byte[], bool[]>(ReadBinaryFlags);

        private static string ReadBinaryPadding(byte[] data)
        {
            string output = "";
            for (int i = 0; i < data.Length; i++)
            {
                output += data[i].ToString() + ",";
            }

            return output;
        }
        private static Delegate ReadBinaryPaddingDelegate = new Func<byte[], string>(ReadBinaryPadding);

        private struct BinaryBlockField
        {
            public int Size;
            public Delegate Method;
            public string[] ParamNames;
        }

        private static Dictionary<string, string> ReadBinaryBlock(byte[] data, BinaryBlockField[] fields)
        {
            Dictionary<string, string> outdic = new Dictionary<string, string>();
            foreach (BinaryBlockField field in fields)
            {
                var fieldval = field.Method.DynamicInvoke(new object[] { data.Take(field.Size).ToArray() });
                data = data.Skip(field.Size).ToArray();
                if (fieldval.GetType() == typeof(bool[]))
                {
                    bool[] fieldvalbools = (bool[])fieldval;
                    for (int i = 0; i < fieldvalbools.Length && i < field.ParamNames.Length; i++)
                    {
                        if (outdic.ContainsKey(field.ParamNames[i])) outdic[field.ParamNames[i]] = fieldvalbools[i].ToString();
                        else outdic.Add(field.ParamNames[i], fieldvalbools[i].ToString());
                    }
                }
                else
                {
                    if (outdic.ContainsKey(field.ParamNames[0])) outdic[field.ParamNames[0]] = fieldval.ToString();
                    else outdic.Add(field.ParamNames[0], fieldval.ToString());
                }
            }
            return outdic;
        }

        private static bool IsImageOpaqueGrayscale(string path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path);

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            byte[] dataBytes = new byte[bmp.Width * bmp.Height * 4];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, dataBytes, 0, bmp.Width * bmp.Height * 4);

            bool foundDifference = false;
            for (int i = 0; i < bmp.Height * bmp.Width * 4; i += 4)
            {
                if ((dataBytes[i] != dataBytes[i + 1]) // B != G
                    | (dataBytes[i] != dataBytes[i + 2]) // | B != R
                    | (dataBytes[i + 3] != 255)) // | A != 255
                {
                    foundDifference = true;
                    break;
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

        public class BMFont
        {
            public FontGenInfo GenInfo;
            public FontCommonInfo CommonInfo;
            public Dictionary<int, CharacterInfo> Characters = new Dictionary<int, CharacterInfo>();
            public Dictionary<Tuple<int, int>, KerningInfo> KernPairs = new Dictionary<Tuple<int, int>, KerningInfo>();
            public string[] PageTexPaths = { };
            public bool CanLoad8Bit;
            private string baseDir;
            
            private void ApplyTag(Dictionary<string, string> tag)
            {
                switch (tag["tagtype"])
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
                            if (PageTexPaths.Length <= texId) Array.Resize(ref PageTexPaths, texId + 1);
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
                    Dictionary<string, string> tag = getTextTagParams(infoLines[i]);
                    if (!tag.ContainsKey("tagtype")) continue;
                    ApplyTag(tag);
                }
            }

            private void LoadFromBinaryDef(System.IO.Stream infoBytes)
            {
                byte[] readbuf = new byte[4];
                infoBytes.Read(readbuf, 0, 4);

                if (!IsBinaryHeaderGood(readbuf)) throw new FormatException("not a BMF v3 file");

                while(infoBytes.Position <= infoBytes.Length - 4)
                {
                    readbuf = new byte[1];
                    infoBytes.Read(readbuf, 0, 1);

                    byte blockType = readbuf[0];

                    readbuf = new byte[4];
                    infoBytes.Read(readbuf, 0, 4);

                    int blocklen = ReadBinaryInt(readbuf);

                    readbuf = new byte[blocklen];
                    infoBytes.Read(readbuf, 0, blocklen);

                    // TODO: add functions for parsing blocks into Dictionary<string, string> (tag format)
                    // make everything go through the least strict format for getting same behaviour in applying tags to class properties

                    BinaryBlockField[][] fields = new BinaryBlockField[1][];
                    string blockName = null;

                    switch (blockType)
                    {
                        case 1: // info
                            {
                                blockName = "info";
                                fields[0] = new BinaryBlockField[]
                                {
                                    new BinaryBlockField() { ParamNames = new string[] {"size"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"smooth", "unicode", "italic", "bold", "fixedheight"}, Size = 1, Method = ReadBinaryFlagsDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"charset"}, Size = 1, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"stretchH"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"aa"}, Size = 1, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"padding"}, Size = 4, Method = ReadBinaryPaddingDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"spacing"}, Size = 2, Method = ReadBinaryPaddingDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"outline"}, Size = 1, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"face"}, Size = blocklen-14, Method = ReadBinaryStringDelegate },
                                };
                                break;
                            }
                        case 2: // common
                            {
                                blockName = "common";
                                fields[0] = new BinaryBlockField[]
                                {
                                    new BinaryBlockField() { ParamNames = new string[] {"lineHeight"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"base"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"scaleW"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"scaleH"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"pages"}, Size = 2, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"none", "none", "none", "none", "none", "none", "none", "packed"}, Size = 1, Method = ReadBinaryFlagsDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"alphaChnl"}, Size = 1, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"redChnl"}, Size = 1, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"greenChnl"}, Size = 1, Method = ReadBinaryIntDelegate },
                                    new BinaryBlockField() { ParamNames = new string[] {"blueChnl"}, Size = 1, Method = ReadBinaryIntDelegate },
                                };
                                break;
                            }
                        case 3: // pages
                            {
                                blockName = "page";
                                int nameLen = readbuf.TakeWhile((x) => x != 0).Count() + 1;
                                int numNames = readbuf.Length / nameLen;

                                fields = new BinaryBlockField[numNames][];

                                for (int i = 0; i < numNames; i++)
                                {
                                    fields[i] = new BinaryBlockField[]
                                    {
                                        new BinaryBlockField() { ParamNames = new string[] { "id" }, Size = 0, Method = new Func<byte[], string>((x) => i.ToString()) },
                                        new BinaryBlockField() { ParamNames = new string[] { "file" }, Size = nameLen, Method = ReadBinaryStringDelegate },
                                    };
                                }

                                break;
                            }
                    }

                    foreach (BinaryBlockField[] fieldset in fields)
                    {
                        Dictionary<string, string> tag = ReadBinaryBlock(readbuf, fieldset);
                        tag.Add("tagtype", blockName);
                        ApplyTag(tag);
                    }
                }
            }

            public BMFont(string path)
            {
                try
                {
                    baseDir = new System.IO.FileInfo(path).Directory.FullName;
                }
                catch
                {
                    return;
                }
                

                string infoStr = ReadFile_EmptyStringIfException(path);
                LoadFromTextDef(infoStr);


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
        }
    }
}
