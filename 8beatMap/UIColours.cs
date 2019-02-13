using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8beatMap
{
    public static class UIColours
    {
        [Serializable]
        public struct UIColourDef
        {
            public string TypeName;
            //public int TypeId;
        }

        public static class UIColourDefs
        {
            public static UIColourDef Chart_BG = new UIColourDef() { TypeName = "Chart_BG" };
            public static UIColourDef Chart_BarLine = new UIColourDef() { TypeName = "Chart_BarLine" };
            public static UIColourDef Chart_BarText = new UIColourDef() { TypeName = "Chart_BarText" };
            public static UIColourDef Chart_QuarterLine = new UIColourDef() { TypeName = "Chart_QuarterLine" };
            public static UIColourDef Chart_EigthLine = new UIColourDef() { TypeName = "Chart_EigthLine" };

            public static UIColourDef Form_BG = new UIColourDef() { TypeName = "Form_BG" };
        }
    }
}
