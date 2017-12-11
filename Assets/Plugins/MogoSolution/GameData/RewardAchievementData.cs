using Mogo.Util;
using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class RewardAchievementData : GameData<RewardAchievementData>
    {
        public int level { get; protected set; }
        public int aid { get; protected set; }
        public int type { get; protected set; }
        public List<int> args { get; protected set; }
        public int diamond { get; protected set; }
        public int title { get; protected set; }
        public int text { get; protected set; }

        public static readonly string fileName = "xml/Reward_Achievement";
        //public static Dictionary<int, RewardAchievementData> dataMap { get; set; }
    }
}
