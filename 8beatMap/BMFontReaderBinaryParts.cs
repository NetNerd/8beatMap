using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static partial class BMFontReader
    {
        private static bool IsBinaryHeaderGood(byte[] header)
        {
            return (header.Length >= 4 && header[0] == 66 && header[1] == 77 && header[2] == 70 && header[3] == 3);
        }

        private static int ReadBinaryInt(byte[] data)
        {
            if (data.Length < 2) return data[0];
            else if (data.Length < 4) return BitConverter.ToInt16(data, 0);
            else return BitConverter.ToInt32(data, 0);
        }

        private static string ReadBinaryString(byte[] data)
        {
            data = data.TakeWhile((x) => x != 0).ToArray();
            string output = Encoding.UTF8.GetString(data);
            return output;
        }

        private static bool[] ReadBinaryFlags(byte[] data)
        {
            bool[] output = new bool[data.Length * 8];
            for (int i = 0; i < data.Length; i++)
            {
                output[i] = (data[i] & 0x80) > 0;
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

        private static string ReadBinaryPadding(byte[] data)
        {
            string output = "";
            for (int i = 0; i < data.Length; i++)
            {
                output += data[i].ToString() + ",";
            }

            return output;
        }

        private enum BinaryFieldType
        {
            Integer,
            Flags,
            String,
            Padding,
            ExternalData
        }

        private struct BinaryBlockField
        {
            public int Size;
            public BinaryFieldType Type;
            public string[] ParamNames;
            public string ExternalData;
        }

        private static Dictionary<string, string> ReadBinaryBlock(byte[] data, BinaryBlockField[] fields)
        {
            System.IO.MemoryStream datastream;
            if (data.Length > 256) datastream = new System.IO.MemoryStream(data, 0, 256, false);
            else datastream = new System.IO.MemoryStream(data, false);
            byte[] databuf;

            Dictionary<string, string> outdic = new Dictionary<string, string>();
            foreach (BinaryBlockField field in fields)
            {
                databuf = new byte[field.Size];
                datastream.Read(databuf, 0, field.Size);

                switch (field.Type)
                {
                    case BinaryFieldType.Integer:
                        {
                            if (outdic.ContainsKey(field.ParamNames[0])) outdic[field.ParamNames[0]] = ReadBinaryInt(databuf).ToString();
                            else outdic.Add(field.ParamNames[0], ReadBinaryInt(databuf).ToString());

                            break;
                        }
                    case BinaryFieldType.String:
                        {
                            if (outdic.ContainsKey(field.ParamNames[0])) outdic[field.ParamNames[0]] = ReadBinaryString(databuf);
                            else outdic.Add(field.ParamNames[0], ReadBinaryString(databuf));

                            break;
                        }
                    case BinaryFieldType.Padding:
                        {
                            if (outdic.ContainsKey(field.ParamNames[0])) outdic[field.ParamNames[0]] = ReadBinaryPadding(databuf).ToString();
                            else outdic.Add(field.ParamNames[0], ReadBinaryPadding(databuf).ToString());

                            break;
                        }
                    case BinaryFieldType.Flags:
                        {
                            bool[] fieldbools = ReadBinaryFlags(databuf);
                            for (int i = 0; i < fieldbools.Length && i < field.ParamNames.Length; i++)
                            {
                                if (outdic.ContainsKey(field.ParamNames[i])) outdic[field.ParamNames[i]] = (fieldbools[i] ? "1" : "0");
                                else outdic.Add(field.ParamNames[i], (fieldbools[i] ? "1" : "0"));
                            }

                            break;
                        }
                    case BinaryFieldType.ExternalData:
                        {
                            if (outdic.ContainsKey(field.ParamNames[0])) outdic[field.ParamNames[0]] = field.ExternalData;
                            else outdic.Add(field.ParamNames[0], field.ExternalData);

                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            return outdic;
        }

        private class NotABinaryBMFontException : Exception
        {
            new string Message;

            public NotABinaryBMFontException(string message)
            {
                this.Message = message;
            }
        }

        public partial class BMFont
        {
            private void LoadFromBinaryDef(System.IO.Stream infoBytes)
            {
                byte[] readbuf = new byte[4];
                infoBytes.Read(readbuf, 0, 4);

                if (!IsBinaryHeaderGood(readbuf)) throw new NotABinaryBMFontException("not a BMF v3 file");

                while (infoBytes.Position <= infoBytes.Length - 4)
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
                                    new BinaryBlockField() { ParamNames = new string[] {"size"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"smooth", "unicode", "italic", "bold", "fixedheight"}, Size = 1, Type = BinaryFieldType.Flags },
                                    new BinaryBlockField() { ParamNames = new string[] {"charset"}, Size = 1, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"stretchH"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"aa"}, Size = 1, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"padding"}, Size = 4, Type = BinaryFieldType.Padding },
                                    new BinaryBlockField() { ParamNames = new string[] {"spacing"}, Size = 2, Type = BinaryFieldType.Padding },
                                    new BinaryBlockField() { ParamNames = new string[] {"outline"}, Size = 1, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"face"}, Size = blocklen-14, Type = BinaryFieldType.String },
                                };
                                break;
                            }
                        case 2: // common
                            {
                                blockName = "common";
                                fields[0] = new BinaryBlockField[]
                                {
                                    new BinaryBlockField() { ParamNames = new string[] {"lineHeight"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"base"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"scaleW"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"scaleH"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"pages"}, Size = 2, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"none", "none", "none", "none", "none", "none", "none", "packed"}, Size = 1, Type = BinaryFieldType.Flags },
                                    new BinaryBlockField() { ParamNames = new string[] {"alphaChnl"}, Size = 1, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"redChnl"}, Size = 1, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"greenChnl"}, Size = 1, Type = BinaryFieldType.Integer },
                                    new BinaryBlockField() { ParamNames = new string[] {"blueChnl"}, Size = 1, Type = BinaryFieldType.Integer },
                                };
                                break;
                            }
                        case 3: // pages
                            {
                                blockName = "page";
                                int nameLen = readbuf.TakeWhile((x) => x != 0).Count() + 1;
                                int numNames = blocklen / nameLen;

                                fields = new BinaryBlockField[numNames][];

                                for (int i = 0; i < numNames; i++)
                                {
                                    fields[i] = new BinaryBlockField[]
                                    {
                                        new BinaryBlockField() { ParamNames = new string[] { "id" }, Size = 0, ExternalData = i.ToString(), Type = BinaryFieldType.ExternalData },
                                        new BinaryBlockField() { ParamNames = new string[] { "file" }, Size = nameLen, Type = BinaryFieldType.String },
                                    };
                                }

                                break;
                            }
                        case 4: // chars
                            {
                                blockName = "char";
                                int defLen = 20;
                                int numChars = blocklen / defLen;

                                fields = new BinaryBlockField[numChars][];

                                for (int i = 0; i < numChars; i++)
                                {
                                    fields[i] = new BinaryBlockField[]
                                    {
                                        new BinaryBlockField() { ParamNames = new string[] {"id"}, Size = 4, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"x"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"y"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"width"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"height"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"xoffset"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"yoffset"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"xadvance"}, Size = 2, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"page"}, Size = 1, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"chnl"}, Size = 1, Type = BinaryFieldType.Integer }
                                    };
                                }

                                break;
                            }
                        case 5: // kerning
                            {
                                blockName = "kerning";
                                int defLen = 10;
                                int numChars = blocklen / defLen;

                                fields = new BinaryBlockField[numChars][];

                                for (int i = 0; i < numChars; i++)
                                {
                                    fields[i] = new BinaryBlockField[]
                                    {
                                        new BinaryBlockField() { ParamNames = new string[] {"first"}, Size = 4, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"second"}, Size = 4, Type = BinaryFieldType.Integer },
                                        new BinaryBlockField() { ParamNames = new string[] {"amount"}, Size = 2, Type = BinaryFieldType.Integer }
                                    };
                                }

                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    
                    foreach (BinaryBlockField[] fieldset in fields)
                    {
                        if (fieldset != null)
                        {
                            Dictionary<string, string> tag = ReadBinaryBlock(readbuf, fieldset);
                            tag.Add("tagtype", blockName);
                            ApplyTag(tag);
                            
                            int skipAmount = 0;
                            foreach (BinaryBlockField field in fieldset)
                            {
                                skipAmount += field.Size;
                            }
                            readbuf = readbuf.Skip(skipAmount).ToArray();
                        }
                    }
                }
            }
        }
    }
}
