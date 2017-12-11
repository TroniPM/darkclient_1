using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class ArenaCreditReward4ChallengeData : GameData
#else
    public class ArenaCreditReward4ChallengeData : GameData<ArenaCreditReward4ChallengeData>
#endif
	{
        public int type { get; protected set; }
        public List<int> level { get; protected set; }
        public Dictionary<int, int> win { get; protected set; }
        public Dictionary<int, int> los { get; protected set; }
        static public readonly string fileName = "xml/ArenaCreditReward4Challenge";
#if UNITY_IPHONE
		static public Dictionary<int, ArenaCreditReward4ChallengeData> dataMap { get; set; }
#endif
    }
}
