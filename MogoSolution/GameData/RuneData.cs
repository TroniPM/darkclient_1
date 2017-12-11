using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class RuneData : GameData<RuneData>
    {
        public int name { get; protected set; }
        public int icon { get; protected set; }
        public int type { get; protected set; }
        public int subtype { get; protected set; }
        public string desc { get; protected set; }
        public int quality { get; protected set; }
        public int level { get; protected set; }
        public int expNeed { get; protected set; }
        public int expValue { get; protected set; }
        public int price { get; protected set; }
        public int score { get; protected set; }
        public int effectID { get; protected set; }
        public int effectDesc { get; protected set; }

        public static readonly string fileName = "xml/Rune";
        //public static Dictionary<int, RuneData> dataMap { get; set; }

        //可优化为hash
        public static RuneData GetNextLvRune(int type, int subType, int quality, int level)
        {
            foreach (var i in dataMap.Values)
            {
                if (i.type == type && i.subtype == subType && i.quality == quality && i.level == level)
                {
                    return i;
                }
            }
            return null;
        }
    }
}
