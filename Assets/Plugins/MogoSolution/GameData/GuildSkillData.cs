using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class GuildSkillData : GameData<GuildSkillData>
    {
        public int type {get;set;}
        public int level { get; set; }
        public int money { get; set; }
        public int add { get; set; }

        static public readonly string fileName = "xml/GuildSkill";
        //static public Dictionary<int, GuildSkillData> dataMap { get; set; }
    }
}
