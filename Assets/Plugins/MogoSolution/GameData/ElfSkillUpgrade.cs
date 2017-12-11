using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ElfSkillUpgradeData : GameData<ElfSkillUpgradeData>
    {
        public int id { get; set; }
        public int nameCode { get; set; }
        public Dictionary<int,int> consume { get; set; }
        public int preSkillId { get; set; }
        public static readonly string fileName = "xml/ElfSkillUpgrade";
    }
}
