using System.Collections.Generic;

namespace Mogo.GameData
{
    public class OccupyTowerTeamReward : GameData<OccupyTowerTeamReward>
    {
        public int level { get; set; }
        public int condition { get; set; }
        public Dictionary<int, int> reward { get; set; }
        public int exp { get; set; }
        public int gold { get; set; }

        static public readonly string fileName = "xml/Reward_DefecsePvP_Team";
    }
}
