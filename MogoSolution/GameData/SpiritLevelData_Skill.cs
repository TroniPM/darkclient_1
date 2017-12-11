using UnityEngine;
using System.Collections;
using Mogo.GameData;
using System.Collections.Generic;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class SpiritLevelData_Skill : GameData
#else
    public class SpiritLevelData_Skill : GameData<SpiritLevelData_Skill>
#endif
	{
        public int cost { get; protected set; }
        public int slot_num { get; protected set; }

        public static readonly string fileName = "xml/SpiritLevelData_Skill";
#if UNITY_IPHONE
        public static Dictionary<int, SpiritLevelData_Skill> dataMap { get; set; }
#endif
    }
}
