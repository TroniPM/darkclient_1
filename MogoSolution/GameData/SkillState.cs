using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class SkillState : GameData<SkillState>
    {
        public int name { get; protected set; }
        public int direction { get; protected set; }
        public int moveAble { get; protected set; }
        public int attackAble { get; protected set; }
        public float moveSpeedRate { get; protected set; }
        public int hittable { get; protected set; }
        public int act { get; protected set; }
        public int sfx { get; protected set; }
        public int showPriority { get; protected set; }
        public int showToOther { get; protected set; }
        public int immuneShift { get; protected set; }

        static public readonly string fileName = "xml/SkillState";
        //static public Dictionary<int, SkillState> dataMap { get; set; }
    }
}
