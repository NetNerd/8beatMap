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
            public static UIColourDef Chart_BG_Lane1 = new UIColourDef() { TypeName = "Chart_BG_Lane1" };
            public static UIColourDef Chart_BG_Lane2 = new UIColourDef() { TypeName = "Chart_BG_Lane2" };
            public static UIColourDef Chart_BG_Lane3 = new UIColourDef() { TypeName = "Chart_BG_Lane3" };
            public static UIColourDef Chart_BG_Lane4 = new UIColourDef() { TypeName = "Chart_BG_Lane4" };
            public static UIColourDef Chart_BG_Lane5 = new UIColourDef() { TypeName = "Chart_BG_Lane5" };
            public static UIColourDef Chart_BG_Lane6 = new UIColourDef() { TypeName = "Chart_BG_Lane6" };
            public static UIColourDef Chart_BG_Lane7 = new UIColourDef() { TypeName = "Chart_BG_Lane7" };
            public static UIColourDef Chart_BG_Lane8 = new UIColourDef() { TypeName = "Chart_BG_Lane8" };
            
            public static UIColourDef Chart_LaneLine = new UIColourDef() { TypeName = "Chart_LaneLine" };
            public static UIColourDef Chart_BarLine = new UIColourDef() { TypeName = "Chart_BarLine" };
            public static UIColourDef Chart_BarText = new UIColourDef() { TypeName = "Chart_BarText" };
            public static UIColourDef Chart_QuarterLine = new UIColourDef() { TypeName = "Chart_QuarterLine" };
            public static UIColourDef Chart_EigthLine = new UIColourDef() { TypeName = "Chart_EigthLine" };

            public static UIColourDef Chart_Playhead = new UIColourDef() { TypeName = "Chart_Playhead" };

            public static UIColourDef Form_BG = new UIColourDef() { TypeName = "Form_BG" };
        }
    }
}
