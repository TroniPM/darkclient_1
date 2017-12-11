using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ActivityRewardData : GameData<ActivityRewardData>
    {
        public int wave { get; set; }
        public int level { get; set; }
        public Dictionary<int, int> items { get; set; }
        public Dictionary<int, int> items2_m { get; set; }

        public static readonly string fileName = "xml/ActivityReward";
    }
}
