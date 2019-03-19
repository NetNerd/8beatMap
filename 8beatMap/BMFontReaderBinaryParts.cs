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
                        if (outdic.ContainsKey(field.ParamNames[i])) outdic[field.ParamNames[i]] = (fieldvalbools[i] ? "1" : "0");
                        else outdic.Add(field.ParamNames[i], (fieldvalbools[i] ? "1" : "0"));
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
                                    int iCopy = i; // otherwise when it's read by delegate it'll be incremented
                                    fields[i] = new BinaryBlockField[]
                                    {
                                        new BinaryBlockField() { ParamNames = new string[] { "id" }, Size = 0, Method = new Func<byte[], string>((x) => iCopy.ToString()) },
                                        new BinaryBlockField() { ParamNames = new string[] { "file" }, Size = nameLen, Method = ReadBinaryStringDelegate },
                                    };
                                }

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
                        }
                    }
                }
            }
        }
    }
}
