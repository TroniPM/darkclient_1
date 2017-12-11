using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class GuildLevelData : GameData<GuildLevelData>
    {
        public int level {get;set;}
        public int memberCount { get; set; }
        public int upgradeMoney { get; set; }
        public int skillLevelLimit { get; set; }

        static public readonly string fileName = "xml/GuildLevel";
        //static public Dictionary<int, GuildLevelData> dataMap { get; set; }
    }
}
