using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ElfSkillData : GameData<ElfSkillData>
    {
        public int id { get; set; }
        public int skillId { get; set; }
        public int nameCode { get; set; }
        public int weight { get; set; }
        public static readonly string fileName = "xml/ElfSkill";
    }
}
