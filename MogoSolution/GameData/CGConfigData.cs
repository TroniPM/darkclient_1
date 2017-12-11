using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class CGConfigData : GameData<CGConfigData>
    {
        public int quest { get; protected set; }
        static public readonly string fileName = "xml/Cg_Config";
        //static public Dictionary<int, CGConfigData> dataMap { get; set; }

    }
}
