using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static class CharaIcons
    {
        private static int maxlanes = 8;

        public struct CharaIconInfo
        {
            public string ImagePath;
            public int Type;
            public int Rarity;
            public int IconSize;

        }

        public static CharaIconInfo[] LoadCharaIconsDef(string defs)
        {
            CharaIconInfo[] outinfo = new CharaIconInfo[maxlanes];

            string[] defslines = defs.Split("\n".ToCharArray());
            for (int i = 0; i < defslines.Length && i < maxlanes; i++)
            {
                if (defslines[i].StartsWith("#") || defslines[i].Trim().Length == 0) continue;

                if (!defslines[i].Contains(",")) continue;

                string[] defsplit = defslines[i].Split(",".ToCharArray());
                outinfo[i].ImagePath = defsplit[0];
                outinfo[i].Type = int.Parse(defsplit[1]);
                outinfo[i].Rarity = int.Parse(defsplit[2]);
                outinfo[i].IconSize = int.Parse(defsplit[3]);
            }

            for (int i = 0; i < maxlanes; i++)
            {
                if (outinfo[i].IconSize <= 0)
                    outinfo[i].IconSize = 127;
            }

            return outinfo;
        }

        public static string GenCharaIconsDef(CharaIconInfo[] defs)
        {
            string outstr = "";
            
            foreach (CharaIconInfo info in defs)
            {
                outstr += info.ImagePath;
                outstr += info.Type.ToString();
                outstr += info.Rarity.ToString();
                outstr += info.IconSize.ToString();
                outstr += "\n";
            }

            return outstr;
        }
    }
}
