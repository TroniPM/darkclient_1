using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class MissionRandomRewardData : GameData
#else
    public class MissionRandomRewardData : GameData<MissionRandomRewardData>
#endif
	{
        public int mission { get; set; }
        public int difficulty { get; set; }
        public List<int> item1 { get; set; }
        public List<int> item2 { get; set; }
        public List<int> item3 { get; set; }
        public List<int> item4 { get; set; }
        public List<int> random { get; set; }
        public List<int> num { get; set; }

        public static readonly string fileName = "xml/MissionRandomReward";
#if UNITY_IPHONE
        public static Dictionary<int, MissionRandomRewardData> dataMap { get; set; }
#endif
	}
}
