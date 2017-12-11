using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ElfNodeData : GameData<ElfNodeData>
    {
        public int id { get; set; }
        public int nameCode { get; set; }
        public int iconId { get; set; }
        public int consume { get; set; }
        public int AwardPropCfgId { get; set; }
        public int AwardSkillPoint { get; set; }
        public static readonly string fileName = "xml/ElfNode";
    }
}
