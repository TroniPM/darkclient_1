using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class BossChestData : GameData<BossChestData>
    {
        public int mission { get; set; }
        public int difficulty { get; set; }
        public int name { get; set; }
        public int icon { get; set; }
        public Dictionary<int, int> reward { get; set; }

        public static readonly string fileName = "xml/MissionBossTreasure";
        //public static Dictionary<int, BossChestData> dataMap { get; set; }
    }
}
