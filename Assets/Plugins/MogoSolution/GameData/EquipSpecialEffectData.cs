using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class EquipSpecialEffectData : GameData<EquipSpecialEffectData>
    {
        public int name { get; protected set; }
        public int group { get; protected set; }
        public int groupName { get; protected set; }
        public int level { get; protected set; }
        public int icon { get; protected set; }
        public Dictionary<int,int> scoreList { get; protected set; }
        public int activeScore { get; protected set; }
        public int activeDesp { get; protected set; }
        public int fxid { get; protected set; }

        static public readonly string fileName = "xml/EquipSpecialEffects";
        //static public Dictionary<int, EquipSpecialEffectData> dataMap { get; set; }
    }
}
