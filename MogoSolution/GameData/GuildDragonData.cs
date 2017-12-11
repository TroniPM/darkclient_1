using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class GuildDragonData : GameData<GuildDragonData>
    {
        public int guild_level {get;set;}
        public int player_level { get; set; }
        public int dragon_limit { get; set; }
        public int gold_recharge_cost { get; set; }
        public int gold_recharge_exp { get; set; }
        public int gold_recharge_money { get; set; }
        public int diamond_recharge_cost { get; set; }
        public int diamond_recharge_exp { get; set; }
        public int get_diamond_reward { get; set; }
        public int get_gold_reward { get; set; }

        static public readonly string fileName = "xml/GuildDragon";
        //static public Dictionary<int, GuildDragonData> dataMap { get; set; }
    }
}
