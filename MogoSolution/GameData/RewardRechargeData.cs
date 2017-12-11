using Mogo.Util;
using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class RewardRechargeData : GameData<RewardRechargeData>
    {
        public int money { get; protected set; }
        public Dictionary<int, int> items1 { get; protected set; }
        public Dictionary<int, int> items2 { get; protected set; }
        public Dictionary<int, int> items3 { get; protected set; }
        public Dictionary<int, int> items4 { get; protected set; }

        public static readonly string fileName = "xml/Reward_Recharge";
        //public static Dictionary<int, RewardRechargeData> dataMap { get; set; }
    }
}
