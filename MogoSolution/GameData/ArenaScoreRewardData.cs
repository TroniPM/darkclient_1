using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class ArenaScoreRewardData : GameData
#else
    public class ArenaScoreRewardData : GameData<ArenaScoreRewardData>
#endif
	{
        public int type { get; protected set; }
        public List<int> level { get; protected set; }
        public int score { get; protected set; }
        public Dictionary<int, int> reward { get; protected set; }
        static public readonly string fileName = "xml/ArenaScoreReward";
#if UNITY_IPHONE
        static public Dictionary<int, ArenaScoreRewardData> dataMap { get; set; }
#endif
	}
}
